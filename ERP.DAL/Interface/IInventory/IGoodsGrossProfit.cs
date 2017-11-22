using System;
using System.Collections.Generic;
using ERP.Model.Report;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 商品毛利接口
    /// </summary>
    public interface IGoodsGrossProfit
    {
        /// <summary>
        /// 是否存在商品毛利数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool Exists(DateTime dayTime);

        /// <summary>
        /// 判断当前月份数据是否存在
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        bool CurrentIsExist(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 删除特定时间内的临时数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        bool DeleteData(int year, int month);

        /// <summary>
        /// 批量插入商品毛利记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddData(IList<GoodsGrossProfitInfo> list);

        /// <summary>
        /// 修改商品毛利记录(如果已存在则修改，不存在添加)
        /// </summary>
        /// <param name="goodsGrossProfitInfo"></param>
        /// <returns></returns>
        bool UpdateGoodsGrossProfitInfo(GoodsGrossProfitInfo goodsGrossProfitInfo);

        /// <summary>
        /// 查询历史月份商品毛利信息
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<GoodsGrossProfitInfo> SelectGoodsGrossProfitInfos(DateTime startTime, DateTime? endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-18
        IList<GoodsGrossProfitInfo> SumGoodsGrossProfitFromMonthByGoodsIdAndSaleFilialeId(DateTime startTime, DateTime? endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes);
    }
}
