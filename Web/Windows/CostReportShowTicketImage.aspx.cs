using System;
using System.Web;
using System.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CostReportShowTicketImage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var imagePath= Request.QueryString["ImageUrl"];
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    string imageUrl = "~/Windows/DownloadPage.aspx?tag=report&fullname=" + HttpUtility.UrlEncode(Server.MapPath(imagePath));
                    IB_ShowImg.ImageUrl = imageUrl;
                }
                else
                    Response.Write("系统提示：未找该票据图片！");
            }
        }
    }
}