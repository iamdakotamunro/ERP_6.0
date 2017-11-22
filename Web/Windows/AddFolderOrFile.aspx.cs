using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AddFolderOrFile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Files.Count > 0)
            {
                string type = Request.QueryString["Type"] ?? "0";  //0为对比上传，1为完成上传
                HttpPostedFile file = Request.Files[0];
                string date = Request.QueryString["D"]??DateTime.Now.ToString("yyyyMM").ToString(CultureInfo.InvariantCulture);
                try
                {
                    string root = type == "0" ? Server.MapPath("../UserDir/ZzzContrastFolder") : Server.MapPath("../UserDir/ZzzFinishedFolder");

                    string rootPath = root + "/" + date;
                    if(!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }
                    string tempPath = rootPath + "/" + file.FileName; 
                    if (!File.Exists(tempPath))
                    {
                        file.SaveAs(tempPath);
                    }
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}