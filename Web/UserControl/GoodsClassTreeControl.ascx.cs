using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Goods;
using ERP.Model.Goods;
using Telerik.Web.UI;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// 功能：商品分类的树控件
    /// 时间：2010.11.2
    /// 作者：邓杨焱
    /// </summary>
    public partial class GoodsClassTreeControl : System.Web.UI.UserControl
    {
        private readonly GoodsClassManager goodsClass = new GoodsClassManager();
        public delegate void TreeNodeClickHandler(object sender, RadTreeNodeEventArgs e);
        public event TreeNodeClickHandler TreeNodeClick;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindTreeGoodsClass();
            }
        }

        /// <summary>
        /// 点击事件公开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void tvGoodsClass_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (TreeNodeClick != null)
                TreeNodeClick(this, e);
        }

        /// <summary>
        /// 创建商品分类树
        /// </summary>
        private void BindTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            tvGoodsClass.Nodes.Add(rootNode);
            RecursivelyGoodsClass(Guid.Empty, rootNode);
        }

        /// <summary>
        /// 遍历产品分类
        /// </summary>
        /// <param name="goodsClassId">分类ID</param>
        /// <param name="node">当前节点</param>
        private void RecursivelyGoodsClass(Guid goodsClassId, RadTreeNode node)
        {
            IList<GoodsClassInfo> goodsClassList = goodsClass.GetChildGoodsClassList(goodsClassId);
            foreach (GoodsClassInfo goodsClassInfo in goodsClassList)
            {
                RadTreeNode goodsClassNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                if (goodsClassInfo.State == 0)
                {
                    goodsClassNode.CssClass = "UseState";
                    goodsClassNode.ContentCssClass = "UseState";
                }
                if (node == null)
                    tvGoodsClass.Nodes.Add(goodsClassNode);
                else
                    node.Nodes.Add(goodsClassNode);
                RecursivelyGoodsClass(goodsClassInfo.ClassId, goodsClassNode);
            }
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="text">节点文字</param>
        /// <param name="expanded">是否展开</param>
        /// <param name="id">分类ID</param>
        /// <returns>创建的节点</returns>
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            return  new RadTreeNode(text, id) {ToolTip = text, Expanded = expanded};
        }
    }
}