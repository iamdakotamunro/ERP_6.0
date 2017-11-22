using B2C.Service.Contract;
using ERP.Model.Goods;
using ERP.Service.Contract;
using Framework.Common;
using Framework.WCF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintLabel.Manager
{
    public class StockManager
    {
        private const string CMS_ENDPOINT = "Group.ERP";
        private const string KEEDE_ENDPOINT = "B2C.Keede";

        public GoodsBarcodeInfo GetGoodsBarcodeInfo(string goodscode)
        {
            GoodsBarcodeInfo info;
            using (var client = new WcfClient<IService>(CMS_ENDPOINT))
            {
                info = client.Call(e=>e.GetGoodsBarcodeInfo(goodscode));
            }
            if (info != null)
            {
                using (var client2 = new WcfClient<IKeedeAdmin>(KEEDE_ENDPOINT))
                {
                    var ids = new List<Guid> {
                        info.GoodsId
                    };
                    var priceDict = client2.Call(e=>e.GetGoodsSellPrice(ids));
                    var originDict = client2.Call(e=>e.GetGoodsAttrValues(ids, Configuration.AppSettings["MadeInIndex"].ToInt()));
                    if ((priceDict != null) && priceDict.Count>0)
                    {
                        info.SellPrice = priceDict.ContainsKey(info.GoodsId) ? priceDict[info.GoodsId] : "-";
                    }
                    if ((originDict != null) && originDict.Count > 0)
                    {
                        info.Origin = originDict.ContainsKey(info.GoodsId) ? originDict[info.GoodsId] : string.Empty; 
                    }
                }
            }
            return info;
        }

        public IList<GoodsBarcodeInfo> GetGoodsBarcodeListByOutStock(string tradeNo)
        {
            IList<GoodsBarcodeInfo> list;
            using (var client = new WcfClient<IService>(CMS_ENDPOINT))
            {
                list = client.Call(e=>e.GetOutStockGoodsBarcode(tradeNo));
            }
            if ((list != null) && (list.Count > 0))
            {
                IDictionary<Guid, string> priceDict;
                IDictionary<Guid, string> originDict;
                using (var client2 = new WcfClient<IKeedeAdmin>(KEEDE_ENDPOINT))
                {
                    List<Guid> ids = (from ent in list select ent.GoodsId).Distinct<Guid>().ToList<Guid>();
                    priceDict = client2.Call(e=>e.GetGoodsSellPrice(ids));
                    originDict = client2.Call(e=>e.GetGoodsAttrValues(ids, Configuration.AppSettings["MadeInIndex"].ToInt()));
                }
                if (priceDict == null || originDict==null)
                {
                    return list;
                }
                foreach (GoodsBarcodeInfo info in list)
                {
                    info.SellPrice = priceDict.ContainsKey(info.GoodsId) ? priceDict[info.GoodsId] : "-";
                    info.Origin = originDict.ContainsKey(info.GoodsId) ? originDict[info.GoodsId] : string.Empty;
                }
            }
            return list;
        }
    }
}

