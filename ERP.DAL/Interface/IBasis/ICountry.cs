//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年12月28日
// 文件创建人:马力
// 最后修改时间:2007年12月28日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>国家接口 2015-05-08  陈重文  （优化代码，增加注释，去除无使用方法）
    /// </summary>
    public interface ICountry
    {
        /// <summary> 添加国家信息
        /// </summary>
        /// <param name="country">国家对象</param>
        int Insert(CountryInfo country);

        /// <summary> 更新国家信息
        /// </summary>
        /// <param name="country"></param>
        int Update(CountryInfo country);

        /// <summary> 删除指定国家
        /// </summary>
        /// <param name="countryId">国家编号</param>
        int Delete(Guid countryId);

        /// <summary> 获取国家信息
        /// </summary>
        /// <param name="countryId">国家编号</param>
        /// <returns></returns>
        CountryInfo GetCountry(Guid countryId);

        /// <summary>返回所有国家
        /// </summary>
        /// <returns></returns>
        IList<CountryInfo> GetCountryList();

        /// <summary>获取使用国家的省份数
        /// </summary>
        /// <param name="countryId">国家编号</param>
        /// <returns>返回使用该国家的省份数</returns>
        int GetCountryUseCount(Guid countryId);
    }
}
