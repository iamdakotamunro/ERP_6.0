using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.WMS
{
    [Serializable]
    [DataContract]
    public class WMSReturnGoodsRequest
    {
        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public string BillNo { get; set; }

        [DataMember]
        public string OutGoodsBillNo { get; set; }

        [DataMember]
        public string OperatorName { get; set; }

        [DataMember]
        public Dictionary<Guid,int> StockQuantitys { get; set; }

        [DataMember]
        public List<WMSReturnGoodsDetailRequest> Details { get; set; }
    }
}
