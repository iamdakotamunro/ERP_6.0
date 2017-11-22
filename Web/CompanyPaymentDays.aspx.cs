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
    /// <summary>往来单位应付款查询  ADD  2015-02-03  陈重文 
    /// </summary>
    public partial class CompanyPaymentDays : BasePage
    {
        //其他公司
        private readonly static Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private static readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Read);
        //公司列表
        private static readonly IList<FilialeInfo> _filialeList = CacheCollection.Filiale.GetHeadList();
        private static Dictionary<Guid, string> _dics = new Dictionary<Guid, string>();

        /// <summary>
        /// 月应付款金额总汇
        /// </summary>
        private static Dictionary<int, decimal> _dicSum = new Dictionary<int, decimal>();

        /// <summary>
        /// 月未付款金额总汇
        /// </summary>
        private static Dictionary<int, decimal> _dicPaySum = new Dictionary<int, decimal>();

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

        public bool Goon
        {
            get
            {
                if (ViewState["Goon"] == null) return true;
                return (bool)ViewState["Goon"];
            }
            set { ViewState["Goon"] = value; }
        }

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        protected void RgCompanyPaymentDaysOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            _dicSum = new Dictionary<int, decimal>();
            _dicPaySum = new Dictionary<int, decimal>();

            var list=new List<SupplierPaymentsReportInfo>();
                //_supplierReportBll.SelectPaymentsReprotsGroupByFilialeId(Year, true);
            for (int i = 1; i <= 12; i++)
            {
                var monthList = _reckoning.SelectCurrentMonthPaymentsRecords(Year, i);
                foreach (var supplierPaymentsRecordInfo in monthList)
                {
                    var item = list.FirstOrDefault(act => act.FilialeId == supplierPaymentsRecordInfo.FilialeId)??
                        new SupplierPaymentsReportInfo{FilialeId = supplierPaymentsRecordInfo.FilialeId};
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
                    if (list.All(act => act.FilialeId != supplierPaymentsRecordInfo.FilialeId))
                    {
                        list.Add(item);
                    }
                }
                decimal payData = monthList.Sum(act => act.TotalAmount);
                decimal noPayData = monthList.Sum(act => act.TotalNoPayed);
                if (!_dicSum.ContainsKey(i))
                {
                    _dicSum.Add(i, payData==0?0:-payData);
                }
                if (!_dicPaySum.ContainsKey(i))
                {
                    _dicPaySum.Add(i, noPayData == 0 ? 0 : -noPayData);
                }
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
            RGCompanyPaymentDays.DataSource = list;
        }

        /// <summary>显示合计金额信息
        /// </summary>
        private void ShowFooterText()
        {
            var filialeName = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("FilialeName");
            filialeName.FooterText = "每月合计：";
            filialeName.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            var jan = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("January");
            jan.FooterText = _dicSum.ContainsKey(1) ? _dicSum[1] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[1]),
                WebControl.NumberSeparator(_dicPaySum[1])) : string.Empty;

            var feb = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("February");
            feb.FooterText = _dicSum.ContainsKey(2) ? _dicSum[2] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[2]),
                WebControl.NumberSeparator(_dicPaySum[2])) : string.Empty;

            var mar = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("March");
            mar.FooterText = _dicSum.ContainsKey(3) ? _dicSum[3] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[3]),
                WebControl.NumberSeparator(_dicPaySum[3])) : string.Empty;

            var apr = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("April");
            apr.FooterText = _dicSum.ContainsKey(4) ? _dicSum[4] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[4]),
                WebControl.NumberSeparator(_dicPaySum[4])) : string.Empty;

            var may = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("May");
            may.FooterText = _dicSum.ContainsKey(5) ? _dicSum[5] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[5]),
                WebControl.NumberSeparator(_dicPaySum[5])) : string.Empty;

            var jun = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("June");
            jun.FooterText = _dicSum.ContainsKey(6) ? _dicSum[6] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[6]),
                WebControl.NumberSeparator(_dicPaySum[6])) : string.Empty;

            var july = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("July");
            july.FooterText = _dicSum.ContainsKey(7) ? _dicSum[7] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[7]),
                WebControl.NumberSeparator(_dicPaySum[7])) : string.Empty;

            var aug = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("August");
            aug.FooterText = _dicSum.ContainsKey(8) ? _dicSum[8] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[8]),
                WebControl.NumberSeparator(_dicPaySum[8])) : string.Empty;

            var sept = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("September");
            sept.FooterText = _dicSum.ContainsKey(9) ? _dicSum[9] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[9]),
                WebControl.NumberSeparator(_dicPaySum[9])) : string.Empty;

            var oct = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("October");
            oct.FooterText = _dicSum.ContainsKey(10) ? _dicSum[10] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[10]),
                WebControl.NumberSeparator(_dicPaySum[10])) : string.Empty;

            var nov = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("November");
            nov.FooterText = _dicSum.ContainsKey(11) ? _dicSum[11] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[11]),
                WebControl.NumberSeparator(_dicPaySum[11])) : string.Empty;

            var december = RGCompanyPaymentDays.MasterTableView.Columns.FindByUniqueName("December");
            december.FooterText = _dicSum.ContainsKey(12) ? _dicSum[12] == 0 ? "0" : string.Format("{0}<br/>[{1}]", WebControl.NumberSeparator(_dicSum[12]),
                WebControl.NumberSeparator(_dicPaySum[12])) : string.Empty;
        }

        #endregion

        #region

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
        protected void RGCompanyPaymentDays_OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            Year = Convert.ToInt32(((RadComboBox)e.Item.FindControl("RCB_Year")).SelectedValue);
            RGCompanyPaymentDays.CurrentPageIndex = 0;
            RGCompanyPaymentDays.Rebind();
        }

        /// <summary>搜索后绑定当前选择的账期
        /// </summary>
        protected void RGCompanyPaymentDays_OnItemDataBound(object sender, GridItemEventArgs e)
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

    [Serializable]
    public class TempCompanyPaymentDaysInfo
    {
        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public String FilialeName { get; set; }

        /// <summary>一月
        /// </summary>
        public string Jan { get; set; }

        /// <summary>二月
        /// </summary>
        public string Feb { get; set; }

        /// <summary>三月
        /// </summary>
        public string Mar { get; set; }

        /// <summary>四月
        /// </summary>
        public string Apr { get; set; }

        /// <summary>五月
        /// </summary>
        public string May { get; set; }

        /// <summary>六月
        /// </summary>
        public string Jun { get; set; }

        /// <summary>七月
        /// </summary>
        public string July { get; set; }

        /// <summary>八月
        /// </summary>
        public string Aug { get; set; }

        /// <summary>九月
        /// </summary>
        public string Sept { get; set; }

        /// <summary>十月
        /// </summary>
        public string Oct { get; set; }

        /// <summary>十一月
        /// </summary>
        public string Nov { get; set; }

        /// <summary>十二月
        /// </summary>
        public string December { get; set; }
    }
}