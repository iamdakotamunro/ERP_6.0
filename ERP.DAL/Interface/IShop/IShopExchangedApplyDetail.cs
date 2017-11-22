using System;
using System.Collections.Generic;
using ERP.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    /// <summary>
    /// 退换货申请明细接口
    /// </summary>
    public interface IShopExchangedApplyDetail
    {
        /// <summary>
        /// 添加换货申请明细
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        void InsertShopExchangedApplyDetail(ShopExchangedApplyDetailInfo applyDetailInfo);

        /// <summary>
        /// 添加退货申请明细
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        void InsertShopdApplyDetail(ShopApplyDetailInfo applyDetailInfo);

        /// <summary>
        /// 删除退换货商品明细(单条记录)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int DeleteShopExchangedApplyDetail(Guid id);

        /// <summary>
        /// 删除退换货商品明细(申请单的所有明细)
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        int DeleteShopExchangedApplyDetails(Guid applyId);

        /// <summary>
        /// 根据id获取退货详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ShopApplyDetailInfo GetShopApplyDetailInfo(Guid id);

        /// <summary>
        /// 根据id获取换货详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ShopExchangedApplyDetailInfo GetShopExchangedApplyDetailInfo(Guid id);

        /// <summary>
        /// 根据退货申请Id获取退货明细列表
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        IEnumerable<ShopApplyDetailInfo> GetShopApplyDetailList(Guid applyId);

        /// <summary>
        /// 根据换货申请Id获取换货明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        IEnumerable<ShopExchangedApplyDetailInfo> GetShopExchangedApplyDetailList(Guid applyId);

        /// <summary>
        /// 判断商品是否存在退货记录
        /// </summary>
        /// <param name="shopId">店铺Id</param>
        /// <param name="goodsId">商品Id</param>
        /// <param name="states">退货申请状态</param>
        /// <returns></returns>
        bool IsExistExchangedData(Guid shopId, Guid goodsId, IList<int> states);

        /// <summary>
        /// 判断一段时间内商品是否可采购
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isBarter">false：换货,true 退货</param>
        /// <param name="goodsIds"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>key：主商品Id，value：是否可以采购</returns>
        Dictionary<Guid, bool> IsAllowPurchase(Guid shopId,bool isBarter,IList<Guid> goodsIds,DateTime startTime,DateTime endTime);

        /// <summary>
        /// 获取退换货商品已退还的商品信息
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isbarter"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="states"></param>
        /// <param name="goodsIds"> </param>
        /// <returns></returns>
        Dictionary<Guid, Dictionary<Guid, int>> GetExchangedApplyGoodsQuantity(Guid shopId, int isbarter,
                                                                               DateTime startTime, DateTime endTime,
                                                                               IList<int> states,IList<Guid> goodsIds);
        /// <summary>
        /// 获取商品该店铺最近一次调拨出库的商品单价
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        decimal GetLastUnitPrice(Guid shopId,Guid goodsId);

        /// <summary>
        /// 通过退换货单据号获取退换货申请明细 
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        IEnumerable<ShopExchangedApplyDetailInfo> GetShopExchangedApplyDetailListByNo(string no);
    }
}
