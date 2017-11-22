using System;


namespace Keede.Ecsoft.Model
{
    ///<summary>
    /// 
    ///</summary>
    [Serializable]
    public class GoodsOrderPayInfo
    {
        ///<summary>
        /// 订单ID
        ///</summary>
        public Guid OrderId { get; set; }

        ///<summary>
        /// 订单号
        ///</summary>
        public string PaidNo { get; set; }

        ///<summary>
        /// 支付金额
        ///</summary>
        public decimal PaiSum { get; set; }

        ///<summary>
        /// 支付状态
        ///</summary>
        public int PayState { get; set; }

        ///<summary>
        /// 银行号
        ///</summary>
        public Guid BankAccountId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string BankTradeNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PaidTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 当前兑换美元汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }
    }
}
