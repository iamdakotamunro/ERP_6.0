using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class StockWarning : BllInstance<StockWarning>
    {
        private readonly IStockWarning _stockWarningDao;
        readonly IGoodsCenterSao _goodsInfoSao;

        public StockWarning(IStockWarning stockWarning, IGoodsCenterSao goodsInfoSao)
        {
            _stockWarningDao = stockWarning;
            _goodsInfoSao = goodsInfoSao;
        }

        public StockWarning(Environment.GlobalConfig.DB.FromType fromType)
        {
            _goodsInfoSao = new GoodsCenterSao();
            _stockWarningDao = InventoryInstance.GetStockWarningDao(fromType);
        }

        /// <summary>
        /// 原始查询  查询30天
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="days"> </param>
        /// <param name="goodsInfo"> </param>
        /// <param name="childGoods"></param>
        /// <returns></returns>
        public IList<StockWarningInfo> GetStockWarningList(Guid warehouseId, Guid hostingFilialeId, int days, GoodsInfo goodsInfo, Dictionary<Guid, ChildGoodsInfo> childGoods)
        {
            IList<StockWarningInfo> stockWarningList = new List<StockWarningInfo>();
            if (goodsInfo != null && goodsInfo.GoodsId != Guid.Empty)
            {
                var realgoodsIds = childGoods.Count > 0 ? childGoods.Keys.ToList() : new List<Guid> {goodsInfo.GoodsId};
                stockWarningList = _stockWarningDao.GetStockWarningList(warehouseId, hostingFilialeId, realgoodsIds, days);
                if (stockWarningList.Count > 0)
                {
                    var quantityList = WMSSao.GetStockStatisticsDtos(realgoodsIds.ToDictionary(k=>k,v=>goodsInfo.GoodsType), warehouseId, hostingFilialeId);

                    foreach (var info in stockWarningList)
                    {
                        info.GoodsName = goodsInfo.GoodsName;
                        info.GoodsCode = goodsInfo.GoodsCode;
                        info.IsOnShelf = goodsInfo.IsOnShelf;
                        if (childGoods.ContainsKey(info.GoodsId))
                        {
                            var childGoodsInfo = childGoods[info.GoodsId] ?? new ChildGoodsInfo();
                            info.Specification = childGoodsInfo.Specification;
                            info.IsScarcity = childGoodsInfo.IsScarcity;
                        }
                        var statisInfo = !quantityList.ContainsKey(hostingFilialeId) ? null : quantityList[hostingFilialeId].FirstOrDefault(act => act.RealGoodsId == info.GoodsId);
                        if (statisInfo != null)
                        {
                            info.NonceWarehouseGoodsStock = statisInfo.CurrentStock;
                            info.UppingQuantity = statisInfo.UppingQuantity;
                            info.RequireQuantity = statisInfo.RequireQuantity;
                            info.SubtotalQuantity = statisInfo.SubtotalQuantity;
                            info.NonceRequest= statisInfo.RequireQuantity;
                        }
                    }
                }
            }
            return stockWarningList.OrderBy(ent => ent.Specification).ToList();
        }

        /// <summary>
        /// 原始查询  查询10天
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="goodsId"></param>
        /// <param name="days"> </param>
        /// <param name="goodsInfo"> </param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public IList<StockWarningInfo> GetStockWarningListNew(Guid warehouseId, Guid filialeId, Guid goodsId, int days, GoodsInfo goodsInfo)
        {
            IList<StockWarningInfo> stockWarnings = new List<StockWarningInfo>();
            if (goodsId == Guid.Empty)
                return new List<StockWarningInfo>();
            List<Guid> realGoodsIds=new List<Guid>();
            if (goodsInfo != null && goodsInfo.GoodsId != Guid.Empty)
            {
                var list = _goodsInfoSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId });
                var realGoodsInfoList = list.ToDictionary(k=>k.RealGoodsId,v=>v);
                realGoodsIds.AddRange(realGoodsInfoList.Keys);
                if (realGoodsIds.Count == 0)
                {
                    throw new Exception(goodsInfo.GoodsName + ": 无子商品");
                }
                var stockWarningList = _stockWarningDao.GetStockWarningList(warehouseId, filialeId, realGoodsIds, days);
                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var totalSales = _stockWarningDao.GetGoodsDaySalesInfos(realGoodsIds, warehouseId, now.AddDays(-110), now, filialeId);
                //计算前第一个备货周期的销售额
                var childSaleList1 = totalSales.Where(act => act.DayTime >= now.AddDays(-30) && act.DayTime < now);

                //计算前第二个备货周期的销售额
                var childSaleList2 = totalSales.Where(act => act.DayTime >= now.AddDays(-60) && act.DayTime < now.AddDays(-30));

                //计算前第三个备货周期的销售额
                var childSaleList3 = totalSales.Where(act => act.DayTime >= now.AddDays(-90) && act.DayTime < now.AddDays(-60));
                //获取子商品对应库存(按托管公司分组)
                var quantityList = WMSSao.GetStockStatisticsDtos(realGoodsIds.ToDictionary(k=>k,v=>goodsInfo.GoodsType), warehouseId, filialeId);

                //获取商品的销售天数
                var salesDays = _stockWarningDao.GetSaleDays(realGoodsIds, warehouseId, now, filialeId);
                foreach (var guid in realGoodsIds)
                {
                    
                    double count1 = childSaleList1.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                    double count2 = childSaleList2.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                    double count3 = childSaleList3.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                    var preStepSales = GetPreStepSales(totalSales, guid, now, salesDays.ContainsKey(guid) ? salesDays[guid] : 0);
                    var warningInfo = stockWarningList.FirstOrDefault(act => act.GoodsId == guid) ?? new StockWarningInfo();
                    warningInfo.SetStepSales(preStepSales.OrderBy(act => act.Key).ToDictionary(k => k.Key, v => v.Value));
                    warningInfo.GoodsId = guid;
                    warningInfo.StockDay = days;
                    warningInfo.FirstNumberOneStockUpSale = count1;
                    warningInfo.FirstNumberTwoStockUpSale = count2;
                    warningInfo.FirstNumberThreeStockUpSale = count3;
                    warningInfo.GoodsName = goodsInfo.GoodsName;
                    warningInfo.GoodsCode = goodsInfo.GoodsCode;
                    warningInfo.IsOnShelf = goodsInfo.IsOnShelf;

                    if (realGoodsInfoList.ContainsKey(guid))
                    {
                        var childGoodsInfo = realGoodsInfoList[guid]??new ChildGoodsInfo();
                        warningInfo.IsScarcity = childGoodsInfo.IsScarcity;
                        warningInfo.Specification = childGoodsInfo.Specification;
                    }

                    var statisInfo = quantityList.ContainsKey(filialeId) && quantityList[filialeId] != null ? quantityList[filialeId].FirstOrDefault(act => act.RealGoodsId == guid) : null;
                    if (statisInfo != null)
                    {
                        warningInfo.NonceWarehouseGoodsStock = statisInfo.CurrentStock;
                        warningInfo.UppingQuantity = statisInfo.UppingQuantity;
                        warningInfo.RequireQuantity = statisInfo.RequireQuantity;
                        warningInfo.SubtotalQuantity = statisInfo.SubtotalQuantity;
						warningInfo.NonceRequest= statisInfo.RequireQuantity;
                    }
                    stockWarnings.Add(warningInfo);
                }
            }
            return stockWarnings.OrderBy(ent => ent.Specification).ToList();
        }

        private Dictionary<int, double> GetPreStepSales(IEnumerable<Model.GoodsDaySalesInfo> totalSales, Guid realGoodsId, DateTime dateTime, int days)
        {
            var preStepSales = new Dictionary<int, double>();
            if (totalSales == null || !totalSales.Any()) return preStepSales;
            if (days > 0)
            {
                var realGoodsSales = totalSales.Where(act => act.RealGoodsID == realGoodsId).ToList();
                if (realGoodsSales.Count == 0) return preStepSales;
                if (days < 90)
                {
                    if (days <= 2 || realGoodsSales.Count <= 2)
                    {
                        preStepSales.Add(1, realGoodsSales.Sum(act => act.GoodsSales) / realGoodsSales.Count);
                        return preStepSales;
                    }
                    var max = realGoodsSales.Max(act => act.GoodsSales);
                    var min = realGoodsSales.Min(act => act.GoodsSales);
                    var totalSalesNum = realGoodsSales.Sum(act => act.GoodsSales) - max - min;
                    if (totalSalesNum > 0)
                        preStepSales.Add(1, totalSalesNum / (days - 2));
                }
                else
                {
                    var effictiveDays = days / 10;
                    effictiveDays = effictiveDays > 11 ? 11 : effictiveDays;
                    var effitiveSales = effictiveDays == 11 ? realGoodsSales
                        : realGoodsSales.Where(act => act.DayTime >= dateTime.AddDays(-effictiveDays * 10) && act.DayTime < dateTime);
                    var dics = new Dictionary<int, double>();
                    var dayTime = dateTime;
                    for (int i = 1; i <= effictiveDays; i++)
                    {
                        var salesNum = effitiveSales.Where(act =>
                            act.DayTime >= dayTime.AddDays(-10) &&
                            act.DayTime < dayTime).Sum(act => act.GoodsSales);
                        dics.Add(i, salesNum);
                        dayTime = dayTime.AddDays(-10);
                    }

                    if (effictiveDays == 10)
                    {
                        var latelyMax = dics.OrderBy(act => act.Value).ThenByDescending(act => act.Key).Last();
                        dics.Remove(latelyMax.Key);
                    }
                    else
                    {
                        var latelyMax = dics.OrderBy(act => act.Value).ThenByDescending(act => act.Key).Last();
                        var latelyMin = dics.OrderBy(act => act.Value).ThenBy(act => act.Key).First();
                        dics.Remove(latelyMax.Key);
                        dics.Remove(latelyMin.Key);
                    }
                    preStepSales = dics;
                }
            }
            return preStepSales;
        }

        /// <summary>
        /// 计算商品日均价   原加权平均销量WeightedAverageSaleQuantity
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="days"> </param>
        /// <returns></returns>
        public IList<StockWarningInfo> GetStockWarningListForReport(Guid warehouseId, Guid filialeId, List<Guid> realGoodsIds, int days)
        {
            if (realGoodsIds != null && realGoodsIds.Count > 0)
            {
                return GetStockWarningList(realGoodsIds, warehouseId, filialeId, days);
            }
            return new List<StockWarningInfo>(); ;
        }

        /// <summary>
        /// 计算商品日均价   原加权平均销量WeightedAverageSaleQuantity*增长率SaleInCrease
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public IList<StockWarningInfo> GetMonthAvgGoodsSales(List<Guid> realGoodsIds, Guid warehouseId, Guid hostingFilialeId, int days)
        {
            return GetStockWarningList(realGoodsIds, warehouseId, hostingFilialeId, days);
        }

        private IList<StockWarningInfo> GetStockWarningList(List<Guid> realGoodsIds, Guid warehouseId, Guid hostingFilialeId, int days)
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var totalSales = _stockWarningDao.GetGoodsDaySalesInfos(realGoodsIds, warehouseId, now.AddDays(-110), now, hostingFilialeId);
            //计算前第一个备货周期的销售额
            var childSaleList1 = totalSales.Where(act => act.DayTime >= now.AddDays(-30) && act.DayTime < now);

            //计算前第二个备货周期的销售额
            var childSaleList2 = totalSales.Where(act => act.DayTime >= now.AddDays(-60) && act.DayTime < now.AddDays(-30));

            //计算前第三个备货周期的销售额
            var childSaleList3 = totalSales.Where(act => act.DayTime >= now.AddDays(-90) && act.DayTime < now.AddDays(-60));

            var stockWarningInfos = new List<StockWarningInfo>();
            var saleDays = _stockWarningDao.GetSaleDays(realGoodsIds, warehouseId, now, hostingFilialeId);
            foreach (var guid in realGoodsIds)
            {
                double count1 = childSaleList1.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                double count2 = childSaleList2.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                double count3 = childSaleList3.Where(act => act.RealGoodsID == guid).Sum(act => act.GoodsSales);
                var preStepSales = GetPreStepSales(totalSales, guid, now, saleDays.ContainsKey(guid) ? saleDays[guid] : 0);
                var newStockWarning = new StockWarningInfo
                {
                    GoodsId = guid,
                    FirstNumberOneStockUpSale = count1,
                    FirstNumberTwoStockUpSale = count2,
                    FirstNumberThreeStockUpSale = count3,
                    StockDay = days
                };
                newStockWarning.SetStepSales(preStepSales);
                stockWarningInfos.Add(newStockWarning);
            }
            return stockWarningInfos;
        }
    }
}
