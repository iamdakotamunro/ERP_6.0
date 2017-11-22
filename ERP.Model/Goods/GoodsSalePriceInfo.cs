using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 主商品销售价格
    /// </summary>
    [Serializable]
    public class GoodsSalePriceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsGroupInfo GoodsGroupInfo { get; set; }

        /// <summary>
        /// 会员等级价格
        /// </summary>
        public IList<GoodsRolePriceInfo> GoodsRolePriceList { get; set; }

        /// <summary>
        /// 第三方商品价格
        /// </summary>
        public List<ThirdGoodsSalePriceInfo> ThirdPriceList { get; set; }
    }

    /// <summary>
    /// 设置价格
    /// </summary>
    [Serializable]
    public class DealGoodsSalePriceInfo
    {
        /// <summary>
        /// 
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
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 参考价
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// 加盟价
        /// </summary>
        public decimal JoinPrice { get; set; }

        /// <summary>
        /// 隐性成本
        /// </summary>
        public decimal ImplicitCost { get; set; }

        /// <summary>
        /// 年终扣率
        /// </summary>
        public decimal YearDiscount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<GoodsSalePriceInfo> GroupGoodsPriceList { get; set; }
    }
}
