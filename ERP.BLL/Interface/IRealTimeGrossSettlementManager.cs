using ERP.Model;
using ERP.Model.FinanceModule;
using System;
using System.Collections.Generic;

namespace ERP.BLL.Interface
{
    /// <summary>
    /// 即时结算价业务层 Add by Jerry Bai 2017/4/27
    /// </summary>
    public interface IRealTimeGrossSettlementManager
    {
        /// <summary>
        /// 获取最新即时结算价
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <returns></returns>
        decimal GetLatestUnitPrice(Guid filialeId, Guid goodsId);

        /// <summary>
        /// 在指定时间之前，获取最新的结算价信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        decimal GetLatestUnitPriceBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime);

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <returns></returns>
        IDictionary<Guid, decimal> GetLatestUnitPriceListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds);

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        IDictionary<Guid, decimal> GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime);

        /// <summary>
        /// 获取最新即时结算价信息
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <returns></returns>
        RealTimeGrossSettlementInfo GetLatest(Guid filialeId, Guid goodsId);

        /// <summary>
        /// 获取最新即时结算价信息
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        RealTimeGrossSettlementInfo GetLatestBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime);

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime);

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds);

        #region 放入队列操作，此次暂时不做

        ///// <summary>
        ///// 入待处理队列
        ///// </summary>
        ///// <param name="item"></param>
        //void Enqueue(RealTimeGrossSettlementProcessQueueInfo item);

        ///// <summary>
        ///// 批量入待处理队列
        ///// </summary>
        ///// <param name="items"></param>
        //void EnqueueBatch(IEnumerable<RealTimeGrossSettlementProcessQueueInfo> items);

        #endregion

        /// <summary>
        /// 计算即时结算价（此次同步生成，以后考虑采用队列方式）
        /// </summary>
        /// <param name="item"></param>
        void Calculate(RealTimeGrossSettlementProcessQueueInfo item);

        /// <summary>
        /// 根据采购入库单，创建待处理队列数据
        /// </summary>
        /// <param name="purchaseStockInId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByPurchaseStockIn(Guid purchaseStockInId, DateTime occurTime);

        /// <summary>
        /// 根据采购退货出库单，创建待处理队列数据
        /// </summary>
        /// <param name="purchaseReturnStockOutId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByPurchaseReturnStockOut(Guid purchaseReturnStockOutId, DateTime occurTime);

        /// <summary>
        /// 根据采购入库单的入库红冲生成的新入库单，创建待处理队列数据
        /// </summary>
        /// <param name="newInDocumentRedId"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByNewInDocumentAtRed(Guid newInDocumentRedId, DateTime occurTime);

        /// <summary>
        /// 根据来自WMS的拆分组合单，创建待处理队列数据
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="occurTime"></param>
        /// <returns></returns>
        IEnumerable<RealTimeGrossSettlementProcessQueueInfo> CreateByCombineSplit(ERP.SAL.WMS.CombineSplitBillDTO bill, DateTime occurTime);

        /// <summary>
        /// 归档上个月的结算价
        /// </summary>
        void ArchiveLastMonth();

        /// <summary>
        /// 获取已按月归档的商品结算价历史列表
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        IList<GoodsGrossSettlementByMonthInfo> GetArchivedUnitPriceHistoryList(Guid filialeId, Guid goodsId);

        /// <summary>
        /// 获取指定时间下某公司的某商品的最近结算价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-07-28
        Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime);
    }
}
