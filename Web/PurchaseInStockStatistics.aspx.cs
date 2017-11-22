using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class PurchaseInStockStatistics : BasePage
    {
        //其他公司
        readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        //公司列表
        private static readonly IList<FilialeInfo> _filialeList = CacheCollection.Filiale.GetHeadList();
        private static readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Read);
        //private readonly SupplierReportBll _supplierReportBll = new SupplierReportBll(new SupplierReportDao(), new CompanyCussent(GlobalConfig.DB.FromType.Read), new Reckoning(GlobalConfig.DB.FromType.Read));

        private static Dictionary<int, decimal> _dicSum = new Dictionary<int, decimal>();

        private static Dictionary<Guid, string> _dics = new Dictionary<Guid, string>(); 

        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var item = _filialeList.Where(ent => ent.ID == _reckoningElseFilialeid).ToList();
                if (item.Count == 0)
                {
                    _filialeList.Add(new FilialeInfo
                    {
                        ID = _reckoningElseFilialeid,
                        Name = "ERP"
                    });
                }
                _dics = _filialeList.ToDictionary(k => k.ID, v => v.Name);
            }
        }

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        protected void RgPurchaseInStockStatisticsOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            _dicSum = new Dictionary<int, decimal>();
            var list = new List<SupplierPaymentsReportInfo>();
            //var list = _supplierReportBll.SelectStockReprotsGroupByFilialeId(Year, true);
            for (int i = 1; i <= 12; i++)
            {
                var dataList = _reckoning.SelectCurrentMonthStockRecords(Year, i);
                decimal payData = dataList.Sum(act => act.TotalAmount);
                foreach (var supplierPaymentsRecordInfo in dataList)
                {
                    var item = list.FirstOrDefault(act => act.FilialeId == supplierPaymentsRecordInfo.FilialeId)
                        ??new SupplierPaymentsReportInfo{FilialeId = supplierPaymentsRecordInfo.FilialeId};
                    switch (i)
                    {
                        case 1:
                            item.January = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 2:
                            item.February = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 3:
                            item.March = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 4:
                            item.April = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 5:
                            item.May = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 6:
                            item.June = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 7:
                            item.July = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 8:
                            item.August = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 9:
                            item.September = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 10:
                            item.October = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 11:
                            item.November = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                        case 12:
                            item.December = supplierPaymentsRecordInfo.TotalAmount;
                            break;
                    }
                    if (list.All(act=>act.FilialeId!=supplierPaymentsRecordInfo.FilialeId))
                    {
                        list.Add(item);
                    }
                }
                _dicSum.Add(i, payData);
            }
            if (DateTime.Now.Year == Year)
            {
                switch (DateTime.Now.Month)
                {
                    case 1:
                        list = list.OrderByDescending(act => act.January).ToList();
                        break;
                    case 2:
                        list = list.OrderByDescending(act => act.February).ToList();
                        break;
                    case 3:
                        list = list.OrderByDescending(act => act.March).ToList();
                        break;
                    case 4:
                        list = list.OrderByDescending(act => act.April).ToList();
                        break;
                    case 5:
                        list = list.OrderByDescending(act => act.May).ToList();
                        break;
                    case 6:
                        list = list.OrderByDescending(act => act.June).ToList();
                        break;
                    case 7:
                        list = list.OrderByDescending(act => act.July).ToList();
                        break;
                    case 8:
                        list = list.OrderByDescending(act => act.August).ToList();
                        break;
                    case 9:
                        list = list.OrderByDescending(act => act.September).ToList();
                        break;
                    case 10:
                        list = list.OrderByDescending(act => act.October).ToList();
                        break;
                    case 11:
                        list = list.OrderByDescending(act => act.November).ToList();
                        break;
                    case 12:
                        list = list.OrderByDescending(act => act.December).ToList();
                        break;
                }
            }
            else
            {
                list = list.OrderByDescending(act => act.December).ToList();
            }
            ShowFooterText();
            RgPurchaseInStockStatistics.DataSource = list;
        }

        /// <summary>显示合计金额信息
        /// </summary>
        private void ShowFooterText()
        {
            var filialeName = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("FilialeName");
            filialeName.FooterText = "每月合计：";
            filialeName.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            var jan = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("January");
            jan.FooterText = _dicSum.ContainsKey(1) ? _dicSum[1] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[1])) : string.Empty;

            var feb = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("February");
            feb.FooterText = _dicSum.ContainsKey(2) ? _dicSum[2] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[2])) : string.Empty;

            var mar = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("March");
            mar.FooterText = _dicSum.ContainsKey(3) ? _dicSum[3] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[3])) : string.Empty;

            var apr = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("April");
            apr.FooterText = _dicSum.ContainsKey(4) ? _dicSum[4] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[4])) : string.Empty;

            var may = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("May");
            may.FooterText = _dicSum.ContainsKey(5) ? _dicSum[5] == 0 ? "0"  : string.Format("{0}", WebControl.NumberSeparator(_dicSum[5])) : string.Empty;

            var jun = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("June");
            jun.FooterText = _dicSum.ContainsKey(6) ? _dicSum[6] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[6])) : string.Empty;

            var july = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("July");
            july.FooterText = _dicSum.ContainsKey(7) ? _dicSum[7] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[7])) : string.Empty;

            var aug = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("August");
            aug.FooterText = _dicSum.ContainsKey(8) ? _dicSum[8] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[8])) : string.Empty;

            var sept = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("September");
            sept.FooterText = _dicSum.ContainsKey(9) ? _dicSum[9] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[9])) : string.Empty;

            var oct = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("October");
            oct.FooterText = _dicSum.ContainsKey(10) ? _dicSum[10] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[10])) : string.Empty;

            var nov = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("November");
            nov.FooterText = _dicSum.ContainsKey(11) ? _dicSum[11] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[11])) : string.Empty;

            var december = RgPurchaseInStockStatistics.MasterTableView.Columns.FindByUniqueName("December");
            december.FooterText = _dicSum.ContainsKey(12) ? _dicSum[12] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[12])) : string.Empty;
        }

        protected string GetFilialeName(object obj)
        {
            if (obj == null) return string.Empty;
            var filialeId = new Guid(obj.ToString());
            return _dics.ContainsKey(filialeId) ? _dics[filialeId] : string.Empty;
        }

        #endregion

        #region [搜索]

        /// <summary>搜索
        /// </summary>
        protected void RgPurchaseInStockStatisticsOnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            Year = Convert.ToInt32(((RadComboBox)e.Item.FindControl("RCB_Year")).SelectedValue);
            RgPurchaseInStockStatistics.CurrentPageIndex = 0;
            RgPurchaseInStockStatistics.Rebind();
        }

        /// <summary>搜索后绑定当前选择的账期
        /// </summary>
        protected void RgPurchaseInStockStatisticsOnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var rcbYear = e.Item.FindControl("RCB_Year") as RadComboBox;
                if (rcbYear != null)
                {
                    //加载年份
                    var year = DateTime.Now.Year;
                    for (var i = year-1; i <= year + 1; i++)
                    {
                        rcbYear.Items.Add(new RadComboBoxItem(string.Format("{0}", i), string.Format("{0}", i)));
                    }
                    rcbYear.SelectedValue = string.Format("{0}", Year);
                }
            }
        }

        #endregion

        #region [ViewState] (往来单位名称，年份)

        /// <summary>往来单位名称
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


        /// <summary>年份
        /// </summary>
        protected int Year
        {
            get
            {
                return ViewState["Year"] == null ? DateTime.Now.Year : Convert.ToInt32(ViewState["Year"]);
            }
            set
            {
                ViewState["Year"] = value;
            }
        }

        #endregion

        #region

        public string ShowStr(object filialeId, object money, object other)
        {
            if (money == null) return string.Empty;
            if (money.ToString().Length == 0) return string.Empty;
            if (Convert.ToDecimal(money) == 0) return string.Empty;
            return string.Format("{0}[{1}]", WebControl.NumberSeparator(money), WebControl.NumberSeparator(other));
        }
        #endregion
    }
}