using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 购买类型
    /// </summary>
    [Serializable]
    public enum BuyType
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Enum("所有")]
        All = 0,

        /// <summary>
        /// 普通购买
        /// </summary>
        [Enum("普通购买")]
        Normal = 1,

        /// <summary>
        /// 组合购买
        /// </summary>
        [Enum("组合购买")]
        Group = 2,

        /// <summary>
        /// 配镜购买
        /// </summary>
        [Enum("配镜购买")]
        CompFrame = 3,

        /// <summary>
        /// 框架购买
        /// </summary>
        [Enum("框架购买")]
        Frame = 4,

        /// <summary>
        /// 赠换购买
        /// </summary>
        [Enum("赠换购买")]
        Give = 5,

        /// <summary>
        /// 积分购买
        /// </summary>
        [Enum("积分购买")]
        Score = 6,

        /// <summary>第三方平台促销赠品
        /// </summary>
        [Enum("第三方平台促销赠品")]
        ThirdPlatformPromotionGift = 7,

        /// <summary>
        /// 配件
        /// </summary>
        [Enum("配件")]
        ErpGift = 8
    }
}
