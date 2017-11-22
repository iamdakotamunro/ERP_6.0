using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using MIS.Model.View;
using Telerik.Web.UI;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;

namespace ERP.UI.Web
{
    /// <summary>费用申报票据权限
    /// </summary>
    public partial class CostReportInvoiceAuditingPower : BasePage
    {
        private readonly ICostReportAuditingPower _costReportAuditingPower=new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetFilialeName(object filialeId)
        {
            return CacheCollection.Filiale.GetName(filialeId.ToString().ToGuid());
        }

        protected string GetBranchName(object filialeId, object branchId)
        {
            return CacheCollection.Branch.GetName(filialeId.ToString().ToGuid(), branchId.ToString().ToGuid());
        }

        public string GetPositionName(object filialeId, object branchId, object positionId)
        {
            return CacheCollection.Position.GetName(filialeId.ToString().ToGuid(), branchId.ToString().ToGuid(), positionId.ToString().ToGuid());
        }

        #region[设置列表数据源]
        protected void RG_Power_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RG_Power.DataSource = _costReportAuditingPower.GetPowerList().Where(p => p.Kind == (int)CostReportAuditingType.Invoice).ToList();
        }
        #endregion

        #region[加载公司]
        public List<FilialeInfo> LoadFiliale()
        {
            var list = new List<FilialeInfo>();
            var info = new FilialeInfo { Name = "请选择公司" };
            list.Add(info);
            list.AddRange(CacheCollection.Filiale.GetHeadList().ToList());
            return list;
        }
        #endregion

        #region[加载部门]
        public IList<BranchInfo> LoadBranch()
        {
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList().Where(f => f.Rank == (int)FilialeRank.Head).ToList();
            var branchlist = new List<BranchInfo>();
            foreach (FilialeInfo info in filialelist)
            {
                branchlist.AddRange(RecursionBranch(info.ID, Guid.Empty, 0));
            }
            branchlist.Add(new BranchInfo());
            return branchlist;
        }
        #endregion

        #region[加载申报部门]
        public List<BranchInfo> LoadReportBranch()
        {
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList().Where(f => f.Rank == (int)FilialeRank.Head).ToList();
            var branchlist = new List<BranchInfo>();
            foreach (FilialeInfo info in filialelist)
            {
                branchlist.AddRange(RecursionBranch(info.ID, Guid.Empty, 0));
            }
            return branchlist;
        }
        #endregion

        #region[加载职务]
        public List<PositionInfo> LoadPosition()
        {
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList().Where(f => f.Rank == (int)FilialeRank.Head).ToList();
            var positionlist = new List<PositionInfo>();
            foreach (FilialeInfo info in filialelist)
            {
                positionlist.AddRange(CacheCollection.Position.GetList(info.ID));
            }
            positionlist.Add(new PositionInfo());
            return positionlist;
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
            if (dropDownList == null) return;
            var ddlBranch = dropDownList.Parent.FindControl("DDL_Branch") as DropDownList;
            var ddlFiliale = dropDownList.Parent.FindControl("DDL_Filiale") as DropDownList;
            if (ddlBranch != null) ddlBranch.Items.Clear();
            if (ddlFiliale == null) return;
            var filialeId = new Guid(ddlFiliale.SelectedValue);
            if (ddlBranch == null) return;
            ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
            ddlBranch.DataTextField = "Name";
            ddlBranch.DataValueField = "ID";
            ddlBranch.DataBind();
            ddlBranch.Items.Add(new ListItem("", Guid.Empty.ToString()));
            ddlBranch.SelectedValue = Guid.Empty.ToString();
        }

        #endregion

        #region[申报公司选择后显示部门]
        /// <summary>
        /// 申报公司选择后显示部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RCB_ReportFiliale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlFiliale = sender as RadComboBox;
            if (ddlFiliale == null) return;
            var ddlBranch = ddlFiliale.Parent.FindControl("RCB_ReportBranch") as RadComboBox;
            if (ddlBranch != null) ddlBranch.Items.Clear();
            var filialeId = new Guid(ddlFiliale.SelectedValue);
            if (ddlBranch == null) return;
            ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
            ddlBranch.DataTextField = "Name";
            ddlBranch.DataValueField = "ID";
            ddlBranch.DataBind();
            //ddlBranch.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            //ddlBranch.SelectedValue = Guid.Empty.ToString();
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
            if (dropDownList == null) return;
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
                        ddlPosition.Items.Add(new ListItem("", Guid.Empty.ToString()));
                        ddlPosition.SelectedValue = Guid.Empty.ToString();
                    }
                }
            }
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
            Ccb_CheckedChanged(e);
            var editedItem = e.Item as GridEditableItem;
            var info = new CostReportAuditingInfo();
            if (editedItem != null)
            {
                info.PowerId = new Guid(editedItem.GetDataKeyValue("PowerId").ToString());
                info.AuditingFilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.AuditingBranchId = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.AuditingPositionId = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
            }
            info.MinAmount = -1;
            info.MaxAmount = -1;
            info.ReportBranchId = BranchID;
            if (editedItem != null) info.Description = ((TextBox)editedItem.FindControl("TB_Description")).Text;
            info.Kind = (int)CostReportAuditingType.Invoice;
            try
            {
                _costReportAuditingPower.UpdatePower(info);
                RG_Power.Rebind();
                CheckChanged = false;
            }
            catch (Exception ex)
            {
                RAM.Alert("权限修改失败！" + ex.Message);
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
            Ccb_CheckedChanged(e);
            var editedItem = e.Item as GridEditableItem;
            var info = new CostReportAuditingInfo { PowerId = Guid.NewGuid() };
            if (editedItem != null)
            {
                info.AuditingFilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.AuditingBranchId = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.AuditingPositionId = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
            }
            info.MinAmount = -1;
            info.MaxAmount = -1;
            info.ReportBranchId = BranchID;
            if (editedItem != null) info.Description = ((TextBox)editedItem.FindControl("TB_Description")).Text;
            info.Kind = (int)CostReportAuditingType.Invoice;
            try
            {
                CostReportAuditingInfo auditingInfo = _costReportAuditingPower.GetPowerList().FirstOrDefault(p => p.AuditingFilialeId == info.AuditingFilialeId && p.AuditingBranchId == info.AuditingBranchId &&
                    p.AuditingPositionId == info.AuditingPositionId && p.ReportBranchId == info.ReportBranchId);
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (auditingInfo != null)
                    {
                        _costReportAuditingPower.DeletePower(auditingInfo.PowerId);
                    }
                    _costReportAuditingPower.InsertPower(info);
                    ts.Complete();
                }
                RG_Power.Rebind();
                CheckChanged = false;
            }
            catch (Exception ex)
            {
                RAM.Alert("权限添加失败！" + ex.Message);
            }
        }
        #endregion

        #region[删除权限]
        protected void RG_Power_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var powerId = new Guid(editedItem.GetDataKeyValue("PowerId").ToString());
                try
                {
                    _costReportAuditingPower.DeletePower(powerId);
                    RG_Power.Rebind();
                }
                catch (Exception ex)
                {
                    RAM.Alert("删除权限失败!" + ex.Message);
                }
            }
        }
        #endregion

        #region[部门列表]
        protected IList<BranchInfo> BranchList
        {
            get
            {
                if (ViewState["BranchList"] == null)
                    return new List<BranchInfo>();
                return (List<BranchInfo>)ViewState["BranchList"];
            }
            set
            {
                ViewState["BranchList"] = value;
            }
        }
        #endregion

        #region[部门ID列表]
        protected IList<Guid> NewBranch
        {
            get
            {
                if (ViewState["NewBranch"] == null)
                    return new List<Guid>();
                return (List<Guid>)ViewState["NewBranch"];
            }
            set
            {
                ViewState["NewBranch"] = value;
            }
        }
        #endregion

        #region[部门]
        protected string BranchID
        {
            get
            {
                if (ViewState["BranchID"] == null)
                    return "";
                return ViewState["BranchID"].ToString();
            }
            set
            {
                ViewState["BranchID"] = value;
            }
        }
        #endregion

        #region[选择允许申报部门]
        protected bool CheckChanged
        {
            get
            {
                if (ViewState["CheckChanged"] == null)
                    return false;
                return (bool)ViewState["CheckChanged"];
            }
            set
            {
                ViewState["CheckChanged"] = value;
            }
        }

        protected void Ccb_CheckedChanged(GridCommandEventArgs e)
        {
            CheckChanged = true;
            var item = e.Item as GridEditableItem;
            if (item != null)
            {
                var rcb = item.FindControl("RCB_ReportBranch") as RadComboBox;
                BranchList = LoadReportBranch();
                string branchId = string.Empty;
                if (rcb != null)
                {
                    for (int a = 0; a < rcb.Items.Count; a++)
                    {
                        var lblId = ((Label)rcb.Items[a].FindControl("lblID"));
                        var ccb = (CheckBox)rcb.Items[a].FindControl("ccb");
                        if (lblId != null && ccb != null && !string.IsNullOrEmpty(lblId.Text))
                        {
                            var wid = new Guid(lblId.Text);
                            int isChecked = ccb.Checked ? 1 : 0;
                            //增加申报部门权限
                            if (isChecked == 1)
                            {
                                branchId += wid.ToString() + ",";
                            }
                        }
                    }
                }
                BranchID = branchId;
                String[] branchIdList = branchId.Split(',');
                {
                    NewBranch = (from id in branchIdList where !string.IsNullOrEmpty(id) select new Guid(id)).ToList();
                }

            }
        }

        protected void ccb_OnLoad(object sender, EventArgs e)
        {
            if (!CheckChanged)
            {
                var ccb = sender as CheckBox;
                if (ccb != null)
                {
                    var item = ccb.Parent.Parent.Parent.Parent.Parent as GridEditableItem;
                    try
                    {
                        if (item != null)
                        {
                            var id = new Guid(item.GetDataKeyValue("PowerId").ToString());
                            var rcb = item.FindControl("RCB_ReportBranch") as RadComboBox;
                            var list = new List<Guid>();
                            var costReportAuditingInfo = _costReportAuditingPower.GetPowerList().FirstOrDefault(p => p.PowerId == id);
                            if (costReportAuditingInfo != null)
                            {
                                String[] branchIdList = costReportAuditingInfo.ReportBranchId.Split(',');
                                {
                                    list.AddRange(from nid in branchIdList where !string.IsNullOrEmpty(nid) select new Guid(nid));
                                }
                            }
                            NewBranch = list;
                            IList<Guid> brList = NewBranch;
                            BranchList = LoadBranch();
                            for (int a = 0; a < BranchList.Count; a++)
                            {
                                Guid bid = BranchList[a].ID;
                                if (brList.Contains(bid))
                                {
                                    try
                                    {
                                        if (rcb != null) ((CheckBox)rcb.Items[a].FindControl("ccb")).Checked = true;
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region[获取绑定的申报部门]
        public string GetBindBranch(Guid filialeId, string branchId)
        {
            string[] brancharr = branchId.Split(',');
            string barnchName = string.Empty;
            foreach (string id in brancharr)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (!string.IsNullOrEmpty(barnchName))
                    {
                        barnchName += ",";
                    }
                    barnchName += CacheCollection.Branch.GetName(filialeId, new Guid(id));
                }
            }
            return barnchName;
        }
        #endregion

        /// <summary>显示所有部门（包括所有子部门）
        /// </summary>
        public static IList<BranchInfo> RecursionBranch(Guid filialeId, Guid branchId, int depth)
        {
            //depth++;
            //IList<MIS.Model.View.BranchInfo> branchsTree = new List<MIS.Model.View.BranchInfo>();
            //IList<MIS.Model.View.BranchInfo> branchsList = CacheCollection.Branch.GetList(filialeId, branchId);
            //string tag = depth == 1 ? " " : "|" + new string('-', depth);
            //foreach (MIS.Model.View.BranchInfo branchInfo in branchsList)
            //{
            //    branchInfo.Name = tag + branchInfo.Name;
            //    branchsTree.Add(branchInfo);
            //    foreach (var childbranchInfo in RecursionBranch(branchInfo.FilialeId, branchInfo.ID, depth))
            //    {
            //        branchsTree.Add(childbranchInfo);
            //    }
            //}
            //return branchsTree;

            IList<BranchInfo> branchsTree = new List<BranchInfo>();
            IList<BranchInfo> branchsList = CacheCollection.Branch.GetList(filialeId, branchId);
            foreach (BranchInfo branchInfo in branchsList)
            {
                branchInfo.Name = branchInfo.Name;
                branchsTree.Add(branchInfo);
            }
            return branchsTree;
        }
    }
}
