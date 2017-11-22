using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 支付状态枚举
    /// </summary>
    [Serializable]
    public enum PayState
    {
        /// <summary>
        /// 所有状态
        /// </summary>
        [Enum("所有状态")]
        NoSet = 0,
        /// <summary>
        /// 未支付
        /// </summary>
        [Enum("未支付")]
        NoPay = 1,
        /// <summary>
        /// 已支付
        /// </summary>
        [Enum("已支付")]
        Paid = 2,
        /// <summary>
        /// 待确认
        /// </summary>
        [Enum("待确认")]
        Wait = 3,

        /// <summary>
        /// 预付款
        /// </summary>
        [Enum("预付款")]
        PrePaid = 4,
    }
}
