using System;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class PrintReckoning : WindowsPage
    {
        private readonly IWasteBook _wasteBook=new WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent=new CompanyCussent(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                NonceReckoning = DateCreated != DateTime.MinValue
                    ? _reckoning.GetReckoning(ReckoningId, DateCreated, GlobalConfig.KeepYear)
                    : _reckoning.GetReckoning(ReckoningId);
            }
            Page.Title = "修改单位往来账单据";
        }

        protected DateTime DateCreated
        {
            get
            {
                if (ViewState["DateCreated"] == null)
                {
                    if (Request.QueryString["DateCreated"] == null)
                    {
                        return DateTime.MinValue;
                    }
                    ViewState["DateCreated"] = Request.QueryString["DateCreated"];
                }
                return DateTime.Parse(ViewState["DateCreated"].ToString());
            }
        }

        private Guid ReckoningId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("ReckoningId");
            }
        }

        private ReckoningInfo NonceReckoning
        {
            set
            {
                if (value == null) value = new ReckoningInfo();
                Lit_TradeCode.Text = value.TradeCode;
                var firstOrDefault = CacheCollection.Filiale.GetName(value.FilialeId);
                Lit_Filiale.Text = !string.IsNullOrEmpty(firstOrDefault) ? firstOrDefault : "ERP";
                Lit_Filiale2.Text = !string.IsNullOrEmpty(firstOrDefault) ? firstOrDefault : "ERP";
                var companyInfo = _companyCussent.GetCompanyCussent(value.ThirdCompanyID);
                if (companyInfo != null)
                {
                    Lit_Company.Text = companyInfo.CompanyName;
                    Lit_Company2.Text = companyInfo.CompanyName;
                }
                else
                {
                    var companyName= CacheCollection.Filiale.GetName(value.ThirdCompanyID);
                    Lit_Company.Text = companyName;
                    Lit_Company2.Text = companyName;
                }
                switch ((ReckoningType)value.ReckoningType)
                {
                    case ReckoningType.Income:
                        Lab_WinTitle.Text = "收款单";
                        Lit_ReckoningType.Text = "收款";
                        break;
                    case ReckoningType.Defray:
                        Lab_WinTitle.Text = "付款单";
                        Lit_ReckoningType.Text = "付款";
                        break;
                }
                Lit_AccountReceivable.Text = (WebControl.CurrencyValue(Math.Abs(value.AccountReceivable))) + WebControl.GetUnitsBySellType((int)SellType.Currency);
                Lit_DateCreated.Text = string.Format("{0}",value.DateCreated);
                txtOldDescriptionAuditing.Text = value.Description;
                // +WebControl.GetCurrency(WebControl.WebRudder.DefaultCurrencyId).CurrencyUnits;
                Lit_AccountReceivable2.Text = string.Format("{0}",(WebControl.CurrencyValue(Math.Abs(value.AccountReceivable))));
                Lit_DateCreated2.Text = string.Format("{0}",value.DateCreated);
                txtOldDescriptionNoAuditing.Text = value.Description;

                //根据tradecode的条数确定一个wastebook记录有没有手续费                
                decimal tradeCodeNum = _wasteBook.GetTradeCodeNum(value.TradeCode);
                if (tradeCodeNum == 2)
                {
                    _poundage = Math.Abs(Convert.ToDecimal(_wasteBook.GetPoundageForReckoning(value.TradeCode)));
                }
                else
                    _poundage = 0;
                Lit_Poundage2.Text = string.Format("{0}",_poundage);
                //end

                _tradeCodeT = value.TradeCode;
                _reckoningIdT = value.ReckoningId;
                if (value.AuditingState == 1)
                {
                    yesAuditing.Visible = true;
                    noAuditing.Visible = false;
                }
                else
                    if (value.AuditingState == 0)
                    {
                        yesAuditing.Visible = false;
                        noAuditing.Visible = true;
                    }

                int reckoningType = _reckoning.GetReckoningType(value.ReckoningId);
                _reckoningTypeT = reckoningType;
                if (reckoningType == 1 || reckoningType == 2 || reckoningType == 3 || reckoningType == 5 || reckoningType == 6)
                {
                    Lit_Poundage2.Visible = false;
                }
            }
            get
            {
                return _reckoning.GetReckoning(ReckoningId);
            }
        }

        //获取Poundage
        private static decimal _poundage;
        public static decimal PoundageT
        {
            get { return _poundage; }
            set { _poundage = value; }
        }

        //获取tradecode
        private static string _tradeCodeT;
        public static string TradeCodeT
        {
            get { return _tradeCodeT; }
            set { _tradeCodeT = value; }
        }

        //获取reckoningType
        private static int _reckoningTypeT;
        public static int ReckoningTypeT
        {
            get { return _reckoningTypeT; }
            set { _reckoningTypeT = value; }
        }

        //获取ReckoningId
        private static Guid _reckoningIdT;
        public static Guid ReckoningIdT
        {
            get { return _reckoningIdT; }
            set { _reckoningIdT = value; }
        }

        //保存修改
        protected void Button_UpdateTransfer(object sender, EventArgs e)
        {
            DateTime dateCreated = DateTime.Now;
            string description = WebControl.RetrunUserAndTime(txtOldDescriptionNoAuditing.Text + txtNewDescriptionNoAuditing.Text.Trim());
            decimal receivable = Convert.ToDecimal(Lit_AccountReceivable2.Text);
            decimal poundage = string.IsNullOrEmpty(Lit_Poundage2.Text) ? 0 : Convert.ToDecimal(Lit_Poundage2.Text);
            int reckoningType = _reckoning.GetReckoningType(ReckoningIdT);

            if (NonceReckoning.AuditingState == (int)AuditingState.Yes)
            {
                _reckoning.UpdateDescription(ReckoningId, WebControl.RetrunUserAndTime(txtNewDescriptionAuditing.Text.Trim()));
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                return;
            }
            _reckoning.UpdateDescription(ReckoningId, WebControl.RetrunUserAndTime(txtNewDescriptionAuditing.Text.Trim()));
            if (receivable <= 0)
            {
                RAM.Alert("转移资金额不正确!");
                return;
            }
            if (reckoningType == 3)
            {
                receivable = -receivable;
            }
            if (poundage > 0)
            {
                if (receivable > 10)
                {
                    if (!WebControl.CheckPoundage(receivable, poundage))
                    {
                        RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                        return;
                    }
                }
            }

            if (ReckoningTypeT == 5 || ReckoningTypeT == 1)
            {
                try
                {
                    var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                    _reckoning.Update(reckoningInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("保存失败!");
                }
            }

            if (ReckoningTypeT == 6 || ReckoningTypeT == 2)
            {
                try
                {
                    var reckoningInfo = new ReckoningInfo(-receivable, description, ReckoningIdT, dateCreated);
                    _reckoning.Update(reckoningInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("保存失败!");
                }
            }

            if (ReckoningTypeT == 3)
            {
                try
                {
                    var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                    _reckoning.Update(reckoningInfo);
                    string wasteBookId = _wasteBook.GetWasteBookIdForUpdate(TradeCodeT);
                    var wastebookId = new Guid(wasteBookId);
                    var wasteBookInfo = new WasteBookInfo(wastebookId, dateCreated, description, -receivable, (int)WasteBookType.Decrease);
                    _wasteBook.Update(wasteBookInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("保存失败!");
                }
            }
            if (_reckoningTypeT == 4)
            {
                string wasteBookId = _wasteBook.GetWasteBookIdForUpdate(TradeCodeT);//wastebook中手续费的wastebookid
                var wastebookId = new Guid(wasteBookId);
                string wasteBookIdOther = _wasteBook.GetWasteBookIdForReckoning(TradeCodeT);//wastebook表中收付款的wastebookid
                var wastebookidOther = new Guid(wasteBookIdOther);
                decimal tradeCodeNum = _wasteBook.GetTradeCodeNum(TradeCodeT);
                PoundageT = -PoundageT;

                if (tradeCodeNum == 2)//有手续费
                {
                    if (poundage == 0)
                    {
                        //delete
                        _wasteBook.DeleteWasteBookPoundage(TradeCodeT, PoundageT);
                        if (description.LastIndexOf("：", StringComparison.Ordinal) > -1)
                        {
                            description = description.Substring(0, description.LastIndexOf("手", StringComparison.Ordinal));
                        }
                        var wasteBookInfo = new WasteBookInfo(wastebookidOther, dateCreated, description, -receivable, (int)WasteBookType.Decrease);
                        _wasteBook.Update(wasteBookInfo);
                        var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                        _reckoning.Update(reckoningInfo);
                    }
                    else
                    {
                        //update
                        poundage = -poundage;
                        _wasteBook.UpdatePoundageForReckoning(wastebookId, dateCreated, poundage);
                        if (description.LastIndexOf("：", StringComparison.Ordinal) > -1)
                        {
                            description = description.Substring(0, description.LastIndexOf("：", StringComparison.Ordinal));
                            description += "：" + (-poundage);
                        }
                        var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                        _reckoning.Update(reckoningInfo);
                        var wasteBookInfo = new WasteBookInfo(wastebookidOther, dateCreated, description, -receivable, (int)WasteBookType.Decrease);
                        _wasteBook.Update(wasteBookInfo);
                    }
                }

                if (tradeCodeNum == 1 && poundage!=0)
                {
                    //insert
                    Guid bankAccountsId = _wasteBook.GetWasteBook(wastebookidOther).BankAccountsId;
                    //decimal nonceBalance = wasteBook.GetBalance(bankAccountsId);
                    const string PD_DESC = "[转出] [手续费]";
                    var wasteBookInfoIn = new WasteBookInfo(Guid.NewGuid(), bankAccountsId, TradeCodeT, PD_DESC,
                                                                      -poundage, (int) AuditingState.No,
                                                                      (int) WasteBookType.Decrease,
                                                                      NonceReckoning.FilialeId)
                                                        {
                                                            LinkTradeCode = TradeCodeT,
                                                            LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                                            BankTradeCode = string.Empty,
                                                            State = (int)WasteBookState.Currently,
                                                            IsOut = false
                                                        };
                    _wasteBook.Insert(wasteBookInfoIn);
                    description += " [手续费]：" + poundage;

                    var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                    _reckoning.Update(reckoningInfo);
                    var wasteBookInfo = new WasteBookInfo(wastebookidOther, dateCreated, description, -receivable, (int)WasteBookType.Decrease);
                    _wasteBook.Update(wasteBookInfo);
                }
                else
                {
                    var reckoningInfo = new ReckoningInfo(receivable, description, ReckoningIdT, dateCreated);
                    _reckoning.Update(reckoningInfo);
                    var wasteBookInfo = new WasteBookInfo(wastebookidOther, dateCreated, description, -receivable, (int)WasteBookType.Decrease);
                    _wasteBook.Update(wasteBookInfo);
                }
                try
                {
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("保存失败!");
                }
            }
        }

        //删除
        protected void Button_Delete(object sender, EventArgs e)
        {
            _reckoning.Delete(TradeCodeT);
            _wasteBook.DeleteWasteBook(TradeCodeT);
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}