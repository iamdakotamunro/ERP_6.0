//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年5月3日
// 文件创建人:马力
// 最后修改时间:2010年3月23日
// 最后一次修改人:刘修明
//================================================

using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model.ASYN;
using ERP.Model.Report;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>▄︻┻┳═一 往来账接口  最后修改提交  陈重文  2014-12-24 
    /// </summary>
    public interface IReckoning
    {
        #region [新增往来账]

        /// <summary>新增往来账
        /// </summary>
        /// <param name="reckoningInfo">帐务记录条目类</param>
        /// <param name="errorMessage"></param>
        bool Insert(ReckoningInfo reckoningInfo, out string errorMessage);

        /// <summary>
        /// 用于快递损坏插入往来帐
        /// </summary>
        /// <param name="reckoningInfo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool IsExist(ReckoningInfo reckoningInfo, out string errorMsg);

        /// <summary>插入异步往来账
        /// </summary>
        /// <param name="asynInfo">异步往来账模型</param>
        /// <returns></returns>
        bool InsertAsyn(ASYNReckoningInfo asynInfo);

        #endregion

        #region [更新往来账]

        /// <summary>根据往来账记录ID修改往来账AccountReceivable，追加Description，DateCreated
        /// </summary>
        /// <param name="reckoningInfo"></param>
        void Update(ReckoningInfo reckoningInfo);

        /// <summary>批量修改往来账对账类型（对账服务用）
        /// </summary>
        /// <param name="lstModify">往来账集合</param>
        /// <param name="checkType">对账类型 1 已对账 0 未对账 2 异常对账</param>
        void UpdateCheckState(IList<ReckoningInfo> lstModify, Int32 checkType);

        /// <summary>
        /// 修改往来账对账类型
        /// </summary>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="linkTradeType">对应单据类型</param>
        /// <param name="reckoningType">账单类型：0收入，1支出</param>
        /// <param name="reckoningCheckType">往来账对账类型</param>
        /// <param name="isChecked">对账类型 1 已对账 0 未对账 2 异常对账</param>
        /// zal 2016-06-05
        bool UpdateCheckState(string linkTradeCode, int linkTradeType, int reckoningType, int reckoningCheckType, int isChecked);

        /// <summary> 更新往来账 追加 Description
        /// </summary>
        /// <param name="reckoningId">往来账ID</param>
        /// <param name="description"> </param>
        void UpdateDescription(Guid reckoningId, String description);

        /// <summary>单据审核追加备注信息（审核人信息）
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="description"></param>
        void UpdateDescriptionForAuditing(string tradeCode, string description);

        /// <summary>往来账==》审核
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        void Auditing(string tradeCode);

        /// <summary>红冲往来帐
        /// </summary>
        /// <param name="linkTradeCode">原始单据号 </param>
        void CancellationReckoning(string linkTradeCode);

        #endregion

        #region [删除往来账]

        /// <summary> 根据单据编号删除往来账信息
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        void Delete(string tradeCode);

        /// <summary>根据异步数据ID删除异步往来账
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteAsyn(Guid id);

        #endregion

        #region [获取往来账集合]

        /// <summary>按是否对账,日期,账单类型获取往来账（未对账）用于列表显示   
        /// </summary>
        /// <param name="companyClass">往来单位分类</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="cType">对账类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="receiptType">收入/支出</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="money">金额</param>
        /// <param name="start">当前页</param>
        /// <param name="limit">每页显示条数</param>
        /// <returns></returns>
        IList<ReckoningInfo> GetValidateDataPage(Guid companyClass, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType, AuditingState auditingState,
            ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, params int[] money);

        /// <summary>按是否对账,日期,账单类型获取往来账（未对账）用于列表显示   
        /// </summary>
        /// <param name="companyClassId">往来单位分类</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="cType">对账类型</param>
        /// <param name="auditingState">审核状态</param>
        /// <param name="receiptType">收入/支出</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="type"> </param>
        /// <param name="isOut"></param>
        /// <param name="money">金额</param>
        /// <param name="start">当前页</param>
        /// <param name="limit">每页显示条数</param>
        /// <param name="isCheck"> </param>
        /// <returns></returns>
        IList<ReckoningInfo> GetValidateDataPage(Guid companyClassId, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType, AuditingState auditingState,
            ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, int isCheck, int type, bool? isOut, params int[] money);

        /// <summary>获取往来帐集合（对账服务使用） ADD  2015-03-16  陈重文 
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="reckoningCheckType">对账类型</param>
        /// <param name="checkType">对账状态</param>
        /// <returns></returns>
        IList<ReckoningInfo> GetReckoningListByReconciliation(Guid companyId, DateTime startTime, DateTime endTime, ReckoningCheckType reckoningCheckType, CheckType checkType);

        /// <summary>获取异步往来账
        /// </summary>
        /// <param name="top">top</param>
        /// <returns></returns>
        IList<ASYNReckoningInfo> GetAsynList(int top);

        #endregion

        #region [获取往来账信息]

        /// <summary> 根据往来账记录ID获取往来账信息
        /// </summary>
        /// <param name="reckoningId">帐务记录Id</param>
        /// <returns></returns>
        ReckoningInfo GetReckoning(Guid reckoningId);

        /// <summary>根据往来账记录ID获取往来账信息（含历史数据库）
        /// </summary>
        /// <param name="reckoningId">往来账ID</param>
        /// <param name="dateTime">时间</param>
        /// <param name="keepyear">保存今年数据</param>
        /// <returns></returns>
        ReckoningInfo GetReckoning(Guid reckoningId, DateTime dateTime, int keepyear);

        /// <summary>根据往来单位ID、原始单据号、对账类型获取往来账信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="checkType">对账类型</param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        ReckoningInfo GetReckoningInfo(Guid companyId, string linkTradeCode, CheckType checkType, int reckoningCheckType=2);

        #endregion

        #region [获取往来总账]

        /// <summary>根据往来单位ID获取往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        decimal GetTotalled(Guid companyId);

        /// <summary>根据往来单位，公司，日期，获取往来总帐
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="endDate">日期 </param>
        /// <returns></returns>
        decimal GetReckoningNonceTotalledByFilialeId(Guid companyId, Guid filialeId, DateTime endDate);

        /// <summary>根据往来单位，公司，获取往来总帐（出库审核）
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        decimal GetNonceTotalled(Guid companyId, Guid filialeId);

        /// <summary>根据公司、往来单位、日期、单据状态获取往来总账
        /// </summary>
        /// <param name="filialeId">公司ID </param>
        /// <param name="companyId">往来单位</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="reckoningState">账务类型</param>
        /// <returns></returns>
        decimal GetReckoningNonceTotalled(Guid filialeId, Guid companyId, DateTime startDate, DateTime endDate, int reckoningState);

        #endregion

        #region [获取往来账单据类型 （0收入，1支出，-1未获取到）]

        /// <summary> 根据往来账ID获取往来账单据类型（0收入，1支出，-1未获取到）
        /// </summary>
        /// <param name="reckoningId"></param>
        int GetReckoningType(Guid reckoningId);

        #endregion

        #region [判断往来账是否存在]

        /// <summary>根据公司，往来单位，单据类型，原始单据号判断是否存在该往来账
        /// </summary>
        /// <param name="filialeID">公司ID</param>
        /// <param name="companyID">往来单位ID</param>
        /// <param name="reckoningType">往来账单位类型</param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <returns></returns>
        bool Exists(Guid filialeID, Guid companyID, int reckoningType, string linkTradeCode);

        #endregion

        #region [获取往来账ID]

        /// <summary>获取往来单位按日期最近的一条记录ID，用于记录差额说明
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">截至日期</param>
        /// <returns></returns>
        Guid GetReckoningInfoByDateLast(Guid filialeId, Guid companyId, DateTime startTime, DateTime endTime);

        /// <summary>根据单据编号或原始单据号获取往来帐ID
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        Guid GetReckoningInfoId(string tradeCode, int? isChecked);

        #endregion

        #region 供应商对账使用 add by liangcanren at 2015-05-29

        /// <summary>
        ///  获取入库单对应的未对账的往来帐
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        Guid GetReckoningInfoByTradeCode(string tradeCodeOrLinkTradeCode);

        /// <summary>
        /// 判断入库单对应的往来帐是否存在
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        bool IsExists(string tradeCodeOrLinkTradeCode);


        /// <summary>
        /// 将往来帐标识为已对账
        /// </summary>
        /// <param name="reckoningid"></param>
        /// <param name="isChecked"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        bool UpdateReckoningIsChecked(Guid reckoningid, int isChecked, DateTime startTime);

        /// <summary>
        /// 获取根据日期付款未对账的往来帐总额
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId"></param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isChecked">是否对账</param>
        /// <param name="stockNos">入库单据</param>
        /// <param name="removerNos"></param>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetTotalledByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked, IList<string> stockNos, IList<string> removerNos);


        /// <summary>
        /// 获取采购单Id获取已对账的入库单类型的往来帐金额
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="linkTradeId"></param>
        /// <param name="isChecked"></param>
        /// <param name="reckoningType"></param>
        /// <param name="linkTradeCodeType"></param>
        /// <returns></returns>
        decimal GetTotalledAccountReceivableByLinkTradeId(Guid filialeId,Guid companyId, Guid linkTradeId, int isChecked, int reckoningType, IList<int> linkTradeCodeType);

        /// <summary>
        /// 已进行采购单付款的往来账金额
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="purchasingNo"></param>
        /// <param name="isChecked"></param>
        /// <param name="reckoningType"></param>
        /// <param name="linkTradeCodeType"></param>
        /// <returns></returns>
        decimal GetTotalledAccountReceivable(Guid filialeId, Guid companyId, string purchasingNo, int isChecked,int reckoningType,
            IList<int> linkTradeCodeType);

        /// <summary>根据单据号获取往来总帐
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="linkTradeCode">往来单位编号</param>
        decimal GetTotalledByLinkTradeCode(Guid companyId, string linkTradeCode);

        #endregion

        #region  往来帐存档、应付款查询、入库金额统计  ADD BY LiangCanren AT 2015-08-17

        /// <summary>
        /// 查询当前月份入库往来帐及账期往来帐
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        IList<RecordReckoningInfo> SelectRecordReckoningInfos(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取当年当前月份下公司对应的应付款
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecords(int year, int month);

        /// <summary>
        /// 获取公司当年当前月份下供应商对应的应付款数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecordsDetail(int year, int month, Guid filialeId, string companyName);

        /// <summary>
        /// 获取当年当前月份下公司对应的采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecords(int year, int month);

        /// <summary>
        /// 获取公司当年当前月份下供应商对应的采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecordsDetail(int year, int month, Guid filialeId, string companyName);

        #endregion

        bool CheckByPurchaseOrder(IEnumerable<string> purchaseOrder,Guid filialeId,Guid thirdCompanyId,DateTime startTime);

        bool CheckByStorageTradeCode(IEnumerable<string> purchaseOrder, Guid filialeId, Guid thirdCompanyId, DateTime startTime);

        bool CheckByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked,
            IList<string> stockNos, IList<string> removerNos);
    }
}
