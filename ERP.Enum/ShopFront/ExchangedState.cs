using System;
using ERP.Enum.Attribute;

namespace ERP.Enum.ShopFront
{
    /// <summary>
    /// 联盟店退换货申请状态
    /// </summary>
    [Serializable]
    public enum ExchangedState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1,

        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        CheckPending = 0,

        /// <summary>
        /// 审核通过
        /// </summary>
        [Enum("审核通过")]
        Pass = 1,

        /// <summary>
        /// 审核不通过
        /// </summary>
        [Enum("审核不通过")]
        NoPass = 2,

        /// <summary>
        /// 待检查
        /// </summary>
        [Enum("待检查")]
        Checking = 3,

        /// <summary>
        /// 检查通过
        /// </summary>
        [Enum("检查通过")]
        Checked = 4,

        /// <summary>
        /// 商品退回
        /// </summary>
        [Enum("商品退回")]
        GoodsReturn = 5,

        /// <summary>
        /// 换货中
        /// </summary>
        [Enum("换货中")]
        Bartering = 6,

        /// <summary>
        /// 换货完成
        /// </summary>
        [Enum("换货完成")]
        BarterEnd = 7,

        /// <summary>
        /// 退货中
        /// </summary>
        [Enum("退货中")]
        Returning= 8,

        /// <summary>
        /// 退货完成
        /// </summary>
        [Enum("退货完成")]
        ReturnEnd = 9,

        /// <summary>
        /// 取消
        /// </summary>
        [Enum("取消")]
        Cancel = 10,
    }
}
