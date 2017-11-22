using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 卖库存状态
    /// </summary>
    [Serializable]
    public enum SaleStockState
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Enum("未设置")]
        NoSet = -1,

        /// <summary>
        /// 申请中
        /// </summary>
        [Enum("申请中")]
        Apply=0,

        /// <summary>
        /// 审核通过
        /// </summary>
        [Enum("审核通过")]
        Approve=1,

        /// <summary>
        /// 申请不通过
        /// </summary>
        [Enum("申请不通过")]
        ApplyNotPass=2,
    }
}
