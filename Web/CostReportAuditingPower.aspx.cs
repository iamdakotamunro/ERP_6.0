using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Model.View;
using Telerik.Web.UI;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;

//最后修改人：刘彩军
// 修改时间：2011-August-10th
// 修改内容：代码优化
namespace ERP.UI.Web
{
    /// <summary>费用申报审核权限
    /// </summary>
    public partial class CostReportAuditingPower : BasePage
    {
        private readonly ICostReportAuditingPower _costReportAuditingPowerDao=
            new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Write);
        private readonly IPersonnelSao _personnelSao=new PersonnelSao();
        readonly Dictionary<Guid,string> _dics=new Dictionary<Guid, string>(); 

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

        protected string GetPositionName(object filialeId, object branchId, object positionId)
        {
            return CacheCollection.Position.GetName(filialeId.ToString().ToGuid(), branchId.ToString().ToGuid(), positionId.ToString().ToGuid());
        }

        protected string GetPersonnelName(object personnelId)
        {
            var id = personnelId.ToString().ToGuid();
            if (_dics.ContainsKey(id)) return _dics[id];
            var name=_personnelSao.GetName(id);
            _dics.Add(id,name);
            return name;
        }

        #region[设置列表数据源]
        protected void RgPowerNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RG_Power.DataSource = _costReportAuditingPowerDao.GetPowerList().Where(p => p.Kind == (int)CostReportAuditingType.Auditing).ToList();
        }
        #endregion

        protected void RgPower_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.EditFormItem)
            {
                var editableItem = (GridEditableItem)e.Item;
                //审批人公司、部门、职务
                var ddlFiliale = (DropDownList)editableItem.FindControl("DDL_Filiale");
                var ddlBranch = (DropDownList)editableItem.FindControl("DDL_Branch");
                var ddlPosition = (DropDownList)editableItem.FindControl("DDL_Position");
                var hfAuditingFilialeId = (HiddenField)editableItem.FindControl("hfAuditingFilialeId");
                var hfAuditingBranchId = (HiddenField)editableItem.FindControl("hfAuditingBranchId");
                var hfAuditingPositionId = (HiddenField)editableItem.FindControl("hfAuditingPositionId");
                if (hfAuditingFilialeId != null)
                {
                    string strAuditingFilialeId = hfAuditingFilialeId.Value;
                    if (!string.IsNullOrEmpty(strAuditingFilialeId))
                    {
                        var filialeId = new Guid(strAuditingFilialeId);
                        ddlFiliale.SelectedValue = filialeId.ToString();
                        if (hfAuditingBranchId != null)
                        {
                            var branchId = new Guid(hfAuditingBranchId.Value);
                            ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
                            ddlBranch.DataTextField = "Name";
                            ddlBranch.DataValueField = "ID";
                            ddlBranch.DataBind();
                            ddlBranch.Items.Add(new ListItem("", Guid.Empty.ToString()));
                            ddlBranch.SelectedValue = branchId.ToString();
                            if (hfAuditingPositionId != null)
                            {
                                var positionId = new Guid(hfAuditingPositionId.Value);
                                ddlPosition.DataSource = CacheCollection.Position.GetList(filialeId, branchId);
                                ddlPosition.DataTextField = "Name";
                                ddlPosition.DataValueField = "ID";
                                ddlPosition.DataBind();
                                ddlPosition.Items.Add(new ListItem("", Guid.Empty.ToString()));
                                ddlPosition.SelectedValue = positionId.ToString();
                            }
                        }
                    }
                }
                var hfReportBranchIds = (HiddenField)editableItem.FindControl("hfReportBranchIds");
                if (hfReportBranchIds != null)
                {
                    try
                    {
                        var id = new Guid(editableItem.GetDataKeyValue("PowerId").ToString());
                        var costReportAuditingInfo = _costReportAuditingPowerDao.GetPowerList().FirstOrDefault(p => p.PowerId == id);
                        if (costReportAuditingInfo != null)
                            hfReportBranchIds.Value = costReportAuditingInfo.ReportBranchId;
                    }
                    catch
                    {
                    }
                }

                var rpReportFiliale = (Repeater)editableItem.FindControl("RP_ReportFiliale");
                if (rpReportFiliale != null)
                {
                    rpReportFiliale.DataSource = CacheCollection.Filiale.GetHeadList().ToList();
                    rpReportFiliale.DataBind();
                }
            }
        }

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

        #region[加载部门]
        public List<BranchInfo> LoadBranch()
        {
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList();
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
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList();
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
            IList<FilialeInfo> filialelist = CacheCollection.Filiale.GetList();
            var positionlist = new List<PositionInfo>();
            foreach (var info in filialelist)
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
        protected void DdlFilialeSelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlBranch = ((DropDownList)sender).Parent.FindControl("DDL_Branch") as DropDownList;
            var ddlFiliale = ((DropDownList)sender).Parent.FindControl("DDL_Filiale") as DropDownList;
            if (ddlBranch != null) ddlBranch.Items.Clear();
            if (ddlFiliale != null)
            {
                var filialeId = new Guid(ddlFiliale.SelectedValue);
                if (ddlBranch != null) ddlBranch.DataSource = RecursionBranch(filialeId, Guid.Empty, 0);
            }
            if (ddlBranch != null)
            {
                ddlBranch.DataTextField = "Name";
                ddlBranch.DataValueField = "ID";
                ddlBranch.DataBind();
                ddlBranch.Items.Add(new ListItem("", Guid.Empty.ToString()));
                ddlBranch.SelectedValue = Guid.Empty.ToString();
            }
        }
        #endregion

        #region[选择审批职务]
        /// <summary>
        /// 选择审批职务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlBranchSelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlPosition = ((DropDownList)sender).Parent.FindControl("DDL_Position") as DropDownList;
            var ddlBranch = ((DropDownList)sender).Parent.FindControl("DDL_Branch") as DropDownList;
            var ddlFiliale = ((DropDownList)sender).Parent.FindControl("DDL_Filiale") as DropDownList;
            if (ddlPosition != null) ddlPosition.Items.Clear();
            if (ddlFiliale != null)
            {
                var filialeId = new Guid(ddlFiliale.SelectedValue);
                if (ddlBranch != null)
                {
                    var branchId = new Guid(ddlBranch.SelectedValue);
                    if (ddlPosition != null) ddlPosition.DataSource = CacheCollection.Position.GetList(filialeId, branchId);
                }
            }
            if (ddlPosition != null)
            {
                ddlPosition.DataTextField = "Name";
                ddlPosition.DataValueField = "ID";
                ddlPosition.DataBind();
                ddlPosition.Items.Add(new ListItem("", Guid.Empty.ToString()));
                ddlPosition.SelectedValue = Guid.Empty.ToString();
            }
        }
        #endregion

        #region[修改权限]
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgPowerUpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var info = new CostReportAuditingInfo();
            if (editedItem != null)
            {
                info.PowerId = new Guid(editedItem.GetDataKeyValue("PowerId").ToString());
                info.AuditingFilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.AuditingBranchId = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.AuditingPositionId = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.MinAmount = decimal.Parse(((TextBox)editedItem.FindControl("TB_MinAmount")).Text);
                info.MaxAmount = decimal.Parse(((TextBox)editedItem.FindControl("TB_MaxAmount")).Text);
                string strReportBranchIds = string.Empty;
                var rpReportFiliale = (Repeater)editedItem.FindControl("RP_ReportFiliale");
                strReportBranchIds = (from RepeaterItem item in rpReportFiliale.Items 
                                      select (CheckBoxList) item.FindControl("cblReportBranch"))
                                      .Aggregate(strReportBranchIds, (current1, cblReportBranch) => cblReportBranch.Items.Cast<ListItem>()
                                          .Where(listItem => listItem.Selected).Aggregate(current1, (current, listItem) => current + (listItem.Value + ",")));
                info.ReportBranchId = strReportBranchIds;
            }
            if (editedItem != null) info.Description = ((TextBox)editedItem.FindControl("TB_Description")).Text;
            info.Kind = (int)CostReportAuditingType.Auditing;
            try
            {
                _costReportAuditingPowerDao.UpdatePower(info);
                RG_Power.Rebind();
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
        protected void RgPowerInsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var info = new CostReportAuditingInfo { PowerId = Guid.NewGuid() };
            if (editedItem != null)
            {
                info.AuditingFilialeId = new Guid(((DropDownList)editedItem.FindControl("DDL_Filiale")).SelectedValue);
                info.AuditingBranchId = new Guid(((DropDownList)editedItem.FindControl("DDL_Branch")).SelectedValue);
                info.AuditingPositionId = new Guid(((DropDownList)editedItem.FindControl("DDL_Position")).SelectedValue);
                info.MinAmount = decimal.Parse(((TextBox)editedItem.FindControl("TB_MinAmount")).Text);
                info.MaxAmount = decimal.Parse(((TextBox)editedItem.FindControl("TB_MaxAmount")).Text);
                string strReportBranchIds = string.Empty;
                var rpReportFiliale = (Repeater)editedItem.FindControl("RP_ReportFiliale");
                strReportBranchIds = (from RepeaterItem item in rpReportFiliale.Items 
                                      select (CheckBoxList) item.FindControl("cblReportBranch"))
                                      .Aggregate(strReportBranchIds, (current1, cblReportBranch) => cblReportBranch.Items.Cast<ListItem>().Where(listItem => listItem.Selected).Aggregate(current1, (current, listItem) => current + (listItem.Value + ",")));
                info.ReportBranchId = strReportBranchIds;
            }
            if (editedItem != null) info.Description = ((TextBox)editedItem.FindControl("TB_Description")).Text;
            info.Kind = (int)CostReportAuditingType.Auditing;
            try
            {
                CostReportAuditingInfo auditingInfo = _costReportAuditingPowerDao.GetPowerList().FirstOrDefault(p => p.AuditingFilialeId == info.AuditingFilialeId && p.AuditingBranchId == info.AuditingBranchId &&
                    p.AuditingPositionId == info.AuditingPositionId && p.ReportBranchId == info.ReportBranchId);
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (auditingInfo != null)
                    {
                        _costReportAuditingPowerDao.DeletePower(auditingInfo.PowerId);
                    }
                    _costReportAuditingPowerDao.InsertPower(info);
                    ts.Complete();
                }
                RG_Power.Rebind();
            }
            catch (Exception ex)
            {
                RAM.Alert("权限添加失败！" + ex.Message);
            }
        }
        #endregion

        #region[删除权限]
        protected void RgPowerDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;
            var powerId = new Guid(editedItem.GetDataKeyValue("PowerId").ToString());
            try
            {
                _costReportAuditingPowerDao.DeletePower(powerId);
                RG_Power.Rebind();
            }
            catch (Exception ex)
            {
                RAM.Alert("删除权限失败!" + ex.Message);
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
        protected string BranchId
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
                    barnchName += GetBranchName(filialeId, id.ToGuid());
                }
            }
            return barnchName;
        }
        #endregion

        /// <summary>显示对应公司所有部门（包括所有子部门）
        /// </summary>
        public static IList<BranchInfo> RecursionBranch(Guid filialeId, Guid branchId, int depth)
        {
            var branchsTree = new List<BranchInfo>();
            var branchsList = CacheCollection.Branch.GetList(filialeId, branchId);
            foreach (var branchInfo in branchsList)
            {
                branchInfo.Name = branchInfo.Name;
                branchsTree.Add(branchInfo);
            }
            return branchsTree;
        }

        protected void Rp_Filiale_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var filialeInfo = (FilialeInfo)e.Item.DataItem;
                if (filialeInfo != null)
                {
                    var cblReportBranch = (CheckBoxList)e.Item.FindControl("cblReportBranch");
                    if (cblReportBranch != null)
                    {
                        var rp = sender as Repeater;
                        if (rp != null)
                        {
                            cblReportBranch.DataSource = RecursionBranch(filialeInfo.ID, Guid.Empty, 0);
                            cblReportBranch.DataValueField = "ID";
                            cblReportBranch.DataTextField = "Name";
                            cblReportBranch.DataBind();

                            var editableItem = (GridEditableItem)rp.Parent.Parent.Parent;
                            var hfReportBranchIds = (HiddenField)editableItem.FindControl("hfReportBranchIds");
                            string strReportBranchIds = hfReportBranchIds.Value;
                            if (!string.IsNullOrEmpty(strReportBranchIds))
                            {
                                foreach (ListItem item in cblReportBranch.Items)
                                {
                                    if (strReportBranchIds.Contains(item.Value))
                                    {
                                        item.Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
