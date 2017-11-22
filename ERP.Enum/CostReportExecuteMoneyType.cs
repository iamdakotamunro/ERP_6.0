using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报打款类型枚举
    /// </summary>
    public enum CostReportExecuteMoneyType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Enum("现金")]
        Cash = 1,

        /// <summary>
        /// 转账
        /// </summary>
        [Enum("转账")]
        Transfer = 2
    }
}
