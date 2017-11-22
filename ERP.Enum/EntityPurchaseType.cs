using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 门店公司采购类型
    /// </summary>
    [Serializable]
    public enum EntityPurchaseType
    {
        /// <summary>
        /// 直营
        /// </summary>
        [Enum("直营")]
        Direct = 1,
        /// <summary>
        /// 加盟
        /// </summary>
        [Enum("加盟")]
        Join = 2,
        /// <summary>
        /// 联盟
        /// </summary>
        [Enum("联盟")]
        Alliance = 3,
    }
}
