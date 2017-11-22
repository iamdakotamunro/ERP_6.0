using System;
using System.Linq;
using System.Transactions;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using MIS.Enum;

namespace ERP.UI.Web.Windows
{
    /// <summary>公司资金账号转移  ADD  2015-03-03  陈重文
    /// </summary>
    public partial class TransferBankAccountsForm : WindowsPage
    {
        private readonly IBankAccounts _bankAccountsWrite = new BankAccounts(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowData();
            }
        }

        private void LoadShowData()
        {
            //加载销售公司
            var list = CacheCollection.Filiale.GetHeadList();
            RCB_SaleFiliale.DataSource = list.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem("销售公司", Guid.Empty.ToString()));
            RCB_SaleFiliale.SelectedIndex = 0;

            //销售销售平台
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("销售平台", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;

            var bankAccountsId = Request.QueryString["BankAccountsId"];
            if (!string.IsNullOrWhiteSpace(bankAccountsId))
            {
                var bankInfo = _bankAccountsWrite.GetBankAccounts(new Guid(bankAccountsId));
                if (bankInfo != null && bankInfo.BankAccountsId != Guid.Empty)
                {
                    Lb_BankName.Text = bankInfo.BankName + "-" + bankInfo.AccountsName;
                }
                else
                {
                    Lb_BankName.Text = "-";
                }
            }
            else
            {
                RAM.Alert("系统提示：操作异常，请尝试刷新后继续操作！");
            }

        }

        /// <summary>
        /// </summary>
        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var radComboBox = o as RadComboBox;
            if (radComboBox == null) return;
            var rcbSaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            if (rcbSaleFilialeId == Guid.Empty)
            {
                RCB_SalePlatform.Items.Clear();
                RCB_SalePlatform.SelectedValue = Guid.Empty.ToString();
                return;
            }
            RCB_SalePlatform.DataSource = CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("销售平台", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
        }

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtSaveClick(object sender, EventArgs e)
        {
            var bankAccountsId = Request.QueryString["BankAccountsId"];
            if (!string.IsNullOrWhiteSpace(bankAccountsId))
            {
                var filialeId = new Guid(RCB_SaleFiliale.SelectedValue);
                if (filialeId == Guid.Empty)
                {
                    RAM.Alert("系统提示：请选择需要转移的销售公司");
                    return;
                }
                var salePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    _bankAccountsWrite.InsertBindBankAccounts(filialeId, new Guid(bankAccountsId));
                    if (salePlatformId != Guid.Empty)
                    {
                        _bankAccountsWrite.InsertBindBankAccounts(salePlatformId, new Guid(bankAccountsId));
                    }
                    var result = _bankAccountsWrite.SetBankAccountsIsMain(new Guid(bankAccountsId), true);
                    if (!result)
                    {
                        RAM.Alert("系统提示：操作异常，请尝试刷新再试！");
                        return;
                    }
                    var filialeIds = System.Configuration.ConfigurationManager.AppSettings["FilialeIds"];
                    if (filialeIds.ToLower().IndexOf(filialeId.ToString().ToLower(), StringComparison.Ordinal) == -1)
                    {
                        try
                        {
                            var filialeInfo = CacheCollection.Filiale.GetHeadList().First(ent => ent.ID == filialeId);
                            if (filialeInfo != null)
                            {
                                B2CSao.SetBankAccountsIsMain(filialeInfo.ID, new Guid(bankAccountsId), true);
                                B2CSao.AddBankAccountBinding(filialeId, new Guid(bankAccountsId));
                                if (salePlatformId != Guid.Empty)
                                {
                                    B2CSao.AddBankAccountBinding(salePlatformId, new Guid(bankAccountsId));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            RAM.Alert("系统同步异常，异常信息：" + ex.Message);
                            return;
                        }
                    }
                    ts.Complete();
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
    }
}