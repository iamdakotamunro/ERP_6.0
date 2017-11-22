using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsPriceInfo
    {
        /// <summary>
        /// 商品信息记录ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 最后的采购价
        /// </summary>
        public decimal LastPurchasePrice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string InterestRate
        {
            get
            {
                var keekeprice = KeedeSellPrice;
                var eyeseeprice = EyeseeSellPrice;
                var purchasePrice = LastPurchasePrice;
                if (purchasePrice == 0) return "-";
                var price = keekeprice;
                if (keekeprice == 0) price = eyeseeprice;
                var interestRate = (price - purchasePrice)/purchasePrice*100;
                if (interestRate >= 0)
                {
                    return Math.Round(interestRate, 2) + "%";
                }
                return "<font color='red'>" + Math.Round(interestRate, 2) + "%" + "</font>";
            }
        }

        /// <summary>
        /// 可得销售价
        /// </summary>
        public decimal KeedeSellPrice { get; set; }

        /// <summary>
        /// 可得差额价
        /// </summary>
        public decimal KeedeDifferencePrice { get; set; }

        /// <summary>
        /// 艾视销售价
        /// </summary>
        public decimal EyeseeSellPrice { get; set; }

        /// <summary>
        /// 可得差额价
        /// </summary>
        public decimal EyeseeDifferencePrice { get; set; }

    }

    /// <summary>
    /// 商品的抓取价格简要信息
    /// </summary>
    [Serializable]
    public class GoodsFetchPriceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal GoodsPrice { get; set; }
    }
}
