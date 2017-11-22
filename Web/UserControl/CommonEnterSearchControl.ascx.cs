using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Enum.Attribute;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.UserControl
{
    public partial class CommonEnterSearchControl : System.Web.UI.UserControl
    {
        public event EventHandler Click;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchLableText) && !String.IsNullOrEmpty(SearchLableText.Trim()))
            {
                lblText.Text = SearchLableText.Trim();
            }
            if (!String.IsNullOrEmpty(CommandNameX) && !String.IsNullOrEmpty(CommandNameX.Trim()))
                SearchBtn.CommandName = CommandNameX.Trim();
            else
                SearchBtn.CommandName = "Search";
        }
        /// <summary>
        /// 把按钮事件公开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnToSearch(object sender, ImageClickEventArgs e)
        {
            if (Click != null)
            {
                CommonSearchClickEventArgs e3 = new CommonSearchClickEventArgs { SearchText = SearchText };
                Click(sender, e3);
            }
        }


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
        
    }
}