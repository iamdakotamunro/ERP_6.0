using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 出入库单据模型
    /// 作者：刘彩军
    /// 时间：2012-06-27
    /// </summary>
    [Serializable]
    public class StockInfo
    {
        /// <summary>
        /// 出入库单据ID
        /// </summary>
        public Guid StockId { get; set; }
        /// <summary>
        /// 所属公司ID
        /// </summary>
        public Guid FilialeId { get; set; }
        /// <summary>
        /// 所属仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }
        /// <summary>
        /// 出入库单据号
        /// </summary>
        public String TradeCode { get; set; }
        /// <summary>
        /// 关联单据号
        /// </summary>
        public String OriginalCode { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Transactor { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 申请总数
        /// </summary>
        public int SubtotalQuantity { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public int StockState { get; set; }
        /// <summary>
        /// 采购申请单号ID
        /// </summary>
        public Guid ApplyId { get; set; }
        /// <summary>
        /// 单据类型（出库/入库）
        /// </summary>
        public int StockType { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public String FilialeName { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public String WarehouseName { get; set; }
        /// <summary>
        /// 出库单这个字段为入库公司ID，入库单这个字段为出库公司ID
        /// </summary>
        public Guid CompanyFilialeId { get; set; }
        /// <summary>
        /// 出库单这个字段为入库仓库ID，入库单这个字段为出库仓库ID
        /// </summary>
        public Guid CompanyWarehouseId { get; set; }
        /// <summary>
        /// 出库单这个字段为入库公司名称，入库单这个字段为出库公司名称(此字段插入时不需要，仅供读取数据使用)
        /// </summary>
        public String CompanyFilialeName { get; set; }
        /// <summary>
        /// 出库单这个字段为入库仓库名称，入库单这个字段为出库仓库名称(此字段插入时不需要，仅供读取数据使用)
        /// </summary>
        public String CompanyWarehouseName { get; set; }

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool IsError { get; set; }
    }
}
