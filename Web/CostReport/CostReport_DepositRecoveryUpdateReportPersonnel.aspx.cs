using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_DepositRecoveryUpdateReportPersonnel : WindowsPage
    {
        private static readonly ICostReport _costReport =
           new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RG_Report_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            if (string.IsNullOrEmpty(hf_oldReportPersonnel.Value))
            {
                var info = new List<CostReportInfo>();
                RG_ReportDepositRecovery.DataSource = info;
            }
            else
            {
                var reportPersonnelIds =hf_oldReportPersonnel.Value.Split(',').Select(personnelId => new Guid(personnelId)).ToList();
                var costReportList = _costReport.GetReportListForDeposit(0, DateTime.MinValue,DateTime.MinValue, reportPersonnelIds,string.Empty);
                var query = costReportList.AsQueryable();

                RG_ReportDepositRecovery.DataSource = query.OrderByDescending(p => p.ReportDate).ToList();
            }
        }

        #region 行绑定帮助事件


        /// <summary>
        /// 获取申请人公司
        /// </summary>
        /// <returns></returns>
        protected string GetPersonnelFiliale(string personnelId)
        {
            var filialeId = new PersonnelManager().Get(new Guid(personnelId)).FilialeId;
            return CacheCollection.Filiale.Get(filialeId).Name;
        }

        #endregion

        /// <summary>
        /// 申请人转移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var arrReportId = Request["ckId"].Split(',');
                var reportIdlist = new List<Guid>();
                foreach (var item in arrReportId)
                {
                    reportIdlist.Add(new Guid(item));
                }

                var result = _costReport.UpdateReportPersonnelIdByReportId(reportIdlist, new Guid(hf_newReportPersonnel.Value));
                if (!result)
                {
                    MessageBox.Show(this, "转派失败！");
                }
                else
                {
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }


        protected void btn_Bind_Click(object sender, EventArgs e)
        {
            RG_ReportDepositRecovery.Rebind();
        }
    }
}