using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报票据类型枚举
    /// </summary>
    public enum CostReportInvoiceType
    {
        /// <summary>
        /// 普通发票
        /// </summary>
        [Enum("普通发票")]
        Invoice = 1,

        /// <summary>
        /// 收据
        /// </summary>
        [Enum("收据")]
        Voucher = 2,

        /// <summary>
        /// 网页凭证
        /// </summary>
        [Enum("网页凭证")]
        NoVoucher = 3,

        /// <summary>
        /// 费用收入
        /// </summary>
        [Enum("-")]
        WaitCheck = 4,

        /// <summary>
        /// 增值税专用发票
        /// </summary>
        [Enum("增值税专用发票")]
        VatInvoice = 5
    }
}
