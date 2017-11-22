using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 作废位置枚举-dyy at 2010 Jan 7th
    /// </summary>
    public enum CancelPosition
    {
        /// <summary>
        /// 订单受理
        /// </summary>
        [Enum("订单受理页面")]
        GoodsOrder = 1,

        /// <summary>
        /// 订单查询
        /// </summary>
        [Enum("订单查询页面")]
        GoodsOrderSee = 2,

        /// <summary>
        /// 待付款确认
        /// </summary>
        [Enum("待付款确认页面")]
        AffirmPayment = 3,

        /// <summary>
        /// 打印订单
        /// </summary>
        [Enum("打印订单页面")]
        LetterOfAdvice = 4,

        /// <summary>
        /// 匹配快递单
        /// </summary>
        [Enum("匹配快递单页面")]
        MatchingExpress = 5,

        /// <summary>
        /// 需调拨订单
        /// </summary>
        [Enum("需调拨订单页面")]
        Redeploy = 6,

        /// <summary>
        /// 完成订单
        /// </summary>
        [Enum("完成订单页面")]
        Accomplish = 7,

        /// <summary>
        /// 
        /// </summary>
        [Enum("订单配货页面")]
        OrderPick = 8
    }
}
