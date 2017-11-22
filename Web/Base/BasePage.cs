using System;
using System.Web.UI;
using ERP.Model;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Base
{
    public class BasePage : Page
    {
        protected PersonnelInfo PersonnelInfo
        {
            get { return ViewState["LoginPersonnelInfo"] as PersonnelInfo; }
            set { ViewState["LoginPersonnelInfo"] = value; }
        }

        private AvoidResubmitHelper AvoidResubmitController
        {
            get
            {
                if (ViewState["__AvoidResubmit"] == null)
                {
                    ViewState["__AvoidResubmit"] = new AvoidResubmitHelper(Guid.NewGuid());
                }
                return (AvoidResubmitHelper)ViewState["__AvoidResubmit"];
            }
            set
            {
                ViewState["__AvoidResubmit"] = value;
            }
        }

        protected string Token
        {
            get { return Context.Request.QueryString["token"]; }
        }

        public virtual bool IsWindowsPage()
        {
            return false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var pageUrl = Context.Request.Url.AbsolutePath;
            var len = pageUrl.Length-pageUrl.Replace("/","").Length;
            if (len==1)
            {
                pageUrl = pageUrl.Replace("/", "");
            }
            PersonnelInfo personnelInfo;
            PageControl.VerifyPermission(Context, pageUrl, IsWindowsPage(), out personnelInfo);
            PersonnelInfo = personnelInfo;
            CurrentSession.Personnel.Set(personnelInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        protected void AjaxRequest(RadGrid obj, AjaxRequestEventArgs e)
        {
            switch (e.Argument)
            {
                case "Rebind":
                    obj.MasterTableView.SortExpressions.Clear();
                    obj.MasterTableView.GroupByExpressions.Clear();
                    obj.Rebind();
                    break;
                case "RebindAndNavigate":
                    obj.MasterTableView.SortExpressions.Clear();
                    obj.MasterTableView.GroupByExpressions.Clear();
                    obj.MasterTableView.CurrentPageIndex = obj.MasterTableView.PageCount - 1;
                    obj.Rebind();
                    break;
            }
        }

        /// <summary>
        /// 是否可提交
        /// </summary>
        /// <returns></returns>
        protected bool CanSubmit()
        {
            return AvoidResubmitController.Enabled;
        }

        /// <summary>
        /// 执行提交操作
        /// </summary>
        /// <param name="submitMethod">执行提交的方法，返回值为成功或失败；失败时，可以再次执行提交</param>
        protected void ExecuteSubmit(Action<AvoidResubmitHelper.Context> submitMethod)
        {
            if (CanSubmit())
            {
                BeginSubmit();
                try
                {
                    if(submitMethod != null)
                    {
                        var ctx = new AvoidResubmitHelper.Context();
                        submitMethod.Invoke(ctx);
                        if (!ctx.IsSucceed)
                        {
                            Rollback();
                        }
                    }
                }
                catch
                {
                    Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 开始标记为已提交
        /// </summary>
        private void BeginSubmit()
        {
            var ctrl = AvoidResubmitController;
            ctrl.Submit();
            AvoidResubmitController = ctrl;
        }

        /// <summary>
        /// 开始标记为已提交
        /// </summary>
        private void Rollback()
        {
            var ctrl = AvoidResubmitController;
            ctrl.Rollback();
            AvoidResubmitController = ctrl;
        }
    }
}