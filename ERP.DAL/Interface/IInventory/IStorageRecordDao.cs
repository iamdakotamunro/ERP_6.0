using System;
using System.Collections.Generic;
using AllianceShop.Contract.DataTransferObject;
using ERP.Enum;
using ERP.Model;
using ERP.Model.Finance;
using ERP.Model.Goods;
using ERP.Model.Report;
using Keede.Ecsoft.Model.ShopFront;


namespace ERP.DAL.Interface.IInventory
{
    /// <summary>▄︻┻┳═一  商品出入库记录接口  最后修改提交 陈重文  2014-12-25   （更新、删除、优化方法）
    /// </summary>
    public interface IStorageRecordDao
    {

        /// <summary>获取每月月末库存记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(DateTime dayTime);

        /// <summary> 获取采购入库、拆分组合入库数据
        /// </summary>
        /// <param name="stockTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<MonthGoodsStockInfo> SelectGoodsStockInfos(List<int> stockTypes, DateTime startTime, DateTime endTime);

        /// <summary>从临时出入库表中获取销售数据
        /// </summary>
        /// <returns></returns>
        IList<TempStorageRecordDetailInfo> GeTempStorageRecordDetailInfos(DateTime startTime, DateTime endTime);

        /// <summary>根据“单据编号”获取数据
        /// </summary>
        /// <param name="tradeCode">单据编号(多个单据编号，用英文状态下的逗号隔开)</param>
        /// <returns></returns>
        /// zal 2016-01-21
        IList<StorageRecordInfo> GetStorageRecordList(string tradeCode);

        Boolean DeleteStorageRecord(Guid stockId, out string errorMessage);

        IList<GoodsOutInStorageStatisticsInfo> GetGoodsOutInStorageStatisticsList(int stockType, DateTime startTime, DateTime endTime, Guid companyId, int keepyear, Guid filialeId, Guid warehouseId);

        IList<GoodsSemiStockStateInfo> GetSemiGoodsQuanityWithState(string linkTradeCode, IList<Guid> goodsIdList);

        IList<GoodsStockRunning> GetStockRunning(Guid goodsId, DateTime starttime, DateTime endtime, Guid warehouseId, int keepyear, List<StorageRecordType> storageRecordTypes);

        bool IsExistNormalStorageRecord(Guid goodsId, List<Guid> realGoodsIds);


        Dictionary<Guid, String> GetStorageRecordStockIdAndTradeCodeDic(Guid companyId, StorageRecordType[] storageRecordTypes, String searchKey);

        Boolean SetStorageRecordDetailToJoinPrice(Guid stockId, Guid goodsId, Decimal joinPrice);

        Boolean IsExistNormalStorageRecord(String tradeCode);

        IList<StorageRecordInfo> GetStorageRecordByLinkTradeCode(String linkTradeCode);

        /// <summary> 根据门店采购申请编号获取(待审核)商品明细实际出库数量
        /// </summary>
        /// <param name="linkTradeCode">门店采购申请编号</param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="stockType">出入库类型</param>
        /// <param name="states">审核状态列表</param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        Dictionary<Guid, int> GetSendQuantityByLinkTradeCode(string linkTradeCode, Guid filialeId, Guid thirdCompanyId, int stockType, List<int> states);

        #region 新出入库使用方法
        /// <summary> 根据出入库记录ID获取出入库明细
        /// </summary>
        /// <param name="stockId">出入库记录ID</param>
        /// <returns></returns>
        IList<StorageRecordDetailInfo> GetStorageRecordDetailListByStockId(Guid stockId);

        IList<StorageRecordDetailInfo> GetStorageRecordDetailListByLinkTradeCode(string linkTradeCode);

        /// <summary>根据出入库记录ID获取出入库记录信息
        /// </summary>
        /// <param name="stockId">出入库记录Id</param>
        /// <returns></returns>
        StorageRecordInfo GetStorageRecord(Guid stockId);

        /// <summary>根据出入库单据号获取出入库记录信息
        /// </summary>
        /// <param name="tradeCode">出入库单据号</param>
        /// <returns></returns>
        StorageRecordInfo GetStorageRecord(String tradeCode);

        /// <summary>根据出入库单据号获取出入库记录信息
        /// </summary>
        /// <param name="billNo">WMS进出货单号</param>
        /// <param name="tradeCode">出入库单据号</param>
        /// <returns></returns>
        StorageRecordInfo GetStorageRecordByBillNoExcludeTradeCode(string billNo, string tradeCode);

        /// <summary>根据出入库来源单据ID和出入库单据状态获取其对应的出入库记录单据号集合
        /// </summary>
        /// <param name="linkTradeId"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        IList<String> GetStorageRecordTradeNos(Guid linkTradeId, StorageRecordState storageRecordState);


        /// <summary>根据出入库来源单据ID和出入库单据状态获取其对应的出入库记录集合
        /// </summary>
        /// <param name="linkTradeId"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        IList<StorageRecordInfo> GetStorageRecordsByLinkTradeId(Guid linkTradeId, StorageRecordState storageRecordState);


        /// <summary>根据出入库来源单据号和出入库单据状态获取其对应的出入库记录集合
        /// </summary>
        /// <param name="linkTradeNo"></param>
        /// <param name="storageRecordState"></param>
        /// <returns></returns>
        IList<StorageRecordInfo> GetStorageRecordsByLinkTradeNo(String linkTradeNo, StorageRecordState storageRecordState);


        /// <summary>根据来源单据号获取商品的出库数量
        /// </summary>
        /// <param name="linkTradeNo">来源单据号</param>
        /// <returns></returns>
        Dictionary<Guid, Int32> GetStorageRecordNormalGoodsQuantityByLinkTradeNo(String linkTradeNo);

        ///// <summary>多条件分页查询出入库记录
        ///// </summary>
        ///// <returns></returns>
        //IList<StorageRecordInfo> GetStorageRecordListToPage(Guid warehouseId, List<Guid> companyIds, String goodsName, String serarchKey, List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates, int storageType, Guid filialeId, DateTime startTime, DateTime endTime, int keepYear, int startPage, int pageSize, out long recordCount);

        IList<StorageRecordInfo> GetStorageRecordListToPages(Guid warehouseId, Guid companyId, string goodsName, string serarchKey,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates, int storageType,
            Guid filialeId, DateTime startTime, DateTime endTime, int mode,
            int keepYear, int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 根据入库仓库，托管公司，供应商获取出入库集合
        /// </summary>
        /// <param name="warehouseId">入库仓库</param>
        /// <param name="storageType">入库储</param>
        /// <param name="hostingFilialeId">托管公司</param>
        /// <param name="companyId">供应商</param>
        /// <param name="stockType">入库类型</param>
        /// <param name="stockState">单据状态</param>
        /// <param name="strKey">单据编号</param>
        /// <returns></returns>
        IList<StorageRecordInfo> GetStorageRecordListByWarehouseIdAndCompanyId(Guid warehouseId, int storageType,
            Guid hostingFilialeId, Guid companyId, int stockType, int stockState, string strKey);

        /// <summary>
        /// 根据入库仓库，供应商获取出入库集合
        /// </summary>
        /// <param name="warehouseId">入库仓库</param>
        /// <param name="companyId">供应商</param>
        /// <param name="stockType">入库类型</param>
        /// <param name="stockState">单据状态</param>
        /// <param name="strKey">单据编号</param>
        /// <returns></returns>
        IList<StorageRecordInfo> GetStorageRecordListByWarehouseAndCompany(Guid warehouseId, Guid companyId,
            int stockType, int stockState, string strKey);

        /// <summary>
        /// 获取出入库Id和出入库单号
        /// </summary>
        /// <param name="warehouseId">仓库Id</param>
        /// <param name="companyId">第三方公司Id</param>
        /// <param name="hostingFilialeId">托管公司Id</param>
        /// <param name="tradeCode">出入库单号</param>
        /// <returns></returns>
        /// zal 2017-02-07
        Dictionary<Guid, string> GetDicStockIdAndTradeCode(Guid warehouseId, Guid companyId, Guid hostingFilialeId, string tradeCode);

        /// <summary>
        /// 新增或修改最后一次进货价
        /// </summary>
        /// <param name="goodsPurchaseLastPriceInfo">最后一次进货价模型</param>
        Boolean InsertLastPrice(GoodsPurchaseLastPriceInfo goodsPurchaseLastPriceInfo);

        /// <summary>
        /// 获取商品的最后一次进货价信息
        /// </summary>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        List<GoodsPurchaseLastPriceInfo> GetGoodsPurchaseLastPriceInfoByWarehouseId(Guid warehouseId);

        /// <summary>新增出入库单据含明细
        /// </summary>
        /// <param name="storageRecordInfo">出入库单据模型</param>
        /// <param name="storageRecordDetailList">出入库单据详细模型</param>
        /// <param name="errorMessage">异常信息</param>
        bool Insert(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList, out string errorMessage);

        /// <summary>更新出入库单据状态和描述
        /// </summary>
        /// ADD ww  
        /// 2016-08-01
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="description">描述</param>
        bool NewSetStateStorageRecord(Guid stockId, StorageRecordState state, string description);

        /// <summary>
        /// 更新出入库单据状态、单据总额、描述
        /// </summary>
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="accountReceivable">单据总额</param>
        /// <param name="description">描述</param>
        /// zal 2017-08-17
        bool SetStateAndAccountReceivableForStorageRecord(Guid stockId, StorageRecordState state, decimal accountReceivable, string description);

        /// <summary>
        /// 新增出入库记录（含判断是否存在）
        /// ADD ww
        /// 2016-07-29
        /// </summary>
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailList">出入库详细记录</param>
        bool NewSaveStoreRecord(StorageRecordInfo storageRecordInfo,
            IList<StorageRecordDetailInfo> storageRecordDetailList);

        /// <summary>
        /// 新增出入库记录（外层必须使用事物）
        /// </summary>
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailList">出入库详细记录</param>
        bool NewSaveStoreRecordNoTrans(StorageRecordInfo storageRecordInfo,
            IList<StorageRecordDetailInfo> storageRecordDetailList);

        /// <summary>
        /// 添加出入库记录（明细采用SQL批量插入）
        /// </summary>
        /// <param name="storageRecordInfo"></param>
        /// <param name="storageRecordDetailList"></param>
        bool AddStorageRecord(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList);

        /// <summary>
        /// 更新出入库记录和出入库库明细
        /// </summary>
        /// ADD ww 
        /// 2016-07-29
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailInfoList">出入库明细</param>
        void NewUpdateStockAndGoods(StorageRecordInfo storageRecordInfo,
            IList<StorageRecordDetailInfo> storageRecordDetailInfoList);

        /// <summary>
        /// 进货/出货 单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        bool RefuseInorOutGoodsBill(string no, string description);

        /// <summary>
        /// 进货/出货 单据核退（更新ERP出入库单据状态）。
        /// </summary>
        /// <param name="tradeCode">单号</param>
        /// <param name="storageState"></param>
        /// <param name="description">描述</param>
        /// <param name="errorMsg"></param>
        /// For WMS
        /// <returns></returns>
        bool UpdateStorageState(string tradeCode, int storageState, string description, out string errorMsg);

        /// <summary>
        /// 获取商品的未出库数(商品需求查询页面使用)
        /// ADD ww
        /// 2016-08-17
        /// </summary>
        /// <param name="goodsId">商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        int GetNotOutQuantity(Guid goodsId, Guid filialeId, Guid warehouseId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates);

        /// <summary>
        /// 获取商品的未出库数(商品需求查询页面使用)
        /// </summary>
        /// <param name="realGoodsIdIds">子商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        /// zal 2017-03-03
        Dictionary<Guid, int> GetDicNotOutQuantity(List<Guid> realGoodsIdIds, Guid filialeId, Guid warehouseId, List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates);
        /// <summary>
        /// <summary>
        /// 获取未出库数出库单信息(商品需求查询页面使用)
        /// ADD ww
        /// 2016-08-19
        /// </summary>
        /// <param name="goodsId">商品id</param>
        /// <param name="storageRecordTypes">入库类型</param>
        /// <param name="storageRecordStates">入库状态</param>
        /// <param name="filialeId">公司id</param>
        /// <param name="warehouseId">仓库id</param>
        /// <returns></returns>
        List<StorageRecordInfo> GetStorageRecordListByNotOutQuantity(Guid goodsId, Guid filialeId, Guid warehouseId,
        List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates);

        /// <summary>退货验证状态更改
        /// </summary>
        /// <param name="stockId">出入库记录ID</param>
        /// <param name="stockValidationType">是否验证</param>
        void UpdateStorageRecordDetailByStockId(Guid stockId, bool stockValidationType);

        Boolean UpdateNonCurrentStockByStockId(Guid stockId, Dictionary<Guid, int> stockQuantitys);

        bool UpdateNonCurrentStockByRealGoodsId(Guid stockId, Guid realGoodsId, int quantity);

        List<StorageRecordInfo> GetStorageRecordList(Guid warehouseId, Guid filialeId, int storageRecordType,
            int storageRecordState, int storageType);

        Dictionary<Guid, int> GetReturnRealGoods(Guid warehouseId, int storageRecordType, int storageType, List<int> storageRecordStates, Guid realGoodsId);

        #endregion

        bool SetBillNo(Guid stockId, string billNo);

        /// <summary>
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="stockType">关联单号的进出库类型</param>
        /// <param name="currentTradeCode">编辑时当前单据号</param>
        /// <returns></returns>
        Dictionary<Guid, int> GetEffictiveSellReturn(string tradeCode, int stockType, string currentTradeCode);

        Dictionary<Guid, int> GetAllReturnRealGoods(Guid warehouseId, int storageRecordType, int storageType, List<int> storageRecordStates);

        bool IsAbeyancedThirdComapny(string tradeCode);

        IList<Guid> GetGoodsIdListFromStorageRecordDetail(string tradeCode);
        /// <summary>
        /// 获取出货单内部采购关联表
        /// </summary>
        /// <param name="outStockId">出库ID</param>
        /// <returns></returns>
        InnerPurchaseRelationInfo GetInnerPurchaseRelationInfo(Guid outStockId);

        /// <summary>
        /// 获取入库单内部采购关联表
        /// </summary>
        /// <param name="inStockId">出库ID</param>
        /// <returns></returns>
        InnerPurchaseRelationInfo GetInnerPurchaseRelationInfoIn(Guid inStockId);

        /// <summary>
        /// 更新入库单内部采购关联表
        /// </summary>
        /// <param name="inStockId">出库ID</param>
        /// <returns></returns>
        void UpdateInnerPurchaseRelationInfo(Guid outStockId, Guid inStockId, Guid purchasingId, Guid outWarehouseId, Guid outHostingFilialeId, int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType);

        void InsertInnerPurchaseRelationInfo(Guid outStockId, Guid inStockId, Guid purchasingId, Guid outWarehouseId,
            Guid outHostingFilialeId, int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType);

        void UpdateEditInnerPurchaseRelationInfo(Guid outStockId, Guid outWarehouseId, Guid outHostingFilialeId,
            int outStorageType, Guid inWarehouseId, Guid inHostingFilialeId, int inStorageType);

        bool DeleteStorageRecordByLinkTradeCode(string orderNo, out string errorMessage);

        bool CancelStorageRecordByLinkTradeCode(string orderNo, out string errorMessage);

        bool UpdateStockPurchse(StorageRecordInfo storageRecordInfo);

        /// <summary>
        /// 获取物流公司销售给销售公司商品及均价
        /// </summary>
        /// <param name="hostingFilialeId"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetSellOutGoodsUnitPriceDic(Guid hostingFilialeId, Guid saleFilialeId, string billNo);

        Dictionary<Guid, int> GetCancelRealGoods(Guid warehouseId, int storageRecordType, int storageType,
            int storageRecordState, Guid realGoodsId);

        #region

        /// <summary>
        /// 查找指定时间段内物流公司的销售出库单据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="storageRecordType"></param>
        /// <param name="storageRecordState"></param>
        /// <param name="linkTradeType"></param>
        /// <returns></returns>
        List<StorageRecordInfo> GetStorageRecordList(DateTime startTime, DateTime endTime, int storageRecordType,
            int storageRecordState, int linkTradeType);

        /// <summary>
        /// 查找指定时间段内物流公司的销售出库单据明细
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="storageRecordType"></param>
        /// <param name="storageRecordState"></param>
        /// <param name="linkTradeType"></param>
        /// <returns></returns>
        List<StorageRecordDetailInfo> GetStorageRecordDetailList(DateTime startTime, DateTime endTime,
            int storageRecordType, int storageRecordState, int linkTradeType);

        #endregion

        #region

        List<StockBillDTO> GetStockBillDtos(DateTime startTime,DateTime endTime,int stockState,IEnumerable<int> stockTypes);

        #endregion

        #region
        List<SourceBindGoods> GetStorageRecordDetailsByLinkTradeCode(IEnumerable<string> linkTradeCodes); 
        #endregion
    }
}
