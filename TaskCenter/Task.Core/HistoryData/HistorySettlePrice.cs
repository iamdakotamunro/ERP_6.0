using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IStorage;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IOrder;
using ERP.Model;
using Framework.Core.Utility;
using Keede.DAL.Helper.Common;

namespace HistoryData
{
    public class HistorySettlePrice
    {
        static readonly ISupplierSaleRecord _supplierSaleRecordDao = new SupplierSaleRecordDao();

        static readonly IGoodsStockRecord _goodsStockRecord = new GoodsStockRecordDao();

        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);

        static readonly IGoodsCenterSao _goodsInfoSao = new GoodsCenterSao();

        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);

        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);

        static readonly SupplierSaleRecordBll _supplierSaleRecordBll = new SupplierSaleRecordBll(_supplierSaleRecordDao, _companyCussent, _purchaseSet, _goodsStockRecord, _storageRecordDao);

        static readonly GoodsStockSettleRecordBll _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_goodsStockRecord, _storageRecordDao, _goodsInfoSao);

        static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(GlobalConfig.DB.FromType.Write);

        static readonly IGoodsGrossProfit _goodsGrossProfit = new GoodsGrossProfitDao();

        static readonly IPromotionSao _promotionSao = new PromotionSao();

        static readonly IGoodsGrossProfitRecordDetail _goodsGrossProfitRecordDetail = new GoodsGrossProfitRecordDetailDao();

        static readonly GoodsGrossProfitBll _goodsGrossProfitBll = new GoodsGrossProfitBll(_goodsGrossProfit, _goodsOrderDetail, _promotionSao, _goodsGrossProfitRecordDetail);

        static readonly ICompanyGrossProfitRecord _companyGrossProfitRecord = new CompanyGrossProfitRecordDao();

        static readonly ICompanyGrossProfitRecordDetail _companyGrossProfitRecordDetail = new CompanyGrossProfitRecordDetailDao();

        static readonly IWasteBookReport _wasteBookReport = new WasteBookDao();

        static readonly CompanyGrossProfitRecordBll _companyGrossProfitRecordBll = new CompanyGrossProfitRecordBll(_goodsOrderDetail, _companyGrossProfitRecord, _companyGrossProfitRecordDetail, _wasteBookReport);

        static readonly IGoodsOrder _goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);

        /// <summary>
        /// 商品结算价存档 历史数据
        /// </summary>
        public static void RunHistorySettlePriceTask()
        {
            //结算价的起点是2014年12月份，2014年12月份的结算价数据由老龚初始化，故程序从2015年1月开始计算
            int year = 0, month = 2;

            while (!(DateTime.Now.Year.Equals(2015 + year) && DateTime.Now.Month.Equals(month - 1)))
            {
                if (month == 13)
                {
                    year++;
                    month = 1;
                }

                try
                {
                    var result = _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(2015 + year, month);
                    if (result)
                    {
                        month++;
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("商品库存、结算价报表存档(历史数据)", "RunGoodsStockRecordTask.Error", ex);
                }
            }
        }

        /// <summary>
        /// 商品结算价存档处理(当前系统时间的上一个月的结算价)
        /// </summary>
        public static void RunHistorySettlePriceTaskForPreMonth()
        {
            try
            {
                _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(DateTime.Now.Year, DateTime.Now.Month);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("商品库存、结算价报表存档(历史数据)", "RunGoodsStockRecordTask.Error", ex);
            }
        }

        /// <summary>
        /// 供应商月销售额存档 历史数据
        /// </summary>
        public static void RunHistorySupplierSaleTask()
        {
            //结算价的起点是2014年12月份，2014年12月份的结算价数据由老龚初始化，供应商月销售额从2015年1月开始计算
            int year = 0, month = 1;

            while (!(DateTime.Now.Year.Equals(2015 + year) && DateTime.Now.Month.Equals(month - 1)))
            {
                if (month == 13)
                {
                    year++;
                    month = 1;
                }

                try
                {
                    var result = _supplierSaleRecordBll.InsertSaleRecord(new DateTime(2015 + year, month, 1));
                    if (result)
                    {
                        month++;
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("供应商月销售额报表存档(历史数据)", "RunGoodsStockRecordTask.Error", ex);
                }
            }
        }

        /// <summary>
        /// 供应商月销售额存档(当前系统时间的上一个月的销售额)
        /// </summary>
        public static void RunHistorySupplierSaleTaskForPreMonth()
        {
            try
            {
                _supplierSaleRecordBll.InsertSaleRecord(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1));
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("供应商月销售额报表存档(历史数据)", "RunGoodsStockRecordTask.Error", ex);
            }
        }

        /// <summary>
        /// 商品毛利记录存档 历史数据
        /// </summary>
        public static void RunHistoryGoodsGrossProfitTask()
        {
            while (true)
            {
                Console.WriteLine("商品毛利:" + DateTime.Now);
                var historyStartTime = Configuration.AppSettings["HistoryStartTime"];
                if (DateTime.Now > DateTime.Parse(historyStartTime))
                {
                    break;
                }
            }

            //结算价的起点是2014年12月份，2014年12月份的结算价数据由老龚初始化，商品毛利记录从2015年1月开始计算
            int year = 0, month = 1, currentDay = 2;
            int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
            while ((currentDay <= days + 1) && !(DateTime.Now.Year.Equals(2015 + year) && DateTime.Now.Month.Equals(month - 1)))
            {
                if (currentDay == days + 1)
                {
                    currentDay = 1;
                    month++;

                    if (month == 13)
                    {
                        year++;
                        month = 1;
                    }

                    days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
                }

                var dateTime = new DateTime(2015 + year, month, currentDay);
                try
                {
                    string errorMsg;
                    var result = _goodsGrossProfitBll.AddGoodsGrossProfitRecord(dateTime, out errorMsg);
                    if (result)
                    {
                        currentDay++;
                    }
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常(历史数据)", "RunGoodsGrossProfitTask.Error", new Exception(errorMsg));
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常(历史数据)", "RunGoodsGrossProfitTask.Error", ex);
                }

                #region 输出，指示程序在执行
                Console.WriteLine("商品毛利:"+dateTime);
                #endregion
            }
        }

        /// <summary>
        /// 商品毛利记录存档 历史数据
        /// 在指定间隔时间，无限次重复执行，直到执行成功
        /// </summary>
        public static void RunHistoryGoodsGrossProfitByIntervalTimeTask()
        {
            /*注意：调用此方法需要注释掉所涉及方法中的异常捕获代码，否则可能因为超时等原因报错，导致此方法无法在指定间隔时间后进行下一次调用*/

            /* ExecutionTime说明
             * 如果要补缺少的“天”的数据，则要指定缺少的天的后一天时间，例如：2017-04-05这天的数据没有生成，则此处的时间应该是2017-04-06；
             * 如果要补缺少的“月”的数据，则要指定缺少的月的后两个月的时间，例如：2017-02-01这个月的数据没有生成，则此处的时间应该是2017-04-01；
             */
            var executionTime = Configuration.AppSettings["ExecutionTime"];
            var intervalTime = int.Parse(Configuration.AppSettings["IntervalTime"]);//指定间隔时间
            int i = 0;
            var flag = true;
            var errorTime = DateTime.Now;

            //结算价的起点是2014年12月份，2014年12月份的结算价数据由老龚初始化，商品毛利记录从2015年1月开始计算
            int year = 0, month = 1, currentDay = 2;
            int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
            while ((currentDay <= days + 1) && !(DateTime.Now.Year.Equals(2015 + year) && DateTime.Now.Month.Equals(month - 1)))
            {
                if (currentDay == days + 1)
                {
                    currentDay = 1;
                    month++;

                    if (month == 13)
                    {
                        year++;
                        month = 1;
                    }

                    days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
                }

                if (!flag)//如果报错,则间隔一定的时间之后重新开始执行
                {
                    flag = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:00")).Equals(Convert.ToDateTime(errorTime.ToString("yyyy-MM-dd HH:mm:00")).AddMinutes(intervalTime));
                }
                if (flag)
                {
                    var daytime = new DateTime(2015 + year, month, currentDay);
                    if (daytime.Equals(DateTime.Parse(executionTime)))//只有在指定时间才会执行
                    {
                        string errorMsg;
                        var result = _goodsGrossProfitBll.AddGoodsGrossProfitRecord(new DateTime(2015 + year, month, currentDay), out errorMsg);
                        if (result)
                        {
                            currentDay++;
                        }
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            flag = false;
                            errorTime = DateTime.Now;
                            ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常(历史数据)", "RunGoodsGrossProfitTask.Error", new Exception(errorMsg));
                        }
                    }
                    else
                    {
                        currentDay++;
                    }
                }

                #region 输出，指示程序在执行
                Console.WriteLine(i++);
                if (i == 100000)
                {
                    i = 0;
                }
                #endregion
            }
        }

        /// <summary>
        /// 商品毛利记录存档(当前系统时间的前一天的商品毛利)
        /// </summary>
        public static void RunHistoryGoodsGrossProfitTaskForPreDay()
        {
            try
            {
                string errorMsg;
                _goodsGrossProfitBll.AddGoodsGrossProfitRecord(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常(历史数据)", "RunGoodsGrossProfitTask.Error", new Exception(errorMsg));
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常(历史数据)", "RunGoodsGrossProfitTask.Error", ex);
            }
        }

        /// <summary>
        /// 公司毛利记录存档 历史数据
        /// </summary>
        public static void RunHistoryCompanyGrossProfitTask()
        {
            while (true)
            {
                Console.WriteLine("公司毛利:" + DateTime.Now);
                var historyStartTime = Configuration.AppSettings["HistoryStartTime"];
                if (DateTime.Now > DateTime.Parse(historyStartTime))
                {
                    break;
                }
            }

            //结算价的起点是2014年12月份，2014年12月份的结算价数据由老龚初始化，公司毛利记录从2015年1月开始计算
            int year = 0, month = 1, currentDay = 2;
            int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
            while ((currentDay <= days + 1) && !(DateTime.Now.Year.Equals(2015 + year) && DateTime.Now.Month.Equals(month - 1)))
            {
                if (currentDay == days + 1)
                {
                    currentDay = 1;
                    month++;

                    if (month == 13)
                    {
                        year++;
                        month = 1;
                    }

                    days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(new DateTime(2015 + year, month, 1).Year, new DateTime(2015 + year, month, 1).Month);
                }

                var dateTime = new DateTime(2015 + year, month, currentDay);
                try
                {
                    string errorMsg;
                    var result = _companyGrossProfitRecordBll.AddCompanyGrossProfitRecord(dateTime, out errorMsg);
                    if (result)
                    {
                        currentDay++;
                    }
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        ERP.SAL.LogCenter.LogService.LogError("生成公司毛利异常(历史数据)", "RunCompanyGrossProfitTask.Error", new Exception(errorMsg));
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成公司毛利异常(历史数据)", "RunCompanyGrossProfitTask.Error", ex);
                }

                #region 输出，指示程序在执行
                Console.WriteLine("公司毛利:"+dateTime);
                #endregion
            }
        }

        /// <summary>
        /// 公司毛利记录存档(当前系统时间的前一天的公司毛利)
        /// </summary>
        public static void RunHistoryCompanyGrossProfitTaskForPreDay()
        {
            try
            {
                string errorMsg;
                _companyGrossProfitRecordBll.AddCompanyGrossProfitRecord(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成公司毛利异常(历史数据)", "RunCompanyGrossProfitTask.Error", new Exception(errorMsg));
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("生成公司毛利异常(历史数据)", "RunCompanyGrossProfitTask.Error", ex);
            }
        }

        /// <summary>
        /// 处理日销量表中AvgSettlePrice 历史数据
        /// AvgSettlePrice:对应月结算价表中的结算价的值
        /// </summary>
        public static void RunHistoryGoodsDaySalesStatisticsTask()
        {
            while (true)
            {
                Console.WriteLine("日销量:" + DateTime.Now);
                var historyStartTime = Configuration.AppSettings["HistoryStartTime"];
                if (DateTime.Now > DateTime.Parse(historyStartTime))
                {
                    break;
                }
            }
            //先计算结算价，处理日销量表中的数据的时候，结算价已经计算完成
            int startYear = 0, startMonth = 12, endYear = 0, endMonth = 13;

            while (!(DateTime.Now.Year.Equals(2014 + startYear) && DateTime.Now.Month.Equals(startMonth - 1)))
            {
                #region StartTime
                if (startMonth == 13)
                {
                    startYear++;
                    startMonth = 1;
                }
                #endregion

                #region endTime
                if (endMonth == 13)
                {
                    endYear++;
                    endMonth = 1;
                }
                #endregion

                var dateTime = new DateTime(2014 + startYear, startMonth, 1);
                try
                {
                    var goodsIds = _goodsOrderDetail.GetGoodsDaySalesStatisticsList(new DateTime(2014 + startYear, startMonth, 1), new DateTime(2014 + endYear, endMonth, 1));
                    if (goodsIds.Any())
                    {
                        var settlePrices = _goodsStockRecord.GetGoodsSettlePriceOrPurchasePriceDicts(new DateTime(2014 + startYear, startMonth, 1));
                        if (settlePrices.Any())
                        {
                            foreach (var item in goodsIds)
                            {
                                if (settlePrices.ContainsKey(item))
                                {
                                    _goodsOrderDetail.UpdateGoodsDaySalesStatisticsAvgSettlePrice(item, settlePrices[item], new DateTime(2014 + startYear, startMonth, 1), new DateTime(2014 + endYear, endMonth, 1));
                                }
                            }
                            startMonth++;
                            endMonth++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("处理日销量表中AvgSettlePrice(历史数据)", "GoodsDaySalesStatistics.Error", ex);
                }

                #region 输出，指示程序在执行
                Console.WriteLine("日销量:" + dateTime);
                #endregion
            }
        }

        /// <summary>
        /// 处理日销量表中AvgSettlePrice(当前系统时间的上一个月的数据)
        /// AvgSettlePrice:对应月结算价表中的结算价的值
        /// </summary>
        public static void RunHistoryGoodsDaySalesStatisticsTaskForPreMonth()
        {
            try
            {
                var goodsIds = _goodsOrderDetail.GetGoodsDaySalesStatisticsList(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                if (goodsIds.Any())
                {
                    var settlePrices = _goodsStockRecord.GetGoodsSettlePriceOrPurchasePriceDicts(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                    if (settlePrices.Any())
                    {
                        foreach (var item in goodsIds)
                        {
                            if (settlePrices.ContainsKey(item))
                            {
                                _goodsOrderDetail.UpdateGoodsDaySalesStatisticsAvgSettlePrice(item, settlePrices[item], new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("处理日销量表中AvgSettlePrice(历史数据)", "GoodsDaySalesStatistics.Error", ex);
            }
        }

        /// <summary>
        /// WMS系统上线后，作废订单销量没有扣除
        /// </summary>
        public static void RunHistoryGoodsDaySalesStatisticsForZuoFeiTask()
        {
            try
            {
                var flag = false;
                var startTime = Configuration.AppSettings["StartTime"];
                var pageSize = Configuration.AppSettings["PageSize"];
                while (true)
                {
                    if (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) > DateTime.Parse(startTime))
                    {
                        int i = 1;
                        while (DateTime.Now <= DateTime.Now.AddHours(7))
                        {
                            flag = true;
                            var goodsOrderInfoList = _goodsOrder.GetGoodsOrderInfoListForHistory(i, int.Parse(pageSize));
                            if (goodsOrderInfoList.Count == 0)
                            {
                                break;
                            }
                            foreach (var item in goodsOrderInfoList)
                            {
                                var goodsOrderDetailInfoList = _goodsOrderDetail.GetGoodsOrderDetailList(item.OrderId);
                                var orderJsonStr = Serialization.JsonSerialize(item);
                                var orderDetailJsonStr = Serialization.JsonSerialize(goodsOrderDetailInfoList);
                                var asynGoodsDaySalesStatisticsInfo = ASYN_GoodsDaySalesStatisticsInfo.DeleteGoodsDaySale(item.OrderNo, orderJsonStr, orderDetailJsonStr);
                                bool result = _goodsOrderDetail.InsertASYN_GoodsDaySalesStatisticsInfo(asynGoodsDaySalesStatisticsInfo);
                                if (result)
                                {
                                    _goodsOrder.UpdateOrderIdTable(item.OrderId);
                                }
                            }
                            i++;
                        }
                    }
                    else if (flag)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError("处理WMS上线之后作废订单的销量(历史数据)", "RunHistoryGoodsDaySalesStatisticsForZuoFeiTask.Error", ex);
            }
        }

        /// <summary>
        /// 处理完成时间超过一个自然月或一个自然月以上的数据
        /// </summary>
        public static void CompanyGrossProfitRecordInfosForMoreMonth()
        {
            #region 处理完成时间超过一个自然月或一个自然月以上的数据
            //合计公司毛利中超过一个自然月或一个自然月以上未完成的数据
            var companyGrossProfitRecordList = _companyGrossProfitRecordDetail.GetCompanyGrossProfitDetailInfosForMoreMonth(DateTime.Now);
            if (companyGrossProfitRecordList.Count > 0)
            {
                foreach (var item in companyGrossProfitRecordList)
                {
                    item.SalesAmount = -item.SalesAmount;
                    item.GoodsAmount = -item.GoodsAmount;
                    item.ShipmentIncome = -item.ShipmentIncome;
                    item.PromotionsDeductible = -item.PromotionsDeductible;
                    item.PointsDeduction = -item.PointsDeduction;
                    item.ShipmentCost = -item.ShipmentCost;
                    item.PurchaseCosts = -item.PurchaseCosts;
                    item.CatCommission = -item.CatCommission;
                    _companyGrossProfitRecord.UpdateCompanyGrossProfitRecordInfo(item);
                }
            }
            #endregion
        }

        /// <summary>
        /// 处理完成时间超过一个自然月或一个自然月以上的数据
        /// </summary>
        public static void GoodsGrossProfitInfosForMoreMonth()
        {
            #region 处理完成时间超过一个自然月或一个自然月以上的数据
            //合计商品毛利中超过一个自然月或一个自然月以上未完成的数据
            var goodsGrossProfitRecordList = _goodsGrossProfitRecordDetail.GetGoodsGrossProfitRecordDetailInfosForMoreMonth(DateTime.Now);
            if (goodsGrossProfitRecordList.Any())
            {
                foreach (var item in goodsGrossProfitRecordList)
                {
                    item.SalesPriceTotal = -item.SalesPriceTotal;
                    item.PurchaseCostTotal = -item.PurchaseCostTotal;
                    item.Quantity = -item.Quantity;
                    _goodsGrossProfit.UpdateGoodsGrossProfitInfo(item);
                }
            }
            #endregion
        }
    }
}
