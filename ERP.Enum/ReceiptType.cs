using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 收据类型枚举
    /// </summary>
    [DataContract]
    public enum ReceiptType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = -1,
        /// <summary>
        /// 支出
        /// </summary>
        [Enum("支出")]
        [EnumMember]
        Expenditure = 0,
        /// <summary>
        /// 收入
        /// </summary>
        [Enum("收入")]
        [EnumMember]
        Income = 1,
    }
}
