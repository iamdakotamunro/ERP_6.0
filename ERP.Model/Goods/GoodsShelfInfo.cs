using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsShelfInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShelfNo { get; set; }
    }
}
