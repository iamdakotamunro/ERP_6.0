using ERP.Enum;
using ERP.Model.Finance.Gathering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Finance.Interface
{
    /// <summary>
    /// 财务API：收款单的IDAL
    /// </summary>
    public interface IGatheringDAL
    {
        #region 收款单

        /// <summary>
        /// 获取收款单：往来单位收付款（收付款类型=收款单、劳务类型）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptDTO> GetGathering_CompanyFundReceipt(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 获取收款单：费用申报（申报类型=费用收入）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        IList<CostReportDTO> GetGathering_CostReport(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 获取收款单：资金流（转账）。百秀兰溪》上海百秀旗舰店转账。
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="LogisticsCompany">物流公司</param>
        /// <param name="SalesCompany">销售公司</param>
        /// <returns></returns>
        IList<WasteBookDTO> GetGathering_WasteBook(DateTime StartTime, DateTime EndTime, string LogisticsCompany, string SalesCompany);

        /// <summary>
        /// 获取收款单：客户付款-款到发货（排除 补偿、赠送 ）、余额支付（排除 补偿、赠送 ）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        IList<GoodsOrderDTO> GetGathering_GoodsOrder(DateTime StartTime, DateTime EndTime);

        #endregion 收款单

        #region 收款单（负数）

        /// <summary>
        /// 获取收款单（负数)：商品检查
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        IList<GatheringDTO> GetGathering_ReturnMoney(DateTime StartTime, DateTime EndTime);

        #endregion 收款单（负数）
    }
}