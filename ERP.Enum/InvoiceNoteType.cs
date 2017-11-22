using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 发票票据类型枚举
    /// </summary>
    [DataContract]
    public enum InvoiceNoteType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Enum("未知")]
        [EnumMember]
        None =-1,

        /// <summary>
        /// 正票
        /// </summary>
        [Enum("正票")]
        [EnumMember]
        Effective = 0,

        /// <summary>
        /// 废票
        /// </summary>
        [Enum("废票")]
        [EnumMember]
        Abolish = 1,

        /// <summary>
        /// 退票
        /// </summary>
        [Enum("退票")]
        [EnumMember]
        Retreat = 2,
    }
}
