using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using Label = System.Web.UI.WebControls.Label;

namespace ERP.UI.Web
{
    /// <summary>会员订单页面
    /// </summary>
    public partial class MemberGoodsOrderAw : BasePage
    {
        readonly IGoodsOrder _goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
        private readonly IOperationLogManager _operationLogManager =new OperationLogManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AllSourceList = CacheCollection.Filiale.GetList().ToDictionary(ent => ent.ID, ent => ent.Name);
                RCB_OrderState.DataSource = GetOrderStateList().OrderBy(w => w.Key).ToList();
                RCB_OrderState.DataBind();

                var personinfo = CurrentSession.Personnel.Get();
                var wList = WMSSao.GetWarehouseAuthDics(personinfo.PersonnelId);
                RCB_Warehouse.DataSource = wList;
                RCB_Warehouse.DataBind();


                var yearlist = new List<int>();
                for (var i = 2007; i <= (DateTime.Now.Year - GlobalConfig.KeepYear); i++)
                {
                    yearlist.Add(i);
                }
                DDL_Years.DataSource = yearlist.OrderByDescending(y => y);
                DDL_Years.DataBind();
                DDL_Years.Items.Add(new ListItem(GlobalConfig.KeepYear + "年内数据", "0"));
                DDL_Years.SelectedValue = "0";

                YearsSelectedIndex();
                BindWebSiteInfo();
            }
        }

        /// <summary>
        /// 绑定站点信息
        /// </summary>
        protected void BindWebSiteInfo()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();


            RCB_SalePlatform.DataSource = CacheCollection.SalePlatform.GetList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
        }

        /// <summary>
        /// 来源ID
        /// </summary>
        public Guid SaleFilialeId
        {
            get
            {
                if (ViewState["SaleFilialeId"] == null) return Guid.Empty;
                return new Guid(ViewState["SaleFilialeId"].ToString());
            }
            set
            {
                ViewState["SaleFilialeId"] = value.ToString();
            }
        }

        public Guid SalePlatformId
        {
            get
            {
                if (ViewState["SalePlatformId"] == null) return Guid.Empty;
                return new Guid(ViewState["SalePlatformId"].ToString());
            }
            set
            {
                ViewState["SalePlatformId"] = value.ToString();
            }
        }

        protected Dictionary<Guid, string> AllSourceList
        {
            get
            {
                if (ViewState["AllSourceList"] == null) return new Dictionary<Guid, string>();
                return (Dictionary<Guid, string>)ViewState["AllSourceList"];
            }
            set
            {
                ViewState["AllSourceList"] = value;
            }
        }

        protected IDictionary<Guid, IList<OperationLogInfo>> DicOrderClew
        {
            get
            {
                if (ViewState["DicOrderClew"] == null) return new Dictionary<Guid, IList<OperationLogInfo>>();
                return (Dictionary<Guid, IList<OperationLogInfo>>)ViewState["DicOrderClew"];
            }
            set { ViewState["DicOrderClew"] = value; }
        }

        /// <summary> 根据订单id获取MIS的管理意见
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        protected string GetMisClew(object orderId)
        {
            if (orderId != null)
            {
                var order = orderId.ToString();
                if (DicOrderClew.Count == 0) return "用户下单";
                var list = DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime);
                string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                return str;
            }
            return string.Empty;
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected OrderState NonceOrderState
        {
            get
            {
                if (ViewState["NonceOrderState"] == null) return OrderState.All;
                return (OrderState)Convert.ToInt32(ViewState["NonceOrderState"].ToString());
            }
            set
            {
                ViewState["NonceOrderState"] = ((int)value).ToString();
            }
        }

        protected Guid MemberIdOrConsignee
        {
            get
            {
                if (ViewState["MemberIdOrConsignee"] == null) return Guid.Empty;
                return (Guid)ViewState["MemberIdOrConsignee"];
            }
            set
            {
                ViewState["MemberIdOrConsignee"] = value;
            }
        }

        protected string LbTab
        {
            get
            {
                if (ViewState["LBTab"] == null) return null;
                return ViewState["LBTab"].ToString();
            }
            set
            {
                ViewState["LBTab"] = value;
            }
        }

        protected string PaidNo
        {
            get
            {
                if (ViewState["PaidNo"] == null) return string.Empty;
                return ViewState["PaidNo"].ToString();
            }
            set { ViewState["PaidNo"] = value; }
        }

        protected Dictionary<Guid,String> WarehouseAuthDics
        {
            get
            {
                if (ViewState["WarehouseAuthDics"] == null)
                {
                    var list = WarehouseManager.GetWarehouseDic();
                    ViewState["WarehouseAuthDics"] = list;
                }
                return (Dictionary<Guid, string>)ViewState["WarehouseAuthDics"];
            }
            set
            {
                ViewState["WarehouseAuthDics"] = value;
            }
        }

        protected string GetPayMode(Object payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }

        protected Dictionary<int, string> GetOrderStateList()
        {
            return (Dictionary<int, string>)EnumAttribute.GetDict<OrderState>();
        }

        protected string GetOrderState(Object orderState)
        {
            try
            {
                return EnumAttribute.GetKeyName((OrderState)orderState);
            }
            catch (Exception)
            {
                return "未知状态";
            }

        }

        protected string GetWarehouseName(Guid wareHouseId)
        {
            return WarehouseAuthDics.ContainsKey(wareHouseId) ? WarehouseAuthDics[wareHouseId] : "";
        }

        protected void Rgmgo_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsOrderInfo> goodsOrderList = new List<GoodsOrderInfo>();
            long recordCount = 0;
            var pageIndex = RGMGO.CurrentPageIndex + 1;
            var pageSize = RGMGO.PageSize;
            if (IsPostBack)
            {
                if (!string.IsNullOrEmpty(RCB_Warehouse.SelectedValue))
                {
                    var time = RDP_EndTime.SelectedDate ?? DateTime.MinValue;
                    var endTime = Convert.ToDateTime(time.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                    goodsOrderList = _goodsOrder.GetOrderList(StartTime, endTime, NonceOrderState, RTB_Search.Text,
                                                             MemberIdOrConsignee, RTB_Mobil.Text.Trim(),
                                                             RTB_RealName.Text.Trim(), new Guid(RCB_Warehouse.SelectedValue), 
                                                             RTB_ExpressNo.Text.Trim(), SaleFilialeId, SalePlatformId, GlobalConfig.KeepYear, pageIndex, pageSize, out recordCount);
                    
                }
                else
                {
                    RAM.Alert("请选择仓库!");
                    return;
                }
            }
            RGMGO.DataSource = goodsOrderList;
            RGMGO.VirtualItemCount = (int)recordCount;
            if (goodsOrderList.Count <= 0) return;
            var orderIdList = goodsOrderList.Select(w => w.OrderId).ToList();
            if (orderIdList.Count > pageSize)
            {
                orderIdList = pageIndex == 1 ? orderIdList.Take(pageSize).ToList() : orderIdList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            DicOrderClew = _operationLogManager.GetOperationLogList(orderIdList);
        }

        protected void Rgmgo_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var sourceId = new Guid(item.GetDataKeyValue("SaleFilialeId").ToString());
                var lab = (Label)item.FindControl("LbTab");
                lab.Text = AllSourceList[sourceId];
            }
        }

        protected void Ib_Search_Click(object sender, ImageClickEventArgs e)
        {
            NonceOrderState = (OrderState)Convert.ToInt32(RCB_OrderState.SelectedValue);
            StartTime = RDP_StartTime.SelectedDate ?? DateTime.MinValue;
            //EndTime = RDP_EndTime.SelectedDate != null ? RDP_EndTime.SelectedDate.Value : DateTime.MinValue;
            PaidNo = RTB_PaidNo.Text.Trim();//支付号
            var orderNo = RTB_Search.Text.Trim();//订单号
            var mobil = RTB_Mobil.Text.Trim();//手机号
            var realName = RTB_RealName.Text.Trim();//收货人
            var userName = RTB_UserName.Text.Trim();//会员名
            var expressNo = RTB_ExpressNo.Text.Trim();//快递号

            if (string.IsNullOrWhiteSpace(PaidNo) && string.IsNullOrWhiteSpace(orderNo) && string.IsNullOrWhiteSpace(mobil) && string.IsNullOrWhiteSpace(realName) && string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(expressNo))
            {
                RefreshRadTextBoxBorderColor(Color.Red);
                RAM.Alert("系统提示：必须满足一个搜索条件！");
                return;
            }
            RefreshRadTextBoxBorderColor(Color.Empty);
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            if (!string.IsNullOrEmpty(RTB_UserName.Text))
            {
                var salePlatformId = SalePlatformId;
                if (SaleFilialeId == new Guid("B6B39773-E76E-4A53-9AAC-634E7DF973EA"))//门店总公司
                {
                    salePlatformId = new Guid("3FE5AEF4-2CFD-4998-8D88-385321179B80");//可得官网
                }
                if (salePlatformId == Guid.Empty)
                {
                    RAM.Alert("温馨提示：通过会员名称搜索数据必须选择具体的销售平台，谢谢配合！");
                    return;
                }
                var info = MemberCenterSao.GetUserBase(salePlatformId, RTB_UserName.Text);
                if (info != null)
                    MemberIdOrConsignee = info.MemberId;
                else
                {
                    RAM.Alert("温馨提示：请核对会员名称是否正确！");
                    return;
                }
            }
            else
            {
                MemberIdOrConsignee = Guid.Empty;
            }
            RGMGO.Rebind();
        }

        //会员订单导出  注释
        //protected void Ib_ExportData_Click(object sender, EventArgs e)
        //{
        //    string fileName = Regex.Replace(DateTime.Now.ToString(), @"[\s\|\-\/\<>\*\?\\]", "");
        //    if (EndTime == DateTime.MinValue)
        //        fileName += "-" + WebControl.GetNowTime().ToShortDateString();
        //    else
        //        fileName += "-" + EndTime.ToShortDateString();
        //    fileName = Server.UrlEncode(fileName);
        //    RGMGO.ExportSettings.ExportOnlyData = true;
        //    RGMGO.ExportSettings.IgnorePaging = true;
        //    RGMGO.ExportSettings.FileName = fileName;
        //    RGMGO.MasterTableView.ExportToExcel();
        //}

        private void RefreshRadTextBoxBorderColor(Color color)
        {
            RTB_Search.BorderColor = color;
            RTB_RealName.BorderColor = color;
            RTB_Mobil.BorderColor = color;
            RTB_UserName.BorderColor = color;
            RTB_PaidNo.BorderColor = color;
            RTB_ExpressNo.BorderColor = color;
        }

        protected string ReturnCarriage(int orderstate, decimal carriage)
        {
            return carriage.ToString("0.##");
        }

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
        {
            YearsSelectedIndex();
        }

        private void YearsSelectedIndex()
        {
            if (DDL_Years.SelectedValue == "0")
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;

                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now.AddDays(1);

                RDP_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_EndTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_EndTime.MaxDate = DateTime.Now;

                StartTime = RDP_StartTime.SelectedDate.Value;
                //EndTime = RDP_EndTime.SelectedDate.Value;
            }
            else
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;
                RDP_StartTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_StartTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                if (RDP_StartTime.SelectedDate != null)
                    StartTime = DateTime.Parse(DDL_Years.SelectedValue + RDP_StartTime.SelectedDate.Value.ToString("-MM-dd"));
            }
        }

        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var radComboBox = o as RadComboBox;
            if (radComboBox == null) return;
            var rcbSaleFiliale = radComboBox.Parent.FindControl("RCB_SaleFiliale") as RadComboBox;
            var rcbSalePlatform = radComboBox.Parent.FindControl("RCB_SalePlatform") as RadComboBox;
            if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
            if (rcbSaleFiliale == null) return;
            var rcbSaleFilialeId = new Guid(rcbSaleFiliale.SelectedValue);
            if (rcbSaleFilialeId == Guid.Empty)
            {
                if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
                SalePlatformId = Guid.Empty;
                IsEnabledRTB_Member();
                return;
            }
            RCB_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();

            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            IsEnabledRTB_Member();
            if (SaleFilialeId == new Guid("B6B39773-E76E-4A53-9AAC-634E7DF973EA"))//门店总公司
            {
                RTB_UserName.Enabled = true;
            }
        }

        protected void RCB_SalePlatform_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            var salePlatformInfo = CacheCollection.SalePlatform.Get(SalePlatformId);
            if (salePlatformInfo != null)
            {
                RCB_SaleFiliale.SelectedValue = salePlatformInfo.FilialeId.ToString();
            }
            IsEnabledRTB_Member();
        }

        private void IsEnabledRTB_Member()
        {
            if (SalePlatformId == Guid.Empty)
            {
                RTB_UserName.Text = string.Empty;
                RTB_UserName.Enabled = false;
            }
            else
            {
                RTB_UserName.Enabled = true;
            }
        }
    }
}