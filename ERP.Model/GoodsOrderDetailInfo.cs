using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 购物车记录类
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsOrderDetailInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid RealGoodsID { get; set; }

        /// <summary>
        /// 主商品编号
        /// </summary>
        [DataMember]
        public Guid CompGoodsId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int CompIndex { get; set; }

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
        /// 规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

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
        ///原始 销售价
        /// </summary>
        [DataMember]
        public decimal OriginalSellPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public double Quantity { get; set; }

        /// <summary>
        /// 总计（数量*销售价）
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 积分总计（商品对应赠送积分）
        /// </summary>
        [DataMember]
        public int GiveScore { get; set; }

        /// <summary>
        /// 采购规格
        /// </summary>
        [DataMember]
        public string PurchaseSpecification { get; set; }

        /// <summary>
        /// 是否加工
        /// </summary>
        [DataMember]
        public bool IsProcess { get; set; }

        /// <summary>
        /// 占用库存数量
        /// </summary>
        [DataMember]
        public int DemandQuantity { get; set; }

        /// <summary>
        /// 购买类型: 1，普通购买；2，组合购买；3，配镜购买；4，框架购买,5.赠换购买
        /// </summary>
        [DataMember]
        public int BuyType { get; set; }

        /// <summary>
        /// 商品积分
        /// </summary>
        [DataMember]
        public int GoodsScore { get; set; }

        /// <summary>
        /// 商品总积分
        /// </summary>
        [DataMember]
        public int TotalGoodsScore { get; set; }

        /// <summary>
        /// 销售类型
        /// zhangfan added at 2013-Dec-16th
        /// </summary>
        [DataMember]
        public int SellType { get; set; }

        /// <summary>
        /// 售后处理个数。售后处理过的，记录此数量。
        ///  </summary>
        [DataMember]
        public int ReturnCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int GoodsType { get; set; }

        /// <summary>
        /// 该商品的积分抵扣钱的金额上限（商品设置的那个值）
        /// </summary>
        [DataMember]
        public int ScoreDeductionMoneyUpper { get; set; }

        /// <summary>促销ID(当前只做插入不做查询使用)
        /// </summary>
        [DataMember]
        public Guid PromotionId { get; set; }

        /// <summary>商品计量单位
        /// </summary>
        [DataMember]
        public String Unit { get; set; }

        /// <summary>
        /// (促销+积分)扣除金额
        /// </summary>
        [IgnoreDataMember]
        public decimal PromotionDeductionAmount { get; set; }

        /// <summary>
        /// 购物车类
        /// </summary>
        public GoodsOrderDetailInfo()
        {
            CompIndex = 0;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="realGoodsId">产品Id</param>
        /// <param name="compGoodsId">打包商品组</param>
        /// <param name="compIndex">购买时打包生成组，用于区分同一打包产品的多次购买</param>
        /// <param name="goodsName">产品名称</param>
        /// <param name="goodsCode">产品编号</param>
        /// <param name="specification">产品描述</param>
        /// <param name="marketPrice">商品市价</param>
        /// <param name="sellPrice">购买价格</param>
        /// <param name="quantity">购买数量</param>
        /// <param name="totalPrice">小计</param>
        /// <param name="giveScore">获得会员积分</param>
        public GoodsOrderDetailInfo(Guid orderId, Guid realGoodsId, Guid compGoodsId, int compIndex, string goodsName, string goodsCode, string specification, decimal marketPrice, decimal sellPrice, double quantity, decimal totalPrice, int giveScore)
        {
            OrderId = orderId;
            RealGoodsID = realGoodsId;
            CompGoodsId = compGoodsId;
            CompIndex = compIndex;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            MarketPrice = marketPrice;
            SellPrice = sellPrice;
            Quantity = quantity;
            TotalPrice = totalPrice;
            GiveScore = giveScore;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="realGoodsId">产品Id</param>
        /// <param name="compGoodsId">打包商品组</param>
        /// <param name="compIndex">购买时打包生成组，用于区分同一打包产品的多次购买</param>
        /// <param name="goodsName">产品名称</param>
        /// <param name="goodsCode">产品编号</param>
        /// <param name="specification">产品描述</param>
        /// <param name="marketPrice">商品市价</param>
        /// <param name="sellPrice">购买价格</param>
        /// <param name="quantity">购买数量</param>
        /// <param name="totalPrice">小计</param>
        /// <param name="giveScore">获得会员积分</param>
        /// <param name="buyType"> </param>
        /// <param name="goodsScore"> </param>
        /// <param name="totalGoodsScore"> </param>
        public GoodsOrderDetailInfo(Guid orderId, Guid realGoodsId, Guid compGoodsId, int compIndex, string goodsName, string goodsCode, string specification, decimal marketPrice, decimal sellPrice, double quantity, decimal totalPrice, int giveScore, int buyType, int goodsScore, int totalGoodsScore)
            : this(orderId, realGoodsId, compGoodsId, compIndex, goodsName, goodsCode, specification, marketPrice, sellPrice, quantity, totalPrice, giveScore)
        {
            BuyType = buyType;
            GoodsScore = goodsScore;
            TotalGoodsScore = totalGoodsScore;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="realGoodsId">产品Id</param>
        /// <param name="compGoodsId">打包商品组</param>
        /// <param name="compIndex">购买时打包生成组，用于区分同一打包产品的多次购买</param>
        /// <param name="goodsName">产品名称</param>
        /// <param name="goodsCode">产品编号</param>
        /// <param name="specification">产品描述</param>
        /// <param name="marketPrice">商品市价</param>
        /// <param name="sellPrice">购买价格</param>
        /// <param name="quantity">购买数量</param>
        /// <param name="totalPrice">小计</param>
        /// <param name="giveScore">获得会员积分</param>
        /// <param name="pretreatmentCount">预处理数量</param>
        /// <param name="returncount">已退数量</param>
        public GoodsOrderDetailInfo(Guid orderId, Guid realGoodsId, Guid compGoodsId, int compIndex, string goodsName, string goodsCode, string specification, decimal marketPrice, decimal sellPrice, double quantity, decimal totalPrice, int giveScore, int returncount, int pretreatmentCount)
        {
            OrderId = orderId;
            RealGoodsID = realGoodsId;
            CompGoodsId = compGoodsId;
            CompIndex = compIndex;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            MarketPrice = marketPrice;
            SellPrice = sellPrice;
            Quantity = quantity;
            TotalPrice = totalPrice;
            GiveScore = giveScore;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="realGoodsId">产品Id</param>
        /// <param name="compGoodsId">打包商品组</param>
        /// <param name="compIndex">购买时打包生成组，用于区分同一打包产品的多次购买</param>
        /// <param name="goodsName">产品名称</param>
        /// <param name="goodsCode">产品编号</param>
        /// <param name="specification">产品描述</param>
        /// <param name="marketPrice">商品市价</param>
        /// <param name="sellPrice">购买价格</param>
        /// <param name="quantity">购买数量</param>
        /// <param name="totalPrice">小计</param>
        /// <param name="giveScore">获得会员积分</param>
        /// <param name="pretreatmentCount">预处理数量</param>
        /// <param name="returncount">已退数量</param>
        /// <param name="buyType"> </param>
        /// <param name="goodsScore"> </param>
        /// <param name="totalGoodsScore"> </param>
        public GoodsOrderDetailInfo(Guid orderId, Guid realGoodsId, Guid compGoodsId, int compIndex, string goodsName, string goodsCode, string specification, decimal marketPrice, decimal sellPrice, double quantity, decimal totalPrice, int giveScore, int returncount, int pretreatmentCount, int buyType, int goodsScore, int totalGoodsScore)
            : this(orderId, realGoodsId, compGoodsId, compIndex, goodsName, goodsCode, specification, marketPrice, sellPrice, quantity, totalPrice, giveScore, returncount, pretreatmentCount)
        {
            BuyType = buyType;
            GoodsScore = goodsScore;
            TotalGoodsScore = totalGoodsScore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is GoodsOrderDetailInfo)
            {
                var godObj = compareObj as GoodsOrderDetailInfo;
                return (godObj.RealGoodsID == RealGoodsID && godObj.OrderId == OrderId && godObj.CompGoodsId == CompGoodsId && godObj.CompIndex == CompIndex && godObj.Specification == Specification);
            }
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (RealGoodsID == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + RealGoodsID.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
