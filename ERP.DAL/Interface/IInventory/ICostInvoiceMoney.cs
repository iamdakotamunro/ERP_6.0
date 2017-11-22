using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>▄︻┻┳═一 费用发票金额接口   ADD 2014-12-20  陈重文
    /// </summary>
    public interface ICostInvoiceMoney
    {
        /// <summary>获取具体年月份费用发票金额
        /// </summary>
        /// <returns>Return:费用发票金额集合</returns>
        IList<CostInvoiceMoneyInfo> GetCostInvoiceMoneyList(int dateYear,int dateMonth);
    }
}
