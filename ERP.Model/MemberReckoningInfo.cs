using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>会员总账模型  ADD   2014-09-09  陈重文
    /// </summary>
    public class MemberReckoningInfo
    {
        /// <summary>单据ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>单据编号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>原始单据号
        /// </summary>
        public string OriginalTradeCode { get; set; }

        /// <summary>余额变动单据号
        /// </summary>
        public string BalanceFlowNo { get; set; }

        /// <summary>用户ID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>当前余额
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>流动金额 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>单据描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>销售公司ID
        /// </summary>
        public Guid SaleFilialeID { get; set; }
    }
}
