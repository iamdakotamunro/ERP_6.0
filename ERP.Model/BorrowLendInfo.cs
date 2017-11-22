using System;

namespace ERP.Model
{
    /// <summary>
    /// 借入返还单、借出返还单
    /// </summary>
    [Serializable]
    public class BorrowLendInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid BorrowLendId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid StockId { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal AccountReceivable { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal SubtotalQuantity { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
