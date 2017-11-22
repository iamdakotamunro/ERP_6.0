using System;
using System.Collections.Generic;
using System.Linq;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;

namespace ERP.SAL.B2CModel
{
    [Serializable]
    public class DemandOrderInfo:GoodsOrderInfo
    {
        public string DownBillNo
        {
            get
            {
                if (ProcessOrders == null || ProcessOrders.Count==0) return "-";
                if (ProcessOrders.Count == 1) return ProcessOrders.First().BillNo;
                return ProcessOrders.Aggregate("", (current, processOrder) => current + string.Format("{0}{1}", current.Length == 0 ? "" : "<hr/>", processOrder.BillNo));
            }
        }

        public string DownBillState
        {
            get
            {
                if (ProcessOrders == null || ProcessOrders.Count == 0) return "-";
                if (ProcessOrders.Count == 1) return ProcessOrders.First().BillState;
                return ProcessOrders.Aggregate("", (current, processOrder) => current + string.Format("{0}{1}", current.Length == 0 ? "" : "<hr/>", processOrder.BillState));
            }
        }

        public List<ERPDemandBill> ProcessOrders { get; set; }
    }
}
