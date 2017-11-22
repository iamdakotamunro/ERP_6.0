using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class WaitConsignmentOrderInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
