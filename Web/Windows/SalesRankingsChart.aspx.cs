using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Charting;

namespace ERP.UI.Web.Windows
{
    public partial class SalesRankingsChart : WindowsPage
    {
        private readonly ISalesGoodsRanking _salesGoodsRankingDao=new SalesGoodsRanking(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (StartTime == DateTime.MinValue || EndTime == DateTime.MinValue)
                {
                    RAM.Alert("操作无效！只能根据一段时间查看！");
                    RAM.ResponseScripts.Add("CancelWindow()");
                    return;
                }
                MemberRadChart.Clear();
                var seriesId = new Guid(Request.QueryString["SeriesId"]);
                var saleFilialeId = new Guid(Request.QueryString["SalefilialeId"]);
                List<SalesGoodsRankingInfo> list;
                if (seriesId == Guid.Empty)
                {
                    string imageSize = GlobalConfig.ImageSize;
                    //ImGoodsId.ImageUrl = System.Configuration.ConfigurationManager.AppSettings["ResourceServerImg"] +
                    //                     "/"+"4d7a5149-f880-4fc7-b601-f7bcd26ed709-90-90.jpg";
                    IGoodsCenterSao goodsCenterSao = new GoodsCenterSao();
                    var path = goodsCenterSao.GetProductFilesImage(Id, true);
                    if (!String.IsNullOrWhiteSpace(path))
                    {
                        const string URLFORMAT = "{0}{1}{2}-{3}-{3}{4}";
                        string id = path.Substring(0, path.IndexOf(".", StringComparison.Ordinal));
                        string file = path.Substring(path.IndexOf(".", StringComparison.Ordinal));
                        ImGoodsId.ImageUrl = string.Format(URLFORMAT, GlobalConfig.ResourceServerImg, "/", id, imageSize, file);
                    }
                    list = _salesGoodsRankingDao.GetSalesRankingChart(Id, WarehouseId, HostingFilialeId, SalefilialeId, SalePlatformId, StartTime, EndTime).Where(ent => ent.SalesNumber > 0 || ent.ZeroNumber > 0).ToList();
                }
                else
                {
                    list = _salesGoodsRankingDao.GetSalesRankingChartBySeriesId(seriesId, WarehouseId, HostingFilialeId, SalefilialeId, SalePlatformId, StartTime, EndTime).Where(ent => ent.SalesNumber > 0 || ent.ZeroNumber > 0).ToList();
                    ImGoodsId.Visible = false;
                }
                if (list.Count == 0)
                {
                    MemberRadChart.ChartTitle.TextBlock.Text = "无发生数据";
                }
                else
                {
                    MemberRadChart.PlotArea.XAxis.AutoScale = false;
                    MemberRadChart.PlotArea.Appearance.Dimensions.Margins = "15%, 6%, 6%, 6%";
                    MemberRadChart.Legend.Appearance.Dimensions.Margins = "2px, 1%, 1px, 1px";
                    //当天非0元销售数量
                    var chartSeriesSalesNumber = new ChartSeries();
                    chartSeriesSalesNumber.Appearance.FillStyle.MainColor = Color.Red;
                    chartSeriesSalesNumber.Type = ChartSeriesType.Line;
                    chartSeriesSalesNumber.Name = "非0销售数量左Y值";

                    //当天0元销售数量
                    var chartSeriesZeroNumber = new ChartSeries();
                    chartSeriesZeroNumber.Appearance.FillStyle.MainColor = Color.Blue;
                    chartSeriesZeroNumber.Type = ChartSeriesType.Line;
                    chartSeriesZeroNumber.Name = "0元销售数量左Y值";

                    //当天销售平均价
                    var chartSeriesGoodsPrice = new ChartSeries();
                    chartSeriesGoodsPrice.Appearance.FillStyle.MainColor = Color.Green;
                    chartSeriesGoodsPrice.Type = ChartSeriesType.Line;
                    chartSeriesGoodsPrice.Name = "平均价格右Y值";

                    //当天是否参与促销
                    var chartSeriesIsPromotion = new ChartSeries();
                    chartSeriesIsPromotion.Appearance.FillStyle.MainColor = Color.Yellow;
                    chartSeriesIsPromotion.Type = ChartSeriesType.Line;
                    chartSeriesIsPromotion.Name = "是否促销";

                    Dictionary<string, string> goodsSalePromotionDic;
                    try
                    {
                        if (Id != Guid.Empty && seriesId==Guid.Empty)
                            goodsSalePromotionDic = (Dictionary<string, string>)new PromotionSao().GetGoodsSalePromotionDict(Id, StartTime, EndTime);
                        else
                        {
                            goodsSalePromotionDic = new Dictionary<string, string>();
                        }
                    }
                    catch (Exception)
                    {
                        goodsSalePromotionDic = new Dictionary<string, string>();
                    }

                    foreach (SalesGoodsRankingInfo t in list)
                    {
                        var dic = goodsSalePromotionDic.FirstOrDefault(ent => ent.Key == t.DayTime.ToString("MM-dd"));
                        if (!string.IsNullOrWhiteSpace(dic.Value))
                        {
                            //促销
                            var isPromotionItem = new ChartSeriesItem(0, dic.Value);
                            chartSeriesIsPromotion.Items.Add(isPromotionItem);

                            //X轴文字
                            MemberRadChart.PlotArea.XAxis.AddItem(t.DayTime.ToString("MM-dd"), Color.Black);
                        }
                        else
                        {
                            //无促销   此处注意,lableText为空(包括"")则会显示value,故写成" "。
                            var isNotPromotionItem = new ChartSeriesItem(0, " ") { Empty = true };
                            chartSeriesIsPromotion.Items.Add(isNotPromotionItem);
                            //X轴文字
                            MemberRadChart.PlotArea.XAxis.AddItem(t.DayTime.ToString("MM-dd"));
                        }

                        //X轴文字旋转
                        MemberRadChart.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 90;

                        //销售数量
                        chartSeriesSalesNumber.Items.Add(new ChartSeriesItem(t.SalesNumber));
                        
                        //0元销量
                        chartSeriesZeroNumber.Items.Add(new ChartSeriesItem(t.ZeroNumber));

                        //平均价格
                        //var avgGoodsPrice = Convert.ToDouble(((double)list[i].GoodsPrice / (list[i].SalesNumber + list[i].ZeroNumber)).ToString("#0.00"));
                        var avgGoodsPrice = Math.Abs(t.SalesNumber) > 0 ? Convert.ToDouble(((double)t.GoodsPrice / t.SalesNumber).ToString("#0.00")) : 0;
                        chartSeriesGoodsPrice.Items.Add(new ChartSeriesItem(avgGoodsPrice));
                        
                    }

                    #region 
                    //MemberRadChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.
                    //MemberRadChart.PlotArea.YAxis2.Clear();
                    //MemberRadChart.PlotArea.YAxis2.MinValue = minPrice * 0.9;
                    //MemberRadChart.PlotArea.YAxis2.MaxValue = maxPrice * 1.1;
                    //MemberRadChart.PlotArea.YAxis2.Step = 20;


                    //MemberRadChart.PlotArea.YAxis2.MaxItemsCount = chartSeriesGoodsPrice.Items.Count;
                    //var ploat = chartSeriesGoodsPrice.PlotArea;
                    //chartSeriesGoodsPrice.PlotArea.YAxis.Step = 0.5;
                    //ChartPlotArea chartPlotArea = new ChartPlotArea();
                    //chartPlotArea.YAxis2.MinValue = minPrice * 0.9;
                    //chartPlotArea.YAxis2.MinValue = maxPrice * 0.9;
                    //chartPlotArea.YAxis2.Step = 0.5;
                    //chartSeriesGoodsPrice.PlotArea.Add(new ChartYAxis(chartPlotArea, ChartYAxisType.Secondary));

                    #endregion

                    #region 设置显示数据的颜色
                    chartSeriesSalesNumber.Items.First().Label.Appearance.FillStyle.MainColor =
                    chartSeriesSalesNumber.Items.First().Label.Appearance.FillStyle.SecondColor = Color.Red;
                    chartSeriesSalesNumber.Items.Last().Label.Appearance.FillStyle.MainColor =
                    chartSeriesSalesNumber.Items.Last().Label.Appearance.FillStyle.SecondColor = Color.Red;
                    
                    chartSeriesZeroNumber.Items.First().Label.Appearance.FillStyle.MainColor =
                    chartSeriesZeroNumber.Items.First().Label.Appearance.FillStyle.SecondColor = Color.Blue;
                    chartSeriesZeroNumber.Items.Last().Label.Appearance.FillStyle.MainColor =
                    chartSeriesZeroNumber.Items.Last().Label.Appearance.FillStyle.SecondColor = Color.Blue;
                    
                    chartSeriesGoodsPrice.Items.First().Label.Appearance.FillStyle.MainColor =
                    chartSeriesGoodsPrice.Items.First().Label.Appearance.FillStyle.SecondColor = Color.Green;
                    chartSeriesGoodsPrice.Items.Last().Label.Appearance.FillStyle.MainColor =
                    chartSeriesGoodsPrice.Items.Last().Label.Appearance.FillStyle.SecondColor = Color.Green;
                    
                    chartSeriesIsPromotion.Items.First().Label.Appearance.FillStyle.MainColor =
                    chartSeriesIsPromotion.Items.First().Label.Appearance.FillStyle.SecondColor = Color.Yellow;
                    chartSeriesIsPromotion.Items.Last().Label.Appearance.FillStyle.MainColor =
                    chartSeriesIsPromotion.Items.Last().Label.Appearance.FillStyle.SecondColor = Color.Yellow;
                    #endregion
                    
                    //添加
                    MemberRadChart.AddChartSeries(chartSeriesSalesNumber);
                    MemberRadChart.AddChartSeries(chartSeriesZeroNumber);
                    MemberRadChart.AddChartSeries(chartSeriesGoodsPrice);
                    MemberRadChart.AddChartSeries(chartSeriesIsPromotion);

                    string titleName=string.Empty;

                    var goodsCenterSao = new GoodsCenterSao();
                    if (seriesId==Guid.Empty)
                    {
                        var goodsInfo = goodsCenterSao.GetGoodsBaseInfoById(Id);
                        if(goodsInfo!=null)
                            titleName = goodsInfo.GoodsName + "(" + goodsInfo.GoodsCode + ")";
                    }
                    else
                    {
                        var seriesNames = goodsCenterSao.GetSeriesDict(saleFilialeId,new List <Guid> { seriesId });
                        titleName = seriesNames.ContainsKey(seriesId)
                            ? seriesNames[seriesId]
                            : string.Empty;
                    }
                    Page.Title = titleName;
                    
                    MemberRadChart.ChartTitle.TextBlock.Text = StartTime.ToShortDateString() + "至" + EndTime.ToShortDateString() + "销量如下:";
                }
            }
        }

        #region [ViewState]
        protected Guid Id
        {
            get
            {
                if (Request.QueryString["Id"] == null || string.IsNullOrEmpty(Request.QueryString["Id"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["Id"].Trim());
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                if (Request.QueryString["WarehouseId"] == null || string.IsNullOrEmpty(Request.QueryString["WarehouseId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["WarehouseId"].Trim());
            }
        }

        protected Guid SalefilialeId
        {
            get
            {
                if (Request.QueryString["SalefilialeId"] == null || string.IsNullOrEmpty(Request.QueryString["SalefilialeId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["SalefilialeId"].Trim());
            }
        }

        protected Guid SalePlatformId
        {
            get
            {
                if (Request.QueryString["SalePlatformId"] == null || string.IsNullOrEmpty(Request.QueryString["SalePlatformId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["SalePlatformId"].Trim());
            }
        }

        protected Guid HostingFilialeId
        {
            get
            {
                if (Request.QueryString["HostingFilialeId"] == null || string.IsNullOrEmpty(Request.QueryString["HostingFilialeId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["HostingFilialeId"].Trim());
            }
        }

        protected DateTime StartTime
        {
            get
            {
                if (Request.QueryString["StartTime"] == null || string.IsNullOrEmpty(Request.QueryString["StartTime"].Trim()))
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(Request.QueryString["StartTime"].Trim());
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (Request.QueryString["EndTime"] == null || string.IsNullOrEmpty(Request.QueryString["EndTime"].Trim()))
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(Request.QueryString["EndTime"].Trim());
            }
        }
        #endregion
    }
}