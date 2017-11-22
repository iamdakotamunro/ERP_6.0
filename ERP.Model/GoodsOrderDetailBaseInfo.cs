using System;

namespace ERP.Model
{
    [Serializable]
    public class GoodsOrderDetailBaseInfo
    {
        public string OrderNo { get; set; }

        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public string GoodsName { get; set; }

        public string GoodsCode { get; set; }

        public int Quantity { get; set; }

        public DateTime EffectiveTime { get; set; }

        public string Consignee { get; set; }

        public Guid SaleFilialeId { get; set; }
    }
}
