using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 订单状态枚举
    /// </summary>
    [Serializable]
    public enum OrderState
    {
        /// <summary>
        /// 所有状态
        /// </summary>
        [Enum("所有状态")]
        All = -1,

        /// <summary>
        /// 未经审核
        /// </summary>
        [Enum("未经审核")]
        UnVerify = 0,

        /// <summary>
        /// 待确认
        /// </summary>
        [Enum("待确认")]
        WaitNotarize = 1,

        /// <summary>
        /// 等待付款,已受理
        /// </summary>
        [Enum("已受理")]
        Approved = 2,

        /// <summary>
        /// 待付款确认
        /// </summary>
        [Enum("待付款确认")]
        WaitForPay = 3,

        /// <summary>
        /// 配货中(受理后或待付款确认后)
        /// </summary>
        [Enum("配货中")]
        StockUp = 4,

        /// <summary> 出货中
        /// </summary>
        [Enum("出货中")]
        WaitOutbound = 5,

        /// <summary>
        /// 需调拨
        /// </summary>
        [Enum("需调拨")]
        Redeploy = 6,

        /// <summary>
        /// 需采购
        /// </summary>
        [Enum("需采购")]
        RequirePurchase = 7,

        /// <summary>
        /// 完成发货(完成订单后)
        /// </summary>
        [Enum("完成发货")]
        Consignmented = 9,

        /// <summary>
        /// 作废订单
        /// </summary>
        [Enum("作废订单")]
        Cancellation = 12
    }
}
