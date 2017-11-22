using System;
using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 单据审核状态枚举
    /// </summary>
    [Serializable]
    [DataContract]
    public enum AuditingState
    {
        /// <summary>
        /// 未审核
        /// </summary>
        [Enum("未审核")]
        [EnumMember]
        No = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        [Enum("已审核")]
        [EnumMember]
        Yes = 1,

        /// <summary>
        /// 隐藏
        /// </summary>
        [Enum("隐藏")]
        [EnumMember]
        Hide = 2

    }
}