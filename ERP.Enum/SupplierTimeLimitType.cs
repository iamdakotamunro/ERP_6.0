using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 供应商资质有效期类型
    /// </summary>
    [Serializable]
    public enum SupplierTimeLimitType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = 0,
        /// <summary>
        /// 正常
        /// </summary>
        [Enum("正常")]
        Normal = 1,
        /// <summary>
        /// 快过期
        /// </summary>
        [Enum("快过期")]
        Expire = 2,
        /// <summary>
        /// 已过期
        /// </summary>
        [Enum("已过期")]
        Expired = 3
    }
}
