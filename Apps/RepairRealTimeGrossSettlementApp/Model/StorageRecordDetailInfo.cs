using System;

namespace RepairRealTimeGrossSettlementApp.Model
{
    [Serializable]
    public class StorageRecordDetailInfo
    {

        /// <summary>出入库记录ID
        /// </summary>
        public Guid StockId { get; set; }

        /// <summary>往来单位ID
        /// </summary>
        public Guid ThirdCompanyID { get; set; }

        /// <summary>商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary> 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>当前仓库商品库存
        /// </summary>
        public int NonceWarehouseGoodsStock { get; set; }

        /// <summary>描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>单位（非常用字段，新建时不需要赋值）
        /// </summary>
        public string Units { get; set; }

        /// <summary> 批文号（非常用字段，新建时不需要赋值）
        /// </summary>
        public string ApprovalNO { get; set; }

        /// <summary>加盟价
        /// </summary>
        public decimal JoinPrice { get; set; }

        public int GoodsType { get; set; }

        /// <summary> 
        /// 创建时间
        /// </summary>
        /// zal 2016-07-08
        public DateTime DateCreated { get; set; }

        public string BatchNo { get; set; }

        public DateTime EffectiveDate { get; set; }

        public Byte ShelfType { get; set; }
    }
}
