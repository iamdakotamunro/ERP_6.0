using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    /// <summary>添加赠品借记单备注 ADD 2015-02-06  陈重文  
    /// </summary>
    public partial class DebitNoteAddMemoForm : WindowsPage
    {
        private readonly IDebitNote _debitNote=new DebitNote(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (PurchasingId == Guid.Empty)
            {
                RAM.Alert("系统提示：传递参数错误，请尝试刷新后重新操作！");
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                var info = _debitNote.GetDebitNoteInfo(PurchasingId);
                lable_Memo.Text = Server.HtmlDecode(info.Memo);
                Lb_Title.Text = info.Title;
                if (IsRead)
                {
                    TB_Memo.Visible = false;
                    BT_Save.Visible = false;
                    span_Title.Visible = false;
                    Page.Title = "查看赠品借记单备注";
                }
            }
        }

        /// <summary>借记单ID
        /// </summary>
        private Guid PurchasingId
        {
            get
            {
                var purchasingId = Request.QueryString["PurchasingId"];
                return !string.IsNullOrWhiteSpace(purchasingId) ? new Guid(purchasingId) : Guid.Empty;
            }
        }

        /// <summary>是否查看
        /// </summary>
        private Boolean IsRead
        {
            get
            {
                var read = Request.QueryString["Read"];
                return !string.IsNullOrWhiteSpace(read);
            }
        }

        /// <summary>保存备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtSaveClick(object sender, EventArgs e)
        {
            var memo = TB_Memo.Text;
            if (string.IsNullOrWhiteSpace(memo))
            {
                RAM.Alert("系统提示：备注不能为空");
                return;
            }
            var personnel = CurrentSession.Personnel.Get();
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var description = string.Format("[更新备注:{0};操作人:{1};{2}]<br/><br/>", memo.Trim(), personnel.RealName, dateTime);
            _debitNote.AddDebitNoteMemo(PurchasingId, description);
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}