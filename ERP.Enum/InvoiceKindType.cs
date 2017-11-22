using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 发票类型
    /// </summary>
    [Serializable]
    public enum InvoiceKindType
    {
        /// <summary>
        /// 纸质发票
        /// </summary>
        [Enum("纸质发票")]
        Paper = 0,

        /// <summary>
        /// 电子发票
        /// </summary>
        [Enum("电子发票")]
        Electron = 1
    }
}
