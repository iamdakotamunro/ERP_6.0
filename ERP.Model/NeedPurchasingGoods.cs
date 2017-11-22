using System;

namespace ERP.Model
{
    [Serializable]
    public class NeedPurchasingGoods
    {
        public String OrderNo { get; set; }

        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public Int32 Quantity { get; set; }

        public Guid PersonResponsible { get; set; }

        public NeedPurchasingGoods() { }
    }

    [Serializable]
    public class GoodsAllocateStatistic
    {
        public Guid WarehouseId { get; set; }

        public String WarehouseName { get; set; }

        public Guid PersonResponsible { get; set; }

        public String PersonnelName { get; set; }

        public Int32 OrderCount { get; set; }

        public Int32 GoodsQuantities { get; set; }

        public GoodsAllocateStatistic() { }
    }
}
