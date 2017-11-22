using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Enum.ModelType;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Result;
using ServiceAttrWordsInfo = KeedeGroup.GoodsManageSystem.Public.Model.AttrWordsInfo;
using ServiceAttrGroupInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.AttrGroupInfo;
using ServiceAttributeInfo = KeedeGroup.GoodsManageSystem.Public.Model.Table.AttributeInfo;

namespace ERP.SAL.Goods
{
    /// <summary>商品属性
    /// </summary>
    public partial class GoodsCenterSao 
    {
        #region -- 模型转换

        /// <summary>
        /// 转换成本地AttrGroupInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static AttributeGroupInfo ConvertToAttrGroupInfo(AttrGroupGoodsTypeGridItemModel info)
        {
            return new AttributeGroupInfo
            {
                GroupId = info.GroupID,
                MatchType = info.MatchType,
                GroupName = info.Name,
                OrderIndex = info.OrderIndex,
                IsMChoice = info.IsMChoice,
                EnabledFilter = info.EnabledFilter,
                IsSelect = info.IsSelect,
                GoodsQuantity = info.GoodsQuantity,
                Unit = info.Unit
            };
        }
        /// <summary>
        /// 转换成本地AttrGroupInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static AttributeGroupInfo ConvertToAttrGroupInfo(ServiceAttrGroupInfo info)
        {
            return new AttributeGroupInfo
            {
                GroupId = info.GroupID,
                MatchType = info.MatchType,
                GroupName = info.Name,
                OrderIndex = info.OrderIndex,
                IsMChoice = info.IsMChoice,
                IsPriorityFilter = info.IsPriorityFilter,
                EnabledFilter = info.EnabledFilter,
                Unit = info.Unit,
                IsUploadImage = info.IsUploadImage
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static ServiceAttrGroupInfo ConvertToGoodsSeviceFieldInfo(AttributeGroupInfo info)
        {
            return new ServiceAttrGroupInfo
            {
                GroupID = info.GroupId,
                MatchType = info.MatchType,
                Name = info.GroupName,
                OrderIndex = info.OrderIndex,
                IsMChoice = info.IsMChoice,
                IsPriorityFilter = info.IsPriorityFilter,
                EnabledFilter = info.EnabledFilter,
                Unit = info.Unit,
                IsUploadImage = info.IsUploadImage
            };
        }

        /// <summary>
        /// 转换成本地AttrWords
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static AttributeWordInfo ConvertToAttrWords(AttrWordsGridItemModel info)
        {
            return new AttributeWordInfo
            {
                WordId = info.WordID,
                GroupId = info.GroupID,
                Word = info.Word,
                OrderIndex = info.OrderIndex,
                CompareType = info.CompareType,
                WordValue = info.Value,
                TopValue = info.TopValue,
                IsShow = info.IsShow,
                GoodsQuantity = info.GoodsQuantity,
                AttrWordImage = info.AttrWordImage
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static ServiceAttrWordsInfo ConvertToServiceAttrWordsInfo(AttributeWordInfo info)
        {
            return new ServiceAttrWordsInfo
            {
                WordID = info.WordId,
                GroupID = info.GroupId,
                Word = info.Word,
                OrderIndex = info.OrderIndex,
                CompareType = info.CompareType,
                Value = info.WordValue,
                TopValue = info.TopValue,
                IsShow = info.IsShow,
                AttrWordImage = info.AttrWordImage
            };
        }

        #endregion

        #region [-- 属性组]

        /// <summary>新增高级属性组
        /// </summary>
        /// <param name="attrGroupInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddAttrGroup(AttributeGroupInfo attrGroupInfo, out string errorMessage)
        {
            var result = GoodsServerClient.AddAttrGroup(ConvertToGoodsSeviceFieldInfo(attrGroupInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary> 修改高级属性组
        /// </summary>
        /// <param name="attrGroupInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateAttrGroup(AttributeGroupInfo attrGroupInfo, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateAttrGroup(ConvertToGoodsSeviceFieldInfo(attrGroupInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary> 删除高级属性组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteAttrGroup(int id, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteAttrGroup(id);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>根据类型获取绑定的属性组
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeGroupInfo> GetAttrGroupList()
        {
            var result = GoodsServerClient.GetAllAttrGroup((int)AttrGroupModelType.AttrGroupInfo);
            if (result != null && result.IsSuccess)
            {
                var listResult = (ListResult<ServiceAttrGroupInfo>)result;
                var items = listResult.Data;
                foreach (var item in items)
                {
                    yield return ConvertToAttrGroupInfo(item);
                }
            }
        }

        /// <summary>根据类型获取绑定的属性组
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public IEnumerable<AttributeGroupInfo> GetAttrGroupList(int goodsType)
        {
            var result = GoodsServerClient.GetAttrGroupGoodsTypeGridDataSource(goodsType);
            if (result != null && result.IsSuccess)
            {
                var listResult = (ListResult<AttrGroupGoodsTypeGridItemModel>)result;
                var items = listResult.Data;
                foreach (var item in items)
                {
                    yield return ConvertToAttrGroupInfo(item);
                }
            }
        }

        #endregion

        #region [--属性词]

        /// <summary> 新增高级属性词
        /// </summary>
        /// <param name="attrWordsInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddAttrWords(AttributeWordInfo attrWordsInfo, out string errorMessage)
        {
            var result = GoodsServerClient.AddAttrWords(ConvertToServiceAttrWordsInfo(attrWordsInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary> 修改高级属性词
        /// </summary>
        /// <param name="attrWordsInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateAttrWords(AttributeWordInfo attrWordsInfo, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateAttrWords(ConvertToServiceAttrWordsInfo(attrWordsInfo));
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary> 删除高级属性词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteAttrWords(int id, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteAttrWords(id);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        #endregion

        /// <summary>根据GroupId获取高级属性词
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<AttributeWordInfo> GetAttrWordsListByGroupId(int groupId)
        {
            var result = GoodsServerClient.GetAttrWordsGridDataSource(groupId);
            if (result != null && result.IsSuccess)
            {
                var listResult = (ListResult<AttrWordsGridItemModel>)result;
                var items = listResult.Data;
                foreach (var item in items)
                {
                    yield return ConvertToAttrWords(item);
                }
            }
        }

        /// <summary> 获取高级属性词和商品(分页)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttrWordsGoodsInfo> GetAttrWordsGoodsListByPage(int wordId, int pageIndex, int pageSize, out int totalCount, out string errorMessage)
        {
            var list = new List<AttrWordsGoodsInfo>();
            totalCount = 0;
            var request = new AttrWordsGoodsRequest
            {
                WordID = wordId,
                Page = pageIndex,
                PageSize = pageSize
            };
            var result = GoodsServerClient.GetAttrWordsGoodsGridDataSource(request);
            if (result != null && result.IsSuccess)
            {
                var listResult = (AttrWordsGoods)result;
                var items = listResult.Data;
                totalCount = listResult.Total;
                var wordIdAndWordName = new Dictionary<int, string>();
                if (listResult.WordsList != null && listResult.WordsList.Count > 0)
                {
                    foreach (var erpAttrWordsListItemModel in listResult.WordsList)
                    {
                        wordIdAndWordName.Add(erpAttrWordsListItemModel.WordID, erpAttrWordsListItemModel.Word);
                    }
                }
                list.AddRange(items.Select(item => new AttrWordsGoodsInfo
                {
                    GoodsId = item.GoodsId, GoodsName = item.GoodsName, GoodsCode = item.GoodsCode, Value = item.Value, WordIdAndWordName = wordIdAndWordName
                }));
            }
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return list;
        }

        /// <summary>
        /// 更新商品与属性名称绑定关系
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="attrGroupId"></param>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool ChangeGoodsAttribute(Guid goodsId, int attrGroupId, string value, out string errorMessage)
        {
            var result = GoodsServerClient.ChangeGoodsAttribute(goodsId, attrGroupId, value);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>设置商品类型绑定高级属性组
        /// </summary>
        /// <param name="goodsType"></param>
        /// <param name="attrGroupList"></param>
        /// <param name="errorMessage"> </param>
        /// <returns></returns>
        public bool SetAttrGroupGoodsType(int goodsType, List<int> attrGroupList, out string errorMessage)
        {
            var result = GoodsServerClient.BindingAttrGroupGoodsType(goodsType, attrGroupList.ToArray());
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>根据商品ID获取商品属性
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public IEnumerable<AttributeInfo> GetAttributeListByGoodsId(Guid goodsId)
        {
            var list = new List<AttributeInfo>();
            var result = GoodsServerClient.GetAttributeListByGoodsId(goodsId);
            if (result != null && result.IsSuccess)
            {
                var listResult = (ListResult<GoodsAttributeSelectModel>)result;
                var items = listResult.Data ?? new List<GoodsAttributeSelectModel>();
                foreach (var item in items)
                {
                    var info = new AttributeInfo
                                   {
                                       GroupId = item.GroupId,
                                       GroupName = item.AttrGroupName,
                                       MatchType = item.MatchType,
                                       IsMChoice = item.IsMChoice,
                                       Value = item.Value,
                                       AttributeWordList = new List<AttributeWordInfo>()
                                   };
                    if (item.WordList != null)
                    {
                        var list2 = item.WordList.Select(erpAttrWordsListItemModel => new AttributeWordInfo
                                                                                          {
                                                                                              WordId = erpAttrWordsListItemModel.WordID,
                                                                                              Word = erpAttrWordsListItemModel.Word
                                                                                          }).ToList();
                        info.AttributeWordList = list2;
                    }
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>属性编辑
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="list"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool SetAttribute(Guid goodsId, List<AttributeInfo> list, out string failMessage)
        {
            var groupAndValueList = list.Select(info => new KeyValuePair<int, string>(info.GroupId, info.Value)).ToList();
            var result = GoodsServerClient.SaveGoodsAttribute(goodsId, groupAndValueList);
            if (result == null)
            {
                failMessage = "GMS链接异常";
                return false;
            }
            failMessage = result.ErrorMsg;
            return result.IsSuccess;
        }
    }
}
