using System.Web.UI;

namespace ERP.UI.Web.Common
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :在网页中弹出对话框或添加脚本
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    /// <summary>
    /// 在网页中弹出对话框或添加脚本
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// 弹出自定义的有换行情况或者信息量大的提示信息，alert实现，不会自动隐藏（不能在Page_Load里使用）
        /// </summary>
        /// <param name="control">执行弹出对话框的位置（可传this）</param>
        /// <param name="msg">弹出消息</param>
        public static void Show(Control control, string msg)
        {
            LiteralControl lc = new LiteralControl();
            lc.Text = "<script type=\"text/javascript\">alert(\"" + msg + "\")</script>";
            control.Controls.Add(lc);
        }
        /// <summary>
        /// 在文档末尾加一个脚本块（不能在Page_Load里使用，在文档加载完成后给出提示）
        /// </summary>
        /// <param name="control">要添加脚本块位置（可传this）</param>
        /// <param name="script">脚本内容</param>
        public static void AppendScript(Control control, string script)
        {
            LiteralControl lc = new LiteralControl();
            lc.Text = "<script type=\"text/javascript\">" + script + "</script>";
            control.Controls.Add(lc);
        }
        /// <summary>
        /// 直接在文档头部中写入一段脚本（可在Page_Load里使用,适合提示后就立跳转页面）
        /// </summary>
        /// <param name="page">Page对象，传this即可</param>
        /// <param name="script"></param>
        public static void WriteScript(Page page, string script)
        {
            page.Response.Write("<script type=\"text/javascript\">");
            page.Response.Write(script);
            page.Response.Write(";</script>");
        }
    }
}
