using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>资金流接口  最后修改提交  陈重文  2014-12-25
    /// </summary>
    public interface IWasteBook
    {
        #region [新增资金流]

        /// <summary>新增资金流
        /// </summary>
        /// <param name="wasteBook">资金流</param>
        int Insert(WasteBookInfo wasteBook);

        #endregion

        #region [更新审核资金流]

        ///<summary>更新资金流信息
        /// </summary>
        /// <param name="wasteBook">资金流</param>
        void Update(WasteBookInfo wasteBook);

        /// <summary>更新DateCreated字段
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        void UpdateDateTime(string tradeCode);

        /// <summary>更新资金流描述
        /// </summary>
        /// <param name="wastebookId">资金流单据ID</param>
        /// <param name="description">描述</param>
        void UpdateDescription(Guid wastebookId, String description);

        /// <summary> 资金流审核时添加审核人描述信息
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="description">描述</param>
        void UpdateDescriptionForAuditing(string tradeCode, string description);

        /// <summary>更新资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="poundage">手续费</param>
        /// <param name="dateTime">时间</param>
        void UpdatePoundage(string tradeCode, decimal poundage, DateTime dateTime);

        /// <summary>更改资金流手续费
        /// </summary>
        /// <param name="wastebookId">资金流ID</param>
        /// <param name="dateCreated">时间</param>
        /// <param name="poundage">手续费</param>
        void UpdatePoundageForReckoning(Guid wastebookId, DateTime dateCreated, decimal poundage);

        /// <summary> 修改费用记录时修改资金流
        /// </summary>
        /// <param name="tradeCode">单据ID</param>
        /// <param name="accountReceivable">金额</param>
        /// <param name="description">描述</param>
        /// <param name="dateTime">时间</param>
        void UpdateForReckoningCost(string tradeCode, double accountReceivable, string description, DateTime dateTime);

        /// <summary> 更新资金流 银行账号ID
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <param name="bankAccountsId">银行账号ID</param>
        void UpdateBankAccountsId(Guid wasteBookId, Guid bankAccountsId);

        ///<summary>审核资金流
        /// </summary>
        /// <param name="tradeCode">账单编号</param>
        void Auditing(string tradeCode);

        /// <summary>更新资金流IsOut字段为True,同时更新订单(前台事后申请发票用，其他地方慎用)
        /// </summary>
        /// <param name="orderIds">订单Ids </param>
        /// <param name="paidNo">交易流水号</param>
        /// <returns></returns>
        Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo);

        /// <summary>
        /// 更新交易佣金的操作状态
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-21 操作状态(0:未处理；1:已处理)
        bool UpdateOperateState();
        #endregion

        #region [删除资金流]

        /// <summary>删除资金流
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        void DeleteWasteBook(string tradeCode);

        /// <summary> 删除资金流手续费的记录
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="poundage"></param>
        void DeleteWasteBookPoundage(string tradeCode, decimal poundage);

        #endregion

        #region [获取银行账号余额，手续费，记录数]

        /// <summary>获取银行账号余额
        /// </summary>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <returns></returns>
        decimal GetBalance(Guid bankAccountsId);

        /// <summary>获取资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        decimal GetPoundage(string tradeCode);

        /// <summary>获取资金流手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        decimal GetPoundageForReckoning(string tradeCode);

        /// <summary>获取同一单据号资金流记录数（判断是否有手续费）
        /// </summary>
        /// <param name="tradeCode">单据号</param>
        /// <returns></returns>
        decimal GetTradeCodeNum(string tradeCode);

        #endregion

        #region [获取资金流ID]

        /// <summary>单据编号获取手续费的资金流ID
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        string GetWasteBookId(string tradeCode);

        /// <summary>获取资金流手续费的WasteBookId
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        string GetWasteBookIdForUpdate(string tradeCode);

        /// <summary> 获取手续费的资金流ID
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        string GetWasteBookIdForReckoning(string tradeCode);

        #endregion

        #region [获取资金流]

        /// <summary>获取资金流信息
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBook(Guid wasteBookId);

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBookByBankAccountsId(string tradeCode);

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="linkTradeCode">关联单据编号</param>
        /// <param name="wasteSource">1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额</param>
        /// <returns></returns>
        /// zal 2016-06-15
        WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource);

        /// <summary>获取资金流信息（历史数据库）
        /// </summary>
        /// <param name="wasteBookId">资金流ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="keepyear">保留年份</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBook(Guid wasteBookId, DateTime dateTime, int keepyear);

        /// <summary> 获取资金流单据类型
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <returns></returns>
        WasteTypeInfo GetWasteBookInfo(String tradeCode);

        #endregion

        #region 资金流获取相关方法

        /// <summary>
        /// 根据年份获取资金流信息 add 2015-06-15 CAA
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="year"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        IList<FundPaymentDaysInfo> GetFundPaymentDaysInfos(int keepyear, int year, Guid saleFilialeId, string bankName);

        /// <summary>
        /// 获取公司的银行资金信息
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        IList<FundPaymentDaysInfo> GetFundPaymentDaysBankInfos(int keepyear, Guid filialeId, int year, string bankName);

        #endregion
        #region [获取资金流集合]

        /// <summary>根据有操作权限银行账号的资金流
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="personnelId">员工ID</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, Guid personnelId);

        /// <summary>获取资金列表
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear);

        /// <summary>获取资金列表（分页）
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>获取资金列表（分页）
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="isCheck"> 是否对账</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>获取资金流列表（分页）
        /// </summary>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="receiptType">单据类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="minIncome">金额范围 小</param>
        /// <param name="maxIncome">金额范围 大</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="filialeId">员工公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <param name="keepyear">保存几年数据</param>
        /// <param name="isCheck">是否对账</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 【公司毛利使用】获取订单佣金(此佣金是一个订单号对应的所有佣金的和，原因：第三方订单和本系统订单存在多对多的情况，导致本系统一个订单会产生多笔佣金)
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-20
        IList<WasteBookInfo> GetWasteBookByDateCreatedForProfits();

        #endregion

    }
}
