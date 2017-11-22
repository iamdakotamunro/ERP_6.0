using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
   /// <summary>費用類型
   /// </summary>
    public partial class CostAccountClassAw : BasePage
    {
        private readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Write);
        private readonly ICost _cost = new Cost(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeCompanyClass();
            }
        }

        //获取往来单位分类树
        private void GetTreeCompanyClass()
        {
            RadTreeNode rootNode = CreateNode("费用分类", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RT_CompanyClass.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {

            IList<CostCompanyClassInfo> childcompanyClassList = _cost.GetChildCompanyClassList(companyClassId);
            foreach (CostCompanyClassInfo childCompanyClass in childcompanyClassList)
            {
                RadTreeNode childNode = CreateNode(childCompanyClass.CompanyClassName, false, childCompanyClass.CompanyClassId.ToString());
                node.Nodes.Add(childNode);
                RecursivelyCompanyClass(childCompanyClass.CompanyClassId, childNode);
            }
        }

        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) {ToolTip = text, Expanded = expanded};
            return node;
        }

        protected void RGCussent_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Guid companyClassId = Guid.Empty;
            if (RT_CompanyClass.SelectedNode != null)
            {
                RadTreeNode currentNode = RT_CompanyClass.SelectedNode;
                companyClassId = new Guid(currentNode.Value);
            }
            RGCussent.DataSource = companyClassId == Guid.Empty ? _costCussentDao.GetCompanyCussentList() : _costCussentDao.GetCompanyCussentList(companyClassId);
        }

        //选择往来单位分类树节点
        protected void RT_CompanyClass_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
                RGCussent.Rebind();
        }

        protected void RGCussent_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;
            var companyId = new Guid(editedItem.GetDataKeyValue("CompanyId").ToString());
            try
            {
                if (companyId!=Guid.Empty)
                {
                    
                    if (Math.Abs(_costCussentDao.GetNonceReckoningTotalled(companyId)) > 0)
                    {
                        RAM.Alert("该单位往来账目未平，不允许删除！");
                        return;
                    }

                    if (_costCussentDao.IsExpress(companyId))
                    {
                        RAM.Alert("该单位往来账目被快递公司绑定，不允许删除！");
                        return;
                    }
                    if (_costCussentDao.IsMemberGeneralLedger(companyId))
                    {
                        RAM.Alert("该单位往来账户被设为会员总账户，不允许删除！");
                        return;
                    }
                    _costCussentDao.DeleteCussionPersion(companyId);
                    _costCussentDao.Delete(companyId);
                }
            }
            catch (Exception exp)
            {
                RAM.Alert("费用分类删除失败！\\n\\n错误提示：" + exp.Message);
            }
        }

       protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
       {
           WebControl.RamAjajxRequest(RGCussent, e);
       }
    }
}
