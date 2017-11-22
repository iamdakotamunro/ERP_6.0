using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class PrintReckoningCost : WindowsPage
    {
        private readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        readonly ICostReckoning _costReckoningDao = new CostReckoning(GlobalConfig.DB.FromType.Write);

        readonly DateTime _dateTime = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                NonceReckoning = _costReckoningDao.GetReckoning(ReckoningId);
            }
        }



        private Guid ReckoningId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("ReckoningId");
            }
        }

        private CostReckoningInfo NonceReckoning
        {
            set
            {
                if (value == null) value = new CostReckoningInfo();
                Lit_TradeCode.Text = value.TradeCode;
                Lit_Filiale.Text = CurrentSession.Filiale.Get().Name;
                Lit_Company.Text = _costCussentDao.GetCompanyCussent(value.CompanyId).CompanyName;
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
                    //case ReckoningType.AccountGathering:
                    //    Lab_WinTitle.Text = "收款单";
                    //    Lit_ReckoningType.Text = "收款";
                    //    break;
                    //case ReckoningType.AccountPayable:
                    //    Lab_WinTitle.Text = "应付款单";
                    //    Lit_ReckoningType.Text = "应付";
                    //    break;
                    //case ReckoningType.AccountPayment:
                    //    Lab_WinTitle.Text = "付款单";
                    //    Lit_ReckoningType.Text = "付款";
                    //    break;
                    //case ReckoningType.AccountReceivable:
                    //    Lab_WinTitle.Text = "应收款单";
                    //    Lit_ReckoningType.Text = "应收";
                    //    break;
                    //case ReckoningType.AddBankroll:
                    //    Lab_WinTitle.Text = "应收增加调帐单";
                    //    Lit_ReckoningType.Text = "增加";
                    //    break;
                    //case ReckoningType.DecreaseBankroll:
                    //    Lab_WinTitle.Text = "应收减少调帐单";
                    //    Lit_ReckoningType.Text = "减少";
                    //    break;
                }
                Lit_AccountReceivable.Text = (WebControl.CurrencyValue(Math.Abs(value.AccountReceivable))) + WebControl.GetUnitsBySellType((int)SellType.Currency);
                Lit_DateCreated.Text = string.Format("{0}",value.DateCreated);
                rtbDescriptionAuditing.Text = value.Description;

                Lit_TradeCode2.Text = value.TradeCode;
                Lit_Filiale2.Text = CurrentSession.Filiale.Get().Name;
                Lit_Company2.Text = _costCussentDao.GetCompanyCussent(value.CompanyId).CompanyName;
                Lit_DateCreated2.Text = string.Format("{0}",value.DateCreated);
                Lit_AccountReceivable2.Text = string.Format("{0}",(WebControl.CurrencyValue(Math.Abs(value.AccountReceivable))));
                Lit_Description3.Text = value.Description;

                ReckoningIdT = value.ReckoningId;
                TradeCodeT = value.TradeCode;
                AccountReceivableForT = value.AccountReceivable;

                //不可编辑
                if (value.AuditingState == 1)
                {
                    yesAuditing.Visible = true;
                    noAuditing.Visible = false;
                }
                else//可编辑
                {
                    yesAuditing.Visible = false;
                    noAuditing.Visible = true;
                }
            }
            get
            {
                return _costReckoningDao.GetReckoning(ReckoningId);
            }
        }

        public static Guid ReckoningIdT
        {
            get;
            set;
        }

        public static string TradeCodeT
        {
            get;
            set;
        }

        public static decimal AccountReceivableForT
        {
            get;
            set;
        }

        public static DateTime DateCreatedT
        {
            get;
            set;
        }

        //保存
        protected void Button_UpdateTransfer(object sender, EventArgs e)
        {
            string description = WebControl.RetrunUserAndTime(Lit_Description2.Text);
            if (NonceReckoning.AuditingState == (int)AuditingState.No)
            {
                if (AccountReceivableForT < 0)
                {
                    _costReckoningDao.Update(ReckoningIdT, -Convert.ToDecimal(Lit_AccountReceivable2.Text), Lit_Description3.Text + description, _dateTime);
                }
                if (AccountReceivableForT > 0)
                {
                    _costReckoningDao.Update(ReckoningIdT, Convert.ToDecimal(Lit_AccountReceivable2.Text), Lit_Description3.Text + description, _dateTime);
                    _wasteBook.UpdateForReckoningCost(TradeCodeT, -Convert.ToDouble(Lit_AccountReceivable2.Text), Lit_Description3.Text + description, WebControl.GetNowTime());
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
                if (NonceReckoning.AuditingState == (int)AuditingState.Yes)
                {
                    _costReckoningDao.UpdateDescription(ReckoningIdT, WebControl.RetrunUserAndTime(txtDescription.Text.Trim()));
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
        }

        //审核
        protected void Button_Auditing(object sender, EventArgs e)
        {
            try
            {
                _costReckoningDao.Auditing(TradeCodeT);

                _wasteBook.Auditing(TradeCodeT);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception exp)
            {
                RAM.Alert("审核失败！ \n\n消息提示：'" + exp.Message);
            }
        }

        //删除
        protected void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                _costReckoningDao.Delete(TradeCodeT);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception exp)
            {
                RAM.Alert("删除失败！ \n\n消息提示：'" + exp.Message);
            }
        }
    }
}
