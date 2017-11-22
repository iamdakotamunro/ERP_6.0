using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一  商品出入库记录详细模型  最后修改提交 陈重文  2014-12-25   （更新、删除、新增字段）
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorageRecordDetailInfo
    {

        /// <summary>出入库记录ID
        /// </summary>
        [DataMember]
        public Guid StockId { get; set; }

        /// <summary>往来单位ID
        /// </summary>
        [DataMember]
        public Guid ThirdCompanyID { get; set; }

        /// <summary>商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>子商品ID
        /// </summary>
        [DataMember]
        public Guid RealGoodsId { get; set; }

        /// <summary>规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

        /// <summary> 数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>单价
        /// </summary>
        [DataMember]
        public decimal UnitPrice { get; set; }

        /// <summary>当前仓库商品库存
        /// </summary>
        [DataMember]
        public int NonceWarehouseGoodsStock { get; set; }

        /// <summary>描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>商品编号
        /// </summary>
        [DataMember]
        public string GoodsCode { get; set; }

        /// <summary>单位（非常用字段，新建时不需要赋值）
        /// </summary>
        [DataMember]
        public string Units { get; set; }

        /// <summary> 批文号（非常用字段，新建时不需要赋值）
        /// </summary>
        [DataMember]
        public string ApprovalNO { get; set; }

        /// <summary>加盟价
        /// </summary>
        [DataMember]
        public decimal JoinPrice { get; set; }

        [DataMember]
        public int GoodsType { get; set; }

        /// <summary> 
        /// 创建时间
        /// </summary>
        /// zal 2016-07-08
        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public string BatchNo { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public Byte ShelfType { get; set; }
    }
}
