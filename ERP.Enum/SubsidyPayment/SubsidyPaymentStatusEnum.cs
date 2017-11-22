using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum.SubsidyPayment
{
    /// <summary>
    /// 补贴审核（1：待审核、2：待财务审核、3：待打款、4：核退、5：已作废、6：已打款）
    /// </summary>
    [Serializable]
    [DataContract]
    public enum SubsidyPaymentStatusEnum
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        [EnumMember]
        PendingCheck = 1,

        /// <summary>
        /// 待财务审核
        /// </summary>
        [Enum("待财务审核")]
        [EnumMember]
        PendingFinanceCheck = 2,
        
        /// <summary>
        /// 待打款
        /// </summary>
        [Enum("待打款")]
        [EnumMember]
        PendingPayment = 3,

        /// <summary>
        /// 核退
        /// </summary>
        [Enum("核退")]
        [EnumMember]
        Rejected = 4,

        /// <summary>
        /// 已作废
        /// </summary>
        [Enum("已作废")]
        [EnumMember]
        Cancel = 5,

        /// <summary>
        /// 已打款
        /// </summary>
        [Enum("已打款")]
        [EnumMember]
        HadPayment = 6,
    }
}