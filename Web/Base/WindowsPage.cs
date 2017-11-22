using System;
using System.Web.UI;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Base
{
    /// <summary>WindowsPage 继承，所有窗体页面都需继承
    /// </summary>
    public class WindowsPage : BasePage
    {
        public override bool IsWindowsPage()
        {
            return true;
        }

        /// <summary>初始化验证
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var personnel = CurrentSession.Personnel.Get();
            //if (Request.UrlReferrer == null)
            //{
            //    Response.Write("系统提示：非法请求链接，请登录后再做操作！");
            //    Response.End();
            //}
            //else if (Request.UrlReferrer.Authority != Request.Url.Authority)
            //{
            //    Response.Write("系统提示：非法请求链接，请登录后再做操作！");
            //    Response.End();
            //}
            //else
            if (personnel == null || personnel.PersonnelId == Guid.Empty)
            {
                Response.Write("系统提示：登录信息已失效！");
                Response.End();
            }
        }
    }
}