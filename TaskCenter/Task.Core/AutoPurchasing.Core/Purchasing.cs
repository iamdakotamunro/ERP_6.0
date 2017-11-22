using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.Model;
using ERP.Model.Goods;
using AutoPurchasing.Core.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper.Common;

namespace AutoPurchasing.Core
{
    public class PurchasingWorkFlow
    {

        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();

        /// <summary>
        /// 获取实际需求的采购数量
        /// </summary>
        /// <param name="boxNumber">每箱多少瓶</param>
        /// <param name="realNumber">此次总共多少瓶</param>
        /// <returns></returns>
        public static double GetBoxNumber(int boxNumber, int realNumber)
        {
            if (boxNumber > 0)
            {
                double revalue = (double)realNumber / boxNumber;

                if (revalue < 0.8)
                {
                    return revalue * boxNumber;
                }

                if (revalue >= 0.8 && revalue <= 5)
                {
                    return Convert.ToInt32(revalue) * boxNumber;
                }

                if (revalue > 5)
                {
                    if (revalue == (int)revalue)//整数
                    {
                        return revalue * boxNumber;
                    }
                    return Math.Ceiling(revalue) * boxNumber;
                }
            }
            return realNumber;
        }

        public static void RunTask()
        {
            var pg = DataAccessor.GetPurchasingGoodsList(TaskType.All);
            var goodsIdList = pg.Select(w => w.GoodsId).Distinct().ToList();
            IList<GoodsInfo> goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList).ToList();
            var warehouses = WMSSao.GetAllCanUseWarehouseDics();
            foreach (var p in pg)
            {
                bool isRun;
                TaskType taskType;
                bool hasSaveLastPurchasingDate;
                int avgStockDays;
                DateTime nextStockDate;
                DateTime nextPurchasingDate;
                int step;

                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == p.GoodsId);
                if (goodsInfo == null) continue;

                p.GoodsName = goodsInfo.GoodsName;
                p.GoodsCode = goodsInfo.GoodsCode;
                p.Units = goodsInfo.Units;
                if (goodsInfo.ExpandInfo != null)
                    p.PackQuantity = goodsInfo.ExpandInfo.PackCount;

                //计算报备日期是否匹配
                CalculateStockDay(p, p.LastPurchasingDate, p.FilingDay, p.StockingDays, out isRun, out taskType, out avgStockDays, out nextStockDate, out step, out hasSaveLastPurchasingDate, out nextPurchasingDate);

                if (isRun)
                {
                    Routine.DoPurchasing(p, goodsInfo, avgStockDays, nextStockDate, taskType, step, nextPurchasingDate, warehouses);
                }

                if (hasSaveLastPurchasingDate)
                {
                    //记录最后的采购运行时间
                    DataAccessor.UpdateLastPurchasingDate(p.WarehouseId,p.HostingFilialeId,p.GoodsId,DateTime.Now);
                }
            }
        }

        #region -- 计算报备时间差
        private static void CalculateStockDay(PurchasingGoods p, DateTime lastPurchasingDate, int filingDay, int stockDays, out bool isRun, out TaskType taskType, out int avgStockDays, out DateTime nextStockDate, out int step, out bool saveLastPurchasingDate, out DateTime nextPurchasingDate)
        {
            nextPurchasingDate = DateTime.MinValue;
            step = 0;
            isRun = false;
            taskType = TaskType.Routine;
            saveLastPurchasingDate = false;
            var addMonths = 3;
            if (stockDays == 30)
            {
                addMonths = 1;
            }
            else if (stockDays == 60)
            {
                addMonths = 2;
            }

            int nowDay = DateTime.Now.Day;
            avgStockDays = Convert.ToInt32(Math.Ceiling((double)stockDays / 4));

            //计算出上一次虚拟的报备时间
            if (lastPurchasingDate == DateTime.MinValue)
            {
                if (nowDay > filingDay)
                {
                    lastPurchasingDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-") + filingDay);
                }
                else if (nowDay <= filingDay)
                {
                    lastPurchasingDate = DateTime.Parse(DateTime.Now.AddMonths(-addMonths).ToString("yyyy-MM-") + filingDay);
                }

                //记录最后的采购运行时间
                DataAccessor.UpdateLastPurchasingDate(p.WarehouseId,p.HostingFilialeId,p.GoodsId, lastPurchasingDate.Date);
            }


            //计算报备周期的天数间隔，并平均四等分
            nextStockDate = lastPurchasingDate.AddMonths(addMonths);

            //开始匹配四次备货的时间
            if (DateTime.Now.ToShortDateString() == lastPurchasingDate.AddDays(avgStockDays).ToShortDateString())
            {
                nextPurchasingDate = DateTime.Now.AddDays(avgStockDays);
                taskType = TaskType.Warning;
                isRun = true;
                step = 2;
            }
            else if (DateTime.Now.ToShortDateString() == lastPurchasingDate.AddDays(avgStockDays * 2).ToShortDateString())
            {
                nextPurchasingDate = DateTime.Now.AddDays(avgStockDays * 2);
                taskType = TaskType.Routine;
                isRun = true;
                step = 3;
            }
            else if (DateTime.Now.ToShortDateString() == lastPurchasingDate.AddDays(avgStockDays * 3).ToShortDateString())
            {
                nextPurchasingDate = nextStockDate;
                taskType = TaskType.Warning;
                isRun = true;
                step = 4;
            }
            else if ((DateTime.Now - nextStockDate).Days == 0)
            {
                nextPurchasingDate = DateTime.Now.AddDays(avgStockDays);
                taskType = TaskType.Routine;
                isRun = true;
                saveLastPurchasingDate = true;
                nextStockDate = DateTime.Now.AddMonths(addMonths);
                step = 1;
            }
            //else if ((DateTime.Now - nextStockDate).Days > 0)
            //{
            //    var tempDay = p.FilingDay;
            //    while (DateTime.Now.Day > tempDay)
            //    {
            //        tempDay += avgStockDays;
            //    }
            //    var tempDate = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + tempDay);
            //    nextPurchasingDate = tempDate;
            //    taskType = TaskType.Routine;
            //    isRun = true;
            //    saveLastPurchasingDate = true;
            //    nextStockDate = DateTime.Now.AddMonths(addMonths);
            //    step = 1; 
            //}
        }
        #endregion

        #region -- 新的报备，包含预警报备
        /// <summary>
        /// 新的报备
        /// </summary>
        public class Routine
        {
            /// <summary>
            /// 开始常规报备
            /// </summary>
            public static void DoPurchasing(PurchasingGoods p, GoodsInfo goodsInfo, int avgStockDays, DateTime nextStockDate, TaskType taskType, int step, DateTime nextPurchasingDate, Dictionary<Guid, string> warehouseDics)
            {
#if debug
                Console.WriteLine("");
                Console.WriteLine("商品ID：{0}", p.GoodsId);
                Console.WriteLine("商品名称：{0}", p.GoodsName);
                if (p.WarehouseId != Guid.Empty)
                {
                    Console.WriteLine("对应仓库：{0}", p.WarehouseName);
                }
                else
                {
                    Console.WriteLine("对应仓库：全部仓库");
                }
#endif
                List<Guid> goodsIdList = _goodsCenterSao.GetRealGoodsIdsByGoodsId(p.GoodsId).ToList();
                if (goodsIdList.Count == 0)
                    goodsIdList.Add(p.GoodsId);
                //指定仓库报备
                if (p.WarehouseId != Guid.Empty)
                {
                    //获取销售总统计
                    var childGoodsSaleAll = GetChildGoodsSaleTotalByDays(goodsIdList, p.WarehouseId, p.StockingDays);
                    //逐个算出计划采购
                    if (childGoodsSaleAll.Count > 0)
                    {
                        var stocks = WMSSao.GetStockStatisticsDtos(goodsIdList.ToDictionary(k => k, v => goodsInfo.GoodsType), p.WarehouseId, Guid.Empty);
                        foreach (var childGoodsSalePurchasing in childGoodsSaleAll.GroupBy(act => act.HostingFilialeId))
                        {
                            var key = childGoodsSalePurchasing.Key;
                            //循环遍历子商品的销售和采购信息
                            OperationSaleAndPurchasing(p, childGoodsSalePurchasing, p.WarehouseId, key, avgStockDays, nextStockDate, taskType, step, nextPurchasingDate, stocks.ContainsKey(key)
                                ? stocks[key] : new List<ERP.SAL.WMS.StockStatisticsDTO>());
                        }
                    }
                    else
                    {
                        Console.WriteLine("近期都没有销售记录！");
                    }

#if debug

#endif
                }
                else
                {
                    //全仓库报备
                    foreach (var info in warehouseDics)
                    {
                        var stocks = WMSSao.GetStockStatisticsDtos(goodsIdList.ToDictionary(k => k, v => goodsInfo.GoodsType), p.WarehouseId, Guid.Empty);
                        //获取销售总统计
                        var childGoodsSaleAll = GetChildGoodsSaleTotalByDays(goodsIdList, info.Key, p.StockingDays);
                        foreach (var childGoodsSalePurchasing in childGoodsSaleAll.GroupBy(act => act.HostingFilialeId))
                        {
                            var key = childGoodsSalePurchasing.Key;
                            //循环遍历子商品的销售和采购信息
                            OperationSaleAndPurchasing(p, childGoodsSalePurchasing, info.Key, key, avgStockDays, nextStockDate, taskType, step, nextPurchasingDate, stocks.ContainsKey(key)
                                ? stocks[key] : new List<ERP.SAL.WMS.StockStatisticsDTO>());
                        }
                    }
                }


#if debug
                {
                    Console.WriteLine("");
                    Console.WriteLine("========================================================================");
                    Console.WriteLine("");
                }
#endif
            }

            #region -- 循环遍历子商品的销售和采购信息 >> OperationSaleAndPurchasing

            /// <summary>
            /// 循环遍历子商品的销售和采购信息
            /// </summary>
            /// <param name="p"></param>
            /// <param name="childGoodsSaleAll"></param>
            /// <param name="warehouseId"></param>
            /// <param name="hostingFilialeId"></param>
            /// <param name="avgStockDays"></param>
            /// <param name="nextStockDate"></param>
            /// <param name="taskType"></param>
            /// <param name="step"></param>
            /// <param name="nextPurchasingDate"></param>
            private static void OperationSaleAndPurchasing(PurchasingGoods p, IEnumerable<ChildGoodsSalePurchasing> childGoodsSaleAll, Guid warehouseId, Guid hostingFilialeId,
                int avgStockDays, DateTime nextStockDate, TaskType taskType, int step, DateTime nextPurchasingDate, List<ERP.SAL.WMS.StockStatisticsDTO> stocks)
            {
                //如果传过来的仓库ID是空，说明是指定仓库
                if (warehouseId == Guid.Empty)
                {
                    warehouseId = p.WarehouseId;
                }
                //产生一个采购单ID
                var purchasingId = Guid.NewGuid();
                var tempPurchasingId = DataAccessor.GetSamePurchasingId(p.CompanyId, p.PersonResponsible, warehouseId, hostingFilialeId);
                if (tempPurchasingId != Guid.Empty)
                {
                    purchasingId = tempPurchasingId;
                }

                //是否有子商品采购信息
                var hasChildGoodsPurchasingInfo = false;
                var isException = false;
                string purchasingNo = DataAccessor.GetCode(BaseInfo.CodeType.PH);
                List<ChildGoodsSalePurchasing> childGoodsSalePurchasingList = childGoodsSaleAll.ToList();
                List<Guid> goodsIdOrRealGoodsIdList = childGoodsSalePurchasingList.Select(w => w.GoodsId).Distinct().ToList();
                Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
                //如果赠送方式为总数量赠送时使用  key 主商品ID  value 额外赠送 
                var dics = new Dictionary<Guid, int>();
                //循环遍历子商品的销售和采购信息
                foreach (var sale in childGoodsSaleAll)
                {
#if debug
                    Console.WriteLine("规格：{0}，销售一：{1}，销售二：{2}，销售三：{3}，平均销售：{4}，增长率：{5}", sale.Specification, sale.FirstNumberOneStockUpSale, sale.FirstNumberTwoStockUpSale, sale.FirstNumberThreeStockUpSale, sale.WeightedAverageSaleQuantity, sale.SaleInCrease);
#endif

                    //计算计划采购数量
                    if (taskType == TaskType.Warning)
                    {
                        if (step == 4)
                        {
                            sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * ((nextStockDate - DateTime.Now).Days + 1 + p.ArrivalDays);
                        }
                        else
                        {
                            sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * (avgStockDays + p.ArrivalDays);
                        }
                    }
                    else if (taskType == TaskType.Routine)
                    {
                        if (step == 3)
                        {
                            sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * ((nextPurchasingDate - DateTime.Now).Days + (avgStockDays * 2) + 1 + p.ArrivalDays);
                        }
                        else
                        {
                            sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * ((nextStockDate - DateTime.Now).Days + 1 + p.ArrivalDays);
                        }
                    }

                    //计算当前的采购商品数量,包含扣除的已经采购完成和部分采购完成和赠品类型
                    sale.SubtractPurchasingQuantity = DataAccessor.GetSumPurchasingQuantity(sale.GoodsId, warehouseId, hostingFilialeId);

                    //计算当前仓库存货数量
                    var info = stocks.FirstOrDefault(act => act.RealGoodsId == sale.GoodsId);
                    sale.NonceWarehouseStockQuantity = info != null ? info.CurrentStock + info.UppingQuantity - info.RequireQuantity : 0;


#if debug
                    Console.WriteLine("预计采购：{0}，采购中的数量：{1}，当前库存数量：{2}", sale.PlanPurchasingquantity, sale.SubtractPurchasingQuantity, sale.NonceWarehouseStockQuantity);
                    if (sale.GoodsId == "D41FBCB0-DB51-447D-8F7C-9767D612EC2B".ToGuid())
                    {
                        sale.IsNull();
                    }
#endif

                    //判断实际的采购数量大于零时，新增一条采购记录
                    if (sale.RealityNeedPurchasingQuantity > 0)
                    {
                        //如果是预警报备，计划采购还是到下一次周期的报备
                        if (taskType == TaskType.Warning)
                        {
                            sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * ((nextStockDate - DateTime.Now).Days + 1 + p.ArrivalDays);
                        }

#if debug
                        Console.WriteLine("实际采购数量：{0}", sale.RealityNeedPurchasingQuantity);
#endif

                        //判断是否有最低库存，如果有要判断采购是否满足最低库存
                        var minStockQuantity = DataAccessor.GetGoodsStockMinQuantity(sale.GoodsId, p.WarehouseId);
                        if (minStockQuantity > 0)
                        {
                            if (sale.RealityNeedPurchasingQuantity < minStockQuantity)
                            {
                                var newPlanPurchasingquantity = (minStockQuantity - sale.RealityNeedPurchasingQuantity) + sale.PlanPurchasingquantity;
                                sale.PlanPurchasingquantity = newPlanPurchasingquantity;
                            }
                        }

                        //采购数
                        int purchasingQuantity = 0;

                        #region [现返]
                        //处理原理：
                        //赠品数量=原采购数量/(买几个+送几个)*送几个
                        //购买数量=原采购数量-赠品数量
                        int zpquantity = 0;
                        var goodsInfo = new GoodsInfo();
                        if (dicGoods != null)
                        {
                            bool hasKey = dicGoods.ContainsKey(sale.GoodsId);
                            if (hasKey)
                            {
                                goodsInfo = dicGoods.FirstOrDefault(w => w.Key == sale.GoodsId).Value;
                            }
                        }
                        IList<PurchasePromotionInfo> ppList = DataAccessor.GetPurchasePromotionList(goodsInfo.GoodsId, (int)PurchasePromotionType.Back,hostingFilialeId);
                        PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 17:29:59"));
                        if (ppInfo != null)
                        {
                            //赠品数量=原采购数量/(买几个+送几个)*送几个
                            zpquantity = (int)sale.RealityNeedPurchasingQuantity / (ppInfo.BuyCount + ppInfo.GivingCount) * ppInfo.GivingCount;
                            if (zpquantity > 0)
                            {
                                int xfQuantity = (int)sale.RealityNeedPurchasingQuantity - zpquantity;//现返类型实际要采购多少数量
                                purchasingQuantity = (int)GetBoxNumber(p.PackQuantity, xfQuantity);//加入箱数采购算法计算采购数
                            }
                        }
                        else
                        {
                            purchasingQuantity = (int)GetBoxNumber(p.PackQuantity, (int)sale.RealityNeedPurchasingQuantity);//加入箱数采购算法计算采购数
                        }
                        #endregion

                        #region [非现返生成借记单]

                        var addedExtra = new List<Guid>();
                        //借记单明细
                        var debitNoteDetailList = new List<DebitNoteDetailInfo>();
                        IList<PurchasePromotionInfo> ppList2 = DataAccessor.GetPurchasePromotionList(goodsInfo.GoodsId, (int)PurchasePromotionType.NoBack,hostingFilialeId);
                        PurchasePromotionInfo ppInfo2 = ppList2.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 17:29:59"));
                        if (ppInfo2 != null)
                        {
                            int pQuantity = (int)sale.RealityNeedPurchasingQuantity;
                            //赠品数量=原采购数量/买几个*送几个
                            int fxfzpquantity = pQuantity / ppInfo2.BuyCount * ppInfo2.GivingCount;
                            int extra = addedExtra.Contains(goodsInfo.GoodsId)
                                        ? 0
                                        : dics[goodsInfo.GoodsId];
                            if (!addedExtra.Contains(goodsInfo.GoodsId))
                            {
                                addedExtra.Add(goodsInfo.GoodsId);
                            }
                            if (fxfzpquantity > 0 || extra > 0)
                            {
                                var debitNoteDetailInfo = new DebitNoteDetailInfo
                                {
                                    PurchasingId = purchasingId,
                                    GoodsId = sale.GoodsId,
                                    GoodsName = p.GoodsName,
                                    Specification = sale.Specification,
                                    GivingCount = fxfzpquantity + extra,
                                    ArrivalCount = 0,
                                    Price = p.Price,
                                    State = 0,
                                    Amount = fxfzpquantity * p.Price,
                                    Memo = "",
                                    Id = Guid.NewGuid()
                                };
                                debitNoteDetailList.Add(debitNoteDetailInfo);
                            }
                            if (debitNoteDetailList.Count > 0)
                            {
                                var debitNoteInfo = new DebitNoteInfo
                                {
                                    PurchasingId = purchasingId,
                                    PurchasingNo = purchasingNo,
                                    CompanyId = p.CompanyId,
                                    PresentAmount = debitNoteDetailList.Sum(w => w.Amount),
                                    CreateDate = DateTime.Now,
                                    FinishDate = DateTime.MinValue,
                                    State = (int)DebitNoteState.ToPurchase,
                                    WarehouseId = p.WarehouseId,
                                    Memo = "",
                                    PersonResponsible = p.PersonResponsible,
                                    NewPurchasingId = Guid.Empty
                                };
                                var dnInfo = DataAccessor.GetDebitNoteInfo(purchasingId);//是否已经有借记单
                                if (dnInfo != null && dnInfo.PurchasingId != Guid.Empty)//已经有借记单则修改借记单
                                {
                                    var dnlist = DataAccessor.GetDebitNoteDetailList(purchasingId);
                                    if (dnlist != null && dnlist.Any(act => act.GoodsId == sale.GoodsId))
                                    {
                                        DataAccessor.UpdateDebitNoteDetail(purchasingId, sale.GoodsId, fxfzpquantity);
                                    }
                                    else
                                    {
                                        foreach (var dinfo in debitNoteDetailList)
                                        {
                                            DataAccessor.AddDebitNoteDetail(dinfo);
                                        }
                                    }
                                }
                                else
                                {
                                    DataAccessor.AddPurchaseSetAndDetail(debitNoteInfo, debitNoteDetailList);
                                }
                            }
                        }
                        #endregion

                        var purchasingGoodsId = Guid.Empty;
                        if (tempPurchasingId != Guid.Empty)
                        {
                            purchasingGoodsId = DataAccessor.GetSamePurchasingGoodsId(tempPurchasingId, sale.GoodsId, sale.Specification, sale.HostingFilialeId);
                        }
                        if (purchasingGoodsId != Guid.Empty)
                        {
                            DataAccessor.UpdatePurchasingGoodsPlanQuantity(purchasingGoodsId, purchasingQuantity);

                            if (zpquantity > 0)//如果有现反赠品则添加一条采购明细记录
                            {
                                var salesinfo = DataAccessor.GetChildGoodsSale(sale.GoodsId, warehouseId, hostingFilialeId, DateTime.Now);
                                DataAccessor.InsertPurchasingDetail(Guid.NewGuid(), (tempPurchasingId == Guid.Empty ? purchasingId : tempPurchasingId), sale.GoodsId,
                                                                    p.GoodsName, p.Units, p.GoodsCode, sale.Specification,
                                                                    p.CompanyId, 0, zpquantity, 0, 0, string.Empty,
                                                                    sale.WeightedAverageSaleQuantity / 30,
                                                                    zpquantity, p.StockingDays + p.ArrivalDays, isException,
                                                                    salesinfo.SixtyDaySales, salesinfo.ThirtyDaySales,
                                                                    (salesinfo.ElevenDaySales / 11), (int)PurchasingGoodsType.Gift);
                            }
                        }
                        else
                        {
                            if (sale.RealityNeedPurchasingQuantity > 0)
                            {
                                var salesinfo = DataAccessor.GetChildGoodsSale(sale.GoodsId, warehouseId, hostingFilialeId, DateTime.Now);

                                if (salesinfo.GoodsId != Guid.Empty)
                                {
                                    //当前活动报备异常
                                    if ((salesinfo.ElevenDaySales / 11) >= ((salesinfo.ThirtyDaySales / 30) * 1.8))
                                    {
                                        isException = true;
                                    }
                                    //历史活动报备异常
                                    if ((salesinfo.ElevenDaySales / 11) * 1.6 <= (salesinfo.ThirtyDaySales / 30) || (salesinfo.ElevenDaySales / 11) * 1.6 <= (salesinfo.SixtyDaySales / 30))
                                    {
                                        isException = true;
                                    }
                                }

                                //插入具体的商品采购信息记录

                                if (zpquantity > 0)//如果有现反赠品则添加一条采购明细记录
                                {
                                    DataAccessor.InsertPurchasingDetail(Guid.NewGuid(), (tempPurchasingId == Guid.Empty ? purchasingId : tempPurchasingId), sale.GoodsId, p.GoodsName, p.Units, p.GoodsCode, sale.Specification,
                                                                        p.CompanyId, 0, zpquantity, 0, 0, string.Empty, sale.WeightedAverageSaleQuantity / 30,
                                                                        zpquantity, p.StockingDays + p.ArrivalDays, isException, salesinfo.SixtyDaySales,
                                                                        salesinfo.ThirtyDaySales, (salesinfo.ElevenDaySales / 11), (int)PurchasingGoodsType.Gift);//赠品
                                }
                                DataAccessor.InsertPurchasingDetail(Guid.NewGuid(), purchasingId, sale.GoodsId, p.GoodsName, p.Units, p.GoodsCode,
                                                                    sale.Specification, p.CompanyId, p.Price, purchasingQuantity, 0, 0,
                                                                    string.Empty, sale.WeightedAverageSaleQuantity / 30,
                                                                    purchasingQuantity, p.StockingDays + p.ArrivalDays, isException,
                                                                    salesinfo.SixtyDaySales, salesinfo.ThirtyDaySales,
                                                                    (salesinfo.ElevenDaySales / 11), (int)PurchasingGoodsType.NoGift);//非赠品

                                hasChildGoodsPurchasingInfo = true;

#if debug
                                Console.WriteLine("新增采购单成功，单据ID：{0}", purchasingId);
#endif

                            }
                        }
                    }
                }

                //插入采购单记录
                if (hasChildGoodsPurchasingInfo && tempPurchasingId == Guid.Empty)
                {
                    if (step == 3)
                    {
                        nextPurchasingDate.AddDays(avgStockDays * 2);
                    }

                    DataAccessor.InsertPurchasing(purchasingId, purchasingNo, p.CompanyId, p.CompanyName, hostingFilialeId, warehouseId, hostingFilialeId, 0, 1, DateTime.Now, DateTime.Now,
                                                "[自动报备]", Guid.Empty, nextStockDate.AddDays(p.ArrivalDays), nextPurchasingDate, isException, p.PersonResponsible);
                }
            }
            #endregion

            #region -- 获取商品的总销售统计信息 >> GetChildGoodsSaleTotalByDays

            /// <summary>
            /// 获取商品的总销售统计信息
            /// </summary>
            /// <param name="goodsIdList"></param>
            /// <param name="warehouseId"></param>
            /// <param name="stockUpDays"></param>
            /// <returns></returns>
            public static IList<ChildGoodsSalePurchasing> GetChildGoodsSaleTotalByDays(List<Guid> goodsIdList, Guid warehouseId, int stockUpDays)
            {
                var dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var total = DataAccessor.GetChildGoodsSaleList(goodsIdList, warehouseId, dateTime.AddDays(-110), dateTime);
                var childGoodsSaleAll = new List<ChildGoodsSalePurchasing>();
                var totalSales = DataAccessor.GetSaleDays(goodsIdList, warehouseId, dateTime);
                foreach (var guid in goodsIdList)
                {
                    var realList = total.Where(act => act.GoodsId == guid);
                    //计算前第一个备货周期的销售额
                    var childSaleList1 = realList.Where(act => act.DayTime >= dateTime.AddDays(-30) && act.DayTime < dateTime);

                    //计算前第二个备货周期的销售额
                    var childSaleList2 = realList.Where(act => act.DayTime >= dateTime.AddDays(-60) && act.DayTime < dateTime.AddDays(30));

                    //计算前第三个备货周期的销售额
                    var childSaleList3 = realList.Where(act => act.DayTime >= dateTime.AddDays(-90) && act.DayTime < dateTime.AddDays(-60));

                    foreach (var hostingFilialeGroup in realList.GroupBy(act => act.HostingFilialeId))
                    {
                        var filialeSales = totalSales.Where(act => act.RealGoodsId == guid && act.HostingFilialeId == hostingFilialeGroup.Key);
                        childGoodsSaleAll.Add(new ChildGoodsSalePurchasing
                        {
                            GoodsId = guid,
                            FirstNumberOneStockUpSale = childSaleList1.Where(act => act.HostingFilialeId == hostingFilialeGroup.Key).Sum(act => act.SaleQuantity),
                            FirstNumberTwoStockUpSale = childSaleList2.Where(act => act.HostingFilialeId == hostingFilialeGroup.Key).Sum(act => act.SaleQuantity),
                            FirstNumberThreeStockUpSale = childSaleList3.Where(act => act.HostingFilialeId == hostingFilialeGroup.Key).Sum(act => act.SaleQuantity),
                            Specification = hostingFilialeGroup.First().Specification,
                            HostingFilialeId = hostingFilialeGroup.Key,
                            PerStepSales = GetPreStepSales(realList, dateTime, filialeSales.Max(act => act.Days))
                        });
                    }
                }
                return childGoodsSaleAll;
            }

            private static Dictionary<int, double> GetPreStepSales(IEnumerable<ChildGoodsSale> realGoodsSales, DateTime dateTime, int days)
            {
                var preStepSales = new Dictionary<int, double>();
                if (realGoodsSales == null || !realGoodsSales.Any()) return preStepSales;
                if (days > 0)
                {
                    if (days < 90)
                    {
                        if (days <= 2 || realGoodsSales.Count() <= 2)
                        {
                            preStepSales.Add(1, realGoodsSales.Sum(act => act.SaleQuantity) / realGoodsSales.Count());
                            return preStepSales;
                        }
                        var max = realGoodsSales.Max(act => act.SaleQuantity);
                        var min = realGoodsSales.Min(act => act.SaleQuantity);
                        var totalSalesNum = realGoodsSales.Sum(act => act.SaleQuantity) - max - min;
                        if (totalSalesNum > 0)
                            preStepSales.Add(1, totalSalesNum / (days - 2));
                    }
                    else
                    {
                        var effictiveDays = days / 10;
                        List<int> removeIndex = effictiveDays == 9 ? new List<int>() :
                            effictiveDays == 10 ? new List<int> { 10 } : new List<int> { 1, 11 };
                        var effitiveSales = effictiveDays == 11 ? realGoodsSales
                            : realGoodsSales.Where(act => act.DayTime >= dateTime.AddDays(-effictiveDays * 10) && act.DayTime < dateTime);
                        var dics = new Dictionary<int, double>();
                        var dayTime = dateTime;
                        for (int i = 1; i <= effictiveDays; i++)
                        {
                            var salesNum = effitiveSales.Where(act =>
                                act.DayTime >= dayTime.AddDays(-10) &&
                                act.DayTime < dayTime).Sum(act => act.SaleQuantity);
                            dics.Add(i, salesNum);
                            dayTime = dayTime.AddDays(-10);
                        }
                        var index = 1;
                        foreach (var dic in dics.OrderBy(act => act.Value))
                        {
                            if (!removeIndex.Contains(index))
                            {
                                preStepSales.Add(dic.Key, dic.Value);
                            }
                            index++;
                        }
                    }
                }
                return preStepSales;
            }
            #endregion
        }
        #endregion
    }
}
