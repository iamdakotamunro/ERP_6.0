using System;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一 费用发票金额模型   ADD 2014-12-20  陈重文
    /// </summary>
    [Serializable]
    public class CostInvoiceMoneyInfo
    {
        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>年份 
        /// </summary>
        public int DateYear { get; set; }

        /// <summary>月份
        /// </summary>
        public int DateMonth { get; set; }

        /// <summary>额度
        /// </summary>
        public decimal Limit { get; set; }

        /// <summary>实报金额
        /// </summary>
        public decimal RealityCost { get; set; }

        /// <summary>费用申报状态
        /// </summary>
        public int State { get; set; }

        /// <summary>待收取发票金额
        /// </summary>
        public decimal WaitCollect { get; set; }

        /// <summary>待核销发票金额
        /// </summary>
        public decimal WaitChargeOff { get; set; }

        /// <summary>已核销发票金额
        /// </summary>
        public decimal AlreadyChargeOff { get; set; }
    }
}
