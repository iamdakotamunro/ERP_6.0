using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;

namespace CMS.BLL.IGoods
{
    public interface IGoodsPrice
    {
        /// <summary>
        /// 获取商品价格集合
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="classId"></param>
        /// <param name="goodsNameKey"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        IList<GoodsPriceInfo> GetList(int pageIndex, int pageSize, Guid classId, string goodsNameKey, out long recordCount);

        /// <summary>
        /// 获取抓取价格
        /// </summary>
        /// <param name="goodsList"></param>
        /// <returns></returns>
        IList<GoodsFetchPriceInfo> GetFetchPriceList(IList<string> goodsList);

        /// <summary>
        /// 商品采集价格,根据商品名称模糊搜索
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="goodsName"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        IList<FetchDataInfo> GetFetchDataList(int pageIndex, int pageSize, string goodsName, out long recordCount);
    }
}
