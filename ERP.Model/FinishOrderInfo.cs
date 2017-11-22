using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FinishOrderInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string TaskDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Operationer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFinish { get; set; }
    }
}
