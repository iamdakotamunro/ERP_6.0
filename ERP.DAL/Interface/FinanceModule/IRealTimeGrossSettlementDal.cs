using ERP.Model;
using ERP.Model.FinanceModule;
using System;
using System.Collections.Generic;

namespace ERP.DAL.Interface.FinanceModule
{
    /// <summary>
    /// 即时结算价数据访问层
    /// </summary>
    public interface IRealTimeGrossSettlementDal
    {
        /// <summary>
        /// 获取最新的结算价
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
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
        /// 获取最新的结算价信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        RealTimeGrossSettlementInfo GetLatest(Guid filialeId, Guid goodsId);

        /// <summary>
        /// 获取最新的结算价
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        RealTimeGrossSettlementInfo GetLatestBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime);

        /// <summary>
        /// 按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds);

        /// <summary>
        /// 在指定时间之前，按主商品列表获取最新即时结算价列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="goodsId">主商品ID列表</param>
        /// <param name="specificTime">指定时间</param>
        /// <returns></returns>
        IDictionary<Guid, RealTimeGrossSettlementInfo> GetLatestListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="info"></param>
        void Save(RealTimeGrossSettlementInfo info);

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
