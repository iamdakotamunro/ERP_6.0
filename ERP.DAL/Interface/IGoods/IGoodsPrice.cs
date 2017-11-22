using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoods
{
    public interface IGoodsPrice
    {
        /// <summary>
        /// 获取抓取价格
        /// </summary>
        /// <param name="goodsIdString"></param>
        /// <returns></returns>
        IList<GoodsFetchPriceInfo> GetFetchPriceList(string goodsIdString);

        /// <summary>
        /// 商品采集价格,根据商品名称模糊搜索
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="goodsName"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        IList<FetchDataInfo> GetFetchDataList(int pageIndex, int pageSize, string goodsName, out long recordCount);

        /// <summary>获取商品的最近进货价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        decimal GetGoodsRecentInPrice(Guid goodsId, Guid warehouseId, Guid filialeId);
    }
}
