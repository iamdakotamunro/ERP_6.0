using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BorrowLendDetailInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid BorrowLendId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}
