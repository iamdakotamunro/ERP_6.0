using System;
using System.Collections.Generic;
using ERP.SAL.Interface;
using Framework.WCF;
using PromotionCenter.Public.Contract;

namespace ERP.SAL
{
    public sealed class PromotionSao : IPromotionSao
    {

        private const String NEWPROMOTIONSERVICE = "NewPromotionService";

        /// <summary>实例化促销中心客服端
        /// </summary>
        /// <returns></returns>
        private ServiceClient<IPromotionAdmin> GetInstance()
        {
            return new ServiceClient<IPromotionAdmin>(NEWPROMOTIONSERVICE);
        }

        /// <summary>获取商品促销信息
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public IDictionary<string, string> GetGoodsSalePromotionDict(Guid goodsId, DateTime startTime, DateTime endTime)
        {
            using (var client = GetInstance())
            {
                var result = client.Instance.GetGoodsSalePromotionDict(goodsId, startTime, endTime);
                if (result != null && result.IsSuccess)
                {
                    return result.Data;
                }
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 根据订单Id获取实际商品价格(参加促销活动的商品要抵扣相应的促销价，从而得到新的实际商品价格)
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        /// zal 2015-08-28
        public Dictionary<Guid, decimal> GetGoodsPriceDict(Guid orderId)
        {
            using (var client = GetInstance())
            {
                var result = client.Instance.GetGoodsPriceDict(orderId);
                if (result != null && result.IsSuccess)
                {
                    return result.Data;
                }
                return new Dictionary<Guid, decimal>();
            }
        }
    }
}
