using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class SupplierSaleReport : BasePage
    {
        private static Dictionary<int, decimal> _dicSum = new Dictionary<int, decimal>();

        private static readonly SupplierSaleRecordBll _supplierSaleRecordBll=new SupplierSaleRecordBll();

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

        /// <summary>年份
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        /// <summary>
        /// 绑定数据原
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSupplierSaleOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            _dicSum=new Dictionary<int, decimal>();
            var list = _supplierSaleRecordBll.SelectSupplierSaleReportInfos(Year, CompanyName);
            for (int i = 1; i <= 12; i++)
            {
                decimal data = 0;
                switch (i)
                {
                    case 1:
                        data = list.Sum(act => act.January);
                        break;
                    case 2:
                        data = list.Sum(act => act.February);
                        break;
                    case 3:
                        data = list.Sum(act => act.March);
                        break;
                    case 4:
                        data = list.Sum(act => act.April);
                        break;
                    case 5:
                        data = list.Sum(act => act.May);
                        break;
                    case 6:
                        data = list.Sum(act => act.June);
                        break;
                    case 7:
                        data = list.Sum(act => act.July);
                        break;
                    case 8:
                        data = list.Sum(act => act.August);
                        break;
                    case 9:
                        data = list.Sum(act => act.September);
                        break;
                    case 10:
                        data = list.Sum(act => act.October);
                        break;
                    case 11:
                        data = list.Sum(act => act.November);
                        break;
                    case 12:
                        data = list.Sum(act => act.December);
                        break;
                }
                _dicSum.Add(i,data);
            }
            list = list.OrderByDescending(act => act.January + act.February + act.March + act.April + act.May +
                 act.June +
                  act.July + act.August + act.September + act.October + act.November + act.December).ToList();
            ShowFooterText();
            RgSupplierSale.DataSource = list;
        }

        /// <summary>显示合计金额信息
        /// </summary>
        private void ShowFooterText()
        {
            var name = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("CompanyName");
            name.FooterText = "每月合计：";
            name.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            var jan = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("January");
            jan.FooterText = _dicSum.ContainsKey(1) ? _dicSum[1] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[1])) : string.Empty;

            var feb = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("February");
            feb.FooterText = _dicSum.ContainsKey(2) ? _dicSum[2] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[2])) : string.Empty;

            var mar = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("March");
            mar.FooterText = _dicSum.ContainsKey(3) ? _dicSum[3] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[3])) : string.Empty;

            var apr = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("April");
            apr.FooterText = _dicSum.ContainsKey(4) ? _dicSum[4] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[4])) : string.Empty;

            var may = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("May");
            may.FooterText = _dicSum.ContainsKey(5) ? _dicSum[5] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[5])) : string.Empty;

            var jun = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("June");
            jun.FooterText = _dicSum.ContainsKey(6) ? _dicSum[6] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[6])) : string.Empty;

            var july = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("July");
            july.FooterText = _dicSum.ContainsKey(7) ? _dicSum[7] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[7])) : string.Empty;

            var aug = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("August");
            aug.FooterText = _dicSum.ContainsKey(8) ? _dicSum[8] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[8])) : string.Empty;

            var sept = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("September");
            sept.FooterText = _dicSum.ContainsKey(9) ? _dicSum[9] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[9])) : string.Empty;

            var oct = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("October");
            oct.FooterText = _dicSum.ContainsKey(10) ? _dicSum[10] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[10])) : string.Empty;

            var nov = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("November");
            nov.FooterText = _dicSum.ContainsKey(11) ? _dicSum[11] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[11])) : string.Empty;

            var december = RgSupplierSale.MasterTableView.Columns.FindByUniqueName("December");
            december.FooterText = _dicSum.ContainsKey(12) ? _dicSum[12] == 0 ? "0" : string.Format("{0}", WebControl.NumberSeparator(_dicSum[12])) : string.Empty;
        }

        protected void RgSupplierSaleOnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            var year = (RadComboBox) e.Item.FindControl("RcbYear");
            Year = year != null?Convert.ToInt32(year.SelectedValue):DateTime.Now.Year;
            var txt = (RadTextBox) e.Item.FindControl("RtbCompanyName");
            CompanyName = txt!=null?txt.Text:string.Empty;
            RgSupplierSale.CurrentPageIndex = 0;
            RgSupplierSale.Rebind();
        }

        /// <summary>
        /// 加载年份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgSupplierSaleOnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var rcbYear = e.Item.FindControl("RcbYear") as RadComboBox;
                if (rcbYear != null)
                {
                    //加载年份
                    var year = DateTime.Now.Year;
                    for (var i = year-5; i <= year + 10; i++)
                    {
                        rcbYear.Items.Add(new RadComboBoxItem(string.Format("{0}", i), string.Format("{0}", i)));
                    }
                    rcbYear.SelectedValue = string.Format("{0}", Year);
                }
            }
        }
    }
}