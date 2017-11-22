using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.Model.Report;

namespace ERP.DAL.Interface.IStorage
{
    /// <summary>
    /// 库存金额记录相关接口  
    /// 包含表：GoodsStockPriceRecord 商品月结算价记录
    ///  MonthGoodsStockRecord    商品月库存记录
    /// </summary>
    public interface IGoodsStockRecord
    {
        #region 表GoodsStockPriceRecord 商品月结算价记录

        /// <summary>
        /// 添加商品结算价记录
        /// </summary>
        /// <param name="recordInfo"></param>
        /// <returns></returns>
        bool InsertSettlePriceRecord(GoodsStockPriceRecordInfo recordInfo);

        /// <summary>
        /// 批量添加商品结算价
        /// </summary>
        /// <param name="priceRecordInfos"></param>
        /// <returns></returns>
        bool BatchInsert(IList<GoodsStockPriceRecordInfo> priceRecordInfos);

        /// <summary>
        /// 月末库存备份
        /// </summary>
        /// <param name="stockRecordInfos"></param>
        /// <returns></returns>
        bool CopyMonthGoodsStockInfos(IList<MonthGoodsStockRecordInfo> stockRecordInfos);

        /// <summary>
        /// 是否存在商品结算价记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool IsExistsSettlePriceRecord(DateTime dayTime);

        /// <summary>
        /// 查询商品结算价记录
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        GoodsStockPriceRecordInfo SelectStockPriceRecordInfo(Guid goodsId, DateTime datetime);

        /// <summary>
        /// 查询商品结算价列表
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        IList<GoodsStockPriceRecordInfo> SelectGoodsStockPriceRecordInfos(DateTime? dayTime, Guid? goodsId);

        #endregion

        #region   表MonthGoodsStockRecord    商品月库存记录

        /// <summary>
        /// 添加月商品库存记录
        /// </summary>
        /// <param name="recordInfo"></param>
        /// <returns></returns>
        bool InsertGoodsStockRecord(MonthGoodsStockRecordInfo recordInfo);


        /// <summary>
        /// 是否存在商品库存记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool IsExistsGoodsStockRecord(DateTime dayTime);

        /// <summary>
        /// 查询月商品库存记录
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        MonthGoodsStockRecordInfo SelectMonthGoodsStockRecordInfo(Guid realGoodsId, Guid filialeId, Guid warehouseId,
            DateTime dayTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(DateTime dayTime,
            Guid? filialeId, Guid? warehouseId, Guid? goodsId, int? goodsType);

        /// <summary>
        /// 商品类型下商品库存明细  按金额大小排序
        /// </summary>
        /// <param name="goodsType"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(int goodsType, int year, int month);

        /// <summary>
        /// 生成库存金额报表数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        IList<MonthGoodsStockReportInfo> SelectMonthGoodsStockReportInfos(int year, Guid warehouseId);

        /// <summary>
        /// 获取商品特定时间下最近的结算价存档，如果最近结算价没有(即表示是新添加的商品)，则取该商品的采购价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2016-05-19
        Dictionary<Guid, decimal> GetGoodsSettlePriceOrPurchasePriceDicts(DateTime dateTime);

        #endregion

        #region  获取商品月平均价

        /// <summary>
        /// 获取商品特定时间下最近的结算价存档
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetGoodsSettlePriceDicts(DateTime dateTime);

        #endregion
    }
}
