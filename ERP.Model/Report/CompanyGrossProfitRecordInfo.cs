using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 公司毛利
    /// </summary>
    /// zal 2016-05-20
    [Serializable]
    public class CompanyGrossProfitRecordInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal SalesAmount { get; set; }

        /// <summary>
        /// 商品金额
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        ///运费收入 
        /// </summary>
        public decimal ShipmentIncome { get; set; }

        /// <summary>
        /// 促销抵扣
        /// </summary>
        public decimal PromotionsDeductible { get; set; }

        /// <summary>
        /// 积分抵扣
        /// </summary>
        public decimal PointsDeduction { get; set; }

        /// <summary>
        /// 运费成本
        /// </summary>
        public decimal ShipmentCost { get; set; }
        /// <summary>
        /// 进货成本
        /// </summary>
        public decimal PurchaseCosts { get; set; }
        /// <summary>
        /// 天猫佣金
        /// </summary>
        public decimal CatCommission { get; set; }

        /// <summary>
        /// 订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 记录年月份
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 毛利=(销售金额-运费收入)-进货成本
        /// </summary>
        public decimal Profit
        {
            get
            {
                return (SalesAmount - ShipmentIncome) - PurchaseCosts;
            }
        }

        /// <summary>
        /// 毛利率=毛利/(销售金额-运费收入)
        /// </summary>
        public decimal ProfitMargin
        {
            get
            {
                if (Profit == 0 || (SalesAmount - ShipmentIncome) == 0) return 0;
                return Profit / (SalesAmount - ShipmentIncome);
            }
        }
    }
}
