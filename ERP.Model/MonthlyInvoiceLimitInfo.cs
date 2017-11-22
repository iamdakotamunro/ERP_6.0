using System;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一 每月开票额度模型（针对订单）   ADD 2014-12-20  陈重文
    /// </summary>
    public class MonthlyInvoiceLimitInfo
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

        /// <summary>开票额度 
        /// </summary>
        public decimal MakeOutInvoiceLimit { get; set; }

        /// <summary>非开票额度
        /// </summary>
        public decimal NotMakeOutInvoiceLimit { get; set; }

        /// <summary>实际开票金额
        /// </summary>
        public decimal RealityInvoiceSum { get; set; }

        /// <summary>实际未开金额
        /// </summary>
        public decimal NoRequestInvoiceSum { get; set; }

        /// <summary>开票总金额（实际开票金额+实际未开票金额）
        /// </summary>
        public decimal TotalMakeOutInvoice { get; set; }
    }
}
