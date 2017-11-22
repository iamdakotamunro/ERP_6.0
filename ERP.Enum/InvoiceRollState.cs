using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 发票卷状态枚举
    /// </summary>
    public enum InvoiceRollState
    {
        /// <summary>
        /// 未启用
        /// </summary>
        [Enum("未启用")]
        NoInvocation=0,

        /// <summary>
        /// 分发
        /// </summary>
        [Enum("分发")]
        Distribute =1,

        /// <summary>
        /// 启用
        /// </summary>
        [Enum("启用")]
        Invocation=2,

        /// <summary>
        /// 打印完
        /// </summary>
        [Enum("打印完")]
        Print = 3,

        /// <summary>
        /// 遗失
        /// </summary>
        [Enum("遗失")]
        Lost = 4
    }
}
