using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class PurchaseStatisticsPage : BasePage
    {
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["startTime"] == null) return Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00.000"));
                return Convert.ToDateTime(ViewState["startTime"].ToString());
            }
            set { ViewState["startTime"] = value; }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["endTime"] == null) return DateTime.Now;
                return Convert.ToDateTime(ViewState["endTime"].ToString());
            }
            set { ViewState["endTime"] = value; }
        }

        protected void Btn_Search_Click(object sender, EventArgs e)
        {
            //默认一个月的
            //StartTime = RDP_StartTime.SelectedDate == null ? DateTime.Now.AddMonths(-1) : RDP_StartTime.SelectedDate.Value;
            StartTime = Convert.ToDateTime(RDP_StartTime.SelectedDate != null ?
                RDP_StartTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000") :
                DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00.000"));

            //EndTime = RDP_EndTime.SelectedDate == null ? DateTime.Now : RDP_EndTime.SelectedDate.Value;
            EndTime = Convert.ToDateTime(RDP_EndTime.SelectedDate != null ?
                RDP_EndTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000") :
                DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            Rgd_PS.Rebind();
        }

        protected void Rgd_PS_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Rgd_PS.DataSource = _purchasing.GetPurchaseStatisticsList(StartTime, EndTime);
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(Rgd_PS, e);
        }
    }
}
