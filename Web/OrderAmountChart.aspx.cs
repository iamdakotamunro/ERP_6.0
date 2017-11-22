using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Telerik.Charting;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>区域订单金额分布
    /// </summary>
    public partial class OrderAmountChartAw : BasePage
    {
        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitRadChart();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSaleFilialeAndSalePlatform();
                LoadCountry();
                LoadProvince(RCB_CountryId.SelectedValue);
                MemberChart.ChartTitle.TextBlock.Text = "暂无数据";

                var yearlist = new List<int>();
                for (var i = 2007; i <= (DateTime.Now.Year - GlobalConfig.KeepYear); i++)
                {
                    yearlist.Add(i);
                }
                DDL_Years.DataSource = yearlist.OrderByDescending(y => y);
                DDL_Years.DataBind();
                DDL_Years.Items.Add(new ListItem(GlobalConfig.KeepYear + "年内数据", "0"));
                DDL_Years.SelectedValue = "0";

                if (DDL_Years.SelectedValue == "0")
                {
                    RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                    RDP_EndTime.SelectedDate = DateTime.Now;
                }
                else
                {
                    RDP_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                    RDP_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                }
            }
        }

        private void LoadSaleFilialeAndSalePlatform()
        {
            RCB_SaleFiliale.DataSource = CacheCollection.Filiale.GetHeadList();
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SaleFiliale.SelectedIndex = 0;

            RCB_SalePlatform.DataSource = CacheCollection.SalePlatform.GetList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;

            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
        }

        private void LoadCountry()
        {
            RCB_CountryId.DataValueField = "CountryId";
            RCB_CountryId.DataTextField = "CountryName";
            RCB_CountryId.SelectedValue = null;
            RCB_CountryId.DataSource = ERP.Cache.Country.Instance.ToList();
            RCB_CountryId.DataBind();
        }

        private void LoadProvince(string countryId)
        {
            if (!string.IsNullOrEmpty(countryId))
            {
                var countryIdGuid = new Guid(countryId);
                RCB_ProvinceId.DataValueField = "ProvinceId";
                RCB_ProvinceId.DataTextField = "ProvinceName";
                RCB_ProvinceId.SelectedValue = null;
                RCB_ProvinceId.DataSource = ERP.Cache.Province.Instance.ToList().Where(e => e.CountryId == countryIdGuid);
                RCB_ProvinceId.DataBind();
                RCB_ProvinceId.Items.Insert(0, new RadComboBoxItem("请选择", ""));
            }
            else
            {
                RCB_ProvinceId.Items.Clear();
            }
        }

        private void LoadCity(string provinceId)
        {
            if (!string.IsNullOrEmpty(provinceId))
            {
                var provinceIdGuid = new Guid(provinceId);
                RCB_CityId.DataValueField = "CityId";
                RCB_CityId.DataTextField = "CityName";
                RCB_CityId.SelectedValue = null;
                RCB_CityId.DataSource = ERP.Cache.City.Instance.ToList().Where(e => e.ProvinceId == provinceIdGuid);
                RCB_CityId.DataBind();
                RCB_CityId.Items.Insert(0, new RadComboBoxItem("请选择", ""));
            }
            else
            {
                RCB_CityId.Items.Clear();
            }
        }

        protected void RcbCountryIdSelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string countryId = RCB_CountryId.SelectedValue;
            LoadProvince(countryId);
        }

        protected void RcbProvinceIdSelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string provinceId = RCB_ProvinceId.SelectedValue;
            LoadCity(provinceId);
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null)
                {
                    ViewState["StartTime"] = new DateTime(1900, 1, 1);
                }
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set { ViewState["StartTime"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        protected DateTime Endtime
        {
            get
            {
                if (ViewState["Endtime"] == null)
                {
                    ViewState["Endtime"] = new DateTime(2099, 1, 1);
                }
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["Endtime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            }
            set { ViewState["Endtime"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        protected Guid ProvinceId
        {
            get
            {
                if (ViewState["ProvinceId"] == null)
                {
                    ViewState["ProvinceId"] = Guid.Empty;
                }
                return new Guid(ViewState["ProvinceId"].ToString());
            }
            set { ViewState["ProvinceId"] = value.ToString(); }
        }

        protected Guid CityId
        {
            get
            {
                if (ViewState["CityId"] == null)
                {
                    ViewState["CityId"] = Guid.Empty;
                }
                return new Guid(ViewState["CityId"].ToString());
            }
            set { ViewState["CityId"] = value.ToString(); }
        }

        /// <summary>公司
        /// </summary>
        protected Guid SaleFilialeId
        {
            get
            {
                return ViewState["SaleFilialeID"] == null ? Guid.Empty : new Guid(ViewState["SaleFilialeID"].ToString());
            }
            set
            {
                ViewState["SaleFilialeID"] = value.ToString();
            }
        }

        /// <summary>销售平台
        /// </summary>
        protected Guid SalePlatformId
        {
            get
            {
                return ViewState["SalePlatformID"] == null ? Guid.Empty : new Guid(ViewState["SalePlatformID"].ToString());
            }
            set
            {
                ViewState["SalePlatformID"] = value.ToString();
            }
        }

        protected int PayMode
        {
            get
            {
                if (ViewState["PayMode"] == null)
                {
                    ViewState["PayMode"] = -1;
                }
                return Convert.ToInt32(ViewState["PayMode"]);
            }
            set
            {
                ViewState["PayMode"] = value;
            }
        }

        protected int[] OrderState
        {
            get
            {
                if (ViewState["OrderState"] == null)
                {
                    ViewState["OrderState"] = new[] { -1 };
                }


                return new[] { Convert.ToInt32(ViewState["OrderState"]) };
            }
            set { ViewState["OrderState"] = value.ToString(); }
        }

        protected int ShowMode
        {
            get
            {
                if (ViewState["ShowMode"] == null)
                {
                    ViewState["ShowMode"] = 0;
                }
                return Convert.ToInt32(ViewState["ShowMode"]);
            }
            set { ViewState["ShowMode"] = value; }
        }

        protected void IbCreationChartClick(object sender, ImageClickEventArgs e)
        {
            if (RDP_StartTime.SelectedDate != null)
            {
                StartTime = RDP_StartTime.SelectedDate.Value;
            }
            if (RDP_EndTime.SelectedDate != null)
            {
                Endtime = RDP_EndTime.SelectedDate.Value;
            }
            SaleFilialeId = RCB_SaleFiliale.SelectedValue == "" ? Guid.Empty : new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = RCB_SalePlatform.SelectedValue == "" ? Guid.Empty : new Guid(RCB_SalePlatform.SelectedValue);
            ProvinceId = RCB_ProvinceId.SelectedValue == "" ? Guid.Empty : new Guid(RCB_ProvinceId.SelectedValue);
            CityId = RCB_CityId.SelectedValue == "" ? Guid.Empty : new Guid(RCB_CityId.SelectedValue);
            ShowMode = CheckBox1.Checked ? 1 : 0;
            PayMode = Convert.ToInt32(RadComboBoxPayMode.SelectedValue);
            ViewState["OrderState"] = Convert.ToInt32(RadComboBoxOrderState.SelectedValue);
            CreationChart();
        }

        private void CreationChart()
        {
            MemberChart.Clear();
            IGoodsOrder goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
            IList<KeyValuePair<int, double>> memberShopingChartList = goodsOrder.GetOrderAmountRecord(ProvinceId, CityId, StartTime, Endtime, PayMode, OrderState, 1, ShowMode, GlobalConfig.KeepYear, SaleFilialeId, SalePlatformId);
            if (memberShopingChartList.Count > 0)
            {
                var count = memberShopingChartList.Aggregate(0, (current, t) => current + t.Key);
                MemberChart.DataSource = memberShopingChartList;
                MemberChart.ChartTitle.TextBlock.Text = StartTime.ToShortDateString() + "到" + Endtime.ToShortDateString() + "订单总数为" + count + ",详细如下:";
                MemberChart.DataBind();
            }
            else
            {
                MemberChart.ChartTitle.TextBlock.Text = "无发生数据";
            }
        }

        private void InitRadChart()
        {
            MemberChart.Chart.Series.Clear();
            MemberChart.PlotArea.XAxis.AutoScale = false;
            MemberChart.PlotArea.XAxis.AddItem("0-50");
            MemberChart.PlotArea.XAxis.AddItem("50+");
            MemberChart.PlotArea.XAxis.AddItem("75+");
            MemberChart.PlotArea.XAxis.AddItem("100+");
            MemberChart.PlotArea.XAxis.AddItem("125+");
            MemberChart.PlotArea.XAxis.AddItem("150+");
            MemberChart.PlotArea.XAxis.AddItem("175+");
            MemberChart.PlotArea.XAxis.AddItem("200+");
            MemberChart.PlotArea.XAxis.AddItem("225+");
            MemberChart.PlotArea.XAxis.AddItem("250+");
            MemberChart.PlotArea.XAxis.AddItem("275+");
            MemberChart.PlotArea.XAxis.AddItem("300+");
            MemberChart.PlotArea.XAxis.AddItem("325+");
            MemberChart.PlotArea.XAxis.AddItem("350+");
            MemberChart.PlotArea.XAxis.AddItem("375+");
            MemberChart.PlotArea.XAxis.AddItem("400+");
            MemberChart.PlotArea.XAxis.AddItem("425+");
            MemberChart.PlotArea.XAxis.AddItem("450+");
            MemberChart.PlotArea.XAxis.AddItem("475+");
            MemberChart.PlotArea.XAxis.AddItem("500+");
        }

        protected void Chart_ItemDataBound(object sender, ChartItemDataBoundEventArgs e)
        {
            if (e.ChartSeries.Index == 0)
                e.ChartSeries.Name = "当前状态下订单数量";
            else if (e.ChartSeries.Index == 1)
                e.ChartSeries.Name = "百分比(%)";
        }

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDL_Years.SelectedValue == "0")
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;

                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now.AddDays(1);

                RDP_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_EndTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_EndTime.MaxDate = DateTime.Now;

                StartTime = RDP_StartTime.SelectedDate.Value;
                Endtime = RDP_EndTime.SelectedDate.Value;
            }
            else
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;
                RDP_StartTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_StartTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                if (RDP_StartTime.SelectedDate != null)
                    StartTime = DateTime.Parse(DDL_Years.SelectedValue + RDP_StartTime.SelectedDate.Value.ToString("-MM-dd"));
                if (RDP_EndTime.SelectedDate != null)
                    Endtime = DateTime.Parse(DDL_Years.SelectedValue + RDP_EndTime.SelectedDate.Value.ToString("-MM-dd"));
            }
        }

        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var radComboBox = o as RadComboBox;
            if (radComboBox == null) return;
            var rcbSaleFiliale = radComboBox.Parent.FindControl("RCB_SaleFiliale") as RadComboBox;
            var rcbSalePlatform = radComboBox.Parent.FindControl("RCB_SalePlatform") as RadComboBox;
            if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
            if (rcbSaleFiliale == null) return;
            var rcbSaleFilialeId = new Guid(rcbSaleFiliale.SelectedValue);
            RCB_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            if (rcbSaleFilialeId != Guid.Empty)
            {
                var shopList = CacheCollection.Filiale.GetChildShopList(rcbSaleFilialeId);
                for (var i = 0; i < shopList.Count; i++)
                {
                    RCB_SalePlatform.Items.Insert(i, new RadComboBoxItem(shopList[i].Name, shopList[i].ID.ToString()));
                }
            }
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
        }

        protected void RCB_SalePlatform_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
        }
    }
}
