using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 采购申请
    /// 作者：刘彩军
    /// 时间：2012-06-20
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApplyStockInfo
    {
        /// <summary>
        /// 单据ID
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 申请采购单据号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// 单据所在公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }
        /// <summary>
        /// 所在公司名
        /// </summary>
        [DataMember]
        public string FilialeName { get; set; }
        /// <summary>
        /// 入库仓库
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }
        /// <summary>
        /// 入库仓库
        /// </summary>
        [DataMember]
        public string WarehouseName { get; set; }
        /// <summary>
        /// 出库公司
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 出库仓库
        /// </summary>
        [DataMember]
        public Guid CompanyWarehouseId { get; set; }
        /// <summary>
        /// 出库公司名
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// 申请调拨人
        /// </summary>
        [DataMember]
        public string Transactor { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public double SubtotalQuantity { get; set; }
        /// <summary>
        /// 所属主单
        /// </summary>
        [DataMember]
        public Guid ParentApplyId { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        [DataMember]
        public int StockState { get; set; }
        /// <summary>
        /// 出库单据ID
        /// </summary>
        [DataMember]
        public Guid SemiStockId { get; set; }
        /// <summary>
        /// 出库单据号
        /// </summary>
        [DataMember]
        public string SemiStockCode { get; set; }

        /// <summary>
        /// 采购单类型，1：订单类型，2：采购类型
        /// </summary>
        [DataMember]
        public int PurchaseType { get; set; }

        /// <summary>
        /// 临时地址
        /// </summary>
        [DataMember]
        public string Direction { get; set; }
    }
}
