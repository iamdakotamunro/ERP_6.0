using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.WMS
{
    [Serializable]
    public class RequireSearchDTO
    {
        public Guid HostingFilialeId { get; set; }

        public String HostingFilialeName { get; set; }

        public Dictionary<Guid, int> RequiresDics { get; set; }

        public Dictionary<Guid, int> StockQuantity { get; set; }

        public RequireSearchDTO() { }

        public RequireSearchDTO(Guid hostingFilialeId,string hostingFilialeName,Dictionary<Guid,int> requiresDics,Dictionary<Guid,int> stockQuantity)
        {
            HostingFilialeId = hostingFilialeId;
            HostingFilialeName = hostingFilialeName;
            RequiresDics = requiresDics;
            StockQuantity = stockQuantity;
        }
    }
}
