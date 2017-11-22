using System;
using System.Collections.Generic;
using GoodsSeriesInfo = ERP.Model.Goods.GoodsSeriesInfo;
using ServiceGoodsInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsInfo;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao
    {
        //#region -- 模型转换

        ///// <summary>
        ///// 转换成本地GoodsSeriesInfo
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //static GoodsSeriesInfo ConvertToGoodsSeriesInfo(ServiceSeriesInfo info)
        //{
        //    return new GoodsSeriesInfo
        //    {
        //        SeriesID = info.SeriesId,
        //        SeriesName = info.SeriesName
        //    };
        //}

        ///// <summary>
        ///// 转换成服务ServiceSeriesInfo
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //static ServiceSeriesInfo ConvertToServiceSeriesInfo(GoodsSeriesInfo info)
        //{
        //    return new ServiceSeriesInfo
        //    {
        //        SeriesId = info.SeriesID,
        //        SeriesName = info.SeriesName
        //    };
        //}

        //#endregion

        ///// <summary>
        ///// 添加系列
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //public bool AddSeries(GoodsSeriesInfo info, out string errorMessage)
        //{
        //    var result = GoodsServerClient.AddSeries(ConvertToServiceSeriesInfo(info));
        //    errorMessage = string.Empty;
        //    if (result == null) errorMessage = "GMS连接异常";
        //    else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
        //    return result != null && result.IsSuccess;
        //}

        ///// <summary>
        ///// 修改系列
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //public bool UpdateSeries(GoodsSeriesInfo info, out string errorMessage)
        //{
        //    var result = GoodsServerClient.UpdateSeries(ConvertToServiceSeriesInfo(info));
        //    errorMessage = string.Empty;
        //    if (result == null) errorMessage = "GMS连接异常";
        //    else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
        //    return result != null && result.IsSuccess;
        //}

        ///// <summary>
        ///// 删除系列
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //public bool DeleteSeries(Guid seriesId, out string errorMessage)
        //{
        //    var result = GoodsServerClient.DeleteSeries(seriesId);
        //    errorMessage = string.Empty;
        //    if (result == null) errorMessage = "GMS连接异常";
        //    else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
        //    return result != null && result.IsSuccess;
        //}

        ///// <summary>
        ///// 绑定系列商品
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <param name="goodsIds"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //public bool SetGoodsSeries(Guid seriesId, List<Guid> goodsIds, out string errorMessage)
        //{
        //    var result = GoodsServerClient.SetGoodsSeries(seriesId, goodsIds);
        //    errorMessage = string.Empty;
        //    if (result == null) errorMessage = "GMS连接异常";
        //    else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
        //    return result != null && result.IsSuccess;
        //}

        ///// <summary>
        ///// 获取商品系列下绑定的商品
        ///// </summary>
        ///// <param name="seriesId"></param>
        ///// <returns></returns>
        //public Dictionary<Guid, string> GetGoodsIdAndGoodsNameBySeriesId(Guid seriesId)
        //{
        //    var dic = new Dictionary<Guid, string>();
        //    var result = GoodsServerClient.GetGoodsItemModelBySeriesId(seriesId);
        //    if (result != null && result.IsSuccess)
        //    {
        //        foreach (var goodsItemModel in result.Data)
        //        {
        //            dic.Add(goodsItemModel.GoodsId, goodsItemModel.GoodsName);
        //        }
        //    }
        //    return dic;
        //}

        ///// <summary>
        ///// 根据系列名称获取系列
        ///// </summary>
        ///// <param name="seriesName"></param>
        ///// <returns></returns>
        //public IEnumerable<GoodsSeriesInfo> GetSeriesList(string seriesName)
        //{
        //    var result = GoodsServerClient.GetSeriesList(seriesName);
        //    if (result != null && result.IsSuccess)
        //    {
        //        var items = result.Data ?? new List<ServiceSeriesInfo>();
        //        foreach (var item in items)
        //        {
        //            yield return ConvertToGoodsSeriesInfo(item);
        //        }
        //    }
        //}

        /// <summary>
        /// 根据系列ID列表获取系列名称
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="seriesIds"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetSeriesDict(Guid saleFilialeId, List<Guid> seriesIds)
        {
            var result = GoodsServerClient.GetSeriesDict(saleFilialeId,seriesIds);
            var dics =new Dictionary<Guid, string>();
            if (result != null && result.IsSuccess)
            {
                dics = result.Data ?? new Dictionary<Guid, string>();
            }
            return dics;
        }

    }
}
