using System;
using System.Web;
using ERP.BLL.Implement;

namespace ERP.UI.Web
{
    /// <summary>
    /// RemoveCache 的摘要说明
    /// </summary>
    public class RemoveCache : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var key = context.Request.QueryString["key"];
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    if (key.ToLower() == "all")
                    {
                        CacheManager.RemoveAll();
                    }
                    else
                    {
                        CacheManager.Remove(key);
                    }
                    context.Response.Write("OK");
                }
                catch (Exception exp)
                {
                    context.Response.Write(exp.Message+"\r\n"+exp.StackTrace);
                }
            }
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}