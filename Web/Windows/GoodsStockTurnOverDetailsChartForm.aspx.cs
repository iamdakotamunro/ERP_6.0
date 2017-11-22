using System;
using System.Collections.Generic;
using System.Drawing;
using ERP.BLL.Implement.Inventory;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.UI.Web.Base;
using Telerik.Charting;

namespace ERP.UI.Web.Windows
{
    /// <summary>商品详细库存周转情况   2015-04-30  陈重文
    /// </summary>
    public partial class GoodsStockTurnOverDetailsChartForm : WindowsPage
    {

        #region [页面加载Page_Load，生成报表图]

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var goodsIdStr = Request.QueryString["GoodsId"];
                if (!string.IsNullOrWhiteSpace(goodsIdStr))
                {
                    var goodsId = new Guid(goodsIdStr);
                    var warehouseId = GlobalConfig.MainWarehouseID;
                    GoodsStockTurnOverDetailsChart.Clear();
                    var startTime = Convert.ToDateTime(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd 00:00:00"));
                    var endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                    IList<StockTurnOverInfo> stockTurnOverList = GoodsStockPile.ReadInstance.GetGoodsStockTurnOverByGoodsId(startTime, endTime, goodsId, warehouseId);
                    if (stockTurnOverList.Count == 0)
                    {
                        GoodsStockTurnOverDetailsChart.ChartTitle.TextBlock.Text = "无数据发生";
                    }
                    else
                    {
                        GoodsStockTurnOverDetailsChart.PlotArea.XAxis.AutoScale = false;
                        //库存周转率
                        ChartSeries chartSeriesTurnOver = new ChartSeries();
                        chartSeriesTurnOver.Appearance.FillStyle.MainColor = Color.ForestGreen;
                        chartSeriesTurnOver.Type = ChartSeriesType.Line;
                        chartSeriesTurnOver.Name = "库存周转率";
                        //库存数量
                        ChartSeries chartSeriesStockNums = new ChartSeries();
                        chartSeriesStockNums.Appearance.FillStyle.MainColor = Color.Firebrick;
                        chartSeriesStockNums.Type = ChartSeriesType.Line;
                        chartSeriesStockNums.Name = "库存数量";
                        //销售数量
                        ChartSeries chartSeriesSaleNums = new ChartSeries();
                        chartSeriesSaleNums.Appearance.FillStyle.MainColor = Color.CornflowerBlue;
                        chartSeriesSaleNums.Type = ChartSeriesType.Line;
                        chartSeriesSaleNums.Name = "销售数量";
                        for (int i = 0; i < stockTurnOverList.Count; i++)
                        {
                            GoodsStockTurnOverDetailsChart.PlotArea.XAxis.AddItem(string.Format("{0}", (i + 1)));
                            //库存周转率
                            var turnOver = Convert.ToInt32(stockTurnOverList[i].AvgTurnOver);
                            var turnOverItem = new ChartSeriesItem(turnOver);
                            chartSeriesTurnOver.Items.Add(turnOverItem);
                            //库存数量
                            var stockNums = Convert.ToInt32(stockTurnOverList[i].StockNums);
                            var stockNumsItem = new ChartSeriesItem(stockNums);
                            chartSeriesStockNums.Items.Add(stockNumsItem);
                            //销售数量
                            var saleNums = Convert.ToInt32(stockTurnOverList[i].SaleNums);
                            var saleNumsItem = new ChartSeriesItem(saleNums);
                            chartSeriesSaleNums.Items.Add(saleNumsItem);
                        }
                        //添加
                        GoodsStockTurnOverDetailsChart.AddChartSeries(chartSeriesTurnOver);
                        GoodsStockTurnOverDetailsChart.AddChartSeries(chartSeriesStockNums);
                        GoodsStockTurnOverDetailsChart.AddChartSeries(chartSeriesSaleNums);
                        var goodsInfo = new GoodsCenterSao().GetGoodsBaseInfoById(goodsId);
                        var titleName = "当前商品库存周转曲线图";
                        if (goodsInfo != null)
                        {
                            titleName = goodsInfo.GoodsName;
                        }
                        GoodsStockTurnOverDetailsChart.ChartTitle.TextBlock.Text = titleName;
                    }
                }
            }
        }

        #endregion 
    }
}