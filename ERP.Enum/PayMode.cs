using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 支付方式枚举
    /// </summary>
    [Serializable]
    public enum PayMode
    {
        /// <summary>
        /// 
        /// </summary>
        [Enum("不可确认")]
        NoSet=-1,
        /// <summary>
        /// 货到付款
        /// </summary>
        [Enum("货到付款")]
        COD=0,
        /// <summary>
        /// 款到发货
        /// </summary>
        [Enum("款到发货")]
        COG=1,

        /// <summary>
        ///货到付款手机支付 
        /// </summary>
        [Enum("货到付款手机支付")]
        COM=2

    }
}
