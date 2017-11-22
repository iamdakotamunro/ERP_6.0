using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CompleteOrderTaskRecordInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid TaskId { get; set; }

        public Guid HostingFilialeId { get; set; }
    }
}
