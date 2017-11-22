using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Model.View;
using Telerik.Web.UI;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;

namespace ERP.UI.Web
{
    /// <summary>往来收付款发票权限
    /// </summary>
    public partial class CompanyInvoicePower : BasePage
    {
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IPersonnelSao _personnelSao=new PersonnelSao();
        private readonly ICompanyInvoicePower _companyInvoicePower=new DAL.Implement.Inventory.CompanyInvoicePower(GlobalConfig.DB.FromType.Write);
        readonly Dictionary<Guid,string> _dics=new Dictionary<Guid, string>(); 

        #region[页面加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetCompanyCussent();
            }
        }
        #endregion

        protected string GetFilialeName(object filialeId)
        {
            return CacheCollection.Filiale.GetName(new Guid(filialeId.ToString()));
        }

        protected string GetBranchName(object filialeId, object branchId)
        {
            return CacheCollection.Branch.GetName(new Guid(filialeId.ToString()), new Guid(branchId.ToString()));
        }

        protected string GetPositionName(object filialeId, object branchId, object positionId)
        {
            return CacheCollection.Position.GetName(new Guid(filialeId.ToString()), new Guid(branchId.ToString()), new Guid(positionId.ToString()));
        }

        protected string GetPersonnelName(object personnelId)
        {
            var id = new Guid(personnelId.ToString());
            if (_dics.ContainsKey(id)) return _dics[id];
            var name = _personnelSao.GetName(id);
            _dics.Add(id,name);
            return name;
        }

        #region[绑定左侧往来单位树]
        //遍历往来的单位
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("往来单位选择", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            rootNode.PostBack = false;
            RT_CompanyClass.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
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
            if (CompanyID != Guid.Empty)
            {
                RG_Power.DataSource = _companyInvoicePower.GetCompanyInvoicePowerByCompanyID(CompanyID);
                return;
            }
            RG_Power.DataSource = new List<CompanyInvoicePowerInfo>();
        }
        #endregion

        protected void RgPower_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.EditFormItem)
            {
                var editableItem = (GridEditableItem)e.Item;
                //发票类型、审批人公司、部门、职务和审批人
                var ddlInvoiceType = (DropDownList)editableItem.FindControl("DDL_InvoiceType");
                var ddlFiliale = (DropDownList)editableItem.FindControl("DDL_Filiale");
                var ddlBranch = (DropDownList)editableItem.FindControl("DDL_Branch");
                var ddlPosition = (DropDownList)editableItem.FindControl("DDL_Position");
                var ddlPersonnel = (DropDownList)editableItem.FindControl("DDL_Personnel");
                var hfInvoicesType = (HiddenField)editableItem.FindControl("hfInvoicesType");
                var hfFilialeId = (HiddenField)editableItem.FindControl("hfFilialeId");
                var hfBranchId = (HiddenField)editableItem.FindControl("hfBranchId");
                var hfPositionId = (HiddenField)editableItem.FindControl("hfPositionId");
                var hfPersonnel = (HiddenField)editableItem.FindControl("hfPersonnel");

                if (hfInvoicesType != null)
                {
                    ddlInvoiceType.SelectedValue = hfInvoicesType.Value;
                }

                if (hfFilialeId != null)
                {
                    var filialeId = hfFilialeId.Value == string.Empty ? Guid.Empty : new Guid(hfFilialeId.Value);
                    ddlFiliale.SelectedValue = filialeId.ToString();
                    if (hfBranchId != null)
                    {
                        var branchId = hfBranchId.Value == string.Empty ? Guid.Empty : new Guid(hfBranchId.Value);
                        ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
                        ddlBranch.DataTextField = "Name";
                        ddlBranch.DataValueField = "ID";
                        ddlBranch.DataBind();
                        ddlBranch.Items.Add(new ListItem("", Guid.Empty.ToString()));
                        ddlBranch.SelectedValue = branchId.ToString();
                        if (hfPositionId != null)
                        {
                            var positionId = hfPositionId.Value == string.Empty ? Guid.Empty : new Guid(hfPositionId.Value);
                            ddlPosition.DataSource = CacheCollection.Position.GetList(filialeId, branchId);
                            ddlPosition.DataTextField = "Name";
                            ddlPosition.DataValueField = "ID";
                            ddlPosition.DataBind();
                            ddlPosition.Items.Add(new ListItem("", Guid.Empty.ToString()));
                            ddlPosition.SelectedValue = positionId.ToString();
                            if (hfPersonnel != null)
                            {
                                var personnelId = hfPersonnel.Value == string.Empty ? Guid.Empty : new Guid(hfPersonnel.Value);
                                ddlPersonnel.Items.Clear();
                                ddlPersonnel.DataSource = _personnelSao.GetList().Where(item => item.FilialeId == filialeId && item.BranchId == branchId && item.PositionId == positionId);
                                ddlPersonnel.DataTextField = "RealName";
                                ddlPersonnel.DataValueField = "PersonnelId";
                                ddlPersonnel.DataBind();
                                ddlPersonnel.Items.Add(new ListItem("", Guid.Empty.ToString()));
                                ddlPersonnel.SelectedValue = personnelId.ToString();
                            }
                        }
                    }
                }
            }
        }

        #region[修改权限]
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RG_Power_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var info = new CompanyInvoicePowerInfo();
            if (editedItem != null)
            {
                info.PowerID = new Guid(editedItem.GetDataKeyValue("PowerID").ToString());
                info.FilialeID = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.BranchID = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.PositionID = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.AuditorID = new Guid(((DropDownList)editedItem.FindControl("DDL_Personnel")).SelectedValue);
                info.InvoicesType = int.Parse(((DropDownList)editedItem.FindControl("DDL_InvoiceType")).SelectedValue);
            }
            info.BindingType = (int)CompanyFundReceiptPowerBindType.DirectBind;
            info.CompanyID = CompanyID;
            info.ParentPowerID = Guid.Empty;
            try
            {
                _companyInvoicePower.UpdateCompanyInvoicePower(info, 0);
                IList<CompanyInvoicePowerInfo> companyList = _companyInvoicePower.GetCompanyInvoicePowerByPowerID(info.PowerID);
                if (companyList.Count > 0)
                {
                    info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                    info.ParentPowerID = info.PowerID;
                    _companyInvoicePower.UpdateCompanyInvoicePower(info, 1);
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
            var info = new CompanyInvoicePowerInfo();
            Guid parentPowerID = Guid.NewGuid();
            info.PowerID = parentPowerID;
            if (editedItem != null)
            {
                info.FilialeID = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.BranchID = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.PositionID = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.AuditorID = new Guid(((DropDownList)editedItem.FindControl("DDL_Personnel")).SelectedValue);
                info.InvoicesType = int.Parse(((DropDownList)editedItem.FindControl("DDL_InvoiceType")).SelectedValue);
            }
            info.BindingType = (int)CompanyFundReceiptPowerBindType.DirectBind;
            info.CompanyID = CompanyID;
            info.ParentPowerID = Guid.Empty;
            try
            {
                _companyInvoicePower.InsertCompanyInvoicePower(info);
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
                            _companyInvoicePower.InsertCompanyInvoicePower(info);

                            IList<CompanyCussentInfo> cussentList = _companyCussent.GetCompanyCussentList(cInfo.CompanyClassId).ToList();
                            if (cussentList.Count > 0)
                            {
                                foreach (CompanyCussentInfo cussentInfo in cussentList)
                                {
                                    info.PowerID = Guid.NewGuid();
                                    info.BindingType = (int)CompanyFundReceiptPowerBindType.ExpandBind;
                                    info.CompanyID = cussentInfo.CompanyId;
                                    info.ParentPowerID = parentPowerID;
                                    _companyInvoicePower.InsertCompanyInvoicePower(info);
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
                                _companyInvoicePower.InsertCompanyInvoicePower(info);
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
                _companyInvoicePower.DeleteCompanyInvoicePower(powerId);
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

        #region[选择审批公司]
        /// <summary>
        /// 选择审批公司
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Filiale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDownList = sender as DropDownList;
            if (dropDownList != null)
            {
                var ddlBranch = dropDownList.Parent.FindControl("DDL_Branch") as DropDownList;
                var ddlFiliale = dropDownList.Parent.FindControl("DDL_Filiale") as DropDownList;
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

        #region[选择审批职务]
        /// <summary>
        /// 选择审批职务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Position_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDownList = sender as DropDownList;
            if (dropDownList != null)
            {
                var ddlPosition = dropDownList.Parent.FindControl("DDL_Position") as DropDownList;
                var ddlBranch = dropDownList.Parent.FindControl("DDL_Branch") as DropDownList;
                var ddlFiliale = dropDownList.Parent.FindControl("DDL_Filiale") as DropDownList;
                var ddlPersonnel = dropDownList.Parent.FindControl("DDL_Personnel") as DropDownList;
                if (ddlFiliale != null)
                {
                    var filialeId = new Guid(ddlFiliale.SelectedValue);
                    if (ddlBranch != null)
                    {
                        var branchId = new Guid(ddlBranch.SelectedValue);
                        if (ddlPosition != null)
                        {
                            var positionId = new Guid(ddlPosition.SelectedValue);
                            if (ddlPersonnel != null)
                            {
                                ddlPersonnel.Items.Clear();
                                ddlPersonnel.DataSource = _personnelSao.GetList().Where(item => item.FilialeId == filialeId && item.BranchId == branchId && item.PositionId == positionId);
                                ddlPersonnel.DataTextField = "RealName";
                                ddlPersonnel.DataValueField = "PersonnelId";
                                ddlPersonnel.DataBind();
                                ddlPersonnel.Items.Add(new ListItem("", Guid.Empty.ToString()));
                                ddlPersonnel.SelectedValue = Guid.Empty.ToString();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region[绑定发票类型枚举]
        public Dictionary<int, string> GetInvoiceType()
        {
            var dict = new Dictionary<int, string> { { -1, "请选择发票类型" } };
            var dt = EnumAttribute.GetDict<CompanyFundReceiptInvoiceType>();
            foreach (var t in dt)
            {
                dict.Add(t.Key, t.Value);
            }
            return dict;
        }
        #endregion

        public string GetEnumIntro(object enumIndex)
        {
            var enumKey = (CompanyFundReceiptInvoiceType)int.Parse(enumIndex.ToString());
            return EnumAttribute.GetKeyName(enumKey);
        }

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
