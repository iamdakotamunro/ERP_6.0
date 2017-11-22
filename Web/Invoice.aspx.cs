using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace LonmeShop.AdminWeb
{
    /// <summary>发票管理
    /// </summary>
    public partial class InvoiceAw : BasePage
    {
        private readonly IGoodsOrder _goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
        private readonly SynchronousData sync = new SynchronousData();
        //private readonly Invoice _invoice = new Invoice();

        #region [Properties 自定义属性]
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToShortDateString() + " 00:00:00");
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["StartTime"]).ToShortDateString() + " 00:00:00");
            }
            set
            {
                ViewState["StartTime"] = value.ToString();
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null || Convert.ToDateTime(ViewState["EndTime"]) == DateTime.MinValue) return DateTime.Now;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndTime"] = value.ToString();
            }
        }

        protected string CancelPersonel
        {
            get
            {
                if (ViewState["CancelPersonel"] == null) return string.Empty;
                return ViewState["CancelPersonel"].ToString();
            }
            set
            {
                ViewState["CancelPersonel"] = value;
            }
        }

        protected InvoiceState CurrentInvoiceState
        {
            get
            {
                if (ViewState["InvoiceState"] == null) return InvoiceState.Request;
                return (InvoiceState)ViewState["InvoiceState"];
            }
            set
            {
                ViewState["InvoiceState"] = value;
            }
        }

        protected int CurrentInvoiceType
        {
            get
            {
                if (ViewState["InvoiceType"] == null) return -1;
                return (int)ViewState["InvoiceType"];
            }
            set
            {
                ViewState["InvoiceType"] = value;
            }
        }

        protected Boolean IsAfterSearch
        {
            get
            {
                if (ViewState["IsAfterSearch"] == null) return false;
                return (Boolean)ViewState["IsAfterSearch"];
            }
            set
            {
                ViewState["IsAfterSearch"] = value;
            }
        }

        protected Boolean IsAfterSearchType
        {
            get
            {
                if (ViewState["IsAfterSearchType"] == null) return false;
                return (Boolean)ViewState["IsAfterSearchType"];
            }
            set
            {
                ViewState["IsAfterSearchType"] = value;
            }
        }

        protected Boolean IsOrderComplete
        {
            get
            {
                if (ViewState["IsOrderComplete"] == null) return false;
                return (Boolean)ViewState["IsOrderComplete"];
            }
            set
            {
                ViewState["IsOrderComplete"] = value;
            }
        }

        /// <summary>
        /// 是否需要手动打印
        /// </summary>
        protected Boolean IsNeedManual
        {
            get
            {
                if (ViewState["IsNeedManual"] == null) return false;
                return (Boolean)ViewState["IsNeedManual"];
            }
            set
            {
                ViewState["IsNeedManual"] = value;
            }
        }

        /// <summary>
        /// 订单来源
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] =
                        CacheCollection.Filiale.GetList()
                                       .Where(act => act.Rank == (int)FilialeRank.Head && act.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }

        protected string SelectSaleFilialeId
        {
            get
            {
                return ViewState["FromSource"] == null ? string.Empty : ViewState["FromSource"].ToString();
            }
            set
            {
                ViewState["FromSource"] = value;
            }
        }

        protected Guid SelectWarehouse
        {
            get
            {
                return ViewState["SelectWarehouse"] == null ? Guid.Empty : new Guid(ViewState["SelectWarehouse"].ToString());
            }
            set
            {
                ViewState["SelectWarehouse"] = value;
            }
        }

        //订单号
        protected string OrderNo
        {
            get
            {
                if (ViewState["OrderNo"] == null) return string.Empty;
                return ViewState["OrderNo"].ToString();
            }
            set
            {
                ViewState["OrderNo"] = value;
            }
        }

        //发票抬头
        protected string InvoiceName
        {
            get
            {
                if (ViewState["InvoiceName"] == null) return string.Empty;
                return ViewState["InvoiceName"].ToString();
            }
            set
            {
                ViewState["InvoiceName"] = value;
            }
        }

        //发票号
        protected string InvoiceNo
        {
            get
            {
                if (ViewState["InvoiceNo"] == null) return string.Empty;
                return ViewState["InvoiceNo"].ToString();
            }
            set
            {
                ViewState["InvoiceNo"] = value;
            }
        }

        //发票地址
        protected string Address
        {
            get
            {
                if (ViewState["Address"] == null) return string.Empty;
                return ViewState["Address"].ToString();
            }
            set
            {
                ViewState["Address"] = value;
            }
        }

        //发票名称
        protected string InvoiceContent
        {
            get
            {
                if (ViewState["InvoiceContent"] == null) return string.Empty;
                return ViewState["InvoiceContent"].ToString();
            }
            set
            {
                ViewState["InvoiceContent"] = value;
            }
        }

        protected Dictionary<Guid,String> AuthWarehouses
        {
            get
            {
                if (ViewState["AuthWarehouses"] == null) return new Dictionary<Guid, string>();
                return (Dictionary<Guid, String>)ViewState["AuthWarehouses"];
            }
            set
            {
                ViewState["AuthWarehouses"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var personnelInfo = CurrentSession.Personnel.Get();
                //获取授权仓库列表
                AuthWarehouses = WMSSao.GetWarehouseAuthDics(personnelInfo.PersonnelId);
                RDP_StartTime.SelectedDate=DateTime.Now.AddMonths(-1);
                RDP_EndTime.SelectedDate = DateTime.Now;
            }
        }

        protected void RGInvoice_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (AuthWarehouses.Count == 0)
            {
                RAM.Alert("没有授权的仓库，无法查询！");
                return;
            }
            var list = Invoice.ReadInstance.GetInvoiceList(StartTime, EndTime, IsOrderComplete, OrderNo, InvoiceName, InvoiceNo, Address, InvoiceContent, CurrentInvoiceState, IsNeedManual, 
                SelectWarehouse!= Guid.Empty?new List<Guid> { SelectWarehouse }:AuthWarehouses.Keys.ToList(),CurrentInvoiceType,string.IsNullOrEmpty(SelectSaleFilialeId)?Guid.Empty:new Guid(SelectSaleFilialeId), CancelPersonel);
            RGInvoice.DataSource = list;
            RGInvoice.VirtualItemCount = list.Count;
            IsAfterSearch = true;
            IsAfterSearchType = true;
            // End modify
        }

        protected void RGInvoice_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                var invoiceId = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                var enumInvoiceState = (InvoiceState)int.Parse(dataItem.GetDataKeyValue("InvoiceState").ToString());
                //var invoice = new Invoice();
                if (e.CommandName == "Approved")
                {
                    try
                    {
                        if (enumInvoiceState == InvoiceState.Request)
                        {
                            //invoice.SetInvoiceState(invoiceId, InvoiceState.Success, WebControl.RetrunUserAndTime("受理发票"), CurrentSession.Personnel.Get().RealName);
                            Invoice.WriteInstance.SetInvoiceState(invoiceId, InvoiceState.Success, CurrentSession.Personnel.Get().RealName);
                            IList<GoodsOrderInfo> goodOrderInfoList =invoiceId!=Guid.Empty?_goodsOrder.GetInvoiceGoodsOrderList(invoiceId):new List<GoodsOrderInfo>();
                            foreach (GoodsOrderInfo goodsOrderInfo in goodOrderInfoList)
                            {
                                sync.SyncSetInvoiceState(goodsOrderInfo.SaleFilialeId, invoiceId, InvoiceState.Success, true, WebControl.RetrunUserAndTime("受理发票"));
                                break;
                            }
                            //发票管理受理操作记录添加
                            var invoiceInfo = Invoice.ReadInstance.GetInvoice(invoiceId);
                            var personnelInfo = CurrentSession.Personnel.Get();
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceId, invoiceInfo == null ? "" : invoiceInfo.InvoiceNo.ToMendString(8),
                                OperationPoint.InvoiceManage.Approved.GetBusinessInfo(), string.Empty);
                        }
                        RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    catch
                    {

                        RAM.Alert("更改发票状态失败！");
                    }
                }
                else if (e.CommandName == "Cancel")
                {
                    try
                    {
                        //invoice.SetInvoiceState(invoiceId, InvoiceState.Cancel, WebControl.RetrunUserAndTime("取消发票"), CurrentSession.Personnel.Get().RealName);
                        Invoice.WriteInstance.SetInvoiceState(invoiceId, InvoiceState.Cancel, CurrentSession.Personnel.Get().RealName);
                        IList<GoodsOrderInfo> goodOrderInfoList = invoiceId != Guid.Empty ? _goodsOrder.GetInvoiceGoodsOrderList(invoiceId) : new List<GoodsOrderInfo>();
                        foreach (GoodsOrderInfo goodsOrderInfo in goodOrderInfoList)
                        {
                            sync.SyncSetInvoiceState(goodsOrderInfo.SaleFilialeId, invoiceId, InvoiceState.Cancel, true,WebControl.RetrunUserAndTime("取消发票"));
                            break;
                        }

                        //发票管理取消操作记录添加
                        var invoiceInfo = Invoice.ReadInstance.GetInvoice(invoiceId);
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceId, invoiceInfo == null ? "" : invoiceInfo.InvoiceNo.ToMendString(8),
                            OperationPoint.InvoiceManage.Cancel.GetBusinessInfo(), string.Empty);
                        RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    catch
                    {
                        RAM.Alert("更改发票状态失败！");
                    }
                }
                else if ("waste" == e.CommandName)
                {
                    //var success = invoice.SetInvoiceState(invoiceId, InvoiceState.Waste, "该发票财务确认作废.[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]", CurrentSession.Personnel.Get().RealName);
                    var success = Invoice.WriteInstance.SetInvoiceState(invoiceId, InvoiceState.Waste, CurrentSession.Personnel.Get().RealName);
                    if (success)
                    {
                        IList<GoodsOrderInfo> goodOrderInfoList = invoiceId != Guid.Empty ? _goodsOrder.GetInvoiceGoodsOrderList(invoiceId) : new List<GoodsOrderInfo>();
                        foreach (GoodsOrderInfo goodsOrderInfo in goodOrderInfoList)
                        {
                            sync.SyncSetInvoiceState(goodsOrderInfo.SaleFilialeId, invoiceId, InvoiceState.Waste, true,"该发票财务确认作废.[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]");
                            break;
                        }

                        RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    else
                    {
                        RAM.Alert("发票拒绝作废，有可能发票已经报送税务！");
                    }
                }
                else if ("return" == e.CommandName)
                {
                    //invoice.SetInvoiceState(invoiceId, InvoiceState.Success, "该发票财务拒绝作废.状态返回已开[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]", CurrentSession.Personnel.Get().RealName);
                    Invoice.WriteInstance.SetInvoiceState(invoiceId, InvoiceState.Success, CurrentSession.Personnel.Get().RealName);
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }

        protected void RCBInvoiceState_Init(object sender, EventArgs e)
        {
            var invoiceState = (RadComboBox)sender;
            if (!Page.IsPostBack)
            {
                invoiceState.SelectedIndex = 1;
            }
        }

        protected void RCBFromSource_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var rcb = (RadComboBox)sender;
                foreach (var info in SaleFilialeList)
                {
                    rcb.Items.Add(new RadComboBoxItem { Text = info.Name, Value = info.ID.ToString() });
                }
            }
        }

        protected string FromSourceMessage(Guid saleFilialeId)
        {
            var fromSource = "";
            foreach (var items in SaleFilialeList.Where(items => items.ID == saleFilialeId))
            {
                fromSource = items.Name;
            }
            return fromSource;
        }

        protected void tbSearch_Load(object sender, EventArgs e)
        {
            var tbSearch = (TextBox)sender;
            tbSearch.Text = "订单号/发票号/开票地址";
            var search = hfSearch.Value.Trim();
            if (string.IsNullOrEmpty(search))
            {
                Color myColor = Color.FromName("#CCC");
                tbSearch.ForeColor = myColor;
            }
            else
            {
                tbSearch.ForeColor = Color.Black;
            }
        }

        protected string GetNoteType(object state, object noteType)
        {
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Retreat)
            {
                return "退票";
            }
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Abolish)
            {
                return "废票";
            }
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Effective)
            {
                return "正票";
            }
            return "-";
        }

        protected void RCBWarehouse_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var rcb = (RadComboBox)sender;
                foreach (var info in AuthWarehouses)
                {
                    rcb.Items.Add(new RadComboBoxItem { Text = info.Value, Value = info.Key.ToString() });
                }
            }
        }

        protected void RadGridAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            AjaxRequest(RGInvoice, e);
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            if (AuthWarehouses.Count == 0)
            {
                RAM.Alert("没有授权的仓库，无法查询！");
                return;
            }
            SelectWarehouse = new Guid(RCB_Warehouse.SelectedValue);
            StartTime = RDP_StartTime.SelectedDate == null ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(RDP_StartTime.SelectedDate);
            EndTime = RDP_EndTime.SelectedDate == null ? DateTime.Now : Convert.ToDateTime(RDP_EndTime.SelectedDate);
            CancelPersonel = TB_CancelPersonel.Text.Trim();
            IsOrderComplete = CB_IsOrderComplete.Checked;
            IsNeedManual = CB_IsNeedManual.Checked;
            OrderNo = TB_OrderNo.Text.Trim();
            InvoiceName = TB_InvoiceName.Text.Trim();
            InvoiceNo = TB_InvoiceNo.Text.Trim();
            Address = TB_Address.Text.Trim();
            InvoiceContent = TB_InvoiceContent.Text.Trim();
            CurrentInvoiceState = (InvoiceState)Convert.ToInt32(rcbInvoiceState.SelectedValue);
            CurrentInvoiceType = Convert.ToInt32(rcbInvoiceType.SelectedValue);
            SelectSaleFilialeId = RCB_SaleFiliale.SelectedValue;
            RGInvoice.CurrentPageIndex = 0;
            if (rcbInvoiceState.SelectedValue != "-1" && rcbInvoiceState.SelectedValue != "4" && CancelPersonel != "")
            {
                RAM.Alert("非作废发票不存在作废申请人!");
                return;
            }
            IsAfterSearch = true;
            RGInvoice.Rebind();
        }
    }
}