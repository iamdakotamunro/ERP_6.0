using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {

        #region --> 生成子商品

        /// <summary>
        /// 生成子商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="fieldList"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        bool CreatRealGoods(Guid goodsId, Dictionary<Guid, List<Guid>> fieldList, out string failMessage);
        #endregion

        #region --> 添加主商品

        /// <summary>
        /// 添加主商品
        /// </summary>
        /// <param name="goodsInfo"></param>
        /// <param name="fields"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        bool AddGoods(GoodsInfo goodsInfo, Dictionary<Guid, List<Guid>> fields, out string failMessage);
        #endregion

        #region --> 修改主商品信息

        /// <summary>
        /// 修改主商品信息
        /// </summary>
        /// <param name="goodsInfo"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        bool UpdateGoods(GoodsInfo goodsInfo, out string failMessage);

        /// <summary>
        /// 修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// zal 2016-04-21
        bool UpdateGoodsAuditStateAndAuditStateMemo(Guid goodsId, int goodsState, string goodsStateMemo, out string failMessage);


        /// <summary>
        /// 批量修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id集合</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// ww 2016-06-29
        bool PlUpdateGoodsAuditStateAndAuditStateMemo(List<Guid> goodsId, int goodsState, string goodsStateMemo,
            out string failMessage);

    
        #endregion

        #region --> 删除主商品信息

        /// <summary>
        /// 删除主商品信息
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        bool DeleteGoods 
        (List<Guid> goodsIds, string operatorName, Guid personId, out string errorMessage);

        #endregion

        #region --> 获取主商品信息

        /// <summary>
        /// 获取主商品信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        GoodsInfo GetGoodsBaseInfoById (Guid goodsId);
        
        Dictionary<Guid, RealGoodsUnitModel> GetDictRealGoodsUnitModel(List<Guid> realGoodsIds);
        #endregion

        #region --> 根据子商品或无属性商品，获取主商品信息

        /// <summary>
        /// 根据子商品或无属性商品，获取主商品信息
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        GoodsInfo GetGoodsBaseInfoByRealGoodsId 
        (Guid
        realGoodsId)
        ;

        #endregion

        /// <summary>
        /// Use New ServiceClient
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <returns></returns>
        Dictionary<Guid, GoodsInfo> GetGoodsBaseListByGoodsIdOrRealGoodsIdList 
        (List < Guid > realGoodsIdList);

        /// <summary>
        /// Use New ServiceClient
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="goodsIdList"></param>
        /// <returns></returns>
        /// zal 2017-06-28
        List<GoodsSeriesModel> GetGoodsSeriesList(Guid saleFilialeId, List<Guid> goodsIdList);

            #region --> 根据主商品名称或主商品Code获取主商品

            /// <summary>
            /// 根据商品名称或商品Code获取商品
            /// </summary>
            /// <param name="goodsCode"></param>
            /// <returns></returns>
            GoodsInfo GetGoodsBaseInfoByCode 
        (string
        goodsCode)
        ;

        #endregion

        #region --> 获取主商品详细信息

        /// <summary>
        /// 获取主商品详细信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        GoodsInfo GetGoodsDetailById 
        (Guid
        goodsId)
        ;

        #endregion

        #region --> 搜索商品信息(分页)

        /// <summary>
        /// 搜索商品信息(分页)
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="hasInformation"></param>
        /// <param name="saleStockType"></param>
        /// <param name="goodsAuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<GoodsInfo> GetGoodsListToPage 
        (Guid classId, string goodsNameOrCode, bool? hasInformation,
            int? saleStockType, int? goodsAuditState, int pageIndex, int pageSize, out int totalCount);

        #endregion

        #region --> 根据主商品名称或主商品编号获取子商品ID集合

        /// <summary>根据主商品名称或主商品编号获取子商品ID集合
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        IList<Guid> GetRealGoodsIdListByGoodsNameOrCode 
        (string
        goodsNameOrCode)
        ;

        #endregion

        #region --> 设置主商品是否缺货

        /// <summary>
        /// 设置商品是否缺货
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="isScarcity"></param>
        /// <param name="operatorName"> </param>
        /// <param name="personId"> </param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool SetGoodsIsScarcity 
        (Guid goodsId, bool isScarcity, string operatorName, Guid personId, out string errorMsg);

        #endregion

        #region --> 设置子商品是否缺货

        /// <summary>
        /// 设置子商品是否缺货
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="isScarcity"></param>
        /// <param name="operatorName"> </param>
        /// <param name="personId"> </param>
        /// <returns></returns>
        bool SetRealGoodsIsScarcity 
        (Guid realGoodsId, bool isScarcity, string operatorName, Guid personId);

        #endregion

        #region --> 设置主商品采购状态

        /// <summary>
        /// 设置主商品采购状态
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="isOnShelf"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        bool SetPurchaseState 
        (Guid goodsId, bool isOnShelf, string operatorName, Guid personId,
            out string errorMessage);

        #endregion

        #region --> 获取主商品简单键值对集合

        /// <summary>
        /// 获取主商品简单键值对集合
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> GetGoodsSelectList 
        (string
        goodsNameOrCode)
        ;

        #endregion

        #region --> 指定商品类型，获取主商品简单键值对集合

        /// <summary>
        /// 指定商品类型，获取主商品简单键值对集合
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        IDictionary<string, string> GetGoodsSelectList 
        (string goodsNameOrCode, int goodsType);

        #endregion

        #region --> 根据主商品ID和属性ID获取子商品

        /// <summary>
        /// 根据主商品ID和属性ID获取子商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        IList<ChildGoodsInfo> GetRealGoodsListByGoodsIdAndFields 
        (Guid goodsId,
            Dictionary<Guid, List<Guid>> fieldList);

        #endregion

        #region --> 根据商品分类ID和采购状态查询主商品

        /// <summary>
        /// 根据商品分类ID和采购状态查询主商品
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"> </param>
        /// <returns></returns>
        IList<GoodsInfo> GetGoodsBaseInfoListByClassId 
        (Guid classId, string goodsNameOrCode);

        #endregion

        /// <summary> 根据商品分类ID和采购状态查询主商品GoodsId/GoodsName/GoodsCode
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"> </param>
        /// <returns></returns>
        IList<GoodsInfo> GetGoodsInfoListSimpleByClassId 
        (Guid classId, string goodsNameOrCode);

        #region --> 根据主商品ID获取其所有子商品信息

        /// <summary>根据主商品ID获取所有子商品
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        IEnumerable<ChildGoodsInfo> GetRealGoodsListByGoodsId 
        (List < Guid > goodsIds);

        #endregion

        #region --> 获取子商品的基本信息

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        ChildGoodsInfo GetChildGoodsInfo 
        (Guid
        realGoodsId)
        ;

        #endregion

        #region --> 修改子商品信息

        bool UpdateChildGoodsInfo 
        (ChildGoodsInfo childGoodsInfo, string operatorName, Guid personId,
            out string errorMessage);

        #endregion

        #region -->还原商品添加时的数据（修改之前用）

        GoodsInfo GetGoodsInfoBeforeUpdate 
        (Guid
        goodsId)
        ;

        #endregion

        #region --> 根据主商品ID集合获取主商品信息

        /// <summary>根据主商品ID集合获取主商品信息
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        IList<GoodsInfo> GetGoodsListByGoodsIds 
        (List < Guid > goodsIds);

        #endregion

        /// <summary> 商品转移
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="classId"></param>
        /// <param name="failMessage"></param>
        bool UpdateGoodsClass 
        (Guid goodsId, Guid classId, out string failMessage);

        /// <summary> 根据主商品(GoodsId)获取子商品(RealGoodsId)集合
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        IEnumerable<Guid> GetRealGoodsIdsByGoodsId 
        (Guid
        goodsId)
        ;

        bool GetRealGoodsGoodsEditRequestModel 
        (Guid goodsId, out List<GoodsFieldInfo> fieldList,
            out List<GoodsFieldInfo> selectedFieldList, out string errorMessage);

        IEnumerable<ChildGoodsInfo> GetRealGoodsListByPage 
        (Guid goodsId, List<Guid> fieldList, int pageIndex,
            int pageSize, out int totalCount);

        IEnumerable<GoodsInfo> GetGoodsStockGridList 
        (Guid classId, string goodsNameOrGoodsCode,
            List<Guid> goodsIdList, int? page, int? pageSize, out int totalCount);

        IEnumerable<ChildGoodsInfo> GetStockDeclareGridList 
        (List < Guid > realgoodsIds);

        #region--> 商品价格查询

        /// <summary>商品价格查询  
        /// </summary>
        /// <param name="classId">商品分类Id</param>
        /// <param name="brandId">品牌Id</param>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        IList<GoodsPriceSerachInfo> GetGoodsPriceGridByPage 
        (Guid? classId, Guid? brandId, string goodsNameOrCode,
            int pageIndex, int pageSize, out int totalCount);

        #endregion

        #region [赠品绑定页面使用的一些方法 ]

        /// <summary>设置商品的赠品   
        /// </summary>
        /// <param name="goodsGiftList">商品及赠品集合</param>
        /// <returns>true/false</returns>
        bool SetGoodsGift 
        (Dictionary < Guid,
        List < Guid >> goodsGiftList)
        ;

        /// <summary>获取商品的赠品   
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <returns>key:赠品商品Id，value:赠品商品名称</returns>
        Dictionary<Guid, string> GetGoodsGiftList 
        (Guid
        goodsId)
        ;

        /// <summary>获取绑定过该赠品的商品    
        /// </summary>
        /// <param name="giftGoodsId">赠品商品Id</param>
        /// <returns>key:商品Id，value:商品名称</returns>
        Dictionary<Guid, string> GetGoodsListByGiftID 
        (Guid
        giftGoodsId)
        ;

        /// <summary>获取所有绑定过的赠品    
        /// </summary>
        /// <returns>key:赠品商品Id，value:赠品商品名称</returns>
        Dictionary<Guid, string> GetAllGiftList 
        ()
        ;

        /// <summary>导出商品及赠品  
        /// </summary>
        /// <param name="goodsKindType">商品类型</param>
        /// <param name="brandId">商品品牌Id</param>
        /// <param name="goodsNameOrCode">商品名称或商品编号</param>
        Dictionary<GoodsBaseModel, List<GoodsBaseModel>> GetGoodsListAndGiftList 
        (int? goodsKindType,
            Guid? brandId, string goodsNameOrCode);

        /// <summary>根据商品类型和商品品牌和商品名称及编号搜索商品   
        /// </summary>
        /// <param name="goodsKindType">商品类型</param>
        /// <param name="brandId">商品品牌ID</param>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        List<GoodsBaseModel> GetGoodsItemList 
        (int? goodsKindType, Guid? brandId, string goodsNameOrCode);

        #endregion

        /// <summary>根据主商品Id 集合获取商品的价格  
        /// </summary>
        /// <param name="goodsIds">主商品Id集合</param>
        /// <returns></returns>
        IList<GoodsPriceSerachInfo> GetGoodsPriceByGoodsIds 
        (List < Guid > goodsIds);

        IList<Guid> GetGoodsIDListByBrandID 
        (Guid
        brandId)
        ;

        /// <summary>根据商品分类ID，商品ID集合，商品名称/编号，是否统计绩效获取商品信息（库存周转率查询使用）
        /// </summary>
        /// <param name="classId">商品分类</param>
        /// <param name="goodsIds">商品ID集合</param>
        /// <param name="goodsNameOrCode">商品名称/编号</param>
        /// <param name="isStatisticalPerformance">是否统计绩效</param>
        /// <returns></returns>
        List<GoodsPerformance> GetGoodsPerformanceList 
        (Guid classId, List<Guid> goodsIds, String goodsNameOrCode,
            Boolean isStatisticalPerformance);

        /// <summary>
        /// 从商品中心获取平台及其对应的第三销售平台信息
        /// </summary>
        /// <returns></returns>
        IList<GroupDetailModel> GetGroupDetailList 
        ()
        ;

        /// <summary>
        /// 根据商品Id列表获取商品ID和该商品对应的平台价格信息列表的字典
        /// </summary>
        /// <param name="guidList">商品Id列表</param>
        /// <returns></returns>
        Dictionary<Guid, List<GroupGoodsPriceModel>> GetGroupGoodsPriceDictionary 
        (List < Guid > guidList);

        /// <summary>
        /// 获取商品主图
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isGoodsId"></param>
        /// <returns></returns>
        string GetProductFilesImage 
        (Guid id, bool isGoodsId);

        /// <summary>根据商品名称及编号搜索商品   ADD  2016-06-12    文雯
        /// </summary>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        List<GoodsBaseModel> GetGoodsItemListByGoodsNameOrCode 
        (string
        goodsNameOrCode)
        ;
    }
    }
