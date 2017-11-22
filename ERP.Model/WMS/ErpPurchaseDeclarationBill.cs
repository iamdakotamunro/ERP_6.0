using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.WMS
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ErpPurchaseDeclarationBill
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UppingQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Detail> OrderBills { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, int> StorageBills { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class Detail
        {
            public Guid FilialeId { get; set; }

            public String OrderNo { get; set; }

            public Guid OrderId { get; set; }

            public int Quantity { get; set; }

            public int OrderState { get; set; }

            public Detail(string orderNo, Guid orderId, int quantity, int orderState,Guid hostingFilialeId)
            {
                OrderNo = orderNo;
                OrderId = orderId;
                Quantity = quantity;
                OrderState = orderState;
                FilialeId = hostingFilialeId;
            }
        }
    }
}
