using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using System.Collections;
using System.Text;
using System.Transactions;
using ERP.Model;
using OperationLog.Core;
using ERP.DAL.Implement.Inventory;
using System.Configuration;
using ERP.SAL.Interface;
using System.Web.UI;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报审批
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_AuditManage : BasePage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReckoning _costReckoning = new CostReckoning(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private readonly ICostReportAuditingPower _costReportAuditingPower = new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReportTravel _costReportTravel = new CostReportTravelDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTermini _costReportTermini = new CostReportTerminiDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        public List<CostReportInfo> ReportList = new List<CostReportInfo>();

        #region 属性
        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }

        /// <summary>
        /// O2O事业部
        /// </summary>
        private string ShopBranchId
        {
            get
            {
                if (ViewState["ShopBranchId"] == null)
                {
                    ViewState["ShopBranchId"] = GlobalConfig.ShopBranchId;
                }
                return ViewState["ShopBranchId"].ToString();
            }
        }

        /// <summary>
        /// 票据列表属性
        /// </summary>
        protected List<CostReportBillInfo> CostReportBillInfoList
        {
            get
            {
                if (ViewState["CostReportBillInfoList"] == null)
                    return new List<CostReportBillInfo>();
                return ViewState["CostReportBillInfoList"] as List<CostReportBillInfo>;
            }
            set
            {
                ViewState["CostReportBillInfoList"] = value;
            }
        }


        /// <summary>
        /// 差旅费列表属性
        /// </summary>
        protected List<CostReportTravelInfo> CostReportTravelInfoList
        {
            get
            {
                if (ViewState["CostReportTravelInfoList"] == null)
                    return new List<CostReportTravelInfo>();
                return ViewState["CostReportTravelInfoList"] as List<CostReportTravelInfo>;
            }
            set
            {
                ViewState["CostReportTravelInfoList"] = value;
            }
        }

        /// <summary>
        /// 起讫列表属性
        /// </summary>
        protected List<CostReportTerminiInfo> CostReportTerminiInfoList
        {
            get
            {
                if (ViewState["CostReportTerminiInfoList"] == null)
                    return new List<CostReportTerminiInfo>();
                return ViewState["CostReportTerminiInfoList"] as List<CostReportTerminiInfo>;
            }
            set
            {
                ViewState["CostReportTerminiInfoList"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAssumeFilialeData();
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_Report.DataBind();
        }

        #region 数据准备
        //结算公司
        protected void LoadAssumeFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            ddl_AssumeFiliale.DataSource = list;
            ddl_AssumeFiliale.DataTextField = "Name";
            ddl_AssumeFiliale.DataValueField = "ID";
            ddl_AssumeFiliale.DataBind();
            ddl_AssumeFiliale.Items.Insert(0, new ListItem("全部", ""));
        }

        //申请人数据绑定
        protected void rcb_ReportPersonnel_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var personnelList = new PersonnelManager().GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in personnelList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.RealName, item.PersonnelId.ToString()));
                    }
                }
            }
        }
        #endregion

        #region 数据列表相关
        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var states = new List<int>();
            switch (ddl_State.SelectedValue)
            {
                case "0": //已处理
                    states.Add((int)CostReportState.AlreadyAuditing);
                    states.Add((int)CostReportState.WaitVerify);
                    states.Add((int)CostReportState.NoAuditing);
                    break;
                case "1": //未处理
                    states.Add((int)CostReportState.Auditing);
                    break;
            }

            var costReportList = _costReport.GetReportList(states);
            var query = costReportList.AsQueryable();

            #region 查询条件
            if (!string.IsNullOrEmpty(txt_ReportNo.Text))
            {
                query = query.Where(p => p.ReportNo.Equals(txt_ReportNo.Text.Trim()));
            }

            if (Hid_TimeType.Value.Equals("1"))
            {
                if (!string.IsNullOrEmpty(txt_DateTimeStart.Text))
                {
                    var startTime = DateTime.Parse(txt_DateTimeStart.Text);
                    query = query.Where(p => p.ReportDate >= startTime);
                }
                if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
                {
                    var endtime = DateTime.Parse(txt_DateTimeEnd.Text);
                    query = query.Where(p => p.ReportDate < endtime.AddDays(1));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txt_DateTimeStart.Text))
                {
                    var startTime = DateTime.Parse(txt_DateTimeStart.Text);
                    query = query.Where(p => p.AuditingDate >= startTime);
                }
                if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
                {
                    var endtime = DateTime.Parse(txt_DateTimeEnd.Text);
                    query = query.Where(p => p.AuditingDate < endtime.AddDays(1));
                }
            }
            if (!string.IsNullOrEmpty(txt_ReportName.Text))
            {
                query = query.Where(p => p.ReportName.Contains(txt_ReportName.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(rcb_ReportPersonnel.SelectedValue) && !rcb_ReportPersonnel.SelectedValue.Equals(Guid.Empty.ToString()))
            {
                query = query.Where(p => p.ReportPersonnelId.Equals(new Guid(rcb_ReportPersonnel.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(txt_ReportCostStart.Text))
            {
                query = query.Where(p => p.ReportCost >= decimal.Parse(txt_ReportCostStart.Text));
            }
            if (!string.IsNullOrEmpty(txt_ReportCostEnd.Text))
            {
                query = query.Where(p => p.ReportCost <= decimal.Parse(txt_ReportCostEnd.Text));
            }
            if (!string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue))
            {
                query = query.Where(p => p.AssumeFilialeId.Equals(new Guid(ddl_AssumeFiliale.SelectedValue)));
            }
            query = query.Where(p => p.UrgentOrDefer.Equals(int.Parse(rbl_UrgentOrDefer.SelectedValue)));
            #endregion

            #region 获取权限数据集

            #region 获取数据集中所有申报人的部门集合
            var reportPersonnelIdList = query.Select(act => act.ReportPersonnelId).Distinct().ToList();
            var branchIds = MISService.GetBranchIdsByPersonnelIds(reportPersonnelIdList);
            #endregion

            #region 费用申报审批权限
            var costReportAuditingList = _costReportAuditingPower.GetCanAuditingInfo(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(p => p.Kind == (int)CostReportAuditingType.Auditing);
            #endregion

            foreach (CostReportAuditingInfo info in costReportAuditingList)
            {
                //筛选该部门中在相应“审批金额范围”内的数据
                var branchAuditingList = query.Where(p => info.MinAmount <= p.ReportCost + p.PayCost && p.ReportCost + p.PayCost <= info.MaxAmount).ToList();
                foreach (var item in branchAuditingList)
                {
                    //此筛选条件没有放到where中的原因：因为有些用户缺少部门信息，所以有些“ReportPersonnelId”在branchIds中不存在，如果放到where中筛选会报错
                    if (branchIds.ContainsKey(item.ReportPersonnelId) && info.ReportBranchId.Contains(branchIds[item.ReportPersonnelId].ToString()))
                    {
                        if (ReportList.Count(p => p.ReportId.Equals(item.ReportId)).Equals(0))
                        {
                            ReportList.Add(item);
                        }
                    }
                }
            }
            #endregion

            #region 合计
            var totalName = RG_Report.MasterTableView.Columns.FindByUniqueName("TotalName");
            var reportCost = RG_Report.MasterTableView.Columns.FindByUniqueName("ReportCost");
            var payCost = RG_Report.MasterTableView.Columns.FindByUniqueName("PayCost");
            if (query.Any())
            {
                var sumReportCost = query.Sum(p => Math.Abs(p.ReportCost));
                var sumPayCost = query.Sum(p => Math.Abs(p.PayCost));
                totalName.FooterText = "合计：";
                reportCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumReportCost));
                payCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPayCost));
            }
            else
            {
                totalName.FooterText = string.Empty;
                reportCost.FooterText = string.Empty;
                payCost.FooterText = string.Empty;
            }
            #endregion

            RG_Report.DataSource = query.OrderByDescending(r => r.ReportDate).ToList();
        }

        //行绑定事件
        protected void RG_Report_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                #region “申报金额”>=1万元(绿色);“申报金额”>=10万元(黄色);“申报金额”>=100万元(紫色)
                var reportCost = DataBinder.Eval(e.Item.DataItem, "ReportCost");
                if (decimal.Parse(reportCost.ToString()) >= 1000000)
                {
                    e.Item.Style["color"] = "#CC00FF";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 100000)
                {
                    e.Item.Style["color"] = "#FF9900";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 10000)
                {
                    e.Item.Style["color"] = "#009900";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                #endregion
            }
        }

        #region 列表显示辅助方法
        /// <summary>
        /// 验证是否存在票据信息(用于判断押金回收的“凭证报销”是否信息完整)
        /// </summary>
        /// <param name="reportKind"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        protected bool DepositRecoveryLater(int reportKind, Guid reportId)
        {
            if (reportKind.Equals((int)CostReportKind.Later))
            {
                var costReportBillList = _costReportBill.Getlmshop_CostReportBillByReportId(reportId);
                if (!costReportBillList.Any())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        #endregion

        //批量审批提示(提示申请单总数、申报金额)
        protected void btn_Audit_Click(object sender, EventArgs e)
        {
            Hid_ReportId.Value = string.Empty;
            var errorMsg = new StringBuilder();
            if (Request["ckId"] != null)
            {
                var datas = Request["ckId"].Split(',');
                string reportIds = string.Empty;
                foreach (var item in datas)
                {
                    var reportId = item.Split('&')[0];
                    var state = item.Split('&')[1];
                    var reportNo = item.Split('&')[2];
                    if (!int.Parse(state).Equals((int)CostReportState.Auditing))
                    {
                        errorMsg.Append("“").Append(reportNo).Append("”状态已更新，不允许此操作！").Append("<br/>");
                        continue;
                    }
                    reportIds += "," + reportId;
                }

                if (!string.IsNullOrEmpty(reportIds))
                {
                    Hid_ReportId.Value = reportIds.Substring(1);
                    ArrayList arrayList = _costReport.GetSumReport(Hid_ReportId.Value.Split(','));
                    if (arrayList.Count > 0)
                    {
                        errorMsg.Append("<b>申请单</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[0] + "</span>张、<b>申报金额</b><span style='color:red; padding:0 5px 0 5px;'>" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(arrayList[1].ToString())) + "</span>元");
                    }
                }
            }
            else
            {
                errorMsg.Append("<span style='color:red;'>请选择相关数据！</span>");
            }
            lit_Msg.Text = errorMsg.ToString();
            MessageBox.AppendScript(this, "moveShow();ShowValue('" + Hid_ReportId.Value + "');");
        }

        //批量审批通过
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Hid_ReportId.Value))
            {
                var errorMsg = new StringBuilder();
                var reportIds = Hid_ReportId.Value.Split(',');
                foreach (var item in reportIds)
                {
                    CostReportInfo model = _costReport.GetReportByReportId(new Guid(item));
                    if (model.State != (int)CostReportState.Auditing)
                    {
                        errorMsg.Append("“").Append(model.ReportNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        AuditPass(model);
                    }
                    catch
                    {
                        errorMsg.Append("“").Append(model.ReportNo).Append("”保存失败！").Append("\\n");
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        #region 批量审批
        #region 审批通过
        protected void AuditPass(CostReportInfo model)
        {
            bool execute = false;
            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Paying)
            {
                execute = true;
                //已扣款核销
                PayVerification(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model);
            }
            else if (model.ReportKind == (int)CostReportKind.FeeIncome)
            {
                //费用收入
                FeeIncome(model);
            }

            model.AuditingMan = Personnel.PersonnelId;
            model.AuditingDate = DateTime.Now;

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);

                    if (execute)
                    {
                        _costReport.UpdatePayCostAndExecuteDate(model.ReportId, model.RealityCost);
                        string errorMessage;
                        bool result = ExecuteFinishHandle(model, Personnel, out errorMessage);
                        if (!result)
                        {
                            throw new Exception(errorMessage);
                        }
                    }

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }
        #endregion

        #region 预借款
        protected void BeforeLoan(CostReportInfo model)
        {
            if (model.IsSystem)
            {
                model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
            }
            else
            {
                //获取票据
                CostReportBillInfoList = _costReportBill.Getlmshop_CostReportBillByReportId(model.ReportId);
                //查询没有付款的票据
                var isPayList = CostReportBillInfoList.Where(p => !p.IsPay);
                if (isPayList.Any())//有没有付款的票据
                {
                    model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
                }
                else
                {
                    if (model.IsLastTime)
                    {
                        //获取差旅费
                        CostReportTravelInfoList = _costReportTravel.GetlmShop_CostReportTravelByReportId(model.ReportId);
                        //获取起讫
                        CostReportTerminiInfoList = _costReportTermini.GetmShop_CostReportTerminiByReportId(model.ReportId);
                        if (CostReportTravelInfoList.Any() || CostReportTerminiInfoList.Any() || !string.IsNullOrEmpty(model.GoodsCode) || !model.InvoiceId.Equals(Guid.Empty))
                        {
                            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
                        }
                        else
                        {
                            model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
                        }
                    }
                    else
                    {
                        model.State = (int)CostReportState.AlreadyAuditing;//状态：待付款
                    }
                }
            }
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【审核】:审核通过;]");
        }
        #endregion

        #region 已扣款核销
        protected void PayVerification(CostReportInfo model)
        {
            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理

            #region 结算账号
            IBankAccounts bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
            var bankAccountInfo = bankAccounts.GetBankAccounts(model.PayBankAccountId);
            string payBankAccount = bankAccountInfo == null ? "暂无结算" : (bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】");
            #endregion

            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【审核】:审核通过;]") + ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【已付款】:已支付" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model)
        {
            model.State = (int)CostReportState.NoAuditing;//状态：票据待受理
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【审核】:审核通过;]");
        }
        #endregion

        #region 费用收入
        protected void FeeIncome(CostReportInfo model)
        {
            model.State = (int)CostReportState.WaitVerify;
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【审核】:审核通过;]");
        }
        #endregion

        #region 执行完成时的操作
        protected bool ExecuteFinishHandle(CostReportInfo model, PersonnelInfo personnelInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                model.RealityCost = -model.RealityCost;
            }

            var costReportBll = new BLL.Implement.Inventory.CostReport(_bankAccounts, _personnelSao, _costReckoning);
            #region 新增资金流
            var wasteBookInfo = costReportBll.AddWasteBookInfo(model, personnelInfo, false);
            if (_wasteBook.Insert(wasteBookInfo) <= 0)
            {
                errorMsg = "新增资金流失败！";
                return false;
            }
            #endregion

            #region 新增帐务记录
            var costReckoningInfo = costReportBll.AddCostReckoningInfo(model, personnelInfo, false);
            if (_costReckoning.Insert(costReckoningInfo) <= 0)
            {
                errorMsg = "新增帐务记录失败！";
                return false;
            }
            #endregion

            #region 与门店费用交互
            if (model.AssumeBranchId == new Guid(ShopBranchId) && !model.AssumeShopId.Equals(Guid.Empty))
            {
                string strErrorMsg;
                var description = string.Format("费用申报{0}{1}可用余额!", model.ReceiptNo, model.RealityCost < 0 ? "添加" : "扣除");
                var parentId = FilialeManager.GetShopHeadFilialeId(model.AssumeShopId);//获取门店所属公司
                if (!ShopSao.DeductBalance(parentId, model.AssumeShopId, model.RealityCost, description, out strErrorMsg))
                {
                    errorMsg = strErrorMsg;
                    return false;
                }

                var costRecordDto = costReportBll.AddCostRecordDto(model);
                if (!ShopSao.InsertCostRecord(parentId, costRecordDto))
                {
                    errorMsg = "扣除门店费用失败！";
                    return false;
                }
            }
            #endregion

            #region 新增资金流(手续费)
            if (model.RealityCost >= 0 && model.Poundage > 0 && !model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                var wasteBookInfoPoundage = costReportBll.AddWasteBookInfo(model, personnelInfo, true);
                if (_wasteBook.Insert(wasteBookInfoPoundage) <= 0)
                {
                    errorMsg = "新增手续费失败！";
                    return false;
                }
            }
            #endregion

            return true;
        }
        #endregion
        #endregion
    }
}