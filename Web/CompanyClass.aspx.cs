using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class CompanyClassAw : BasePage
    {
        private readonly ICompanyClass _companyClass = new CompanyClass();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IB_Add.Visible = false;
                LBAddSpace.Visible = false;
                IB_Update.Visible = false;
                LBUpdateSpace.Visible = false;
                IB_Cancel.Visible = false;
                NonceCompanyClass = new CompanyClassInfo();
                GetTreeCompanyClass();
            }
        }

        //获取往来单位分类树
        private void GetTreeCompanyClass()
        {
            RadTreeNode rootNode = CreateNode("往来单位分类", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            TV_CompanyClass.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyClassInfo> childcompanyClassList = _companyClass.GetChildCompanyClassList(companyClassId);
            foreach (CompanyClassInfo childCompanyClass in childcompanyClassList)
            {
                RadTreeNode childNode =
                    CreateNode(childCompanyClass.CompanyClassName + "(" + childCompanyClass.CompanyClassCode + ")",
                               false, childCompanyClass.CompanyClassId.ToString());
                node.Nodes.Add(childNode);
                RecursivelyCompanyClass(childCompanyClass.CompanyClassId, childNode);
            }
        }

        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) {ToolTip = text, Expanded = expanded};
            return node;
        }

        //往来单位信息属性化
        protected CompanyClassInfo NonceCompanyClass
        {
            get
            {
                if (string.IsNullOrEmpty(HF_CompanyClassId.Value) &&
                    string.IsNullOrEmpty(HF_ParentCompanyClassId.Value) &&
                    string.IsNullOrEmpty(TB_CompanyClassCode.Text) &&
                    string.IsNullOrEmpty(TB_CompanyClassName.Text))
                    return null;
                var companyClassId = new Guid(HF_CompanyClassId.Value);
                var parentCompanyClassId = new Guid(HF_ParentCompanyClassId.Value);
                string companyClassCode = TB_CompanyClassCode.Text;
                string companyClassName = TB_CompanyClassName.Text;
                return new CompanyClassInfo(companyClassId, parentCompanyClassId, companyClassCode, companyClassName);
            }
            set
            {
                if (value == null) value = new CompanyClassInfo();
                HF_CompanyClassId.Value = value.CompanyClassId.ToString();
                HF_ParentCompanyClassId.Value = value.ParentCompanyClassId.ToString();
                TB_CompanyClassCode.Text = value.CompanyClassCode;
                TB_CompanyClassName.Text = value.CompanyClassName;
            }
        }

        //选择添加新的往来单位分类
        protected void InsterItem(object sender, EventArgs e)
        {
            Guid parentCompanyClassId;
            if (TV_CompanyClass.SelectedNode == null)
            {
                parentCompanyClassId = Guid.Empty;
            }
            else
            {
                RadTreeNode currentNode = TV_CompanyClass.SelectedNode;
                parentCompanyClassId = new Guid(currentNode.Value);
            }
            var companyClassInfo = new CompanyClassInfo(Guid.NewGuid(), parentCompanyClassId, null, null);
            NonceCompanyClass = companyClassInfo;
            ControlPanel.Visible = false;
            IB_Add.Visible = true;
            LBAddSpace.Visible = true;
            IB_Update.Visible = false;
            LBUpdateSpace.Visible = false;
            IB_Cancel.Visible = true;
            TB_CompanyClassCode.ReadOnly = false;
            TB_CompanyClassName.ReadOnly = false;
        }

        //编辑选择的往来的单位分类
        protected void EditItem(object sender, EventArgs e)
        {
            if (TV_CompanyClass.SelectedNode != null)
            {
                RadTreeNode currentNode = TV_CompanyClass.SelectedNode;
                var companyClassId = new Guid(currentNode.Value);
                if (companyClassId != Guid.Empty)
                {
                    NonceCompanyClass = _companyClass.GetCompanyClass(companyClassId);
                    ControlPanel.Visible = false;
                    IB_Add.Visible = false;
                    LBAddSpace.Visible = false;
                    IB_Update.Visible = true;
                    LBUpdateSpace.Visible = true;
                    IB_Cancel.Visible = true;
                    TB_CompanyClassCode.ReadOnly = false;
                    TB_CompanyClassName.ReadOnly = false;
                }
                else
                {
                    RAM.Alert("根节点不允许编辑。");
                }
            }
            else
            {
                RAM.Alert("您没有选择要编辑的往来单位分类！\n\n请从左边的往来单位分类树中选择要编辑的往来单位分类。");
            }
        }

        //添加往来单位分类
        protected void Add_Click(object sender, EventArgs e)
        {
            if(NonceCompanyClass==null)return;
            CompanyClassInfo companyClassInfo = NonceCompanyClass;
            try
            {
                _companyClass.Insert(companyClassInfo);
                if (TV_CompanyClass.SelectedNode == null)
                {
                    RadTreeNode addNode =
                        CreateNode(companyClassInfo.CompanyClassName + "(" + companyClassInfo.CompanyClassCode + ")",
                                   false, companyClassInfo.CompanyClassId.ToString());
                    TV_CompanyClass.Nodes.Add(addNode);
                }
                else
                {
                    RadTreeNode currentNode = TV_CompanyClass.SelectedNode;
                    RadTreeNode addNode =
                        CreateNode(companyClassInfo.CompanyClassName + "(" + companyClassInfo.CompanyClassCode + ")",
                                   false, companyClassInfo.CompanyClassId.ToString());
                    currentNode.Nodes.Add(addNode);
                    currentNode.Expanded = true;
                }
                InsterItem(sender, e);
            }
            catch
            {
                RAM.Alert("往来单位分类添加失败！");
            }
        }

        //编辑分类
        protected void Update_Click(object sender, EventArgs e)
        {
            CompanyClassInfo companyClassInfo = NonceCompanyClass;
            if (companyClassInfo.CompanyClassId != Guid.Empty)
                try
                {
                    if (TV_CompanyClass.SelectedNode != null)
                    {
                        _companyClass.Update(companyClassInfo);
                        RadTreeNode currentNode = TV_CompanyClass.SelectedNode;
                        currentNode.Text = companyClassInfo.CompanyClassName + "(" + companyClassInfo.CompanyClassCode +
                                           ")";
                        currentNode.ToolTip = companyClassInfo.CompanyClassName + "(" +
                                              companyClassInfo.CompanyClassCode + ")";
                    }
                }
                catch
                {
                    RAM.Alert("往来单位分类更改失败！");
                }
            else
            {
                RAM.Alert("根节点不允许更改！");
            }
        }

        //删除选择的往来单位分类
        protected void Delete_Click(object sender, EventArgs e)
        {
            if (TV_CompanyClass.SelectedNode != null)
            {
                RadTreeNode currentNode = TV_CompanyClass.SelectedNode;
                RadTreeNode parentCurrentNode = currentNode.ParentNode;
                var companyClassId = new Guid(currentNode.Value);
                if (companyClassId != Guid.Empty)
                {
                    try
                    {
                        var companyClass=new BLL.Implement.Inventory.CompanyClass(_companyClass);
                        companyClass.Delete(companyClassId);

                        UnselectAllNodes(TV_CompanyClass);

                        if (currentNode.Parent != null)
                        {
                            parentCurrentNode.Selected = true;
                            parentCurrentNode.Nodes.Remove(currentNode);
                            NonceCompanyClass = _companyClass.GetCompanyClass(new Guid(parentCurrentNode.Value));
                        }
                        else
                        {
                            if (currentNode.Index > 0)
                            {
                                currentNode.TreeView.Nodes[currentNode.Index - 1].Selected = true;
                                NonceCompanyClass =
                                    _companyClass.GetCompanyClass(
                                        new Guid(currentNode.TreeView.Nodes[currentNode.Index - 1].Value));
                            }
                            TV_CompanyClass.Nodes.Remove(currentNode);
                        }
                    }
                    catch (Exception exp)
                    {
                        RAM.Alert("往来单位分类删除失败！\n\n错误提示：" + exp.Message);
                    }
                }
                else
                {
                    RAM.Alert("根节点不允许删除！");
                }
            }
        }

        private void UnselectAllNodes(RadTreeView treeView)
        {
            foreach (RadTreeNode node in treeView.GetAllNodes())
            {
                node.Selected = false;
            }
        }

        //取消编辑或添加
        protected void Cancel_Click(object sender, EventArgs e)
        {
            ControlPanel.Visible = true;
            IB_Add.Visible = false;
            LBAddSpace.Visible = false;
            IB_Update.Visible = false;
            LBUpdateSpace.Visible = false;
            IB_Cancel.Visible = false;
            TB_CompanyClassCode.ReadOnly = true;
            TB_CompanyClassName.ReadOnly = true;
            if (TV_CompanyClass.SelectedNode != null)
            {
                var companyClassId = new Guid(TV_CompanyClass.SelectedNode.Value);
                NonceCompanyClass = _companyClass.GetCompanyClass(companyClassId);
            }
        }

        //选择往来单位分类树节点
        protected void TvCompanyClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            var companyClassId = new Guid(e.Node.Value);
            if (!IsInster)
            {
                NonceCompanyClass = companyClassId == Guid.Empty ? new CompanyClassInfo() : _companyClass.GetCompanyClass(companyClassId);
            }
            else
            {
                NonceCompanyClass = new CompanyClassInfo(Guid.NewGuid(), companyClassId, null, null);
            }
        }

        //编辑器所处的当前状态
        private bool IsInster
        {
            get { return IB_Add.Visible; }
        }
    }
}
