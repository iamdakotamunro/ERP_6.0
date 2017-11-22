using ERP.BLL.Interface;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 内部采购价设置 业务层代码
    /// </summary>
    public class InternalPriceSetManager : BllInstance<InternalPriceSetManager>, IInternalPriceSetManager
    {
        private readonly IInternalPriceSetDao _internalPriceSetDal = null;

        private readonly IRealTimeGrossSettlementManager _realTimeGrossSettlementManager = null;

        public InternalPriceSetManager(GlobalConfig.DB.FromType fromType)
        {
            _internalPriceSetDal = new InternalPriceSetDao(fromType);
            _realTimeGrossSettlementManager = RealTimeGrossSettlementManager.WriteInstance;
        }

        /// <summary>
        /// 获取内部采购价
        /// </summary>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsIdGoodsTypeDict"></param>
        /// <returns></returns>
        public Dictionary<Guid, decimal> GetInternalPurchasePriceByHostingFilialeIdGoodsIds(Guid hostingFilialeId, IDictionary<Guid, int> goodsIdGoodsTypeDict)
        {
            var internalPriceSets = _internalPriceSetDal.GetGoodsTypeInternalPriceSets(hostingFilialeId, goodsIdGoodsTypeDict.Values.Distinct());
            var settlePrices = _realTimeGrossSettlementManager.GetLatestUnitPriceListByMultiGoods(hostingFilialeId, goodsIdGoodsTypeDict.Keys);
            var purchasePrices = (from item in goodsIdGoodsTypeDict
                                     let settlePrice = settlePrices.ContainsKey(item.Key) ? settlePrices[item.Key] : 0 //结算价
                                     let internalPrice = internalPriceSets.ContainsKey(item.Value) ? internalPriceSets[item.Value] : 0 //比例
                                     select new KeyValuePair<Guid, decimal>(item.Key, settlePrice * internalPrice)).ToDictionary(k => k.Key, v => v.Value);
            return purchasePrices;
        }
    }
}
