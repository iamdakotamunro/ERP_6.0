using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品出售类型
    /// </summary>
    public enum SellType
    {
        /// <summary>
        /// 货币
        /// </summary>
        [Enum("货币")]
        Currency = -1,
        /// <summary>
        /// 积分
        /// </summary>
        [Enum("积分")]
        Score = 0,
        /// <summary>
        /// 积分+货币
        /// </summary>
        [Enum("货币+积分均可")]
        CurrencyAndScore = 1,

    }
}
