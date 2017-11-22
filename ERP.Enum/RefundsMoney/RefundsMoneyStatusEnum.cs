using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum.RefundsMoney
{
    /// <summary>
    /// 退款打款的状态（1：待审核、2：核退、3：待打款、4：已作废、5：已打款）
    /// </summary>
    [Serializable]
    [DataContract]
    public enum RefundsMoneyStatusEnum
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        [EnumMember]
        PendingCheck = 1,

        /// <summary>
        /// 核退
        /// </summary>
        [Enum("核退")]
        [EnumMember]
        Rejected = 2,
        
        /// <summary>
        /// 待打款
        /// </summary>
        [Enum("待打款")]
        [EnumMember]
        PendingPayment = 3,

        /// <summary>
        /// 已作废
        /// </summary>
        [Enum("已作废")]
        [EnumMember]
        Cancel = 4,

        /// <summary>
        /// 已打款
        /// </summary>
        [Enum("已打款")]
        [EnumMember]
        HadPayment = 5,
    }
}