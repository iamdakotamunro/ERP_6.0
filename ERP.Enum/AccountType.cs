using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 银行账号类型
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Enum("未设置")]
        NoSet = -1,

        /// <summary>
        /// 支付平台
        /// </summary>
        [Enum("支付平台")]
        PayPlatform = 0,

        /// <summary>
        /// 银行支付
        /// </summary>
        [Enum("银行支付")]
        BankPay = 1,

        /// <summary>
        /// 信用卡支付
        /// </summary>
        [Enum("信用卡支付")]
        CreditCardPay = 2
    }
}
