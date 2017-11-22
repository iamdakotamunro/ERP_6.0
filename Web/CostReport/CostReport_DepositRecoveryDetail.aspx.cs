using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_DepositRecoveryDetail : WindowsPage
    {

        private static readonly ICostReport _costReport =
            new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);

        private static readonly ICostReportDepositRecovery _costReportDepositRecovery =
            new DAL.Implement.Inventory.CostReportDepositRecovery(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportId = Request.QueryString["ReportId"];
                if (string.IsNullOrEmpty(reportId)) return;
                CostReportInfo costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));
                if (costReportInfo!=null)
                {
                    LoadReportData(costReportInfo); //初始化页面数据
                }
                
            }
        }

        //初始化页面数据
        private void LoadReportData(CostReportInfo model)
        {
            var branchinfo =
                CacheCollection.Branch.GetSystemBranchList().FirstOrDefault(p => p.ID == model.AssumeBranchId);

            lbl_ReportBranch.Text = branchinfo == null ? "" : branchinfo.Name;
            lbl_CompanyClass.Text = Cost.ReadInstance.GetCompanyName(model.CompanyClassId, model.CompanyId);
            lbl_ReportName.Text = model.ReportName;
            lbl_ReportDate.Text = model.StartTime.ToString("yyyy年MM月") + "至" + model.EndTime.ToString("yyyy年MM月");
            lbl_ReportCost.Text = model.ReportCost.ToString(CultureInfo.InvariantCulture);
            lbl_ReportNo.Text = model.ReportNo;
            lbl_PayCompany.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
            lbl_ExecuteDate.Text = model.ExecuteDate.ToString("yyyy-MM-dd");
            lbl_BankAccount.Text = model.BankName;
            lbl_ReportPersonnelId.Text = new PersonnelManager().Get(model.ReportPersonnelId).RealName;

            var depositRecoveryList = _costReportDepositRecovery.GetDepositRecoveryList(model.ReportId);
            if (depositRecoveryList.Count>0)
            {
                lbl_RecoveryPersonnelId.Text = new PersonnelManager().Get(depositRecoveryList[0].RecoveryPersonnelId).RealName ;
            }
            if (depositRecoveryList.Count==1)
            {
                lbl_RecoveryType.Text = depositRecoveryList[0].RecoveryType ? "票据" : "现金";
                lbl_RecoveryCost.Text = depositRecoveryList[0].RecoveryCost.ToString();
            }
            else if (depositRecoveryList.Count == 2)
            {
                lbl_RecoveryType.Text = depositRecoveryList[0].RecoveryType ? "票据" : "现金";
                lbl_RecoveryCost.Text = depositRecoveryList[0].RecoveryCost.ToString();
                lbl_RecoveryType2.Text = depositRecoveryList[1].RecoveryType ? "票据" : "现金";
                lbl_RecoveryCost2.Text = depositRecoveryList[1].RecoveryCost.ToString();
            }
        }
    }
}