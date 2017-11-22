//================================================
// 源码所属公司:
// 文件产生时间:
// 文件创建人:
// 最后修改时间:
// 最后一次修改人:
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 缓存检查的订单信息
    /// </summary>
    [Serializable]
    public class CacheOrderCheck
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CacheOrderCheck() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressNo">快递号</param>
        /// <param name="expressId">快递ID</param>
        /// <param name="orderstate">订单状态</param>
        /// <param name="orderId">订单ID</param>
        public CacheOrderCheck(string orderNo, string expressNo, Guid expressId, int orderstate, Guid orderId)
        {
            OrderNo = orderNo;
            ExpressNo = expressNo;
            ExpressId = expressId;
            OrderState = orderstate;
            OrderId = orderId;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 快递号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 快递ID
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browseItemObj"></param>
        /// <returns></returns>
        public override bool Equals(object browseItemObj)
        {
            if (browseItemObj is CacheOrderCheck)
            {
                var browseItem = browseItemObj as CacheOrderCheck;
                return (browseItem.OrderNo == OrderNo);
            }
            return base.Equals(browseItemObj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(OrderNo)) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + OrderNo;
            return stringRepresentation.GetHashCode();
        }

    }
}
