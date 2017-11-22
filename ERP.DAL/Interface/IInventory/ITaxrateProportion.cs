using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ITaxrateProportion
    {
        /// <summary>
        /// 新增税率记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Insert(TaxrateProportionInfo info);

        /// <summary>
        /// 获取当前包含的税率比例
        /// </summary>
        /// <returns></returns>
        List<decimal> AllPercentage();

        /// <summary>
        /// 获取商品类型税率设置记录
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        List<TaxrateProportionInfo> GetTaxrateProportionInfos(int goodsType);

        /// <summary>
        /// 获取商品类型最新的税率比例
        /// </summary>
        /// <param name="goodsTypes"></param>
        /// <returns></returns>
        List<TaxrateProportionInfo> GetNewPercentages(IEnumerable<int> goodsTypes);

        /// <summary>
        /// 获取商品类型当前的税率
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns>返回-1为不存在记录</returns>
        decimal GetLastPercentage(int goodsType);
    }
}
