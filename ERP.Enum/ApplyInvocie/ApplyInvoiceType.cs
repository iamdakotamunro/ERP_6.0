using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum.ApplyInvocie
{
    [Serializable]
    [DataContract]
    public enum ApplyInvoiceType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = 0,

        /// <summary>
        /// 增值税专用发票
        /// </summary>
        [Enum("增值税专用发票")]
        [EnumMember]
        Special = 1,

        /// <summary>
        /// 普通发票
        /// </summary>
        [Enum("普通发票")]
        [EnumMember]
        Custom = 2,

        /// <summary>
        /// 收据
        /// </summary>
        [Enum("收据")]
        [EnumMember]
        Receipt = 3,
    }
}
