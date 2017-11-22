using System;
using System.Collections.Generic;

namespace ERP.SAL.WMS
{
    [Serializable]
    public class ERPGoodsDemand
    {
     /// <summary>
        /// 出货中订单列表
        /// </summary>
        public List<ERPDemandBill> OrderBills { get; set; }

        /// <summary>
        /// 出货中的对应的出库单号列表
        /// </summary>
        public List<string> SourceNos { get; set; }
    }

    [Serializable]
    public class ERPDemandBill
    {
        public string SourceNo { get; set; }

        public string BillNo { get; set; }

        public string BillState { get; set; }
    }
}
