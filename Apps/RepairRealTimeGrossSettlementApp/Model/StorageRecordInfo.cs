using System;

namespace RepairRealTimeGrossSettlementApp.Model
{
    [Serializable]
    public class StorageRecordInfo
    {
        /// <summary>记录ID
        /// </summary>
        public Guid StockId { get; set; }

        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 入库储
        /// </summary>
        public int StorageType { get; set; }

        /// <summary>往来单位ID
        /// </summary>
        public Guid ThirdCompanyID { get; set; }

        /// <summary>单据编号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>原始单据号
        /// </summary>
        public string LinkTradeCode { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        /// zal 2016-12-22
        public string LogisticsCode { get; set; }

        /// <summary> 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary> 处理人
        /// </summary>
        public string Transactor { get; set; }

        /// <summary>单据描述
        /// </summary>
        public string Description { get; set; }

        /// <summary> 总金额
        /// </summary>
        public decimal AccountReceivable { get; set; }

        /// <summary>总数量
        /// </summary>
        public decimal SubtotalQuantity { get; set; }

        /// <summary>出入库类型，参见<see cref="ERP.Enum.StorageRecordType"/>
        /// </summary>
        public int StockType { get; set; }

        /// <summary>出入库状态，参见<see cref="ERP.Enum.StorageRecordState"/>
        /// </summary>
        public int StockState { get; set; }

        /// <summary> 操作仓库
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary> 内部关联公司
        /// </summary>
        public Guid RelevanceFilialeId { get; set; }

        /// <summary> 内部关联仓库
        /// </summary>
        public Guid RelevanceWarehouseId { get; set; }

        /// <summary>原始单据ID
        /// </summary>
        public Guid LinkTradeID { get; set; }

        /// <summary>待确认
        /// </summary>
        public bool StockValidation { get; set; }

        /// <summary>审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }

        /// <summary>生成出入库记录的原始单据类型，参见<see cref="ERP.Enum.StorageRecordLinkTradeType"/>
        /// </summary>
        public int LinkTradeType { get; set; }

        /// <summary>
        /// 是否报税
        /// </summary>
        /// <remarks>目前所有都要报税 By Jerry Bai 2017/6/2</remarks>

        /// <summary>
        /// 进出单据号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 交易双方类型，参见<see cref="ERP.Enum.TradeBothPartiesType"/>
        /// </summary>
        public int TradeBothPartiesType { get; set; }
    }
}
