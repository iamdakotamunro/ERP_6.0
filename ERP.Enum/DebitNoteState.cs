using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 借记单状态
    /// </summary>
    [Serializable]
    public enum DebitNoteState
    {
        /// <summary>
        /// 所有状态
        /// </summary>
        [Enum("所有状态")]
        All = -1,

        /// <summary>
        /// 待采购
        /// </summary>
        [Enum("待采购")]
        ToPurchase = 0,

        /// <summary>
        /// 采购中
        /// </summary>
        [Enum("采购中")]
        Purchasing = 1,

        /// <summary>
        /// 部分完成
        /// </summary>
        [Enum("部分完成")]
        PartComplete = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        [Enum("已完成")]
        AllComplete = 4,

        /// <summary>
        /// 已注销
        /// </summary>
        [Enum("已注销")]
        Logout = 5,

        /// <summary>待核销（手动新建的赠品借记单为采购中，点击完成则变为待核销，核销后完成）
        /// </summary>
        [Enum("待核销")]
        WaitChargeOff = 6,
    }
}
