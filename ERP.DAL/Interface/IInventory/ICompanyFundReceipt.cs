using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyFundReceipt
    {
        /// <summary>
        /// 指定状态、申请付款的时间段或是申请的单据号搜索
        /// for 付款审核添加公司 modify by liangcanren at 2015-05-27
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="page">页面枚举 </param>
        /// <param name="state"></param>
        /// <param name="appStartTime"></param>
        /// <param name="appEndTime"></param>
        /// <param name="companyFundReceiptNo"></param>
        /// <param name="type"></param>
        /// <param name="invoicesDemander">指定发票索取人</param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetAllFundReceiptInfoList(Guid filialeId, ReceiptPage page, CompanyFundReceiptState state, DateTime appStartTime,
            DateTime appEndTime, string companyFundReceiptNo, CompanyFundReceiptType type, params Guid[] invoicesDemander);

        List<CompanyFundReceiptInfo> GetFundReceiptInfoList(List<int> status,List<int> otherStatus, int state, DateTime appStartTime,
            DateTime appEndTime, string companyFundReceiptNo, int type);

        /// <summary>
        /// 更改单据状态
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int UpdateFundReceiptState(Guid receiptId, CompanyFundReceiptState state);

        /// <summary>
        /// 更新审核人
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        int UpdateFundReceiptAuditorID(Guid receiptId, Guid personnelId);

        /// <summary>
        /// 更新索取发票人
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        int UpdataFundReceiptInvoicesDemander(Guid receiptId, Guid personnelId);

        /// <summary>
        /// 更改备注信息
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        int UpdateFundReceiptRemark(Guid receiptId, string remark);

        /// <summary>
        /// 得到登录者可以操作的单据信息
        /// </summary>
        /// <param name="personnelId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetAllPrompinfo(Guid personnelId, Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// 指定ID的单据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CompanyFundReceiptInfo GetCompanyFundReceiptInfo(Guid id);

        /// <summary>
        /// 往来单位收付款
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetDefaultFundReceiptInfoList(Guid applicantId);

        /// <summary>
        /// 往来单位收付款列表
        /// </summary>
        /// <param name="applicantId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        /// zal 2016-03-22
        IList<CompanyFundReceiptInfo> GetDefaultFundReceiptInfoListByPage(Guid applicantId, int pageIndex, int pageSize,
            out int total);

        /// <summary>
        /// 往来单位收付款 查询按钮调用的方法
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="page"></param>
        /// <param name="state"></param>
        /// <param name="appStartTime"></param>
        /// <param name="appEndTime"></param>
        /// <param name="companyFundReceiptNo"></param>
        /// <param name="type"></param>
        /// <param name="companyId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="invoicesDemander"></param>
        /// <returns></returns>
        /// zal 2016-03-22
        IList<CompanyFundReceiptInfo> GetAllFundReceiptInfoListByPage(Guid filialeId, ReceiptPage page,
            CompanyFundReceiptState state, DateTime appStartTime, DateTime appEndTime, string companyFundReceiptNo,
            CompanyFundReceiptType type, Guid companyId, int pageIndex, int pageSize, out int total,
            params Guid[] invoicesDemander);

        /// <summary>
        /// 插入一条收付款单据数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Insert(CompanyFundReceiptInfo info);

        /// <summary>
        /// 修改保存单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(CompanyFundReceiptInfo entity);

        /// <summary>
        /// 收款(开具发票)
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="receiptStatus"></param>
        /// <param name="invoiceUnit"></param>
        /// <param name="invoiceState"></param>
        /// <param name="remark"></param>
        /// <param name="auditorId"></param>
        /// <returns></returns>
        bool UpdateInvoice(Guid receiptId, int receiptStatus, string invoiceUnit, int invoiceState, string remark,Guid auditorId);

        /// <summary>
        /// 统计
        /// Add by liucaijun at 2011-June-13th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="auditingId">审批人ID</param>
        /// <param name="payBankAccountsId">付款银行</param>
        /// <param name="hasInvoice">是否有发票</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetCompanyFundStatistics(Guid companyId, Guid auditingId, Guid payBankAccountsId, bool hasInvoice,
            DateTime startDate, DateTime endDate, Guid filialeId);

        /// <summary>
        /// 更新设置单据状态
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="receiptStatus"></param>
        /// <returns></returns>
        bool UpdateToReceiptStatus(Guid receiptId, int receiptStatus);

        /// <summary>
        /// 增加备注信息
        /// </summary>
        /// <param name="receiptID"></param>
        /// <param name="remarkContent"></param>
        /// <returns></returns>
        bool InsertToRemark(Guid receiptID, string remarkContent);

        /// <summary>
        /// 获取备注内容
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        string GetRemarkContent(Guid receiptId);

        /// <summary>
        /// 根据往来单位获取付款单
        /// Add by liucaijun at 2011-June-14th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="receiptType"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetFundReceiptListByCompanyID(Guid companyId, bool? receiptType);

        /// <summary>
        /// 根据往来单位获取收款单
        /// Add by liucaijun at 2011-August-24th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetFundListByCompanyId(Guid companyId);

        /// <summary>
        /// 更新执行、审核、完成时间
        /// Add by liucaijun at 2011-October-10th
        /// Modify by liucaijun at 2011-03-15th
        /// </summary>
        /// <param name="receiptId">单据ID</param>
        /// <param name="type">更新哪个时间，1审核时间2执行时间3完成时间</param>
        void SetDateTime(Guid receiptId, int type);

        /// <summary>
        /// 根据往来单位获取付款单
        /// Add by liucaijun at 2011-June-14th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetAllFundReceiptListByCompanyId(Guid companyId);

        /// <summary>
        /// 更新往来单位收付款手续费
        /// Add by liucaijun at 2012-02-08
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poundage"></param>
        void UpdatePoundage(Guid id, decimal poundage);

        /// <summary>根据公司ID获取当前月付款单据信息
        /// </summary>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetFundReceiptListByFilialeIdAndCurrentMonth(Guid filialeId);

        /// <summary>
        /// 根据往来帐收付款单据查找单据信息
        /// </summary>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        CompanyFundReceiptInfo GetFundReceiptInfoByReceiptNo(string receiptNo);

        /// <summary>
        /// 更新往来帐收付款的交易流水号
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="dealFlowNo"></param>
        void UpdateDealFlowNo(Guid receiptId, string dealFlowNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <param name="receiptType"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptInfo> GetFundReceiptList(DateTime? startTime, DateTime? endTime, Guid? companyId,
                                                         Guid filialeId, int? receiptType, string searchKey);


        /// <summary>
        /// 更新往来帐收付款状态和添加备注
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateCompanyFundReceiptStatus(Guid receiptId, int status, string remark);

        /// <summary>获取往来单位付款单据最近一条付款单据（填写付款单按日期付款使用）
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        CompanyFundReceiptInfo GetFundReceiptInfoByLately(Guid companyId, Guid filialeId);

        /// <summary>设置往来单位单位收付款银行帐号  ADD  陈重文  2015-03-02
        /// </summary>
        /// <param name="receiptId">往来单位收付款ID</param>
        /// <param name="bankAccountsId">银行账号ID</param>
        Boolean SetPayBankAccountsId(Guid receiptId, Guid bankAccountsId);

        /// <summary>设置往来单位单位收付款公司  ADD  陈重文  2015-03-30
        /// </summary>
        /// <param name="receiptId">往来单位收付款ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="isOut"></param>
        Boolean UpdateFilialeId(Guid receiptId, Guid filialeId, Boolean isOut);

        /// <summary>
        /// 判断入库单是否已经生成收付款单据 排除已作废的
        /// </summary>
        /// <param name="stockNo"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        bool IsExistsByStockOrderNos(string stockNo, string receiptNo);

        #region 根据“单据编号”获取数据
        /// <summary>
        /// 根据“单据编号”获取数据
        /// </summary>
        /// <param name="receiptNo">单据编号(多个单据编号，用英文状态下的逗号隔开)</param>
        /// <returns></returns>
        /// zal 2016-01-21
        IList<CompanyFundReceiptInfo> GetCompanyFundReceiptList(string receiptNo);
        #endregion

        #region 获取所有单据数据
        /// <summary>
        /// 获取所有单据数据
        /// </summary>
        /// <returns></returns>
        /// zal 2016-02-17
        IList<CompanyFundReceiptInfo> GetAllCompanyFundReceiptList();
        #endregion
        #region 验证“退货单”或者“付款单”是否被占用

        /// <summary>
        /// 验证“退货单”或者“付款单”是否被占用
        /// </summary>
        /// <param name="receiptId">往来单位收付款主键id</param>
        /// <param name="returnOrder">退货单</param>
        /// <param name="payOrder">付款单</param>
        /// <returns></returns>
        bool CheckReturnOrderOrPayOrder(Guid receiptId, string returnOrder, string payOrder);
        #endregion

        /// <summary>
        /// 按日期付款时，验证是否存在重复付款单据
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId">付款公司</param>
        /// <param name="settleStartDate">结账开始日期</param>
        /// <param name="settleEndDate">结账截止日期</param>
        /// <returns>按日期付款时(1:所填写的结账日期范围内的入库单的审核时间不能包含在已经按日期付款的结账日期范围内;2:所填写的结账日期范围内的入库单不能是按日期付款中已经包含的单据;3:所填写的结账日期范围内的入库单不能是已经按入库单付过款的单据;4:所填写的结账日期范围内的入库单可以是按日期付款中已经排除的单据;5:所填写的入库单不能是按入库单付款中已经退货的单据;)</returns>
        /// zal 2016-10-09
        List<string> CheckExistForDate(Guid companyId, Guid filialeId, DateTime settleStartDate, DateTime settleEndDate);

        /// <summary>
        /// 按入库单付款时，验证是否存在重复付款单据
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId">付款公司</param>
        /// <param name="tradeCode">入库单号</param>
        /// <returns>按入库单付款时(1:所填写的入库单的审核时间不能包含在已经按日期付款的结账日期范围内;2:所填写的入库单不能是按日期付款中已经包含的单据;3:所填写的入库单不能是已经按入库单付过款的单据;4:所填写的入库单可以是按日期付款中已经排除的单据;5:所填写的入库单不能是按入库单付款中已经退货的单据;)</returns>
        /// zal 2016-10-09
        List<string> CheckExistForStorageNo(Guid companyId, Guid filialeId, string tradeCode);

        /// <summary>
        /// 通过出入库单据号获取往来单位收付款单据状态
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="linkTradeCode"></param>
        /// <returns></returns>
        int GetFundReceiptStatusByLinkTradeCode(Guid companyId, string linkTradeCode);

        /// <summary>
        /// 更新收款账户
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        bool SetReceiveBankAccountIdByReceiptId(Guid receiptId, Guid bankAccountId);
    }
}
