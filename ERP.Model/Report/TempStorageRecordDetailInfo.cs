using System;
namespace ERP.Model.Report
{
    /// <summary>
    /// 临时出入库明细模型
    /// </summary>
    [Serializable]
    public class TempStorageRecordDetailInfo
    {
        public Guid ID { get; set; }

        public Guid GoodsId{ get; set; }

        public Guid RealGoodsId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public Guid WarehouseId { get; set; }

        public int StockType { get; set; }
    }
}
