using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品的会员评论
    /// </summary>
    [Serializable]
    public class GoodsMemberRemarkInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public GoodsMemberRemarkInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="memberId">用户ID</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="remarkTimes">评论次数</param>
        /// <param name="remarkedCount">已评论次数</param>
        /// <param name="remarkType">评论类型</param>
        /// <param name="remarkLastDate">最后评论时间</param>
        public GoodsMemberRemarkInfo(Guid goodsId, Guid memberId, string goodsName, int remarkTimes, int remarkedCount, int remarkType, DateTime remarkLastDate)
        {
            GoodsId = goodsId;
            MemberId = memberId;
            GoodsName = goodsName;
            RemarkTimes = remarkTimes;
            RemarkedCount = remarkedCount;
            RemarkType = remarkType;
            RemarkLastDate = remarkLastDate;
        }


        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid MemberId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 评论次数
        /// </summary>
        public int RemarkTimes { get; set; }
        /// <summary>
        /// 已评论次数
        /// </summary>
        public int RemarkedCount { get; set; }
        /// <summary>
        /// 评论类型
        /// </summary>
        public int RemarkType { get; set; }
        /// <summary>
        /// 最后评论时间
        /// </summary>
        public DateTime RemarkLastDate { get; set; }

    }
}
