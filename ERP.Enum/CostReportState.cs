using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报审批状态枚举
    /// </summary>
    public enum CostReportState
    {
        /// <summary>
        /// 票据待受理
        /// </summary>
        [Enum("票据待受理")] 
        NoAuditing = 0,

        /// <summary>
        /// 票据不合格
        /// </summary>
        [Enum("票据不合格")] 
        InvoiceNoPass = 1,

        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")] 
        Auditing = 2,

        /// <summary>
        /// 审核不通过
        /// </summary>
        [Enum("审核不通过")] 
        AuditingNoPass = 3,

        /// <summary>
        /// 待付款
        /// </summary>
        [Enum("待付款")] 
        AlreadyAuditing = 4,

        /// <summary>
        /// 待打款不通过
        /// </summary>
        [Enum("待打款不通过")] 
        ExecuteNoPass = 5,

        /// <summary>
        /// 票据待核销
        /// </summary>
        [Enum("票据待核销")] 
        Execute = 6,

        /// <summary>
        /// 完成
        /// </summary>
        [Enum("完成")] 
        Complete = 7,

        /// <summary>
        /// 待退款
        /// </summary>
        [Enum("待退款")] 
        WaitReturn = 8,

        /// <summary>
        /// 待收款
        /// </summary>
        [Enum("待收款")] 
        WaitVerify = 9,

        /// <summary>
        /// 票据待选
        /// </summary>
        [Enum("票据待选")] 
        WaitCheck = 10,

        /// <summary>
        /// 待完成
        /// </summary>
        [Enum("待完成")] 
        Pay = 11,

        /// <summary>
        /// 完成打款退回
        /// </summary>
        [Enum("打款退回")]
        PayBack = 12,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Cancel = 13,

        /// <summary>
        /// 打款前审核
        /// </summary>
        [Enum("打款前审核")]
        AuditingBeforePay = 14,

        /// <summary>
        /// 打款前审核不通过
        /// </summary>
        [Enum("打款前审核不通过")]
        AuditingBeforePayNoPass = 15,
        /// <summary>
        /// 完成可申请
        /// </summary>
        [Enum("完成可申请")]
        CompletedMayApply = 21
    }
}
