using System;
using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 对账状态
    /// </summary>
    [Serializable]
    [DataContract]
    public enum CheckType
    {
        /// 所有
        /// </summary>
        [Enum("所有")]
        [EnumMember]
        AllCheck = -1,
        /// <summary>
        ///  0  未对账
        /// </summary>
        [Enum("未对账")]
        NotCheck = 0,
        /// <summary>
        /// 1 已经对账
        /// </summary>
        [Enum("已经对账")]
        [EnumMember]
        IsChecked = 1,
        /// <summary>
        /// 2 异常对账
        /// </summary>
        [Enum("异常对账")]
        [EnumMember]
        QueChecked = 2,
        /// <summary>
        /// 3 对账中
        /// </summary>
        [Enum("对账中")]
        Checking = 3
    }
}
