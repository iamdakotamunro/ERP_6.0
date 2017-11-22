using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Cache.Common;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using MIS.Enum.Attributes;
using MIS.Model.View;
using Telerik.Web.UI;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;

/*
 * 创建人：刘彩军
 * 创建时间：2011-June-08th
 */
namespace ERP.UI.Web
{
    /// <summary>往来收付款审核权限
    /// </summary>
    public partial class CompanyAuditingPower : BasePage
    {
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyAuditingPower _powerWrite = new DAL.Implement.Inventory.CompanyAuditingPower(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

        protected string GetFilialeName(object filialeId)
        {
            return CacheCollection.Filiale.GetName(filialeId.ToString().ToGuid());
        }

        protected string GetBranchName(object filialeId, object branchId)
        {
            return CacheCollection.Branch.GetName(filialeId.ToString().ToGuid(), branchId.ToString().ToGuid());
        }

        protected string GetPositionName(object filialeId, object branchId, object positionId)
        {
            return CacheCollection.Position.GetName(filialeId.ToString().ToGuid(), branchId.ToString().ToGuid(), positionId.ToString().ToGuid());
        }

        #region[页面加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetCompanyCussent();
            }
        }
        #endregion

        #region[绑定左侧往来单位树]
        //遍历往来的单位
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("往来单位选择", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            rootNode.PostBack = false;
            RT_CompanyClass.Nodes.Add(rootNode);
            #region  添加门店往来单位收付款审核权限节点 add by liangcanren at 2015-03-16
            var joinTypeList = ERP.Enum.Attribute.EnumAttribute.GetDict<ShopJoinType>();
            foreach (var joinTypeKey in joinTypeList.Keys)
            {
                RadTreeNode typeNode = CreateNode(string.Format("门店类型-{0}", joinTypeList[joinTypeKey]), false, Guid.Empty.ToString());
                typeNode.Category = "ShopJoinType";
                typeNode.Selected = true;
                typeNode.PostBack = false;
                RT_CompanyClass.Nodes.Add(typeNode);
                RecursivelyCompanyShop(typeNode,joinTypeKey);
            }
            RT_CompanyClass.Nodes.Add(rootNode);
            #endregion
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        /// <summary>
        /// 添加门店节点 
        /// add by liangcanren at 2015-03-16
        /// </summary>
        /// <param name="node"></param>
        /// <param name="shopJoinType"></param>
        private void RecursivelyCompanyShop(RadTreeNode node,int shopJoinType)
        {
            var shopList = CacheCollection.Filiale.GetShopList();
            if (shopList == null)
                return;
            shopList = shopList.Where(act => act.ShopJoinType == shopJoinType).ToList();
            foreach (var shopInfo in shopList)
            {
                RadTreeNode childNode = CreateNode(shopInfo.Name, false, shopInfo.ID.ToString());
                node.Nodes.Add(childNode);
            }
        }

        //遍历子公司
        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyClassInfo> childCompanyClassList = _companyClass.GetChildCompanyClassList(companyClassId).Where(c => c.CompanyClassId != _companyCussent.GetMemberGeneralLedger().CompanyClassId).ToList();
            foreach (CompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName, false, companyClassInfo.CompanyClassId.ToString());
                //childNode.PostBack = false;
                node.Nodes.Add(childNode);
                RecursivelyCompanyClass(companyClassInfo.CompanyClassId, childNode);
                RepetitionCompanyCussent(companyClassInfo.CompanyClassId, childNode);
            }
        }
        //遍历部门
        private void RepetitionCompanyCussent(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyCussentInfo> companyCussentList = _companyCussent.GetCompanyCussentList(companyClassId);
            foreach (CompanyCussentInfo companyCussentInfo in companyCussentList)
            {
                _companyCussent.GetNonceReckoningTotalled(companyCussentInfo.CompanyId);
                RadTreeNode childNode = CreateNode(companyCussentInfo.CompanyName, false, companyCussentInfo.CompanyId.ToString());
                node.Nodes.Add(childNode);
            }
        }
        //创建节点
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }
        #endregion

        #region[选择往来单位树节点]
        //选择往来单位分类树节点
        protected void RT_CompanyClass_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                CompanyID = new Guid(e.Node.Value);
                RG_Power.Rebind();
            }
        }
        #endregion

        #region[权限列表数据源]
        protected void RG_Power_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<CompanyAuditingPowerInfo> list = new List<CompanyAuditingPowerInfo>();
            if (CompanyID != Guid.Empty)
            {
                list = _powerWrite.GetCompanyAuditingPowerByCompanyID(CompanyID);
            }
            RG_Power.DataSource = list;
        }
        #endregion

        #region[修改权限]
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RG_Power_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var info = new CompanyAuditingPowerInfo();
            if (editedItem != null)
            {
                info.PowerID = new Guid(editedItem.GetDataKeyValue("PowerID").ToString());
                info.FilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.BranchID = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.PositionID = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.LowerMoney = decimal.Parse(((TextBox)editedItem.FindControl("TB_MinAmount")).Text);
                info.UpperMoney = decimal.Parse(((TextBox)editedItem.FindControl("TB_MaxAmount")).Text);
            }
            info.BindingType = (int)CompanyFundReceiptPowerBindType.DirectBind;
            info.CompanyID = CompanyID;
            info.ParentPowerID = Guid.Empty;
            try
            {
                _powerWrite.UpdateCompanyAuditingPower(info, 0);
                CacheHelper.Remove(Key.AllCompanyAuditingPower);
                IList<CompanyAuditingPowerInfo> companyList = _powerWrite.GetCompanyAuditingPowerByPowerID(info.PowerID);
                if (companyList.Count > 0)
                {
                    info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                    info.ParentPowerID = info.PowerID;
                    _powerWrite.UpdateCompanyAuditingPower(info, 1);
                    CacheHelper.Remove(Key.AllCompanyAuditingPower);
                }
                RG_Power.Rebind();
            }
            catch (Exception ex)
            {
                RAM.Alert("权限修改失败！" + ex);
            }
        }
        #endregion

        #region[添加权限]
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RG_Power_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var info = new CompanyAuditingPowerInfo();
            Guid parentPowerID = Guid.NewGuid();
            info.PowerID = parentPowerID;
            if (editedItem != null)
            {
                info.FilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.BranchID = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.PositionID = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.LowerMoney = decimal.Parse(((TextBox)editedItem.FindControl("TB_MinAmount")).Text);
                info.UpperMoney = decimal.Parse(((TextBox)editedItem.FindControl("TB_MaxAmount")).Text);
            }
            info.BindingType = (int)CompanyFundReceiptPowerBindType.DirectBind;
            info.CompanyID = CompanyID;
            info.ParentPowerID = Guid.Empty;
            try
            {
                _powerWrite.InsertCompanyAuditingPower(info);
                CacheHelper.Remove(Key.AllCompanyAuditingPower);
                CompanyClassInfo companyClassInfo = _companyClass.GetCompanyClass(CompanyID);
                if (companyClassInfo.CompanyClassId != Guid.Empty)
                {
                    IList<CompanyClassInfo> classList = _companyClass.GetChildCompanyClassList(CompanyID).ToList();
                    if (classList.Count > 0)
                    {
                        foreach (CompanyClassInfo cInfo in classList)
                        {
                            info.PowerID = Guid.NewGuid();
                            info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                            info.CompanyID = cInfo.CompanyClassId;
                            info.ParentPowerID = parentPowerID;
                            _powerWrite.InsertCompanyAuditingPower(info);
                            CacheHelper.Remove(Key.AllCompanyAuditingPower);
                            IList<CompanyCussentInfo> cussentList =_companyCussent.GetCompanyCussentList(cInfo.CompanyClassId).ToList();
                            if (cussentList.Count > 0)
                            {
                                foreach (CompanyCussentInfo cussentInfo in cussentList)
                                {
                                    info.PowerID = Guid.NewGuid();
                                    info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                                    info.CompanyID = cussentInfo.CompanyId;
                                    info.ParentPowerID = parentPowerID;
                                    _powerWrite.InsertCompanyAuditingPower(info);
                                    CacheHelper.Remove(Key.AllCompanyAuditingPower);
                                }
                            }
                        }
                    }
                    else
                    {
                        IList<CompanyCussentInfo> cussentList = _companyCussent.GetCompanyCussentList(CompanyID).ToList();
                        if (cussentList.Count > 0)
                        {
                            foreach (CompanyCussentInfo cussentInfo in cussentList)
                            {
                                info.PowerID = Guid.NewGuid();
                                info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                                info.CompanyID = cussentInfo.CompanyId;
                                info.ParentPowerID = parentPowerID;
                                _powerWrite.InsertCompanyAuditingPower(info);
                                CacheHelper.Remove(Key.AllCompanyAuditingPower);
                            }
                        }
                    }
                }
                RG_Power.Rebind();
            }
            catch (Exception ex)
            {
                RAM.Alert("权限添加失败！" + ex);
            }
        }
        #endregion

        #region[删除权限]
        protected void RG_Power_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;
            var powerId = new Guid(editedItem.GetDataKeyValue("PowerID").ToString());
            try
            {
                _powerWrite.DeleteCompanyAuditingPower(powerId);
                CacheHelper.Remove(Key.AllCompanyAuditingPower);
                RG_Power.Rebind();
            }
            catch (Exception ex)
            {
                RAM.Alert("删除权限失败!" + ex);
            }
        }
        #endregion

        #region[往来单位ID]
        protected Guid CompanyID
        {
            get
            {
                if (ViewState["CompanyID"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["CompanyID"].ToString());
            }
            set
            {
                ViewState["CompanyID"] = value;
            }
        }
        #endregion

        #region[加载公司]
        public List<FilialeInfo> LoadFiliale()
        {
            var list = new List<FilialeInfo>();
            var info = new FilialeInfo { Name = "请选择公司" };
            list.Add(info);
            list.AddRange(CacheCollection.Filiale.GetHeadList());
            return list;
        }
        #endregion
        
        #region[选择审批公司和部门]
        /// <summary>
        /// 选择审批公司和部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Filiale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlBranch = ((DropDownList)sender).Parent.FindControl("DDL_Branch") as DropDownList;
            var ddlFiliale = ((DropDownList)sender).Parent.FindControl("DDL_Filiale") as DropDownList;
            if (ddlBranch != null) ddlBranch.Items.Clear();
            if (ddlFiliale != null)
            {
                var filialeId = new Guid(ddlFiliale.SelectedValue);
                if (ddlBranch != null)
                {
                    ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
                    ddlBranch.DataTextField = "Name";
                    ddlBranch.DataValueField = "ID";
                    ddlBranch.DataBind();
                    ddlBranch.Items.Add(new ListItem("", Guid.Empty.ToString()));
                    ddlBranch.SelectedValue = Guid.Empty.ToString();
                }
            }
        }
        #endregion

        #region[选择审批部门]
        /// <summary>
        /// 选择审批部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Branch_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDownList = sender as DropDownList;
            if (dropDownList != null)
            {
                var ddlPosition = dropDownList.Parent.FindControl("DDL_Position") as DropDownList;
                var ddlBranch = dropDownList.Parent.FindControl("DDL_Branch") as DropDownList;
                var ddlFiliale = dropDownList.Parent.FindControl("DDL_Filiale") as DropDownList;
                if (ddlPosition != null) ddlPosition.Items.Clear();
                if (ddlFiliale != null)
                {
                    var filialeId = new Guid(ddlFiliale.SelectedValue);
                    if (ddlBranch != null)
                    {
                        var branchId = new Guid(ddlBranch.SelectedValue);
                        if (ddlPosition != null)
                        {
                            ddlPosition.DataSource = CacheCollection.Position.GetList(filialeId, branchId);
                            ddlPosition.DataTextField = "Name";
                            ddlPosition.DataValueField = "ID";
                            ddlPosition.DataBind();
                            ddlPosition.Items.Add(new ListItem("", "-1"));
                            ddlPosition.SelectedValue = "-1";
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>显示所有部门（包括所有子部门）
        /// </summary>
        public static IList<BranchInfo> RecursionBranch(Guid filialeId, Guid branchId, int depth)
        {
            IList<BranchInfo> branchsTree = new List<BranchInfo>();
            IList<BranchInfo> branchsList = CacheCollection.Branch.GetList(filialeId, branchId);
            foreach (var branchInfo in branchsList)
            {
                branchInfo.Name = branchInfo.Name;
                branchsTree.Add(branchInfo);
            }
            return branchsTree;
        }
    }
}
