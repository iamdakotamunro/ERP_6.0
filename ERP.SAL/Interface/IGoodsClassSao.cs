using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        #region [新增商品分类]

        /// <summary> 
        /// 新增商品分类
        /// </summary>
        /// <param name="goodsClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddClass(GoodsClassInfo goodsClassInfo, out string errorMessage);
        #endregion

        #region [修改商品分类]

        /// <summary> 修改商品分类
        /// </summary>
        /// <param name="goodsClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateClass(GoodsClassInfo goodsClassInfo, out string errorMessage);
        #endregion

        #region [删除商品分类]

        /// <summary> 删除商品分类
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteClass(Guid classId, out string errorMessage);
        #endregion

        #region [获取分类下的所有子分类]

        /// <summary>获取分类下的所有子分类
        /// </summary>
        /// <param name="parentClassId"></param>
        /// <returns></returns>
        IList<GoodsClassInfo> GetChildClassList(Guid parentClassId);
        #endregion

        #region [获取商品分类信息]

        /// <summary>获取商品分类信息
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        GoodsClassInfo GetClassDetail(Guid classId);
        #endregion

        #region [获取所有商品分类]

        /// <summary>获取所有商品分类
        /// </summary>
        /// <returns></returns>
        List<GoodsClassInfo> GetAllClassList();

        #endregion
    }
}
