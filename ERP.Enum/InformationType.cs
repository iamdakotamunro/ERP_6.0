using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 资料类型枚举
    /// </summary>
    public enum InformationType
    {

        /// <summary>
        /// 商品
        /// </summary>
        [Enum("商品")]
        Goods = 1,

        /// <summary>
        /// 品牌
        /// </summary>
        [Enum("品牌")]
        Brand = 2,

        /// <summary>
        /// 往来单位
        /// </summary>
        [Enum("往来单位")]
        CompanyCussent = 3,

    }
}
