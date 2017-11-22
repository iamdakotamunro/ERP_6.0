using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// 功能：对文件下载
    /// 编码：dyy
    /// 日期：2009-07-15
    /// </summary>
    public static class DownloadUtility
    {
        /// <summary>Func:文件的下载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="filename"></param>
        /// <param name="fullPath"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static bool ResponseFile(HttpRequest request, HttpResponse response, string filename, string fullPath, long speed)
        {
            try
            {
                FileStream myFile = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    //==**==//
                    String fileName = HttpUtility.UrlEncode(filename, Encoding.UTF8);
                    //==**==/


                    response.AddHeader("Accept-Ranges", "bytes");
                    response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;

                    const double PACK = 10240; //10K bytes
                    //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                    int sleep = (int)Math.Floor(1000 * PACK / speed) + 1;
                    if (request.Headers["Range"] != null)
                    {
                        response.StatusCode = 206;
                        string[] range = request.Headers["Range"].Split('=', '-');
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        //Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength-1, fileLength));
                    }
                    response.AddHeader("Connection", "Keep-Alive");
                    //_Response.ContentType = "application/octet-stream";
                    response.ContentType = "text/x-xls";
                    //_Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(_fileName, System.Text.Encoding.UTF8));
                    response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    response.HeaderEncoding = Encoding.UTF8;

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((fileLength - startBytes) / PACK) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (response.IsClientConnected)
                        {
                            response.BinaryWrite(br.ReadBytes(int.Parse(PACK.ToString(CultureInfo.InvariantCulture))));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();

                    myFile.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
