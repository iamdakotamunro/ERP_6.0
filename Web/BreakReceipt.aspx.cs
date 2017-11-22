using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>
    /// add by dinghq 2011-06-14
    /// </summary>
    public partial class BreakReceipt : BasePage
    {
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly IPersonnelSao _personnelManager=new PersonnelSao();
        private readonly ICompanyFundReceipt _companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                State = CompanyFundReceiptState.NoHandle;
                InvoicesDemander = Guid.Empty;
                ReceiptNo = string.Empty;
                IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty, ReceiptPage.Else, State, StartTime, EndTime, string.Empty, CompanyFundReceiptType.All);
                if (list.Count > 0)
                {
                    BankIds = list.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
                }
                RG_CheckInfo.DataSource = list;

            }
        }

        protected void RG_CheckInfo_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<CompanyFundReceiptInfo> list = InvoicesDemander != Guid.Empty ? _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty,ReceiptPage.Else, State, StartTime, EndTime, ReceiptNo, CompanyFundReceiptType.Payment, InvoicesDemander)
                : _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty,ReceiptPage.Else, State, StartTime, EndTime, ReceiptNo, CompanyFundReceiptType.Payment);
            if (SelectSaleFilialeId != Guid.Empty.ToString())
            {
                list = list.Where(act => act.FilialeId.ToString() == SelectSaleFilialeId).ToList();
            }
            if (SelectBankAccountId != Guid.Empty.ToString())
            {
                list = list.Where(ent => ent.PayBankAccountsId == new Guid(SelectBankAccountId)).ToList();
            }
            if (list.Count > 0)
            {
                BankIds = list.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
            }
            RG_CheckInfo.DataSource = list;
        }

        // ReSharper disable once FunctionComplexityOverflow
        protected void RG_CheckInfo_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Verification")
            {
                if (RG_CheckInfo.SelectedItems.Count == 0)
                {
                    RAM.Alert("请选择核销数据!");
                    return;
                }
                if (verification.Value == "false") return;
                var builder = new StringBuilder("");
                var companyFundReceiptBll=new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                foreach (GridDataItem item in RG_CheckInfo.SelectedItems)
                {
                    var receiptId = new Guid(item.GetDataKeyValue("ReceiptID").ToString());
                    var receiptNo = item.GetDataKeyValue("ReceiptNo").ToString();
                    try
                    {
                        string remark = WebControl.RetrunUserAndTime("核销发票转入完成状态");
                        companyFundReceiptBll.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish, remark);
                        //往来收付款核销发票增加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptId, receiptNo,
                             OperationPoint.CurrentReceivedPayment.InvoiceVerification.GetBusinessInfo(), string.Empty);
                    }
                    catch (Exception)
                    {
                        builder.AppendLine(receiptNo);
                    }
                }
                if (builder.Length > 0)
                {
                    RAM.Alert(builder + "核销失败!");
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            if (e.CommandName == "Search")
            {
                var txtManager = (RadTextBox)e.Item.FindControl("RTB_InvioceManager");
                var rdpstart = (RadDatePicker)e.Item.FindControl("RDP_StartTime");
                var rdpend = (RadDatePicker)e.Item.FindControl("RDP_EndTime");
                var tboxNo = (TextBox)e.Item.FindControl("TB_CompanyFundReciptNO");
                var rcbState = e.Item.FindControl("DDL_CheckState") as DropDownList;
                var saleFilialeId = e.Item.FindControl("DdlSaleFiliale") as DropDownList;
                var bankAccountId = e.Item.FindControl("DdlBankAccount") as DropDownList;
                if (saleFilialeId != null)
                {
                    SelectSaleFilialeId = saleFilialeId.SelectedValue;
                }
                if (bankAccountId != null)
                {
                    SelectBankAccountId = bankAccountId.SelectedValue;
                }
                if (rcbState != null) State = (CompanyFundReceiptState)Convert.ToInt32(rcbState.SelectedValue);
                if (txtManager.Text != "")
                {
                    List<PersonnelInfo> infoList = _personnelManager.GetList().Where(o => o.RealName.Contains(txtManager.Text)).ToList();
                    if (infoList.Count == 0)
                    {
                        RAM.Alert("该人不存在");
                        return;
                    }
                    InvoicesDemander = infoList[0].PersonnelId;
                }
                else
                    InvoicesDemander = Guid.Empty;
                if (rdpstart != null) if (rdpstart.SelectedDate != null) StartTime = rdpstart.SelectedDate.Value;
                if (rdpend != null)
                    if (rdpend.SelectedDate != null)
                        EndTime = Convert.ToDateTime(rdpend.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                ReceiptNo = tboxNo.Text;
                RG_CheckInfo.CurrentPageIndex = 0;
                RG_CheckInfo.Rebind();
            }
            var gridDataItem = e.Item as GridDataItem;
            if (gridDataItem != null)
            {
                if (e.CommandName == "DemandReceipt")
                {
                    if (isdo.Value == "false") return;
                    var dataItem = gridDataItem;
                    try
                    {
                        var companyFundReceiptBll = new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                        var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                        var receiptNo = dataItem.GetDataKeyValue("ReceiptNo").ToString();
                        string remark = WebControl.RetrunUserAndTime("核销发票转入完成状态");
                        companyFundReceiptBll.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish, remark);
                        //往来收付款核销发票增加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptId, receiptNo,
                             OperationPoint.CurrentReceivedPayment.InvoiceVerification.GetBusinessInfo(), string.Empty);
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("索取发票失败" + ex.Message);
                        return;
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }

        //绑定所有状态
        protected void BindStatusDataBound(object sender, EventArgs e)
        {
            var ddlCheckState = (DropDownList)sender;
            if (ddlCheckState != null)
            {
                var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
                foreach (KeyValuePair<int, string> kvp in stateDic)
                {
                    var item = new ListItem(kvp.Value, string.Format("{0}",kvp.Key));
                    ddlCheckState.Items.Add(item);
                }
            }
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

        protected Guid InvoicesDemander
        {
            set { ViewState["InvoicesDemander"] = value; }
            get
            {
                return new Guid(ViewState["InvoicesDemander"].ToString());
            }
        }

        protected CompanyFundReceiptState State
        {
            get { return (CompanyFundReceiptState)(ViewState["State"]); }
            set { ViewState["State"] = value; }
        }

        /// <summary>
        /// 销售公司
        /// </summary>
        protected string SelectSaleFilialeId
        {
            get
            {
                return ViewState["SaleFilialeId"] == null ? Guid.Empty.ToString() : ViewState["SaleFilialeId"].ToString();
            }
            set
            {
                ViewState["SaleFilialeId"] = value;
            }
        }

        /// <summary>
        /// 付款银行
        /// </summary>
        protected string SelectBankAccountId
        {
            get
            {
                return ViewState["SelectBankAccountId"] == null ? Guid.Empty.ToString() : ViewState["SelectBankAccountId"].ToString();
            }
            set
            {
                ViewState["SelectBankAccountId"] = value;
            }
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
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetHeadList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }

        protected List<Guid> BankIds
        {
            set { ViewState["BankIds"] = value; }
            get
            {
                if (ViewState["BankIds"] == null)
                {
                    return new List<Guid>();
                }
                return (List<Guid>)ViewState["BankIds"];
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
            return info == null ? "-" : info.CompanyName;
        }

        protected string GetPersonName(string personId)
        {
            var info = _personnelManager.Get(new Guid(personId));
            return info == null ? "-" : info.RealName;
        }

        protected string GetReceiptStatus(string receiptStatus)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (receiptStatus == string.Format("{0}",kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知状态";
        }

        protected bool IsShow(string status)
        {
            if (status == string.Format("{0}",((int)CompanyFundReceiptState.FinishAttestation)))
                return true;
            return false;
        }

        protected string GetReceiptTypeName(string type)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptType>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (type == string.Format("{0}",kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知类型";
        }

        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
        }

        protected Dictionary<int, string> BindStatusDataBound()
        {
            var newDic = new Dictionary<int, string>();
            try
            {
                var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
                foreach (var status in stateDic)
                {
                    if (status.Key == -2 || status.Key == -3)
                    {
                        newDic.Add(status.Key, status.Value);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return newDic;
        }

        /// <summary>
        /// 显示公司
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        protected string GetFilialeName(string filialeId)
        {
            var info = SaleFilialeList.FirstOrDefault(
                act => act.ID.ToString() == filialeId);
            if (info != null) return info.Name;
            if (filialeId != Guid.Empty.ToString()) return "ERP";
            return "-";
        }

        /// <summary>
        /// 绑定公司
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindFilialeDataBound()
        {
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), string.Empty } };
            foreach (var info in SaleFilialeList)
            {
                newDic.Add(info.ID.ToString(), info.Name);
            }
            newDic.Add(_reckoningElseFilialeid.ToString(), "ERP");
            return newDic;
        }

        /// <summary>
        /// 绑定银行
        /// </summary>
        /// <returns></returns>
        protected void BindBankDataBound(object sender, EventArgs eventArgs)
        {
            var ddlBank = sender as DropDownList;
            if (ddlBank != null)
            {
                ddlBank.Items.Clear();
                if (BankIds.Count > 0)
                {
                    foreach (var bankInfo in BankIds.Distinct().Select(id => _bankAccounts.GetBankAccounts(id)).Where(bankInfo => bankInfo.IsUse))
                    {
                        ddlBank.Items.Add(new ListItem(bankInfo.BankName + "-" + bankInfo.AccountsName, bankInfo.BankAccountsId.ToString()));
                    }
                }
                ddlBank.Items.Insert(0, new ListItem("全部", Guid.Empty.ToString()));
                ddlBank.SelectedValue = Guid.Empty.ToString();
            }
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
    }
}
