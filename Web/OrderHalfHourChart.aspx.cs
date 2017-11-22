using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Charting;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class OrderHalfHourChartAw : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               LoadSaleFilialeAndSalePlatform();
                MemberChart.ChartTitle.TextBlock.Text = "暂无数据";
                InitRadChart();
            }
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return new DateTime(1900, 1, 1);
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set { ViewState["StartTime"] = value; }
        }
        protected int[] OrderStates
        {
            get
            {
                if (ViewState["OrderStates"] == null) return new[] { -1 };
                return new[] { Convert.ToInt32(ViewState["OrderStates"]) };
            }
            set { ViewState["OrderStates"] = value; }
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

            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
        }

        protected void IB_CreationChart_Click(object sender, ImageClickEventArgs e)
        {
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
            if (RDP_StartDate.SelectedDate != null)
            {
                StartTime = RDP_StartDate.SelectedDate.Value;
            }

            ViewState["OrderStates"] = Convert.ToInt32(RadComboBoxOrderState.SelectedValue);
            CreationChart();
        }

        private void CreationChart()
        {
            MemberChart.Clear();
            var mgArray = new decimal[48];
            IGoodsOrder order = new GoodsOrder(GlobalConfig.DB.FromType.Read);
            IList<KeyValuePair<int, int>> memberShopingChartList = order.GetOrderHalfHour(StartTime, OrderStates, SaleFilialeID, SalePlatformID);

            if (memberShopingChartList.Count > 0)
            {
                int count = 0;
                for (int i = 0; i < memberShopingChartList.Count; i++)
                {
                    count = count + memberShopingChartList[i].Key;
                }

                foreach (KeyValuePair<int, int> keyValuePair in memberShopingChartList)
                {
                    mgArray[keyValuePair.Value] = WebControl.CurrencyValue(keyValuePair.Key);
                }


                MemberChart.DataSource = mgArray;
                MemberChart.DataBind();

                MemberChart.ChartTitle.TextBlock.Text = StartTime.ToShortDateString() + "发生选择状态单总数为" + count + ",详细如下:";
            }
            else
            {
                MemberChart.ChartTitle.TextBlock.Text = "无发生数据";
            }

        }

        protected void Chart_ItemDataBound(object sender, ChartItemDataBoundEventArgs e)
        {
            e.ChartSeries.Name = "订单数量";
        }
        private void InitRadChart()
        {
            MemberChart.Chart.Series.Clear();
            MemberChart.PlotArea.XAxis.AutoScale = false;
            for (int i = 0; i < 48; i++)
            {
                DateTime dt = StartTime.AddMinutes(30 * i);

                MemberChart.PlotArea.XAxis.AddItem(string.Format("{0}", dt.Hour));
            }
        }
        protected Dictionary<int, string> GetOrderStateList()
        {
            var dcOrderState = (Dictionary<int, string>)EnumAttribute.GetDict<OrderState>();
            return dcOrderState;
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
                ViewState["SaleFilialeID"] = value;
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
                ViewState["SalePlatformID"] = value;
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
