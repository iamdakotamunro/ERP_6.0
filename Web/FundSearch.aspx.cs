using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class FundSearch : BasePage
    {
        //其他公司
        readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        //公司列表
        private static readonly IList<FilialeInfo> _filiales = CacheCollection.Filiale.GetHeadList();

        private static readonly IWasteBook _wasteBookDao = new WasteBook(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var item = _filiales.Where(ent => ent.ID == _reckoningElseFilialeid).ToList();
                if (item.Count == 0)
                {
                    _filiales.Add(new FilialeInfo
                    {
                        ID = _reckoningElseFilialeid,
                        Name = "ERP"
                    });
                }
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

        protected void RGFundsPaymentDays_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var listResult = (from filialeInfo in _filiales
                              let list = _wasteBookDao.GetFundPaymentDaysInfos(GlobalConfig.KeepYear, Year, filialeInfo.ID, string.Empty)
                              where list != null && list.Count > 0
                              select new FundPaymentDaysInfo
                              {
                                  SaleFilialeId = filialeInfo.ID,
                                  SaleFilialeName = filialeInfo.Name,
                                  Jan = list.Sum(s => s.Jan),
                                  Feb = list.Sum(s => s.Feb),
                                  Mar = list.Sum(s => s.Mar),
                                  Apr = list.Sum(s => s.Apr),
                                  May = list.Sum(s => s.May),
                                  Jun = list.Sum(s => s.Jun),
                                  July = list.Sum(s => s.July),
                                  Aug = list.Sum(s => s.Aug),
                                  Sept = list.Sum(s => s.Sept),
                                  Oct = list.Sum(s => s.Oct),
                                  Nov = list.Sum(s => s.Nov),
                                  December = list.Sum(s => s.December),
                                  MaxJan = list.Sum(s => s.MaxJan),
                                  MaxFeb = list.Sum(s => s.MaxFeb),
                                  MaxMar = list.Sum(s => s.MaxMar),
                                  MaxApr = list.Sum(s => s.MaxApr),
                                  MaxMay = list.Sum(s => s.MaxMay),
                                  MaxJun = list.Sum(s => s.MaxJun),
                                  MaxJuly = list.Sum(s => s.MaxJuly),
                                  MaxAug = list.Sum(s => s.MaxAug),
                                  MaxSept = list.Sum(s => s.MaxSept),
                                  MaxOct = list.Sum(s => s.MaxOct),
                                  MaxNov = list.Sum(s => s.MaxNov),
                                  MaxDecember = list.Sum(s => s.MaxDecember),
                                  Year = Year
                              }).ToList();
            //list =
            //    list.Where(
            //        w =>
            //            w.Jan != 0 || w.Feb != 0 || w.Mar != 0 || w.Apr != 0 || w.May != 0 || w.Jun != 0 || w.July != 0 ||
            //            w.Aug != 0 || w.Sept != 0 || w.Oct != 0 || w.Nov != 0 || w.December != 0).ToList();
            ShowFooterTotalText(listResult);
            var month = DateTime.Now.Month;
            switch (month)
            {
                case 1:
                    listResult = listResult.OrderByDescending(o => o.Jan).ToList();
                    break;
                case 2:
                    listResult = listResult.OrderByDescending(o => o.Feb).ToList();
                    break;
                case 3:
                    listResult = listResult.OrderByDescending(o => o.Mar).ToList();
                    break;
                case 4:
                    listResult = listResult.OrderByDescending(o => o.Apr).ToList();
                    break;
                case 5:
                    listResult = listResult.OrderByDescending(o => o.May).ToList();
                    break;
                case 6:
                    listResult = listResult.OrderByDescending(o => o.Jun).ToList();
                    break;
                case 7:
                    listResult = listResult.OrderByDescending(o => o.July).ToList();
                    break;
                case 8:
                    listResult = listResult.OrderByDescending(o => o.Aug).ToList();
                    break;
                case 9:
                    listResult = listResult.OrderByDescending(o => o.Sept).ToList();
                    break;
                case 10:
                    listResult = listResult.OrderByDescending(o => o.Oct).ToList();
                    break;
                case 11:
                    listResult = listResult.OrderByDescending(o => o.Nov).ToList();
                    break;
                case 12:
                    listResult = listResult.OrderByDescending(o => o.December).ToList();
                    break;
            }
            RGFundsPaymentDays.DataSource = listResult;
        }
        /// <summary>显示合计金额信息
        /// </summary>
        /// <param name="list"></param>
        private void ShowFooterTotalText(ICollection<FundPaymentDaysInfo> list)
        {
            var companyName = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("SaleFilialeName");
            companyName.FooterText = list.Count <= 0 ? string.Empty : "合计：";
            companyName.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
            var jan = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Jan");
            var janSum = list.Sum(ent => ent.Jan);
            var janSumStr = list.Sum(ent => ent.MaxJan);
            jan.FooterText = (janSum == 0 ? string.Empty : janSum.ToString("N") + "<br/>") + (janSumStr == 0 ? string.Empty : "[" + janSumStr.ToString("N") + "]");
            jan.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var feb = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Feb");
            var febSum = list.Sum(ent => ent.Feb);
            var febSumStr = list.Sum(ent => ent.MaxFeb);
            feb.FooterText = (febSum == 0 ? string.Empty : febSum.ToString("N") + "<br/>") + (febSumStr == 0 ? string.Empty : "[" + febSumStr.ToString("N") + "]");
            feb.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var mar = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Mar");
            var marSum = list.Sum(ent => ent.Mar);
            var marSumStr = list.Sum(ent => ent.MaxMar);
            mar.FooterText = (marSum == 0 ? string.Empty : marSum.ToString("N") + "<br/>") + (marSumStr == 0 ? string.Empty : "[" + marSumStr.ToString("N") + "]");
            mar.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var apr = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Apr");
            var aprSum = list.Sum(ent => ent.Apr);
            var aprSumStr = list.Sum(ent => ent.MaxApr);
            apr.FooterText = (aprSum == 0 ? string.Empty : aprSum.ToString("N") + "<br/>") + (aprSumStr == 0 ? string.Empty : "[" + aprSumStr.ToString("N") + "]");
            apr.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var may = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("May");
            var maySum = list.Sum(ent => ent.May);
            var maySumStr = list.Sum(ent => ent.MaxMay);
            may.FooterText = (maySum == 0 ? string.Empty : maySum.ToString("N") + "<br/>") + (maySumStr == 0 ? string.Empty : "[" + maySumStr.ToString("N") + "]");
            may.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var jun = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Jun");
            var junSum = list.Sum(ent => ent.Jun);
            var junSumStr = list.Sum(ent => ent.MaxJun);
            jun.FooterText = (junSum == 0 ? string.Empty : junSum.ToString("N") + "<br/>") + (junSumStr == 0 ? string.Empty : "[" + junSumStr.ToString("N") + "]");
            jun.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var july = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("July");
            var julySum = list.Sum(ent => ent.July);
            var julySumStr = list.Sum(ent => ent.MaxJuly);
            july.FooterText = (julySum == 0 ? string.Empty : julySum.ToString("N") + "<br/>") + (julySumStr == 0 ? string.Empty : "[" + julySumStr.ToString("N") + "]");
            july.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var aug = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Aug");
            var augSum = list.Sum(ent => ent.Aug);
            var augSumStr = list.Sum(ent => ent.MaxAug);
            aug.FooterText = (augSum == 0 ? string.Empty : augSum.ToString("N") + "<br/>") + (augSumStr == 0 ? string.Empty : "[" + augSumStr.ToString("N") + "]");
            aug.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var sept = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Sept");
            var septSum = list.Sum(ent => ent.Sept);
            var septSumStr = list.Sum(ent => ent.MaxSept);
            sept.FooterText = (septSum == 0 ? string.Empty : septSum.ToString("N") + "<br/>") + (septSumStr == 0 ? string.Empty : "[" + septSumStr.ToString("N") + "]");
            sept.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var oct = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Oct");
            var octSum = list.Sum(ent => ent.Oct);
            var octSumStr = list.Sum(ent => ent.MaxOct);
            oct.FooterText = (octSum == 0 ? string.Empty : octSum.ToString("N") + "<br/>") + (octSumStr == 0 ? string.Empty : "[" + octSumStr.ToString("N") + "]");
            oct.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var nov = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("Nov");
            var novSum = list.Sum(ent => ent.Nov);
            var novSumStr = list.Sum(ent => ent.MaxNov);
            nov.FooterText = (novSum == 0 ? string.Empty : novSum.ToString("N") + "<br/>") + (novSumStr == 0 ? string.Empty : "[" + novSumStr.ToString("N") + "]");
            nov.FooterStyle.HorizontalAlign = HorizontalAlign.Center;

            var december = RGFundsPaymentDays.MasterTableView.Columns.FindByUniqueName("December");
            var decemberSum = list.Sum(ent => ent.December);
            var decemberSumStr = list.Sum(ent => ent.MaxDecember);
            december.FooterText = (decemberSum == 0 ? string.Empty : decemberSum.ToString("N") + "<br/>") + (decemberSumStr == 0 ? string.Empty : "[" + decemberSumStr.ToString("N") + "]");
            december.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
        }
        /// <summary>
        /// 绑定年份列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGFundsPaymentDays_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var rcbYear = e.Item.FindControl("RCB_Year") as RadComboBox;
                if (rcbYear != null)
                {
                    //加载年份
                    int endYear = DateTime.Now.Year;
                    for (int i = 0; i < GlobalConfig.KeepYear; i++)
                    {
                        var yearStr = (endYear - i).ToString(CultureInfo.InvariantCulture);
                        rcbYear.Items.Add(new RadComboBoxItem(string.Format("{0}", yearStr), string.Format("{0}", yearStr)));
                    }

                    rcbYear.SelectedValue = string.Format("{0}", Year);
                }
            }
        }

        protected void RGFundsPaymentDays_OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            Year = Convert.ToInt32(((RadComboBox)e.Item.FindControl("RCB_Year")).SelectedValue);
            RGFundsPaymentDays.Rebind();
        }
    }
}