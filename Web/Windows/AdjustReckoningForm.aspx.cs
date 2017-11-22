using System;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using OperationLog.Core.Attributes;
using Telerik.Web.UI;

/*
 * 最后修改人：刘彩军
 * 修改时间：2011-september-28th
 * 修改内容：代码优化
 */
namespace ERP.UI.Web.Windows
{
    public partial class AdjustReckoningForm : WindowsPage
    {
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
        protected Boolean IsEdit
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["optype"]) && Request.QueryString["optype"] == "edit";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RCB_FilialeList.DataSource = CacheCollection.Filiale.GetHeadList();
                RCB_FilialeList.DataTextField = "Name";
                RCB_FilialeList.DataValueField = "ID";
                RCB_FilialeList.DataBind();
                RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
                if (IsEdit)
                {
                    ReckoningInfo reckoningInfo = _reckoning.GetReckoning(WebControl.GetGuidFromQueryString("ReckoningId"));
                    if (CacheCollection.Filiale.GetList().Any(f => f.ID == reckoningInfo.ThirdCompanyID))
                    {
                        var filialeInfo = CacheCollection.Filiale.GetList().FirstOrDefault(f => f.ID == reckoningInfo.ThirdCompanyID);
                        if (filialeInfo != null)
                            TB_CompanyName.Text = filialeInfo.Name;
                    }
                    else
                    {
                        var companyCussentInfo = _companyCussent.GetCompanyCussent(reckoningInfo.ThirdCompanyID);
                        if (companyCussentInfo == null)
                        {
                            RAM.Alert("不是有效的往来单位信息");
                            RAM.ResponseScripts.Add("CancelWindow();");
                            return;
                        }
                        TB_CompanyName.Text = companyCussentInfo.CompanyName;
                    }
                    TB_CompanyName.Enabled = false;
                    TB_PaymentPrice.Text = Math.Abs(reckoningInfo.AccountReceivable).ToString("0.##");
                    TB_PaymentPrice.Enabled = false;
                    TB_Description.Text = string.Empty;
                    if (reckoningInfo.ReckoningType == (int)ReckoningType.Income)
                    {
                        Page.Title = "应收增加";
                        NonceReckoningType = ReckoningType.Income;
                        LB_Delete.Visible = GetPowerOperationPoint("Delete");
                        LB_Inster.Visible = GetPowerOperationPoint("Increase");
                        Auditing.Visible = GetPowerOperationPoint("IncreaseAuditing");
                    }
                    else
                    {
                        Page.Title = "应收减少";
                        NonceReckoningType = ReckoningType.Defray;
                        LB_Delete.Visible = GetPowerOperationPoint("Delete");
                        LB_Inster.Visible = GetPowerOperationPoint("Receivable");
                        Auditing.Visible = GetPowerOperationPoint("SubtractAuditing");
                    }
                    TradeCodeT = reckoningInfo.TradeCode;
                    ReckoningId = reckoningInfo.ReckoningId;
                    var tempFilialeInfo = CacheCollection.Filiale.Get(reckoningInfo.FilialeId);
                    RCB_FilialeList.Items.Clear();
                    RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(tempFilialeInfo == null ? "ERP" : tempFilialeInfo.Name, reckoningInfo.FilialeId.ToString()));
                    RCB_FilialeList.SelectedIndex = 0;
                    RCB_FilialeList.Enabled = false;
                }
                else
                {
                    CompanyId = new Guid(Request.QueryString["CompanyId"]);
                    var filialeId = new Guid(Request.QueryString["FilialeId"]);
                    if (filialeId != Guid.Empty)
                    {
                        RCB_FilialeList.Items.Clear();
                        if (filialeId == _reckoningElseFilialeid)
                        {
                            RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("ERP", filialeId.ToString()));
                        }
                        else
                        {
                            var filialeInfo = CacheCollection.Filiale.Get(filialeId);
                            RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                        }
                        RCB_FilialeList.SelectedValue = filialeId.ToString();
                    }

                    if (CacheCollection.Filiale.GetList().Any(f => f.ID == CompanyId))
                    {
                        var filialeInfo = CacheCollection.Filiale.GetList().FirstOrDefault(f => f.ID == CompanyId);
                        if (filialeInfo != null)
                        {
                            TB_CompanyName.Text = filialeInfo.Name;
                            //note 如果是门店填写应收付款，公司则固定为父公司  2015-03-20  陈重文 
                            var parentFilialeInfo = CacheCollection.Filiale.Get(filialeInfo.ParentId);
                            if (parentFilialeInfo != null)
                            {
                                RCB_FilialeList.Items.Clear();
                                RCB_FilialeList.Items.Insert(0, new RadComboBoxItem(parentFilialeInfo.Name, parentFilialeInfo.ID.ToString()));
                                RCB_FilialeList.SelectedValue = parentFilialeInfo.ID.ToString();
                            }
                        }
                    }
                    else
                    {
                        var companyCussentInfo = _companyCussent.GetCompanyCussent(CompanyId);
                        if (companyCussentInfo == null)
                        {
                            RAM.Alert("不是有效的往来单位信息");
                            RAM.ResponseScripts.Add("CancelWindow();");
                            return;
                        }
                        TB_CompanyName.Text = companyCussentInfo.CompanyName;
                    }
                    if (Request.QueryString["ReckoningType"] == ReckoningType.Income.ToString())
                    {
                        Page.Title = "应收增加";
                        NonceReckoningType = ReckoningType.Income;
                        LB_Delete.Visible = false; //GetPowerOperationPoint("Delete");
                        LB_Inster.Visible = GetPowerOperationPoint("Increase");
                        Auditing.Visible = false; //GetPowerOperationPoint("IncreaseAuditing");
                    }
                    else
                    {
                        Page.Title = "应收减少";
                        NonceReckoningType = ReckoningType.Defray;
                        LB_Delete.Visible = false; //GetPowerOperationPoint("Delete");
                        LB_Inster.Visible = GetPowerOperationPoint("Receivable");
                        Auditing.Visible = false; //GetPowerOperationPoint("SubtractAuditing");
                    }
                }
            }
        }

        #region[取得用户操作权限]
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "CussentReckoningT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
        #endregion

        #region[公用属性]
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

        public static string TradeCodeT
        {
            get;
            set;
        }

        protected Guid ReckoningId
        {
            get
            {
                return ViewState["ReckoningId"] == null ? Guid.Empty : new Guid(ViewState["ReckoningId"].ToString());
            }
            set
            {
                ViewState["ReckoningId"] = value.ToString();
            }
        }
        #endregion

        #region[保存]
        protected void Button_InsterReckoning(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                Guid reckoningId = WebControl.GetGuidFromQueryString("ReckoningId");
                ReckoningInfo reckoningif = _reckoning.GetReckoning(reckoningId);
                string description = WebControl.RetrunUserAndTime(TB_Description.Text);
                decimal paymentPrice = Convert.ToDecimal(TB_PaymentPrice.Text);
                DateTime dateCreated = WebControl.GetNowTime();
                TradeCodeT = reckoningif.TradeCode;
                if (NonceReckoningType != ReckoningType.Income)
                {
                    paymentPrice = -paymentPrice;
                }
                if (TB_Description.Text == string.Empty)
                {
                    description = string.Empty;
                }
                var reckoningInfo = new ReckoningInfo(paymentPrice, description, reckoningId, dateCreated);
                try
                {
                    _reckoning.Update(reckoningInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("资金账户操作失败!");
                }
            }
            else
            {
                // 默认备注 
                string sBeforeDesc = Request.QueryString["ReckoningType"] == ReckoningType.Income.ToString() ? " [增加资金] " : " [减少资金] ";

                if (RCB_FilialeList.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择公司!");
                    return;
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                var description = sBeforeDesc + TB_Description.Text + " [申请人:" + personnelInfo.RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                decimal paymentPrice = Convert.ToDecimal(TB_PaymentPrice.Text);
                string tradeCode = (new CodeManager()).GetCode(CodeType.AJ);
                TradeCodeT = tradeCode;
                if (NonceReckoningType != ReckoningType.Income)
                {
                    paymentPrice = -paymentPrice;
                }
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, CompanyId, tradeCode, description, paymentPrice, (int)NonceReckoningType, (int)ReckoningStateType.Currently, (int)CheckType.NotCheck, (int)AuditingState.No, null, Guid.Empty)
                {
                    LinkTradeType = (int)ReckoningLinkTradeType.Other,
                    IsOut = new Guid(RCB_FilialeList.SelectedValue) != _reckoningElseFilialeid
                };
                try
                {
                    string errorMsg;
                    var result = _reckoning.Insert(reckoningInfo, out errorMsg);
                    if (result)
                    {
                        //往来账应收增加操作记录添加

                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, reckoningInfo.ReckoningId, reckoningInfo.TradeCode,
                            NonceReckoningType != ReckoningType.Income ? OperationPoint.ReckongingManage.IncomeAbatement.GetBusinessInfo() : OperationPoint.ReckongingManage.IncomeIncrease.GetBusinessInfo(), string.Empty);

                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    else
                    {
                        RAM.Alert("系统提示：" + sBeforeDesc + "，记录异常，请稍后重试！");
                    }
                }
                catch
                {
                    RAM.Alert("资金账户操作失败!");
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
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }
        #endregion

        #region[审核]
        //审核 add by lxm 20100325
        protected void Button_Auditing(object sender, EventArgs e)
        {
            try
            {
                if (TradeCodeT != string.Empty)
                {
                    var description = "[审核人:" + CurrentSession.Personnel.Get().RealName + ":" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 审核备注：" +
                             TB_Description.Text + "]";
                    var info = _reckoning.GetReckoning(ReckoningId);
                    var reckoningType = info.ReckoningType;
                    info.Description = info.Description + description;
                    info.AuditingState = (int)AuditingState.Yes;
                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        _reckoning.Delete(info.TradeCode);
                        string message;
                        var result = _reckoning.Insert(info, out message);
                        if (!result)
                        {
                            RAM.Alert("系统提示：审核失败！");
                            return;
                        }
                        ts.Complete();
                    }
                    //reckoning.Auditing(info.TradeCode);
                    //reckoning.UpdateDescription(info.ReckoningId, description);
                    //审核增加操作记录
                    EnumPointAttribute currentAccountState = reckoningType == (int)ReckoningType.Income ? OperationPoint.ReckongingManage.IncreaseAuditing.GetBusinessInfo() : OperationPoint.ReckongingManage.SubtractAuditing.GetBusinessInfo();
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.ReckoningId, info.TradeCode, currentAccountState, string.Empty);

                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
            catch
            {
                RAM.Alert("审核失败！");
            }
        }
        #endregion
    }
}