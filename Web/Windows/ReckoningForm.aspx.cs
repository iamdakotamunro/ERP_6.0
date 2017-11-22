using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using OperationLog.Core.Attributes;
using Telerik.Web.UI;
using Code = ERP.BLL.Implement.Inventory.CodeManager;
using Reckoning = ERP.BLL.Implement.Inventory.ReckoningManager;
using WasteBook = ERP.BLL.Implement.Inventory.WasteBookManager;
using TurnoverType = AllianceShop.Enum.TurnOverType;

namespace ERP.UI.Web.Windows
{
    /*最后修改：刘彩军
     修改内容：去掉手续费最高上限50
     修改时间：2011-March-16th*/
    public partial class ReckoningForm : WindowsPage
    {
        private readonly IWasteBook _wasteBook=new DAL.Implement.Inventory.WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IReckoning _reckoning = new DAL.Implement.Inventory.Reckoning(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        // ReSharper disable once FunctionComplexityOverflow
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //修改付款单，收款单
                if (IsEdit)
                {
                    ReckoningInfo reckoningInfo = _reckoning.GetReckoning(WebControl.GetGuidFromQueryString("ReckoningId"));
                    if (CacheCollection.Filiale.GetList().Any(f => f.ID == reckoningInfo.ThirdCompanyID))
                    {
                        var firstOrDefault = CacheCollection.Filiale.GetList().FirstOrDefault(f => f.ID == reckoningInfo.ThirdCompanyID);
                        if (firstOrDefault != null)
                            TB_CompanyName.Text = firstOrDefault.Name;
                    }
                    else
                    {
                        var info = _companyCussent.GetCompanyCussent(reckoningInfo.ThirdCompanyID);
                        if (info != null)
                        {
                            if (info.CompanyType == (int)CompanyType.Express) IsExpress = true;
                            TB_CompanyName.Text = info.CompanyName;
                        }
                    }
                    TB_CompanyName.Enabled = false;
                    if (FilialeHelper.IsEntityShop(reckoningInfo.ThirdCompanyID))
                    {
                        TR_ShopBankAccount.Visible = true;
                        var shopBankAccountsList = ReckoningSao.GetBankAccountsByEntityShop(reckoningInfo.ThirdCompanyID).ToList();
                        for (var i = 0; i < shopBankAccountsList.Count; i++)
                        {
                            RCB_ShopBankAccountsId.Items.Insert(i, new RadComboBoxItem(shopBankAccountsList[i].BankName + " - " + shopBankAccountsList[i].BankName, shopBankAccountsList[i].ID.ToString()));
                        }
                    }
                    RCB_FilialeList.DataSource = CacheCollection.Filiale.GetHeadList().ToList();
                    RCB_FilialeList.DataTextField = "Name";
                    RCB_FilialeList.DataValueField = "ID";
                    RCB_FilialeList.DataBind();
                    RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
                    WasteBookInfo wbInfo = _wasteBook.GetWasteBook(new Guid(_wasteBook.GetWasteBookIdForReckoning(reckoningInfo.TradeCode)));

                    RCB_BankAccountsId.SelectedValue = wbInfo.BankAccountsId.ToString();
                    BkId = wbInfo.BankAccountsId.ToString();
                    TB_PaymentPrice.Text = Math.Abs(wbInfo.Income).ToString("0.##");
                    TB_PaymentPrice.Enabled = false;
                    TB_Description.Text = string.Empty;
                    decimal tradeCodeNum = _wasteBook.GetTradeCodeNum(reckoningInfo.TradeCode);
                    TradeCodeT = wbInfo.TradeCode;
                    if (tradeCodeNum == 2)
                    {
                        var poundage = Math.Abs(_wasteBook.GetPoundageForReckoning(reckoningInfo.TradeCode));
                        TB_HC.Text = poundage.ToString("0.##");
                        PoundageT = Convert.ToDecimal(poundage);
                    }
                    else
                    {
                        TB_HC.Text = "0";
                        PoundageT = 0;
                    }

                    if (reckoningInfo.ReckoningType == (int)ReckoningType.Defray)
                    {
                        Page.Title = "付款单";
                        NonceReckoningType = ReckoningType.Defray;
                        LB_Delete.Visible = GetPowerOperationPoint("OrderDelete");
                        Auditing.Visible = GetPowerOperationPoint("IncreaseAuditing");
                        LB_Inster.Visible = GetPowerOperationPoint("PaymentOrder");
                    }
                    else
                    {
                        Page.Title = "收款单";
                        NonceReckoningType = ReckoningType.Income;
                        TB_HC.Enabled = false;
                        LB_Delete.Visible = GetPowerOperationPoint("OrderDelete");
                        Auditing.Visible = GetPowerOperationPoint("SubtractAuditing");
                        LB_Inster.Visible = GetPowerOperationPoint("CollectionOrder");
                    }
                    var list = CacheCollection.Filiale.Get(reckoningInfo.FilialeId);
                    RCB_FilialeList.Items.Clear();
                    RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(list.Name, list.ID.ToString()));
                    RCB_FilialeList.SelectedIndex = 0;
                    RCB_FilialeList.Enabled = false;
                    var bankAccountsId = _wasteBook.GetWasteBookByBankAccountsId(reckoningInfo.TradeCode).BankAccountsId;
                    var bank = _bankAccounts.GetBankAccounts(bankAccountsId);
                    RCB_BankAccountsId.Items.Insert(0, new RadComboBoxItem(bank.BankName + bank.AccountsName, bank.BankAccountsId.ToString()));
                    RCB_BankAccountsId.SelectedIndex = 0;
                    RCB_BankAccountsId.Enabled = false;
                }
                else
                {
                    CompanyId = new Guid(Request.QueryString["CompanyId"]);
                    var filialeId = new Guid(Request.QueryString["FilialeId"]);
                    if (filialeId == Guid.Empty)
                    {
                        RCB_FilialeList.DataSource = CacheCollection.Filiale.GetHeadList().ToList();
                        RCB_FilialeList.DataTextField = "Name";
                        RCB_FilialeList.DataValueField = "ID";
                        RCB_FilialeList.DataBind();
                        RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
                    }
                    else
                    {
                        var list = CacheCollection.Filiale.Get(filialeId);
                        RCB_FilialeList.Items.Clear();
                        RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(list.Name, list.ID.ToString()));
                        RCB_FilialeList.SelectedIndex = 0;
                        ShowBankNameAndAccountsName(filialeId);
                    }
                    if (CacheCollection.Filiale.GetList().Any(f => f.ID == CompanyId))
                    {
                        var firstOrDefault = CacheCollection.Filiale.GetList().FirstOrDefault(f => f.ID == CompanyId);
                        if (firstOrDefault != null)
                            TB_CompanyName.Text = firstOrDefault.Name;
                    }
                    else
                    {
                        var info = _companyCussent.GetCompanyCussent(CompanyId);
                        if (info != null)
                        {
                            if (info.CompanyType == (int)CompanyType.Express) IsExpress = true;
                            TB_CompanyName.Text = info.CompanyName;
                        }
                    }
                    if (string.IsNullOrEmpty(TB_CompanyName.Text))
                    {
                        RAM.Alert("不是有效的往来单位信息");
                        RAM.ResponseScripts.Add("CancelWindow();");
                        return;
                    }
                    if (Request.QueryString["ReckoningType"] == ReckoningType.Defray.ToString())
                    {
                        Page.Title = "付款单";
                        NonceReckoningType = ReckoningType.Defray;
                        LB_Delete.Visible = false;// GetPowerOperationPoint("OrderDelete");
                        Auditing.Visible = false; //GetPowerOperationPoint("IncreaseAuditing");
                        LB_Inster.Visible = GetPowerOperationPoint("PaymentOrder");
                    }
                    else
                    {
                        Page.Title = "收款单";
                        NonceReckoningType = ReckoningType.Income;
                        TB_HC.Enabled = false;
                        LB_Delete.Visible = false; //GetPowerOperationPoint("OrderDelete");
                        Auditing.Visible = false; //GetPowerOperationPoint("SubtractAuditing");
                        LB_Inster.Visible = GetPowerOperationPoint("CollectionOrder");
                    }
                }
            }
        }

        #region[取得用户操作权限]
        /// <summary>
        /// 取得用户操作权限（通用方法）
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "CussentReckoningT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
        #endregion

        #region[公用属性]

        public static string TradeCodeT
        {
            get;
            set;
        }

        public static string BkId
        {
            get;
            set;
        }

        public static decimal PoundageT
        {
            get;
            set;
        }
        protected Boolean IsEdit
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["optype"]) && Request.QueryString["optype"] == "edit";
            }
        }

        //是否是快递公司
        protected bool IsExpress
        {
            get
            {
                if (ViewState["IsExpress"] == null) return false;
                return Convert.ToBoolean(ViewState["IsExpress"]);
            }
            set
            {
                ViewState["IsExpress"] = value;
            }
        }

        //往来单位编号
        protected Guid CompanyId
        {
            get
            {
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value.ToString();
            }
        }

        protected ReckoningType NonceReckoningType
        {
            get
            {
                return (ReckoningType)ViewState["NonceReckoningType"];
            }
            set
            {
                ViewState["NonceReckoningType"] = value;
            }
        }

        #endregion

        #region[保存]
        // ReSharper disable once FunctionComplexityOverflow
        protected void Button_InsterReckoning(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                Guid reckoningId = WebControl.GetGuidFromQueryString("ReckoningId");
                ReckoningInfo rinfo = _reckoning.GetReckoning(reckoningId);
                Guid compId = rinfo.ThirdCompanyID;

                Auditing.Enabled = true;
                Guid filialeId = new Guid(RCB_FilialeList.SelectedValue);
                string tradeCode = rinfo.TradeCode;
                TradeCodeT = rinfo.TradeCode;
                string description = WebControl.RetrunUserAndTime("[" + RCB_BankAccountsId.Text + "]" + TB_Description.Text);
                string descriptionForWastebook = WebControl.RetrunUserAndTime(TB_Description.Text);
                decimal paymentPrice = Convert.ToDecimal(TB_PaymentPrice.Text);
                decimal nonceTotalled = _reckoning.GetTotalled(compId);
                DateTime dateCreated = WebControl.GetNowTime();
                decimal poundage = 0;

                if (NonceReckoningType == ReckoningType.Defray)
                {
                    poundage = string.IsNullOrEmpty(TB_HC.Text) ? 0 : Convert.ToDecimal(TB_HC.Text);

                    if (poundage > 0)
                    {
                        if (paymentPrice <= 200)
                        {
                            if (poundage > (decimal)1.8)
                            {
                                RAM.Alert("200元以下手续费不能超过1.8!");
                                LB_Inster.Enabled = true;
                                Auditing.Enabled = false;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    paymentPrice = -paymentPrice;
                }
                if (poundage != PoundageT)
                {
                    descriptionForWastebook += " 手续费：" + poundage;
                }
                if (RCB_BankAccountsId.SelectedValue != BkId)
                {
                    descriptionForWastebook += "[" + _bankAccounts.GetBankAccounts(new Guid(RCB_BankAccountsId.SelectedValue)).BankName + "]" + WebControl.RetrunUserAndTime("");
                }
                var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, compId, tradeCode, dateCreated, description, paymentPrice, nonceTotalled, (int)NonceReckoningType, (int)ReckoningStateType.Currently, (int)CheckType.NotCheck, (int)AuditingState.No, null)
                                                ;
                TradeCodeT = tradeCode;
                try
                {
                    // Begin modify by tianys at 2009-06-04
                    using (var ts = new TransactionScope())
                    {
                        var reckoningInfoUpdate = new ReckoningInfo(paymentPrice, descriptionForWastebook, reckoningId, DateTime.Now);

                        if (reckoningInfo.ContructType != ContructType.Update)
                            return;
                        _reckoning.Update(reckoningInfoUpdate);

                        var bankAccountsId = new Guid(RCB_BankAccountsId.SelectedValue);
                        var wasteBookInfo = reckoningInfo.ReckoningType == (int)ReckoningType.Defray ? _wasteBook.GetWasteBookInfo(reckoningInfo.TradeCode).TransferOut : _wasteBook.GetWasteBookInfo(reckoningInfo.TradeCode).TransferIn;
                        wasteBookInfo.BankAccountsId = bankAccountsId;
                        wasteBookInfo.Income = -paymentPrice;
                        wasteBookInfo.Description = descriptionForWastebook;
                        wasteBookInfo.SaleFilialeId = filialeId;
                        wasteBookInfo.LinkTradeCode = TradeCodeT;
                        wasteBookInfo.LinkTradeType = (int)WasteBookLinkTradeType.Other;
                        wasteBookInfo.BankTradeCode = string.Empty;
                        wasteBookInfo.State = (int)WasteBookState.Currently;
                        wasteBookInfo.IsOut = false;
                        _wasteBook.Update(wasteBookInfo);
                        _wasteBook.UpdateDateTime(wasteBookInfo.TradeCode);
                        var wastFeeInfo = _wasteBook.GetWasteBookInfo(reckoningInfo.TradeCode).TransferFee;
                        // 扣除手续费
                        if (poundage > 0 && NonceReckoningType == ReckoningType.Defray)
                        {
                            if (wastFeeInfo == null)
                            {
                                description = "[付款] [手续费]";
                                wasteBookInfo.WasteBookId = Guid.NewGuid();
                                wasteBookInfo.Income = -poundage;
                                wasteBookInfo.Description = description;
                                wasteBookInfo.AuditingState = (int)AuditingState.Hide;
                                wasteBookInfo.SaleFilialeId = filialeId;
                                if (wasteBookInfo.Income != 0)
                                {
                                    _wasteBook.Insert(wasteBookInfo);
                                }
                                _wasteBook.UpdateDateTime(wasteBookInfo.TradeCode);
                            }
                            else
                            {
                                description = "";
                                wastFeeInfo.BankAccountsId = bankAccountsId;
                                wastFeeInfo.Income = -poundage;
                                wastFeeInfo.Description = description;
                                wastFeeInfo.SaleFilialeId = filialeId;
                                _wasteBook.Update(wastFeeInfo);
                                _wasteBook.UpdateDateTime(wastFeeInfo.TradeCode);
                            }
                        }
                        else
                        {
                            if (wastFeeInfo != null)
                            {
                                _wasteBook.DeleteWasteBookPoundage(wastFeeInfo.TradeCode, wastFeeInfo.Income);
                            }
                        }
                        ts.Complete();
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("资金账户操作失败！");
                }
            }
            else
            {
                #region 添加默认备注 add by dyy at 2010.6.29
                String sBeforeDesc;
                String sBeforeDescForWastbook;
                if (Request.QueryString["ReckoningType"] == ReckoningType.Defray.ToString())
                {
                    sBeforeDesc = " [付款] ";
                    sBeforeDescForWastbook = " [付款] " + TB_CompanyName.Text;
                }
                else
                {
                    sBeforeDesc = " [收到] ";
                    sBeforeDescForWastbook = " [收到] " + TB_CompanyName.Text;
                }
                #endregion

                LB_Inster.Enabled = false;
                Auditing.Enabled = true;
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                string tradeCode;
                string description = sBeforeDesc + " [" + RCB_BankAccountsId.Text + "]" + TB_Description.Text + "[申请人:" + CurrentSession.Personnel.Get().RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                string descriptionForWastebook = sBeforeDescForWastbook + " " + TB_Description.Text + "[申请人:" + CurrentSession.Personnel.Get().RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                decimal paymentPrice = Convert.ToDecimal(TB_PaymentPrice.Text);
                // Begin add by tianys at 2009-06-04
                decimal poundage = 0;
                // End add
                int wbType;
                if (NonceReckoningType == ReckoningType.Defray)
                {
                    tradeCode = (new Code()).GetCode(CodeType.PY);
                    wbType = (int)WasteBookType.Decrease;
                    // Begin add by tianys at 2009-06-04
                    poundage = string.IsNullOrEmpty(TB_HC.Text) ? 0 : Convert.ToDecimal(TB_HC.Text);

                    if (poundage > 0)
                    {
                        if (paymentPrice <= 200)
                        {
                            if (!WebControl.CheckPoundage(paymentPrice, poundage))
                            {
                                RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                                LB_Inster.Enabled = true;
                                Auditing.Enabled = false;
                                return;
                            }
                        }
                        description += " 手续费：" + poundage;
                        descriptionForWastebook += " 手续费：" + poundage;
                    }
                    // End add
                }
                else
                {
                    tradeCode = (new Code()).GetCode(CodeType.GT);
                    paymentPrice = -paymentPrice;
                    wbType = (int)WasteBookType.Increase;
                }

                var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, CompanyId, tradeCode, description,
                                                      paymentPrice, (int)NonceReckoningType,
                                                      (int)ReckoningStateType.Currently, (int)CheckType.NotCheck,
                                                      (int)AuditingState.No, null, Guid.Empty)
                                        {
                                            LinkTradeType = (int)ReckoningLinkTradeType.Other,
                                            IsOut = false
                                        };

                TradeCodeT = tradeCode;
                try
                {
                    // Begin modify by tianys at 2009-06-04
                    var bankAccountsId = new Guid(RCB_BankAccountsId.SelectedValue);
                    var wasteBookInfoOut = new WasteBookInfo(Guid.NewGuid(), bankAccountsId, tradeCode, descriptionForWastebook, -paymentPrice, (int)AuditingState.Hide, wbType, filialeId)
                                               {
                                                   LinkTradeCode = TradeCodeT,
                                                   LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                                   BankTradeCode = string.Empty,
                                                   State = (int)WasteBookState.Currently,
                                                   IsOut = false
                                               };
                    using (var ts = new TransactionScope())
                    {
                        string errorMsg;
                        _reckoning.Insert(reckoningInfo,out errorMsg);

                        // 扣除手续费
                        if (poundage > 0 && NonceReckoningType == ReckoningType.Defray)
                        {
                            var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(), bankAccountsId, tradeCode,descriptionForWastebook, -paymentPrice, (int)AuditingState.Hide,(int)WasteBookType.Decrease, filialeId)
                            {
                                LinkTradeCode = TradeCodeT,
                                LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                BankTradeCode = string.Empty,
                                State = (int)WasteBookState.Currently,
                                IsOut = false,
                                WasteBookId = Guid.NewGuid(),
                                Income = -poundage,
                                Description = "[付款] [手续费]"
                            };
                            _wasteBook.Insert(wasteBookInfo);
                        }
                        if (wasteBookInfoOut.Income!=0)
                        {
                            _wasteBook.Insert(wasteBookInfoOut);
                        }
                        ts.Complete();
                    }

                    //往来账付款收款单填写增加操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    EnumPointAttribute currentAccountState = reckoningInfo.ReckoningType == (int)ReckoningType.Defray ? OperationPoint.CurrentReceivedPayment.FillPayment.GetBusinessInfo() : OperationPoint.CurrentReceivedPayment.FillBill.GetBusinessInfo();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, reckoningInfo.ReckoningId, reckoningInfo.TradeCode,
                        currentAccountState, string.Empty);
                    // End modify
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("资金账户操作失败！");
                }
            }
        }
        #endregion

        #region[删除]
        protected void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                _reckoning.Delete(TradeCodeT);
                _wasteBook.DeleteWasteBook(TradeCodeT);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }
        #endregion

        #region[审核]
        protected void Button_Auditing(object sender, EventArgs e)
        {
            var personnelInfo = CurrentSession.Personnel.Get();
            using (var ts = new TransactionScope())
            {
                try
                {
                    var description = "[审核人:" + CurrentSession.Personnel.Get().RealName + " " + DateTime.Now + " 审核备注：" + TB_Description.Text + "]";
                    Guid reckoningId = WebControl.GetGuidFromQueryString("ReckoningId");
                    var info = _reckoning.GetReckoning(reckoningId);
                    var reckoningType = info.ReckoningType;
                    _reckoning.UpdateDescription(info.ReckoningId, description);
                    _reckoning.Auditing(TradeCodeT);
                    _wasteBook.UpdateDescription(info.ReckoningId, description);
                    _wasteBook.Auditing(TradeCodeT);
                    #region
                    ReckoningInfo reckoningInfo = _reckoning.GetReckoning(reckoningId);
                    if (FilialeHelper.IsEntityShop(reckoningInfo.ThirdCompanyID))
                    {
                        var companyId = reckoningInfo.FilialeId;
                        var filialeId = reckoningInfo.ThirdCompanyID;
                        var paymentPrice = -reckoningInfo.AccountReceivable;
                        reckoningInfo.ThirdCompanyID = companyId;
                        reckoningInfo.FilialeId = filialeId;
                        reckoningInfo.AccountReceivable = paymentPrice;
                        reckoningInfo.JoinTotalPrice = paymentPrice;
                        if (NonceReckoningType == ReckoningType.Income)
                        {
                            reckoningInfo.ReckoningType = (int)ReckoningType.Income;
                            reckoningInfo.Description = WebControl.RetrunUserAndTime("[" + TB_CompanyName.Text + "]" + "[收到][" + RCB_FilialeList.SelectedItem.Text + "]");
                        }
                        else
                        {
                            reckoningInfo.ReckoningType = (int)ReckoningType.Defray;
                            reckoningInfo.Description = WebControl.RetrunUserAndTime("[" + TB_CompanyName.Text + "]" + "[付款][" + RCB_FilialeList.SelectedItem.Text + "]");
                        }
                        ReckoningSao.AddShopFrontReckoning(reckoningInfo.FilialeId, reckoningInfo);
                        // 扣除手续费
                        decimal poundage = string.IsNullOrEmpty(TB_HC.Text) ? 0 : Convert.ToDecimal(TB_HC.Text);
                        if (poundage > 0 && NonceReckoningType == ReckoningType.Defray)
                        {
                            ShopSao.InsertRecord(reckoningInfo.FilialeId, reckoningInfo.FilialeId, reckoningInfo.TradeCode,
                                -poundage, false, (int)TurnoverType.Pay, reckoningInfo.Description);
                        }
                        else if (NonceReckoningType == ReckoningType.Defray)
                        {
                            ShopSao.InsertRecord(reckoningInfo.FilialeId, reckoningInfo.FilialeId,
                                reckoningInfo.TradeCode, -reckoningInfo.AccountReceivable, false, (int)TurnoverType.Pay, reckoningInfo.Description);
                        }
                        else
                        {
                            ShopSao.InsertRecord(reckoningInfo.FilialeId, reckoningInfo.FilialeId, reckoningInfo.TradeCode,
                                reckoningInfo.AccountReceivable, false, (int)TurnoverType.Income, reckoningInfo.Description);
                        }
                    }
                    #endregion
                    //审核增加操作记录
                    EnumPointAttribute currentAccountState = reckoningType == (int)ReckoningType.Income ? OperationPoint.ReckongingManage.IncreaseAuditing.GetBusinessInfo() : OperationPoint.ReckongingManage.SubtractAuditing.GetBusinessInfo();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.ReckoningId, reckoningInfo.TradeCode, currentAccountState, string.Empty);

                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    ts.Complete();
                }
                catch
                {
                    RAM.Alert("审核失败！");
                }
            }
        }
        #endregion

        #region[选择公司后加载出对应的银行账号]
        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ShowBankNameAndAccountsName(new Guid(e.Value.Trim()));
        }
        #endregion

        protected void ShowBankNameAndAccountsName(Guid filialeId)
        {
            var banklist = _bankAccountDao.GetListByTargetId(filialeId).ToList();
            if (!banklist.Any() || filialeId == Guid.Empty)
            {
                RCB_BankAccountsId.Items.Clear();
                RCB_BankAccountsId.DataSource = new List<BankAccountInfo>();
                RCB_BankAccountsId.DataBind();
            }
            else
            {
                RCB_BankAccountsId.Items.Clear();
                for (var i = 0; i < banklist.Count; i++)
                {
                    RCB_BankAccountsId.Items.Insert(i, new RadComboBoxItem(banklist[i].BankName + " - " + banklist[i].AccountsName, banklist[i].BankAccountsId.ToString()));
                }
            }
        }
    }
}