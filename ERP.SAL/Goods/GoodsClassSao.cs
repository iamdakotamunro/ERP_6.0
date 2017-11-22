using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;

namespace ERP.SAL.Goods
{
    /// <summary>商品分类
    /// </summary>
    public partial class GoodsCenterSao 
    {
        #region [模型转换]
        
        #region --> (ERP)GoodsClassInfo > (GMS)ClassInfo
        static ClassInfo ConvertToClassInfo(GoodsClassInfo goodsClassInfo)
        {
            var classInfo = new ClassInfo
            {
                ClassID = goodsClassInfo.ClassId,
                Name = goodsClassInfo.ClassName,
                OrderIndex = goodsClassInfo.OrderIndex,
                ParentID = goodsClassInfo.ParentClassId
            };
            return classInfo;
        }
        #endregion

        #region --> (GMS)ClassTreeModel> (ERP)GoodsClassInfo
        static GoodsClassInfo ConvertToMyGoodsClassInfo(ClassTreeModel classInfo)
        {
            var goodsClassInfo = new GoodsClassInfo
                                     {
                                         ClassId = classInfo.ClassID,
                                         ClassName = classInfo.ClassName,
                                         OrderIndex = classInfo.OrderIndex,
                                         ParentClassId = classInfo.ParentID,
                                     };
            return goodsClassInfo;
        }
        #endregion

        #region --> (GMS)ClassInfo >(ERP)GoodsClassInfo
        static GoodsClassInfo ConvertToMyGoodsClassInfo(ClassDetail classDetail)
        {
            var goodsClassInfo = new GoodsClassInfo
            {
                ClassId = classDetail.ClassInfo.ClassID,
                ClassName = classDetail.ClassInfo.Name,
                OrderIndex = classDetail.ClassInfo.OrderIndex,
                ParentClassId = classDetail.ClassInfo.ParentID,
                GoodsClassFieldList = classDetail.ClassFieldList
            };
            return goodsClassInfo;
        }
        #endregion

        #endregion

        #region [新增商品分类]
        /// <summary> 
        /// 新增商品分类
        /// </summary>
        /// <param name="goodsClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddClass(GoodsClassInfo goodsClassInfo, out string errorMessage)
        {
            var request = new ClassDetail
            {
                ClassInfo = ConvertToClassInfo(goodsClassInfo),
                ClassFieldList = goodsClassInfo.GoodsClassFieldList,
            };
            var result = GoodsServerClient.AddClass(request);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [修改商品分类]
        /// <summary> 修改商品分类
        /// </summary>
        /// <param name="goodsClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateClass(GoodsClassInfo goodsClassInfo, out string errorMessage)
        {
            var request = new ClassDetail
            {
                ClassInfo = ConvertToClassInfo(goodsClassInfo),
                ClassFieldList = goodsClassInfo.GoodsClassFieldList,
            };
            var result = GoodsServerClient.UpdateClass(request);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [删除商品分类]
        /// <summary> 删除商品分类
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteClass(Guid classId, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteClass(classId);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [获取分类下的所有子分类]
        /// <summary>获取分类下的所有子分类
        /// </summary>
        /// <param name="parentClassId"></param>
        /// <returns></returns>
        public IList<GoodsClassInfo> GetChildClassList(Guid parentClassId)
        {
            var result = GoodsServerClient.GetChildClassList(parentClassId);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<ClassTreeModel>();
                return items.Select(ConvertToMyGoodsClassInfo).ToList();
            }
            return new List<GoodsClassInfo>();
        }
        #endregion

        #region [获取商品分类信息]
        /// <summary>获取商品分类信息
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public GoodsClassInfo GetClassDetail(Guid classId)
        {
            var result = GoodsServerClient.GetClassDetail(classId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGoodsClassInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region [获取所有商品分类]
        /// <summary>获取所有商品分类
        /// </summary>
        /// <returns></returns>
        public List<GoodsClassInfo> GetAllClassList()
        {
            var list = new List<GoodsClassInfo>();
            var result = GoodsServerClient.GetAllClassList();
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<ClassTreeModel>();
                list.AddRange(items.Select(item => new GoodsClassInfo
                                                       {
                                                           ClassId = item.ClassID, 
                                                           ClassName = item.ClassName, 
                                                           OrderIndex = item.OrderIndex, 
                                                           ParentClassId = item.ParentID
                                                       }));
            }
            return list;
        }
        #endregion
    }
}
