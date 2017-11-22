using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 费用申报票据状态 zal 2016-08-04
    /// </summary>
    public enum CostReportBillState
    {
        /// <summary>
        /// 未提交
        /// </summary>
        [Enum("未提交")]
        UnSubmit = 0,
        /// <summary>
        /// 未接收
        /// </summary>
        [Enum("未接收")]
        Submit = 1,
        /// <summary>
        /// 未认证
        /// </summary>
        [Enum("未认证")]
        Receive = 2,
        /// <summary>
        /// 未完成
        /// </summary>
        [Enum("未完成")]
        Finish = 4,
        /// <summary>
        /// 票据完成
        /// </summary>
        [Enum("票据完成")]
        Verification = 3
    }
}
