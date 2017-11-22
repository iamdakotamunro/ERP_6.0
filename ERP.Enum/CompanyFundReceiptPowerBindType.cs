using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来单位收付款权限绑定方式枚举
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptPowerBindType
    {
        /// <summary>
        /// 直接绑定
        /// </summary>
        [Enum("直接绑定")]
        DirectBind = 0,
        /// <summary>
        /// 扩展绑定
        /// </summary>
        [Enum("扩展绑定")]
        ExpandBind = 1
    }
}
