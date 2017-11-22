using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.Model;
using ERP.UI.Web.Base;
using Framework.Common;
using Telerik.Web.UI;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class FundDetailForm : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var year = Request.QueryString["Year"];
                IsNowYear = DateTime.Now.Year == Convert.ToInt32(year);
                var filialeId = Request.QueryString["FilialeId"];
                if (string.IsNullOrWhiteSpace(filialeId))
                {
                    return;
                }
                Page.Title = "资金详情";
            }
        }
        /// <summary>年份
        /// </summary>
        protected bool IsNowYear
        {
            get
            {
                return ViewState["IsNowYear"] != null && Convert.ToBoolean(ViewState["IsNowYear"]);
            }
            set
            {
                ViewState["IsNowYear"] = value;
            }
        }
        /// <summary>
        /// 绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGFundDetails_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = new List<FundPaymentDaysInfo>();
            var filialeId = Request.QueryString["FilialeId"];
            var year = Request.QueryString["Year"];
            if (!string.IsNullOrWhiteSpace(filialeId))
            {
                if (string.IsNullOrWhiteSpace(year))
                    year = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                list = WasteBookManager.ReadInstance.GetFundPaymentDaysInfos(GlobalConfig.KeepYear, Convert.ToInt32(year),new Guid(filialeId),  BankName).ToList();
            }
            list =
                list.Where(
                    w =>
                        w.Jan != 0 || w.Feb != 0 || w.Mar != 0 || w.Apr != 0 || w.May != 0 || w.Jun != 0 || w.July != 0 ||
                        w.Aug != 0 || w.Sept != 0 || w.Oct != 0 || w.Nov != 0 || w.December != 0 || w.MaxJan != 0 ||
                        w.MaxFeb != 0 || w.MaxMar != 0 || w.MaxApr != 0 || w.MaxMay != 0 || w.MaxJun != 0 || w.MaxJuly != 0 ||
                        w.MaxAug != 0 || w.MaxSept != 0 || w.MaxOct != 0 || w.MaxNov != 0 || w.MaxDecember != 0).ToList();
            ShowBankTotalFootText(list);
            var month = DateTime.Now.Month;
            switch (month)
            {
                case 1:
                    list = list.OrderByDescending(o => o.Jan).ToList();
                    break;
                case 2:
                    list = list.OrderByDescending(o => o.Feb).ToList();
                    break;
                case 3:
                    list = list.OrderByDescending(o => o.Mar).ToList();
                    break;
                case 4:
                    list = list.OrderByDescending(o => o.Apr).ToList();
                    break;
                case 5:
                    list = list.OrderByDescending(o => o.May).ToList();
                    break;
                case 6:
                    list = list.OrderByDescending(o => o.Jun).ToList();
                    break;
                case 7:
                    list = list.OrderByDescending(o => o.July).ToList();
                    break;
                case 8:
                    list = list.OrderByDescending(o => o.Aug).ToList();
                    break;
                case 9:
                    list = list.OrderByDescending(o => o.Sept).ToList();
                    break;
                case 10:
                    list = list.OrderByDescending(o => o.Oct).ToList();
                    break;
                case 11:
                    list = list.OrderByDescending(o => o.Nov).ToList();
                    break;
                case 12:
                    list = list.OrderByDescending(o => o.December).ToList();
                    break;
            }
            RGFundDetails.DataSource = list;
        }

        /// <summary>
        /// 显示统计信息
        /// </summary>
        /// <param name="list"></param>
        public void ShowBankTotalFootText(List<FundPaymentDaysInfo> list)
        {
            var bankName = RGFundDetails.MasterTableView.Columns.FindByUniqueName("BankName");
            bankName.FooterText = list.Count <= 0?string.Empty:"合计：";
            bankName.FooterStyle.HorizontalAlign=HorizontalAlign.Center;
            var jan = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Jan");
            var janSum = list.Sum(ent => ent.Jan);
            var janSumStr = list.Sum(ent => ent.MaxJan);
            jan.FooterText = (janSum == 0 ? string.Empty : janSum.ToString("N") + "<br/>") + (janSumStr == 0 ? string.Empty : "[" + janSumStr.ToString("N") + "]");
            jan.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var feb = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Feb");
            var febSum = list.Sum(ent => ent.Feb);
            var febSumStr = list.Sum(ent => ent.MaxFeb);
            feb.FooterText = (febSum == 0 ? string.Empty : febSum.ToString("N") + "<br/>") + (febSumStr == 0 ? string.Empty : "[" + febSumStr.ToString("N") + "]");
            feb.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var mar = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Mar");
            var marSum = list.Sum(ent => ent.Mar);
            var marSumStr = list.Sum(ent => ent.MaxMar);
            mar.FooterText = (marSum == 0 ? string.Empty : marSum.ToString("N") + "<br/>") + (marSumStr == 0 ? string.Empty : "[" + marSumStr.ToString("N") + "]");
            mar.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var apr = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Apr");
            var aprSum = list.Sum(ent => ent.Apr);
            var aprSumStr = list.Sum(ent => ent.MaxApr);
            apr.FooterText = (aprSum == 0 ? string.Empty : aprSum.ToString("N") + "<br/>") + (aprSumStr == 0 ? string.Empty : "[" + aprSumStr.ToString("N") + "]");
            apr.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var may = RGFundDetails.MasterTableView.Columns.FindByUniqueName("May");
            var maySum = list.Sum(ent => ent.May);
            var maySumStr = list.Sum(ent => ent.MaxMay);
            may.FooterText = (maySum == 0 ? string.Empty : maySum.ToString("N") + "<br/>") + (maySumStr == 0 ? string.Empty : "[" + maySumStr.ToString("N") + "]");
            may.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var jun = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Jun");
            var junSum = list.Sum(ent => ent.Jun);
            var junSumStr = list.Sum(ent => ent.MaxJun);
            jun.FooterText = (junSum == 0 ? string.Empty : junSum.ToString("N") + "<br/>") + (junSumStr == 0 ? string.Empty : "[" + junSumStr.ToString("N") + "]");
            jun.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var july = RGFundDetails.MasterTableView.Columns.FindByUniqueName("July");
            var julySum = list.Sum(ent => ent.July);
            var julySumStr = list.Sum(ent => ent.MaxJuly);
            july.FooterText = (julySum == 0 ? string.Empty : julySum.ToString("N") + "<br/>") + (julySumStr == 0 ? string.Empty : "[" + julySumStr.ToString("N") + "]");
            july.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var aug = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Aug");
            var augSum = list.Sum(ent => ent.Aug);
            var augSumStr = list.Sum(ent => ent.MaxAug);
            aug.FooterText = (augSum == 0 ? string.Empty : augSum.ToString("N") + "<br/>") + (augSumStr == 0 ? string.Empty : "[" + augSumStr.ToString("N") + "]");
            aug.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var sept = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Sept");
            var septSum = list.Sum(ent => ent.Sept);
            var septSumStr = list.Sum(ent => ent.MaxSept);
            sept.FooterText = (septSum == 0 ? string.Empty : septSum.ToString("N") + "<br/>") + (septSumStr == 0 ? string.Empty : "[" + septSumStr.ToString("N") + "]");
            sept.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var oct = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Oct");
            var octSum = list.Sum(ent => ent.Oct);
            var octSumStr = list.Sum(ent => ent.MaxOct);
            oct.FooterText = (octSum == 0 ? string.Empty : octSum.ToString("N") + "<br/>") + (octSumStr == 0 ? string.Empty : "[" + octSumStr.ToString("N") + "]");
            oct.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var nov = RGFundDetails.MasterTableView.Columns.FindByUniqueName("Nov");
            var novSum = list.Sum(ent => ent.Nov);
            var novSumStr = list.Sum(ent => ent.MaxNov);
            nov.FooterText = (novSum == 0 ? string.Empty : novSum.ToString("N") + "<br/>") + (novSumStr == 0 ? string.Empty : "[" + novSumStr.ToString("N") + "]");
            nov.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var december = RGFundDetails.MasterTableView.Columns.FindByUniqueName("December");
            var decemberSum = list.Sum(ent => ent.December);
            var decemberSumStr = list.Sum(ent => ent.MaxDecember);
            december.FooterText = (decemberSum == 0 ? string.Empty : decemberSum.ToString("N") + "<br/>") + (decemberSumStr == 0 ? string.Empty : "[" + decemberSumStr.ToString("N") + "]");
            december.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGFundDetails_OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            BankName = ((RadTextBox)e.Item.FindControl("RTB_CompanyName")).Text.Trim();
            RGFundDetails.Rebind();
        }

        /// <summary>
        /// 银行名称
        /// </summary>
        protected string BankName
        {
            get
            {
                return ViewState["BankName"] == null ? string.Empty : ViewState["BankName"].ToString();
            }
            set
            {
                ViewState["BankName"] = value;
            }
        }
    }
}