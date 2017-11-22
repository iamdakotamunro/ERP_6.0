using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoods
{
    /// <summary>
    /// 产品需求信息接口类
    /// </summary>
    public interface IGoodsDemand
    {
        /// <summary>
        /// 获取商品的货位号（分仓后使用）
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        string GetGoodsShelfNo(Guid goodsId, Guid filialeId, Guid warehouseId);

        /// <summary>
        /// 删除该商品到货通知
        /// </summary>
        /// <param name="goodsId"></param>
        void DeleteGoodsStockStatement(Guid goodsId);

        /// <summary>
        /// 获取商品销量列表
        /// Key是商品ID，Value是销售个数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime);
    }
}
