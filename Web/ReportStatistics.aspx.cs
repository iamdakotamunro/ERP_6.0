using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ReportStatistics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Rgss_GoodsNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsInfo> goodsInfoList = new List<GoodsInfo>();
            RGSS.DataSource = goodsInfoList;
        }

        /// <summary>采购单商品详情数据绑定
        /// </summary>
        protected void Rgss_NeedDataSource(object source, GridDetailTableDataBindEventArgs e)
        {
            IList<StockWarningInfo> stockWarningList = null;
            e.DetailTableView.DataSource = stockWarningList;
        }
    }
}