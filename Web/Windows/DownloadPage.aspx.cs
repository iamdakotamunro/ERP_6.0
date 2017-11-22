using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public partial class DownloadPage : WindowsPage
    {
        string fileName, fullName;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                String sTag = String.Empty;
                String ss = Request["tag"];
                if (!String.IsNullOrEmpty(Request["tag"]))
                {
                    if (ss == "origal")
                        sTag = "[原]";
                    else if (ss == "auto")
                        sTag = "[异]";
                    else if (ss == "success")
                        sTag = "[成]";
                    else if (ss == "error")
                        sTag = "[错]";
                    else if (ss == "google")
                        sTag = "GOOGLE购物搜索";
                    else if (ss == "baidu")
                        sTag = "Baidu购物搜索";
                    else if (ss == "baidus")
                        sTag = "Baidu商品列表购物搜索";
                    else if (ss == "yicike")
                        sTag = "YiCiKe购物搜索";
                    else if (ss == "youdao")
                        sTag = "Youdao购物搜索";
                    else if (ss == "clever")
                        sTag = "Clever购物搜索";
                    else if (ss == "yigou")
                        sTag = "YiGou购物搜索";
                    else if (ss == "f1")
                        sTag = "[原始]";
                    else if (ss == "f2")
                        sTag = "[对比]";
                    else if (ss == "f3")
                        sTag = "[财务确认]";
                    else if (ss == "f4")
                        sTag = "[成功对账]";
                    else if (ss == "report")
                        sTag = "费用申报附件";
                    else if (ss == "goodsInformation")
                        sTag = "商品资料附件";
                    else if (ss == "companyInformation")
                        sTag = "往来单位资料附件";
                }

                if (String.IsNullOrEmpty(Request["fullname"]))
                    throw new ApplicationException("下载文件的路径为空");
                fullName = HttpUtility.UrlDecode(Request["fullname"], Encoding.GetEncoding("GB2312"));
                if (ss == "f1" || ss == "f2" || ss == "f3" || ss == "f4")
                {
                    string root = Server.MapPath("").Replace("Windows", "UserDir");
                    fullName = root + "/" + fullName;
                }
                else
                {
                    if (!File.Exists(fullName))
                        throw new ApplicationException("下载文件的路径有错误");
                }
                if (String.IsNullOrEmpty(Request["filename"]))
                    this.fileName = fullName.Split('/')[fullName.Split('/').Length - 1];

                String sP1 = String.Empty;
                String sP2 = String.Empty;
                if (sTag == String.Empty)
                {
                    sP1 = Regex.Match(this.fileName, @"\([^)]*\)\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p1"].Value;
                    sP2 = Regex.Match(this.fileName, @"\([^)]*\)\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p2"].Value;
                    this.fileName = "[" + Regex.Match(this.fileName, @"\([^)]*\)\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p1"].Value + "]" + Regex.Match(this.fileName, @"\([^)]*\)\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p2"].Value;
                }
                else if (sTag == "[成功对账]" || sTag == "[往来账]")
                {
                    this.fileName = sTag + Regex.Match(this.fileName, @"\(.*\)(?<name>[^.]*.*)").Groups["name"].Value;
                }
                else if (sTag == "[原]" || sTag == "[原始]" || sTag == "[对比]" || sTag == "[财务确认]")
                {
                    sP1 = Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)(?<p2>.*)").Groups["p1"].Value;
                    sP2 = Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)(?<p2>.*)").Groups["p2"].Value;
                    this.fileName = sTag + "[" + Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)(?<p2>.*)").Groups["p1"].Value + "]" + Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)(?<p2>.*)").Groups["p2"].Value;
                }
                else
                {
                    sP1 = Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p1"].Value;
                    sP2 = Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p2"].Value;
                    this.fileName = sTag + "[" + Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p1"].Value + "]" + Regex.Match(this.fileName, @"\((?<p1>[^(]*)\([^)]*\)\)\[[^]]*\](?<p2>.*)").Groups["p2"].Value;
                }
                //============
                String fileName = HttpUtility.UrlEncode(this.fileName, Encoding.UTF8);
                int p2Len = sP2.Length;
                int start = 1;
                if (sP1 != string.Empty && sP2 != string.Empty)
                {
                    while (fileName.Length > 100)
                    {
                        this.fileName = sTag + "[" + sP1 + "]" + sP2.Substring(start, p2Len - 1);
                        start++;
                        p2Len--;
                        fileName = HttpUtility.UrlEncode(this.fileName, Encoding.UTF8);
                    }
                }
                //============

                Page.Response.Clear();
                if (sTag == "GOOGLE购物搜索")
                    this.fileName = "GOOGLE购物搜索" + DateTime.Now.ToShortDateString() + ".xml";
                if (sTag == "Baidu购物搜索")
                    this.fileName = "Baidu购物搜索" + DateTime.Now.ToShortDateString() + ".xml";
                if (sTag == "Baidu商品列表购物搜索")
                    this.fileName = "Baidu商品列表购物搜索" + DateTime.Now.ToShortDateString() + ".xml";
                if (sTag == "YiCiKe购物搜索")
                    this.fileName = "YiCiKe购物搜索" + DateTime.Now.ToShortDateString() + ".csv";
                if (sTag == "Youdao购物搜索")
                    this.fileName = "Youdao购物搜索" + DateTime.Now.ToShortDateString() + ".csv";
                if (sTag == "Clever购物搜索")
                    this.fileName = "Clever购物搜索" + DateTime.Now.ToShortDateString() + ".csv";
                if (sTag == "YiGou购物搜索")
                    this.fileName = "YiGou购物搜索" + DateTime.Now.ToShortDateString() + ".xml";
                if (sTag == "费用申报附件")
                    this.fileName = "费用申报附件" + fullName.Substring(fullName.LastIndexOf('.'));
                if (sTag == "商品资料附件")
                    this.fileName = "商品资料附件" + fullName.Substring(fullName.LastIndexOf('.'));
                if (sTag == "往来单位资料附件")
                    this.fileName = "往来单位资料附件" + fullName.Substring(fullName.LastIndexOf('.'));
                bool success = DownloadUtility.ResponseFile(Page.Request, Page.Response, this.fileName, fullName, 1024000);
                if (!success)
                    throw new ApplicationException("文件下载不成功");
                Page.Response.End();
            }
        }
    }
}
