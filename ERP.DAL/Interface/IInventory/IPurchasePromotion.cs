using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 采购促销明细
    /// </summary>
    public interface IPurchasePromotion
    {
        /// <summary>
        /// 根据PromotionId获取采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        IList<PurchasePromotionInfo> GetPurchasePromotionList(Guid promotionId);

        /// <summary>
        /// 根据商品Id获取采购促销信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        IList<PurchasePromotionInfo> GetPurchasePromotionListByGoodsId(Guid goodsId,Guid hostingFilialeId,Guid warehouseId);

        /// <summary>
        /// 根据商品ID和是否现返获取采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="promotionType"></param>
        /// <returns></returns>
        IList<PurchasePromotionInfo> GetPurchasePromotionList(Guid promotionId, Guid goodsId, Guid warehouseId,Guid hostingFilialeId,int promotionType);

        /// <summary>
        /// 添加采购促销明细
        /// </summary>
        /// <param name="info"></param>
        void AddPurchasePromotion(PurchasePromotionInfo info);

        /// <summary>
        /// 删除采购促销明细
        /// </summary>
        /// <param name="promotionId"></param>
        void DeletePurchasePromotion(Guid promotionId);
    }
}
