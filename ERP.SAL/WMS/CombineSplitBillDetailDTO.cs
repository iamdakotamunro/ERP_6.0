using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /// <summary>
    /// 拆分组合明细
    /// </summary>
    [Serializable]
    public class CombineSplitBillDetailDTO
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Quantity { get; set; }
    }
}
