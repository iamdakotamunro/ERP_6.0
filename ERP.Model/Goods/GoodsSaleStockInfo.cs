using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 产品分类模型
    /// </summary>
    [Serializable]
    public class GoodsSaleStockInfo
    {

        /// <summary>
        /// 主键
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }
        
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        public string ApplyReason { get; set; }
        
        /// <summary>
        /// 申请人
        /// </summary>
        public Guid Applicant { get; set; }
        
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
        
        /// <summary>
        /// 审核人
        /// </summary>
        public Guid Auditor { get; set; }
        
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }
        
        /// <summary>
        /// 审核理由
        /// </summary>
        public string AuditReason { get; set; }

        /// <summary>
        /// 卖库存状态
        /// </summary>
        public int SaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SaleStockState { get; set; }

        /// <summary>
        /// 补货周期
        /// </summary>
        public int ReplenishmentCycle { get; set; }
    }

    /// <summary>
    /// 卖库存管理 Grid
    /// </summary>
    [Serializable]
    public class GoodsSaleStockGridModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OldSaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NewSaleStockType { get; set; }
    }
}
