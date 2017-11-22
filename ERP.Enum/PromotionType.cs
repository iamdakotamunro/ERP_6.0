using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 促销类型枚举
    /// </summary>
    [DataContract]
    public enum PromotionType
    {
        /// <summary>
        /// 活动
        /// </summary>
        [EnumMember] [Enum("活动")] 
        ACTIVITY = 0,

        /// <summary>
        /// 礼券
        /// </summary>
        [EnumMember] [Enum("礼券")] COUPON = 1
    }
}