using System;

namespace SaleFilialeGenerateStockInAndPurchaseForm.Model
{
    [Serializable]
    public class SaleFilialeWarehouseStorageTypeInfo
    {
        public SaleFilialeWarehouseStorageTypeInfo() { }

        /// <summary>
        /// 销售公司对应的往来单位
        /// </summary>
        public Guid SaleFilialeThirdCompanyID { get; set; }

        public Guid HostingFilialeId { get; set; }

        public Guid WarehouseId { get; set; }

        public int StorageType { get; set; }
    }
}
