using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.ApplyInvocie;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>开具发票
    /// </summary>
    public partial class DoFoudReceive : BasePage
    {
        readonly ICompanyFundReceipt _companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        readonly ICompanyInvoicePower _companyInvoicePower = new DAL.Implement.Inventory.CompanyInvoicePower(GlobalConfig.DB.FromType.Read);
        readonly PersonnelManager _personnelManager=new PersonnelManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReceiptNo = string.Empty;
                Status = 0;
                List<int> status=new List<int> {(int)CompanyFundReceiptState.WaitInvoice };
                List<int> otherStatus = new List<int> { (int)CompanyFundReceiptState.Audited };
                IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetFundReceiptInfoList(status, otherStatus, (int)ApplyInvoiceState.Finished, StartTime, EndTime, "", (int)CompanyFundReceiptType.Receive);
                TotalAmount = list.Sum(ent => ent.RealityBalance);
                RG_CheckInfo.DataSource = list;
            }
        }

        protected void RG_CheckInfo_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            List<int> status = new List<int> { (int)CompanyFundReceiptState.WaitInvoice };
            List<int> otherStatus = new List<int> { (int)CompanyFundReceiptState.Audited };
            int state = (int)ApplyInvoiceState.Finished;
            if (Status!=0)
            {
                switch (Status)
                {
                    case 10: //待开票
                        state = 0;
                        status=new List<int>();
                        otherStatus = new List<int> { (int)CompanyFundReceiptState.WaitInvoice };
                        break;
                    case 20: //开票中
                        status = new List<int>();
                        otherStatus = new List<int> { (int)CompanyFundReceiptState.WaitInvoice };
                        state = (int)ApplyInvoiceState.Invoicing;
                        break;
                    case 30: //已开
                        status = new List<int>();
                        break;
                    default:
                        status=new List<int> { Status };
                        break;
                }
            }
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetFundReceiptInfoList(status, otherStatus, state, StartTime, EndTime, ReceiptNo, (int)CompanyFundReceiptType.Receive);
            TotalAmount = list.Sum(ent => ent.RealityBalance);
            RG_CheckInfo.DataSource = list;
        }

        protected void RG_CheckInfo_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                var ddlStatus = e.Item.FindControl("DDL_CheckState") as DropDownList;
                var rdpstart = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var rdpend = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var tboxNo = e.Item.FindControl("TB_CompanyFundReciptNO") as TextBox;
                if (rdpstart != null) if (rdpstart.SelectedDate != null) StartTime = rdpstart.SelectedDate.Value;
                if (rdpend != null)
                    if (rdpend.SelectedDate != null)
                        EndTime = Convert.ToDateTime(rdpend.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                if (ddlStatus != null) Status = int.Parse(ddlStatus.SelectedValue);
                if (tboxNo != null) ReceiptNo = tboxNo.Text;
                RG_CheckInfo.CurrentPageIndex = 0;
                RG_CheckInfo.Rebind();
            }
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                if (e.CommandName == "DoFoundReceipt")
                {
                    if (isdo.Value == "false") return;
                    var dataItem = item;
                    try
                    {
                        var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                        var receiptNo = dataItem.GetDataKeyValue("ReceiptNo").ToString();
                        string remark = WebControl.RetrunUserAndTime("开具发票,审核");
                        BLL.Implement.Inventory.CompanyFundReceipt.WriteInstance.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Audited, remark, CurrentSession.Personnel.Get().PersonnelId);
                        //往来收付款开具发票增加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptId, receiptNo,
                            OperationPoint.CurrentReceivedPayment.MakeInvoice.GetBusinessInfo(), string.Empty);
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("开具发票失败" + ex.Message);
                        return;
                    }
                    RG_CheckInfo.Rebind();
                }
            }
        }

        //绑定所有状态
        protected Dictionary<int, string> BindStatusDataBound()
        {
            return new Dictionary<int, string>
            {
                {0,"全部"},{10,"待开票"},{20,"开票中"},{30,"已开票"}
            };
        }

        #region 属性

        protected DateTime StartTime
        {
            set { ViewState["StartTime"] = value; }
            get
            {
                if (ViewState["StartTime"] == null)
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(ViewState["StartTime"].ToString());
            }
        }

        protected DateTime EndTime
        {
            set { ViewState["EndTime"] = value; }
            get
            {
                if (ViewState["EndTime"] == null)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.Parse(ViewState["EndTime"].ToString());
            }
        }

        protected string ReceiptNo
        {
            set { ViewState["ReceiptNo"] = value; }
            get
            {
                return ViewState["ReceiptNo"].ToString();
            }
        }

        protected int Status
        {
            set { ViewState["Status"] = value; }
            get
            {
                return (int)ViewState["Status"];
            }
        }

        public decimal TotalAmount
        {
            set { ViewState["TotalAmount"] = value; }
            get
            {
                return (decimal)ViewState["TotalAmount"];
            }
        }

        #endregion

        #region 显示文字方法

        protected string GetCompName(string compId)
        {
            var list = RelatedCompany.Instance.ToList();
            if (list == null)
                return "-";
            var info = list.FirstOrDefault(o => o.CompanyId == new Guid(compId));
            if (info == null)
            {
                var filialeName = FilialeManager.GetName(new Guid(compId));
                return string.IsNullOrEmpty(filialeName) ? "-" : filialeName;
            }  
            return info.CompanyName;
        }

        protected string GetPersonName(string personId)
        {
            return _personnelManager.GetName(new Guid(personId));
        }

        protected string GetReceiptStatus(object receiptStatus,object invoiceState)
        {
            var status = Convert.ToInt32(receiptStatus);
            var state = Convert.ToInt32(invoiceState);
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
            if (status==(int)CompanyFundReceiptState.WaitInvoice)
            {
                if (state > 0)
                    return EnumAttribute.GetKeyName((ApplyInvoiceState) invoiceState);
                return stateDic[status];
            }
            if (status == (int) CompanyFundReceiptState.Audited && state > 0)
                return "已开";
            return stateDic.ContainsKey(status)? stateDic[status]:"未知状态";
        }

        protected bool IsShow(string status, string companyID)
        {
            var personnelInfo = CurrentSession.Personnel.Get();
            var branchInfo = CacheCollection.Branch.Get(personnelInfo.FilialeId, personnelInfo.BranchId);
            var personnelBranchId = branchInfo == null
                ? personnelInfo.BranchId
                : branchInfo.ParentBranchId != Guid.Empty
                    ? branchInfo.ParentBranchId
                    : personnelInfo.BranchId;
            // &&  _companyInvoicePower.GetALLCompanyInvoicePower().Any(o => o.InvoicesType == (int)CompanyFundReceiptInvoiceType.OpenInvoice
                    //&& o.CompanyID.ToString() == companyID &&
                    //(o.AuditorID == personnelInfo.PersonnelId ||
                    //(o.FilialeID == personnelInfo.FilialeId &&
                    //o.BranchID == personnelBranchId &&
                    //o.PositionID == personnelInfo.PositionId)))
            if (status == string.Format("{0}", (int)CompanyFundReceiptState.WaitInvoice))
                return true;
            return false;
        }

        #endregion

        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
        }

        protected void RG_CheckInfo_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var startTime = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var endTime = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                if (StartTime != DateTime.MinValue)
                {
                    if (startTime != null) startTime.SelectedDate = StartTime;
                }
                if (EndTime != DateTime.MaxValue)
                {
                    if (endTime != null) endTime.SelectedDate = EndTime;
                }
            }
        }

        /// <summary>
        /// 显示公司
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        protected string GetFilialeName(string filialeId)
        {
            var info = SaleFilialeList.FirstOrDefault(act => act.ID.ToString() == filialeId);
            if (info != null) return info.Name;
            if (filialeId != Guid.Empty.ToString()) return "ERP";
            return "-";
        }

        /// <summary>
        /// 公司列表
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }
    }
}
