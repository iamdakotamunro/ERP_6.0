using System;

namespace ERP.Model
{
    /// <summary>费用分类绑定公司资金账户模型  ADD  2015-01-20  陈重文
    /// </summary>
    public class CostCompanyBindingBankAccountsInfo
    {
        /// <summary>费用分类ID
        /// </summary>
        public Guid CostCompanyId { get; set; }

        /// <summary> 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>发票打款账户
        /// </summary>
        public Guid InvoiceAccountsId { get; set; }

        /// <summary>凭证打款账户
        /// </summary>
        public Guid VoucherAccountsId { get; set; }

        /// <summary>现金打款账户 
        /// </summary>
        public Guid CashAccountsId { get; set; }

        /// <summary>无凭证打款账户
        /// </summary>
        public Guid NoVoucherAccountsId { get; set; }
    }
}
