using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 发票购货方企业类型枚举
    /// </summary>
    [DataContract]
    public enum InvoicePurchaserType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumMember]
        None =0,

        /// <summary>
        /// 企业
        /// </summary>
        [EnumMember]
        Enterprise = 1,

        /// <summary>
        /// 政府机关单位
        /// </summary>
        [EnumMember]
        Government = 2,

        /// <summary>
        /// 个人
        /// </summary>
        [EnumMember]
        Individual = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [EnumMember]
        Other = 4
    }
}
