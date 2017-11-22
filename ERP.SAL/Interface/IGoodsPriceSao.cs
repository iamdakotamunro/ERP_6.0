using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        #region --> 更新参考价

        /// <summary>
        /// 更新参考价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="referencePrice"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReferencePrice(Guid goodsId, decimal referencePrice, out string errorMessage);
        #endregion

        #region --> 更新加盟价

        /// <summary>
        /// 更新加盟价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="joinPrice"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateJoinPrice(Guid goodsId, decimal joinPrice, out string errorMessage);
        #endregion

        #region --> 更新批发价

        /// <summary>
        /// 更新批发价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="wholesalePrice">批发价</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateWholesalePrice(Guid goodsId, decimal wholesalePrice, out string errorMessage);
        #endregion

        #region --> 更新隐性成本

        /// <summary>
        /// 更新隐性成本
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="implicitCost"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateImplicitCost(Guid goodsId, decimal implicitCost, out string errorMessage);
        #endregion
        #region --> 更新年终扣率

        /// <summary>
        /// 更新年终扣率
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="yearDiscount"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateYearDiscount(Guid goodsId, decimal yearDiscount, out string errorMessage);
        #endregion

        #region --> 价格更新

        /// <summary>
        /// 价格更新
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="goodsSalePriceList"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool SetGoodsPrice(Guid goodsId, IEnumerable<GoodsSalePriceInfo> goodsSalePriceList,
            out string errorMessage);
        #endregion

        #region --> 根据商品ID获取价格

        /// <summary>
        /// 根据商品ID获取价格
        /// </summary>
        /// <returns></returns>
        DealGoodsSalePriceInfo GetDealGoodsSalePriceInfoByGoodsId(Guid goodsId);
        #endregion

        #region --> 获取商品在所有平台的销售价

        /// <summary>
        /// 获取商品在所有平台的销售价
        /// </summary>
        /// <returns></returns>
        IList<GoodsSalePriceInfo> GetGoodsPriceListByGoodsList(List<Guid> goodsIdList);

        #endregion
    }
}
