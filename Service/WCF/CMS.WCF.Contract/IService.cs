using System;
using System.Collections.Generic;
using System.ServiceModel;
using ERP.Enum;
using ERP.Model;
using ERP.Model.Goods;
using Framework;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;
using ERP.Model.RefundsMoney;
using ERP.Model.SubsidyPayment;

namespace ERP.Service.Contract
{
    /// <summary>
    ///
    /// </summary>
    [ServiceContract]
    public partial interface IService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="wasteBookInfo"></param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        [OperationContract]
        bool InsertWastebook(Guid pushDataId, WasteBookInfo wasteBookInfo);

        /// <summary>
        /// 插入一条资金流数据
        /// </summary>
        /// <param name="wasteBookInfo">资金流参数</param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        /// zal 2016-06-15
        [OperationContract]
        bool InsertWastebookForB2C(Guid pushDataId, WasteBookInfo wasteBookInfo);

        /// <summary>
        /// 设置商品上下架
        /// Add by liucaijun at 2011-March-25th
        /// </summary>
        /// <param name="personnelId"> </param>
        /// <param name="pushDataId">命令ID</param>
        /// <param name="goodsList">商品列表</param>
        /// <param name="state">上下架状态</param>
        [OperationContract]
        WCFReturnInfo SetGoodsState(Guid personnelId, Guid pushDataId, List<Guid> goodsList, int state);

        /// <summary>
        /// 设置商品是否缺货
        /// Add by liucaijun at 2011-March-25th
        /// </summary>
        /// <param name="pushDataId">命令ID</param>
        /// <param name="realGoodsList">商品列表</param>
        /// <param name="state">上下架状态</param>
        [OperationContract]
        WCFReturnInfo SetGoodsIsScarcityState(Guid pushDataId, List<Guid> realGoodsList, int state);

        /// <summary>
        /// 根据商品唯一标识获取 商品上下架状态
        /// </summary>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        [OperationContract]
        int GetGoodsState(Guid goodsid);

        /// <summary>
        /// 得到商品销量列表
        /// zhangfan created at 2012-July-12th
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 新增门店采购申请单
        /// </summary>
        /// <param name="applyStockContent"></param>
        /// <param name="applyStockDetailContent"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddShopFrontApplyStock(string applyStockContent, string applyStockDetailContent);

        /// <summary>
        /// 根据商品ID设置商品是否缺货
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="isScarcity">是否缺货</param>
        /// <param name="state"></param>
        [OperationContract]
        WCFReturnInfo UpdateGoodsISScarcity(Guid pushDataId, Guid goodsId, int isScarcity, int state);

        /// <summary>
        /// 设置子商品是否缺货
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="isScarcity"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateRealGoodsISScarcity(Guid pushDataId, Guid realGoodsId, int isScarcity);

        /// <summary>
        /// 更新申请单状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateApplyStockState(Guid applyId, int state);

        /// <summary>添加往来账
        /// </summary>
        /// <param name="reckoningContent"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddReckoningInfo(string reckoningContent);

        /// <summary> 添加帐务记录数据
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="info"></param>
        [OperationContract]
        WCFReturnInfo InsertReckoningInfo(Guid pushDataId, ReckoningInfo info);

        /// <summary>
        /// 修改往来账对账类型
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="linkTradeType">对应单据类型</param>
        /// <param name="reckoningType">账单类型：0收入，1支出</param>
        /// <param name="reckoningCheckType">往来账对账类型</param>
        /// <param name="isChecked">对账类型 1 已对账 0 未对账 2 异常对账</param>
        /// zal 2016-06-05
        [OperationContract]
        bool UpdateCheckState(Guid pushDataId, string linkTradeCode, int linkTradeType, int reckoningType, int reckoningCheckType, int isChecked);

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetCompanyCussentList(int companyType);

        /// <summary>获取公司下银行账号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<BankAccountInfo> GetBankAccountList(Guid filialeId);

        /// <summary>
        /// 对帐
        /// </summary>
        /// <param name="lstModify"></param>
        /// <param name="lstAdd"></param>
        /// <param name="wastBookinfo"></param>
        [OperationContract]
        void Checking(IList<ReckoningInfo> lstModify, IList<ReckoningInfo> lstAdd, WasteBookInfo wastBookinfo);

        /// <summary>
        /// 指定快递单号获取往来单位帐
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="originalTradeCode"></param>
        /// <param name="checkType"></param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        [OperationContract]
        ReckoningInfo GetReckoningInfo(Guid companyId, string originalTradeCode, CheckType checkType, int reckoningCheckType = 2);

        // 增加往来单位类型字段搜索
        /// <summary>
        /// 按是否对账,日期,账单类型获取往来账（未对账）
        /// 用于列表显示
        /// </summary>
        /// <param name="companyClass">往来单位类型</param>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="filialeId">公司ID </param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cType"></param>
        /// <param name="auditingState"></param>
        /// <param name="receiptType"></param>
        /// <param name="tradeCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="money"></param>
        /// <param name="start"> </param>
        /// <param name="limit"> </param>
        /// <returns></returns>
        [OperationContract]
        DataListInfo<ReckoningInfo> GetValidateDataPage(Guid companyClass, Guid companyId, Guid filialeId, DateTime startDate,
                                             DateTime endDate, CheckType cType,
                                             AuditingState auditingState, ReceiptType receiptType, String tradeCode,
                                             Guid warehouseId, int keepyear, long start, int limit, params int[] money);

        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        [OperationContract]
        IList<BankAccountInfo> GetBankAccountsListByFBP(Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList();

        /// <summary>
        /// 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        [OperationContract]
        double GetBankAccountsNonce(Guid bankAccountsId);

        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<BankAccountInfo> GetBankAccountsListFromERP();

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="startDate"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <returns></returns>
        [OperationContract]
        IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                                     ReceiptType receiptType,
                                                     AuditingState auditingState, double minIncome, double maxIncome,
                                                     String tradeCode, Guid filialeId, Guid branchId, Guid positionId,
                                                     int keepyear);

        /// <summary>
        /// 根据条件获取账目信息(分页)
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="startDate"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Dictionary<int, IList<WasteBookInfo>> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, String tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int startPage, int pageSize);

        /// <summary> 根据条件获取账目信息
        /// </summary>
        /// <param name="saleFilialeId">公司Id</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <param name="isCheck"> </param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Dictionary<int, IList<WasteBookInfo>> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize);

        /// <summary>
        /// 获取指定tradecode在表中的记录数
        /// </summary>
        /// <param name="tradecode"></param>
        /// <returns></returns>
        [OperationContract]
        decimal GetTradeCodeNum(string tradecode);

        /// <summary>
        /// 获取手续费（为reckoning表修改记录用）
        /// </summary>
        /// <param name="tradecode"></param>
        /// <returns></returns>
        [OperationContract]
        decimal GetPoundageForReckoning(string tradecode);

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWasteBookId(string tradeCode);

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWasteBookIdForUpdate(string tradeCode);

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWasteBookIdForReckoning(string tradeCode);

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="wasteBookId"></param>
        /// <returns></returns>
        [OperationContract]
        WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId);

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="wasteBookId"></param>
        /// <returns></returns>
        [OperationContract]
        WasteBookInfo GetWasteBook(Guid wasteBookId);

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="linkTradeCode">关联单据编号</param>
        /// <param name="wasteSource">1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额</param>
        /// <returns></returns>
        /// zal 2016-06-15
        [OperationContract]
        WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource);

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [OperationContract]
        WasteTypeInfo GetWasteBookInfo(string tradeCode);

        /// <summary>
        /// 根据条件获取子往来单位列表
        /// </summary>
        /// <param name="companyClassId"> </param>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyClassInfo> GetChildCompanyClassList(Guid companyClassId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyClassId"> </param>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyCussentInfo> GetCompanyCussentListByClass(Guid companyClassId);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyCussentInfo> GetAllCompanyCussentList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyId"> </param>
        /// <returns></returns>
        [OperationContract]
        CompanyCussentInfo GetCompanyCussent(Guid companyId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyId"> </param>
        /// <returns></returns>
        [OperationContract]
        double GetNonceReckoningTotalled(Guid companyId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyId"> </param>
        /// <param name="filialeId"> </param>
        /// <returns></returns>
        [OperationContract]
        double GetNonceReckoningTotalledDetail(Guid companyId, Guid filialeId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="reckoningId"> </param>
        /// <returns></returns>
        [OperationContract]
        ReckoningInfo GetReckoning(Guid reckoningId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="reckoningId"> </param>
        /// <returns></returns>
        [OperationContract]
        int GetReckoningType(Guid reckoningId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyId"> </param>
        /// <returns></returns>
        [OperationContract]
        decimal GetTotalled(Guid companyId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        decimal GetPoundage(string tradeCode);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool CheckPoundage(decimal sum, decimal poundage);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="reckoningInfo"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateReckoning(Guid pushDataId, ReckoningInfo reckoningInfo);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="wasteBookId"> </param>
        /// <param name="description"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBookDescription(Guid pushDataId, Guid wasteBookId, String description);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <param name="description"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateDescriptionForAuditing(Guid pushDataId, string tradeCode, String description);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="reckongId"> </param>
        /// <param name="description"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateReckoningDescription(Guid pushDataId, Guid reckongId, String description);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AuditingReckoning(Guid pushDataId, string tradeCode);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeleteReckoning(Guid pushDataId, string tradeCode);

        /// <summary> 获取编码
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetCode(CodeType codeType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="wasteBookInfo"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBook(Guid pushDataId, WasteBookInfo wasteBookInfo);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="wasteBookInfo"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBookWithBank(Guid pushDataId, WasteBookInfo wasteBookInfo);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBookDateTime(Guid pushDataId, string tradeCode);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="wastebookId"> </param>
        /// <param name="dateCreated"> </param>
        /// <param name="poundage"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdatePoundageForReckoning(Guid pushDataId, Guid wastebookId, DateTime dateCreated, decimal poundage);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeleteWasteBook(Guid pushDataId, string tradeCode);

        /// <summary>
        ///
        /// </summary>
        /// <param name="poundage"> </param>
        /// <param name="pushDataId"> </param>
        /// <param name="tradeCode"> </param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeleteWasteBookPoundage(Guid pushDataId, string tradeCode, decimal poundage);

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBookDescriptionForAuditing(Guid pushDataId, string tradeCode, String description);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AuditingWasteBook(Guid pushDataId, string tradeCode);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateWasteBookCheck(Guid pushDataId, WasteBookCheckInfo checkInfo);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo InsertWasteBookCheck(Guid pushDataId, WasteBookCheckInfo checkInfo);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateBll(Guid pushDataId, Guid outWasteBookId, string description, decimal income, string tradecode, decimal poundage, Guid bankAccountsId);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateBankAccountsId(Guid pushDataId, Guid wasteBookId, Guid bankAccountsId);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo InsertPoundage(Guid pushDataId, Guid outBankAccountsId, string tradeCode, decimal poundage, Guid filialeId);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdatePoundage(Guid pushDataId, string tradeCode, decimal poundage);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo Virement(Guid pushDataId, Guid inBankAccountsId, Guid outBankAccountsId, decimal sum, decimal poundage, string description, string tradeCode, Guid filialeId);

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AuditingWaste(Guid pushDataId, string tradeCode);

        /// <summary>
        /// 获取商品的预计到货时间
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        [OperationContract]
        DateTime GetGoodsPredictArrivalTime(Guid realGoodsId);

        /// <summary>
        /// 获取商品条码信息
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        [OperationContract]
        GoodsBarcodeInfo GetGoodsBarcodeInfo(string goodsCode);

        /// <summary> 根据商品ID获取会员ID集合
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Guid> GetMemberIdListByRealGoodsIds(DateTime startTime, DateTime endTime, List<Guid> realGoodsIds);

        /// <summary>
        /// 售后日销量添加,修改
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="orderId"></param>
        /// <param name="afterSaleDetailList"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateGoodsDaySalesStatistics(Guid pushDataId, Guid orderId, IList<AfterSaleDetailInfo> afterSaleDetailList);

        #region 往来单位收付款

        /// <summary>
        /// 往来单位收付款添加,修改
        /// </summary>
        /// <param name="companyFundReceiptInfo"></param>
        /// <returns></returns>
        [OperationContract]
        CompanyFundReceiptInfo InsertCompanyFundReceiptForShop(CompanyFundReceiptInfo companyFundReceiptInfo);

        /// <summary>
        /// 往来单位收付款查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <param name="receiptType"></param>
        /// <param name="searchKey"> </param>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyFundReceiptInfo> GetCompanyFundReceiptForShop(DateTime? startTime, DateTime? endTime, Guid? companyId, Guid filialeId, int? receiptType, string searchKey);

        /// <summary>
        /// 更改往来单位收付款状态(门店)
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="receiptId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateCompanyFundReceiptStatus(Guid pushDataId, Guid receiptId, int status, string remark);

        /// <summary>
        /// 根据id获取往来帐收付款状态
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        [OperationContract]
        int GetCompanyFundReceiptStatus(Guid receiptId);

        #endregion 往来单位收付款

        /// <summary>针对支付宝订单IsOut为False且事后申请发票更新订单和资金流字段IsOut为True（其他地方慎用）
        /// </summary>
        /// <param name="orderIds">订单Ids</param>
        /// <param name="paidNo">交易流水号</param>
        /// <returns></returns>
        [OperationContract]
        Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo);

        /// <summary>
        /// 是否进入
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ReturnResult IsEnterFaxAudit(bool isEnable);

        /// <summary>
        /// 获取税务审核状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ReturnResult<bool> GetEnterFaxAuditState();

        /// <summary>
        /// 根据员工ID获取对应的预借款预警信息
        /// 财务查询全部可输入Guid.Empty
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int GetPreloanWarningsCount(Guid personnelId, Guid systemBranchId);

        #region 重新添加对B2C服务

        /// <summary>
        /// 根据年份和指定条件查询订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="orderTime"></param>
        /// <returns></returns>
        [OperationContract]
        GoodsOrderInfo GetOrderInfoByNo(string orderNo, DateTime orderTime);

        /// <summary>根据年份和指定条件查询订单明细
        /// </summary>
        /// <param name="orderTime"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [OperationContract]
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(DateTime orderTime, string orderNo);

        /// <summary>
        /// 修改发票
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceInfo"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo SetGoodsOrderInvoice(Guid pushDataId, InvoiceInfo invoiceInfo);

        /// <summary>
        /// 获取商品类型对应的税率比例(包含：商品类型、类型编码、税率)
        /// </summary>
        /// <param name="goodsTypes"></param>
        /// <returns></returns>
        [OperationContract]
        List<TaxrateProportionInfo> GeTaxrateProportionInfos(IEnumerable<int> goodsTypes);

        /// <summary>
        /// 修改发票状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="clew">备注</param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo SetGoodsOrderInvoiceState(Guid pushDataId, Guid invoiceId, InvoiceState invoiceState, String clew);

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        [OperationContract]
        WCFReturnInfo BatchSetInvoiceState(Guid pushDataId, List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel);

        #endregion 重新添加对B2C服务

        /// <summary>第三方订单直接完成发货【四维】        ADD  2016年12月6日  陈重文
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressId">订单快递Id</param>
        /// <param name="expressNo">订单快递号</param>
        /// <param name="companyId">配置的往来单位ID</param>
        /// <returns></returns>
        [OperationContract]
        Boolean ThirdPartyOrderComplete(String orderNo, Guid expressId, String expressNo, Guid companyId);

        /// <summary>根据销售公司获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        [OperationContract]
        Dictionary<Guid, int> GetGoodsSaleBySaleFilialeId(DateTime fromTime, DateTime toTime, Guid saleFilialeId);

        /// <summary>根据销售平台获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="salePlatformIdList">销售平台ID集合</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        /// zal 2017-07-27
        [OperationContract]
        Dictionary<Guid, int> GetGoodsSaleBySalePlatformIdList(DateTime fromTime, DateTime toTime, List<Guid> salePlatformIdList);

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <returns></returns>
        /// zal 2017-08-08
        [OperationContract]
        List<VocabularyInfo> GetVocabularyListByState();

        /// <summary>
        /// 获取违禁词(State=1)
        /// </summary>
        /// <returns></returns>
        /// zal 2017-10-25
        [OperationContract]
        List<string> GetVocabularyNameList();

        #region 海外购相关接口

        /// <summary>
        /// 根据商品IDS，批量获取对应的商品 供应商关系（供应商ID，商品ID）
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<Guid, Guid> GetGoodsAndCompanyList(IList<Guid> goodsId);

        /// <summary>
        /// 获取境外供应商列表（返回值：供应商ID，名称）
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<Guid, String> GetAbroadCompanyList();

        /// <summary>
        /// 根据登录账号、销售公司Id获取已授权的境外供应商列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<AuthorizeCompanyDTO> GetAuthorizeCompanyDtos(string accountNo, Guid saleFilialeId);

        #endregion 海外购相关接口

        #region 补贴审核、补贴打款

        /// <summary>
        /// 新增补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AddSubsidyPayment(SubsidyPaymentInfo_Add model);

        /// <summary>
        /// 修改补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateSubsidyPayment(SubsidyPaymentInfo_Edit model);

        /// <summary>
        /// 审核补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo ApprovalSubsidyPayment(SubsidyPaymentInfo_Approval model);

        /// <summary>
        /// 删除补贴审核（作废）
        /// </summary>
        /// <param name="ID">补贴审核编号</param>
        /// <param name="ModifyUser">修改人</param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeleteSubsidyPayment(Guid ID, string ModifyUser);

        /// <summary>
        /// 获取补贴审核列表
        /// </summary>
        /// <param name="seachModel"></param>
        /// <param name="Totalcount">总数</param>
        /// <returns></returns>
        [OperationContract]
        IList<SubsidyPaymentInfo> GetSubsidyPaymentList(SubsidyPaymentInfo_SeachModel seachModel, out int Totalcount);

        /// <summary>
        /// 获取补贴审核信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        SubsidyPaymentInfo GetSubsidyPaymentByID(Guid ID);

        /// <summary>
        /// 根据第三方订单号 判断（1：有没有进行中（待审核、待财务审核、不通过）的费用补贴，2：补贴次数不超过2次）
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ResultModel IsExistSubsidyPayment(string ThirdPartyOrderNumber);

        #endregion 补贴审核、补贴打款

        #region 退款打款

        /// <summary>
        /// 新增退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AddRefundsMoney(RefundsMoneyInfo_Add model);

        /// <summary>
        /// 修改退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdateRefundsMoney(RefundsMoneyInfo_Edit model);

        /// <summary>
        /// 审核退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo ApprovalRefundsMoney(RefundsMoneyInfo_Approval model);

        /// <summary>
        /// 删除退款打款（作废）
        /// </summary>
        /// <param name="ID">退款打款编号</param>
        /// <param name="ModifyUser">修改人</param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeleteRefundsMoney(Guid ID, string ModifyUser);

        /// <summary>
        /// 获取退款打款列表
        /// </summary>
        /// <param name="seachModel"></param>
        /// <param name="Totalcount">总数</param>
        /// <returns></returns>
        [OperationContract]
        IList<RefundsMoneyInfo> GetRefundsMoneyList(RefundsMoneyInfo_SeachModel seachModel, out int Totalcount);

        /// <summary>
        /// 获取退款打款信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        RefundsMoneyInfo GetRefundsMoneyByID(Guid ID);

        #endregion 退款打款
    }
}