using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using ERP.DAL.Implement.Order;
using ERP.Environment;
using ERP.Model;
using Framework.Common;

namespace STO11TempTask
{
    public class STO
    {
        private const string requestUrl = "http://116.236.128.194:7000/?API=ORDER";
        private static readonly GoodsOrder _goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Write);

        public static void Handle()
        {
            var stoTempList = _goodsOrder.GetStoTempList(Configuration.AppSettings["Sto11HandleNum"].ToInt());
            foreach (var stoTmep in stoTempList)
            {
                try
                {
                    stoTmep.address = FilterSpecialStr(stoTmep.address);
                    stoTmep.name = FilterSpecialStr(stoTmep.name);
                    var xml = ConvertXML(stoTmep);
                    var result = ValidateResult(HttpPostToSTO(xml));
                    if (result)
                    {
                        _goodsOrder.SetStoTempHandled(stoTmep.OrderNo);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log("Sto11TaskLog.Error", String.Format("申通双11临时接口任务记录日志 {0}", stoTmep.OrderNo), ex);
                }
            }
        }

        static Boolean ValidateResult(String resultXml)
        {
            //var resultXml = "<root><success>T</success><code></code><data></data></root>";
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resultXml);
                var findNode = xmlDoc.GetElementsByTagName("success")[0];
                return findNode != null && findNode.InnerText.Equals("T", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        static string HttpPostToSTO(string xmlContent)
        {
            string responseResult;
            //HTTP POST 编码
            var data = Encoding.GetEncoding("UTF-8").GetBytes(xmlContent);
            try
            {
                //创建POST请求链接 
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUrl + "&SIGN=" + xmlContent.Length);

                //请求方式
                httpRequest.Method = "POST";

                //资源建立持久性连接
                httpRequest.KeepAlive = false;

                //http标头
                httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                //请求字节长度
                httpRequest.ContentLength = data.Length;

                // 发送数据 
                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                //响应数据
                HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;

                //读取相应数据
                StreamReader responseStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseResult = responseStreamReader.ReadToEnd();
                responseStreamReader.Close();

                //关闭资源链接
                response.Close();

                //终止请求
                httpRequest.Abort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseResult;
        }

        static string ConvertXML(StoTempInfo model)
        {
            var sb = new StringBuilder();
            sb.Append("<xml>");
            var modelName = model.GetType().Name;
            sb.Append("<").Append(modelName).Append(">");
            sb.AppendLine();
            foreach (var property in model.GetType().GetProperties())
            {
                string propertyName = property.Name;
                var propertyValue = property.GetValue(model, null);
                if (propertyValue == null || propertyName == "OrderNo")
                {
                    continue;
                }
                sb.Append("<").Append(propertyName).Append(">");
                sb.Append(propertyValue);
                sb.Append("</").Append(propertyName).Append(">");
                sb.AppendLine();
            }
            sb.Append("</").Append(modelName).Append(">");
            sb.AppendLine();
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>过滤特殊字符串
        /// </summary>
        /// <returns></returns>
        public static String FilterSpecialStr(String theString)
        {
            theString = theString.Replace("&middot;", "");
            theString = theString.Replace("&nbsp;", "");
            theString = theString.Replace("&mdash;", "");
            theString = theString.Replace("&ldquo;", "");
            theString = theString.Replace("&rdquo;", "");
            theString = theString.Replace("&gt;", "");
            theString = theString.Replace("&lt;", "");
            theString = theString.Replace("&quot;", "");
            theString = theString.Replace("&apos;", "");
            theString = theString.Replace("&amp;", "");
            theString = theString.Replace(">", "");
            theString = theString.Replace("<", "");
            theString = theString.Replace("》", "");
            theString = theString.Replace("《", "");
            theString = theString.Replace("&", "");
            theString = theString.Replace("'", "");
            theString = theString.Replace("‘", "");
            return theString;
        }
    }
}
