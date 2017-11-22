using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.DAL.Interface.ICompany;

namespace ERP.UI.Web
{
    /// <summary>申请打款
    /// </summary>
    public partial class DoReceivePay : BasePage
    {
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        readonly ICompanyBankAccountBind _companyBankAccountBindBll = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReceiptNo = string.Empty;
                Type = CompanyFundReceiptType.All;
                if (Request["type"] != null)
                {
                    if (Request["type"] == string.Format("{0}",(int)CompanyFundReceiptType.Payment))
                        Type = CompanyFundReceiptType.Payment;
                    if (Request["type"] == string.Format("{0}",(int)CompanyFundReceiptType.Receive))
                        Type = CompanyFundReceiptType.Receive;
                }
                Status = CompanyFundReceiptState.NoHandle;
            }
        }

        #region  门店公司信息列表

        protected IList<FilialeInfo> ShopFilialeList
        {
            get
            {
                return CacheCollection.Filiale.GetShopList();
            }
        }
        #endregion

        protected void RgCheckInfoNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(new Guid(SelectSaleFilialeId), ReceiptPage.DoReceivePay, Status, 
                StartTime, EndTime, ReceiptNo, Type).ToList();
            if (BankId != Guid.Empty)
            {
                list = list.Where(ent => ent.PayBankAccountsId == BankId).ToList();
            }
            if (SExecuteTime != DateTime.MinValue)
            {
                list = list.Where(c => c.ExecuteDateTime >= SExecuteTime).ToList();
            }
            if (EExecuteTime != DateTime.MaxValue)
            {
                list = list.Where(c => c.ExecuteDateTime < EExecuteTime).ToList();
            }
            if (list.Count != 0)
                CompanyFundReceiptsList = list;
            if (list.Count > 0)
            {
                BankIds = list.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
            }
            //合计金额
            var sum = RG_CheckInfo.MasterTableView.Columns.FindByUniqueName("RealityBalance");
            if (list.Count > 0)
            {
                var realityBalanceSum = list.Sum(ent => Math.Abs(ent.RealityBalance));
                sum.FooterText = string.Format("合计：{0}", WebControl.NumberSeparator(realityBalanceSum));
            }
            else
            {
                sum.FooterText = string.Empty;
            }
            RG_CheckInfo.DataSource = list;
        }

        protected void RgCheckInfoItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                var ddlStatus = e.Item.FindControl("DDL_CheckState") as DropDownList;
                var rdpstart = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var rdpend = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var tboxNo = e.Item.FindControl("TB_CompanyFundReciptNO") as TextBox;
                var ddlType = e.Item.FindControl("DDL_ReceivePayType") as DropDownList;
                var ddlBank = e.Item.FindControl("DDL_Bank") as DropDownList;
                var rdpSExecuteTime = e.Item.FindControl("RDP_SExecuteTime") as RadDatePicker;
                var rdpEExecuteTime = e.Item.FindControl("RDP_EExecuteTime") as RadDatePicker;
                var filialeId = e.Item.FindControl("DdlSaleFiliale") as DropDownList;
                if (filialeId != null && !string.IsNullOrEmpty(filialeId.SelectedValue))
                    SelectSaleFilialeId = filialeId.SelectedValue;

                #region[申请时间段]
                if (rdpstart != null) if (rdpstart.SelectedDate != null) StartTime = rdpstart.SelectedDate.Value;
                if (rdpend != null)
                    if (rdpend.SelectedDate != null)
                        EndTime = Convert.ToDateTime(rdpend.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                #endregion

                #region[打款时间段]
                if (rdpSExecuteTime != null) if (rdpSExecuteTime.SelectedDate != null) SExecuteTime = rdpSExecuteTime.SelectedDate.Value;
                if (rdpEExecuteTime != null)
                    if (rdpEExecuteTime.SelectedDate != null)
                        EExecuteTime = Convert.ToDateTime(rdpEExecuteTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                #endregion
                if (ddlType != null) Type = (CompanyFundReceiptType)int.Parse(ddlType.SelectedValue);
                if (ddlStatus != null) Status = (CompanyFundReceiptState)int.Parse(ddlStatus.SelectedValue);
                if (tboxNo != null) ReceiptNo = tboxNo.Text;
                if (ddlBank != null) BankId = new Guid(ddlBank.SelectedValue);
                RG_CheckInfo.CurrentPageIndex = 0;
                RG_CheckInfo.Rebind();
            }
            else if (e.CommandName=="AllDo")
            {
                int num = 0;
                foreach (GridDataItem dataItem in RG_CheckInfo.Items)
                {
                    var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                    if (!cbCheck.Checked)
                    {
                        continue;
                    }
                    var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                    string remark = WebControl.RetrunUserAndTime("执行");
                    var receiptInfo = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId);
                    if (receiptInfo == null || receiptInfo.ReceiptStatus >= (int)CompanyFundReceiptState.Executed
                         || string.IsNullOrEmpty(receiptInfo.DealFlowNo) || receiptInfo.PayBankAccountsId==Guid.Empty)
                        continue;
                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        //付款
                        if (receiptInfo.ReceiptType == (int)CompanyFundReceiptType.Payment)
                        {
                            var accountCount = BankAccountManager.ReadInstance.GetBankAccountsNonce(receiptInfo.PayBankAccountsId);
                            if (accountCount <= 0)
                            {
                                RAM.Alert("帐户余额为0，无法付款！");
                                return;
                            }
                            if (Convert.ToDecimal(accountCount) < receiptInfo.RealityBalance + receiptInfo.Poundage)
                            {
                                RAM.Alert("帐户余额为不足，无法付款！");
                                return;
                            }
                            _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                            _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Executed);
                            _companyFundReceipt.SetDateTime(receiptId, 2);
                            if (receiptInfo.Poundage!=0)
                            {
                                _companyFundReceipt.UpdatePoundage(receiptId, receiptInfo.Poundage);
                            }
                        }
                        //收款
                        if (receiptInfo.ReceiptType == (int)CompanyFundReceiptType.Receive)
                        {
                            _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                            _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Executed);
                            _companyFundReceipt.SetDateTime(receiptId, 2);
                            if (receiptInfo.Poundage != 0)
                            {
                                _companyFundReceipt.UpdatePoundage(receiptId, receiptInfo.Poundage);
                            }
                        }
                        ts.Complete();
                    }
                    num++;
                }
                if(num==0)
                {
                    RAM.Alert("请选择");
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        #region[绑定所有状态]
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
        #endregion

        protected Dictionary<int, string> BindTypeDataBound()
        {
            return (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptType>();
        }

        #region 属性

        protected Guid BankId
        {
            set { ViewState["BankId"] = value; }
            get
            {
                if (ViewState["BankId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["BankId"].ToString());
            }
        }

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

        protected DateTime SExecuteTime
        {
            set { ViewState["SExecuteTime"] = value; }
            get
            {
                if (ViewState["SExecuteTime"] == null)
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(ViewState["SExecuteTime"].ToString());
            }
        }

        protected DateTime EExecuteTime
        {
            set { ViewState["EExecuteTime"] = value; }
            get
            {
                if (ViewState["EExecuteTime"] == null)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.Parse(ViewState["EExecuteTime"].ToString());
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

        protected CompanyFundReceiptType Type
        {
            set { ViewState["Type"] = value; }
            get
            {
                return (CompanyFundReceiptType)ViewState["Type"];
            }
        }

        public IList<CompanyFundReceiptInfo> CompanyFundReceiptsList
        {
            get
            {
                if (ViewState["CompanyFundReceiptsList"] == null)
                {
                    var dataList = _companyFundReceipt.GetAllFundReceiptInfoList(Guid.Empty,ReceiptPage.DoReceivePay, Status, StartTime,
                                                                     EndTime, ReceiptNo, Type);
                    if (dataList != null)
                    {
                        return dataList;
                    }
                    return new List<CompanyFundReceiptInfo>();
                }
                return (IList<CompanyFundReceiptInfo>)ViewState["CompanyFundReceiptsList"];
            }
            set
            {
                ViewState["CompanyFundReceiptsList"] = value;
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
        /// <summary>
        /// 往来单位
        /// </summary>
        /// <param name="compId"></param>
        /// <returns></returns>
        protected string GetCompName(string compId)
        {
            var list = RelatedCompany.Instance.ToList();
            if (list == null)
                return "-";
            var info = list.FirstOrDefault(o => o.CompanyId == new Guid(compId));
            if (info == null)
            {
                var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == new Guid(compId));
                return shopInfo == null ? "-" : shopInfo.Name;
            }
            return info.CompanyName;
        }

        protected string GetPersonName(string personId)
        {
            return new PersonnelManager().GetName(new Guid(personId));
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

        protected string GetColor(string type)
        {
            if (type == string.Format("{0}",(int)CompanyFundReceiptType.Receive))
                return "#FF8C69";
            return "black";
        }
        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
        }

        #region[根据往来单位ID获取往来单位信息]

        /// <summary>
        /// 根据往来单位ID获取往来单位信息
        /// Add by liucaijun at 2011-October-10th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId"></param>
        /// <param name="type">0 website 1 AccountNumber 2 BankAccounts</param>
        /// <returns></returns>
        public string GetCompanyCussentByCompanyId(Guid companyId,Guid filialeId,int type)
        {
            var info = _companyBankAccountBindBll.GetCompanyBankAccountIdBind(companyId, filialeId)??_companyBankAccountBindBll.GetCompanyBankAccountBindInfo(companyId, filialeId);
            if (info != null && info.CompanyId != Guid.Empty)
            {
                return type == 0 ? info.WebSite : type == 1 ? info.AccountsNumber : info.BankAccounts;
            }
            var companyCussentInfo = _companyCussent.GetCompanyCussent(companyId);

            if (companyCussentInfo == null)
            {
                var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == companyId);
                if (shopInfo != null)
                {
                    return type == 0 ? shopInfo.Name : "";
                }
            }
            else
            {
                return type == 0 ? companyCussentInfo.WebSite : type == 1 ? companyCussentInfo.AccountNumber : companyCussentInfo.BankAccounts;
            }
            return string.Empty;
        }
        #endregion

        #region[绑定银行账号]
        protected List<ListItem> BindBankDataBound()
        {
            var listItem = new List<ListItem>();
            if (BankIds.Count > 0)
            {
                foreach (var bankInfo in BankIds.Distinct().Select(id => BankAccountManager.ReadInstance.GetBankAccounts(id)).Where(bankInfo => bankInfo.IsUse))
                {
                    listItem.Add(new ListItem(bankInfo.BankName + "-" + bankInfo.AccountsName, bankInfo.BankAccountsId.ToString()));
                }
            }
            listItem.Insert(0, new ListItem("全部", Guid.Empty.ToString()));
            return listItem;
        }
        #endregion

        /// <summary>
        /// 打开审核窗口
        /// </summary>
        /// <param name="recepitId"></param>
        /// <param name="companyId"> </param>
        protected string ShowAuditFormJs(object recepitId, object companyId)
        {
            var info = ShopFilialeList.FirstOrDefault(act => act.ID == new Guid(companyId.ToString()));
            if (info != null)
            {
                return string.Format(@"return ShowAudtiForm('{0}')", recepitId);
            }
            return string.Format(@"return ShowCheckForm('{0}')", recepitId);
        }

        protected void RG_CheckInfo_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var startTime = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var endTime = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var startTime1 = e.Item.FindControl("RDP_SExecuteTime") as RadDatePicker;
                var endTime1 = e.Item.FindControl("RDP_EExecuteTime") as RadDatePicker;
                if (StartTime != DateTime.MinValue)
                {
                    if (startTime != null) startTime.SelectedDate = StartTime;
                }
                if (EndTime != DateTime.MaxValue)
                {
                    if (endTime != null) endTime.SelectedDate = EndTime;
                }
                if (SExecuteTime != DateTime.MinValue)
                {
                    if (startTime1 != null) startTime1.SelectedDate = SExecuteTime;
                }
                if (EExecuteTime != DateTime.MaxValue)
                {
                    if (endTime1 != null) endTime1.SelectedDate = EExecuteTime;
                }

            }
        }

        #region  添加公司
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

        /// <summary>
        /// 销售公司
        /// </summary>
        protected string SelectSaleFilialeId
        {
            get
            {
                return ViewState["SaleFilialeId"] == null ? Guid.Empty.ToString()
                    : ViewState["SaleFilialeId"].ToString();
            }
            set
            {
                ViewState["SaleFilialeId"] = value;
            }
        }

        protected string GetBankNameById(object obj)
        {
            var bankAccountInfo = BankAccountManager.ReadInstance.GetBankAccounts(new Guid(obj.ToString()));
            return bankAccountInfo != null ? bankAccountInfo.BankName : "-";
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
        #endregion

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
    }
}
