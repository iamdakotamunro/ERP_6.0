using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 联盟店退换货申请模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopExchangedApplyInfo
    {
        #region  属性
        /// <summary>
        /// 退换货申请ID
        /// </summary>
        [DataMember]
        public Guid ApplyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ApplyNo { get; set; }

        /// <summary>
        /// 联盟店店铺ID
        /// </summary>
        [DataMember]
        public Guid ShopID { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }

        /// <summary>
        /// 退换货申请时间
        /// </summary>
        [DataMember]
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 退换货宗商品数
        /// </summary>
        [DataMember]
        public int SubQuantity { get; set; }

        /// <summary>
        /// 退换货总额
        /// </summary>
        [DataMember]
        public decimal SubPrice { get; set; }

        /// <summary>
        /// 退换货申请状态
        /// </summary>
        [DataMember]
        public int ExchangedState { get; set; }

        /// <summary>
        /// false 换货,true 退货
        /// </summary>
        [DataMember]
        public bool IsBarter { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 退货留言ID
        /// </summary>
        [DataMember]
        public Guid MsgID { get; set; }

        /// <summary>
        /// 退货留言
        /// </summary>
        [DataMember]
        public string MsgContent { get; set; }
        #endregion

        #region  构造函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopExchangedApplyInfo()
        {
            
        }

        /// <summary>
        /// 退货申请构造函数
        /// </summary>
        public ShopExchangedApplyInfo(Guid applyId, string applyNo, Guid shopId,string shopName, DateTime applyTime, int subQuantity,
            decimal subPrice, int exchangedState,bool isBarter,string description,Guid msgId,string msgContent):
            this(applyId,applyNo,shopId,shopName,applyTime,subQuantity,subPrice,exchangedState,isBarter,description)
        {
            MsgID = msgId;
            MsgContent = msgContent;
        }

        /// <summary>
        /// 换货申请构造函数
        /// </summary>
        public ShopExchangedApplyInfo(Guid applyId, string applyNo, Guid shopId, string shopName, DateTime applyTime, int subQuantity,
            decimal subPrice, int exchangedState, bool isBarter, string description)
        {
            ApplyID = applyId;
            ApplyNo = applyNo;
            ShopID = shopId;
            ShopName = shopName;
            ApplyTime = applyTime;
            SubQuantity = subQuantity;
            SubPrice = subPrice;
            ExchangedState = exchangedState;
            IsBarter = isBarter;
            Description = description;
        }
        #endregion
    }
}
