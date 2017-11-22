using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using ERP.Enum.Attribute;
using ERP.Model;
using System.Web.UI;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;
using ERP.DAL.Implement.Inventory;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用票据受理
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_InvoiceAcceptManage : BasePage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAuditingPower _costReportAuditingPower = new DAL.Implement.Inventory.CostReportAuditingPower(GlobalConfig.DB.FromType.Read);
        private static readonly IUtility _utility = new UtilityDal(GlobalConfig.DB.FromType.Write);
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
        /// 申请金额列表属性
        /// </summary>
        protected List<CostReportAmountInfo> CostReportAmountInfoList
        {
            get
            {
                if (ViewState["CostReportAmountInfoList"] == null)
                    return new List<CostReportAmountInfo>();
                return ViewState["CostReportAmountInfoList"] as List<CostReportAmountInfo>;
            }
            set
            {
                ViewState["CostReportAmountInfoList"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReportKindData();
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
        //申报类型
        protected void LoadReportKindData()
        {
            var list = EnumAttribute.GetDict<CostReportKind>().OrderBy(p => p.Key);
            ddl_ReportKind.DataSource = list;
            ddl_ReportKind.DataTextField = "Value";
            ddl_ReportKind.DataValueField = "Key";
            ddl_ReportKind.DataBind();
            ddl_ReportKind.Items.Insert(0, new ListItem("请选择", ""));
        }

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
                var personnelList = _personnelSao.GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
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
                    states.Add((int)CostReportState.CompletedMayApply);
                    //states.Add((int)CostReportState.Auditing);
                    states.Add((int)CostReportState.WaitVerify);
                    states.Add((int)CostReportState.Complete);
                    break;
                case "1": //未处理
                    states.Add((int)CostReportState.NoAuditing);
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
                    query = query.Where(p => p.AcceptDate >= startTime);
                }
                if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
                {
                    var endtime = DateTime.Parse(txt_DateTimeEnd.Text);
                    query = query.Where(p => p.AcceptDate < endtime.AddDays(1));
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
            if (!string.IsNullOrEmpty(txt_InvoiceAndReceiptNo.Text))
            {
                query = query.Where(p => p.InvoiceNo.Equals(txt_InvoiceAndReceiptNo.Text) || p.ReceiptNo.Equals(txt_InvoiceAndReceiptNo.Text));
            }
            if (!string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue))
            {
                query = query.Where(p => p.AssumeFilialeId.Equals(new Guid(ddl_AssumeFiliale.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_ReportKind.SelectedValue))
            {
                query = query.Where(p => p.ReportKind.Equals(int.Parse(ddl_ReportKind.SelectedValue)));
            }
            query = query.Where(p => p.InvoiceType.Equals((int)CostReportInvoiceType.Invoice) || p.InvoiceType.Equals((int)CostReportInvoiceType.VatInvoice) || p.InvoiceType.Equals((int)CostReportInvoiceType.Voucher));
            #endregion

            #region 获取权限数据集

            #region 获取数据集中所有申报人的部门集合
            var reportPersonnelIdList = query.Select(act => act.ReportPersonnelId).Distinct().ToList();
            var branchIds = MISService.GetBranchIdsByPersonnelIds(reportPersonnelIdList);
            #endregion

            #region 费用申报受理权限
            var costReportAuditingList = _costReportAuditingPower.GetCanAuditingInfo(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(p => p.Kind == (int)CostReportAuditingType.Invoice);
            #endregion

            foreach (CostReportAuditingInfo info in costReportAuditingList)
            {
                foreach (var item in query)
                {
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
            if (query.Any())
            {
                var sumReportCost = query.Sum(p => Math.Abs(p.ReportCost));
                totalName.FooterText = "合计：";
                reportCost.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumReportCost));
            }
            else
            {
                totalName.FooterText = string.Empty;
                reportCost.FooterText = string.Empty;
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
        #endregion

        //批量受理提示(提示申请单总数、发票总数、收据总数)
        protected void btn_Accept_Click(object sender, EventArgs e)
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
                    if (!int.Parse(state).Equals((int)CostReportState.NoAuditing))
                    {
                        errorMsg.Append("“").Append(reportNo).Append("”状态已更新，不允许此操作！").Append("<br/>");
                        continue;
                    }
                    reportIds += "," + reportId;
                }

                if (!string.IsNullOrEmpty(reportIds))
                {
                    Hid_ReportId.Value = reportIds.Substring(1);
                    ArrayList arrayList = _costReport.GetSumInvoiceAccept(Hid_ReportId.Value.Split(','));
                    if (arrayList.Count > 0)
                    {
                        errorMsg.Append("<b>申请单</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[0] + "</span>张、<b>发票</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[1] + "</span>张、<b>收据</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[2] + "</span>张");
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

        //批量受理通过
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Hid_ReportId.Value))
            {
                var errorMsg = new StringBuilder();
                var reportIds = Hid_ReportId.Value.Split(',');
                foreach (var item in reportIds)
                {
                    CostReportInfo model = _costReport.GetReportByReportId(new Guid(item));
                    if (model.State != (int)CostReportState.NoAuditing)
                    {
                        errorMsg.Append("“").Append(model.ReportNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        AcceptPass(model);
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

        #region 批量受理
        #region 受理通过
        protected void AcceptPass(CostReportInfo model)
        {
            bool isPayBill = false;//是否将票据设置成付款完成
            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model, out isPayBill);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Paying)
            {
                //已扣款核销
                PayVerification(model);
            }
            model.AcceptDate = DateTime.Now;

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReportBill.Updatelmshop_CostReportBillForPassByReportId(model.ReportId, true);
                    if (isPayBill)
                    {
                        _utility.UpdateFieldByPk("lmShop_CostReportBill", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                    }
                    _costReport.UpdateReport(model);
                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            #endregion
        }
        #endregion

        #region 预借款
        protected void BeforeLoan(CostReportInfo model, out bool isPayBill)
        {
            CostReportAmountInfoList = _costReportAmount.GetmShop_CostReportAmountByReportId(model.ReportId).Where(p => !p.IsSystem).ToList();
            //获取未付款的申请金额的数据
            var isPayAmountList = CostReportAmountInfoList.Where(p => !p.IsPay);
            if (isPayAmountList.Any())
            {
                model.State = (int)CostReportState.AlreadyAuditing;
                isPayBill = false;
            }
            else
            {
                if (model.IsLastTime)
                {
                    //预借款时，可能出现(“票据金额”=“申请金额”,“票据金额”>“申请金额”,“票据金额”<“申请金额”)三种情况
                    var difference = model.ApplyForCost - model.ActualAmount;//“票据金额”和“申请金额”的差额
                    if (difference == 0 || difference > 0)
                    {
                        model.RealityCost = difference;
                        model.State = (int)CostReportState.Complete;
                        model.FinishDate = DateTime.Now;
                        model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;]");
                    }
                    else if (difference < 0)
                    {
                        model.RealityCost = difference;
                        model.ReportMemo = "打款金额多了" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(difference)) + "元";
                        model.State = (int)CostReportState.WaitVerify;
                        model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;" + model.ReportMemo + ";]");

                        #region 插入系统生成的金额
                        var maxNum = CostReportAmountInfoList.Any() ? CostReportAmountInfoList.Select(p => p.Num).Max() : 0;
                        CostReportAmountInfoList.Add(new CostReportAmountInfo
                        {
                            AmountId = Guid.NewGuid(),
                            ReportId = Guid.Empty,
                            Num = maxNum + 1,
                            Amount = difference,
                            IsPay = true,
                            IsSystem = true
                        });
                        model.IsSystem = true;
                        #endregion
                    }
                }
                else
                {
                    model.ApplyNumber = ++model.ApplyNumber;
                    model.State = (int)CostReportState.CompletedMayApply;
                }
                isPayBill = true;
            }
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;]");
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model)
        {
            if (string.IsNullOrEmpty(model.DepositNo))//正常流程的“凭证报销”
            {
                model.State = (int)CostReportState.AlreadyAuditing;
            }
            else//押金回收的“凭证报销”
            {
                model.State = (int)CostReportState.Complete;
            }
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;]");
        }
        #endregion

        #region 已扣款核销
        protected void PayVerification(CostReportInfo model)
        {
            model.State = (int)CostReportState.Complete;
            model.FinishDate = DateTime.Now;
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;]");
        }
        #endregion
        #endregion
    }
}