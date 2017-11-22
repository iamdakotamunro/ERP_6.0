using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 售后详情
    /// add:pyw
    /// add time 2014-12-05
    /// </summary>
    [Serializable]
    [DataContract]
    public class AfterSaleDetailInfo
    {
        /// <summary> 
        ///	ID	
        /// </summary> 
        [DataMember]
        public Guid ID { get; set; }
        /// <summary> 
        ///	售后处理ID	
        /// </summary> 
        [DataMember]
        public Guid AfterSaleId { get; set; }
        #region 原单商品
        /// <summary> 
        ///	真实商品ID	
        /// </summary> 
        [DataMember]
        public Guid RealGoodsId { get; set; }
        /// <summary> 
        ///	商品ID	
        /// </summary> 
        [DataMember]
        public Guid GoodsId { get; set; }
        /// <summary> 
        ///	大包商品ID	
        /// </summary> 
        [DataMember]
        public Guid CompGoodsId { get; set; }
        /// <summary> 
        ///		
        /// </summary> 
        [DataMember]
        public int CompIndex { get; set; }
        /// <summary> 
        ///	商品名称	
        /// </summary> 
        [DataMember]
        public string GoodsName { get; set; }
        /// <summary> 
        ///	商品编号	
        /// </summary> 
        [DataMember]
        public string GoodsCode { get; set; }
        /// <summary> 
        ///	商品规格	
        /// </summary> 
        [DataMember]
        public string Specification { get; set; }
        #endregion

        /// <summary> 
        ///		
        /// </summary> 
        [DataMember]
        public decimal MarketPrice { get; set; }
        /// <summary> 
        ///	销售价格	
        /// </summary> 
        [DataMember]
        public decimal SellPrice { get; set; }
        /// <summary> 
        ///	销售数	
        /// </summary> 
        [DataMember]
        public int Quantity { get; set; }
        /// <summary> 
        ///	销售价小计	
        /// </summary> 
        [DataMember]
        public decimal TotalPrice { get; set; }
        /// <summary> 
        ///	赠送积分	
        /// </summary> 
        [DataMember]
        public int GiveScore { get; set; }
        /// <summary> 
        ///	销售类型	
        /// </summary> 
        [DataMember]
        public int SellType { get; set; }
        /// <summary> 
        ///	处理原因	
        /// </summary> 
        [DataMember]
        public int ReasonType { get; set; }
        /// <summary> 
        ///	是否补发	
        /// </summary> 
        [DataMember]
        public bool IsReissue { get; set; }
        /// <summary> 
        ///	发（退）数	
        /// </summary> 
        [DataMember]
        public int AgainCount { get; set; }

        #region 补发商品 属性
        /// <summary> 
        ///	补发商品ID	
        /// </summary> 
        [DataMember]
        public Guid ReissueGoodsId { get; set; }
        /// <summary> 
        ///	补发真实商品ID	
        /// </summary> 
        [DataMember]
        public Guid ReissueRealGoodsId { get; set; }
        /// <summary> 
        ///	补发规格	
        /// </summary> 
        [DataMember]
        public string ReissueSpecification { get; set; }
        #endregion

        #region 退回商品 属性
        /// <summary> 
        ///	退回商品ID	
        /// </summary> 
        [DataMember]
        public Guid BackGoodsId { get; set; }
        /// <summary> 
        ///	退回真实商品ID	
        /// </summary> 
        [DataMember]
        public Guid BackRealGoodsId { get; set; }
        /// <summary> 
        ///	退回商品名称	
        /// </summary> 
        [DataMember]
        public string BackGoodsName { get; set; }
        /// <summary> 
        ///	退回数	
        /// </summary> 
        [DataMember]
        public int BackCount { get; set; }
        /// <summary> 
        ///	退回规格	
        /// </summary> 
        [DataMember]
        public string BackSpecification { get; set; }
        #endregion
        /// <summary> 
        ///	购买类型	
        /// </summary> 
        [DataMember]
        public int BuyType { get; set; }
        /// <summary> 
        ///	花费积分	
        /// </summary> 
        [DataMember]
        public int GoodsScore { get; set; }
        /// <summary> 
        ///	花费积分小计	
        /// </summary> 
        [DataMember]
        public int TotalGoodsScore { get; set; }
        /// <summary> 
        ///	退换货原因 (enum:ReturnReason)	
        /// </summary> 
        [DataMember]
        public int ReturnReasonType { get; set; }
        /// <summary> 
        ///	退换货类型	1 换货，0 退货、退款
        /// </summary> 
        [DataMember]
        public int RefundType { get; set; }
        /// <summary> 
        ///	退换货原因Id	
        /// </summary> 
        [DataMember]
        public int CauseId { get; set; }
        /// <summary> 
        ///	商品类型	
        /// </summary> 
        [DataMember]
        public int GoodsType { get; set; }
        /// <summary> 
        ///	原因（前台客户填写）	
        /// </summary> 
        [DataMember]
        public string Explain { get; set; }
    }
}
