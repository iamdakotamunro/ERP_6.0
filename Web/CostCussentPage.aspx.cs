using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Model.View;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>
    /// 费用单位开放权限
    /// </summary>
    public partial class CostCussentPage : BasePage
    {
        private readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Write);
        private readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);

        #region 初始化

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            RCB_FilialeId.ItemsRequested += RCB_FilialeId_ItemsRequested;
            RCB_BranchId.ItemsRequested += RCB_BranchId_ItemsRequested;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadFiliale();
            }
        }

        #region [初始化费用单位分类树形结构图]

        /// <summary>初始化费用单位分类树形结构图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RTVCostCussent_Init(object sender, EventArgs e)
        {
            GetCompanyCussent();
        }

        /// <summary>创建费用单位分类树
        /// </summary>
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("费用类型选择" + "[" + GetNonceCost(Guid.Empty, Guid.Empty) + "]", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            rootNode.PostBack = false;
            RTVCostCussent.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        /// <summary>遍历费用单位分类
        /// </summary>
        /// <param name="companyClassId">费用单位ID</param>
        /// <param name="node">节点</param>
        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            //var companyClass = new Cost();
            IList<CostCompanyClassInfo> childCompanyClassList = _costDao.GetChildCompanyClassList(companyClassId);
            foreach (CostCompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName + "[" + GetNonceCost(companyClassInfo.CompanyClassId, Guid.Empty) + "]", true, companyClassInfo.CompanyClassId.ToString());
                childNode.PostBack = false;
                node.Nodes.Add(childNode);
                RepetitionCompanyCussent(companyClassInfo.CompanyClassId, childNode);
            }
        }

        /// <summary>遍历费用单位
        /// </summary>
        /// <param name="companyClassId">费用单位ID</param>
        /// <param name="node">节点</param>
        private void RepetitionCompanyCussent(Guid companyClassId, RadTreeNode node)
        {
            IList<CostCussentInfo> companyCussentList = _costCussentDao.GetCompanyCussentList(companyClassId);
            foreach (CostCussentInfo companyCussentInfo in companyCussentList)
            {
                RadTreeNode childNode = CreateNode(companyCussentInfo.CompanyName + "[" + GetNonceCost(Guid.Empty, companyCussentInfo.CompanyId) + "]", false, companyCussentInfo.CompanyId.ToString());
                childNode.PostBack = true;
                node.Nodes.Add(childNode);
            }
        }

        /// <summary>创建节点
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="expanded">是否展开</param>
        /// <param name="id">值</param>
        /// <returns></returns>
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        /// <summary>获取费用单位账户余额
        /// </summary>
        /// <param name="companyClassId">费用单位分类</param>
        /// <param name="companyId">费用单位ID</param>
        /// <returns></returns>
        private string GetNonceCost(Guid companyClassId, Guid companyId)
        {
            if (companyClassId == Guid.Empty && companyId == Guid.Empty)
            {
                return string.Format("{0}", _costCussentDao.GetNonceCostByClassId(Guid.Empty));
            }
            if (companyClassId != Guid.Empty)
            {
                return string.Format("{0}", _costCussentDao.GetNonceCostByClassId(companyClassId));
            }
            if (companyId != Guid.Empty)
            {
                return string.Format("{0}", _costCussentDao.GetNonceCost(companyId));
            }
            return "0.00";
        }

        #endregion

        #region 选择往来单位分类树节点

        //选择往来单位分类树节点
        protected void RTVBankAccounts_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            CompanId = string.IsNullOrEmpty(e.Node.Value) ? Guid.Empty : new Guid(e.Node.Value);
            ReadCostPermissionData(FilialeId, BranchId, CompanId);
        }

        #endregion

        #region 绑定数据源

        protected void RgdGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = _costCussentDao.GetCostPermissionList(Guid.Empty, Guid.Empty, Guid.Empty);
            foreach (var item in list)
            {
                item.FilialeName = CacheCollection.Filiale.GetName(item.FilialeID);
                item.BranchName = CacheCollection.Branch.GetName(item.FilialeID, item.BranchID);
            }
            rgdGrid.DataSource = list;
        }

        private void ReadCostPermissionData(Guid filialeId, Guid branchId, Guid companyId)
        {
            var list = _costCussentDao.GetCostPermissionList(filialeId, branchId, companyId);
            foreach (var item in list)
            {
                item.FilialeName = CacheCollection.Filiale.GetName(item.FilialeID);
                item.BranchName = CacheCollection.Branch.GetName(item.FilialeID, item.BranchID);
            }
            rgdGrid.DataSource = list;
            rgdGrid.DataBind();
        }

        #endregion

        #region 公司部门选择后绑定事件

        private void RCB_FilialeId_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadFiliale();
        }

        private void RCB_BranchId_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadBranch(e.Text);
        }

        #endregion

        #region 加载公司 部门

        /// <summary>加载公司
        /// </summary>
        private void LoadFiliale()
        {
            RCB_FilialeId.DataValueField = "ID";
            RCB_FilialeId.DataTextField = "Name";
            RCB_FilialeId.DataSource = CacheCollection.Filiale.GetHeadList();
            RCB_FilialeId.DataBind();
            RCB_FilialeId.Items.Insert(0, new RadComboBoxItem());
        }

        /// <summary>加载公司部门
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        private void LoadBranch(string filialeId)
        {
            FilialeId = string.IsNullOrEmpty(filialeId) ? Guid.Empty : new Guid(filialeId);
            RCB_BranchId.DataValueField = "ID";
            RCB_BranchId.DataTextField = "Name";
            RCB_BranchId.DataSource = RecursionBranch(FilialeId, Guid.Empty);
            RCB_BranchId.DataBind();
            if (string.IsNullOrEmpty(filialeId))
            {
                RCB_BranchId.Items.Insert(0, new RadComboBoxItem());
            }

        }

        /// <summary>遍历公司部门
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <returns></returns>
        private static IList<BranchInfo> RecursionBranch(Guid filialeId, Guid branchId)
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

        #endregion

        #region 添加费用单位权限

        protected void LB_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(RCB_FilialeId.SelectedValue))
                FilialeId = new Guid(RCB_FilialeId.SelectedValue);
            else
            {
                RAM.Alert("系统提示：请选择公司!");
                return;
            }
            if (!string.IsNullOrEmpty(RCB_BranchId.SelectedValue))
                BranchId = new Guid(RCB_BranchId.SelectedValue);
            else
            {
                RAM.Alert("系统提示：请选择部门!");
                return;
            }
            if (CompanId == Guid.Empty)
            {
                RAM.Alert("系统提示：请选择相应的费用单位!");
                return;
            }

            try
            {
                _costCussentDao.AddCussionPersion(CompanId, FilialeId, BranchId);
                ReadCostPermissionData(Guid.Empty, Guid.Empty, CompanId);
            }
            catch
            {
                RAM.Alert("该用户已有此权限增加失败!");
            }
        }

        #endregion

        #region 删除费用单位权限

        public void RgdGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var bankAccountsid = new Guid(editedItem.GetDataKeyValue("CompanyId").ToString());
                var filialeId = new Guid(editedItem.GetDataKeyValue("FilialeId").ToString());
                var branchId = new Guid(editedItem.GetDataKeyValue("BranchId").ToString());
                _costCussentDao.DeleteCussionPersion(bankAccountsid, filialeId, branchId);
            }
            ReadCostPermissionData(FilialeId, BranchId, CompanId);
        }

        #endregion

        #region [ViewState]

        /// <summary>公司ID
        /// </summary>
        private Guid FilialeId
        {
            set
            {
                ViewState["FilialeId"] = value.ToString();
            }
            get
            {
                if (ViewState["FilialeId"] == null) return Guid.Empty;
                return new Guid(ViewState["FilialeId"].ToString());
            }
        }

        /// <summary>部门ID
        /// </summary>
        private Guid BranchId
        {
            set
            {
                ViewState["BranchId"] = value.ToString();
            }
            get
            {
                if (ViewState["BranchId"] == null) return Guid.Empty;
                return new Guid(ViewState["BranchId"].ToString());
            }
        }

        /// <summary>费用单位ID
        /// </summary>
        protected Guid CompanId
        {
            get
            {
                if (ViewState["CompanId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanId"].ToString());
            }
            set
            {
                ViewState["CompanId"] = value.ToString();
            }
        }
        #endregion
    }
}
