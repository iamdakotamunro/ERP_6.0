using ERP.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 商品毛利明细接口
    /// zal 2016-07-04
    /// </summary>
    public interface IGoodsGrossProfitRecordDetail
    {
        /// <summary>
        /// 是否存在商品毛利数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool Exists(DateTime dayTime);

        /// <summary>
        /// 批量插入商品毛利记录明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddDataDetail(IList<GoodsGrossProfitRecordDetailInfo> list);

        /// <summary>
        /// 根据订单时间修改数据状态
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        bool UpdateState(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 根据条件合计商品毛利明细表数据
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<GoodsGrossProfitInfo> SumGoodsGrossProfitRecordDetailInfos(DateTime startTime, DateTime endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 查询商品毛利中超过一个自然月或一个自然月以上未完成的数据（例如：当前是7月份，订单时间是5月份的订单在7月1号之前没有完成的数据）
        /// </summary>
        /// <param name="dayTime">完成时间</param>
        /// <returns></returns>
        IList<GoodsGrossProfitInfo> GetGoodsGrossProfitRecordDetailInfosForMoreMonth(DateTime dayTime);

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsTypes">商品类型</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<GoodsGrossProfitInfo> SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(DateTime startTime, DateTime endTime, string goodsTypes, Guid saleFilialeId, string salePlatformIds, string orderTypes);
    }
}
