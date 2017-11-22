using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.Finance
{
    /// <summary>
    /// 出入库单
    /// </summary>
    [Serializable]
    [DataContract]
    public class StockBillDTO
    {
        /// <summary>
        /// 出入库单号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 往来单位
        /// </summary>
        [DataMember]
        public Guid ThirdCompanyId { get; set; }

        /// <summary>
        /// 往来单位编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        [DataMember]
        public string Transactor { get; set; }

        /// <summary>
        /// 创建时间 格式：yyyy-MM-dd
        /// </summary>
        [DataMember] 
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 出入库单备注
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool HasInvoice { get; set; }

        /// <summary>
        /// 出入库明细
        /// </summary>
        [DataMember]
        public List<Detail> Details { get; set; } 

        public class Detail
        {
            public Guid GoodsId { get; set; }

            public string GoodsCode { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; } 
        }
    }
}
