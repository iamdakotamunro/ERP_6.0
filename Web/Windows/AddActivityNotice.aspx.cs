using System;
using System.Web.UI;
using ERP.DAL.Implement.Shop;
using ERP.Environment;
using Keede.Ecsoft.Model.ShopFront;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    public partial class AddActivityNotice : WindowsPage
    {
        private readonly ShopActivityNoticeDal _shopActivityNotice = new ShopActivityNoticeDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["NoteId"]))
                {
                    NoteId = new Guid(Request.QueryString["NoteId"]);
                    var info = _shopActivityNotice.SelectActivityNoticeInfo(NoteId);
                    TB_Title.Text = info.NoticeTitle;
                    Editor_NoticeContent.Content = info.NoticeContent;
                    CB_IsNotice.Checked = info.IsNotice;
                    CB_IsShow.Checked = info.IsShow;
                    btnAdd.Visible = false;
                    btnUpdate.Visible = true;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                }
            }
        }
        private Guid NoteId
        {
            get { return (Guid)ViewState["NoteId"]; }
            set { ViewState["NoteId"] = value; }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TB_Title.Text.Trim()))
            {
                RAM.Alert("公告标题必填！");
                return;
            }
            if (string.IsNullOrEmpty(Editor_NoticeContent.Content))
            {
                RAM.Alert("公告内容必填！");
                return;
            }
            var info = new ShopActivityNoticeInfo
            {
                NoticeID = NoteId,
                NoticeTitle = TB_Title.Text,
                NoticeContent = Editor_NoticeContent.Content,
                IsNotice = CB_IsNotice.Checked,
                IsShow = CB_IsShow.Checked,
                CreateTime = DateTime.Now
            };
            var reault = _shopActivityNotice.UpdateShopActivityNoticeInfo(info);
            if (reault)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (string.IsNullOrEmpty(TB_Title.Text.Trim()))
                {
                    RAM.Alert("公告标题必填！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(Editor_NoticeContent.Content))
                {
                    RAM.Alert("公告内容必填！");
                    ctx.SetFail();
                    return;
                }
                var info = new ShopActivityNoticeInfo
                {
                    NoticeID = Guid.NewGuid(),
                    NoticeTitle = TB_Title.Text,
                    NoticeContent = Editor_NoticeContent.Content,
                    IsNotice = CB_IsNotice.Checked,
                    IsShow = CB_IsShow.Checked,
                    CreateTime = DateTime.Now
                };
                var reault = _shopActivityNotice.InsertShopActivityNoticeInfo(info);
                if (reault)
                {
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            });
        }
    }
}