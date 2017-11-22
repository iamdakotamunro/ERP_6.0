//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2008年5月29日
// 文件创建人:马力
// 最后修改时间:2008年10月9日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 销售排行接口类
    /// </summary>
    public interface ISalesGoodsRanking
    {
        /// <summary>
        /// 获取销量排行
        /// <para>
        /// Code by Ruanjianfeng 2012-2-28
        /// </para>
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId">品牌下的商品ID</param>
        /// <param name="goodsName"></param>
        /// <param name="goodsCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="salefilialeId"> </param>
        /// <param name="isContainDisableSalePlatform"> </param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetGoodsSalesRanking(int top, string goodsClassList, Guid brandId, string goodsName, string goodsCode,
                                                           Guid warehouseId, Guid salefilialeId, Guid salePlatformId, DateTime startTime,
                                                          DateTime endTime, bool isContainDisableSalePlatform);

        IList<SalesGoodsRankingInfo> GetGoodsSalesRankingByGoodsIds(List<Guid> goodsIds,
            Guid warehouseId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        bool UpdateGoodsSalesRankingGoodsName(Guid goodsId, string goodsName, string goodsCode, Guid seriesId, Guid brandId);

        /// <summary>
        /// 更新销量表中系列ID
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="seriesId"></param>
        /// <returns></returns>
        bool UpdateGoodsSaleSeriesId(Guid goodsId, Guid seriesId);

        /// <summary>
        /// 获取单个商品一段时间内销量
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetSalesRankingChart(Guid goodsId, Guid warehouseId,Guid hostingFilialeId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取系列商品一段时间内销量
        /// </summary>
        /// <param name="seriesId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<SalesGoodsRankingInfo> GetSalesRankingChartBySeriesId(
            Guid seriesId, Guid warehouseId,Guid hostingFilialeId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime);

        #region  销量查询改进  

        /// <summary>
        /// 按商品销量进行查询 包含系列
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySeriesId(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// 按商品销量进行查询
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySale(int top, string goodsClassList, Guid brandId,
            string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// 按平台分组进行查询
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingBySalePlate(int top, string goodsClassList, Guid brandId,
            string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        /// <summary>
        /// 按品牌分组进行查询
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        IList<SaleRaningShowInfo> GetGoodsSaleRankingByBrand(int top, string goodsClassList, Guid brandId, string goodsName, Guid goodsId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform);

        #endregion


        /// <summary>
        /// 供应商销量查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<CompanySaleStatisticsInfo> SelectGoodsSaleStatisticsInfos(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 明细查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<CompanySaleStatisticsInfo> SelectCompanySaleStatisticsInfosByFilialeId(DateTime startTime,
            DateTime endTime, Guid filialeId, string companyName);


        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品日均销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<GoodsAvgDaySalesInfo> GetAvgRealGoodsSaleNumber(DateTime startTime, DateTime endTime);


        /// <summary>
        /// 根据开始时间,截止时间，子商品ID，销售平台 获取时间段内具体销售平台某个子商品的销量。
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="salePlatformId">销售平台</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId, Guid salePlatformId);

        /// <summary>根据销售公司获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        Dictionary<Guid, int> GetGoodsSaleBySaleFilialeId(DateTime fromTime, DateTime toTime, Guid saleFilialeId);

        /// <summary>根据销售平台获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="salePlatformIdList">销售平台ID集合</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        /// zal 2017-07-27
        Dictionary<Guid, int> GetGoodsSaleBySalePlatformIdList(DateTime fromTime, DateTime toTime,List<Guid> salePlatformIdList);
    }
}
