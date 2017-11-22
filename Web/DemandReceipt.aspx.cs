using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
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
    /// <summary>索取发票
    /// </summary>
    public partial class DemandReceipt : BasePage
    {
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly IPersonnelSao _personnelManager=new PersonnelSao();
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        readonly ICompanyInvoicePower _companyInvoicePower=new DAL.Implement.Inventory.CompanyInvoicePower(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReceiptNo = string.Empty;
                Status = CompanyFundReceiptState.NoHandle;
                IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty,ReceiptPage.DemandReceipt, CompanyFundReceiptState.GettingInvoice, StartTime, EndTime, "", CompanyFundReceiptType.Payment);
                List<Guid> companyList =_companyCussent.GetCompanyCussentList(State.Enable).Where(c => c.IsNeedInvoices).Select(c => c.CompanyId).ToList();
                var newList = list.Where(c => companyList.Contains(c.CompanyID)).ToList();
                ShowFooterText(newList);
                if (newList.Count > 0)
                {
                    BankIds = newList.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
                }
                RG_CheckInfo.DataSource = newList;
            }
        }

        protected void RgCheckInfoNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime endTime = EndTime != DateTime.MaxValue ? Convert.ToDateTime(EndTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00")) : EndTime;
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty,ReceiptPage.DemandReceipt, Status, StartTime, endTime, ReceiptNo, CompanyFundReceiptType.Payment);
            if (SelectSaleFilialeId != Guid.Empty.ToString())
            {
                list = list.Where(act => act.FilialeId.ToString() == SelectSaleFilialeId).ToList();
            }
            if (SelectBankAccountId != Guid.Empty.ToString())
            {
                list = list.Where(act => act.PayBankAccountsId.ToString() == SelectBankAccountId).ToList();
            }
            List<Guid> companyList = _companyCussent.GetCompanyCussentList(State.Enable).Where(c => c.IsNeedInvoices).Select(c => c.CompanyId).ToList();
            IList<CompanyFundReceiptInfo> newlist = list.Where(c => companyList.Contains(c.CompanyID)).ToList();

            #region FooterText 统计显示

            ShowFooterText(newlist);

            #endregion

            if (newlist.Count > 0)
            {
                BankIds = newlist.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
            }
            RG_CheckInfo.DataSource = newlist;
        }

        protected void RgCheckInfoItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                var ddlStatus = e.Item.FindControl("DDL_CheckState") as DropDownList;
                var rdpstart = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var rdpend = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var tboxNo = e.Item.FindControl("TB_CompanyFundReciptNO") as TextBox;
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
                if (rdpstart != null) if (rdpstart.SelectedDate != null) StartTime = rdpstart.SelectedDate.Value;
                if (rdpend != null)
                    if (rdpend.SelectedDate != null)
                        EndTime = Convert.ToDateTime(rdpend.SelectedDate.Value);
                if (ddlStatus != null) Status = (CompanyFundReceiptState)int.Parse(ddlStatus.SelectedValue);
                if (tboxNo != null) ReceiptNo = tboxNo.Text;
                RG_CheckInfo.CurrentPageIndex = 0;
                RG_CheckInfo.Rebind();
            }
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var companyFUndReceiptBll = new BLL.Implement.Inventory.CompanyFundReceipt(_companyFundReceipt);
                if (e.CommandName == "DemandReceipt")
                {
                    if (isdo.Value == "false") return;
                    var dataItem = item;
                    try
                    {
                        var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                        var receiptNo = dataItem.GetDataKeyValue("ReceiptNo").ToString();
                        string remark = WebControl.RetrunUserAndTime("索取发票,待认证");
                        companyFUndReceiptBll.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.WaitAttestation, remark, CurrentSession.Personnel.Get().PersonnelId);
                        ////往来收付款确定已收增加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptId, receiptNo,
                            OperationPoint.CurrentReceivedPayment.InvoiceReceived.GetBusinessInfo(), string.Empty);
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("索取发票失败" + ex.Message);
                        return;
                    }
                    RG_CheckInfo.Rebind();
                }
                if (e.CommandName == "OperateAttestation")
                {
                    if (isdo.Value == "false") return;
                    var dataItem = item;
                    try
                    {
                        var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                        string remark = WebControl.RetrunUserAndTime("发票已认证");
                        var personnel = CurrentSession.Personnel.Get();
                        var result = companyFUndReceiptBll.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.FinishAttestation, remark, personnel.PersonnelId);
                        if (result > 0)
                        {
                            RAM.Alert("系统提示：发票认证成功！");
                        }
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("系统提示：发票认证操作异常，详细异常信息：" + ex.Message);
                        return;
                    }
                    RG_CheckInfo.Rebind();
                }
            }
        }

        //绑定所有状态
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

        private void ShowFooterText(IList<CompanyFundReceiptInfo> list)
        {
            //待索取发票合计显示列（申请金额）
            var realityBalanceColumn = RG_CheckInfo.MasterTableView.Columns.FindByUniqueName("RealityBalance");
            //待认证发票合计显示列（打款日期）
            var finishDateColumn = RG_CheckInfo.MasterTableView.Columns.FindByUniqueName("FinishDate");

            if (list.Count > 0)
            {
                //发票待索取合计
                var sum1 = list.Where(ent => ent.ReceiptStatus == (int)CompanyFundReceiptState.GettingInvoice).Sum(
                        ent => ent.RealityBalance);
                //发票待认证合计
                var sum2 = list.Where(ent => ent.ReceiptStatus == (int)CompanyFundReceiptState.WaitAttestation).Sum(
                        ent => ent.RealityBalance);
                realityBalanceColumn.FooterText = "待索取发票合计：" + WebControl.NumberSeparator(sum1);
                finishDateColumn.FooterText = "待认证发票合计：" + WebControl.NumberSeparator(sum2);
            }
            else
            {
                realityBalanceColumn.FooterText = "待索取发票合计：0.00";
                finishDateColumn.FooterText = "待认证发票合计：0.00";
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

        protected CompanyFundReceiptState Status
        {
            set { ViewState["Status"] = value; }
            get
            {
                return (CompanyFundReceiptState)ViewState["Status"];
            }
        }

        /// <summary>
        /// 加载销售公司
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbFromSourceLoad(object sender, EventArgs e)
        {
            var rcb = (RadComboBox)sender;
            if (!IsPostBack)
            {
                foreach (var info in SaleFilialeList)
                {
                    rcb.Items.Add(new RadComboBoxItem { Text = info.Name, Value = info.ID.ToString() });
                }
            }
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
            return _personnelManager.GetName(new Guid(personId));
        }

        protected string GetReceiptStatus(string receiptStatus)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (receiptStatus == string.Format("{0}", kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知状态";
        }

        protected bool IsShow(string status, string companyId)
        {
            if (status == (string.Format("{0}", (int)CompanyFundReceiptState.GettingInvoice)) && _companyInvoicePower.GetALLCompanyInvoicePower().Any(o => o.InvoicesType == (int)CompanyFundReceiptInvoiceType.CollectInvoice && o.CompanyID.ToString() == companyId && (o.AuditorID == CurrentSession.Personnel.Get().PersonnelId || (o.FilialeID == CurrentSession.Personnel.Get().FilialeId && o.BranchID == CurrentSession.Personnel.Get().BranchId && o.PositionID == CurrentSession.Personnel.Get().PositionId))))
                return true;
            return false;
        }

        /// <summary>是否可以认证
        /// </summary>
        /// <param name="status"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        protected bool IsShowAttestation(string status, string companyId)
        {
            var personnel = CurrentSession.Personnel.Get();
            if (status == (string.Format("{0}", (int)CompanyFundReceiptState.WaitAttestation)) && _companyInvoicePower.GetALLCompanyInvoicePower().Any(o => o.InvoicesType == (int)CompanyFundReceiptInvoiceType.CollectInvoice && o.CompanyID.ToString() == companyId && (o.AuditorID == personnel.PersonnelId || (o.FilialeID == personnel.FilialeId && o.BranchID == personnel.BranchId && o.PositionID == personnel.PositionId))))
                return true;
            return false;
        }

        #region[获取往来单位信息]
        /// <summary>
        /// 获取往来单位信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        protected string GetCompany(Guid companyId, int type)
        {
            var info = _companyCussent.GetCompanyCussent(companyId);
            if (info == null) return "";
            if (type == 1)
                return info.Linkman;
            return info.Mobile;
        }
        #endregion

        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
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
                    foreach (var bankInfo in BankIds.Distinct().Select(id => BankAccountManager.ReadInstance.GetBankAccounts(id)).Where(bankInfo => bankInfo.IsUse))
                    {
                        ddlBank.Items.Add(new ListItem(bankInfo.BankName + "-" + bankInfo.AccountsName, bankInfo.BankAccountsId.ToString()));
                    }
                }
                ddlBank.Items.Insert(0, new ListItem("全部", Guid.Empty.ToString()));
                ddlBank.SelectedValue = Guid.Empty.ToString();
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
