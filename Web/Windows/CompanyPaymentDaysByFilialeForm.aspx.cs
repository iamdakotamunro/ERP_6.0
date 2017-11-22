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
    /// <summary>往来单位账期详情查询  ADD  2015-03-10  陈重文  
    /// </summary>
    public partial class CompanyPaymentDaysByFilialeForm : WindowsPage
    {
        private readonly static Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private static readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Read);
        private static Dictionary<int, decimal> _dicSum = new Dictionary<int, decimal>();
        //private readonly SupplierReportBll _supplierReportBll = new SupplierReportBll(new SupplierReportDao(), new CompanyCussent(GlobalConfig.DB.FromType.Read), new Reckoning(GlobalConfig.DB.FromType.Read));

        private static Dictionary<int, decimal> _dicPaySum = new Dictionary<int, decimal>();

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
                var filialeName = new Guid(filialeId) == _reckoningElseFilialeid ? "ERP" : CacheCollection.Filiale.GetName(new Guid(filialeId));
                Page.Title = "【" + filialeName + "】应付款详情";
            }
        }

        /// <summary>绑定数据源
        /// </summary>
        protected void RgCompanyPaymentDaysDetailsOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<SupplierPaymentsReportInfo> dataList = new List<SupplierPaymentsReportInfo>();
            var filialeId = Request.QueryString["FilialeId"];
            var year = Request.QueryString["Year"];
            _dicSum = new Dictionary<int, decimal>();
            _dicPaySum = new Dictionary<int, decimal>();
            if (!string.IsNullOrWhiteSpace(filialeId) && filialeId != Guid.Empty.ToString())
            {
                var id = new Guid(filialeId);
                if (string.IsNullOrWhiteSpace(year))
                    year = string.Format("{0}", DateTime.Now.Year);

                //dataList = _supplierReportBll.SelectPaymentsReportsGroupByCompanyId(Convert.ToInt32(year), id, CompanyName,
                 //   true);
                for (int i = 1; i <= 12; i++)
                {
                    var detailList = _reckoning.SelectCurrentMonthPaymentsRecordsDetail(Convert.ToInt32(year), i, id, CompanyName);
                    foreach (var supplierPaymentsRecordInfo in detailList)
                    {
                        var item = dataList.FirstOrDefault(act => act.CompanyId == supplierPaymentsRecordInfo.CompanyId) ??
                            new SupplierPaymentsReportInfo{
                                CompanyId = supplierPaymentsRecordInfo.CompanyId,
                                CompanyName = supplierPaymentsRecordInfo.CompanyName,
                                PaymentDays = supplierPaymentsRecordInfo.PaymentDays};
                        switch (i)
                        {
                            case 1:
                                item.January = -supplierPaymentsRecordInfo.TotalAmount;
                                item.January1 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 2:
                                item.February = -supplierPaymentsRecordInfo.TotalAmount;
                                item.February2 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 3:
                                item.March = -supplierPaymentsRecordInfo.TotalAmount;
                                item.March3 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 4:
                                item.April = -supplierPaymentsRecordInfo.TotalAmount;
                                item.April4 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 5:
                                item.May = -supplierPaymentsRecordInfo.TotalAmount;
                                item.May5 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 6:
                                item.June = -supplierPaymentsRecordInfo.TotalAmount;
                                item.June6 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 7:
                                item.July = -supplierPaymentsRecordInfo.TotalAmount;
                                item.July7 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 8:
                                item.August = -supplierPaymentsRecordInfo.TotalAmount;
                                item.August8 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 9:
                                item.September = -supplierPaymentsRecordInfo.TotalAmount;
                                item.September9 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 10:
                                item.October = -supplierPaymentsRecordInfo.TotalAmount;
                                item.October10 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            case 11:
                                item.November = -supplierPaymentsRecordInfo.TotalAmount;
                                item.November11 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                            default:
                                item.December = -supplierPaymentsRecordInfo.TotalAmount;
                                item.December12 = -supplierPaymentsRecordInfo.TotalNoPayed;
                                break;
                        }
                        if (dataList.All(act => act.CompanyId != supplierPaymentsRecordInfo.CompanyId))
                        {
                            dataList.Add(item);
                        }
                    }
                    decimal payData = detailList.Sum(act => act.TotalAmount);
                    decimal noPayData = detailList.Sum(act => act.TotalNoPayed);
                    if (!_dicSum.ContainsKey(i))
                    {
                        _dicSum.Add(i, payData==0?0:-payData);
                    }
                    if (!_dicPaySum.ContainsKey(i))
                    {
                        _dicPaySum.Add(i, noPayData == 0 ? 0 : -noPayData);
                    }
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
            RGCompanyPaymentDaysDetails.DataSource = dataList;
        }

        /// <summary>搜索
        /// </summary>
        protected void RGCompanyPaymentDaysDetails_OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            CompanyName = ((RadTextBox)e.Item.FindControl("RTB_CompanyName")).Text.Trim();
            RGCompanyPaymentDaysDetails.CurrentPageIndex = 0;
            RGCompanyPaymentDaysDetails.Rebind();
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
        /// <param name="money2"></param>
        /// <returns></returns>
        public string ShowStr(object money, object money2)
        {
            if (money == null) return string.Empty;
            if (money.ToString().Length == 0) return string.Empty;
            if (Convert.ToDecimal(money) == 0) return string.Empty;
            return string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(money), WebControl.NumberSeparator(money2));
        }
        #endregion
    }
}