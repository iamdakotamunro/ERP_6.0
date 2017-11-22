using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Telerik.Charting;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>订单量分析
    /// modify by liangcanren at 2015-04-17  
    /// 添加汇总  去除小黄点
    /// </summary>
    public partial class GoodOrderChart : BasePage
    {
        //readonly GoodsOrder _goodsOrderBll = new GoodsOrder();
        readonly IGoodsOrder _goodsOrder=new GoodsOrder(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSaleFilialeAndSalePlatform();
                int year = NonceYear.Year;
                for (int i = year; i >= year - (year - 2007); i--)
                {
                    RCB_Year.Items.Add(new RadComboBoxItem(string.Format("{0}", i), string.Format("{0}", i)));
                }
                RCB_Year.SelectedValue = string.Format("{0}",year);

                int month = NonceYear.Month;
                RCB_Month.Items.Add(new RadComboBoxItem("", ""));
                for (int j = 1; j < 13; j++)
                {
                    RCB_Month.Items.Add(new RadComboBoxItem(string.Format("{0}", j), string.Format("{0}", j)));
                }
                RCB_Month.SelectedValue = string.Format("{0}",month);
                CreationChart();
            }
        }
        /// <summary>
        /// 年份
        /// </summary>
        protected DateTime NonceYear
        {
            get
            {
                if (ViewState["NonceYear"] == null)
                {
                    ViewState["NonceYear"] = DateTime.Now;
                }
                return DateTime.Parse(ViewState["NonceYear"].ToString());
            }
            set { ViewState["NonceYear"] = value; }
        }

        #region  完成订单、作废订单
        /// <summary>
        /// 完成订单数
        /// </summary>
        protected int CompleteOrders
        {
            get
            {
                if (ViewState["CompleteOrders"] == null)
                {
                    ViewState["CompleteOrders"] = 0;
                }
                return Convert.ToInt32(ViewState["CompleteOrders"]);
            }
            set { ViewState["CompleteOrders"] = value; }
        }

        /// <summary>
        /// 取消订单数
        /// </summary>
        protected int CancelOrders
        {
            get
            {
                if (ViewState["CancelOrders"] == null)
                {
                    ViewState["CancelOrders"] = 0;
                }
                return Convert.ToInt32(ViewState["CancelOrders"].ToString());
            }
            set { ViewState["CancelOrders"] = value; }
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

        protected void IbCreationChartClick(object sender, ImageClickEventArgs e)
        {
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
            NonceYear = String.IsNullOrEmpty(RCB_Month.Text)
                            ? Convert.ToDateTime(RCB_Year.Text + "-1" + "-30")
                            : Convert.ToDateTime(RCB_Year.Text + "-" + RCB_Month.Text + "-1");
            CreationChart();
        }

        private void CreationChart()
        {
            MemberChart.Clear();
            CompleteOrders = 0;
            CancelOrders = 0;
            int[,] mgArray;
            IList<KeyValuePair<int, int>> memberChartList;
            IList<KeyValuePair<int, int>> goodorderchartcancellation;

            if (String.IsNullOrEmpty(RCB_Month.SelectedValue))
            {
                //按年（每月）
                mgArray = new int[12, 2];
                memberChartList = _goodsOrder.GetGoodOrderChart(new[] { OrderState.Consignmented }, NonceYear, GlobalConfig.KeepYear, SaleFilialeID, SalePlatformID);
                goodorderchartcancellation = _goodsOrder.GetGoodOrderChart(new[] { OrderState.Cancellation }, NonceYear, GlobalConfig.KeepYear, SaleFilialeID, SalePlatformID);
                MemberChart.ChartTitle.TextBlock.Text = NonceYear.Year + "年订单增量报告";
            }
            else
            {
                //按月（每天）
                mgArray = new int[DateTime.DaysInMonth(NonceYear.Year, NonceYear.Month), 2];
                memberChartList = _goodsOrder.GetGoodOrderChartDay(new[] { OrderState.Consignmented }, NonceYear, GlobalConfig.KeepYear, SaleFilialeID, SalePlatformID);
                goodorderchartcancellation = _goodsOrder.GetGoodOrderChartDay(new[] { OrderState.Cancellation }, NonceYear, GlobalConfig.KeepYear, SaleFilialeID, SalePlatformID);
                MemberChart.ChartTitle.TextBlock.Text = NonceYear.Year + "年" + NonceYear.Month + "月订单增量报告";
            }
            int completeOrders = 0;  //完成总数
            int cancelOrders = 0;   //取消总数
            //完成数
            foreach (KeyValuePair<int, int> keyValuePair in memberChartList)
            {
                mgArray[keyValuePair.Key - 1, 0] = keyValuePair.Value;
                completeOrders += keyValuePair.Value;
            }
            //作废数
            foreach (KeyValuePair<int, int> keyValuePair in goodorderchartcancellation)
            {
                mgArray[keyValuePair.Key - 1, 1] = keyValuePair.Value;
                cancelOrders += keyValuePair.Value;
            }
            CompleteOrders = completeOrders;
            CancelOrders = cancelOrders;

            MemberChart.DataSource = mgArray;
            MemberChart.DataBind();
        }

        protected void Chart_ItemDataBound(object sender, ChartItemDataBoundEventArgs e)
        {
            switch (e.ChartSeries.Index)
            {
                case 0:
                    e.ChartSeries.Name = string.Format("完成订单({0})",CompleteOrders);
                    break;
                case 1:
                    e.ChartSeries.Name = string.Format("作废订单({0})",CancelOrders);
                    break;
            }
        }

        /// <summary>公司
        /// </summary>
        protected Guid SaleFilialeID
        {
            get
            {
                if (ViewState["SaleFilialeID"] == null) return Guid.Empty;
                return new Guid(ViewState["SaleFilialeID"].ToString());
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
                if (ViewState["SalePlatformID"] == null) return Guid.Empty;
                return new Guid(ViewState["SalePlatformID"].ToString());
            }
            set
            {
                ViewState["SalePlatformID"] = value.ToString();
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
