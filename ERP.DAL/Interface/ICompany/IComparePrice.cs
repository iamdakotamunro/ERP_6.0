using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.ICompany
{
    public interface IComparePrice
    {
        /// <summary>
        /// 添加一条匹配信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        int AddProduct(FetchDataInfo product);

        /// <summary>
        /// 更新一条匹配信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        int ModifyProduct(FetchDataInfo product);

        /// <summary>
        /// 更新绑定信息
        /// </summary>
        /// <param name="id">抓取的产品ID</param>
        /// <param name="goodsId">本网站的商品ID</param>
        /// <returns></returns>
        int UpdateBinding(int id, Guid goodsId);
        
        /// <summary>
        /// 根据商品ID 获取
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        IList<FetchDataInfo> GetFetchDataListByGoodsId(Guid goodsId);
        
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        bool SetChecked(int id, bool isChecked);
    }
}
