using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Model.View;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary> 银行账户开放权限
    /// </summary>
    public partial class BankAccountPermission : BasePage
    {
        private double _countBank;
        private readonly IBankAccounts _bankAccountsWrite = new BankAccounts(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccountDaoRead = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));

        protected Guid BankAccountsId
        {
            get
            {
                if (ViewState["BankAccountsId"] == null) return Guid.Empty;
                return new Guid(ViewState["BankAccountsId"].ToString());
            }
            set { ViewState["BankAccountsId"] = value.ToString(); }
        }

        /// <summary>
        /// 树节点公司
        /// </summary>
        private Guid FilialeBankId
        {
            set { ViewState["FilialeBankId"] = value.ToString(); }
            get
            {
                if (ViewState["FilialeBankId"] == null) return Guid.Empty;
                return new Guid(ViewState["FilialeBankId"].ToString());
            }
        }

        private Guid FilialeId
        {
            set { Session["FilialeId"] = value.ToString(); }
            get
            {
                if (Session["FilialeId"] == null) return Guid.Empty;
                return new Guid(Session["FilialeId"].ToString());
            }
        }

        private Guid BranchId
        {
            set { Session["BranchId"] = value.ToString(); }
            get
            {
                if (Session["BranchId"] == null) return Guid.Empty;
                return new Guid(Session["BranchId"].ToString());
            }
        }

        protected Guid PositionId
        {
            get
            {
                if (ViewState["PositionID"] == null) return Guid.Empty;
                return new Guid(ViewState["PositionID"].ToString());
            }
            set { ViewState["PositionID"] = value.ToString(); }
        }

        protected String TradeCode
        {
            get
            {
                if (ViewState["TradeCode"] == null) return String.Empty;
                return ViewState["TradeCode"].ToString();
            }
            set { ViewState["TradeCode"] = value; }
        }

        #region 初始化
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }


        private void InitializeComponent()
        {
            RCB_FilialeId.ItemsRequested += RcbFilialeIdItemsRequested;
            RCB_BranchId.ItemsRequested += RcbBranchIdItemsRequested;
            RCB_PositionId.ItemsRequested += RcbPositionIdItemsRequested;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetBankAccounts();
                LoadFiliale();
            }
        }

        //根目录资金账户
        private void GetBankAccounts()
        {
            RTVBankAccounts.Nodes.Clear();
            RecursivelyFiliale();
        }

        /// <summary>
        /// 遍历公司
        /// </summary>
        private void RecursivelyFiliale()
        {
            foreach (var info in CacheCollection.Filiale.GetHostingAndSaleFilialeList())
            {
                //载入公司
                var childNode = CreateNode(info.Name, true, info.ID.ToString(), "Filiale");
                childNode.PostBack = false;
                RTVBankAccounts.Nodes.Add(childNode);
                RecursivelyBankAccounts(childNode, info.ID);
            }
            //其他公司
            var elseChildNode = CreateNode("ERP", false, _reckoningElseFilialeid.ToString(), "Filiale");
            elseChildNode.PostBack = false;
            RTVBankAccounts.Nodes.Add(elseChildNode);
            RecursivelyBankAccounts(elseChildNode, _reckoningElseFilialeid);
        }

        //遍历资金帐户
        private void RecursivelyBankAccounts(IRadTreeNodeContainer node, Guid filialeId)
        {
            if (_reckoningElseFilialeid == filialeId)
            {
                var bankAccountsInfoList = _bankAccountsWrite.GetList().Where(ent => !ent.IsMain).OrderByDescending(ent => ent.IsUse).ToList();
                foreach (var bankAccountsInfo in bankAccountsInfoList)
                {
                    var childNode = CreateNode((bankAccountsInfo.IsUse ? "" : "【停用】") + bankAccountsInfo.BankName + "[" + GetBankAccountsCount(bankAccountsInfo.BankAccountsId) + "]", false, bankAccountsInfo.BankAccountsId.ToString(), "BankAccount");
                    node.Nodes.Add(childNode);
                }
            }
            else
            {
                var bankAccountsInfoList = _bankAccountDaoRead.GetListByTargetId(filialeId).OrderByDescending(ent => ent.IsUse).ToList();
                foreach (var bankAccountsInfo in bankAccountsInfoList)
                {
                    
                    var childNode = CreateNode(string.Format("{0}{1}[{2}]", !bankAccountsInfo.IsUse ? "【停用】" : "", bankAccountsInfo.BankName, GetBankAccountsCount(bankAccountsInfo.BankAccountsId)), false, bankAccountsInfo.BankAccountsId.ToString(), "BankAccount");
                    node.Nodes.Add(childNode);
                }
            }
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id, string category)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded, Category = category };
            return node;
        }

        #region 选择银行账号树节点
        //选择银行账号树节点
        protected void RtvBankAccountsNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (e.Node.Category == "BankAccount")
            {
                FilialeBankId = Guid.Empty;
                BankAccountsId = string.IsNullOrEmpty(e.Node.Value) ? Guid.Empty : new Guid(e.Node.Value);
            }
            else
            {
                BankAccountsId = Guid.Empty;
                FilialeBankId = string.IsNullOrEmpty(e.Node.Value) ? Guid.Empty : new Guid(e.Node.Value);
            }
            rgdGrid.Rebind();
        }
        #endregion

        protected string GetBankAccountsCount(Guid bankAccountsId)
        {
            double counts = _bankAccountsWrite.GetBankAccountsNonce(bankAccountsId);
            counts = Math.Round(counts, 2);
            _countBank = _countBank + counts;
            return counts.ToString(CultureInfo.InvariantCulture);
        }

        protected void RtvBankAccountsInit(object sender, EventArgs e)
        {
            GetBankAccounts();
        }

        #region 加载页面数据
        protected void RgdGridNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            FilialeId = string.IsNullOrEmpty(RCB_FilialeId.SelectedValue)
                            ? Guid.Empty
                            : new Guid(RCB_FilialeId.SelectedValue);
            BranchId = string.IsNullOrEmpty(RCB_BranchId.SelectedValue)
                           ? Guid.Empty
                           : new Guid(RCB_BranchId.SelectedValue);
            PositionId = string.IsNullOrEmpty(RCB_PositionId.SelectedValue)
                             ? Guid.Empty
                             : new Guid(RCB_PositionId.SelectedValue);
            rgdGrid.DataSource = GetDataSource(FilialeId, BranchId, PositionId, BankAccountsId, FilialeBankId);
        }
        #endregion

        public IList<BankAccountPermissionInfo> GetDataSource(Guid filialeId, Guid branchId, Guid positionId, Guid bankAccountsId, Guid filialeBankAccoutid)
        {
            var list = BankAccountManager.ReadInstance.GetPermissionList(bankAccountsId).ToList();
            foreach (var personnelBankAccountsInfo in list)
            {
                personnelBankAccountsInfo.FilialeName = CacheCollection.Filiale.GetName(personnelBankAccountsInfo.FilialeID);
                personnelBankAccountsInfo.PositionName = CacheCollection.Position.GetName(personnelBankAccountsInfo.FilialeID, personnelBankAccountsInfo.BranchID, personnelBankAccountsInfo.PositionID);
                personnelBankAccountsInfo.BranchName = CacheCollection.Branch.GetName(personnelBankAccountsInfo.FilialeID, personnelBankAccountsInfo.BranchID);
            }
            return list;
        }

        #region 删除权限
        public void RgdGridDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var bankAccountsid = new Guid(editedItem.GetDataKeyValue("BankAccountsId").ToString());
                var filialeId = new Guid(editedItem.GetDataKeyValue("FilialeId").ToString());
                var branchId = new Guid(editedItem.GetDataKeyValue("BranchId").ToString());
                var positionId = new Guid(editedItem.GetDataKeyValue("PositionId").ToString());
                _bankAccountsWrite.DeleteBankPersion(bankAccountsid, filialeId, branchId, positionId);
            }
        }

        #endregion

        #region 选择后绑定
        private void RcbFilialeIdItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadFiliale();
        }

        private void RcbBranchIdItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadBranch(e.Text);
        }

        private void RcbPositionIdItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadPosition(e.Text);
        }
        #endregion

        #region 加载公司 部门 职务
        private void LoadFiliale()
        {
            RCB_FilialeId.DataValueField = "Id";
            RCB_FilialeId.DataTextField = "Name";
            RCB_FilialeId.DataSource = CacheCollection.Filiale.GetHeadList();
            RCB_FilialeId.DataBind();
            RCB_FilialeId.Items.Insert(0, new RadComboBoxItem());
        }
        private void LoadBranch(string filialeId)
        {
            FilialeId = string.IsNullOrEmpty(filialeId) ? Guid.Empty : new Guid(filialeId);
            RCB_BranchId.DataValueField = "Id";
            RCB_BranchId.DataTextField = "Name";
            RCB_BranchId.DataSource = RecursionBranch(FilialeId, Guid.Empty, 0);
            RCB_BranchId.DataBind();
            if (string.IsNullOrEmpty(filialeId))
            {
                RCB_BranchId.Items.Insert(0, new RadComboBoxItem());
            }
        }
        private void LoadPosition(string branchId)
        {
            BranchId = string.IsNullOrEmpty(branchId) ? Guid.Empty : new Guid(branchId);
            RCB_PositionId.DataValueField = "Id";
            RCB_PositionId.DataTextField = "Name";
            RCB_PositionId.DataSource = CacheCollection.Position.GetList(FilialeId, BranchId);
            RCB_PositionId.DataBind();
            if (string.IsNullOrEmpty(branchId))
            {
                RCB_PositionId.Items.Insert(0, new RadComboBoxItem());
            }
        }

        //显示子部门
        private static IList<BranchInfo> RecursionBranch(Guid filialeId, Guid branchId, int depth)
        {
            depth++;
            IList<BranchInfo> branchsTree = new List<BranchInfo>();
            IList<BranchInfo> branchsList = CacheCollection.Branch.GetList(filialeId, branchId);
            string tag = depth == 1 ? " " : "|" + new string('-', depth);
            foreach (var branchInfo in branchsList)
            {
                branchInfo.Name = tag + branchInfo.Name;
                branchsTree.Add(branchInfo);
                foreach (BranchInfo childbranchInfo in RecursionBranch(branchInfo.FilialeId, branchInfo.ID, depth))
                {
                    branchsTree.Add(childbranchInfo);
                }
            }
            return branchsTree;
        }
        #endregion

        private void OnSelectChanged(Guid pos)
        {
            rgdGrid.DataSource = GetDataSource(FilialeId, BranchId, pos, BankAccountsId, FilialeBankId);
            rgdGrid.Rebind();
        }

        protected void RamPositionPowerAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            string[] strArray = e.Argument.Split('?');
            switch (strArray[0])
            {
                case "OnSelectChanged":
                    {
                        Guid positionId = string.IsNullOrEmpty(strArray[1]) ? Guid.Empty : new Guid(strArray[1]);
                        OnSelectChanged(positionId);
                        break;
                    }
            }
        }

        #region 添加权限
        protected void LbSaveClick(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(RCB_FilialeId.SelectedValue))
                FilialeId = new Guid(RCB_FilialeId.SelectedValue);
            else
            {
                RAM.Alert("请选择公司!");
                return;
            }
            if (!string.IsNullOrEmpty(RCB_BranchId.SelectedValue))
                BranchId = new Guid(RCB_BranchId.SelectedValue);
            else
            {
                RAM.Alert("请选择部门!");
                return;
            }
            if (!string.IsNullOrEmpty(RCB_PositionId.SelectedValue))
                PositionId = new Guid(RCB_PositionId.SelectedValue);
            else
            {
                RAM.Alert("请选择职务!");
                return;
            }

            if (BankAccountsId == Guid.Empty)
            {
                RAM.Alert("请选择左边的银行!");
                return;
            }

            //Start Modify by liucaijun at 2011-January-30th
            if (_bankAccountsWrite.GetPersonnelBankAccountsList(BankAccountsId, FilialeId, BranchId, PositionId).BankAccountsId == Guid.Empty)
            {
                try
                {
                    _bankAccountsWrite.AddBankPersion(BankAccountsId, FilialeId, BranchId, PositionId);
                    rgdGrid.DataSource = GetDataSource(FilialeId, BranchId, PositionId, BankAccountsId, FilialeBankId);
                    rgdGrid.Rebind();
                }
                catch (Exception ex)
                {
                    RAM.Alert("权限增加失败!" + ex.Message);
                }
            }
            else
            {
                RAM.Alert("该用户已经拥有此权限,请勿重复添加！");
            }
            //End
        }
        #endregion
    }
}
