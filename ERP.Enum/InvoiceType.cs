using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 开票类型枚举
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Enum("未设置")]
        All = 0,

        /// <summary>
        /// 普通发票
        /// </summary>
        [Enum("普通发票")]
        Normal =1,

        /// <summary>
        /// 增值税专用发票
        /// </summary>
        [Enum("增值税专用发票")]
        Special =2,

        /// <summary>
        /// 收据
        /// </summary>
        [Enum("收据")]
        Receipt =3
    }
}
