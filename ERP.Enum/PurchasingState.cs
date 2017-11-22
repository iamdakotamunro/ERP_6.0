using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 采购单状态枚举
    /// </summary>
    public enum PurchasingState
    {
        /// <summary>
        /// 未提交
        /// </summary>
        [Enum("未提交")]
        NoSubmit = 0,

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
        /// 入库中
        /// </summary>
        [Enum("入库中")]
        StockIn = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        [Enum("已完成")]
        AllComplete = 4,
        /// <summary>
        /// 已删除
        /// </summary>
        [Enum("已删除")]
        Deleted = 5,
        /// <summary>
        /// 未完成
        /// </summary>
        [Enum("未完成")]
        NoComplete = 6,

        /// <summary>
        /// 调价待审核
        /// </summary>
        [Enum("调价待审核")]
        WaitingAudit = 7,

        /// <summary>
        /// 调价拒绝
        /// </summary>
        [Enum("调价拒绝")]
        Refusing = 8
    }
}
