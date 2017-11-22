using ERP.BLL.Interface;
using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.Enum;
using System.Linq;
using ERP.SAL;
using ERP.Environment;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.FinanceModule;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Interface.IInventory;
using ERP.Model.FinanceModule;

namespace ERP.BLL.Implement
{
    /// <summary>
    /// 即时结算价业务层 Add by Jerry Bai 2017/4/27
    /// </summary>
    public class RealTimeGrossSettlementManager : BllInstance<RealTimeGrossSettlementManager>, IRealTimeGrossSettlementManager
    {
        private IRealTimeGrossSettlementDal _realTimeGrossSettlementDal = null;
        private IStorageRecordDao _storageDao = null;
        private IDocumentRedDao _redDao = null;
        private const string LOG_TAG = "即时结算价";

        public RealTimeGrossSettlementManager(GlobalConfig.DB.FromType fromType = GlobalConfig.DB.FromType.Write)
        {
            _realTimeGrossSettlementDal = new RealTimeGrossSettlementDal(fromType);
            _storageDao = new StorageRecordDao(fromType);
            _redDao = new DocumentRedDao(fromType);
        }

        /// <summary>
        /// 获取最新即时结算价
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <returns></returns>
        public decimal GetLatestUnitPrice(Guid filialeId, Guid goodsId)
        {
            return _realTimeGrossSettlementDal.GetLatestUnitPrice(filialeId, goodsId);
        }

        /// <summary>
        /// 在指定时间之前，获取最新的结算价信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public decimal GetLatestUnitPriceBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime)
        {
            return _realTimeGrossSettlementDal.GetLatestUnitPriceBeforeSpecificTime(filialeId, goodsId, specificTime);
        }

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> GetLatestUnitPriceListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds)
        {
            return _realTimeGrossSettlementDal.GetLatestUnitPriceListByMultiGoods(filialeId, goodsIds);
        }

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime)
        {
            return _realTimeGrossSettlementDal.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(filialeId, goodsIds, specificTime);
        }

        /// <summary>
        /// 获取最新即时结算价信息
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <returns></returns>
        public RealTimeGrossSettlementInfo GetLatest(Guid filialeId, Guid goodsId)
        {
            return _realTimeGrossSettlementDal.GetLatest(filialeId, goodsId);
        }

        /// <summary>
        /// 获取最新即时结算价信息
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public RealTimeGrossSettlementInfo GetLatestBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime)
        {
            return _realTimeGrossSettlementDal.GetLatestBeforeSpecificTime(filialeId, goodsId, specificTime);
        }

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        public IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds)
        {
            return _realTimeGrossSettlementDal.GetLatestListByMultiGoods(filialeId, goodsIds);
        }

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        public IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime)
        {
            return _realTimeGrossSettlementDal.GetLatestListBeforeSpecificTimeByMultiGoods(filialeId, goodsIds, specificTime);
        }

        #region 放入队列操作，此次暂时不做

        ///// <summary>
        ///// 入待处理队列
        ///// </summary>
        ///// <param name="item"></param>
        //public void Enqueue(RealTimeGrossSettlementProcessQueueInfo item)
        //{
        //    //TODO:
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 批量入待处理队列
        ///// </summary>
        ///// <param name="items"></param>
        //public void EnqueueBatch(IEnumerable<RealTimeGrossSettlementProcessQueueInfo> items)
        //{
        //    //TODO:
        //    throw new NotImplementedException();
        //}

        #endregion

        /// <summary>
        /// 计算即时结算价（此次同步生成，以后考虑采用队列方式）
        /// </summary>
        /// <param name="item"></param>
        public void Calculate(RealTimeGrossSettlementProcessQueueInfo item)
        {
            try
            {
                _realTimeGrossSettlementDal.Save(Create(item));
                ERP.SAL.LogCenter.LogService.LogInfo(string.Format("保存即时结算价成功！公司ID {0}，主商品ID {1}，关联单据号 {2}", item.FilialeId, item.GoodsId, item.RelatedTradeNo), LOG_TAG, null);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("保存即时结算价失败！公司ID {0}，主商品ID {1}，关联单据号 {2}", item.FilialeId, item.GoodsId, item.RelatedTradeNo), LOG_TAG, ex);
            }
        }

        private RealTimeGrossSettlementInfo Create(RealTimeGrossSettlementProcessQueueInfo item)
        {
            if (item == null)
            {
                return null;
            }
            var result = new RealTimeGrossSettlementInfo
            {
                FilialeId = item.FilialeId,
                GoodsId = item.GoodsId,
                StockQuantity = item.StockQuantity,
                GoodsQuantityInBill = item.GoodsQuantityInBill,
                GoodsAmountInBill = item.GoodsAmountInBill,
                RelatedTradeNo = item.RelatedTradeNo,
                RelatedTradeType = item.RelatedTradeType,
                OccurTime = item.OccurTime,
                ExtField_1 = item.ExtField_1,
            };
            // 即时结算计算方式：(上次结算价 * (最新库存数 - 单据中数量) + 单据中的总金额) / 最新库存数
            switch (item.RelatedTradeType)
            {
                case (int)RealTimeGrossSettlementRelatedTradeType.PurchaseStockIn:
                    if (item.StockQuantity < item.GoodsQuantityInBill)
                    {
                        // 采购后的库存应不能小于采购入库单内的数量
                        result.UnitPrice = item.GoodsAmountInBill / item.GoodsQuantityInBill;
                        if (result.UnitPrice == 0)
                        {
                            result.UnitPrice = item.LastUnitPrice;// 按当前单据计算结算价，如果为0，则取上次结算价
                        }
                    }
                    else
                    {
                        if (item.LastGrossSettlement != null && item.LastGrossSettlement.StockQuantity <= 0
                            && item.LastGrossSettlement.RelatedTradeType == (int)RealTimeGrossSettlementRelatedTradeType.PurchaseReturnStockOut)
                        {
                            // 上次的结算价时采购退货，且库存为0的，则按上次退货和这次进货一起算
                            var x = ((item.StockQuantity - item.GoodsQuantityInBill + item.LastGrossSettlement.GoodsQuantityInBill) * item.LastUnitPrice
                                + item.LastGrossSettlement.GoodsAmountInBill
                                + item.GoodsAmountInBill)
                                / item.StockQuantity;
                        }
                        else
                        {
                            result.UnitPrice = (item.LastUnitPrice * (item.StockQuantity - item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                        }
                    }
                    break;
                case (int)RealTimeGrossSettlementRelatedTradeType.PurchaseReturnStockOut:
                    if (item.StockQuantity <= 0)
                    {
                        // 采购退货后，如果当前库存为0，则不计算，直接取上次结算价
                        result.UnitPrice = item.LastUnitPrice;
                    }
                    else
                    {
                        if (item.LastGrossSettlement != null && item.LastGrossSettlement.StockQuantity < item.GoodsQuantityInBill
                               && item.LastGrossSettlement.RelatedTradeType == (int)RealTimeGrossSettlementRelatedTradeType.PurchaseReturnStockOut)
                        {
                            result.UnitPrice = item.LastUnitPrice;
                        }
                        else
                        {
                            result.UnitPrice = (item.LastUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                        }
                    }
                    break;
                case (int)RealTimeGrossSettlementRelatedTradeType.StockInFormDashAtRed:
                    if (item.GoodsAmountInBill == item.ExtField_1)
                    {
                        // 单据红冲金额和原采购入库单的金额一致，则不需要计算结算价
                        return null;
                    }
                    if (item.StockQuantity <= 0)
                    {
                        result.UnitPrice = item.LastUnitPrice;
                    }
                    else
                    {
                        var tmpUnitPrice = (item.StockQuantity * item.LastUnitPrice + item.GoodsAmountInBill) / (item.StockQuantity + item.GoodsQuantityInBill);// 先按进货方式计算
                        result.UnitPrice = (tmpUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) - item.ExtField_1) / item.StockQuantity;// 再按退货方式计算
                    }
                    break;
                case (int)RealTimeGrossSettlementRelatedTradeType.CombineSplit:
                    if (item.StockQuantity <= 0)
                    {
                        result.UnitPrice = item.GoodsAmountInBill / item.GoodsQuantityInBill;
                    }
                    else
                    {
                        result.UnitPrice = (item.LastUnitPrice * (item.StockQuantity - item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                    }
                    break;
            }
            return result;
        }

        #region 创建待处理队列数据

        /// <summary>
        /// 根据采购入库单，创建待处理队列数据
        /// </summary>
        /// <param name="purchaseStockInId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        public IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByPurchaseStockIn(Guid purchaseStockInId, DateTime occurTime)
        {
            List<RealTimeGrossSettlementProcessQueueInfo> result = new List<RealTimeGrossSettlementProcessQueueInfo>();
            try
            {
                var purchaseStockInRecord = _storageDao.GetStorageRecord(purchaseStockInId);
                var purchaseStockInDetails = _storageDao.GetStorageRecordDetailListByStockId(purchaseStockInId);
                if (purchaseStockInRecord == null || purchaseStockInRecord.StockType != (int)StorageRecordType.BuyStockIn || purchaseStockInRecord.StockState != (int)StorageRecordState.Finished
                    || purchaseStockInDetails == null || purchaseStockInDetails.Count == 0)
                {
                    return result;
                }
                Dictionary<Guid, int> goodsStockQuantityDict = new Dictionary<Guid, int>();
                // 销售公司向物流配送公司采购时，销售公司的库存为0
                if (purchaseStockInRecord.TradeBothPartiesType != (int)TradeBothPartiesType.HostingToSale)
                {
                    goodsStockQuantityDict = WMSSao.GetGoodsStockQuantiyByFilialeIdGoodsIds(purchaseStockInRecord.FilialeId, purchaseStockInDetails.Select(m => m.GoodsId).Distinct());
                }
                var lastGoodsUnitPriceDict = GetLatestListByMultiGoods(purchaseStockInRecord.FilialeId, purchaseStockInDetails.Select(m => m.GoodsId).Distinct());
                foreach (var groupByGoodsId in purchaseStockInDetails.GroupBy(g => g.GoodsId))
                {
                    var item = new RealTimeGrossSettlementProcessQueueInfo
                    {
                        QueueId = Guid.NewGuid(),
                        FilialeId = purchaseStockInRecord.FilialeId,
                        GoodsId = groupByGoodsId.Key,
                        StockQuantity = goodsStockQuantityDict.ContainsKey(groupByGoodsId.Key) ? goodsStockQuantityDict[groupByGoodsId.Key] : 0,
                        LastGrossSettlement = lastGoodsUnitPriceDict.ContainsKey(groupByGoodsId.Key) ? lastGoodsUnitPriceDict[groupByGoodsId.Key] : null,
                        RelatedTradeType = (int)RealTimeGrossSettlementRelatedTradeType.PurchaseStockIn,
                        RelatedTradeNo = purchaseStockInRecord.TradeCode,
                        GoodsQuantityInBill = groupByGoodsId.Sum(m => Math.Abs(m.Quantity)),
                        GoodsAmountInBill = groupByGoodsId.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)),
                        OccurTime = occurTime == DateTime.MinValue ? purchaseStockInRecord.DateCreated : occurTime,
                        CreateTime = DateTime.Now
                    };
                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("创建基于采购入库单 {0} 的即时结算价失败！", purchaseStockInId), LOG_TAG, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据采购退货出库单，创建待处理队列数据
        /// </summary>
        /// <param name="purchaseReturnStockOutId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        public IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByPurchaseReturnStockOut(Guid purchaseReturnStockOutId, DateTime occurTime)
        {
            List<RealTimeGrossSettlementProcessQueueInfo> result = new List<RealTimeGrossSettlementProcessQueueInfo>();
            try
            {
                var purchaseReturnStockOutRecord = _storageDao.GetStorageRecord(purchaseReturnStockOutId);
                var purchaseReturnStockOutDetails = _storageDao.GetStorageRecordDetailListByStockId(purchaseReturnStockOutId);
                if (purchaseReturnStockOutRecord == null || purchaseReturnStockOutRecord.StockType != (int)StorageRecordType.BuyStockOut || purchaseReturnStockOutRecord.StockState != (int)StorageRecordState.Finished
                    || purchaseReturnStockOutDetails == null || purchaseReturnStockOutDetails.Count == 0)
                {
                    return result;
                }
                Dictionary<Guid, int> goodsStockQuantityDict = new Dictionary<Guid, int>();
                // 销售公司向物流配送公司采购退货时，销售公司的库存为0
                if (purchaseReturnStockOutRecord.TradeBothPartiesType != (int)TradeBothPartiesType.HostingToSale)
                {
                    goodsStockQuantityDict = WMSSao.GetGoodsStockQuantiyByFilialeIdGoodsIds(purchaseReturnStockOutRecord.FilialeId, purchaseReturnStockOutDetails.Select(m => m.GoodsId).Distinct());
                }
                var lastGoodsUnitPriceDict = GetLatestListByMultiGoods(purchaseReturnStockOutRecord.FilialeId, purchaseReturnStockOutDetails.Select(m => m.GoodsId).Distinct());
                foreach (var groupByGoodsId in purchaseReturnStockOutDetails.GroupBy(g => g.GoodsId))
                {
                    var item = new RealTimeGrossSettlementProcessQueueInfo
                    {
                        QueueId = Guid.NewGuid(),
                        FilialeId = purchaseReturnStockOutRecord.FilialeId,
                        GoodsId = groupByGoodsId.Key,
                        StockQuantity = goodsStockQuantityDict.ContainsKey(groupByGoodsId.Key) ? goodsStockQuantityDict[groupByGoodsId.Key] : 0,
                        LastGrossSettlement = lastGoodsUnitPriceDict.ContainsKey(groupByGoodsId.Key) ? lastGoodsUnitPriceDict[groupByGoodsId.Key] : null,
                        RelatedTradeType = (int)RealTimeGrossSettlementRelatedTradeType.PurchaseReturnStockOut,
                        RelatedTradeNo = purchaseReturnStockOutRecord.TradeCode,
                        GoodsQuantityInBill = groupByGoodsId.Sum(m => Math.Abs(m.Quantity)),
                        GoodsAmountInBill = -groupByGoodsId.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)),// 采购退货，金额为负数
                        OccurTime = occurTime == DateTime.MinValue ? purchaseReturnStockOutRecord.DateCreated : occurTime,
                        CreateTime = DateTime.Now
                    };
                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("创建基于采购退货出库单 {0} 的即时结算价失败！", purchaseReturnStockOutId), LOG_TAG, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据入库红冲生成的新入库单，创建待处理队列数据
        /// </summary>
        /// <param name="newInDocumentRedId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        public IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByNewInDocumentAtRed(Guid newInDocumentRedId, DateTime occurTime)
        {
            List<RealTimeGrossSettlementProcessQueueInfo> result = new List<RealTimeGrossSettlementProcessQueueInfo>();
            try
            {
                var newInDocument = _redDao.GetDocumentRed(newInDocumentRedId);// 入库红冲单，对应的新入库单
                var newInDocumentDetails = _redDao.GetDocumentRedDetailListByRedId(newInDocumentRedId);// 入库红冲单，对应的新入库单明细
                if (newInDocument == null || newInDocument.DocumentType != (int)DocumentType.NewInDocument || newInDocument.RedType != (int)RedType.ModifyPriceInRed || newInDocument.State != (int)DocumentRedState.Finished
                    || newInDocumentDetails == null || newInDocumentDetails.Count == 0)
                {
                    return result;
                }
                var purchaseStockInRecord = _storageDao.GetStorageRecord(newInDocument.LinkTradeId);// 入库红冲单，对应的原始入库单
                var purchaseStockInDetails = _storageDao.GetStorageRecordDetailListByStockId(newInDocument.LinkTradeId);// 入库红冲单，对应的原始入库单明细
                if (purchaseStockInRecord == null || purchaseStockInRecord.StockType != (int)StorageRecordType.BuyStockIn || purchaseStockInRecord.StockState != (int)StorageRecordState.Finished
                    || purchaseStockInDetails == null || purchaseStockInDetails.Count == 0)
                {
                    return result;
                }
                var goodsStockQuantityDict = WMSSao.GetGoodsStockQuantiyByFilialeIdGoodsIds(newInDocument.FilialeId, newInDocumentDetails.Select(m => m.GoodsId).Distinct());
                var lastGoodsUnitPriceDict = GetLatestListByMultiGoods(newInDocument.FilialeId, newInDocumentDetails.Select(m => m.GoodsId).Distinct());
                foreach (var groupByGoodsId in newInDocumentDetails.GroupBy(g => g.GoodsId))
                {
                    var purchaseStockInDetailsByGoodsId = purchaseStockInDetails.Where(m => m.GoodsId == groupByGoodsId.Key);
                    var item = new RealTimeGrossSettlementProcessQueueInfo
                    {
                        QueueId = Guid.NewGuid(),
                        FilialeId = newInDocument.FilialeId,
                        GoodsId = groupByGoodsId.Key,
                        StockQuantity = goodsStockQuantityDict.ContainsKey(groupByGoodsId.Key) ? goodsStockQuantityDict[groupByGoodsId.Key] : 0,
                        LastGrossSettlement = lastGoodsUnitPriceDict.ContainsKey(groupByGoodsId.Key) ? lastGoodsUnitPriceDict[groupByGoodsId.Key] : null,
                        RelatedTradeType = (int)RealTimeGrossSettlementRelatedTradeType.StockInFormDashAtRed,
                        RelatedTradeNo = newInDocument.TradeCode,
                        GoodsQuantityInBill = groupByGoodsId.Sum(m => Math.Abs(m.Quantity)),
                        GoodsAmountInBill = groupByGoodsId.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)),// 红冲单金额
                        ExtField_1 = purchaseStockInDetailsByGoodsId.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)), // 原采购入库单金额
                        OccurTime = occurTime == DateTime.MinValue ? (newInDocument.AuditTime.HasValue ? newInDocument.AuditTime.Value : newInDocument.DateCreated) : occurTime,
                        CreateTime = DateTime.Now
                    };
                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("创建基于入库红冲生成的新入库单 {0} 的即时结算价失败！", newInDocumentRedId), LOG_TAG, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据来自WMS的拆分组合单，创建待处理队列数据
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        public IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByCombineSplit(ERP.SAL.WMS.CombineSplitBillDTO bill, DateTime occurTime)
        {
            List<RealTimeGrossSettlementProcessQueueInfo> result = new List<RealTimeGrossSettlementProcessQueueInfo>();
            if (bill == null || bill.Details == null || bill.Details.Count == 0 || bill.Details.Any(m => m.Quantity <= 0))
            {
                return result;
            }
            try
            {
                var goodsIds = bill.Details.Select(m => m.GoodsId).Union(new Guid[] { bill.GoodsId }).Distinct();
                var goodsStockQuantityDict = WMSSao.GetGoodsStockQuantiyByFilialeIdGoodsIds(bill.HostingFilialeId, goodsIds);
                
                if (bill.IsSplit)
                {
                    // 拆分单
                    var fromGoodsUnitPrice = GetLatestUnitPrice(bill.HostingFilialeId, bill.GoodsId);
                    var toGoodsUnitPriceDict = GetLatestListByMultiGoods(bill.HostingFilialeId, bill.Details.Select(m => m.GoodsId));
                    var toTotalCost = bill.Details.Sum(m => m.Quantity * (toGoodsUnitPriceDict.ContainsKey(m.GoodsId) ? toGoodsUnitPriceDict[m.GoodsId].UnitPrice : 0));
                    foreach (var detail in bill.Details)
                    {
                        var item = new RealTimeGrossSettlementProcessQueueInfo
                        {
                            QueueId = Guid.NewGuid(),
                            FilialeId = bill.HostingFilialeId,
                            GoodsId = detail.GoodsId,
                            StockQuantity = goodsStockQuantityDict.ContainsKey(detail.GoodsId) ? goodsStockQuantityDict[detail.GoodsId] : 0,
                            LastGrossSettlement = toGoodsUnitPriceDict.ContainsKey(detail.GoodsId) ? toGoodsUnitPriceDict[detail.GoodsId] : null,
                            RelatedTradeType = (int)RealTimeGrossSettlementRelatedTradeType.CombineSplit,
                            RelatedTradeNo = bill.BillNo,
                            GoodsQuantityInBill = detail.Quantity,
                            GoodsAmountInBill = fromGoodsUnitPrice * bill.Quantity * detail.Quantity * (toGoodsUnitPriceDict.ContainsKey(detail.GoodsId) ? toGoodsUnitPriceDict[detail.GoodsId].UnitPrice : 0) / toTotalCost,
                            OccurTime = occurTime == DateTime.MinValue ? DateTime.Now : occurTime,
                            CreateTime = DateTime.Now
                        };
                        result.Add(item);
                    }
                }
                else
                {
                    // 组合单
                    var fromGoodsUnitPriceDict = GetLatestUnitPriceListByMultiGoods(bill.HostingFilialeId, bill.Details.Select(m => m.GoodsId));
                    var toGoodsUnitPrice = GetLatest(bill.HostingFilialeId, bill.GoodsId);
                    var item = new RealTimeGrossSettlementProcessQueueInfo
                    {
                        QueueId = Guid.NewGuid(),
                        FilialeId = bill.HostingFilialeId,
                        GoodsId = bill.GoodsId,
                        StockQuantity = goodsStockQuantityDict.ContainsKey(bill.GoodsId) ? goodsStockQuantityDict[bill.GoodsId] : 0,
                        LastGrossSettlement = toGoodsUnitPrice,
                        RelatedTradeType = (int)RealTimeGrossSettlementRelatedTradeType.CombineSplit,
                        RelatedTradeNo = bill.BillNo,
                        GoodsQuantityInBill = bill.Quantity,
                        GoodsAmountInBill = bill.Details.Sum(m => m.Quantity * (fromGoodsUnitPriceDict.ContainsKey(m.GoodsId) ? fromGoodsUnitPriceDict[m.GoodsId] : 0)),
                        OccurTime = occurTime == DateTime.MinValue ? DateTime.Now : occurTime,
                        CreateTime = DateTime.Now
                    };
                    result.Add(item);
                }
            }
            catch(Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("创建基于WMS的拆分组合单 {0} 的即时结算价失败！", bill.BillNo), LOG_TAG, ex);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 归档上个月的结算价
        /// </summary>
        public void ArchiveLastMonth()
        {
            _realTimeGrossSettlementDal.ArchiveLastMonth();
        }
        
        /// <summary>
         /// 获取已按月归档的商品结算价历史列表
         /// </summary>
         /// <param name="filialeId"></param>
         /// <param name="goodsId"></param>
         /// <returns></returns>
        public IList<GoodsGrossSettlementByMonthInfo> GetArchivedUnitPriceHistoryList(Guid filialeId, Guid goodsId)
        {
            return _realTimeGrossSettlementDal.GetArchivedUnitPriceHistoryList(filialeId, goodsId);
        }

        /// <summary>
        /// 获取指定时间下某公司的某商品的最近结算价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-07-28
        public Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime)
        {
            return _realTimeGrossSettlementDal.GetFilialeIdGoodsIdAvgSettlePrice(dateTime);
        }
    }
}