using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来单位收付款单据类型枚举
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1,

        /// <summary>
        /// 收款
        /// </summary>
        [Enum("收款")]
        Receive = 0,
        /// <summary>
        /// 付款
        /// </summary>
        [Enum("付款")]
        Payment = 1
    }
}
