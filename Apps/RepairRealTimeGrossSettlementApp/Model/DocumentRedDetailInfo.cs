using System;

namespace RepairRealTimeGrossSettlementApp.Model
{
    [Serializable]
    public class DocumentRedDetailInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 单据红冲记录ID
        /// </summary>
        public Guid RedId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary> 
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OldUnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units { get; set; }
    }
}
