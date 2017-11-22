using System;

namespace ERP.Model
{
    /// <summary>
    /// 商品采购设置——采购分组
    /// </summary>
    [Serializable]
    public class CompanyPurchaseGoupInfo
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 采购分组ID，ID为空表示为“默认”
        /// </summary>
        public Guid PurchaseGroupId { get; set; }

        /// <summary>
        /// 采购分组名称
        /// </summary>
        public string PurchaseGroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
