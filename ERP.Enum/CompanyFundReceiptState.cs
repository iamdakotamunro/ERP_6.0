using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来单位收付款状态枚举
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1,

        /// <summary>
        /// 
        /// </summary>
        [Enum("未操作")]
        NoHandle = -2,

        /// <summary>
        /// 
        /// </summary>
        [Enum("已操作")]
        Handle = -3,

        /// <summary>
        /// 未通过
        /// </summary>
        [Enum("未通过")]
        NoAuditing = 0,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Enum("审核未通过")]
        NoAuditingPass = 13,

        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        WaitAuditing = 1,

        /// <summary>
        /// 待开发票
        /// </summary>
        [Enum("待开发票")]
        WaitInvoice = 2,

        /// <summary>
        /// 已审核
        /// </summary>
        [Enum("已审核")]
        Audited = 3,

        /// <summary>
        /// 已执行
        /// </summary>
        [Enum("已执行")]
        Executed = 4,

        /// <summary>
        /// 发票待索取
        /// </summary>
        [Enum("发票待索取")]
        GettingInvoice = 5,

        /// <summary>
        /// 发票已索取
        /// </summary>
        [Enum("发票已索取")]
        HasInvoice = 6,

        /// <summary>
        /// 发票核销
        /// </summary>
        [Enum("发票核销")]
        InvoiceVerification = 7,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Cancel = 8,

        /// <summary>
        /// 完成打款
        /// </summary>
        [Enum("完成打款")]
        Finish = 9,

        /// <summary>
        /// 打款退回
        /// </summary>
        [Enum("打款退回")]
        PayBack = 10,

        /// <summary>发票待认证
        /// </summary>
        [Enum("发票待认证")]
        WaitAttestation = 11,

        /// <summary>发票已认证
        /// </summary>
        [Enum("发票已认证")]
        FinishAttestation = 12
    }
}
