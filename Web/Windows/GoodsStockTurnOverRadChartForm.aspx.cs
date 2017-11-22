using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Framework.Common;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using Telerik.Charting;

namespace ERP.UI.Web.Windows
{
    /// <summary>商品库存周转信息  2015-04-23  陈重文
    /// </summary>
    public partial class GoodsStockTurnOverRadChartForm : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsManager=new GoodsCenterSao();
        
        #region [页面加载Page_Load]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var goodsClassId = Request.QueryString["GoodsClassId"];
                var goodsNameOrCode = Request.QueryString["GoodsNameOrCode"];
                var state = Request.QueryString["State"];
                var personnelId = Request.QueryString["PersonnelId"];
                var companyId = Request.QueryString["CompanyId"];
                if (!string.IsNullOrWhiteSpace(goodsClassId))
                    GoodsClassId = new Guid(goodsClassId);
                if (!string.IsNullOrWhiteSpace(goodsNameOrCode))
                    GoodsNameOrCode = goodsNameOrCode;
                if (!string.IsNullOrWhiteSpace(state))
                    State = Convert.ToInt32(state);
                if (!string.IsNullOrWhiteSpace(personnelId))
                    PersonnelId = new Guid(personnelId);
                if (!string.IsNullOrWhiteSpace(companyId))
                    CompanyId = new Guid(companyId);
                WarehouseId = GlobalConfig.MainWarehouseID;
                BindData();
            }
        }
        #endregion

        /// <summary>生成报表图
        /// </summary>
        private void BindData()
        {
            GoodsStockTurnOverRadChart.Clear();
            GoodsStockTurnOverRadChart.PlotArea.XAxis.Clear();
            var startTime = Convert.ToDateTime(DateTime.Now.AddDays(-46).ToString("yyyy-MM-dd 00:00:00"));
            var endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            var goodsIdList = new List<Guid>();
            IList<GoodsPerformance> goodsList = new List<GoodsPerformance>();
            if (PersonnelId != Guid.Empty || CompanyId != Guid.Empty)
            {
                IPurchaseSet purchaseSet=new PurchaseSet(GlobalConfig.DB.FromType.Read);
                var tempGoodsIds = purchaseSet.GetGoodsIdByPersonnelId(PersonnelId, CompanyId);
                if (tempGoodsIds.Count > 0)
                {
                    var tempList = _goodsManager.GetGoodsListByGoodsIds(tempGoodsIds.ToList()).Where(ent => ent.ExpandInfo.IsStatisticalPerformance).ToList();
                    if (!string.IsNullOrWhiteSpace(GoodsNameOrCode))
                    {
                        tempList = tempList.Where(ent => ent.GoodsName.Contains(GoodsNameOrCode) || ent.GoodsCode.Contains(GoodsNameOrCode)).ToList();
                    }
                    if (tempList.Count > 0)
                        goodsIdList.AddRange(tempList.Select(ent => ent.GoodsId));
                }
            }
            if (PersonnelId == Guid.Empty && (GoodsClassId != Guid.Empty || !string.IsNullOrWhiteSpace(GoodsNameOrCode)))
                goodsList = _goodsManager.GetGoodsPerformanceList(GoodsClassId, new List<Guid>(), GoodsNameOrCode, Request.QueryString["IsPerformance"].ToBool());
            if (goodsList.Count > 0)
            {
                if (goodsIdList.Count > 0)
                {
                    foreach (var info in from info in goodsList
                                         let temp = goodsIdList.FirstOrDefault(ent => ent == info.GoodsId)
                                         where temp == Guid.Empty
                                         select info)
                        goodsIdList.Add(info.GoodsId);
                }
                else
                    goodsIdList.AddRange(goodsList.Select(w => w.GoodsId).ToList());
            }
            IList<StockTurnOverInfo> stockTurnOverList = GoodsStockPile.ReadInstance.GetAvgStockTurnOver(startTime, endTime, WarehouseId, goodsIdList, State);
            if (stockTurnOverList.Count == 0)
                GoodsStockTurnOverRadChart.ChartTitle.TextBlock.Text = "无数据发生";
            else
            {
                GoodsStockTurnOverRadChart.PlotArea.XAxis.AutoScale = false;
                //平均库存周转率
                var chartSeriesAvgTurnOver = new ChartSeries();
                chartSeriesAvgTurnOver.Appearance.FillStyle.MainColor = Color.CornflowerBlue;
                for (var i = 0; i < stockTurnOverList.Count; i++)
                {
                    GoodsStockTurnOverRadChart.PlotArea.XAxis.AddItem(stockTurnOverList[i].CreateDate.ToString("MM-dd"));
                    var avg = Convert.ToInt32(stockTurnOverList[i].AvgTurnOver);
                    var salesNumberItem = new ChartSeriesItem(avg);
                    chartSeriesAvgTurnOver.Items.Add(salesNumberItem);
                }
                //X轴文字旋转
                GoodsStockTurnOverRadChart.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 90;
                //显示类型
                chartSeriesAvgTurnOver.Type = ChartSeriesType.Spline;
                chartSeriesAvgTurnOver.Name = "平均库存周转率";
                GoodsStockTurnOverRadChart.AddChartSeries(chartSeriesAvgTurnOver);
                GoodsStockTurnOverRadChart.ChartTitle.TextBlock.Text = "平均库存周转率";
            }
        }

        #region [绑定数据源]

        ///// <summary> 绑定数据源
        ///// </summary>
        //protected void GridGoodsStock_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        //{
        //    var goodsIdList = new List<Guid>();
        //    var goodsInfoList = new List<GoodsInfo>();
        //    if (PersonnelId != Guid.Empty)
        //    {
        //        var tempGoodsList = BllManage.PurchaseSetManage.GetGoodsIdByPersonnelId(PersonnelId).ToList();
        //        if (tempGoodsList.Count > 0)
        //        {
        //           var tempList = GoodsManager.GetGoodsListByGoodsIds(tempGoodsList).Where(ent => ent.ExpandInfo.IsStatisticalPerformance).ToList();
        //            if (!string.IsNullOrWhiteSpace(GoodsNameOrCode))
        //            {
        //                tempList = tempList.Where(ent => ent.GoodsName.Contains(GoodsNameOrCode) || ent.GoodsCode.Contains(GoodsNameOrCode)).ToList();
        //            }
        //            if (tempList.Count > 0)
        //            {
        //                var goodsIds = tempList.Select(ent => ent.GoodsId);
        //                goodsIdList.AddRange(goodsIds);
        //                goodsInfoList.AddRange(tempList);
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //    }
        //    else
        //    {
        //var        goodsInfoList = GoodsManager.GetGoodsBaseInfoListByClassId(GoodsClassId, GoodsNameOrCode).ToList();
        //    }
        //    if (goodsInfoList.Count > 0)
        //    {
        //        if (goodsIdList.Count > 0)
        //        {
        //            foreach (var info in from info in goodsInfoList
        //                                 let temp = goodsIdList.FirstOrDefault(ent => ent == info.GoodsId)
        //                                 where temp == Guid.Empty
        //                                 select info)
        //                goodsIdList.Add(info.GoodsId);
        //        }
        //        else
        //            goodsIdList.AddRange(goodsInfoList.Select(w => w.GoodsId).ToList());
        //    }
        //    var startTime = Convert.ToDateTime(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd 00:00:00"));
        //    var endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
        //    IList<StockTurnOverInfo> stockTurnOverList = goodsStockPile.GetStockTurnOverNow(startTime, endTime, WarehouseId, goodsIdList);
        //    foreach (var stockTurnOverInfo in stockTurnOverList)
        //    {
        //        var goodsInfo = goodsInfoList.FirstOrDefault(w => w.GoodsId == stockTurnOverInfo.GoodsID);
        //        if (goodsInfo == null) continue;
        //        stockTurnOverInfo.GoodsName = goodsInfo.GoodsName;
        //        stockTurnOverInfo.IsScarcity = goodsInfo.IsStockScarcity ? 1 : 0;
        //        stockTurnOverInfo.IsScarcityStr = goodsInfo.IsStockScarcity ? "√" : string.Empty;
        //        stockTurnOverInfo.State = goodsInfo.IsOnShelf ? 1 : 0;
        //        stockTurnOverInfo.IsStateStr = goodsInfo.IsOnShelf ? string.Empty : "√";
        //        #region [判断商品周转情况]
        //        if (stockTurnOverInfo.State == 0 && stockTurnOverInfo.StockNums == 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = "(下架)";
        //            stockTurnOverInfo.EasySort = 7;
        //        }
        //        else if (stockTurnOverInfo.State == 0 && stockTurnOverInfo.StockNums != 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = "(待清理商品-下架却有库存)";
        //            stockTurnOverInfo.EasySort = 2;
        //        }
        //        else if (stockTurnOverInfo.IsScarcity == 1 && stockTurnOverInfo.StockNums == 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = "(缺货)";
        //            stockTurnOverInfo.EasySort = 4;
        //        }
        //        else if (stockTurnOverInfo.IsScarcity == 1 && stockTurnOverInfo.StockNums != 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = "(待清理商品-缺货却有库存)";
        //            stockTurnOverInfo.EasySort = 3;
        //        }
        //        else if (stockTurnOverInfo.StockNums == 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = "0天";
        //            stockTurnOverInfo.EasySort = 6;
        //        }
        //        else if (stockTurnOverInfo.SaleNums > 0)
        //        {
        //            stockTurnOverInfo.TurnOverStr = stockTurnOverInfo.StockNums * 30 / stockTurnOverInfo.SaleNums + "天";
        //            stockTurnOverInfo.EasySort = 5;
        //        }
        //        else
        //        {
        //            stockTurnOverInfo.TurnOverStr = "(无销售商品)";
        //            stockTurnOverInfo.EasySort = 1;
        //        }
        //        #endregion
        //    }

        //    //下架或缺货且有库存
        //    if (State == 1)
        //        stockTurnOverList = stockTurnOverList.Where(ent => (ent.IsScarcity == 1 || ent.State == 0 && ent.StockNums != 0)).ToList();
        //    //无销售商品
        //    if (State == 2)
        //        stockTurnOverList = stockTurnOverList.Where(ent => ent.SaleNums == 0).ToList();
        //    //GridGoodsStock.DataSource = stockTurnOverList.OrderBy(ent => ent.EasySort).ThenByDescending(ent => ent.StockNums).ToList();
        //    //GridGoodsStock.VirtualItemCount = stockTurnOverList.Count;
        //}

        #endregion

        #region [ViewState]

        /// <summary>商品分类ID
        /// </summary>
        protected Guid GoodsClassId
        {
            get
            {
                if (ViewState["GoodsClassId"] == null)
                    ViewState["GoodsClassId"] = Guid.Empty;
                return new Guid(ViewState["GoodsClassId"].ToString());
            }
            set
            {
                ViewState["GoodsClassId"] = value.ToString();
            }
        }

        /// <summary>仓库ID
        /// </summary>
        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                    ViewState["WarehouseId"] = Guid.Empty;
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set
            {
                ViewState["WarehouseId"] = value.ToString();
            }
        }

        /// <summary>状态
        /// </summary>
        protected int State
        {
            get
            {
                if (ViewState["State"] == null)
                    ViewState["State"] = -1;
                return Convert.ToInt32(ViewState["State"].ToString());
            }
            set
            {
                ViewState["State"] = value;
            }
        }

        /// <summary>采购人ID
        /// </summary>
        protected Guid PersonnelId
        {
            get
            {
                if (ViewState["PersonnelId"] == null)
                    ViewState["PersonnelId"] = Guid.Empty;
                return new Guid(ViewState["PersonnelId"].ToString());
            }
            set
            {
                ViewState["PersonnelId"] = value.ToString();
            }
        }

        /// <summary>供应商ID
        /// </summary>
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null)
                    ViewState["CompanyId"] = Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }

        /// <summary>商品名称/编号
        /// </summary>
        protected string GoodsNameOrCode
        {
            get
            {
                if (ViewState["GoodsNameOrCode"] == null)
                    ViewState["GoodsNameOrCode"] = string.Empty;
                return ViewState["GoodsNameOrCode"].ToString();
            }
            set
            {
                ViewState["GoodsNameOrCode"] = value;
            }
        }

        #endregion
    }
}