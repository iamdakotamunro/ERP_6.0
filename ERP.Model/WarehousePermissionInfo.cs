using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class WarehousePermissionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid BranchId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid PositionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid WarehouseId { get; set; }
    }
}
