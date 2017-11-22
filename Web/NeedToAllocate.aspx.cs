using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using PurchasingManagement = ERP.BLL.Implement.Inventory.PurchasingManagement;

namespace ERP.UI.Web
{
    /// <summary>缺货商品统计
    /// </summary>
    public partial class NeedToAllocate : BasePage
    {
        private static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        readonly PurchasingManagement _purchasingManagement = new PurchasingManagement( _personnelSao, new GoodsCenterSao());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RDP_StartTime.SelectedDate = Convert.ToDateTime(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd 00:00:00.000"));
                RDP_EndTime.SelectedDate = DateTime.Now;
                var personinfo = CurrentSession.Personnel.Get();
                var items = _purchaseSet.GetPersonList();
                foreach (var item in items)
                {
                    item.PersonResponsibleName = _personnelSao.GetName(item.PersonResponsible);
                }
                RCB_Pm.DataSource = items;
                RCB_Pm.DataTextField = "PersonResponsibleName";
                RCB_Pm.DataValueField = "PersonResponsible";
                RCB_Pm.DataBind();
                if (items.FirstOrDefault(ent => ent.PersonResponsible == personinfo.PersonnelId) != null)
                {
                    RCB_Pm.SelectedValue = personinfo.PersonnelId.ToString();
                }
                RCB_Pm.Items.Insert(0, new RadComboBoxItem("负责人列表", Guid.Empty.ToString()));
                var wList = WMSSao.GetWarehouseAuthDics(personinfo.PersonnelId);
                RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("授权仓库列表", Guid.Empty.ToString()));
                RCB_Warehouse.DataSource = wList;
                RCB_Warehouse.DataBind();
                if (wList.Count == 1)
                {
                    RCB_Warehouse.SelectedValue = wList.First().Key.ToString();
                }
            }
        }

        #region[公用属性]
        /// <summary>
        /// 开始时间
        /// </summary>
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["startTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["startTime"].ToString());
            }
            set
            {
                ViewState["startTime"] = value;
            }
        }
        /// <summary>
        /// 完成时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                if (ViewState["endTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["endTime"].ToString());
            }
            set
            {
                ViewState["endTime"] = value;
            }
        }
        #endregion

        protected void Ib_Search_Click(object sender, ImageClickEventArgs e)
        {
            BindData();
            rgd_need.Rebind();
        }

        protected void BindData()
        {
            IList<GoodsAllocateStatistic> list=new List<GoodsAllocateStatistic>();
            if (IsPostBack)
            {
                var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
                if (warehouseId == default(Guid))
                {
                    RAM.Alert("请选择授权仓库，谢谢！");
                    return;
                }
                var personResponsible = new Guid(RCB_Pm.SelectedValue);
                if (personResponsible == default(Guid))
                {
                    RAM.Alert("请选择责任人，谢谢！");
                    return;
                }
                StartTime = RDP_StartTime.SelectedDate == null? Convert.ToDateTime(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd 00:00:00.000")): Convert.ToDateTime(RDP_StartTime.SelectedDate.Value.ToString("yyyy-MM-dd 00:00:00.000"));
                EndTime = RDP_EndTime.SelectedDate == null? DateTime.Now: Convert.ToDateTime(RDP_EndTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
                 list =_purchasingManagement.GetNeedeGoodsOrderList(warehouseId, RCB_Warehouse.SelectedItem.Text, personResponsible, StartTime, EndTime);
                rgd_need.DataSource = list.OrderByDescending(p => p.OrderCount);
            }
            rgd_need.DataSource = list;
        }

        protected void Rgd_Need_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            BindData();
        }
    }
}
