using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    //*****************************//
    //** Func : 费用流水账
    //** Date : 2009-6-22
    //** Code : dyy
    //*****************************//
    public partial class CostReckoningAw : BasePage
    {
        private readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);
        readonly ICostReckoning _costReckoningDao = new CostReckoning(GlobalConfig.DB.FromType.Read);
        //往来单位编号
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value.ToString();
            }
        }
        // 开始时间
        protected DateTime StartDate
        {
            get
            {
                if (ViewState["StartDate"] == null || Convert.ToDateTime(ViewState["StartDate"]) == DateTime.MinValue)
                {
                    return DateTime.MinValue;
                }
                return Convert.ToDateTime(ViewState["StartDate"]);
            }
            set
            {
                ViewState["StartDate"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        // 结束时间
        protected DateTime EndDate
        {
            get
            {
                if (ViewState["EndDate"] == null || Convert.ToDateTime(ViewState["EndDate"]) == DateTime.MinValue)
                {
                    return DateTime.MinValue;
                }
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndDate"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndDate"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        // 单据类型
        protected int ReceiptType
        {
            get
            {
                if (ViewState["ReceiptType"] == null)
                {
                    return -1;
                }

                return Convert.ToInt32(ViewState["ReceiptType"]);
            }
            set
            {
                ViewState["ReceiptType"] = value;
            }
        }

        //搜索选择公司编号
        private Guid SelectFilialeId
        {
            get
            {
                if (ViewState["SelectFilialeId"] == null) return Guid.Empty;
                return new Guid(ViewState["SelectFilialeId"].ToString());
            }
            set
            {
                ViewState["SelectFilialeId"] = value.ToString();
            }
        }

        public IList<FilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    var list = CacheCollection.Filiale.GetHeadList();
                    ViewState["FilialeList"] = list;
                }
                return (IList<FilialeInfo>)ViewState["FilialeList"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetCompanyCussent(Guid.Empty);
                FilialeData();
            }
        }

        //公司数据
        protected void FilialeData()
        {
            RCB_FilialeList.DataSource = FilialeList.Where(f => f.Rank == (int)FilialeRank.Head);
            RCB_FilialeList.DataTextField = "Name";
            RCB_FilialeList.DataValueField = "ID";
            RCB_FilialeList.DataBind();
            RCB_FilialeList.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
        }

        //遍历往来的单位
        private void GetCompanyCussent(Guid assumeFilialeId)
        {
            RadTreeNode rootNode = CreateNode("费用分类选择" + "[" + CountCussent(Guid.Empty, Guid.Empty, Guid.NewGuid(), assumeFilialeId) + "]", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            rootNode.PostBack = false;
            RTVCompanyCussent.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode, assumeFilialeId);
        }
        //遍历子公司
        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node, Guid assumeFilialeId)
        {
            IList<CostCompanyClassInfo> childCompanyClassList = _costDao.GetChildCompanyClassList(companyClassId);
            foreach (CostCompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName + "[" + CountCussent(companyClassInfo.CompanyClassId, Guid.Empty, Guid.Empty, assumeFilialeId) + "]", false, companyClassInfo.CompanyClassId.ToString());
                childNode.PostBack = false;
                IList<CostCussentInfo> companyCussentList = _costCussentDao.GetCompanyCussentList(companyClassInfo.CompanyClassId);
                if (companyCussentList.Count != 0)
                {
                    node.Nodes.Add(childNode);
                    RecursivelyCompanyClass(companyClassInfo.CompanyClassId, childNode, assumeFilialeId);
                    RepetitionCompanyCussent(childNode, companyCussentList, assumeFilialeId);
                }
            }
        }

        private void RepetitionCompanyCussent(RadTreeNode node, IList<CostCussentInfo> companyCussentList, Guid assumeFilialeId)
        {
            foreach (CostCussentInfo companyCussentInfo in companyCussentList)
            {
                RadTreeNode childNode = CreateNode(companyCussentInfo.CompanyName + "[" + CountCussent(Guid.Empty, companyCussentInfo.CompanyId, Guid.Empty, assumeFilialeId) + "]", false, companyCussentInfo.CompanyId.ToString());
                node.Nodes.Add(childNode);
            }
        }
        //创建节点
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        //绑定往来账数据源
        protected void RGReckoning_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RGReckoning.DataSource = new List<CostReckoningInfo>();
            }
            else
            {
                if (CompanyId == Guid.Empty) return;
                var personnelInfo = CurrentSession.Personnel.Get();
                var branchInfo = CacheCollection.Branch.Get(personnelInfo.FilialeId, personnelInfo.BranchId);
                var personnelBranchId = branchInfo == null ? personnelInfo.BranchId : branchInfo.ParentBranchId != Guid.Empty ? branchInfo.ParentBranchId : personnelInfo.BranchId;
                RGReckoning.DataSource = _costReckoningDao.GetReckoningList(CompanyId, StartDate, EndDate, ReceiptType, 1, personnelInfo.FilialeId, personnelBranchId, SelectFilialeId);
            }
        }

        //选择往来单位分类树节点
        protected void RadTreeViewCompanyCussent_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                CompanyId = new Guid(e.Node.Value);

                StartDate = DateTime.MinValue;
                EndDate = DateTime.MinValue;
                ReceiptType = -1;
                RDP_StartDate.SelectedDate = null;
                RDP_EndDate.SelectedDate = null;
                RCB_ReceiptType.SelectedValue = "-1";
                RGReckoning.Rebind();
            }
        }

        #region[选择公司后重新加载树]
        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var value = e.Value.Trim();
            SelectFilialeId = string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
            RTVCompanyCussent.Nodes.Clear();
            GetCompanyCussent(SelectFilialeId);

            RGReckoning.DataSource = new List<CostReckoningInfo>();
            RGReckoning.DataBind();
        }
        #endregion

        protected void LBXLS_Click(object sender, EventArgs e)
        {
            string fileName = RTVCompanyCussent.SelectedNode == null ? "没有选择往来单位" : new Guid(RTVCompanyCussent.SelectedNode.Value) == Guid.Empty ? "没有选择往来单位" : RTVCompanyCussent.SelectedNode.Text;
            fileName = Server.UrlEncode(fileName);
            RGReckoning.ExportSettings.ExportOnlyData = true;
            RGReckoning.ExportSettings.IgnorePaging = true;
            RGReckoning.ExportSettings.FileName = fileName;
            RGReckoning.MasterTableView.ExportToExcel();
        }

        private string CountCussent(Guid companyClassId, Guid companyId, Guid parentCompanyClassId, Guid assumeFilialeId)
        {
            double counts = _costCussentDao.GetCussentCount(companyClassId, companyId, parentCompanyClassId, assumeFilialeId);
            return WebControl.RemoveDecimalEndZero(decimal.Parse(counts.ToString(CultureInfo.InvariantCulture)));
        }

        protected void LB_Search_Click(object sender, EventArgs e)
        {
            if (CompanyId != Guid.Empty)
            {
                StartDate = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
                EndDate = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
                ReceiptType = Convert.ToInt32(RCB_ReceiptType.SelectedValue);
                RGReckoning.Rebind();
            }
        }

        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGReckoning, e);
        }

        protected void RCB_Auditing_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (CompanyId != Guid.Empty)
            {
                StartDate = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
                EndDate = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
                ReceiptType = Convert.ToInt32(RCB_ReceiptType.SelectedValue);
                RGReckoning.Rebind();
            }
        }
    }
}
