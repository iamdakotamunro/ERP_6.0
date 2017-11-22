using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPurchasing.Core.Model
{
    public class ChildGoodsSalePurchasing
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 预警天数内预估的销售数量
        /// </summary>
        public double WarningDaysSaleQuantity { get; set; }

        /// <summary>
        /// 前第一个备货周期的销售记录
        /// </summary>
        public double FirstNumberOneStockUpSale { get; set; }

        /// <summary>
        /// 前第二个备货周期的销售记录
        /// </summary>
        public double FirstNumberTwoStockUpSale { get; set; }

        /// <summary>
        /// 前第三个备货周期的销售记录
        /// </summary>
        public double FirstNumberThreeStockUpSale { get; set; }

        /// <summary>
        /// 计划采购的数量
        /// </summary>
        public double PlanPurchasingquantity { get; set; }

        /// <summary>
        /// 要减去的采购数量，包含不等于未完成的采购数量(其中含有部分未采购成功的商品数量)
        /// </summary>
        public double SubtractPurchasingQuantity { get; set; }

        /// <summary>
        /// 当前库存
        /// </summary>
        public double NonceWarehouseStockQuantity { get; set; }

        /// <summary>
        /// 实际需要的采购数量
        /// </summary>
        public double RealityNeedPurchasingQuantity
        {
            get
            {
                if (SubtractPurchasingQuantity < 0)
                {
                    return Math.Ceiling(PlanPurchasingquantity + SubtractPurchasingQuantity - NonceWarehouseStockQuantity );
                }
                return Math.Ceiling(PlanPurchasingquantity - SubtractPurchasingQuantity - NonceWarehouseStockQuantity );
            }
        }

        public Dictionary<int, double> PerStepSales { get; set; } 

        /// <summary>
        /// 加权平均销量
        /// </summary>
        public double WeightedAverageSaleQuantity
        {
            get
            {
                if (PerStepSales == null || PerStepSales.Count == 0) return 0;
                if (PerStepSales.Count == 1)
                {
                    return PerStepSales.First().Value;
                }
                var preStepSales = PerStepSales.OrderBy(act => act.Key);
                var first = preStepSales.Take(3).Sum(act => act.Value);
                var two = preStepSales.Skip(3).Take(3).Sum(act => act.Value);
                var third = preStepSales.Skip(6).Take(3).Sum(act => act.Value);
                return (3 * first + 2 * two + third) / (10 * 9 * 6);
                //var sumSaleNumber = (FirstNumberOneStockUpSale * 3 + FirstNumberTwoStockUpSale * 2 + FirstNumberThreeStockUpSale);
                //var sumWeightedNumber = ((FirstNumberOneStockUpSale == 0 ? 0 : 3) + (FirstNumberTwoStockUpSale == 0 ? 0 : 2) + (FirstNumberThreeStockUpSale == 0 ? 0 : 1));
                //if (sumWeightedNumber==0)
                //{
                //    return 0;
                //}
                //return sumSaleNumber / sumWeightedNumber;
            }
        }

        /// <summary>
        /// 三个周期的销售增长率
        /// </summary>
        public double SaleInCrease
        {
            get
            {
                var num1 = (FirstNumberTwoStockUpSale == 0 ? 1 : FirstNumberTwoStockUpSale);
                var num2 = (FirstNumberThreeStockUpSale == 0 ? 1 : FirstNumberThreeStockUpSale);
                var num = (FirstNumberOneStockUpSale / num1 + FirstNumberTwoStockUpSale / num2) / 2;
                if (num >=1.3)
                {
                    return 1.1;
                }
                return num;
            }
        }

        /// <summary>
        /// 60天日均销量
        /// </summary>
        public int SixtyDaySales { get; set; }
        /// <summary>
        /// 30天日均销量
        /// </summary>
        public int ThirtyDaySales { get; set; }
        /// <summary>
        /// 11天日均销量
        /// </summary>
        public int ElevenDaySales { get; set; }

        public Guid HostingFilialeId { get; set; }
    }
}
