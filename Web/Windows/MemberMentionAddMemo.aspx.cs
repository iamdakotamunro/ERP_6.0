using System;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class MemberMentionAddMemo : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region [添加备注]
        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ApplyId"]))
            {
                var applyId = new Guid(Request.QueryString["ApplyId"]);
                var salePlatformId = new Guid(Request.QueryString["SalePlatformId"]);
                string memo = WebControl.RetrunUserAndTime(TB_Memo.Text.Trim());
                string errorMessage;
                var result = MemberCenterSao.SetWithdrawApplyDescription(salePlatformId, applyId, memo, out errorMessage);
                if (!result)
                {
                    RAM.Alert("温馨提示：" + errorMessage);
                    return;
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
        #endregion
    }
}
