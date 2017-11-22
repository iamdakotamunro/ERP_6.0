using System;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// Func ： 通用搜索控件
    /// Code :  dyy
    /// Date :   2009 sept 9th
    /// 最后修改人：刘彩军
    /// 修改时间：2011-september-28th
    /// 修改内容：代码优化
    /// </summary>
    public partial class CommonSearch : System.Web.UI.UserControl
    {
        //IList<String> fieldList;

        /// <summary>
        /// 
        /// </summary>
        public string CommandNameX { get; set; }

        /// <summary>
        /// 搜索Lable名称
        /// </summary>
        public string SearchLableText { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public String SearchText
        {
            get
            {
                if (TB_Search.Text.Trim() != String.Empty)
                    ViewState["SearchText"] = TB_Search.Text.Trim();
                if (ViewState["SearchText"] != null)
                    return ViewState["SearchText"].ToString();
                return String.Empty;
            }
            set
            {
                ViewState["SearchText"] = value;
                if (ViewState["SearchText"] != null)
                    TB_Search.Text = ViewState["SearchText"].ToString();
            }
        }

        /// <summary>
        /// 搜索数据库表的字段列表(以字符串的形式，逗号"，"隔开)
        /// 例如：FieldListText = "OrderID,OrderName";
        /// </summary>
        public string FieldListText { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchLableText) && !String.IsNullOrEmpty(SearchLableText.Trim()))
            {
                lblText.Text = SearchLableText.Trim();
            }
            if (!String.IsNullOrEmpty(CommandNameX) && !String.IsNullOrEmpty(CommandNameX.Trim()))
                LB_Search.CommandName = CommandNameX.Trim();
            else
                LB_Search.CommandName = "Search";
        }

        /// <summary>
        /// 搜索事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbSearchClick(object sender, EventArgs e)
        {
            if (SearchClick != null)
            {
                CommonSearchClickEventArgs e3 = new CommonSearchClickEventArgs {SearchText = SearchText};
                SearchClick(this, e3);
            }
        }

        public event EventHandler<CommonSearchClickEventArgs> SearchClick;
    }
}
