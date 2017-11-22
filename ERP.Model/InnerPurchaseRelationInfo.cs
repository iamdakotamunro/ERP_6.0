using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class InnerPurchaseRelationInfo
    {
        /// <summary>出库单ID
        /// </summary>
        public Guid OutStockId { get; set; }

        /// <summary>入库单ID
        /// </summary>
        public Guid InStockId { get; set; }

        /// <summary>采购单ID
        /// </summary>
        public Guid PurchasingId { get; set; }

        /// <summary>出库仓单ID
        /// </summary>
        public Guid OutWarehouseId { get; set; }

        /// <summary>出库公司单ID
        /// </summary>
        public Guid OutHostingFilialeId { get; set; }

        /// <summary>出库储位单ID
        /// </summary>
        public int OutStorageType { get; set; }

        /// <summary>入库仓单ID
        /// </summary>
        public Guid InWarehouseId { get; set; }

        /// <summary>入库公司单ID
        /// </summary>
        public Guid InHostingFilialeId { get; set; }

        /// <summary>入库储位单ID
        /// </summary>
        public int InStorageType { get; set; }

    }
}
