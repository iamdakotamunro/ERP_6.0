using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用种类(0:广告费;1:购买物品;2:纯费用;)
    /// zal 2015-11-04
    /// </summary>
    public enum CostsVarieties
    {
        /// <summary>
        /// 广告费
        /// </summary>
        [Enum("广告费")]
        AdvertisingFee = 0,

        /// <summary>
        /// 购买物品
        /// </summary>
        [Enum("购买物品")]
        BuyArticle = 1,

        /// <summary>
        /// 纯费用
        /// </summary>
        [Enum("纯费用")]
        PureCost = 2
    }
}
