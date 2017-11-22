using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary> 地区 2015-05-08  陈重文  （优化代码，增加注释）
    /// </summary>
    public interface IDistrict
    {
        /// <summary> 插入地区信息
        /// </summary>
        /// <param name="districtInfo">地区信息</param>
        /// <returns></returns>
        bool Insert(DistrictInfo districtInfo);

        /// <summary> 更新地区信息
        /// </summary>
        /// <param name="districtInfo">地区信息</param>
        /// <returns></returns>
        bool Update(DistrictInfo districtInfo);

        /// <summary> 删除地区信息
        /// </summary>
        /// <param name="districtId">地区ID</param>
        /// <param name="provinceId"> 省份ID</param>
        /// <param name="cityId"> 城市ID</param>
        /// <returns></returns>
        bool Delete(Guid districtId, Guid provinceId, Guid cityId);

        /// <summary> 获取地区信息列表
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <param name="cityId">城市Id</param>
        /// <param name="searchKey">关键字</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        IEnumerable<DistrictInfo> GetDistrictList(Guid provinceId, Guid cityId, string searchKey, bool ascOrDesc);

        /// <summary>获取地区信息
        /// </summary>
        /// <param name="districtId">地区ID</param>
        /// <returns></returns>
        DistrictInfo GetDistrictInfo(Guid districtId);

        /// <summary>获取城市下的地区数
        /// </summary>
        /// <param name="cityId">城市ID</param>
        /// <returns></returns>
        int GetDistrictCountByCityId(Guid cityId);
    }
}
