using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 退货留言模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopRefundMessageInfo
    {
        #region 属性 Property
        /// <summary>
        /// 留言ID
        /// </summary>
        [DataMember]
        public Guid MsgID { get; set; }

        /// <summary>
        /// 留言店铺ID
        /// </summary>
        [DataMember]
        public Guid ShopID { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMember]
        public string ShopName { get; set; }

        /// <summary>
        /// 留言时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        [DataMember]
        public string ApplyContent { get; set; }

        /// <summary>
        /// 留言申请状态
        /// </summary>
        [DataMember]
        public int ApplyState { get; set; }

        /// <summary>
        /// 留言备注
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopRefundMessageInfo()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopName"></param>
        /// <param name="createTime"></param>
        /// <param name="content"></param>
        /// <param name="applyState"></param>
        /// <param name="description"></param>
        public ShopRefundMessageInfo(Guid msgId,Guid shopId,string shopName,DateTime createTime,string content,int applyState,string description)
        {
            MsgID = msgId;
            ShopID = shopId;
            ShopName = shopName;
            CreateTime = createTime;
            ApplyContent = content;
            ApplyState = applyState;
            Description = description;
        }
        #endregion
    }
}
