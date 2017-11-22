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
    public class CompanyBalanceDetailInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal NonceBalance { get; set; }
    }
}
