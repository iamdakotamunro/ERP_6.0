using System;

namespace ERP.SAL.WMS
{
    public class OrderGoodsBatchNoDTO
    {
        /// <summary> 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>     批号(可能多个，逗号分隔)
        /// </summary>
        public string BatchNos { get; set; }

        /// <summary>有效期（可能多个，逗号分隔）
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int StockQuantity { get; set; }

    }
}
