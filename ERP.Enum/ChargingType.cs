using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 快递计费方式
    /// </summary>
    [Serializable]
    public enum ChargingType
    {
        /// <summary>
        /// 默认，续重(1Kg)
        /// </summary>
        [Enum("续重1Kg")]
        Default = 0,

        /// <summary>
        /// 中转
        /// </summary>
        [Enum("中转")]
        Transfer = 1,

        /// <summary>
        /// 续重(0.5Kg)
        /// </summary>
        [Enum("续重0.5Kg")]
        AddWeight = 2,

        /// <summary>续重(3Kg)
        /// </summary>
        [Enum("续重3Kg")]
        AddWeight3kg = 3
    }
}
