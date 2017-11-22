using System;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public partial class ShowImg : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imgInformation.ImageUrl = Request.QueryString["path"];
        }
    }
}

