using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>商品需求查询出库量
    /// </summary>
    [Serializable]
    public class RequirementOrderInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String No { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }
    }
}
