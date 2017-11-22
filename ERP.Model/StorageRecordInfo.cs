using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一  商品出入库记录模型  最后修改提交 陈重文  2014-12-25   （更新、删除、新增字段）
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorageRecordInfo
    {
        /// <summary>记录ID
        /// </summary>
        [DataMember]
        public Guid StockId { get; set; }

        /// <summary>公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 入库储
        /// </summary>
        [DataMember]
        public int StorageType { get; set; }

        /// <summary>往来单位ID
        /// </summary>
        [DataMember]
        public Guid ThirdCompanyID { get; set; }

        /// <summary>单据编号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>原始单据号
        /// </summary>
        [DataMember]
        public string LinkTradeCode { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        /// zal 2016-12-22
        [DataMember]
        public string LogisticsCode { get; set; }

        /// <summary> 创建时间
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary> 处理人
        /// </summary>
        [DataMember]
        public string Transactor { get; set; }

        /// <summary>单据描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary> 总金额
        /// </summary>
        [DataMember]
        public decimal AccountReceivable { get; set; }

        /// <summary>总数量
        /// </summary>
        [DataMember]
        public decimal SubtotalQuantity { get; set; }

        /// <summary>出入库类型，参见<see cref="ERP.Enum.StorageRecordType"/>
        /// </summary>
        [DataMember]
        public int StockType { get; set; }

        /// <summary>出入库状态，参见<see cref="ERP.Enum.StorageRecordState"/>
        /// </summary>
        [DataMember]
        public int StockState { get; set; }

        /// <summary> 操作仓库
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary> 内部关联公司
        /// </summary>
        [DataMember]
        public Guid RelevanceFilialeId { get; set; }

        /// <summary> 内部关联仓库
        /// </summary>
        [DataMember]
        public Guid RelevanceWarehouseId { get; set; }

        /// <summary>原始单据ID
        /// </summary>
        [DataMember]
        public Guid LinkTradeID { get; set; }

        /// <summary>待确认
        /// </summary>
        [DataMember]
        public bool StockValidation { get; set; }

        /// <summary>审核时间
        /// </summary>
        [DataMember]
        public DateTime AuditTime { get; set; }

        /// <summary>生成出入库记录的原始单据类型，参见<see cref="ERP.Enum.StorageRecordLinkTradeType"/>
        /// </summary>
        [DataMember]
        public int LinkTradeType { get; set; }

        /// <summary>
        /// 是否报税
        /// </summary>
        /// <remarks>目前所有都要报税 By Jerry Bai 2017/6/2</remarks>

        [IgnoreDataMember]
        public bool IsOut
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// 进出单据号
        /// </summary>
        [DataMember]
        public string BillNo { get; set; }

        /// <summary>
        /// 交易双方类型，参见<see cref="ERP.Enum.TradeBothPartiesType"/>
        /// </summary>
        [DataMember]
        public int TradeBothPartiesType { get; set; }
    }
}
