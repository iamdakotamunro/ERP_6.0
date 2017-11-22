using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报发票审核状态枚举
    /// </summary>
    public enum CostReportInvoiceState
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        NoAuditing = 0,
        /// <summary>
        /// 合格
        /// </summary>
        [Enum("合格")]
        Pass = 1,
        /// <summary>
        /// 不合格
        /// </summary>
        [Enum("不合格")]
        NoPass = 2,
        /// <summary>
        /// 完成核销
        /// </summary>
        [Enum("完成核销")]
        Complete = 3
    }
}
