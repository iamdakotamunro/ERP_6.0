using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        #region [-- 属性组]

        /// <summary>新增高级属性组
        /// </summary>
        /// <param name="attrGroupInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddAttrGroup(AttributeGroupInfo attrGroupInfo, out string errorMessage);

        /// <summary> 修改高级属性组
        /// </summary>
        /// <param name="attrGroupInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateAttrGroup(AttributeGroupInfo attrGroupInfo, out string errorMessage);

        /// <summary> 删除高级属性组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteAttrGroup(int id, out string errorMessage);

        /// <summary>根据类型获取绑定的属性组
        /// </summary>
        /// <returns></returns>
        IEnumerable<AttributeGroupInfo> GetAttrGroupList();

        /// <summary>根据类型获取绑定的属性组
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        IEnumerable<AttributeGroupInfo> GetAttrGroupList(int goodsType);

        #endregion

        #region [--属性词]

        /// <summary> 新增高级属性词
        /// </summary>
        /// <param name="attrWordsInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddAttrWords(AttributeWordInfo attrWordsInfo, out string errorMessage);

        /// <summary> 修改高级属性词
        /// </summary>
        /// <param name="attrWordsInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateAttrWords(AttributeWordInfo attrWordsInfo, out string errorMessage);

        /// <summary> 删除高级属性词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteAttrWords(int id, out string errorMessage);

        #endregion

        /// <summary>根据GroupId获取高级属性词
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IEnumerable<AttributeWordInfo> GetAttrWordsListByGroupId(int groupId);

        /// <summary> 获取高级属性词和商品(分页)
        /// </summary>
        /// <returns></returns>
        IEnumerable<AttrWordsGoodsInfo> GetAttrWordsGoodsListByPage(int wordId, int pageIndex,
            int pageSize, out int totalCount, out string errorMessage);

        /// <summary>
        /// 更新商品与属性名称绑定关系
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="attrGroupId"></param>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool ChangeGoodsAttribute(Guid goodsId, int attrGroupId, string value, out string errorMessage);

        /// <summary>设置商品类型绑定高级属性组
        /// </summary>
        /// <param name="goodsType"></param>
        /// <param name="attrGroupList"></param>
        /// <param name="errorMessage"> </param>
        /// <returns></returns>
        bool SetAttrGroupGoodsType(int goodsType, List<int> attrGroupList, out string errorMessage);

        /// <summary>根据商品ID获取商品属性
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        IEnumerable<AttributeInfo> GetAttributeListByGoodsId(Guid goodsId);


        /// <summary>属性编辑
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="list"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
       bool SetAttribute(Guid goodsId, List<AttributeInfo> list, out string failMessage);
    }
}
