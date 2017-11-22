using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class GoodsAttrWords : BasePage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao = new GoodsCenterSao();
        public bool IsShow = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //商品属性组
                AttrGroupList = _goodsAttributeGroupSao.GetAttrGroupList().Where(p => p.MatchType != 2).ToList();
                BindGroup();//商品属性组
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_AttrWords.CurrentPageIndex = 0;
            RG_AttrWords.DataBind();
        }

        #region 自定义属性
        //商品属性组
        private IList<AttributeGroupInfo> AttrGroupList
        {
            get
            {
                if (ViewState["AttrGroupList"] == null) return new List<AttributeGroupInfo>();
                return (IList<AttributeGroupInfo>)ViewState["AttrGroupList"];
            }
            set { ViewState["AttrGroupList"] = value; }
        }
        #endregion

        #region 数据准备
        #region 商品属性组
        /// <summary> 添加根节点
        /// </summary>
        protected void BindGroup()
        {
            RTV_AttrGroup.Nodes.Clear();
            var rootNode = new RadTreeNode("所有商品属性组", "0");
            RTV_AttrGroup.Nodes.Add(rootNode);
            CreateGroupNodes(rootNode);
            rootNode.Selected = true;
            rootNode.Expanded = true;
        }

        /// <summary> 添加属性组节点
        /// </summary>
        /// <param name="node"></param>
        protected void CreateGroupNodes(RadTreeNode node)
        {
            IList<AttributeGroupInfo> groupInfos = AttrGroupList.OrderBy(g => g.OrderIndex).ToList();
            foreach (var attrGroupInfo in groupInfos)
            {
                var childNode = CreateNode(attrGroupInfo.GroupName, false, string.Format("{0}", attrGroupInfo.GroupId));
                node.Nodes.Add(childNode);
            }
        }

        /// <summary> 创建节点
        /// </summary>
        /// <param name="text"></param>
        /// <param name="expanded"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        // 树形节点点击时
        protected void Rtv_AttrGroup_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            Hid_GroupId.Value = "0";
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                Hid_GroupId.Value = e.Node.Value;
                RG_AttrWords.Rebind();

                #region 用于前台传参
                var attrGroupInfo = AttrGroupList.FirstOrDefault(w => w.GroupId == int.Parse(Hid_GroupId.Value));
                if (attrGroupInfo != null)
                {
                    Hid_MatchType.Value = string.Format("{0}", attrGroupInfo.MatchType);
                    Hid_IsMChoice.Value = attrGroupInfo.IsMChoice ? "1" : "0";
                }
                #endregion
            }
        }

        #endregion
        #endregion

        #region 数据列表相关
        protected void RG_AttrWords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack && !Hid_GroupId.Value.Equals("0"))
            {
                GridDataBind();
            }
            else
            {
                IList<AttributeWordInfo> attrWordsList = new List<AttributeWordInfo>();
                RG_AttrWords.DataSource = attrWordsList;
            }
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var attrWordsList = _goodsAttributeGroupSao.GetAttrWordsListByGroupId(int.Parse(Hid_GroupId.Value)).OrderBy(p => p.OrderIndex);
            RG_AttrWords.DataSource = attrWordsList;
            IsShow = true;
        }
        #endregion

        //删除商品属性
        protected void btn_Del_Click(object sender, EventArgs e)
        {
            var wordId = ((Button)sender).CommandName;
            string errorMessage;
            var result = _goodsAttributeGroupSao.DeleteAttrWords(int.Parse(wordId), out errorMessage);
            if (result)
            {
                RG_AttrWords.Rebind();
            }
        }


    }
}