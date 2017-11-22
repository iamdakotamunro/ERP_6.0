using System;

namespace ERP.Model
{
    /// <summary>
    /// 采购商品分组合计
    /// </summary>
    [Serializable]
    public class PurchasingGoodsAmountInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid PurchasingId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 采购公司
        /// </summary>
        public Guid PurchasingFilialeId { get; set; }

        /// <summary>
        /// 合计价格
        /// </summary>
        public decimal AmountPrice { get; set; }

        public bool IsOut { get; set; }
    }
}
