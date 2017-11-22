using System;
using System.Collections.Generic;
using ERP.Model.Report;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 供应商报表数据相关
    /// 应付款查询、采购入库统计报表
    /// </summary>
    public interface ISupplierReport
    {
        /// <summary>
        /// 查找年份下公司应付款金额、未付款金额
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IList<SupplierPaymentsReportInfo> SelectPaymentsReprotsGroupByFilialeId(int year);

        /// <summary>
        /// 查找公司年份下供应商应付款金额、未付款金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsReportInfo> SelectPaymentsReportsGroupByCompanyId(int year, Guid filialeId,string companyName);

        /// <summary>
        /// 查找年份下公司采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IList<SupplierPaymentsReportInfo> SelectStockReprotsGroupByFilialeId(int year);

        /// <summary>
        /// 查找公司年份下供应商采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsReportInfo> SelectStockReportsGroupByCompanyId(int year, Guid filialeId,string companyName);

        /// <summary>
        /// 出入库红冲更新存档数据
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="dayTime"></param>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        void ModifyRecord(Guid filialeId,Guid companyId,DateTime dayTime,string tradeCode);

        #region  针对当前月数据或者方案相关方法

        //重算每日数据
        bool ReRunEveryData(DateTime dayTime);
        #endregion

        #region  报表往来帐明细相关

        bool InsertRececkoning(bool isTemp,IList<RecordReckoningInfo> reckoningInfos);

        /// <summary>
        /// 判断当月往来帐是否记录
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool IsExists(DateTime dayTime);

        /// <summary>
        /// 判断当月往来帐是否记录 （某天）
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool IsExistsRecent(DateTime dayTime);
        #endregion

        #region  从临时表中获取当月应付款、采购入库数据

        /// <summary>
        /// 获取当月公司往来单位应付款
        /// </summary>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> GetPaymentsByForFiliale();

        /// <summary>
        /// 获取当月往来单位往来单位应付款
        /// </summary>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> GetPaymentsByForCompany(Guid filialeId);


        /// <summary>
        /// 获取当月公司采购入库统计金额
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetPurchasingByForFiliale();

        /// <summary>
        /// 获取当月往来单位往来单位应付款
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetPurchasingByForCompany(Guid filialeId);

        #endregion
    }
}
