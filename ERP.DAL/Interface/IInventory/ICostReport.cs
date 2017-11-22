using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;
using System.Collections;

//================================================
// 功能：费用审批申报接口类
// 作者：刘彩军
// 时间：2011-February-22th
//================================================
namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReport
    {

        bool IsRepeatSubmit(Guid reportId, Guid assumeFilialeId, Guid assumeBranchId, Guid assumeGroupId, Guid reportPersonnelId, Guid companyClassId, Guid companyId, int reportKind, decimal reportCost);

        string GetBankAccount(Guid reportPersonnelId, string bankAccountName);

        /// <summary>
        /// 添加申报
        /// </summary>
        /// <param name="info">申报模型</param>
        /// <param name="errorMessage"></param>
        bool InsertReport(CostReportInfo info, out string errorMessage);

        /// <summary>
        /// 根据用户ID获取申报
        /// </summary>
        /// <param name="reportPersonnelID">用户ID</param>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList(Guid reportPersonnelID);

        /// <summary>
        /// 根据用户IDlist获取申报
        /// </summary>
        /// <param name="reportPersonnelIds">用户IDlist</param>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList(List<Guid> reportPersonnelIds);

        /// <summary>
        /// 修改申报
        /// </summary>
        /// <param name="info">申报详细模型</param>
        void UpdateReport(CostReportInfo info);

        /// <summary>
        /// 获取申报,根据申报ID获取出申报内容
        /// </summary>
        /// <param name="reportID">申报ID</param>
        /// <returns></returns>
        CostReportInfo GetReportByReportId(Guid reportID);

        /// <summary>
        /// 根据获取申报
        /// </summary>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList();

        /// <summary>
        /// 获取押金列表(条件：是押金且已完成的预借款类型的单据)
        /// </summary>
        /// <param name="type">回收状态(0:未回收;1:已回收) </param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="reportPersonnelIds">申请人</param>
        /// <param name="num">预借款编号/押金编号/回收单据号</param>
        /// <returns></returns>
        /// zal 2016-08-17
        IList<CostReportInfo> GetReportListForDeposit(int type, DateTime startTime, DateTime endTime, List<Guid> reportPersonnelIds, string num);

        IList<CostReportInfo> GetReportList(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取申报
        /// </summary>
        /// <param name="reportFilialeId">申报人公司</param>
        /// <param name="reportBranchId">申报人部门</param>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList(Guid reportFilialeId, Guid reportBranchId);

        /// <summary>
        /// 根据获取申报
        /// </summary>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList(IList<int> states);

        /// <summary>
        /// 根据获取申报
        /// </summary>
        /// <returns></returns>
        IList<CostReportInfo> GetReportList(DateTime startTime, DateTime endTime, string branchIdString, string invoiceType, string state);

        ///  <summary>
        /// 修改申报状态
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="auditingMemo">审核说明</param>
        /// <param name="memo">备注</param>
        /// <param name="auditingMan">审批人(为Empty则不更新该字段)</param>
        void UpdateReport(Guid reportId, int state, string auditingMemo, string memo, Guid auditingMan);

        ///  <summary>
        /// 修改申报已支付金额、状态、备注
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="payCost">审核说明</param>
        /// <param name="memo">备注</param>
        void UpdateReport(Guid reportId, int state, Decimal payCost, string memo);

        ///  <summary>
        /// 修改申报金额、已支付金额、状态、备注
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="reportCost">申报金额</param>
        /// <param name="payCost">审核说明</param>
        /// <param name="memo">备注</param>
        void UpdateReport(Guid reportId, int state, Decimal reportCost, Decimal payCost, string memo);

        /// <summary>
        /// 修改实际金额
        /// </summary>
        /// <param name="reportId">申报费用id</param>
        /// <param name="state">状态</param>
        /// <param name="realityCost">实际金额</param>
        /// <param name="memo">备注</param>
        /// zal 2016-01-11
        void UpdateRealityCost(Guid reportId, int state, Decimal realityCost, string memo);

        ///  <summary>
        /// 修改申报状态、备注、付款类型银行账号
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="companyId">打款分类</param>
        /// <param name="bankAccountName">收款账户</param>
        /// <param name="bankAccount">收款款银行</param>
        /// <param name="auditingMan">审核人</param>
        /// <param name="payBankAccountId">打款账号ID</param>
        /// <param name="memo">备注</param>
        /// <param name="companyClassId">费用分类</param>
        void UpdateReportAuditing(Guid reportId, int state, string memo, Guid companyId, string bankAccountName,
                                  string bankAccount, Guid auditingMan, Guid payBankAccountId, Guid companyClassId);

        /// <summary>
        /// 修改申报实际金额
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="realityCost">实际金额</param>
        void UpdateReportRealityCost(Guid reportId, decimal realityCost);

        /// <summary>
        /// 获取申报统计
        /// </summary>
        /// <param name="branch">申报部门</param>
        /// <param name="classId">费用分类</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="filialeId">公司Id</param>
        /// <returns></returns>
        IList<CostReportInfo> GetReportStatistics(DateTime startDate, DateTime endDate, Guid classId, Guid branch, Guid filialeId);

        /// <summary>
        /// 根据部门获取部门申报统计
        /// </summary>
        /// <param name="branch">申报部门</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="filialeId">公司Id </param>
        /// <returns></returns>
        IList<CostReportInfo> GetReportStatisticsByBranch(DateTime startDate, DateTime endDate, Guid branch, Guid filialeId);

        /// <summary>
        /// 得到登录者可以操作的费用申报信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<CostReportInfo> GetAllPrompinfo(Guid filialeId, Guid branchId, Guid positionId);

        ///  <summary>
        /// 修改申报手续费
        /// Add by liucaijun at 2012-02-09
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="poundage">手续费</param>
        void UpdateReportPoundage(Guid reportId, Decimal poundage);

        ///  <summary>
        /// 修改申报本公司打款银行
        /// Add by liucaijun at 2012-02-09
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="executeBankId">本公司打款银行</param>
        void UpdateReportExecuteBankId(Guid reportId, Guid executeBankId);

        ///  <summary>
        /// 修改申报状态
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="state">状态</param>
        /// <param name="memo">备注</param>
        void UpdateReport(Guid reportId, int state, string memo);

        ///  <summary>
        /// 根据申报类型修改相应类型的时间(1:审核时间，2:预付款时间，3:完成时间)
        /// </summary>
        /// <param name="reportId">申报ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="type">类型，1审核时间，2预付款时间，3完成时间</param>
        void UpdateReportDate(Guid reportId, DateTime dateTime, int type);

        /// <summary>
        /// 修改费用承担公司部门小组
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="assumeFilialeId"></param>
        /// <param name="assumeBranchId"></param>
        /// <param name="assumeGroupId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReportAssumeFilialeIdBranchIdGroupId(Guid reportId, Guid assumeFilialeId, Guid assumeBranchId, Guid assumeGroupId, out string errorMessage);

        /// <summary>
        /// 记录交易流水号
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="tradeNo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReportTradeNo(Guid reportId, string tradeNo, out string errorMessage);

        /// <summary>
        /// 修改直营店铺
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="shopId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateAssumeShopId(Guid reportId, Guid shopId, out string errorMessage);

        /// <summary>
        /// 预借款审核通过，待预借款(待收款)
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="auditingMan"></param>
        /// <param name="executeBandId"></param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReportBeforeAuditingPass(Guid reportId, Guid auditingMan, Guid executeBandId, string memo, out string errorMessage);

        /// <summary>
        /// 预借款审核不通过
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="auditingMan"></param>
        /// <param name="auditingMemo"></param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReportBeforeAuditingNoPass(Guid reportId, Guid auditingMan, string auditingMemo, string memo, out string errorMessage);

        /// <summary>
        /// 预借款打款通过，11待打款（已打款）
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="executeBandId"></param>
        /// <param name="tradeNo">交易流水号</param>
        /// <param name="poundage">手续费</param>
        /// <param name="memo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateReportBeforeExecutePass(Guid reportId, Guid executeBandId, string tradeNo, decimal poundage, string memo, out string errorMessage);

        int UpdateReportAuditingBeforePayByReportId(Guid reportId, int auditingBeforePay, out string errorMessage);

        int UpdateCostReportRefundmentBankAccountIdByReportId(Guid reportId, Guid refundmentBankAccountId);

        int UpdateCostReportPayBankAccountIdByReportId(Guid reportId, Guid payBankAccountId);

        /// <summary>根据费用申报编号、名称、收款人、备注获取费用申报
        /// </summary>
        /// <param name="reportNo">申报编号</param>
        /// <param name="reportName">申报名称</param>
        /// <param name="payCompany">收款人</param>
        /// <param name="reportPersonnelId">申报人Id </param>
        /// <param name="memo">备注（模糊）</param>
        /// <returns></returns>
        IList<CostReportInfo> GetCostReportInfoList(string reportNo, string reportName, string payCompany, Guid reportPersonnelId, string memo);

        /// <summary>更新费用申报IsOut为True ADD 陈重文 2015-1-28 
        /// </summary>
        /// <param name="reportId"></param>
        void UpdateCostReportIsOut(Guid reportId);

        /// <summary>
        /// 根据reportId修改“已支付金额”和“执行收付款时间”
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="payCost">已支付金额</param>
        /// <returns></returns>
        /// zal 2015-11-19
        bool UpdatePayCostAndExecuteDate(Guid reportId, decimal payCost);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportKind"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="reportPersonnleId"></param>
        /// <param name="reportState"></param>
        /// <param name="reportNo"></param>
        /// <param name="reportName"></param>
        /// <param name="type">0：完成时间；1：申报时间</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        IList<CostReportInfo> GetCostReportForWarningsInfos(int reportKind, DateTime? startTime, DateTime? endTime,
            Guid reportPersonnleId, string reportState, string reportNo, string reportName, string type, string billState);

        /// <summary>
        /// 获取预借款预警数
        /// </summary>
        /// <param name="reportKind"></param>
        /// <param name="dayTime"></param>
        /// <param name="reportPersonnleId"></param>
        /// <param name="reportStates"></param>
        /// <returns></returns>
        int GetPreloanWarningsCount(int reportKind, DateTime dayTime, Guid reportPersonnleId, string reportStates);

        /// <summary>
        /// 根据申报IDp批量修改申报申请人
        /// </summary>
        bool UpdateReportPersonnelIdByReportId(List<Guid> reportIds, Guid reportPersonnelId);

        /// <summary>
        /// 根据reportIdList统计有票据的申请单总数、发票总数、收据总数
        /// </summary>
        /// <param name="reportIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-18
        ArrayList GetSumInvoiceAccept(string[] reportIdList);

        /// <summary>
        /// 根据reportIdList统计申请单总数、申报金额、已付款金额
        /// </summary>
        /// <param name="reportIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-18
        ArrayList GetSumReport(string[] reportIdList);

        /// <summary>
        /// 获取同一收款单位同一预估申报金额或者同一提交人同一预估申报金额的数据
        /// </summary>
        /// <param name="payCompany"></param>
        /// <param name="reportPersonnelId"></param>
        /// <param name="reportCost"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        /// zal 2016-11-24
        ArrayList[] ReportNoArrayList(string payCompany, Guid reportPersonnelId, decimal reportCost, Guid reportId);
    }
}
