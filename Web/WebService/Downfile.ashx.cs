using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;

namespace ERP.UI.Web.WebService
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Downfile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var fileName = context.Request.QueryString["filename"];
            var exportOrder = context.Request.QueryString["exportOrder"];
            if (fileName != string.Empty)
            {
                string filePath = context.Server.MapPath("~/InvoiceExport/" + fileName); //路径
                if (!string.IsNullOrEmpty(exportOrder))
                {
                    //订单一次完成导出路径
                    filePath = context.Server.MapPath("~/Temp/" + fileName); 
                    if (File.Exists(filePath))
                    {
                        var fs = new FileStream(filePath, FileMode.Open);
                        var bytes = new byte[(int)fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        fs.Close();

                        var file = new FileInfo(filePath);
                        context.Response.ContentEncoding = Encoding.GetEncoding("gb2312"); //解决中文乱码
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + context.Server.UrlEncode(file.Name)); //解决中文文件名乱码    
                        context.Response.AddHeader("Content-length", file.Length.ToString(CultureInfo.InvariantCulture));
                        context.Response.ContentType = "appliction/ms-excel";
                        context.Response.WriteFile(file.FullName);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
                else
                {
                    if (File.Exists(filePath))
                    {
                        Rebuilder(filePath);
                        //var newZipfile =context.Server.MapPath("~/InvoiceExport/")+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".zip";
                        //Common.ZipHelper.ZipFile(newZipfile, filePath);
                        var file = new FileInfo(filePath);
                        context.Response.ContentEncoding = Encoding.GetEncoding("gb2312"); //解决中文乱码
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + context.Server.UrlEncode(file.Name)); //解决中文文件名乱码    
                        context.Response.AddHeader("Content-length", file.Length.ToString());
                        context.Response.ContentType = "appliction/octet-stream";
                        context.Response.WriteFile(file.FullName);
                        context.Response.End();
                    }
                }

            }
        }

        private void Rebuilder(string filepath)
        {
            var strLines = File.ReadAllLines(filepath, Encoding.GetEncoding("gb2312"));
            StringBuilder sb = new StringBuilder();
            foreach (var str in strLines)
            {
                if (str.Replace("\r", "").Replace("\n", "").Trim().Length > 0)
                {
                    sb.AppendLine(str);
                }
            }
            File.WriteAllText(filepath, sb.ToString(), Encoding.GetEncoding("gb2312"));
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
