using System;
using ERP.Enum.Attribute;

namespace ERP.Enum.ShopFront
{
    /// <summary>门店首页活动图片 2015-05-04 陈重文
    /// </summary>
    [Serializable]
    public enum ShopActivityImageType
    {
        /// <summary>网站
        /// </summary>
        [Enum("网站")]
        Web = 1,

        /// <summary>手机
        /// </summary>
        [Enum("手机")]
        Phone = 2
    }
}
