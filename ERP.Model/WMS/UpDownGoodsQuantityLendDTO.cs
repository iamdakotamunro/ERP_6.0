using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.WMS
{
    [Serializable]
    public class UpDownGoodsQuantityLendDTO
    {
        public Guid HostingFilialeId { get; set; }

        public Guid RealGoodsId { get; set; }

        public int Quantity { get; set; }

        public byte ShelfType { get; set; }

        public UpDownGoodsQuantityLendDTO() { }

        public UpDownGoodsQuantityLendDTO(Guid hostingFilialeId, Guid realGoodsId,int quantity,byte shelfType)
        {
            HostingFilialeId = hostingFilialeId;
            RealGoodsId = realGoodsId;
            Quantity = quantity;
            ShelfType = shelfType;
        }
    }
}
