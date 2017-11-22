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
using ERP.Enum.SubsidyPayment;
using ERP.Model.SubsidyPayment;
using MIS.Enum;

namespace ERP.UI.Web.SubsidyPayment
{
    /// <summary>
    /// 补贴审核——编辑
    /// </summary>
    public partial class Check_Edit : WindowsPage
    {
        //TODO:待定
        private readonly SubsidyPaymentManager _subsidyPaymentSerivce = new SubsidyPaymentManager();

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
            var model = _subsidyPaymentSerivce.GetSubsidyPaymentByID(ApplyId);

            txt_OrderNumber.Text = model.OrderNumber;
            txt_ThirdPartyOrderNumber.Text = model.ThirdPartyOrderNumber;
            txt_ThirdPartyAccountName.Text = model.ThirdPartyAccountName;
            txt_RefundsAmount.Text = model.OrderAmount.ToString("#.00");

            txt_BankAccountNo.Text = model.BankAccountNo;
            txt_BankName.Text = model.BankName;
            txt_UserName.Text = model.UserName;

            txt_SubsidyAmount.Text = model.SubsidyAmount.ToString("#.00");

            txt_RejectReason.Text = model.RejectReason;

            if (model.Status != (int)SubsidyPaymentStatusEnum.PendingFinanceCheck)
            {
                txt_RejectReason.Enabled = false;

                BT_Pass.Visible = false;
                BT_Back.Visible = false;
            }

            LoadSaleFilialeData();
            LoadSalePlatformData(model.SaleFilialeId);

            RCB_SaleFiliale.SelectedValue = model.SaleFilialeId.ToString();
            RCB_SalePlatform.SelectedValue = model.SalePlatformId.ToString();

            LoadSubsidyTypeData();
            RCB_SubsidyType.SelectedValue = model.SubsidyType.ToString();


            RCB_QuestionType.SelectedValue = model.QuestionType.ToString();

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

        /// <summary>
        /// 补贴类型状态
        /// </summary>
        public void LoadSubsidyTypeData()
        {
            IDictionary<int, string> di = new Dictionary<int, string>();
            di.Add(0, "");
            di.Add((int)SubsidyTypeEnum.Compensate, "补偿");
            di.Add((int)SubsidyTypeEnum.Gift, "赠送");

            RCB_SubsidyType.DataSource = di;
            RCB_SubsidyType.DataTextField = "Value";
            RCB_SubsidyType.DataValueField = "Key";
            RCB_SubsidyType.DataBind();
        }
        #endregion 绑定数据

        #region 操作按钮

        //核准
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
                
            }
            RAM.ResponseScripts.Add("CloseAndRebind()");
        }

        //核退
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
        /// 审核
        /// </summary>
        private bool SaveData(bool IsApproved)
        {
            SubsidyPaymentInfo_Check model = new SubsidyPaymentInfo_Check()
            {
                ID = ApplyId,
                
                RejectReason = txt_RejectReason.Text,

                IsApproved = IsApproved,
                ModifyUser = Personnel.RealName,
            };
            bool result = _subsidyPaymentSerivce.CheckSubsidyPayment(model);
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