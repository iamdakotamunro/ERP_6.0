//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年12月27日
// 文件创建人:马力
// 最后修改时间:2007年12月27日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>城市 2015-05-08  陈重文  （优化代码，增加注释，去除无使用方法）
    /// </summary>
    public interface ICity
    {
        /// <summary>在表中插入城市信息
        /// </summary>
        /// <param name="city">城市类实例</param>
        int Insert(CityInfo city);

        /// <summary>更新城市记录
        /// </summary>
        /// <param name="city">城市类实例</param>
        int Update(CityInfo city);

        /// <summary> 删除指定城市
        /// </summary>
        /// <param name="cityId">城市Id</param>
        int Delete(Guid cityId);

        /// <summary>获取所有城市列表
        /// </summary>
        /// <returns></returns>
        IList<CityInfo> GetCityList();

        /// <summary>根据所在省份ID获取城市列表
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <returns>返回城市列表</returns>
        IList<CityInfo> GetCityByProvince(Guid provinceId);

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        CityInfo GetCityInfo(Guid cityId);
    }
}
