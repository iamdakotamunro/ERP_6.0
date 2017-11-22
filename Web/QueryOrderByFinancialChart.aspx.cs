using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Basis;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IBasis;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Telerik.Charting;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>区域订单总额年报
    /// </summary>
    public partial class QueryOrderByFinancialChart : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSaleFilialeAndSalePlatform();
                LoadCountry();
                LoadProvince(RCB_CountryId.SelectedValue);
                MemberChart.ChartTitle.TextBlock.Text = "暂无数据";
                MemberChart.Series.Clear();
            }
        }

        #region
        override protected void OnInit(EventArgs e)
        {
            int year = DateTime.Now.Year;
            for (int i = year; i >= year - (year - 2007); i--)
            {
                DDL_Year.Items.Add(new ListItem(string.Format("{0}",i), string.Format("{0}",i)));
            }
            DDL_Year.SelectedValue = string.Format("{0}",year);

            InitializeComponent();
            base.OnInit(e);

        }

        private void InitializeComponent()
        {
            RCB_CountryId.SelectedIndexChanged += RcbCountryIdSelectedIndexChanged;
            RCB_ProvinceId.SelectedIndexChanged += RcbProvinceIdSelectedIndexChanged;
        }
        #endregion

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

            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
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

        private void RcbCountryIdSelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string countryId = RCB_CountryId.SelectedValue;
            LoadProvince(countryId);
        }

        private void RcbProvinceIdSelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string provinceId = RCB_ProvinceId.SelectedValue;
            LoadCity(provinceId);
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return new DateTime(1900, 1, 1);
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected DateTime Endtime
        {
            get
            {
                if (ViewState["Endtime"] == null) return new DateTime(2099, 1, 1);
                return Convert.ToDateTime(ViewState["Endtime"]).AddDays(1).AddSeconds(-1);
            }
            set
            {
                ViewState["Endtime"] = value;
            }
        }

        protected Guid ProvinceId
        {
            get
            {
                if (ViewState["ProvinceId"] == null) return Guid.Empty;
                return new Guid(ViewState["ProvinceId"].ToString());
            }
            set
            {
                ViewState["ProvinceId"] = value.ToString();
            }
        }

        protected Guid CountryId
        {
            get
            {
                if (ViewState["CountryId"] == null) return Guid.Empty;
                return new Guid(ViewState["CountryId"].ToString());
            }
            set
            {
                ViewState["CountryId"] = value.ToString();
            }
        }

        protected Guid CityId
        {
            get
            {
                if (ViewState["CityId"] == null) return Guid.Empty;
                return new Guid(ViewState["CityId"].ToString());
            }
            set
            {
                ViewState["CityId"] = value.ToString();
            }
        }

        /// <summary>公司
        /// </summary>
        protected Guid SaleFilialeID
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
        protected Guid SalePlatformID
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
                if (ViewState["PayMode"] == null) return -1;
                return Convert.ToInt32(ViewState["PayMode"]);
            }
            set
            {
                ViewState["PayMode"] = value;
            }
        }

        protected int OrderState
        {
            get
            {
                if (ViewState["OrderState"] == null) return -1;
                return Convert.ToInt32(ViewState["OrderState"]);
            }
            set
            {
                ViewState["OrderState"] = value;
            }
        }

        protected int Year
        {
            get
            {
                if (ViewState["Year"] == null) return DateTime.Now.Year;
                return Convert.ToInt32(ViewState["Year"]);
            }
            set
            {
                ViewState["Year"] = value;
            }
        }

        protected void IbCreationChartClick(object sender, ImageClickEventArgs e)
        {
            SaleFilialeID = RCB_SaleFiliale.SelectedValue == "" ? Guid.Empty : new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = RCB_SalePlatform.SelectedValue == "" ? Guid.Empty : new Guid(RCB_SalePlatform.SelectedValue);
            Year = Convert.ToInt32(DDL_Year.SelectedItem.ToString());
            CountryId = RCB_CountryId.SelectedValue == "" ? Guid.Empty : new Guid(RCB_CountryId.SelectedValue);
            ProvinceId = RCB_ProvinceId.SelectedValue == "" ? Guid.Empty : new Guid(RCB_ProvinceId.SelectedValue);
            CityId = RCB_CityId.SelectedValue == "" ? Guid.Empty : new Guid(RCB_CityId.SelectedValue);
            PayMode = Convert.ToInt32(RadComboBoxPayMode.SelectedValue);
            OrderState = Convert.ToInt32(RadComboBoxOrderState.SelectedValue);

            CreationChart();

        }

        private void CreationChart()
        {
            MemberChart.Clear();
            var mgArray = new decimal[12];
            IGoodsOrder order = new GoodsOrder(GlobalConfig.DB.FromType.Read);
            var memberShopingChartList = order.GetQueryOrderByFinancial(Year, (OrderState)OrderState, PayMode, CountryId, ProvinceId, CityId, GlobalConfig.KeepYear, SaleFilialeID, SalePlatformID);

            if (memberShopingChartList.Count > 0)
            {
                decimal count = memberShopingChartList.Aggregate<KeyValuePair<int, decimal>, decimal>(0, (current, t) => current + t.Value);

                foreach (var keyValuePair in memberShopingChartList)
                {
                    mgArray[keyValuePair.Key - 1] = WebControl.CurrencyValue(keyValuePair.Value);
                }
                MemberChart.DataSource = mgArray;
                MemberChart.DataBind();
                MemberChart.ChartTitle.TextBlock.Text = "当前条件订单总金额" + WebControl.CurrencyValue(count) + ",详细如下:";
                //ChartSeries memberChart = MemberChart.Series.GetSeries(0);
                //ChartSeries nullChare = MemberChart.Series.GetSeries(1);
                //memberChart.Name = "订单发生额";
                //nullChare.Name = "";
            }
            else
            {
                MemberChart.ChartTitle.TextBlock.Text = "无发生数据";
            }

        }

        protected void Chart_ItemDataBound(object sender, ChartItemDataBoundEventArgs e)
        {
            e.ChartSeries.Name = "订单发生额";
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
            RCB_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetList().Where(item => item.FilialeId == rcbSaleFilialeId).ToList();
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
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
        }

        protected void RCB_SalePlatform_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
        }
    }
}
