using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品订单支付状态枚举
    /// </summary>
    public enum GoodsOrderPayState
    {
        /// <summary>
        /// 禁用
        /// </summary>
        [Enum("禁用")]
        Disabled=0,

        /// <summary>
        /// 启用
        /// </summary>
        [Enum("启用")]
        Enabled=1
    }
}
