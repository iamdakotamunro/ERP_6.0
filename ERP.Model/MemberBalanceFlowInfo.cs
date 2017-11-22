using System;

namespace ERP.Model
{
    /// <summary>会员余额流水模型
    /// </summary>
    public class MemberBalanceFlowInfo
    {

        /// <summary>会员ID
        /// </summary>
        public Guid MemberId { get; set; }
        
        /// <summary>单据编号
        /// </summary>
        public string TradeCode { get; set; }
        
        /// <summary>用户名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>流水创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    
        /// <summary>收入金额
        /// </summary>
        public decimal IncreaseAmount { get; set; }
        /// <summary>支出金额
        /// </summary>
        public decimal SubtractAmount { get; set; }

        /// <summary>当前货币余额
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>管理描述 
        /// </summary>
        public string ManageDescription { get; set; }

        /// <summary>对网站用户的余额流水描述
        /// </summary>
        public string Memo { get; set; }

        
    }
}
