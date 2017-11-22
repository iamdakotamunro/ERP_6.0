using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using KeedeGroup.GoodsManageSystem.Public.Model.Basic;
using GoodsSeviceFieldInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.FieldInfo;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao 
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddField(FieldInfo fieldInfo, out string errorMessage)
        {
            var result = GoodsServerClient.AddField(ConvertToGoodsSeviceFieldInfo(fieldInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateField(FieldInfo fieldInfo, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateField(ConvertToGoodsSeviceFieldInfo(fieldInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 设置属性排序
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="orderIndex"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateFieldOrderIndex(Guid fieldId, int orderIndex, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateFieldOrderIndex(fieldId, orderIndex);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteField(Guid fieldId, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteField(fieldId);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 根据父属性获取子属性
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public IEnumerable<FieldInfo> GetChildFieldListByFieldId(Guid fieldId)
        {
            var result = GoodsServerClient.GetChildFieldListByFieldId(fieldId);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsSeviceFieldInfo>();
                return items.Select(ConvertToMyFieldInfo).ToList();
            }
            return new List<FieldInfo>();
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FieldInfo> GetFieldList()
        {
            var result = GoodsServerClient.GetFieldList();
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsSeviceFieldInfo>();
                return items.Select(ConvertToMyFieldInfo).ToList();
            }
            return new List<FieldInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public IEnumerable<FieldInfo> GetFieldListByGoodsClassId(Guid classId)
        {
            var list = new List<FieldInfo>();
            var result = GoodsServerClient.GetFieldDetailListByClassId(classId);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<FieldDetail>();
                foreach (var item in items)
                {
                    var parentFieldInfo = ConvertToMyFieldInfo(item.ParentFieldInfo);
                    parentFieldInfo.ChildFields = item.ChildFieldList.Select(ConvertToMyFieldInfo).ToList();
                    list.Add(parentFieldInfo);
                }
            }
            return list;
        }

        /// <summary>根据主商品ID获取属性集合
        /// </summary>
        /// <param name="goodsId"></param>
        public IEnumerable<FieldInfo> GetFieldDetailByGoodsId(Guid goodsId)
        {
            var list = new List<FieldInfo>();
            var result = GoodsServerClient.GetFieldDetailByGoodsId(goodsId);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<FieldDetail>();
                foreach (var item in items)
                {
                    var parentFieldInfo = ConvertToMyFieldInfo(item.ParentFieldInfo);
                    parentFieldInfo.ChildFields = item.ChildFieldList.Select(ConvertToMyFieldInfo).ToList();
                    list.Add(parentFieldInfo);
                }
            }
            return list;
        }

        #region -- 模型转换
        /// <summary>
        /// 转换成FieldInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static FieldInfo ConvertToMyFieldInfo(GoodsSeviceFieldInfo info)
        {
            return new FieldInfo
            {
                FieldId = info.FieldId,
                ParentFieldId = info.ParentFieldId,
                OrderIndex = info.OrderIndex,
                FieldName = info.FieldName,
                FieldValue = info.FieldValue
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static GoodsSeviceFieldInfo ConvertToGoodsSeviceFieldInfo(FieldInfo info)
        {
            return new GoodsSeviceFieldInfo
            {
                FieldId = info.FieldId,
                ParentFieldId = info.ParentFieldId,
                OrderIndex = info.OrderIndex,
                FieldName = info.FieldName,
                FieldValue = info.FieldValue
            };
        }
        #endregion
    }
}
