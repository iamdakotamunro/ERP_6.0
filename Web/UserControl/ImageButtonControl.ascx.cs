using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Enum.Attribute;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// 功能：扩展ImageButton，含有文字说明
    /// 作者：邓杨焱
    /// 时间：2010.11.3
    /// </summary>
    public partial class ImageButtonControl : System.Web.UI.UserControl
    {
        public event EventHandler Click;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetImage(SkinType);
        }

        /// <summary>
        /// 设置图片控件url地址
        /// </summary>
        /// <param name="skinType"></param>
        private void SetImage(ImageSkinType skinType)
        {
            const string BASE_IMG_URL = "../App_Themes/default/images/";
            if (String.IsNullOrEmpty(Text))
                Text = EnumAttribute.GetKeyName(skinType);
            else
                Text = Text;
            switch (skinType)
            {
                case ImageSkinType.Cancel:
                    SetHTML(BASE_IMG_URL + "Cancel.gif");
                    break;
                case ImageSkinType.Affirm:
                    SetHTML(BASE_IMG_URL + "Affirm.gif");
                    break;
                case ImageSkinType.Delete:
                    SetHTML(BASE_IMG_URL + "Delete.gif");
                    break;
                case ImageSkinType.Edit:
                    SetHTML(BASE_IMG_URL + "Edit.gif");
                    break;
                case ImageSkinType.Insert:
                    SetHTML(BASE_IMG_URL + "Insert.gif");
                    break;
                case ImageSkinType.Search:
                    SetHTML(BASE_IMG_URL + "Search.gif");
                    break;
                case ImageSkinType.Refresh:
                    SetHTML(BASE_IMG_URL + "Refresh.gif");
                    break;
                case ImageSkinType.Messages:
                    SetHTML(BASE_IMG_URL + "Messages.gif");
                    break;
                case ImageSkinType.Buy:
                    SetHTML(BASE_IMG_URL + "Buy.gif");
                    break;
                case ImageSkinType.Uparrow:
                    SetHTML(BASE_IMG_URL + "Uparrow.gif");
                    break;
                case ImageSkinType.Downarrow:
                    SetHTML(BASE_IMG_URL + "Downarrow.gif");
                    break;
                case ImageSkinType.ExportExcel:
                    SetHTML(BASE_IMG_URL + "Excel.gif");
                    break;
                case ImageSkinType.CreationChart:
                    SetHTML(BASE_IMG_URL + "Chart.gif");
                    break;
                case ImageSkinType.CreationData:
                    SetHTML(BASE_IMG_URL + "Report.gif");
                    break;
                case ImageSkinType.Print:
                    SetHTML(BASE_IMG_URL + "Print.gif");
                    break;
                case ImageSkinType.Redeploy:
                    SetHTML(BASE_IMG_URL + "Redeploy.gif");
                    break;
                case ImageSkinType.Refundment:
                    SetHTML(BASE_IMG_URL + "Refundment.gif");
                    break;
                case ImageSkinType.ExpertsAnswer:
                    SetHTML(BASE_IMG_URL + "ExpertsAnswer.gif");
                    break;
                case ImageSkinType.Quaere:
                    SetHTML(BASE_IMG_URL + "Quaere.gif");
                    break;
                case ImageSkinType.Experience:
                    SetHTML(BASE_IMG_URL + "Experience.gif");
                    break;
                default:
                    SetHTML("");
                    break;

            }
        }

        /// <summary>
        /// 设置图片地址
        /// </summary>
        /// <param name="strImageUrl"></param>
        private void SetHTML(String strImageUrl)
        {
            ibtnAction.ImageUrl = strImageUrl;
        }

        /// <summary>
        /// 图片按钮控件
        /// </summary>
        protected ImageButton ImageButtonAction
        {
            get
            {
                return ibtnAction;
            }
        }

        /// <summary>
        /// 按钮显示的文字
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许修改
        /// </summary>
        public bool Enabled
        {
            set
            {
                ibtnAction.Enabled = value;
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public new bool Visible
        {
            get
            {
                return divImageButton.Visible;
            }
            set
            {
                divImageButton.Visible = value;
            }
        }

        /// <summary>
        /// 图片类型
        /// </summary>
        public ImageSkinType SkinType
        {
            get;
            set;
        }

        /// <summary>
        /// 客户端点击事件
        /// </summary>
        public string OnClientClick
        {
            set
            {
                ibtnAction.OnClientClick = value;
            }
        }

        /// <summary>
        /// 命令名
        /// </summary>
        public string CommandName
        {
            set {
                ibtnAction.CommandName = value;
            }
        }

        /// <summary>
        /// 验证组
        /// </summary>
        public string ValidationGroup
        {
            set
            {
                ibtnAction.ValidationGroup = value;
            }
        }

        public bool CausesValidation
        {
            set
            {
                ibtnAction.CausesValidation = value;
            }
        }

        /// <summary>
        /// 把按钮事件公开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibtnAction_Click(object sender, ImageClickEventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }
        

    }
    /// <summary>
    /// 图片类型
    /// </summary>
    public enum ImageSkinType
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Enum("新建")]
        Insert,
        /// <summary>
        /// 编辑
        /// </summary>
        [Enum("编辑")]
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        [Enum("删除")]
        Delete,
        /// <summary>
        /// 确定
        /// </summary>
        [Enum("确定")]
        Affirm,
        /// <summary>
        /// 取消
        /// </summary>
        [Enum("取消")]
        Cancel,
        /// <summary>
        /// 搜索
        /// </summary>
        [Enum("搜索")]
        Search,
        /// <summary>
        /// 刷新
        /// </summary>
        [Enum("刷新")]
        Refresh,
        /// <summary>
        /// 短消息
        /// </summary>
        [Enum("短消息")]
        Messages,
        /// <summary>
        /// 生成报表
        /// </summary>
        [Enum("生成报表")]
        CreationData,
        /// <summary>
        /// 生成报告
        /// </summary>
        [Enum("生成图表")]
        CreationChart,
        /// <summary>
        /// 导出EXCEL
        /// </summary>
        [Enum("导出EXCEL")]
        ExportExcel,
        /// <summary>
        /// 上升
        /// </summary>
        [Enum("上升")]
        Uparrow,
        /// <summary>
        /// 下降
        /// </summary>
        [Enum("下降")]
        Downarrow,
        /// <summary>
        /// 电话下单
        /// </summary>
        [Enum("电话下单")]
        Buy,
        /// <summary>
        /// 打印
        /// </summary>
        [Enum("打印")]
        Print,
        /// <summary>
        /// 缺货
        /// </summary>
        [Enum("缺货")]
        Redeploy,
        /// <summary>
        /// 退款
        /// </summary>
        [Enum("退款")]
        Refundment,
        /// <summary>
        /// 专家解答
        /// </summary>
        [Enum("专家解答")]
        ExpertsAnswer,
        /// <summary>
        /// 询问
        /// </summary>
        [Enum("询问")]
        Quaere,
        /// <summary>
        /// 经验
        /// </summary>
        [Enum("经验")]
        Experience
    }
}