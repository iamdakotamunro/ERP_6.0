#if !WCFModel
using Dapper.Extension;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.ASYN
{
    /// <summary>
    /// 
    /// </summary>
#if !WCFModel
    [TypeMapper]
#endif
    public class ASYNStorageRecordInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StorageType { get; set; }

        /// <summary>
        /// 
        /// </summary>
#if !WCFModel
        [Column("StockState")]
#endif
        public int StorageState { get; set; }

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
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValidateStock { get; set; }
    }
}
