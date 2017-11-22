//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年5月30日
// 文件创建人:马力
// 最后修改时间:2007年5月30日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>省份 2015-05-08  陈重文  （优化代码，增加注释，删除无用方法）
    /// </summary>
    public interface IProvince
    {
        /// <summary>添加省份信息
        /// </summary>
        /// <param name="provinceInfo">省份类实例</param>
        int Insert(ProvinceInfo provinceInfo);

        /// <summary> 更新省份记录
        /// </summary>
        /// <param name="provinceInfo">省份类实例</param>
        int Update(ProvinceInfo provinceInfo);

        /// <summary> 删除省份信息
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        int Delete(Guid provinceId);
        
        /// <summary> 根据提供Id获取省份类实例
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <returns>返回省份对象</returns>
        ProvinceInfo GetProvince(Guid provinceId);

        /// <summary> 获取所有身份
        /// </summary>
        /// <returns></returns>
        IList<ProvinceInfo> GetProvinceList();

        /// <summary>根据所在国家ID获取省份列表
        /// </summary>
        /// <param name="countryId">国家Id</param>
        /// <returns>返回省份列表</returns>
        IList<ProvinceInfo> GetProvinceList(Guid countryId);
        
        /// <summary>获取使用该省份信息的城市数
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <returns></returns>
        int GetProvinceUseCount(Guid provinceId);
    }
}
