using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.Finance
{
    /// <summary>
    /// 发票
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceDTO
    {
        [DataMember]
        public string TradeCode { get; set; }

        [DataMember]
        public DateTime FinishTime { get; set; }

        [DataMember]
        public Guid FilialeId { get; set; }

        [DataMember]
        public Guid ThirdCompanyId { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public Guid AuditorId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public List<Detail> Details { get; set; }

        public class Detail
        {
            public int Id { get; set; }

            public Guid GoodsId { get; set; }

            public string GoodsCode { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }
        }
    }
}
