using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;

// 最后修改人：刘彩军
// 修改时间：2011-August-10th
// 修改内容：代码优化
namespace ERP.UI.Web.Windows
{
    public partial class PrintWasteBook : WindowsPage
    {
        private static readonly IWasteBook _wasteBook=new WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly BankAccountManager _bankAccountManager = new BankAccountManager(new BankAccounts(GlobalConfig.DB.FromType.Write),
            new BankAccountDao(GlobalConfig.DB.FromType.Read),_wasteBook);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NonceWasteBook = DateCreated != DateTime.MinValue ? _wasteBook.GetWasteBook(WasteBookId, DateCreated, GlobalConfig.KeepYear)
                    : _wasteBook.GetWasteBook(WasteBookId);
            }
            Page.Title = "修改资金记录单据";
        }

        private Guid WasteBookId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("WasteBookId");
            }
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

        private WasteBookInfo NonceWasteBook
        {
            set
            {
                if (value == null) value = new WasteBookInfo();
                var firstOrDefault = MISService.GetAllFiliales().FirstOrDefault(f => f.ID == WebControl.GetGuidFromQueryString("FilialeId"));
                Lit_filialeId.Text = firstOrDefault != null ? firstOrDefault.Name : "未知公司";
                Lab_WinTitle.Text = value.Income >= 0 ? "资金收入单" : "资金支出单";
                Lab_StateTitle.Text = value.AuditingState > 0 ? "已审核" : "未审核";
                Lit_TradeCode.Text = value.TradeCode;
                Lit_BankAccounts.Text = _bankAccountManager.GetBankAccountName(value.BankAccountsId);
                Lit_IncomeType.Text = value.Income >= 0 ? "收入" : "支出";
                Lit_Income.Text = WebControl.NumberSeparator(WebControl.CurrencyValue(Math.Abs(value.Income))) + WebControl.GetUnitsBySellType((int)SellType.Currency);
                Lit_DateCreated.Text = string.Format("{0}", value.DateCreated);
                txtOldDescriptionAuditing.Text = value.Description;

                Lab_StateTitle2.Text = value.AuditingState > 0 ? "已审核" : "未审核";
                Lab_WinTitle2.Text = value.Income >= 0 ? "资金收入单" : "资金支出单";
                Lit_DateCreated2.Text = string.Format("{0}", value.DateCreated);
                txtOldDescriptionNoAuditing.Text = value.Description;
                Lit_BankAccounts2.Text = _bankAccountManager.GetBankAccountName(value.BankAccountsId);
                Lit_Income2.Text = WebControl.NumberSeparator(WebControl.CurrencyValue(Math.Abs(value.Income)));

                _wasteBookIdT = value.WasteBookId;
                _tradeCodeT = value.TradeCode;
                _bankAccountsIdT = value.BankAccountsId;

                decimal poundage = Math.Abs(_wasteBook.GetPoundage(value.TradeCode));
                _poundageT = poundage;
                Lit_Poundage.Text = Convert.ToString(poundage);
                decimal tradeCodeNum = _wasteBook.GetTradeCodeNum(value.TradeCode);

                //不可编辑
                if (value.AuditingState == 1 || (value.WasteBookId.ToString() == _wasteBook.GetWasteBookId(value.TradeCode)) || (Lab_WinTitle.Text == "资金收入单" && tradeCodeNum != 1))
                {
                    yesAuditing.Visible = true;
                    noAuditing.Visible = false;
                }
                else//可编辑
                {
                    yesAuditing.Visible = false;
                    noAuditing.Visible = true;
                }

                if (tradeCodeNum == 1)
                {
                    Lit_Poundage.Visible = false;
                }
            }
            get
            {
                return _wasteBook.GetWasteBook(WasteBookId);
            }
        }

        //获取BankAccountsId
        private static Guid _bankAccountsIdT;
        public static Guid BankAccountsIdT
        {
            get { return _bankAccountsIdT; }
            set { _bankAccountsIdT = value; }
        }

        //获取poundage
        private static decimal _poundageT;
        public static decimal PoundageT
        {
            get { return _poundageT; }
            set { _poundageT = value; }
        }

        //获取wasteBookId
        private static Guid _wasteBookIdT;
        public static Guid WasteBookIdT
        {
            get { return _wasteBookIdT; }
            set { _wasteBookIdT = value; }
        }

        //获取tradecode
        private static string _tradeCodeT;
        public static string TradeCodeT
        {
            get { return _tradeCodeT; }
            set { _tradeCodeT = value; }
        }

        //保存更改
        protected void Button_UpdateTransfer(object sender, EventArgs e)
        {
            LB_Update.Enabled = false;
            string description = WebControl.RetrunUserAndTime(txtOldDescriptionNoAuditing.Text.Trim() + txtNewDescriptionNoAuditing.Text.Trim());
            decimal income = Convert.ToDecimal(Lit_Income2.Text);
            decimal poundage = string.IsNullOrEmpty(Lit_Poundage.Text) ? 0 : Convert.ToDecimal(Lit_Poundage.Text);
            if (NonceWasteBook.AuditingState == (int)AuditingState.Yes)
            {
                _wasteBook.UpdateDescription(WasteBookId, WebControl.RetrunUserAndTime(txtNewDescriptionAuditing.Text.Trim()));
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                return;
            }
            _wasteBook.UpdateDescription(WasteBookId, WebControl.RetrunUserAndTime(txtNewDescriptionAuditing.Text.Trim()));
            if (income <= 0)
            {
                RAM.Alert("转移资金额不正确!");
                LB_Update.Enabled = true;
            }
            else
            {
                if (poundage < 0)
                {
                    RAM.Alert("手续费金额不正确！");
                    LB_Update.Enabled = true;
                }
                else
                {
                    if (poundage > 0)
                    {
                        if (income > 10)
                        {
                            if (!WebControl.CheckPoundage(income, poundage))
                            {
                                RAM.Alert("手续费必须小于等于6或者不超过付款金额的2%！");
                                LB_Update.Enabled = true;
                                return;
                            }
                        }
                    }

                    try
                    {
                        if (PoundageT == 0)
                        {
                            if (poundage == 0)
                            {
                                _bankAccountManager.UpdateBll(WasteBookIdT, description, income, TradeCodeT, poundage, BankAccountsIdT);
                            }
                            if (poundage > 0)
                            {
                                _bankAccountManager.UpdateBll(WasteBookIdT, description, income, TradeCodeT, poundage, BankAccountsIdT);
                                _bankAccountManager.InsertPoundage(BankAccountsIdT, TradeCodeT, poundage, WebControl.GetGuidFromQueryString("FilialeId"));
                            }
                        }
                        if (PoundageT > 0)
                        {
                            if (poundage == 0)
                            {
                                _bankAccountManager.UpdateBll(WasteBookIdT, description, income, TradeCodeT, poundage, BankAccountsIdT);
                                PoundageT = -PoundageT;
                                _wasteBook.DeleteWasteBookPoundage(TradeCodeT, PoundageT);
                            }
                            if (poundage > 0)
                            {
                                _bankAccountManager.UpdateBll(WasteBookIdT, description, income, TradeCodeT, poundage, BankAccountsIdT);
                                _bankAccountManager.UpdatePoundage(TradeCodeT, poundage);
                            }
                        }
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    catch (Exception exp)
                    {
                        RAM.Alert("保存失败！ \n\n消息提示:" + exp.Message);
                    }
                }
            }
        }

        //删除
        protected void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                _wasteBook.DeleteWasteBook(TradeCodeT);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception exp)
            {
                RAM.Alert("删除失败！ \n\n消息提示：'" + exp.Message);
            }
        }
    }

}