using System;

namespace ERP.Model.ASYN
{
    /// <summary>
    /// 
    /// </summary>
    public class ASYNReckoningInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdentifyKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid IdentifyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReckoningFromType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
