using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

/*
 * 最后修改人：刘彩军
 * 修改时间：2011-September-21th
 * 修改内容：将编辑添加功能从列表中分离出来
 */
namespace ERP.UI.Web
{
    public partial class CompanyCussentAw : BasePage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussentRelation _companyCussentRelation = new CompanyCussentRelation(GlobalConfig.DB.FromType.Read);

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
            RadTreeNode rootNode = CreateNode("往来单位分类", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RT_CompanyClass.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyClassInfo> childcompanyClassList = _companyClass.GetChildCompanyClassList(companyClassId);
            foreach (CompanyClassInfo childCompanyClass in childcompanyClassList)
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

        /// <summary>
        /// 绑定GRID数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgCussentNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = _companyCussent.GetCompanyCussentList();
            if (RT_CompanyClass.SelectedNode != null)
            {
                RadTreeNode currentNode = RT_CompanyClass.SelectedNode;
                var companyClassId = new Guid(currentNode.Value);
                if (companyClassId != Guid.Empty)
                    list = list.Where(act => act.CompanyClassId == companyClassId).ToList();
            }
            if (!string.IsNullOrEmpty(SearchKey))
            {
                list=list.Where(ent => ent.CompanyName.Contains(SearchKey)).ToList();
                if (CompanyClassId != Guid.Empty)
                {
                    list=list.Where(ent => ent.CompanyClassId == CompanyClassId).ToList();
                }
            }
            RGCussent.DataSource = list;
        }

        //选择往来单位分类树节点
        protected void RtCompanyClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            SearchKey = string.Empty;
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                CompanyClassId = new Guid(e.Node.Value);
                RGCussent.Rebind();
            }
        }

        protected void RgCussentDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var companyId = new Guid(editedItem.GetDataKeyValue("CompanyId").ToString());
                var salesScope = Convert.ToInt32(editedItem.GetDataKeyValue("SalesScope"));
                try
                {
                    if (companyId!=Guid.Empty)
                    {
                        if (Math.Abs(_companyCussent.GetNonceReckoningTotalled(companyId)) > 0)
                        {
                            RAM.Alert("该单位往来账目未平，不允许删除！");
                            return;
                        }
                        if (_companyCussent.IsExpress(companyId))
                        {
                            RAM.Alert("该单位往来账目被快递公司绑定，不允许删除！");
                            return;
                        }
                        if (_companyCussent.IsMemberGeneralLedger(companyId))
                        {
                            RAM.Alert("该单位往来账户被设为会员总账户，不允许删除！");
                            return;
                        }
                        if (salesScope==(int)Enum.Overseas.SalesScopeType.Overseas && _companyCussentRelation.IsExist(companyId))
                        {
                            RAM.Alert("请先删除该往来单位对应的境外登录权限设置信息！");
                            return;
                        }
                        _companyCussent.Delete(companyId);
                    }
                }
                catch (Exception exp)
                {
                    RAM.Alert("往来单位删除失败！\\n\\n错误提示：" + exp.Message);
                }
            }
        }

        #region[Ajax页面返回]
        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGCussent, e);
        }
        #endregion

        #region[获取下载商品资料地址]
        /// <summary>
        /// 获取下载商品资料地址
        /// </summary>
        /// <param name="path">资料路径</param>
        /// <returns></returns>
        public string GetDownPath(string path)
        {
            string returnPath = "";
            if (!string.IsNullOrEmpty(path))
            {
                returnPath = "./UserControl/DownloadPage.aspx?tag=companyInformation&fullname=" + HttpUtility.UrlEncode(Server.MapPath(path));
            }
            return returnPath;
        }
        #endregion

        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set
            {
                ViewState["SearchKey"] = value;
            }
        }

        protected Guid CompanyClassId
        {
            get
            {
                if (ViewState["CompanyClassId"] == null) return Guid.Empty;
                return (Guid)ViewState["CompanyClassId"];
            }
            set
            {
                ViewState["CompanyClassId"] = value;
            }
        }

        protected void OnItemRgCussent(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                SearchKey = ((TextBox)e.Item.FindControl("TB_Search")).Text.Trim();
                RGCussent.CurrentPageIndex = 0;
                RGCussent.Rebind();
            }
        }
    }
}