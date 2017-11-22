using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum.ApplyInvocie
{
    [Serializable]
    [DataContract]
    public enum ApplyInvoiceKindType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = 0,

        /// <summary>
        /// 贷款发票
        /// </summary>
        [Enum("贷款发票")]
        [EnumMember]
        Credit = 1,

        /// <summary>
        /// 保证金
        /// </summary>
        [Enum("保证金")]
        [EnumMember]
        Bail = 2,
    }
}
