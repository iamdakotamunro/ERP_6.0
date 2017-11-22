using System;
using System.Collections.Generic;

namespace ERP.SAL.WMS
{
    [Serializable]
    public class ERPAllocateDTO
    {
        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public Dictionary<Guid, int> StockQuantity { get; set; }

        public List<ERPAllocateDetailDTO> Details { get; set; }
    }

    [Serializable]
    public class ERPAllocateDetailDTO
    {
        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public string OrderNo { get; set; }

        public Guid PersonResponsibleId { get; set; }

        public String PersonResponsible { get; set; }
    }

    [Serializable]
    public class ERPAllocateOrderDTO
    {
        public Dictionary<Guid, int> StockQuantitys { get; set; }

        public List<ERPAllocateOrderInfo> Details { get; set; }


        public Dictionary<Guid, string> HostingFiliales { get; set; }
    }

    [Serializable]
    public class ERPAllocateOrderInfo
    {
        public Guid GoodsId { get; set; }

        public Guid RealGoodsId { get; set; }

        public string OrderNo { get; set; }

        public string Consignee { get; set; }

        public Guid HostingFilialeId { get; set; }

        public DateTime OrderTime { get; set; }
    }
}
