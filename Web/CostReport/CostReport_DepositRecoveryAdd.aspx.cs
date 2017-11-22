using System;
using System.Globalization;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using ERP.UI.Web.Base;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
    * 创建人：  文雯
    * 创建时间：2016/07/04  
    * 描述    :押金回收
    * =====================================================================
    * 修改时间：2016/07/04  
    * 修改人  ：  
    * 描述    ：
    */
    public partial class CostReport_DepositRecoveryAdd : WindowsPage
    {
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportDepositRecovery _costReportDepositRecovery = new DAL.Implement.Inventory.CostReportDepositRecovery(GlobalConfig.DB.FromType.Write);
        readonly IUtility _utility = new UtilityDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportId = Request.QueryString["ReportId"];
                if (string.IsNullOrEmpty(reportId)) return;
                CostReportInfo costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));
                LoadReportData(costReportInfo);//初始化页面数据
            }
        }

        //初始化页面数据
        private void LoadReportData(CostReportInfo model)
        {
            lbl_FilialeName.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
            lbl_BankAccount.Text = model.BankName;
            lbl_ReportName.Text = model.ReportName;
            lbl_CompanyClass.Text = Cost.ReadInstance.GetCompanyName(model.CompanyClassId, model.CompanyId);
            lbl_ReportNo.Text = model.ReportNo;
            lbl_ExecuteDate.Text = model.ExecuteDate.ToString("yyyy-MM-dd").Equals("1900-01-01") ? "" : model.ExecuteDate.ToString("yyyy-MM-dd");
            lbl_PayCost.Text = model.PayCost.ToString(CultureInfo.InvariantCulture);
            lbl_ReportPersonnelName.Text = new PersonnelManager().Get(model.ReportPersonnelId).RealName;
        }

        /// <summary>
        /// 押金回收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            var reportId = Request.QueryString["ReportId"];
            if (reportId == null)
            {
                return;
            }
            var exist = _utility.CheckExists("lmShop_CostReportDepositRecovery", "ReportId", reportId);
            if (exist)
            {
                MessageBox.Show(this, "该单据已回收，不允许此操作！");
                return;
            }

            var model = _costReport.GetReportByReportId(new Guid(reportId));

            decimal editCost = 0;

            if (Chk_Bill.Checked)
            {
                editCost += txt_BillCost.Text.Trim() == "" ? 0 : Convert.ToDecimal(txt_BillCost.Text);
            }
            if (Chk_Cash.Checked)
            {
                editCost += txt_CashCost.Text.Trim() == "" ? 0 : Convert.ToDecimal(txt_CashCost.Text);
            }
            if (editCost != model.PayCost)
            {
                MessageBox.Show(this, "“回收金额”必须等于“付款金额”！");
                return;
            }

            CostReportInfo costReportLater = null;//凭证报销
            CostReportInfo costReportFeeIncome = null;//费用收入

            CostReportDepositRecoveryInfo depositRecoveryLater = null;//押金回收(凭证报销)
            CostReportDepositRecoveryInfo depositRecoveryFeeIncome = null;//押金回收(费用收入)
            var personnelInfo = CurrentSession.Personnel.Get();
            if (Chk_Bill.Checked)
            {
                #region 凭证报销
                costReportLater = new CostReportInfo();
                costReportLater.ReportId = Guid.NewGuid();
                costReportLater.ReportNo = new CodeManager().GetCode(CodeType.RE);
                costReportLater.ReportKind = (int)CostReportKind.Later;
                costReportLater.AssumeBranchId = model.AssumeBranchId;
                costReportLater.AssumeGroupId = model.AssumeGroupId;
                costReportLater.AssumeShopId = model.AssumeShopId;
                costReportLater.CostsVarieties = model.CostsVarieties;
                costReportLater.GoodsCode = model.GoodsCode;
                costReportLater.CompanyClassId = model.CompanyClassId;
                costReportLater.CompanyId = model.CompanyId;
                costReportLater.UrgentOrDefer = 1;
                costReportLater.UrgentReason = string.Empty;
                costReportLater.ReportName = model.ReportName;
                costReportLater.StartTime = model.StartTime;
                costReportLater.EndTime = model.EndTime;
                costReportLater.PayCompany = model.PayCompany;
                costReportLater.ReportCost = Math.Abs(Decimal.Parse(txt_BillCost.Text));
                costReportLater.RealityCost = costReportLater.ReportCost;
                costReportLater.ApplyForCost = model.ApplyForCost;
                costReportLater.ActualAmount = costReportLater.ReportCost;
                costReportLater.CostType = model.CostType;
                costReportLater.Deposit = 2;
                costReportLater.DepositNo = model.DepositNo;
                costReportLater.BankAccountName = model.BankAccountName;
                costReportLater.BankAccount = model.BankAccount;
                costReportLater.InvoiceType = (int)CostReportInvoiceType.Invoice;
                costReportLater.InvoiceId = model.InvoiceId;
                costReportLater.InvoiceTitle = model.InvoiceTitle;
                costReportLater.InvoiceTitleFilialeId = model.InvoiceTitleFilialeId;
                costReportLater.ReportMemo = string.Empty;

                costReportLater.ReportFilialeId = model.ReportFilialeId;
                costReportLater.ReportBranchId = model.ReportBranchId;
                costReportLater.ReportPersonnelId = model.ReportPersonnelId;
                costReportLater.State = (int)CostReportState.Auditing;
                costReportLater.ReportDate = DateTime.Now;
                costReportLater.Memo = WebControl.RetrunUserAndTime("[【凭证报销】：押金回收;]");
                costReportLater.IsLastTime = true;
                costReportLater.IsSystem = false;
                costReportLater.ApplyNumber = 1;
                costReportLater.IsEnd = false;

                #region 收付款赋值
                costReportLater.PayBankAccountId = model.PayBankAccountId;
                costReportLater.AssumeFilialeId = model.AssumeFilialeId;
                costReportLater.IsOut = model.IsOut;
                #endregion
                #endregion

                #region 押金回收
                depositRecoveryLater = new CostReportDepositRecoveryInfo
                {
                    ReportId = model.ReportId,
                    DepositRecoveryReportId = costReportLater.ReportId,
                    RecoveryCost = costReportLater.ReportCost,
                    RecoveryDate = DateTime.Now,
                    RecoveryType = true,
                    RecoveryRemarks = txt_RecoveryRemarks.Text,
                    RecoveryPersonnelId = personnelInfo.PersonnelId
                };
                #endregion
            }

            if (Chk_Cash.Checked)
            {
                #region 费用收入
                costReportFeeIncome = new CostReportInfo();
                costReportFeeIncome.ReportId = Guid.NewGuid();
                costReportFeeIncome.ReportNo = new CodeManager().GetCode(CodeType.RE);
                costReportFeeIncome.ReportKind = (int)CostReportKind.FeeIncome;
                costReportFeeIncome.AssumeBranchId = model.AssumeBranchId;
                costReportFeeIncome.AssumeGroupId = model.AssumeGroupId;
                costReportFeeIncome.AssumeShopId = model.AssumeShopId;
                costReportFeeIncome.CostsVarieties = model.CostsVarieties;
                costReportFeeIncome.GoodsCode = model.GoodsCode;
                costReportFeeIncome.CompanyClassId = model.CompanyClassId;
                costReportFeeIncome.CompanyId = model.CompanyId;
                costReportFeeIncome.UrgentOrDefer = 1;
                costReportFeeIncome.UrgentReason = string.Empty;
                costReportFeeIncome.ReportName = model.ReportName;
                costReportFeeIncome.StartTime = model.StartTime;
                costReportFeeIncome.EndTime = model.EndTime;
                costReportFeeIncome.PayCompany = string.Empty;
                costReportFeeIncome.ReportCost = Math.Abs(Decimal.Parse(txt_CashCost.Text));
                costReportFeeIncome.RealityCost = costReportFeeIncome.ReportCost;
                costReportFeeIncome.ApplyForCost = model.ReportCost;
                costReportFeeIncome.ActualAmount = costReportFeeIncome.ReportCost;
                costReportFeeIncome.CostType = 2;
                costReportFeeIncome.Deposit = 2;
                costReportFeeIncome.DepositNo = model.DepositNo;
                costReportFeeIncome.BankAccountName = string.Empty;
                costReportFeeIncome.InvoiceType = (int)CostReportInvoiceType.WaitCheck;
                costReportFeeIncome.PayBankAccountId = model.PayBankAccountId; //结算账号
                costReportFeeIncome.AssumeFilialeId = model.AssumeFilialeId; //结算公司
                costReportFeeIncome.ReportMemo = string.Empty;

                costReportFeeIncome.ReportFilialeId = model.ReportFilialeId;
                costReportFeeIncome.ReportBranchId = model.ReportBranchId;
                costReportFeeIncome.ReportPersonnelId = model.ReportPersonnelId;
                costReportFeeIncome.State = (int)CostReportState.WaitVerify;
                costReportFeeIncome.ReportDate = DateTime.Now;
                costReportFeeIncome.Memo = WebControl.RetrunUserAndTime("[【费用收入】：押金回收;]");
                costReportFeeIncome.IsLastTime = true;
                costReportFeeIncome.IsSystem = false;
                costReportFeeIncome.ApplyNumber = 1;
                costReportFeeIncome.IsOut = model.IsOut;
                costReportFeeIncome.IsEnd = false;
                #endregion

                #region 押金回收
                depositRecoveryFeeIncome = new CostReportDepositRecoveryInfo
                {
                    ReportId = model.ReportId,
                    DepositRecoveryReportId = costReportFeeIncome.ReportId,
                    RecoveryCost = costReportFeeIncome.ReportCost,
                    RecoveryDate = DateTime.Now,
                    RecoveryType = false,
                    RecoveryRemarks = txt_RecoveryRemarks.Text,
                    RecoveryPersonnelId = personnelInfo.PersonnelId
                };
                #endregion
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    string errorMessage;
                    //凭证报销
                    if (costReportLater != null)
                    {
                        var result = _costReport.InsertReport(costReportLater, out errorMessage);
                        if (result)
                        {
                            _costReportDepositRecovery.InsertDepositRecovery(depositRecoveryLater);
                        }
                    }
                    //费用收入
                    if (costReportFeeIncome != null)
                    {
                        var result = _costReport.InsertReport(costReportFeeIncome, out errorMessage);
                        if (result)
                        {
                            _costReportDepositRecovery.InsertDepositRecovery(depositRecoveryFeeIncome);
                        }
                    }
                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
                finally
                {
                    //释放资源
                    ts.Dispose();
                }
            }
        }
    }
}