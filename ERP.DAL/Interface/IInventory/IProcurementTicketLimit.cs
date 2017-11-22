using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>▄︻┻┳═一 采购索票额度数据接口   ADD 2014-12-19  陈重文
    /// </summary>
    public interface IProcurementTicketLimit
    {
        /// <summary>获取所有公司采购索取额度
        /// </summary>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns>Return:采购索票额度集合</returns>
        IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitList(int dateYear, int dateMonth);

        /// <summary>保存公司具体年份和月份的采购索票额度
        /// </summary>
        /// <param name="procurementTicketLimitInfo">采购索票额度</param>
        /// <returns>Return：true/false</returns>
        bool SaveProcurementTicketLimit(ProcurementTicketLimitInfo procurementTicketLimitInfo);

        /// <summary>获取公司对应具体供应商采购索票额度集合
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <param name="pageIndex">当前页码 </param>
        /// <param name="pageSize"> 每页显示条数</param>
        /// <param name="totalCount">总记录数 </param>
        /// <returns>Return:供应商采购索票额度集合</returns>
        IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailList(Guid filialeId, int dateYear,
            int dateMonth, int pageIndex, int pageSize, out int totalCount);

        /// <summary>获取具体供应商对应采购索取额度
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyIds">供应商Ids</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns></returns>
        IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailByCompanyIds(Guid filialeId, string companyIds,
            int dateYear, int dateMonth);

        /// <summary>获取当前年月供应商设置的采购索取额度
        /// </summary>
        /// <param name="companyId">供应商ID</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份 </param>
        /// <returns></returns>
        IList<ProcurementTicketLimitInfo> GetProcurementTicketLimitDetailByCompanyId(Guid companyId, int dateYear,
            int dateMonth);

        /// <summary>更新已经采购额度
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyId">供应商ID</param>
        /// <param name="alreadyCompleteLimit">额度</param>
        /// <param name="dateYear">年份</param>
        /// <param name="dateMonth">月份</param>
        /// <returns></returns>
        bool RenewalProcurementTicketLimitAlreadyCompleteLimit(Guid filialeId, Guid companyId,
            decimal alreadyCompleteLimit, int dateYear, int dateMonth);

    }
}
