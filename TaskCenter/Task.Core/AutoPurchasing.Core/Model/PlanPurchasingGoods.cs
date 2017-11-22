using System;

namespace AutoPurchasing.Core.Model
{
    [Serializable]
    public class PlanPurchasingGoods
    {
        public Guid GoodsID { get; set; }

        //public Guid FilialeId { get; set; }

        public int PurchasingQuantity { get; set; }
    }
}
