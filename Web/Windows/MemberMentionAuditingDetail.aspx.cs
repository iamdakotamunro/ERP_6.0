using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    /// <summary>余额流水
    /// </summary>
    public partial class MemberMentionAuditingDetail : WindowsPage
    {
        #region [SubmitController]

        protected SubmitController SubmitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Save"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["Save"] = SubmitController;
            }
            return (SubmitController)ViewState["Save"];
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                MemberId = string.IsNullOrEmpty(Request.QueryString["MemberId"]) ? Guid.Empty : new Guid(Request.QueryString["MemberId"]);
                ApplyId = string.IsNullOrEmpty(Request.QueryString["ApplyId"]) ? Guid.Empty : new Guid(Request.QueryString["ApplyId"]);
                var type = string.IsNullOrEmpty(Request.QueryString["Type"]) ? 1 : int.Parse(Request.QueryString["Type"]);
                SalePlatformId = string.IsNullOrEmpty(Request.QueryString["SalePlatformId"]) ? Guid.Empty : new Guid(Request.QueryString["SalePlatformId"]);
                DIV_AuditingPanel.Visible = type != 2;
            }
        }

        #region[属性]

        protected Guid MemberId
        {
            get
            {
                if (ViewState["MemberId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["MemberId"].ToString());
            }
            set
            {
                ViewState["MemberId"] = value;
            }
        }

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

        protected DateTime StartDate
        {
            get
            {
                if (ViewState["StartDate"] == null)
                {
                    return DateTime.MinValue;
                }
                return Convert.ToDateTime(ViewState["StartDate"]);
            }
            set
            {
                ViewState["StartDate"] = value;
            }
        }

        protected DateTime EndDate
        {
            get
            {
                if (ViewState["EndDate"] == null) return DateTime.MinValue;
                var endTime = Convert.ToDateTime(ViewState["EndDate"]);
                if (endTime != DateTime.MinValue)
                {
                    ViewState["EndDate"] = Convert.ToDateTime(ViewState["EndDate"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                }
                return Convert.ToDateTime(ViewState["EndDate"]);
            }
            set
            {
                ViewState["EndDate"] = value;
            }
        }

        protected int FlowType
        {
            get
            {
                if (ViewState["FlowType"] == null)
                {
                    return 0;
                }
                return Convert.ToInt32(ViewState["FlowType"]);
            }
            set
            {
                ViewState["FlowType"] = value;
            }
        }

        #endregion

        #region[通过]
        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                return;
            }
            if (ApplyId == Guid.Empty) return;

            try
            {
                string description = WebControl.RetrunUserAndTime("审核通过");
                MemberMentionApplyInfo info = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId);
                string errorMessage;
                var result = MemberCenterSao.ConfirmWithdrawWaitRemittance(info.SalePlatformId, info.Id, info.Memo, description, out errorMessage);
                if (!result)
                {
                    RAM.Alert("温馨提示：" + errorMessage);
                    return;
                }
                MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(info.SalePlatformId, info.MemberId);
                if (memberBaseInfo != null)
                {
                    if (!string.IsNullOrEmpty(memberBaseInfo.Mobile))
                    {
                        const string MSG = "您好，您的提现申请已收到，现已完成打款，一般2-5个工作日到账，请注意查收。详情可致电4006202020咨询。感谢您对可得网支持！";
                        MailSMSSao.SendShortMessage(info.SaleFilialeId, info.SalePlatformId, memberBaseInfo.Mobile, MSG);
                    }
                }
                //提现审核审核操作记录添加
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, ApplyId, info.ApplyNo,
                    OperationPoint.MemberWithdrawCash.Audit.GetBusinessInfo(), "审核通过");
                SubmitController.Submit();

                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("操作失败，系统提示：" + ex.Message);
            }
        }
        #endregion

        #region[不通过]
        protected void BtNoPassClick(object sender, EventArgs e)
        {
            try
            {
                MemberMentionApplyInfo info = MemberCenterSao.GetMemberMentionApply(SalePlatformId, ApplyId);
                string memo = info.Memo + " " + TB_Memo.Text.Trim();
                string description = WebControl.RetrunUserAndTime("审核不通过退回");
                string errorMessage;
                var result = MemberCenterSao.ReturnBackWithdrawApply(info.SalePlatformId, info.Id, memo, description, out errorMessage);
                if (!result)
                {
                    RAM.Alert("温馨提示：" + errorMessage);
                    return;
                }
                MemberBaseInfo memberBaseInfo = MemberCenterSao.GetUserBase(info.SalePlatformId, info.MemberId);
                if (memberBaseInfo != null)
                {
                    if (!string.IsNullOrEmpty(memberBaseInfo.Mobile))
                    {
                        string msg = "您好，您的提现申请已收到，遗憾审核未能通过，原因:" + TB_Memo.Text.Trim() + ",烦请尽快联系4006202020。感谢您对可得网支持！";
                        MailSMSSao.SendShortMessage(info.SaleFilialeId, info.SalePlatformId, memberBaseInfo.Mobile, msg);
                    }
                }
                //提现审核审核操作记录添加
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, ApplyId, info.ApplyNo,
                    OperationPoint.MemberWithdrawCash.Audit.GetBusinessInfo(), description);
            }
            catch (Exception ex)
            {
                RAM.Alert("操作失败，系统提示：" + ex.Message);
            }

            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region [搜索]

        /// <summary>搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbSearchClick(object sender, EventArgs e)
        {
            MemberId = MemberId;
            StartDate = rdpStartDate.SelectedDate == null ? DateTime.MinValue : rdpStartDate.SelectedDate.Value;
            EndDate = rdpEndDate.SelectedDate == null ? DateTime.MinValue : rdpEndDate.SelectedDate.Value;
            FlowType = int.Parse(DDL_CostDirection.SelectedValue);
            rgReckoning.Rebind();
        }

        #endregion

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGridReckoning_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = rgReckoning.CurrentPageIndex + 1;
            var pageSize = rgReckoning.PageSize;
            int totalCount = 0;
            IList<MemberBalanceFlowInfo> list = new List<MemberBalanceFlowInfo>();
            if (MemberId != Guid.Empty && ApplyId != Guid.Empty)
            {
                list = MemberCenterSao.GetMemberBalanceFlowByPage(MemberId, FlowType, StartDate, EndDate, pageIndex, pageSize, out totalCount);
            }
            rgReckoning.DataSource = list;
            rgReckoning.VirtualItemCount = totalCount;
        }

        #endregion

    }
}
