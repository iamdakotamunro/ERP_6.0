using System;

namespace ERP.Model.WMS
{
    [Serializable]
    public class HostingFilialeSaleDetailDTO
    {
        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public string GoodsName { get; set; }

        public string GoodsCode { get; set; }

        public string Sku { get; set; }

        public Guid HostingFilialeId { get; set; }

        public string HostingFilialeName { get; set; }

        public decimal SalePrice { get; set; }

        public int Quantity { get; set; }
    }
}
