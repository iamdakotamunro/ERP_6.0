using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 权限类型枚举
    /// </summary>
    [Serializable]
    public enum CostReportAuditingType
    {
        /// <summary>
        /// 审核权限
        /// </summary>
        [Enum("审核权限")]
        Auditing = 0,
        /// <summary>
        /// 票据受理权限
        /// </summary>
        [Enum("票据受理权限")]
        Invoice = 1
    }
}
