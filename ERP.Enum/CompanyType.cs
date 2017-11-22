using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 公司类型枚举
    /// </summary>
    [Serializable]
    public enum CompanyType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Enum("其他")]
        Other = 0,
        /// <summary>
        /// 供应商
        /// </summary>
        [Enum("供应商")]
        Suppliers = 1,
        /// <summary>
        /// 销售商
        /// </summary>
        [Enum("销售商")]
        Vendors = 2,
        /// <summary>
        /// 物流公司
        /// </summary>
        [Enum("物流公司")]
        Express = 3,
        /// <summary>
        /// 会员总帐
        /// </summary>
        [Enum("会员总帐")]
        MemberGeneralLedger = 4
    }
}
