using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddField(FieldInfo fieldInfo, out string errorMessage);

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateField(FieldInfo fieldInfo, out string errorMessage);

        /// <summary>
        /// 设置属性排序
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="orderIndex"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateFieldOrderIndex(Guid fieldId, int orderIndex, out string errorMessage);

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteField(Guid fieldId, out string errorMessage);

        /// <summary>
        /// 根据父属性获取子属性
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        IEnumerable<FieldInfo> GetChildFieldListByFieldId(Guid fieldId);

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<FieldInfo> GetFieldList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        IEnumerable<FieldInfo> GetFieldListByGoodsClassId(Guid classId);

        /// <summary>根据主商品ID获取属性集合
        /// </summary>
        /// <param name="goodsId"></param>
        IEnumerable<FieldInfo> GetFieldDetailByGoodsId(Guid goodsId);
    }
}
