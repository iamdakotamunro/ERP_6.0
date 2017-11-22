using System;

namespace ERP.Model.Invoice
{
    [Serializable]
    public class InvoiceApplyDetailInfo
    {
        public Guid Id { get; set; }

        public Guid ApplyId { get; set; }

        public string Tradecode { get; set; }

        public string LinkTradeCodes { get; set; }

        public decimal PayBalanceAmount { get; set; }

        public decimal PayRebateAmount { get; set; }

        public decimal TotalPayAmount { get; set; }

        public InvoiceApplyDetailInfo(){}

        /// <summary>
        /// 创建保证金收据(收据类型的发票)明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="tradeCode"></param>
        /// <param name="totalPayAmount"></param>
        public InvoiceApplyDetailInfo(Guid applyId,string tradeCode, decimal totalPayAmount)
        {
            Id = Guid.NewGuid();
            ApplyId = applyId;
            Tradecode = tradeCode;
            TotalPayAmount = totalPayAmount;
            LinkTradeCodes = string.Empty;
        }

        /// <summary>
        /// 创建贷款发票(普通发票和增值税专用发票)明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="tradeCode"></param>
        /// <param name="linkTradeCodes"></param>
        /// <param name="payBalance"></param>
        /// <param name="payRebate"></param>
        /// <param name="totalPayAmount"></param>
        public InvoiceApplyDetailInfo(Guid applyId, string tradeCode,string linkTradeCodes,decimal payBalance,decimal payRebate, decimal totalPayAmount)
        {
            Id = Guid.NewGuid();
            ApplyId = applyId;
            Tradecode = tradeCode;
            LinkTradeCodes = linkTradeCodes;
            PayBalanceAmount = payBalance;
            PayRebateAmount = payRebate;
            TotalPayAmount = totalPayAmount;
        }
    }
}
