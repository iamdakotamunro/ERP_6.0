using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    public class ProcessOrderCertificationInfo
    {
        /// <summary>加工单号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>订单号(合并发货的可能会有多个)
        /// </summary>
        public List<string> OrderNos { get; set; }

        /// <summary>加工者
        /// </summary>
        public string Processor { get; set; }

        /// <summary>加工时间
        /// </summary>
        public DateTime ProcessDate { get; set; }

        /// <summary>收货人
        /// </summary>
        public string Name { get; set; }

        /// <summary>托管公司ID
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>加工单明细SKU信息集
        /// </summary>
        public List<string> SkuList { get; set; }
    }
}
