using System;

namespace ERP.Model
{
    /// <summary>
    /// 银行账户余额信息
    /// </summary>
    [Serializable]
    public class BankAccountBalanceInfo
    {
        /// <summary>
        /// 绑定银行的对应目标ID
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public Guid BankAccountId { get; set; }

        /// <summary>
        /// 当前银行余额
        /// </summary>
        public decimal NonceBalance { get; set; }

        public bool IsMain { get; set; }
    }
}
