using System;
using System.Runtime.Serialization;

namespace ERP.Model.WMS
{
    [Serializable]
    [DataContract]
    public class WMSReturnGoodsDetailRequest
    {
        [DataMember]
        public Guid RealGoodsId { get; set; }

        [DataMember]
        public Guid GoodsId { get; set; }

        [DataMember]
        public String BatchNo { get; set; }

        [DataMember]
        public DateTime ExpiryDate { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public Byte ShelfType { get; set; }
    }
}
