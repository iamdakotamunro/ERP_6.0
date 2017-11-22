using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum.ApplyInvocie
{
    [Serializable]
    [DataContract]
    public enum ApplyInvoiceSourceType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = 0,

        /// <summary>
        /// 订单类型
        /// </summary>
        [Enum("订单类型")]
        [EnumMember]
        Order = 1,

        /// <summary>
        /// 加盟店类型
        /// </summary>
        [Enum("加盟店类型")]
        [EnumMember]
        League = 2,
        
    }
}
