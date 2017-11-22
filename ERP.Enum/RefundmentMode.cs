using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 退款模式枚举
    /// </summary>
    [Serializable]
    public enum RefundmentMode
    {
        /// <summary>
        /// 存入帐号(后台)
        /// </summary>
        [Enum("存入帐号")]
        Accounts = 0,
        /// <summary>
        /// 原帐号退回(银行)
        /// </summary>
        [Enum("原帐号退回")]
        Untread = 1
    }
}
