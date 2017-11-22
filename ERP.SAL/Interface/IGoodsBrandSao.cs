using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        #region [新增品牌]

        /// <summary>新增品牌
        /// </summary>
        /// <param name="goodsBrandInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddBrand(GoodsBrandInfo goodsBrandInfo, out string errorMessage);
        #endregion

        #region [修改品牌]

        /// <summary>修改品牌
        /// </summary>
        /// <param name="goodsBrandInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateBrand(GoodsBrandInfo goodsBrandInfo, out string errorMessage);
        #endregion

        #region [删除品牌]

        /// <summary>删除品牌
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteBrand(Guid brandId, out string errorMessage);
        #endregion

        #region [获取品牌信息]

        /// <summary>获取品牌信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        GoodsBrandInfo GetBrandDetail(Guid brandId);
        #endregion

        #region [设置品牌序号]

        /// <summary>设置品牌序号
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        bool UpdateBrandOrderIndex(Guid brandId, int orderIndex);
        #endregion

        #region [根据商品是否有资料获取品牌]

        /// <summary>根据商品是否有资料获取品牌
        /// </summary>
        /// <param name="hasInformation">是否有商品资料</param>
        /// <returns></returns>
        IEnumerable<GoodsBrandInfo> GetBrandList(bool hasInformation);
        #endregion

        #region [获取品牌列表]

        /// <summary>获取品牌列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<GoodsBrandInfo> GetAllBrandList();

        #endregion
    }
}
