using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来单位收付款发票类型枚举
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptInvoiceType
    {
        /// <summary>
        /// 开具发票
        /// </summary>
        [Enum("开具发票")]
        OpenInvoice = 0,
        /// <summary>
        /// 索取发票
        /// </summary>
        [Enum("索取发票")]
        CollectInvoice = 1
    }
}
