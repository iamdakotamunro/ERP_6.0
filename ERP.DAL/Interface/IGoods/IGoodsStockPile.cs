using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoods
{
    /// <summary>
    /// 商品当前库存信息
    /// </summary>
    public interface IGoodsStockPile
    {

        /// <summary>得到平均库存周转 2015-04-29  陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="goodsIds">商品ID集合</param>
        /// <param name="state">0全部，1下架或缺货且有库存，2无销售商品</param>
        /// <returns></returns>
        IList<StockTurnOverInfo> GetAvgStockTurnOver(DateTime startTime, DateTime endTime, Guid warehouseId, List<Guid> goodsIds, int state);

        /// <summary>获取商品的库存周转率  2015-04-30 陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        IList<StockTurnOverInfo> GetGoodsStockTurnOverByGoodsId(DateTime startTime, DateTime endTime, Guid goodsId, Guid warehouseId);

        /// <summary>获取商品近3个月内销量情况  2015-06-16  陈重文
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        IList<SalesVolumeInfo> GetGoodsSalesVolume(Guid warehouseId);
    }
}
