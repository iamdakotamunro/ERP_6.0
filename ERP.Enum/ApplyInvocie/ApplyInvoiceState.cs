using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum.ApplyInvocie
{
    [Serializable]
    [DataContract]
    public enum ApplyInvoiceState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = 0,

        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        [EnumMember]
        WaitAudit = 1,

        /// <summary>
        /// 待开票
        /// </summary>
        [Enum("待开票")]
        [EnumMember]
        WaitInvoicing = 2,

        /// <summary>
        /// 开票中
        /// </summary>
        [Enum("开票中")]
        [EnumMember]
        Invoicing = 3,

        /// <summary>
        /// 已完成
        /// </summary>
        [Enum("已完成")]
        [EnumMember]
        Finished = 4,

        /// <summary>
        /// 核退
        /// </summary>
        [Enum("核退")]
        [EnumMember]
        Retreat = 5,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        [EnumMember]
        Cancel = 6
    }
}
