using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 发票状态枚举
    /// </summary>
    [Serializable]
    public enum InvoiceState
    {
        /// <summary>
        /// 所有状态
        /// </summary>
        [Enum("所有发票")]
        All = -1,
        /// <summary>
        /// 未开
        /// </summary>
        [Enum("未开发票")]
        NoRequest = 0,
        /// <summary>
        /// 申请发票
        /// </summary>
        [Enum("申请发票")]
        Request = 1,
        /// <summary>
        /// 已开
        /// </summary>
        [Enum("已开发票")]
        Success = 2,
        /// <summary>
        /// 取消
        /// </summary>
        [Enum("取消发票")]
        Cancel = 3,
        /// <summary>
        /// 作废申请
        /// </summary>
        [Enum("作废申请")]
        WasteRequest = 4,
        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Waste = 5,
    }
}
