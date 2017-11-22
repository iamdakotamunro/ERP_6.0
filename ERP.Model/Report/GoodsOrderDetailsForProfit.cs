using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 商品订单明细for记录商品毛利
    /// </summary>
    [Serializable]
    public class GoodsOrderDetailsForProfit
    {
        #region 为后续查询判断使用，不对应数据库字段
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }
        #endregion

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 自商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 销售总金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 满减金额
        /// </summary>
        public decimal PromotionValue { get; set; }

        public Guid SaleFilialeId { get; set; }

        public Guid SalePlatformId { get; set; }
    }
}
