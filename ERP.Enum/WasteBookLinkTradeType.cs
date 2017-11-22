using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>▄︻┻┳═一 产生资金流单据对应枚举 ADD  陈重文  2014-12-25
    /// </summary>
    public enum WasteBookLinkTradeType
    {
        /// <summary>订单
        /// </summary>
        [Enum("订单")]
        GoodsOrder = 1,

        /// <summary>费用申报
        /// </summary>
        [Enum("费用申报")]
        CostReport = 2,

        /// <summary> 往来单位收付款
        /// </summary>
        [Enum("往来单位收付款")]
        CompanyFundReceipt = 3,

        /// <summary> 提现
        /// </summary>
        [Enum("提现")]
        WithdrawDeposit = 4,

        /// <summary> 充值
        /// </summary>
        [Enum("充值")]
        Pay = 5,

        /// <summary> 其他
        /// </summary>
        [Enum("其他")]
        Other = 6,
    }
}
