using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        ///// <summary>
        ///// 添加系列
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //bool AddSeries(GoodsSeriesInfo info, out string errorMessage);

        ///// <summary>
        ///// 修改系列
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //bool UpdateSeries(GoodsSeriesInfo info, out string errorMessage);

        ///// <summary>
        ///// 删除系列
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //bool DeleteSeries(Guid seriesId, out string errorMessage);

        ///// <summary>
        ///// 绑定系列商品
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <param name="goodsIds"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //bool SetGoodsSeries(Guid seriesId, List<Guid> goodsIds, out string errorMessage);

        ///// <summary>
        ///// 获取商品系列下绑定的商品
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <returns></returns>
        //Dictionary<Guid, string> GetGoodsIdAndGoodsNameBySeriesId(Guid seriesId);

        ///// <summary>
        ///// 根据系列名称获取系列
        ///// </summary>
        ///// <param name="seriesName"></param>
        ///// <returns></returns>
        //IEnumerable<GoodsSeriesInfo> GetSeriesList(string seriesName);

        /// <summary>
        /// 根据系列ID列表获取系列名称
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="seriesIds"></param>
        /// <returns></returns>
        Dictionary<Guid, string> GetSeriesDict(Guid saleFilialeId,List<Guid> seriesIds);
    }
}
