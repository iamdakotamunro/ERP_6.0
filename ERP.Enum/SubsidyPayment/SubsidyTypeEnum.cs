using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;
using System.Runtime.Serialization;

namespace ERP.Enum.SubsidyPayment
{
    /// <summary>
    /// 补贴类型（1：补偿、2：赠送）
    /// </summary>
    [Serializable]
    [DataContract]
    public enum SubsidyTypeEnum
    {
        /// <summary>
        /// 补偿
        /// </summary>
        [Enum("补偿")]
        [EnumMember]
        Compensate = 1,

        /// <summary>
        /// 赠送
        /// </summary>
        [Enum("赠送")]
        [EnumMember]
        Gift =2,
    }
}
