using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using ERP.BLL.Implement.FinanceModule;
using ERP.BLL.Interface;
using ERP.Enum.RefundsMoney;
using ERP.Model.RefundsMoney;
using MIS.Enum;

namespace ERP.UI.Web.RefundsMoney
{
    /// <summary>
    /// 退款打款——编辑
    /// </summary>
    public partial class RefundsMoney_Edit : WindowsPage
    {
        //TODO:待定
        private readonly RefundsMoneyManager _refundsMoneySerivce = new RefundsMoneyManager();

        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);

        #region 属性

        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }

        /// <summary>
        /// 资金账户列表属性
        /// </summary>
        protected Dictionary<Guid, string> BankAccountInfoDic
        {
            get
            {
                if (ViewState["BankAccountInfoDic"] == null)
                {
                    var saleFilialeId = string.IsNullOrEmpty(Request.QueryString["SaleFilialeId"]) ? Guid.Empty.ToString() : Request.QueryString["SaleFilialeId"];
                    var bankAccountInfoDic = _bankAccounts.GetBankAccountsList(new Guid(saleFilialeId), Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                    bankAccountInfoDic.Add(new Guid("7175FA90-1214-46E7-8D15-F9053E16928C"), "支付宝【zfbahkd@keede.cn】");
                    return bankAccountInfoDic;
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
            }
        }

        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ApplyId"]) && !string.IsNullOrEmpty(Request.QueryString["SalePlatformId"]))
                {
                    ApplyId = new Guid(Request.QueryString["ApplyId"]);
                    SalePlatformId = new Guid(Request.QueryString["SalePlatformId"]);

                    BindData();
                }
                else
                {
                    BT_Back.Enabled = false;
                    BT_Pass.Enabled = false;
                }
            }
        }

        #region 绑定数据

        /// <summary>
        /// 绑定数据
        /// </summary>
        public void BindData()
        {
            var model = _refundsMoneySerivce.GetRefundsMoneyByID(ApplyId);

            txt_AfterSalesNumber.Text = model.AfterSalesNumber;
            txt_OrderNumber.Text = model.OrderNumber;
            txt_ThirdPartyOrderNumber.Text = model.ThirdPartyOrderNumber;
            txt_ThirdPartyAccountName.Text = model.ThirdPartyAccountName;
            txt_RefundsAmount.Text = model.RefundsAmount.ToString("#.00");

            txt_BankAccountNo.Text = model.BankAccountNo;
            txt_BankName.Text = model.BankName;
            txt_UserName.Text = model.UserName;

            txt_Fees.Text = model.Fees.ToString();
            txt_TransactionNumber.Text = model.TransactionNumber;
            txt_RejectReason.Text = model.RejectReason;

            if (model.Status != (int)RefundsMoneyStatusEnum.PendingPayment)
            {
                txt_Fees.Enabled = false;
                txt_TransactionNumber.Enabled = false;
                RCB_AccountID.Enabled = false;
                txt_RejectReason.Enabled = false;

                BT_Pass.Visible = false;
                BT_Back.Visible = false;
            }

            LoadSaleFilialeData();
            LoadSalePlatformData(model.SaleFilialeId);

            RCB_SaleFiliale.SelectedValue = model.SaleFilialeId.ToString();
            RCB_SalePlatform.SelectedValue = model.SalePlatformId.ToString();

            LoadBankAccountData();
            RCB_AccountID.SelectedValue = model.AccountID.ToString();
        }

        /// <summary>
        /// 根据销售公司获取资金账号
        /// </summary>
        protected void LoadBankAccountData()
        {
            RCB_AccountID.DataSource = BankAccountInfoDic;
            RCB_AccountID.DataTextField = "Value";
            RCB_AccountID.DataValueField = "Key";
            RCB_AccountID.DataBind();
            RCB_AccountID.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 获取销售公司名称
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        protected string GetSaleFilialeName(object saleFilialeId)
        {
            var info = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == new Guid(saleFilialeId.ToString()));
            return info != null ? info.Name : string.Empty;
        }

        /// <summary>
        /// 获取销售平台名称
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <returns></returns>
        protected string GetSalePlatformName(object salePlatformId)
        {
            var info = CacheCollection.SalePlatform.Get(new Guid(salePlatformId.ToString()));
            return info != null ? info.Name : string.Empty;
        }

        /// <summary>
        /// 销售公司
        /// </summary>
        public void LoadSaleFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList().Where(ent => ent.IsActive && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        /// <summary>
        /// 销售平台
        /// </summary>
        public void LoadSalePlatformData(Guid saleFilialeId)
        {
            RCB_SalePlatform.DataSource = CacheCollection.SalePlatform.GetListByFilialeId(saleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        #endregion 绑定数据

        #region 操作按钮

        //打款完成
        protected void BtnPassClick(object sender, EventArgs e)
        {
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                bool result = SaveData(true);
                if (!result)
                {
                    RAM.Alert("提交失败！");
                    return;
                }

                WebControl.AddOperationLog(Personnel.PersonnelId, Personnel.RealName, ApplyId, "",
                    OperationPoint.MemberWithdrawCash.PaySuccess.GetBusinessInfo(), string.Empty);
                ts.Complete();

                //TODO:待定

                //添加资金流

                //添加收款单红冲
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
        }

        //退回申请
        protected void BtnBackClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_RejectReason.Text.Trim()))
            {
                RAM.Alert("拒绝理由不能为空！");
                return;
            }

            bool result = SaveData(false);
            if (!result)
            {
                RAM.Alert("提交失败！");
                return;
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
        }

        /// <summary>
        /// 提交打款
        /// </summary>
        private bool SaveData(bool IsPayment)
        {
            RefundsMoneyInfo_Payment model = new RefundsMoneyInfo_Payment()
            {
                ID = ApplyId,

                AccountID = RCB_AccountID.SelectedValue.StrToGuid(),
                Fees = txt_Fees.Text.StrToDecimal(),
                TransactionNumber = txt_TransactionNumber.Text,
                RejectReason = txt_RejectReason.Text,

                IsPayment = IsPayment,
                ModifyUser = Personnel.RealName,
            };
            bool result = _refundsMoneySerivce.ApprovalPaymentRefundsMoney(model);
            return result;
        }

        #endregion 操作按钮

        #region [ViewState]

        protected Guid ApplyId
        {
            get
            {
                if (ViewState["ApplyId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["ApplyId"].ToString());
            }
            set
            {
                ViewState["ApplyId"] = value;
            }
        }

        protected Guid SalePlatformId
        {
            get
            {
                if (ViewState["SalePlatformId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["SalePlatformId"].ToString());
            }
            set
            {
                ViewState["SalePlatformId"] = value;
            }
        }

        #endregion [ViewState]
    }
}