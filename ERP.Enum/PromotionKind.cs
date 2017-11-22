using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 买几赠几/促销信息
    /// </summary>
    public enum PromotionKind
    {
        /// <summary>
        /// 无
        /// </summary>
        [Enum("无")]
        None = 0,

        /// <summary>
        /// 买几赠几
        /// </summary>
        [Enum("买几赠几")]

        BuyGive = 1,
        /// <summary>
        /// 促销信息
        /// </summary>
        [Enum("促销信息")]
        PromotionInfo = 2,

        /// <summary>
        /// 买几赠几和促销信息
        /// </summary>
        [Enum("买几赠几和促销信息")]
        Both = 3
    }
}
