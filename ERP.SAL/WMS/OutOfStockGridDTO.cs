using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ERP.SAL.WMS
{
    [Serializable]
   public  class OutOfStockGridDTO
    {
        public Guid GoodsId { get; set; }

       public Guid RealGoodsId { get; set; }

       public String GoodsName { get; set; }

       public String GoodsCode { get; set; }

       public String Sku { get; set; }

       public int Quantity { get; set; }

       public Guid HostingFilialeId { get; set; }

        public String Units { get; set; }

        public string TransferFiliale
        {
            get
            {
                if (TransferFiliales != null && TransferFiliales.Count > 0)
                {
                    if (TransferFiliales.Count == 1)
                        return TransferFiliales.First().HostingFilialeName;
                    return TransferFiliales.OrderByDescending(act => act.Quantity).Aggregate("", (current, transferFiliale) => current + string.Format("{0}{1}", current.Length == 0 ? "" : "<hr/>", transferFiliale.HostingFilialeName));
                }
                return "-";
            }
        }

        public string TransferQuantity
        {
            get
            {
                if (TransferFiliales != null && TransferFiliales.Count > 0)
                {
                    if (TransferFiliales.Count == 1)
                        return string.Format("{0}", TransferFiliales.First().Quantity);
                    return TransferFiliales.OrderByDescending(act => act.Quantity).Aggregate("", (current, transferFiliale) => current + string.Format("{0}{1}", current.Length == 0 ? "" : "<hr/>", transferFiliale.Quantity));
                }
                return "-";
            }
        }

        public List<FilialeQuantity> TransferFiliales { get; set; }
    }

    [Serializable]
    public class FilialeQuantity
    {
        public Guid HostingFilialeId { get; set; }

        public string HostingFilialeName { get; set; }

        public int Quantity { get; set; }
    }
}
