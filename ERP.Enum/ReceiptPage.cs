
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary> 往来单位收付款页面枚举
    /// </summary>
    public enum ReceiptPage
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Enum("其他")]
        Else=0,

        /// <summary>
        /// 收付款单据
        /// </summary>
        [Enum("收付款单据")]
        CompanyFundReceipt=1,

        /// <summary>
        /// 付款审核
        /// </summary>
        [Enum("付款审核")]
        PayCheckList=2,

        /// <summary>
        /// 申请打款
        /// </summary>
        [Enum("申请打款")]
        DoReceivePay=3,

        /// <summary>
        /// 完成打款
        /// </summary>、
        [Enum("完成打款")]
        CompanyFundPayReceiptFinish=4,

        /// <summary>
        /// 索取发票
        /// </summary>
        [Enum("索取发票")]
        DemandReceipt=5,

        /// <summary>
        /// 开具发票
        /// </summary>
        [Enum("开具发票")]
        DoFoudReceive=6,
    }
}
