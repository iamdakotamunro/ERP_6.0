using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /*最后修改：刘彩军
     修改内容：去掉手续费最高上限50/修复资金流选跟分类,点转账 默认支付宝-》应该是无法选
     修改时间：2011-March-16th*/
    public partial class TransferForm : WindowsPage
    {
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private static readonly IWasteBook _wasteBook=new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Write);
        readonly BankAccountManager _bankAccountManager = new BankAccountManager(_bankAccounts,_bankAccountDao,_wasteBook);
        protected Boolean IsEdit
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["optype"]) && Request.QueryString["optype"] == "edit";
            }
        }

        protected Boolean IsShop
        {
            get
            {
                return ViewState["IsShop"] != null && Convert.ToBoolean(ViewState["IsShop"]);
            }
            set
            {
                ViewState["IsShop"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Page.Title = "转账";
                Guid bankAccountsId = WebControl.GetGuidFromQueryString("BankAccountsId");
                Guid filialeId = WebControl.GetGuidFromQueryString("FilialeId");
                var filialeList = CacheCollection.Filiale.GetList();
                if (filialeList.Any(w => w.ID == filialeId) == false)
                {
                    var salePlatformInfo = CacheCollection.SalePlatform.Get(filialeId);
                    if (salePlatformInfo != null && salePlatformInfo.ID != Guid.Empty)
                    {
                        filialeId = salePlatformInfo.FilialeId;
                    }
                }
                #region 转入公司加载
                var inFilialeList = CacheCollection.Filiale.GetHeadList();
                RCB_InFilialeList.DataSource = inFilialeList;
                RCB_InFilialeList.DataTextField = "Name";
                RCB_InFilialeList.DataValueField = "ID";
                RCB_InFilialeList.DataBind();
                RCB_InFilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                RCB_InFilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                RCB_InFilialeList.SelectedValue = Guid.Empty.ToString();
                #endregion
                FilialeId = filialeId;
                IList<BankAccountInfo> bankAccountsList = _bankAccounts.GetList();
                //转出账户
                for (var i = 0; i < bankAccountsList.Count; i++)
                {
                    RCB_OutBankAccountsId.Items.Insert(i, new RadComboBoxItem(bankAccountsList[i].BankName + " - " + bankAccountsList[i].AccountsName, bankAccountsList[i].BankAccountsId.ToString()));
                }
                RCB_FilialeList.DataSource = filialeList;
                RCB_FilialeList.DataTextField = "Name";
                RCB_FilialeList.DataValueField = "ID";
                RCB_FilialeList.DataBind();
                RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                RCB_FilialeList.SelectedValue = filialeId.ToString();

                #region [编辑]
                if (IsEdit)
                {
                    LB_Inster.Visible = GetPowerOperationPoint("Transfer");
                    Auditing.Visible = GetPowerOperationPoint("TransferAuditing");
                    LB_Delete.Visible = GetPowerOperationPoint("Delete");
                    var wasteBookInfo = _wasteBook.GetWasteBook(WebControl.GetGuidFromQueryString("WasteBookId"))??new WasteBookInfo();
                    WasteBookInfo wbInfoOut = _wasteBook.GetWasteBookInfo(wasteBookInfo.TradeCode).Decrease;
                    if (wbInfoOut == null)
                    {
                        RAM.Alert("不是有效的公司信息，请选择账户");
                        RAM.ResponseScripts.Add("CancelWindow();");
                        return;
                    }
                    TB_Income.Text = Math.Abs(wbInfoOut.Income).ToString("0.00");
                    TB_Description.Text = string.Empty;
                    RCB_OutBankAccountsId.SelectedValue = wbInfoOut.BankAccountsId.ToString();
                    if (FilialeId == _reckoningElseFilialeid)
                    {
                        RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                    }
                    else
                    {
                        var filialeInfo = filialeList.First(ent => ent.ID == FilialeId);
                        RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                    }
                    RCB_FilialeList.SelectedIndex = 0;
                    //手续费
                    WasteBookInfo wbInfoFee = _wasteBook.GetWasteBookInfo(wasteBookInfo.TradeCode).TransferFee;
                    TradeCodeT = wbInfoOut.TradeCode;
                    WasteBookIdOut = wbInfoOut.WasteBookId;
                    if (wbInfoFee != null)
                    {
                        TB_HC.Text = Math.Abs(wbInfoFee.Income).ToString("0.00");
                        PoundageT = Convert.ToDecimal(Math.Abs(Convert.ToDouble(wbInfoFee.Income)));
                    }
                    else
                    {
                        TB_HC.Text = string.Empty;
                        PoundageT = 0;
                    }
                    //转入账户
                    if (FilialeHelper.IsEntityShop(wasteBookInfo.SaleFilialeId))
                    {
                        IsShop = true;
                        RCB_InFilialeList.SelectedValue = string.Format("{0}", _reckoningElseFilialeid);
                        RCB_InFilialeList.Enabled = false;
                        RCB_InBankAccountsId.Items.Insert(0, new RadComboBoxItem("门店资金账户", Guid.Empty.ToString()));
                        RCB_InBankAccountsId.SelectedIndex = 0;
                        RCB_InBankAccountsId.Enabled = false;
                    }
                    else
                    {
                        WasteBookInfo wbInfoIn = _wasteBook.GetWasteBookInfo(wasteBookInfo.TradeCode).Increase;
                        if (wbInfoIn == null)
                        {
                            RAM.Alert("不是有效的公司信息，请选择账户");
                            RAM.ResponseScripts.Add("CancelWindow();");
                            return;
                        }
                        WasteBookIdIn = wbInfoIn.WasteBookId;
                        RCB_InFilialeList.SelectedValue = wbInfoIn.SaleFilialeId.ToString();
                        RCB_InFilialeList.Enabled = false;
                        InFilialeId = wbInfoIn.SaleFilialeId;
                        var bank =wbInfoIn.BankAccountsId!=Guid.Empty? _bankAccounts.GetBankAccounts(wbInfoIn.BankAccountsId):new BankAccountInfo();
                        RCB_InBankAccountsId.Items.Insert(0, new RadComboBoxItem(bank.BankName + " - " + bank.AccountsName, wbInfoIn.BankAccountsId.ToString()));
                        RCB_InBankAccountsId.SelectedIndex = 0;
                        RCB_InBankAccountsId.Enabled = false;
                        InBankId = new Guid(wbInfoIn.BankAccountsId.ToString());
                    }
                    TradeCodeT = wasteBookInfo.TradeCode;

                }
                #endregion
                else
                {
                    if (FilialeId == Guid.Empty)
                    {
                        RAM.Alert("不是有效的公司信息，请选择账户");
                        RAM.ResponseScripts.Add("CancelWindow();");
                        return;
                    }
                    LB_Inster.Visible = GetPowerOperationPoint("Transfer");
                    Auditing.Visible = false; //GetPowerOperationPoint("Auditing");
                    LB_Delete.Visible = false; //GetPowerOperationPoint("Delete");
                    if (bankAccountsId != Guid.Empty)
                        RCB_OutBankAccountsId.SelectedValue = bankAccountsId.ToString();
                    if (filialeId != Guid.Empty)
                        RCB_FilialeList.SelectedValue = filialeId.ToString();
                    Auditing.Enabled = false;
                }
            }
        }

        public static string TradeCodeT
        {
            get;
            set;
        }

        public static decimal PoundageT
        {
            get;
            set;
        }

        public static Guid InBankId
        {
            get;
            set;
        }

        protected Guid FilialeId
        {
            get { return (Guid)ViewState["FilialeId"]; }
            set { ViewState["FilialeId"] = value; }
        }

        protected Guid InFilialeId
        {
            get { return (Guid)ViewState["InFilialeId"]; }
            set { ViewState["InFilialeId"] = value; }
        }

        protected Guid WasteBookIdIn
        {
            get
            {
                return ViewState["WasteBookIdIn"] == null ? Guid.Empty : new Guid(ViewState["WasteBookIdIn"].ToString());
            }
            set
            {
                ViewState["WasteBookIdIn"] = value;
            }
        }

        protected Guid WasteBookIdOut
        {
            get
            {
                return ViewState["WasteBookIdOut"] == null ? Guid.Empty : new Guid(ViewState["WasteBookIdOut"].ToString());
            }
            set
            {
                ViewState["WasteBookIdOut"] = value;
            }
        }

        #region 取得用户操作权限
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "AccountsT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
        #endregion

        //新建&修改
        protected void Button_InsterTransfer(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                var inBankAccountsId = new Guid(RCB_InBankAccountsId.SelectedValue);
                var outBankAccountsId = new Guid(RCB_OutBankAccountsId.SelectedValue);
                Guid wasteBookId = WebControl.GetGuidFromQueryString("WasteBookId");
                WasteBookInfo wbif = _wasteBook.GetWasteBook(wasteBookId)??new WasteBookInfo();
                if (FilialeHelper.IsEntityShop(wbif.SaleFilialeId))
                {
                    RAM.Alert("门店相关资金流不能修改!");
                    return;
                }
                TradeCodeT = wbif.TradeCode;
                string description = " ";
                decimal income = Convert.ToDecimal(TB_Income.Text);
                decimal poundage = string.IsNullOrEmpty(TB_HC.Text) ? 0 : Convert.ToDecimal(TB_HC.Text);
                if (InBankId != inBankAccountsId)
                {
                    description = WebControl.RetrunUserAndTime("[" + _bankAccounts.GetBankAccounts(inBankAccountsId).BankName + "]");

                }
                if (TB_Description.Text != string.Empty)
                {
                    description = WebControl.RetrunUserAndTime(description + TB_Description.Text);
                }
                if (inBankAccountsId == outBankAccountsId)
                {
                    RAM.Alert("同一资金账户不能转移资金!");
                    return;
                }
                if (income <= 0)
                {
                    RAM.Alert("转移资金额不正确!");
                    return;
                }
                if (poundage < 0)
                {
                    RAM.Alert("手续费金额不正确！");
                    return;
                }
                if (poundage > 0)
                {
                    if (!WebControl.CheckPoundage(income, poundage))
                    {
                        RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                        return;
                    }

                    if (poundage != PoundageT)
                    {
                        description = description + "手续费：" + poundage;
                    }
                }
                try
                {
                    if (PoundageT == 0)
                    {
                        if (poundage == 0)
                        {
                            _bankAccountManager.UpdateBll(wasteBookId, description, income, TradeCodeT, poundage, inBankAccountsId);
                            _wasteBook.UpdateDateTime(TradeCodeT);
                        }
                        if (poundage > 0)
                        {
                            _bankAccountManager.UpdateBll(wasteBookId, description, income, TradeCodeT, poundage, inBankAccountsId);
                            _bankAccountManager.InsertPoundage(outBankAccountsId, TradeCodeT, poundage, InFilialeId);
                            _wasteBook.UpdateDateTime(TradeCodeT);
                        }
                    }
                    else if (PoundageT > 0)
                    {
                        if (poundage == 0)
                        {
                            _bankAccountManager.UpdateBll(wasteBookId, description + "手续费：0", income, TradeCodeT, poundage, inBankAccountsId);
                            _wasteBook.DeleteWasteBookPoundage(TradeCodeT, -PoundageT);
                            _wasteBook.UpdateDateTime(TradeCodeT);
                        }
                        if (poundage > 0)
                        {
                            _bankAccountManager.UpdateBll(wasteBookId, description, income, TradeCodeT, poundage, inBankAccountsId);
                            _bankAccountManager.UpdatePoundage(TradeCodeT, poundage);
                            _wasteBook.UpdateDateTime(TradeCodeT);
                        }
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception exp)
                {
                    RAM.Alert("保存失败！ \n\n消息提示:" + exp.Message);
                }
            }
            else
            {
                string tradeCode = (new CodeManager()).GetCode(CodeType.VI);
                TradeCodeT = tradeCode;
                var inBankAccountsId = new Guid(RCB_InBankAccountsId.SelectedValue);
                var outBankAccountsId = new Guid(RCB_OutBankAccountsId.SelectedValue);
                if (inBankAccountsId == outBankAccountsId)
                {
                    RAM.Alert("同一资金账户不能转移资金!");
                    LB_Inster.Enabled = true;
                }
                else
                {
                    decimal sum = Convert.ToDecimal(TB_Income.Text);
                    if (sum <= 0)
                    {
                        RAM.Alert("转移资金额不正确!");
                        LB_Inster.Enabled = true;
                    }
                    else
                    {
                        decimal poundage = string.IsNullOrEmpty(TB_HC.Text) ? 0 : Convert.ToDecimal(TB_HC.Text);
                        string tbDescription = !string.IsNullOrWhiteSpace(TB_Description.Text) ? TB_Description.Text.Trim() : "无";
                        //var description = TB_Description.Text + " [转账申请人:" + CurrentSession.Personnel.Get().RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                        if (poundage > 0)
                        {
                            if (!WebControl.CheckPoundage(sum, poundage))
                            {
                                RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                                LB_Inster.Enabled = true;
                                return;
                            }
                            //description += "手续费：" + poundage.ToString();
                        }
                        //if (!CheckBalance(outBankAccountsId, sum + poundage))
                        //{
                        //    RAM.Alert("系统提示：转账金额超过帐号剩余额度！");
                        //    return;
                        //}
                        try
                        {
                            var info = _bankAccountManager.NewVirement(inBankAccountsId, outBankAccountsId, sum, poundage, tbDescription, tradeCode, FilialeId, InFilialeId, CurrentSession.Personnel.Get().RealName);
                            //资金流转账操作记录添加
                            if (info != null && info.WasteBookId != Guid.Empty)
                            {
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.WasteBookId, info.TradeCode,
                                      OperationPoint.FundFlow.TransferAccounts.GetBusinessInfo(), string.Empty);
                            }
                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        catch (Exception exp)
                        {
                            RAM.Alert("转移资金失败！ \n\n消息提示：'" + exp.Message);
                        }
                    }
                }
            }
        }

        protected void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                _wasteBook.DeleteWasteBook(TradeCodeT);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }

        //审核
        protected void Button_Auditing(object sender, EventArgs e)
        {
            try
            {
                WasteBookInfo wasteBookInfoIn = WasteBookIdIn != Guid.Empty ? _wasteBook.GetWasteBook(WasteBookIdIn) : null;
                WasteBookInfo wasteBookInfoOut = WasteBookIdOut != Guid.Empty ? _wasteBook.GetWasteBook(WasteBookIdOut) : null;
                if (wasteBookInfoOut != null && !CheckBalance(wasteBookInfoOut.BankAccountsId, wasteBookInfoOut.Income, PoundageT))
                {
                    RAM.Alert("系统提示：转账金额超过帐号剩余额度！");
                    return;
                }
                var description = " [审核人:" + CurrentSession.Personnel.Get().RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 审核备注：" +
                             TB_Description.Text + "]";
                using (var tran = new TransactionScope())
                {
                    if (IsShop)
                    {
                        if (wasteBookInfoIn != null)
                        {
                            _wasteBook.UpdateDescription(wasteBookInfoIn.WasteBookId, description);
                        }
                        if (wasteBookInfoOut != null)
                        {
                            _wasteBook.UpdateDescription(wasteBookInfoOut.WasteBookId, description);
                        }
                    }
                    else
                    {
                        if (wasteBookInfoIn == null || wasteBookInfoOut == null) return;
                        _wasteBook.UpdateDescription(wasteBookInfoIn.WasteBookId, description);
                        _wasteBook.UpdateDescription(wasteBookInfoOut.WasteBookId, description);
                    }
                    _wasteBook.Auditing(TradeCodeT);
                    tran.Complete();
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName,
                    wasteBookInfoOut != null ? wasteBookInfoOut.WasteBookId : WebControl.GetGuidFromQueryString("WasteBookId"),
                    wasteBookInfoOut != null ? wasteBookInfoOut.TradeCode : wasteBookInfoIn != null ? wasteBookInfoIn.TradeCode : string.Empty,
                      OperationPoint.FundFlow.Auditing.GetBusinessInfo(), string.Empty);

                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception exp)
            {
                RAM.Alert("审核失败！\n\n消息提示:" + exp.Message);
            }
        }

        protected void RCB_InBankAccounts_OnItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<BankAccountInfo> bankAccountsList = _bankAccounts.GetBankAccountsNoBindingList();
                var bankAccountsLists = bankAccountsList.Where(b => b.IsUse);
                var s = bankAccountsLists.Where(b => b.BankName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= s.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (BankAccountInfo i in s)
                    {
                        var item = new RadComboBoxItem { Text = i.BankName, Value = i.BankAccountsId.ToString() };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        #region[选择公司后加载出对应的银行账号]
        protected void Rcb_InFilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var inFilialeId = new Guid(e.Value.Trim());
            if (inFilialeId == Guid.Empty)
            {
                return;
            }
            InFilialeId = inFilialeId;
            ShowBankNameAndAccountsName(inFilialeId);
        }
        #endregion

        protected void ShowBankNameAndAccountsName(Guid filialeId)
        {
            RCB_InBankAccountsId.Items.Clear();
            RCB_InBankAccountsId.Text = string.Empty;
            IList<BankAccountInfo> bankList = filialeId == _reckoningElseFilialeid ? _bankAccountDao.GetListByNotIsMain(false).Where(ent => ent.IsUse).ToList() 
                : _bankAccountDao.GetListByTargetId(filialeId).Where(ent =>ent.IsUse).ToList();
            if (bankList.Count > 0)
            {
                bool hasMain = false;
                foreach (var info in bankList.Where(ent=>ent.IsMain))
                {
                    hasMain = true;
                    RCB_InBankAccountsId.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                }
                if (!hasMain)
                {
                    foreach (var info in bankList.Where(ent=>!ent.IsMain))
                    {
                        RCB_InBankAccountsId.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                    }
                }
                RCB_InBankAccountsId.SelectedValue = bankList[0].BankAccountsId.ToString();
            }
            else
            {
                RCB_InBankAccountsId.Items.Add(new RadComboBoxItem("未找到相关银行帐号", Guid.Empty.ToString()));
            }
        }

        /// <summary>判断银行帐号余额是否满足当前操作金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <param name="sum">当前操作金额</param>
        /// <param name="transferFee">手续费</param>
        /// <returns></returns>
        private bool CheckBalance(Guid bankAccountsId, decimal sum, decimal transferFee)
        {
            var nonce = (decimal)_bankAccounts.GetBankAccountsNonce(bankAccountsId);
            return nonce >= Math.Abs(sum) + transferFee;
        }
    }
}