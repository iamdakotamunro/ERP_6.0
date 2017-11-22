using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Goods;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class GoodsClassAw : BasePage
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IB_Add.Visible = false;
                LB_AddSpace.Visible = false;
                IB_Update.Visible = false;
                LB_UpdateSpace.Visible = false;
                IB_Cancel.Visible = false;
                BindParentClass();
                NonceGoodsClassInfo = new GoodsClassInfo();
                GetTreeGoodsClass();
                DisableGoodsClassPanel(true);
            }
        }

        private void BindParentClass()
        {
            DDL_ParentClass.Items.Clear();
            //方法里面每个级别已排序，在此排序顺序会错乱，modify by Li Zhongkai,2015-05-27
            DDL_ParentClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();  
            DDL_ParentClass.DataBind();
            DDL_ParentClass.Items.Insert(0, new ListItem("根目录", "00000000-0000-0000-0000-000000000000"));
        }

        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().OrderBy(act => act.OrderIndex).ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
        }

        //遍历产品分类
        private void RecursivelyGoodsClass(Guid goodsClassId, IRadTreeNodeContainer node, IList<GoodsClassInfo> goodsClassList)
        {
            IList<GoodsClassInfo> childGoodsClassList = goodsClassList.Where(w => w.ParentClassId == goodsClassId).ToList();
            foreach (GoodsClassInfo goodsClassInfo in childGoodsClassList)
            {
                RadTreeNode goodsClassNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                if (node == null)
                    TVGoodsClass.Nodes.Add(goodsClassNode);
                else
                    node.Nodes.Add(goodsClassNode);
                RecursivelyGoodsClass(goodsClassInfo.ClassId, goodsClassNode, goodsClassList);
            }
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        private static void OpenTree(RadTreeNode treeNode)
        {
            if (treeNode != null)
            {
                if (!treeNode.Expanded)
                {
                    treeNode.Expanded = true;
                    OpenTree(treeNode.ParentNode);
                }
            }
        }

        //选择商品分类树节点
        protected void TvGoodsClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            var classId = new Guid(e.Node.Value);
            if (!IsInster)
            {
                NonceGoodsClassInfo = new GoodsClassInfo();
                if (classId != Guid.Empty)
                {
                    var goodsClassInfo = _goodsCenterSao.GetClassDetail(classId);
                    if (goodsClassInfo.GoodsClassFieldList == null)
                        goodsClassInfo.GoodsClassFieldList = new List<Guid>();
                    NonceGoodsClassInfo = goodsClassInfo;
                    if (goodsClassInfo.GoodsClassFieldList.Count > 0)
                        GoodsClassFieldIdList = goodsClassInfo.GoodsClassFieldList;
                }
            }
            else
            {
                NonceGoodsClassInfo = new GoodsClassInfo { ClassId = Guid.NewGuid(), ParentClassId = classId, ClassName = String.Empty };
            }
        }

        protected void FieldGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = _goodsCenterSao.GetFieldList().ToList();
            FieldGrid.DataSource = list.Where(w => w.ParentFieldId == Guid.Empty);
        }

        private GoodsClassInfo NonceGoodsClassInfo
        {
            get
            {
                var currentNode = TVGoodsClass.SelectedNode;
                var classId = IsInster ? Guid.NewGuid() : new Guid(currentNode.Value);
                var parentClassId = new Guid(DDL_ParentClass.SelectedValue);
                var orderIndex = Convert.ToInt32(TB_OrderIndex.Text);
                var className = TB_ClassName.Text;
                return new GoodsClassInfo { ClassId = classId, ParentClassId = parentClassId, ClassName = className, OrderIndex = orderIndex };
            }
            set
            {
                if (value == null) value = new GoodsClassInfo();
                TB_ClassName.Text = value.ClassName;
                TB_OrderIndex.Text = string.Format("{0}",value.OrderIndex);
                DDL_ParentClass.SelectedValue = value.ParentClassId.ToString();
                GoodsClassFieldIdList = new List<Guid>();
                if (value.ClassId != Guid.Empty && !IsInster)
                {
                    GoodsClassFieldIdList = _goodsCenterSao.GetFieldListByGoodsClassId(value.ClassId).Select(w => w.FieldId).ToList();
                }
            }
        }

        //设置获取属性选择状态
        private List<Guid> GoodsClassFieldIdList
        {
            get
            {
                return (from GridDataItem dataItem in FieldGrid.SelectedItems 
                        select new Guid(dataItem.GetDataKeyValue("FieldId").ToString())).ToList();
            }
            set
            {
                foreach (GridDataItem dataItem in FieldGrid.MasterTableView.Items)
                {
                    dataItem.Selected = value.Contains(new Guid(dataItem.GetDataKeyValue("FieldId").ToString()));
                }
            }
        }

        protected void InsterItem(object sender, EventArgs e)
        {
            FieldGrid.Enabled = true;
            Guid parentClassId;
            if (TVGoodsClass.SelectedNode == null)
            {
                parentClassId = Guid.Empty;
            }
            else
            {
                RadTreeNode currentNode = TVGoodsClass.SelectedNode;
                parentClassId = new Guid(currentNode.Value);
            }

            NonceGoodsClassInfo = new GoodsClassInfo
            {
                ClassId = Guid.NewGuid(),
                ParentClassId = parentClassId,
                ClassName = string.Empty
            };

            ControlPanel.Visible = false;
            IB_Add.Visible = true;
            LB_AddSpace.Visible = true;
            IB_Update.Visible = false;
            LB_UpdateSpace.Visible = false;
            IB_Cancel.Visible = true;
            DisableGoodsClassPanel(false);

        }

        //编辑选择的商品分类
        protected void EditItem(object sender, EventArgs e)
        {
            if (TVGoodsClass.SelectedNode != null)
            {
                RadTreeNode currentNode = TVGoodsClass.SelectedNode;
                var classId = new Guid(currentNode.Value);
                if (classId != Guid.Empty)
                {
                    var goodsClassInfo = _goodsCenterSao.GetClassDetail(classId);
                    NonceGoodsClassInfo = goodsClassInfo;
                    GoodsClassFieldIdList = goodsClassInfo.GoodsClassFieldList;
                    ControlPanel.Visible = false;
                    IB_Add.Visible = false;
                    LB_AddSpace.Visible = false;
                    IB_Update.Visible = true;
                    LB_UpdateSpace.Visible = true;
                    IB_Cancel.Visible = true;
                    DisableGoodsClassPanel(false);
                }
                else
                {
                    RAM.Alert("根节点不允许编辑。");
                }
            }
            else
            {
                RAM.Alert("您没有选择要编辑的商品分类！\n\n请从左边的商品分类树中选择要编辑的项。");
            }
        }

        //添加商品分类
        protected void Add_Click(object sender, EventArgs e)
        {
            try
            {
                GoodsClassInfo goodsClassInfo = NonceGoodsClassInfo;
                goodsClassInfo.GoodsClassFieldList = GoodsClassFieldIdList;
                string errorMessage;
                var result = _goodsCenterSao.AddClass(goodsClassInfo, out errorMessage);
                if (result)
                {
                    //商品分类添加操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsClassInfo.ClassId, "", 
                        OperationPoint.GoodsClassManager.Add.GetBusinessInfo(), "");

                    if (TVGoodsClass.SelectedNode == null)
                    {
                        RadTreeNode addNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                        TVGoodsClass.Nodes.Add(addNode);
                    }
                    else
                    {
                        RadTreeNode currentNode = TVGoodsClass.FindNodeByValue(goodsClassInfo.ParentClassId.ToString());
                        RadTreeNode addNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                        currentNode.Nodes.Add(addNode);
                        OpenTree(currentNode);
                        BindParentClass();
                    }
                    InsterItem(sender, e);
                }
                else
                {
                    RAM.Alert("商品分类添加失败！操作异常！" + errorMessage);
                }
            }
            catch
            {
                RAM.Alert("产品分类添加失败！");
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            GoodsClassInfo goodsClassInfo = NonceGoodsClassInfo;
            if (goodsClassInfo.ClassId != Guid.Empty)
                try
                {
                    if (TVGoodsClass.SelectedNode != null)
                    {
                        goodsClassInfo.GoodsClassFieldList = GoodsClassFieldIdList;
                        var goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(goodsClassInfo.ClassId, string.Empty);
                        var fieldList = _goodsCenterSao.GetFieldListByGoodsClassId(goodsClassInfo.ClassId).ToList();
                        if (goodsInfoList.Count > 0)
                        {
                            if (GoodsClassFieldIdList.Count != fieldList.Count)
                            {
                                RAM.Alert("该分类下有商品，不能更改商品分类的属性");
                                return;
                            }
                        }

                        string errorMessage;
                        var result = _goodsCenterSao.UpdateClass(goodsClassInfo, out errorMessage);
                        if (result)
                        {
                            // 操作记录添加
                            var personnelInfo = CurrentSession.Personnel.Get();
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsClassInfo.ClassId, "", 
                                OperationPoint.GoodsClassManager.Edit.GetBusinessInfo(), "");

                            RadTreeNode currentNode = TVGoodsClass.SelectedNode;
                            RadTreeNode aimNode = TVGoodsClass.FindNodeByValue(goodsClassInfo.ParentClassId.ToString());
                            currentNode.Text = goodsClassInfo.ClassName;
                            currentNode.ToolTip = goodsClassInfo.ClassName;
                            currentNode.CssClass = "TreeNode";
                            if (currentNode.Parent != null)
                            {
                                if (goodsClassInfo.ParentClassId != new Guid(currentNode.ParentNode.Value))
                                {
                                    currentNode.ParentNode.Nodes.Remove(currentNode);
                                    aimNode.Nodes.Add(currentNode);
                                    OpenTree(aimNode);
                                }
                            }
                        }
                        else
                        {
                            RAM.Alert("商品分类更改失败！" + errorMessage);
                        }
                    }
                }
                catch
                {
                    RAM.Alert("产品分类更改失败！");
                }
            else
            {
                RAM.Alert("根节点不允许更改！");
            }
        }

        protected void Delete_Click(object sender, EventArgs e)
        {
            if (TVGoodsClass.SelectedNode == null) return;
            var currentNode = TVGoodsClass.SelectedNode;
            var panterCurrentNode = currentNode.ParentNode;
            var classId = new Guid(currentNode.Value);
            if (classId != Guid.Empty)
            {
                try
                {
                    GoodsClassInfo goodsClassInfo = _goodsCenterSao.GetClassDetail(classId);
                    string errorMessage;
                    var result = _goodsCenterSao.DeleteClass(classId, out errorMessage);
                    if (result)
                    {
                        UnselectAllNodes(TVGoodsClass);

                        //商品分类添加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsClassInfo.ClassId, "", 
                            OperationPoint.GoodsClassManager.Delete.GetBusinessInfo(), "");

                        if (currentNode.Parent != null)
                        {
                            panterCurrentNode.Selected = true;
                            panterCurrentNode.Nodes.Remove(currentNode);
                            var nodeId = new Guid(panterCurrentNode.Value);
                            GoodsClassInfo goodsClassInfo2 = nodeId == Guid.Empty ? null : _goodsCenterSao.GetClassDetail(nodeId);
                            NonceGoodsClassInfo = goodsClassInfo2;
                        }
                        else
                        {
                            if (currentNode.Index > 0)
                            {
                                currentNode.TreeView.Nodes[currentNode.Index - 1].Selected = true;
                                var id = new Guid(currentNode.TreeView.Nodes[currentNode.Index - 1].Value);
                                GoodsClassInfo goodsClassInfo2 = id == Guid.Empty ? null : _goodsCenterSao.GetClassDetail(id);
                                NonceGoodsClassInfo = goodsClassInfo2;
                            }
                            TVGoodsClass.Nodes.Remove(currentNode);
                        }
                        DDL_ParentClass.SelectedValue = panterCurrentNode.Value;

                        if (panterCurrentNode.Value != Guid.Empty.ToString())
                            BindParentClass();
                    }
                    else
                    {
                        RAM.Alert("商品分类删除失败！" + errorMessage);
                    }
                }
                catch (Exception exp)
                {
                    RAM.Alert("商品分类信息删除失败！\n\n错误提示：" + exp.Message);
                }
            }
            else
            {
                RAM.Alert("根节点不允许删除！");
            }
        }

        //取消编辑或添加
        protected void Cancel_Click(object sender, EventArgs e)
        {
            ControlPanel.Visible = true;
            IB_Add.Visible = false;
            LB_AddSpace.Visible = false;
            IB_Update.Visible = false;
            LB_UpdateSpace.Visible = false;
            IB_Cancel.Visible = false;
            DisableGoodsClassPanel(true);
            if (TVGoodsClass.SelectedNode == null) return;
            var classId = new Guid(TVGoodsClass.SelectedNode.Value);
            var goodsClassInfo = classId == Guid.Empty ? null : _goodsCenterSao.GetClassDetail(classId);
            NonceGoodsClassInfo = goodsClassInfo;
        }

        private static void UnselectAllNodes(RadTreeView treeView)
        {
            foreach (RadTreeNode node in treeView.GetAllNodes())
            {
                node.Selected = false;
            }
        }

        private void DisableGoodsClassPanel(bool inputState)
        {
            TB_ClassName.ReadOnly = inputState;
            TB_OrderIndex.ReadOnly = inputState;
        }

        private bool IsInster
        {
            get
            {
                return IB_Add.Visible;
            }
        }
    }
}