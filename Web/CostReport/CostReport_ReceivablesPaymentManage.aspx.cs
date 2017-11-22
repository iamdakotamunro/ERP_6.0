using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using System.Collections;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报收付款
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_ReceivablesPaymentManage : BasePage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReckoning _costReckoning = new CostReckoning(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);
        private static readonly IUtility _utility = new UtilityDal(GlobalConfig.DB.FromType.Write);

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
        //结算账号
        protected void LoadPayBankAccountData(IList<CostReportInfo> costReportList)
        {
            var bankAccountList = new List<BankAccountInfo>();
            if (costReportList.Any())
            {
                var bankAccountInfoList = _bankAccounts.GetList();
                var payBankAccountIds = costReportList.Where(p => !p.PayBankAccountId.Equals(Guid.Empty)).Select(p => p.PayBankAccountId).Distinct();
                bankAccountInfoList = bankAccountInfoList.Where(p => payBankAccountIds.Contains(p.BankAccountsId)).ToList();
                if (bankAccountInfoList.Any())
                {
                    bankAccountList.AddRange(bankAccountInfoList);
                }
            }
            var selectedValue = ddl_PayBankAccount.SelectedValue;
            ddl_PayBankAccount.Items.Clear();
            ddl_PayBankAccount.Items.Insert(0, new ListItem("请选择", ""));
            if (bankAccountList.Any())
            {
                foreach (var info in bankAccountList)
                {
                    var name = info.BankName + "【" + info.AccountsName + "】";
                    ddl_PayBankAccount.Items.Add(new ListItem(name, info.BankAccountsId.ToString()));
                }
                if (ddl_PayBankAccount.Items.FindByValue(selectedValue) != null)
                {
                    ddl_PayBankAccount.SelectedValue = selectedValue;
                }
            }
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
                    states.Add((int)CostReportState.Pay);
                    states.Add((int)CostReportState.Complete);
                    break;
                case "1": //未处理
                    //CostReportState.WaitVerify 此状态对于“个人”是【待付款确认】，对于“公司”是【待收款确认】
                    states.Add((int)CostReportState.AlreadyAuditing);
                    states.Add((int)CostReportState.WaitVerify);
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
                    query = query.Where(p => p.ExecuteDate >= startTime);
                }
                if (!string.IsNullOrEmpty(txt_DateTimeEnd.Text))
                {
                    var endtime = DateTime.Parse(txt_DateTimeEnd.Text);
                    query = query.Where(p => p.ExecuteDate < endtime.AddDays(1));
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
            if (!string.IsNullOrEmpty(txt_PayCompany.Text))
            {
                query = query.Where(p => p.PayCompany.Equals(txt_PayCompany.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(ddl_PayBankAccount.SelectedValue))
            {
                query = query.Where(p => p.PayBankAccountId.Equals(new Guid(ddl_PayBankAccount.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue))
            {
                query = query.Where(p => p.AssumeFilialeId.Equals(new Guid(ddl_AssumeFiliale.SelectedValue)));
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

            var list = query.ToList();
            RG_Report.DataSource = list;
            LoadPayBankAccountData(list);
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
        /// 获取处理状态
        /// </summary>
        /// <param name="reportState">状态</param>
        /// <param name="realityCost">申报金额</param>
        /// <returns></returns>
        protected string GetReportState(string reportState, int realityCost)
        {
            if (string.IsNullOrEmpty(reportState))
            {
                return "-";
            }

            var state = int.Parse(reportState);
            if (((int)CostReportState.AlreadyAuditing).Equals(state) && realityCost < 0)
            {
                return "待收款";
            }
            if (((int)CostReportState.AlreadyAuditing).Equals(state) && realityCost > 0)
            {
                return "待付款";
            }
            return EnumAttribute.GetKeyName((CostReportState)state);
        }
        #endregion
        #endregion

        //批量收/付款提示(提示申请单总数、申报金额、待收/付款金额)
        protected void btn_PayAndReceive_Click(object sender, EventArgs e)
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
                    if (!int.Parse(state).Equals((int)CostReportState.AlreadyAuditing) && !int.Parse(state).Equals((int)CostReportState.WaitVerify))
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
                        errorMsg.Append("<b>申请单</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[0] + "</span>张、<b>申报金额</b><span style='color:red; padding:0 5px 0 5px;'>" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(arrayList[1].ToString())) + "</span>元<br/><b>待<span style='color:red;'>付</span>款金额</b><span style='color:red; padding:0 5px 0 5px;'>" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(arrayList[2].ToString())) + "</span>元、<b>待<span style='color:red;'>收</span>款金额</b><span style='color:red; padding:0 5px 0 5px;'>" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(arrayList[3].ToString())) + "</span>元");
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

        //批量收/付款确定
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Hid_ReportId.Value))
            {
                var errorMsg = new StringBuilder();
                var reportIds = Hid_ReportId.Value.Split(',');
                foreach (var item in reportIds)
                {
                    CostReportInfo model = _costReport.GetReportByReportId(new Guid(item));
                    if (model.State != (int)CostReportState.AlreadyAuditing &&
                        model.State != (int)CostReportState.WaitVerify)
                    {
                        errorMsg.Append("“").Append(model.ReportNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    if (model.PayBankAccountId.Equals(Guid.Empty) || model.AssumeFilialeId.Equals(Guid.Empty))
                    {
                        errorMsg.Append("“").Append(model.ReportNo).Append("”【结算账号】或【结算公司】为空！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        PayAndReceive(model);
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

        #region 批量操作
        #region 批量收款或者付款
        protected void PayAndReceive(CostReportInfo model)
        {
            bool execute = false;//是否执行完成操作
            bool need = false;//是否需要完成
            decimal realityCost = model.RealityCost;//当是预借款最后一次时，判断金额大小之后，RealityCost的值已经被改变，故此处需要用一个变量提前暂存
            var bankAccountInfo = _bankAccounts.GetBankAccounts(model.PayBankAccountId);

            #region 结算账号
            string payBankAccount = bankAccountInfo == null ? "暂无结算" : (bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】");
            #endregion

            if (model.CostType.Equals(1) || (model.CostType.Equals(2) && (bankAccountInfo != null && !bankAccountInfo.IsFinish)))
            {
                //在申请页面完成
                need = true;
            }
            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model, need, payBankAccount, out execute);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model, need, payBankAccount);
            }
            else if (model.ReportKind == (int)CostReportKind.FeeIncome)
            {
                execute = true;
                //费用收入
                FeeIncome(model, payBankAccount);
            }

            if (bankAccountInfo != null && bankAccountInfo.IsMain)
            {
                model.IsOut = true;
            }
            else
            {
                model.IsOut = false;
            }

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);
                    if (execute || need)
                    {
                        _costReport.UpdatePayCostAndExecuteDate(model.ReportId, realityCost);
                        _utility.UpdateFieldByPk("lmShop_CostReportAmount", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                        _utility.UpdateFieldByPk("lmShop_CostReportBill", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                        model.RealityCost = realityCost;
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
        protected void BeforeLoan(CostReportInfo model, bool need, string payBankAccount, out bool execute)
        {
            execute = false;
            if (need || (model.IsSystem && model.RealityCost < 0))
            {
                if (model.Deposit.Equals(1))
                {
                    model.State = (int)CostReportState.Complete;
                    model.FinishDate = DateTime.Now;
                    model.DepositNo = new CodeManager().GetCode(CodeType.RE);
                    model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
                }
                else
                {
                    if (model.IsSystem)
                    {
                        if (model.RealityCost > 0)
                        {
                            model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
                        }
                        else if (model.RealityCost < 0)
                        {
                            execute = true;
                            model.Memo = WebControl.RetrunUserAndTime("[【已收款】:已收入" + WebControl.RemoveDecimalEndZero(Math.Abs(model.RealityCost)) + "元;结算账号:" + payBankAccount + ";]");
                        }
                        model.State = (int)CostReportState.Complete;
                        model.FinishDate = DateTime.Now;
                    }
                    else if (model.IsLastTime)
                    {
                        model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");

                        //预借款时，可能出现(“票据金额”=“申请金额”,“票据金额”>“申请金额”,“票据金额”<“申请金额”)三种情况
                        var difference = model.ApplyForCost - model.ActualAmount;//“票据金额”和“申请金额”的差额
                        if (difference == 0 || difference > 0)
                        {
                            model.RealityCost = difference;
                            model.State = (int)CostReportState.Complete;
                            model.FinishDate = DateTime.Now;
                        }
                        else if (difference < 0)
                        {
                            model.RealityCost = difference;
                            model.ReportMemo = "打款金额多了" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(difference)) + "元";
                            model.State = (int)CostReportState.WaitVerify;

                            #region 插入系统生成的金额
                            CostReportAmountInfoList = _costReportAmount.GetmShop_CostReportAmountByReportId(model.ReportId).Where(p => !p.IsSystem).ToList();
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
                        model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
                    }
                }
            }
            else
            {
                model.State = (int)CostReportState.Pay;
                model.Memo = WebControl.RetrunUserAndTime("[【待付款】:待支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;]");
            }
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model, bool need, string payBankAccount)
        {
            if (need)
            {
                model.State = (int)CostReportState.Complete;
                model.FinishDate = DateTime.Now;
                model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
            }
            else
            {
                model.State = (int)CostReportState.Pay;
                model.Memo = WebControl.RetrunUserAndTime("[【待付款】:待支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;]");
            }
        }
        #endregion

        #region 费用收入
        protected void FeeIncome(CostReportInfo model, string payBankAccount)
        {
            model.State = (int)CostReportState.Complete;
            model.FinishDate = DateTime.Now;
            model.Memo = WebControl.RetrunUserAndTime("[【已收款】:已收入" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";]");
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
            var costReckoningInfo = costReportBll.AddCostReckoningInfo(model, personnelInfo, true);
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