using System;
using System.Collections.Generic;

namespace ERP.SAL.WMS
{
    /// <summary>
    /// 拆分组合单DTO
    /// </summary>
    [Serializable]
    public class CombineSplitBillDTO
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 物流配送公司
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 主商品数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否为拆分类型的单据，如果不是则表示为组合类的单据
        /// </summary>
        public bool IsSplit { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public IList<CombineSplitBillDetailDTO> Details { get; set; }
    }
}
