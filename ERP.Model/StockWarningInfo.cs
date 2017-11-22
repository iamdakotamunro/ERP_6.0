//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2008年6月3日
// 文件创建人:马力
// 最后修改时间:2008年6月3日
// 最后一次修改人:马力
//================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 库存预警信息类
    /// </summary>
    [Serializable]
    public class StockWarningInfo
    {
        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 当前分公司商品库存
        /// </summary>
        public double NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// 当前需求量
        /// </summary>
        public double NonceRequest { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public Int32 RequireQuantity { get; set; }

        /// <summary>
        /// 待出库数
        /// </summary>
        public Int32 SubtotalQuantity { get; set; }

        /// <summary>
        /// 当前商品总库存
        /// </summary>
        public double NonceGoodsStock { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public double SalesNumber { get; set; }

        /// <summary>
        /// 子商品是否缺货
        /// </summary>
        public bool IsScarcity { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOnShelf { set; get; }

        /// <summary>
        /// 当前仓库商品库存
        /// </summary>
        public double NonceWarehouseGoodsStock
        {
            get { return NonceFilialeGoodsStock; }
            set { NonceFilialeGoodsStock = value; }
        }

        /// <summary>
        /// 往来单位列表
        /// </summary>
        public IList<CompanyCussentInfo> ccInfo { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyId { get; set; }

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
        /// 要减去的采购数量，包含不等于未完成的采购数量(其中含有部分未采购成功的商品数量)
        /// </summary>
        public double SubtractPurchasingQuantity { get; set; }

        /// <summary>
        /// 要减去的采购数量，包含不等于未完成的采购数量(其中含有部分未采购成功的商品数量)
        /// </summary>
        public double PurchasingQuantity { get; set; }

        public double UppingQuantity { get; set; }

        public Dictionary<int, double> PerStepSales { get; set; }

        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 实际需要的采购数量
        /// </summary>
        public double RealityNeedPurchasingQuantity
        {
            get
            {
                double needPurchasingQuantity = SubtractPurchasingQuantity < 0 ? Math.Ceiling(PlanPurchasingquantity + SubtractPurchasingQuantity - (NonceWarehouseGoodsStock + UppingQuantity - RequireQuantity) + SubtotalQuantity) :
                    Math.Ceiling(PlanPurchasingquantity - SubtractPurchasingQuantity - (NonceWarehouseGoodsStock + UppingQuantity - RequireQuantity) + SubtotalQuantity);
                return needPurchasingQuantity;
            }
        }

        /// <summary>
        /// 加权日平均销量
        /// </summary>
        public double WeightedAverageSaleQuantity
        {
            /*
                            9 * N1 + 8 * N2 + 7 * N3 + 6 * N4 + 5 * N5 + 4 * N6+3 * N7 + 2 * N8 + 1*N9
            加权日平均销量 = -----------------------------------------------------------------------------
                                           10 * (9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1)
            */
            get
            {
                if (PerStepSales == null || PerStepSales.Count == 0) return 0;
                if (PerStepSales.Count == 1)
                {
                    return PerStepSales.First().Value;
                }
                var perStepSales = PerStepSales.OrderBy(act => act.Key);

                var one = perStepSales.Take(1).Sum(act => act.Value);
                var two = perStepSales.Skip(1).Take(1).Sum(act => act.Value);
                var three = perStepSales.Skip(2).Take(1).Sum(act => act.Value);
                var four = perStepSales.Skip(3).Take(1).Sum(act => act.Value);
                var five = perStepSales.Skip(4).Take(1).Sum(act => act.Value);
                var six = perStepSales.Skip(5).Take(1).Sum(act => act.Value);
                var seven = perStepSales.Skip(6).Take(1).Sum(act => act.Value);
                var eight = perStepSales.Skip(7).Take(1).Sum(act => act.Value);
                var nine = perStepSales.Skip(8).Take(1).Sum(act => act.Value);

                return (9 * one + 8 * two + 7 * three + 6 * four + 5 * five + 4 * six + 3 * seven + 2 * eight + 1 * nine) /(10 * (9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1));


                //var first = perStepSales.Take(3).Sum(act => act.Value);
                //var two = perStepSales.Skip(3).Take(3).Sum(act => act.Value);
                //var third = perStepSales.Skip(6).Take(3).Sum(act => act.Value);
                //return (3 * first + 2 * two + third) / (10 * 9 * 6);

                //var sumSaleNumber = (FirstNumberOneStockUpSale * 3 + FirstNumberTwoStockUpSale * 2 + FirstNumberThreeStockUpSale);
                //var sumWeightedNumber = ((FirstNumberOneStockUpSale == 0 ? 0 : 3) + (FirstNumberTwoStockUpSale == 0 ? 0 : 2) + (FirstNumberThreeStockUpSale == 0 ? 0 : 1));
                //return sumWeightedNumber == 0 ? 0 : (sumSaleNumber / sumWeightedNumber) / 10;
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
                if (num >= 1.3)
                {
                    return 1.1;
                }
                return num;
            }
        }
        /// <summary>
        /// 三个周期的销售平均增长率
        /// </summary>
        public double SaleAvgCrease
        {
            get
            {
                var num1 = FirstNumberThreeStockUpSale == 0 ? (FirstNumberTwoStockUpSale > 1 ? 1 : 0) : (FirstNumberTwoStockUpSale - FirstNumberThreeStockUpSale) / FirstNumberThreeStockUpSale;
                var num2 = FirstNumberTwoStockUpSale == 0 ? (FirstNumberOneStockUpSale > 1 ? 1 : 0) : (FirstNumberOneStockUpSale - FirstNumberTwoStockUpSale) / FirstNumberTwoStockUpSale;
                var num = (num1 + num2) / 2;
                if (num >= 1.3)
                {
                    return 1.1;
                }
                return Convert.ToDouble(num.ToString("f2"));
            }
        }
        /// <summary>
        /// 备货天数
        /// </summary>
        public int StockDay { get; set; }
        /// <summary>
        /// 计划采购量
        /// </summary>
        public double PlanPurchasingquantity
        {
            get
            {
                //仓储版确认去除增长率 SaleInCrease
                return WeightedAverageSaleQuantity * StockDay;
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public StockWarningInfo() { }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="goodsId">产品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceFilialeGoodsStock">指定公司的库存量</param>
        /// <param name="nonceRequest">当前需求量</param>
        /// <param name="nonceGoodsStock">总库存量</param>
        /// <param name="salesNumber">销售额</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, double nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="specification"></param>
        /// <param name="nonceFilialeGoodsStock"></param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">产品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceFilialeGoodsStock">指定公司的库存量</param>
        /// <param name="nonceGoodsStock">总库存量</param>
        /// <param name="salesNumber">销售额</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;

        }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="goodsId">产品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceFilialeGoodsStock">指定公司的库存量</param>
        /// <param name="nonceRequest">当前需求量</param>
        /// <param name="nonceGoodsStock">总库存量</param>
        /// <param name="salesNumber">销售额</param>
        public StockWarningInfo(Guid goodsId, string specification, double nonceFilialeGoodsStock, int nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">产品ID</param>
        /// <param name="goodsCode">商品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceFilialeGoodsStock">指定公司的库存量</param>
        /// <param name="nonceRequest">当前需求量</param>
        /// <param name="nonceGoodsStock">总库存量</param>
        /// <param name="salesNumber">销售额</param>
        /// <param name="goodsName">商品名</param>
        public StockWarningInfo(Guid goodsId, string goodsName, string goodsCode, string specification, double nonceFilialeGoodsStock, double nonceRequest, double nonceGoodsStock, double salesNumber)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            NonceGoodsStock = nonceGoodsStock;
            SalesNumber = salesNumber;
        }

        public void SetStepSales(Dictionary<int, double> perDoubles)
        {
            PerStepSales = perDoubles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is StockWarningInfo)
                return (compareObj as StockWarningInfo).GoodsId == GoodsId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (GoodsId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + GoodsId.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
