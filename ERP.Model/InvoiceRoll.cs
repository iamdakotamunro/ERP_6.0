using System;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 领取发票的信息
    /// </summary>
    public class InvoiceRoll
    {
        /// <summary>
        /// 领取发票记录ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 领取人
        /// </summary>
        public string Receiptor { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long InvoiceStartNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long InvoiceEndNo { get; set; }

        /// <summary>
        /// 发票份数
        /// </summary>
        public long InvoiceCount { get; set; }

        /// <summary>
        /// 发票卷份数
        /// </summary>
        public int InvoiceRollCount { get; set; }

        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }
    }
}
