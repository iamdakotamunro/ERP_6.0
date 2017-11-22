using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class PurchaseDistributionsFrom : WindowsPage
    {

        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定责任人
                RCB_Persion.DataSource = PersonnelList;
                RCB_Persion.DataBind();
                //绑定供应商
                RCB_Company.DataSource = CompanyCussentList;
                RCB_Company.DataBind();
                //绑定仓库
                RCB_Warehouse.DataSource = CurrentSession.Personnel.WarehouseList;
                RCB_Warehouse.DataBind();
                //绑定转移责任人
                RCB_TargetPersion.DataSource = PersonnelList;
                RCB_TargetPersion.DataBind();
                //绑定转移供应商
                RCB_TargetCompany.DataSource = CompanyCussentList;
                RCB_TargetCompany.DataBind();
            }

        }

        #region [自定义]
        /// <summary>
        /// 获取(采购部)所有员工的信息
        /// </summary>
        /// <returns></returns>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }

        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> CompanyCussentList
        {
            get
            {
                if (ViewState["CompanyCussentList"] == null)
                {
                    ViewState["CompanyCussentList"] = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers);
                }
                return (IList<CompanyCussentInfo>)ViewState["CompanyCussentList"];
            }
        }

        #endregion

        protected void RCB_Company_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<CompanyCussentInfo> companyList = CompanyCussentList.Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= companyList.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (CompanyCussentInfo i in companyList)
                    {
                        var item = new RadComboBoxItem
                                                   {
                                                       Text = i.CompanyName,
                                                       Value = i.CompanyId.ToString()
                                                   };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_PurchaseSet, e);
        }

        protected void RgPurchaseSet_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<PurchaseSetInfo> list = _purchaseSet.GetPurchaseSetList();
            string companyId = RCB_Company.SelectedValue;
            string warehouseId = RCB_Warehouse.SelectedValue;
            string personId = RCB_Persion.SelectedValue;
            if (!string.IsNullOrEmpty(companyId) && companyId != Guid.Empty.ToString())
            {
                list = list.Where(w => w.CompanyId == new Guid(companyId)).ToList();
            }
            if (!string.IsNullOrEmpty(warehouseId))
            {
                list = list.Where(w => w.WarehouseId == new Guid(warehouseId)).ToList();
            }
            if (!string.IsNullOrEmpty(personId))
            {
                list = list.Where(w => w.PersonResponsible == new Guid(personId)).ToList();
            }
            RG_PurchaseSet.DataSource = list;
        }

        /// <summary>获取供应商名称
        /// </summary>
        protected string GetCompanyName(Guid companyId)
        {
            string companyName = "-";
            var info = CompanyCussentList.FirstOrDefault(w => w.CompanyId == companyId);
            if (info != null)
            {
                companyName = info.CompanyName;
            }
            return companyName;
        }

        protected void Btn_Search_OnClick(object sender, EventArgs e)
        {
            RG_PurchaseSet.Rebind();
        }

        protected void Btn_Save_OnClick(object sender, EventArgs e)
        {
            var count = RG_PurchaseSet.SelectedItems.Count;
            if (count == 0)
            {
                RAM.Alert("请选择要转移的商品!");
                return;
            }
            if (string.IsNullOrEmpty(RCB_TargetPersion.SelectedValue) && new Guid(RCB_TargetCompany.SelectedValue) == Guid.Empty)
            {
                RAM.Alert("请选择你要转移的责任人或供应商！");
                return;
            }

            var goodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in RG_PurchaseSet.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (goodsIdList.Count(w => w == goodsId) == 0)
                    goodsIdList.Add(goodsId);
            }
            if (goodsIdList.Count > 0)
            {
                IList<GoodsInfo> goodsList = new GoodsCenterSao().GetGoodsListByGoodsIds(goodsIdList);
                if (goodsList.Count > 0)
                {
                    IList<PurchaseSetInfo> purchaseSetList = _purchaseSet.GetPurchaseSetList(goodsIdList, Guid.Empty);
                    foreach (var goodsInfo in goodsList)
                    {
                        var purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsInfo.GoodsId);
                        if (purchaseSetInfo == null) continue;
                        if (!string.IsNullOrEmpty(RCB_TargetPersion.SelectedValue))
                        {
                            purchaseSetInfo.PersonResponsible = new Guid(RCB_TargetPersion.SelectedValue);
                        }
                        if (new Guid(RCB_TargetCompany.SelectedValue) != Guid.Empty)
                        {
                            purchaseSetInfo.CompanyId = new Guid(RCB_TargetCompany.SelectedValue);
                        }
                        string errorMessage;
                        purchaseSetInfo.PurchaseGroupId = Guid.Empty;
                        _purchaseSet.UpdatePurchaseSet(purchaseSetInfo, out errorMessage);
                        //报备管理已分配管理操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, purchaseSetInfo.GoodsId, goodsInfo.GoodsCode,
                            OperationPoint.ReportManage.DistributionManager.GetBusinessInfo(), string.Empty);
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }

        protected void Btn_Save_OnClick_Company(object sender, EventArgs e)
        {
            var oldCompanyId = new Guid(RCB_Company.SelectedValue);
            var newCompanyId = new Guid(RCB_TargetCompany.SelectedValue);
            if (oldCompanyId == Guid.Empty || newCompanyId == Guid.Empty)
            {
                RAM.Alert("非法操作！");
                return;
            }
            if (oldCompanyId == newCompanyId)
            {
                RAM.Alert("同一个供应商无需转移");
                return;
            }
            try
            {
                _purchaseSet.BatchTransferCompany(oldCompanyId, newCompanyId);
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception)
            {
                RAM.Alert("批量转移商品至供应商失败!");
            }
        }
    }
}
