using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.WMS
{
    [Serializable]
    [DataContract]
    public class WMSLostBackReturnDTO
    {
        [DataMember]
        public List<Guid> InvoiceIds { get; set; }

        [DataMember]
        public List<String> OrderNos { get; set; }

        [DataMember]
        public String OutGoodsBillNo { get; set; }

        [DataMember]
        public string OperatorName { get; set; }

        [DataMember]
        public List<LostBackDTO> Details { get; set; }

        [DataMember]
        public string CancelReasons { get; set; }
    }

    [Serializable]
    [DataContract]
    public class LostBackDTO
    {
        [DataMember]
        public Byte StorageType { get; set; }

        [DataMember]
        public String BillNo { get; set; }

        [DataMember]
        public Dictionary<Guid,int> StockQuantitys { get; set; }

        [DataMember]
        public List<WMSReturnGoodsDetailRequest> Details { get; set; }
    }
}
