//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年10月30日
// 文件创建人:马力
// 最后修改时间:2007年10月30日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary> 基本单位 2015-05-08  陈重文  （优化代码，增加注释，去除无使用方法）
    /// </summary>
    public interface IUnits
    {
        /// <summary> 添加基本单位
        /// </summary>
        /// <param name="units">单位名称</param>
        int Insert(UnitsInfo units);

        /// <summary> 更新基本单位
        /// </summary>
        /// <param name="units">单位名称</param>
        int Update(UnitsInfo units);

        /// <summary> 删除基本单位
        /// </summary>
        /// <param name="unitsId">基本单位编号</param>
        int Delete(Guid unitsId);

        /// <summary> 获取基本单位
        /// </summary>
        /// <param name="unitsId">基本单位Id</param>
        /// <returns>返回基本单位类实例</returns>
        UnitsInfo GetUnits(Guid unitsId);

        /// <summary> 获取基本单位列表
        /// </summary>
        /// <returns>返回基本单位列表</returns>
        IList<UnitsInfo> GetUnitsList();
    }
}
