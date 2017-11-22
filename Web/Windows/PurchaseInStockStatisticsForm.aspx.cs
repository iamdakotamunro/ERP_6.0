using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class PurchaseStatisticsPageForm : WindowsPage
    {
        private readonly static Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        //private readonly SupplierReportBll _supplierReportBll = new SupplierReportBll(new SupplierReportDao(), new CompanyCussent(GlobalConfig.DB.FromType.Read), new Reckoning(GlobalConfig.DB.FromType.Read));
        private static readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);
        private static Dictionary<int, decimal> _dicSum = new Dictionary<int, decimal>();

        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var filialeId = Request.QueryString["FilialeId"];
                if (string.IsNullOrWhiteSpace(filialeId))
                {
                    return;
                }
                var filialeName = new Guid(filialeId) == _reckoningElseFilialeid ? "ERP" 
                    : CacheCollection.Filiale.GetName(new Guid(filialeId));
                Page.Title = "【" + filialeName + "】采购入库详情";
            }
        }

        /// <summary>绑定数据源
        /// </summary>
        protected void RgPurchaseStatisticsDetailsOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            _dicSum=new Dictionary<int, decimal>();
            IList<SupplierPaymentsReportInfo> dataList = new List<SupplierPaymentsReportInfo>();
            var filialeId = Request.QueryString["FilialeId"];
            var year = Request.QueryString["Year"];
            if (!string.IsNullOrWhiteSpace(filialeId) && filialeId != Guid.Empty.ToString())
            {
                var id = new Guid(filialeId);
                if (string.IsNullOrWhiteSpace(year))
                    year = string.Format("{0}", DateTime.Now.Year);
                for (int i = 1; i <= 12; i++)
                {
                    var list = _reckoning.SelectCurrentMonthStockRecordsDetail(Convert.ToInt32(year), i,id,CompanyName);
                    decimal payData = list.Sum(act => act.TotalAmount);
                    foreach (var info in list)
                    {
                        var item = dataList.FirstOrDefault(act => act.CompanyId == info.CompanyId)
                        ?? new SupplierPaymentsReportInfo { CompanyId = info.CompanyId,CompanyName = info.CompanyName };
                        switch (i)
                        {
                            case 1:
                                item.January = info.TotalAmount;
                                break;
                            case 2:
                                item.February = info.TotalAmount;
                                break;
                            case 3:
                                item.March = info.TotalAmount;
                                break;
                            case 4:
                                item.April = info.TotalAmount;
                                break;
                            case 5:
                                item.May = info.TotalAmount;
                                break;
                            case 6:
                                item.June = info.TotalAmount;
                                break;
                            case 7:
                                item.July = info.TotalAmount;
                                break;
                            case 8:
                                item.August = info.TotalAmount;
                                break;
                            case 9:
                                item.September = info.TotalAmount;
                                break;
                            case 10:
                                item.October = info.TotalAmount;
                                break;
                            case 11:
                                item.November = info.TotalAmount;
                                break;
                            case 12:
                                item.December = info.TotalAmount;
                                break;
                        }
                        if (dataList.All(act => act.CompanyId != info.CompanyId))
                        {
                            dataList.Add(item);
                        }
                    }
                    _dicSum.Add(i, payData);
                }
            }
            if (DateTime.Now.Year == Convert.ToInt32(year))
            {
                switch (DateTime.Now.Month)
                {
                    case 1:
                        dataList = dataList.OrderByDescending(act => act.January).ToList();
                        break;
                    case 2:
                        dataList = dataList.OrderByDescending(act => act.February).ToList();
                        break;
                    case 3:
                        dataList = dataList.OrderByDescending(act => act.March).ToList();
                        break;
                    case 4:
                        dataList = dataList.OrderByDescending(act => act.April).ToList();
                        break;
                    case 5:
                        dataList = dataList.OrderByDescending(act => act.May).ToList();
                        break;
                    case 6:
                        dataList = dataList.OrderByDescending(act => act.June).ToList();
                        break;
                    case 7:
                        dataList = dataList.OrderByDescending(act => act.July).ToList();
                        break;
                    case 8:
                        dataList = dataList.OrderByDescending(act => act.August).ToList();
                        break;
                    case 9:
                        dataList = dataList.OrderByDescending(act => act.September).ToList();
                        break;
                    case 10:
                        dataList = dataList.OrderByDescending(act => act.October).ToList();
                        break;
                    case 11:
                        dataList = dataList.OrderByDescending(act => act.November).ToList();
                        break;
                    case 12:
                        dataList = dataList.OrderByDescending(act => act.December).ToList();
                        break;
                }
            }
            else
            {
                dataList = dataList.OrderByDescending(act => act.December).ToList();
            }
            RgPurchaseStatisticsDetails.DataSource = dataList;
        }

        /// <summary>搜索
        /// </summary>
        protected void RgPurchaseStatisticsDetailsOnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            CompanyName = ((RadTextBox)e.Item.FindControl("RTB_CompanyName")).Text.Trim();
            RgPurchaseStatisticsDetails.CurrentPageIndex = 0;
            RgPurchaseStatisticsDetails.Rebind();
        }

        #region [ViewState] (供应商名称)

        /// <summary>供应商名称
        /// </summary>
        protected string CompanyName
        {
            get
            {
                return ViewState["CompanyName"] == null ? string.Empty : ViewState["CompanyName"].ToString();
            }
            set
            {
                ViewState["CompanyName"] = value;
            }
        }

        #endregion

        #region
        /// <summary>
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public string ShowStr(object money)
        {
            if (money == null) return string.Empty;
            if (money.ToString().Length == 0) return string.Empty;
            if (Convert.ToDecimal(money) == 0) return string.Empty;
            return string.Format("{0}", WebControl.NumberSeparator(money));
        }
        #endregion
    }
}