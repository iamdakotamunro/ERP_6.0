using System;
using System.Runtime.Serialization;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 商品所有属性
    /// </summary>
    [Serializable]
    public class GoodsAllInfo
    {
        //史上最全的GoodsInfo,共35个字段。

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string GoodsCode { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public Guid BrandId { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [DataMember]
        public string Units { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember]
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>
        /// 简单描述
        /// </summary>
        [DataMember]
        public string Explain { get; set; }

        /// <summary>
        /// 上架时间，记录商品第一次上架时间
        /// </summary>
        [DataMember]
        public DateTime UsefulLifeUpper { get; set; }

        /// <summary>
        /// 下架时候，修改此时间
        /// </summary>
        [DataMember]
        public DateTime UsefulLifeLower { get; set; }

        /// <summary>
        /// 起销数量（一单至少销售几个）.先前台只控制商品展示个数，未真正限制个数。
        /// </summary>
        [DataMember]
        public double MinSellNumber { get; set; }

        /// <summary>
        /// 限销数量（一单最多销售个数）。0为不限。
        /// </summary>
        [DataMember]
        public double MaxSellNumber { get; set; }

        /// <summary>
        /// 可销订单额。订单满足了多少金额，才能购买这个商品。
        /// </summary>
        [DataMember]
        public double OrdersNeedAmount { get; set; }

        /// <summary>
        /// 库存上限
        /// </summary>
        [DataMember]
        public double StockUpperLimit { get; set; }

        /// <summary>
        /// 库存下限
        /// </summary>
        [DataMember]
        public double StockLowerLimit { get; set; }

        /// <summary>
        /// 库存状况：如果商品缺货，显示在前台的文字信息。
        /// </summary>
        [DataMember]
        public string OOSWait { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        [DataMember]
        public int IsNew { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        [DataMember]
        public int IsCommend { get; set; }

        /// <summary>
        /// 是否特价
        /// </summary>
        [DataMember]
        public int IsSpecialOffer { get; set; }

        /// <summary>
        /// 是否热卖。
        /// </summary>
        [DataMember]
        public int IsHot { get; set; }

        /// <summary>
        /// 1是缺货，0是有库存
        /// </summary>
        [DataMember]
        public int IsScarcity { get; set; }

        /// <summary>
        /// 打包类型：0子商品，1主商品，2打包商品。
        /// </summary>
        [DataMember]
        public int IsCompGoods { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public int GoodsType { get; set; }

        /// <summary>
        /// 可以删除
        /// </summary>
        [DataMember]
        public string SmallImg { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string GoodsImg { get; set; }

        /// <summary>
        /// 积分规则编号
        /// </summary>
        [DataMember]
        public Guid ScoreId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int OrderIndex { get; set; }

        /// <summary>
        /// 状态：1上架，0下架
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 是否卖库存。如果卖库存，卖完了，前台会自动下架；每晚有服务计算上架。
        /// </summary>
        [DataMember]
        public int IsSaleStock { get; set; }

        /// <summary>
        /// 高级属性(框架眼镜用，比如鼻梁宽度等)
        /// </summary>
        [DataMember]
        public string AdvancedField { get; set; }

        /// <summary>
        /// 货位号，可以删除
        /// </summary>
        [DataMember]
        public string ShelfNo { get; set; }

        /// <summary>
        /// seocode
        /// </summary>
        [DataMember]
        public string SEOCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsAllInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品编号</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="goodsCode">商品编码</param>
        /// <param name="brandId">品牌编号</param>
        /// <param name="units">计量单位</param>
        /// <param name="marketPrice">市场价</param>
        /// <param name="sellPrice">销售价</param>
        /// <param name="weight">重量</param>
        /// <param name="explain">简单描述</param>
        /// <param name="usefulLifeUpper">上架时间，记录商品第一次上架时间</param>
        /// <param name="usefulLifeLower">下架时候，修改此时间</param>
        /// <param name="minSellNumber">起销数量（一单至少销售几个）.先前台只控制商品展示个数，未真正限制个数。</param>
        /// <param name="maxSellNumber">限销数量（一单最多销售个数）。0为不限。</param>
        /// <param name="ordersNeedAmount">可销订单额。订单满足了多少金额，才能购买这个商品。</param>
        /// <param name="stockUpperLimit">库存上限</param>
        /// <param name="stockLowerLimit">库存下限</param>
        /// <param name="oOsWait">库存状况：如果商品缺货，显示在前台的文字信息。</param>
        /// <param name="isNew">是否新品</param>
        /// <param name="isCommend">是否推荐</param>
        /// <param name="isSpecialOffer">是否特价</param>
        /// <param name="isHot">是否热销</param>
        /// <param name="isScarcity">1是缺货，0是有库存</param>
        /// <param name="isCompGoods">打包类型：0子商品，1主商品，2打包商品。</param>
        /// <param name="goodsType">商品类型</param>
        /// <param name="smallImg">可以删除</param>
        /// <param name="goodsImg">商品图片</param>
        /// <param name="scoreId">积分编号</param>
        /// <param name="orderIndex">序号</param>
        /// <param name="state">状态：1上架，0下架</param>
        /// <param name="description">详细描述</param>
        /// <param name="isSaleStock">是否卖库存。如果卖库存，卖完了，前台会自动下架；每晚有服务计算上架。</param>
        /// <param name="advancedField">高级属性(框架眼镜用，比如鼻梁宽度等)</param>
        /// <param name="shelfNo">货位号，可以删除</param>
        /// <param name="sEOCode">seocode</param>
        public GoodsAllInfo(Guid goodsId, string goodsName, string goodsCode, Guid brandId, string units, decimal marketPrice, decimal sellPrice, double weight, string explain, DateTime usefulLifeUpper, DateTime usefulLifeLower, double minSellNumber, double maxSellNumber, double ordersNeedAmount, double stockUpperLimit, double stockLowerLimit, string oOsWait, int isNew, int isCommend, int isSpecialOffer, int isHot, int isScarcity, int isCompGoods, int goodsType, string smallImg, string goodsImg, Guid scoreId, int orderIndex, int state, string description, int isSaleStock, string advancedField, string shelfNo, string sEOCode)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            BrandId = brandId;
            Units = units;
            MarketPrice = marketPrice;
            SellPrice = sellPrice;
            Weight = weight;
            Explain = explain;
            UsefulLifeUpper = usefulLifeUpper;
            UsefulLifeLower = usefulLifeLower;
            MinSellNumber = minSellNumber;
            MaxSellNumber = maxSellNumber;
            OrdersNeedAmount = ordersNeedAmount;
            StockUpperLimit = stockUpperLimit;
            StockLowerLimit = stockLowerLimit;
            OOSWait = oOsWait;
            IsNew = isNew;
            IsCommend = isCommend;
            IsSpecialOffer = isSpecialOffer;
            IsHot = isHot;
            IsScarcity = isScarcity;
            IsCompGoods = isCompGoods;
            GoodsType = goodsType;
            SmallImg = smallImg;
            GoodsImg = goodsImg;
            ScoreId = scoreId;
            OrderIndex = orderIndex;
            State = state;
            Description = description;
            IsSaleStock = isSaleStock;
            AdvancedField = advancedField;
            ShelfNo = shelfNo;
            SEOCode = sEOCode;
        }

        //导入时读取用,Keede里没有SEOCode字段，IC里有。
        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品编号</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="goodsCode">商品编码</param>
        /// <param name="brandId">品牌编号</param>
        /// <param name="units">计量单位</param>
        /// <param name="marketPrice">市场价</param>
        /// <param name="sellPrice">销售价</param>
        /// <param name="weight">重量</param>
        /// <param name="explain">简单描述</param>
        /// <param name="usefulLifeUpper">上架时间，记录商品第一次上架时间</param>
        /// <param name="usefulLifeLower">下架时候，修改此时间</param>
        /// <param name="minSellNumber">起销数量（一单至少销售几个）.先前台只控制商品展示个数，未真正限制个数。</param>
        /// <param name="maxSellNumber">限销数量（一单最多销售个数）。0为不限。</param>
        /// <param name="ordersNeedAmount">可销订单额。订单满足了多少金额，才能购买这个商品。</param>
        /// <param name="stockUpperLimit">库存上限</param>
        /// <param name="stockLowerLimit">库存下限</param>
        /// <param name="oOsWait">库存状况：如果商品缺货，显示在前台的文字信息。</param>
        /// <param name="isNew">是否新品</param>
        /// <param name="isCommend">是否推荐</param>
        /// <param name="isSpecialOffer">是否特价</param>
        /// <param name="isHot">是否热销</param>
        /// <param name="isScarcity">1是缺货，0是有库存</param>
        /// <param name="isCompGoods">打包类型：0子商品，1主商品，2打包商品。</param>
        /// <param name="goodsType">商品类型</param>
        /// <param name="smallImg">可以删除</param>
        /// <param name="goodsImg">商品图片</param>
        /// <param name="scoreId">积分编号</param>
        /// <param name="orderIndex">序号</param>
        /// <param name="state">状态：1上架，0下架</param>
        /// <param name="description">详细描述</param>
        /// <param name="isSaleStock">是否卖库存。如果卖库存，卖完了，前台会自动下架；每晚有服务计算上架。</param>
        /// <param name="advancedField">高级属性(框架眼镜用，比如鼻梁宽度等)</param>
        /// <param name="shelfNo">货位号，可以删除</param>
        public GoodsAllInfo(Guid goodsId, string goodsName, string goodsCode, Guid brandId, string units, decimal marketPrice, decimal sellPrice, double weight, string explain, DateTime usefulLifeUpper, DateTime usefulLifeLower, double minSellNumber, double maxSellNumber, double ordersNeedAmount, double stockUpperLimit, double stockLowerLimit, string oOsWait, int isNew, int isCommend, int isSpecialOffer, int isHot, int isScarcity, int isCompGoods, int goodsType, string smallImg, string goodsImg, Guid scoreId, int orderIndex, int state, string description, int isSaleStock, string advancedField, string shelfNo)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            BrandId = brandId;
            Units = units;
            MarketPrice = marketPrice;
            SellPrice = sellPrice;
            Weight = weight;
            Explain = explain;
            UsefulLifeUpper = usefulLifeUpper;
            UsefulLifeLower = usefulLifeLower;
            MinSellNumber = minSellNumber;
            MaxSellNumber = maxSellNumber;
            OrdersNeedAmount = ordersNeedAmount;
            StockUpperLimit = stockUpperLimit;
            StockLowerLimit = stockLowerLimit;
            OOSWait = oOsWait;
            IsNew = isNew;
            IsCommend = isCommend;
            IsSpecialOffer = isSpecialOffer;
            IsHot = isHot;
            IsScarcity = isScarcity;
            IsCompGoods = isCompGoods;
            GoodsType = goodsType;
            SmallImg = smallImg;
            GoodsImg = goodsImg;
            ScoreId = scoreId;
            OrderIndex = orderIndex;
            State = state;
            Description = description;
            IsSaleStock = isSaleStock;
            AdvancedField = advancedField;
            ShelfNo = shelfNo;
        }
    }
}