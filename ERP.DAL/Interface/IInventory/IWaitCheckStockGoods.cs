using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// add by dinghq 2011-06-29
    /// </summary>
    public interface IWaitCheckStockGoods
    {
        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        int InsertWaitCheckGoodsInfo(WaitCheckGoodsInfo info);

        /// <summary>
        /// 指定仓库指定状态的商品集合
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        IList<WaitCheckGoodsInfo> GetWaitCheckGoodsInfo(Guid warehouseId, WaitCheckGoodsState state);
        /// <summary>
        /// 指定仓库指定状态模糊搜索商品
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="state"></param>
        /// <param name="goodsName"></param>
        /// <returns></returns>
        IList<WaitCheckGoodsInfo> GetWaitCheckGoodsInfo(Guid warehouseId, WaitCheckGoodsState state,string goodsName);

        /// <summary>
        /// 更新指定仓库指定产品的盘点状态
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int UpdateWaitCheckGoodsState(Guid goodsId, Guid warehouseId, WaitCheckGoodsState state);

        /// <summary>
        /// 指定仓库指定商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        WaitCheckGoodsInfo GetCheckGoodsInfo(Guid goodsId, Guid warehouseId);
    }
}
