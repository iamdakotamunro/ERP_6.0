using ERP.Enum;
using ERP.Model.Finance.Gathering;
using ERP.Model.Finance.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Finance.Interface
{
    /// <summary>
    /// 财务API：付款单的IDAL
    /// </summary>
    public interface IPaymentDAL
    {
        #region 付款单

        /// <summary>
        /// 获取付款单：往来单位收付款（收付款类型=付款单）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        IList<CompanyFundReceiptDTO> GetPayment_CompanyFundReceipt(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 获取付款单：资金流（转账）。上海百秀旗舰店》百秀兰溪转账。
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="LogisticsCompany">物流公司</param>
        /// <param name="SalesCompany">销售公司</param>
        /// <returns></returns>
        IList<WasteBookDTO> GetPayment_WasteBook(DateTime StartTime, DateTime EndTime, string LogisticsCompany, string SalesCompany);

        #endregion 付款单
    }
}