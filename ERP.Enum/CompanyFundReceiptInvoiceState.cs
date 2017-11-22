using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来单位单据 发票状态 zal 2016-02-17
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptInvoiceState
    {
        /// <summary>
        /// 未提交
        /// </summary>
        [Enum("未提交")]
        UnSubmit=0,
        /// <summary>
        /// 已提交
        /// </summary>
        [Enum("已提交")]
        Submit = 1,
        /// <summary>
        /// 已接收
        /// </summary>
        [Enum("已接收")]
        Receive=2,
        /// <summary>
        /// 待认证
        /// </summary>
        [Enum("待认证")]
        Authenticate = 3,
        /// <summary>
        /// 认证完成
        /// </summary>
        [Enum("认证完成")]
        Verification = 4
    }
}
