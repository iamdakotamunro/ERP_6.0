using System.Web;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;

namespace ERP.UI.Web.Common
{
    /// <summary>判断页面访问是否有权限
    /// </summary>
    public class PageControl
    {
        /// <summary>验证页面是否有权限访问
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pageUrl"></param>
        /// <param name="isWindowPage"></param>
        /// <param name="personnelInfo"></param>
        public static void VerifyPermission(HttpContext context, string pageUrl, bool isWindowPage, out PersonnelInfo personnelInfo)
        {
            personnelInfo = null;

            //验证是否可访问此页面
            var token = context.Request.QueryString["token"];
            if (string.IsNullOrEmpty(token))
            {
                var tokenCookie = context.Request.Cookies.Get("token");
                if (tokenCookie != null)
                {
                    token = tokenCookie.Value;
                }
            }
            else
            {
                context.Response.SetCookie(new HttpCookie("token", token));
            }

            if (!string.IsNullOrEmpty(token))
            {
                var resultInfo = PermissionSao.VerifyLoginToken(token);
                if (resultInfo.IsSuccess)
                {
                    personnelInfo = resultInfo.PersonnelInfo;
                    if (personnelInfo != null)
                    {
                        if (!isWindowPage)
                        {
                            //验证是否可访问此页面
                            if (personnelInfo.RealName != "admin")
                            {
                                var isCanVisit = PermissionSao.VerifyIsAllowVisitPage(personnelInfo.PersonnelId,
                                                                                      CurrentSession.System.ID,
                                                                                      pageUrl);
                                if (!isCanVisit)
                                {
                                    context.Response.Write("系统提示：当前页面无权浏览！");
                                    context.Response.End();
                                }
                            }
                        }
                    }
                    else
                    {
                        LogInOutUrl(context, "系统提示：登录状态失效，请重新登录！");
                    }
                }
                else
                {
                    LogInOutUrl(context, resultInfo.FailMessage);
                }
            }
            else
            {
#if release
                LogInOutUrl(context, "系统提示：登录状态失效，请重新登录！");
#endif
            }
        }

        private static void LogInOutUrl(HttpContext context, string failMessage)
        {
            context.Response.Write("<script>alert('" + failMessage +
                                           "');if (parent.parent.window != undefined) {parent.parent.window.location.href='" +
                                           GlobalConfig.LoginOutUrl + "';}</script>");
            context.Response.End();
        }
    }
}