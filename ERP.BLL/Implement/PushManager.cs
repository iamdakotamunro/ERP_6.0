using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.Environment;
using Keede.Ecsoft.Model;
using MIS.Enum;
using PUSH.Core.Model;

namespace ERP.BLL.Implement
{
    /// <summary>
    /// 推送数据管理
    /// </summary>
    public class PushManager
    {
        /// <summary>
        /// 推送到门店
        /// </summary>
        /// <param name="shopFilialeId">推送到的门店</param>
        /// <param name="methodName"></param>
        /// <param name="identityKey"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static bool AddToShop(Guid shopFilialeId, string methodName, string identityKey, params object[] paramters)
        {
            var headFilialeId = FilialeManager.GetShopHeadFilialeId(shopFilialeId);
            var parms = paramters.ToList();
            var pushDataInfo = PushDataInfo.Create(GlobalConfig.ERPFilialeID, ServiceType.ERP.ToString(), headFilialeId, ServiceType.Shop.ToString(), methodName, identityKey, parms.ToArray());
            return PUSH.Core.Instance.Add(pushDataInfo);
        }

        /// <summary>
        /// 推送到B2C
        /// </summary>
        /// <param name="b2CFilialeId">推送到的B2C公司</param>
        /// <param name="methodName"></param>
        /// <param name="identityKey"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static bool AddToB2C(Guid b2CFilialeId, string methodName, string identityKey, params object[] paramters)
        {
            var parms = paramters.ToList();
            var pushDataInfo = PushDataInfo.Create(GlobalConfig.ERPFilialeID, ServiceType.ERP.ToString(), b2CFilialeId, ServiceType.B2C.ToString(), methodName, identityKey, parms.ToArray());
            return PUSH.Core.Instance.Add(pushDataInfo);
        }


        /// <summary>库存中心完成订单加入推送
        /// </summary>
        /// <returns></returns>
        public static bool AddToStockCenterForFinishOrder(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> detailInfos)
        {
            var pushDataInfo = PushDataInfo.Create(GlobalConfig.ERPFilialeID, ServiceType.ERP.ToString(), orderInfo.SaleFilialeId, ServiceType.StockCenter.ToString(), "FinishOrder", orderInfo.OrderNo, orderInfo, detailInfos);
            return PUSH.Core.Instance.Add(pushDataInfo);
        }

        /// <summary>库存中心匹配订单加入推送
        /// </summary>
        /// <returns></returns>
        public static bool AddToStockCenterForMatchOrder(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> detailInfos)
        {
            var pushDataInfo = PushDataInfo.Create(GlobalConfig.ERPFilialeID, ServiceType.ERP.ToString(), orderInfo.SaleFilialeId, ServiceType.StockCenter.ToString(), "MatchOrder", orderInfo.OrderNo, orderInfo, detailInfos);
            return PUSH.Core.Instance.Add(pushDataInfo);
        }
    }
}
