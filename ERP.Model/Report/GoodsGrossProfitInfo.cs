using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 商品毛利模型
    /// </summary>
    [Serializable]
    public class GoodsGrossProfitInfo
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        ///销售总额 
        /// </summary>
        public decimal SalesPriceTotal { get; set; }

        /// <summary>
        /// 进货成本总额
        /// </summary>
        public decimal PurchaseCostTotal { get; set; }

        /// <summary>
        /// 订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)
        /// </summary>
        /// zal 2016-05-18
        public int OrderType { get; set; }

        /// <summary>
        /// 记录年月份
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        public decimal GrossProfit
        {
            get
            {
                return SalesPriceTotal - PurchaseCostTotal;
            }
        }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossProfitMargin
        {
            get
            {
                if (GrossProfit == 0 || SalesPriceTotal == 0) return 0;
                return GrossProfit / SalesPriceTotal;
            }
        }

        #region 只用于显示，不对应数据库字段
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        ///月平均结算价 
        /// </summary>
        /// zal 2016-06-20
        public decimal AvgSettlePrice { get; set; }
        #endregion
    }
}
