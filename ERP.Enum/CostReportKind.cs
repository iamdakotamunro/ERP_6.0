using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报类型枚举
    /// </summary>
    public enum CostReportKind
    {
        /// <summary>
        /// 预借款
        /// </summary>
        [Enum("预借款")]
        Before = 1,

        /// <summary>
        /// 凭证报销
        /// </summary>
        [Enum("凭证报销")]
        Later = 2,

        /// <summary>
        /// 已扣款核销
        /// </summary>
        [Enum("已扣款核销")]
        Paying = 3,

        /// <summary>
        /// 费用收入
        /// </summary>
        [Enum("费用收入")]
        FeeIncome = 4
    }
}
