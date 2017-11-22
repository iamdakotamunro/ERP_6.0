using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Enum;

/*
 * 最后修改人：陈重文
 * 修改时间：2013-10-22
 * 修改内容：代码优化,需求整改
 */
namespace ERP.UI.Web.Windows
{
    /// <summary>增加资金账户
    /// </summary>
    public partial class EditBankAccountsForm : WindowsPage
    {
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        private readonly IPaymentInterface _paymentInterfaceDao=new PaymentInterface(GlobalConfig.DB.FromType.Read);
        /// <summary>
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (Page.IsPostBack) return;
            if (!string.IsNullOrEmpty(Request.QueryString["BankAccountsId"]))
            {
                LB_Inster.Visible = false;
                BankAccountsId = new Guid(Request.QueryString["BankAccountsId"]);
            }
            else
            {
                LB_Update.Visible = false;
                Lab_UpdateSpace.Visible = false;
                BankAccountsId = Guid.Empty;

            }
        }

        /// <summary>
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            DDL_PaymentInterfaceId.DataSource = _paymentInterfaceDao.GetPaymentInterfaceList();
            DDL_PaymentInterfaceId.DataBind();
            DDL_PaymentInterfaceId.Items.Insert(0, new ListItem("", Guid.Empty.ToString()));
            var dics=EnumAttribute.GetDict<PaymentType>();
            if (dics.Count>0)
            {
                for (int i = 0; i < dics.Keys.Count; i++)
                {
                    DDL_PaymentType.Items.Insert(i, new ListItem(i == 0 ? "" : dics[i-1], string.Format("{0}", i-1)));
                }
            }
            
            //DDL_PaymentType.Items.Insert(0, new ListItem("", Convert.ToInt32(PaymentType.NoSet).ToString()));
            //DDL_PaymentType.Items.Insert(1, new ListItem("在线支付", Convert.ToInt32(PaymentType.OnLine).ToString()));
            //DDL_PaymentType.Items.Insert(2, new ListItem("传统帐号", Convert.ToInt32(PaymentType.Tradition).ToString()));
            //DDL_PaymentType.Items.Insert(3, new ListItem("邮局汇款", Convert.ToInt32(PaymentType.SwipeCard).ToString()));

            #region 账号类型 zal 2015-08-21
            var accountTypeList = EnumAttribute.GetDict<AccountType>().ToList();
            ddl_AccountType.DataTextField = "Value";
            ddl_AccountType.DataValueField = "Key";
            ddl_AccountType.DataSource = accountTypeList;
            ddl_AccountType.DataBind();
            #endregion

            NonceBankAccountsInfo = BankAccountsId != Guid.Empty ? _bankAccounts.GetBankAccounts(BankAccountsId) : new BankAccountInfo

            {
                PaymentType = (int)PaymentType.Tradition
            };
        }

        /// <summary> 编辑资金账户是记录资金账户编号
        /// </summary>
        private Guid BankAccountsId
        {
            get
            {
                return (Guid)ViewState["BankAccountsId"];
            }
            set
            {
                ViewState["BankAccountsId"] = value;
            }
        }

        protected SubmitController SubmitController;
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["EditBankAccountsForm"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["EditBankAccountsForm"] = SubmitController;
            }
            return (SubmitController)ViewState["EditBankAccountsForm"];
        }

        /// <summary>添加
        /// </summary>
        protected void Button_InsterGoods(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                return;
            }
            BankAccountInfo bankAccountsInfo = NonceBankAccountsInfo;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _bankAccounts.Insert(bankAccountsInfo);
                    BankAccount.Instance.Remove();
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    RAM.Alert("操作异常!" + ex.Message);
                    return;
                }
            }
            //TODO 建议配置
            var saleFiliales = CacheCollection.Filiale.GetHeadList().Where(ent => ent.FilialeTypes.Contains((int) FilialeType.SaleCompany));
            var testFilialeIds = new List<Guid>
            {
                new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                new Guid("444E0C93-1146-4386-BAE2-CB352DA70499")
            };
            var filialeB2CList = GlobalConfig.IsTestWebSite ? saleFiliales.Where(ent=> testFilialeIds.Contains(ent.ID)).ToList() :
                saleFiliales.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && !ent.ID.Equals(new Guid("ED58311F-FE6B-4CD9-85E9-FDA26EA209A0"))).ToList();
            foreach (var filialeInfo in filialeB2CList)
            {
                try
                {
                    bankAccountsInfo.TargetId = filialeInfo.ID;
                    B2CSao.AddBankAccount(bankAccountsInfo);
                }
                catch (Exception ex)
                {
                    RAM.Alert(filialeInfo.Name + "同步失败，错误信息：" + ex.Message);
                    return;
                }
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            SubmitController.Submit();
        }

        /// <summary>修改
        /// </summary>
        protected void Button_UpdateGoods(object sender, EventArgs e)
        {
            var bankAccountsInfo = NonceBankAccountsInfo;
            var bankAccountsList = _bankAccountDao.GetListByBankAccountId(bankAccountsInfo.BankAccountsId);
            if (bankAccountsList.Count > 0)
            {
                bankAccountsInfo.TargetId = bankAccountsList[0].TargetId;
                bankAccountsInfo.IsMain = bankAccountsList[0].IsMain;
            }
            try
            {
                _bankAccounts.Update(bankAccountsInfo);
                BankAccount.Instance.Remove();

                //TODO 建议配置
                var saleFiliales = CacheCollection.Filiale.GetHeadList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
                var testFilialeIds = new List<Guid>
                {
                    new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                    new Guid("444E0C93-1146-4386-BAE2-CB352DA70499")
                };
                var filialeB2CList = GlobalConfig.IsTestWebSite ? saleFiliales.Where(ent => testFilialeIds.Contains(ent.ID)).ToList() :
                    saleFiliales.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && !ent.ID.Equals(new Guid("ED58311F-FE6B-4CD9-85E9-FDA26EA209A0"))).ToList();

                foreach (var filialeInfo in filialeB2CList)
                {
                    bankAccountsInfo.TargetId = filialeInfo.ID;
                    B2CSao.UpdateBankAccount(bankAccountsInfo);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch
            {
                RAM.Alert("更改资金账户信息失败！");
            }
        }

        /// <summary>界面控件组对象化
        /// </summary>
        private BankAccountInfo NonceBankAccountsInfo
        {
            get
            {
                Guid bankAccountsId = BankAccountsId == Guid.Empty ? Guid.NewGuid() : BankAccountsId;
                string bankName = TB_BankName.Text;
                var paymentInterfaceId = new Guid(DDL_PaymentInterfaceId.SelectedValue);
                string accountsName = TB_AccountsName.Text;
                string accounts = TB_Accounts.Text;
                string accountsKey = string.IsNullOrEmpty(TB_AccountsKey.Text) ? null : TB_AccountsKey.Text;
                int paymentType = Convert.ToInt32(DDL_PaymentType.SelectedValue);
                string bankIcon;
                if (RU_BankIcon.UploadedFiles.Count > 0)
                {
                    bankIcon = RU_BankIcon.TargetFolder + RU_BankIcon.UploadedFiles[0].GetName();
                }
                else
                {
                    bankIcon = string.IsNullOrEmpty(HF_BankIcon.Value) ? null : HF_BankIcon.Value;
                }
                string description = RE_Description.Content;
                bool isUse = CB_IsUse.Checked;
                bool isFinish = CB_IsFinish.Checked;
                bool isBacktrack = CB_IsBacktrack.Checked;
                int accountType = Convert.ToInt32(ddl_AccountType.SelectedValue);//银行账户
                var bankAccountsInfo = new BankAccountInfo
                    {
                        BankAccountsId = bankAccountsId,
                        BankName = bankName,
                        PaymentInterfaceId = paymentInterfaceId,
                        AccountsName = accountsName,
                        Accounts = accounts,
                        AccountsKey = accountsKey,
                        PaymentType = paymentType,
                        BankIcon = bankIcon,
                        Description = description,
                        IsUse = isUse,
                        IsFinish = isFinish,
                        IsBacktrack = isBacktrack,
                        IsDisplay = CbIsDisplay.Checked,
                        AccountType = accountType//银行账户
                    };
                return bankAccountsInfo;
            }
            set
            {
                TB_BankName.Text = value.BankName;
                DDL_PaymentInterfaceId.SelectedValue = value.PaymentInterfaceId.ToString();
                TB_AccountsName.Text = value.AccountsName;
                TB_Accounts.Text = value.Accounts;
                TB_AccountsKey.Text = value.AccountsKey ?? "";
                DDL_PaymentType.SelectedValue = string.Format("{0}", value.PaymentType);
                if (string.IsNullOrEmpty(value.BankIcon))
                {
                    BankImg.Visible = false;
                }
                else
                {
                    BankImg.ImageUrl = value.BankIcon;
                    HF_BankIcon.Value = value.BankIcon;
                }
                RE_Description.Content = value.Description;
                CB_IsUse.Checked = value.IsUse;
                CB_IsFinish.Checked = value.IsFinish;
                CB_IsBacktrack.Checked = value.IsBacktrack;
                CbIsDisplay.Checked = value.IsDisplay;
                ddl_AccountType.SelectedValue = string.Format("{0}", value.AccountType);//银行账户
            }
        }
    }
}