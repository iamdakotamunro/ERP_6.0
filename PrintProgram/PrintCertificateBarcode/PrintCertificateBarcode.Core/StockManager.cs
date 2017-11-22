using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP.Model.Goods;
using ERP.Service.Contract;
using Framework.WCF;
using KeedeGroup.GoodsManageSystem.Public.Client;
using KeedeGroup.GoodsManageSystem.Public.Enum.ModelType;
using KeedeGroup.GoodsManageSystem.Public.Model.B2C;
using KeedeGroup.GoodsManageSystem.Public.Model.Result;

namespace PrintCertificateBarcode.Core
{
    public class StockManager
    {
        private const string CMS_ENDPOINT = "Group.ERP";

        private readonly static B2CGoodsServiceClient _b2CGoodsServerClient = new B2CGoodsServiceClient(Guid.Parse("3FE5AEF4-2CFD-4998-8D88-385321179B80"), "打印合格证条码");

        public GoodsBarcodeInfo GetGoodsBarcodeInfo(string goodscode)
        {
            GoodsBarcodeInfo info;
            using (var client = new ServiceClient<IService>(CMS_ENDPOINT))
            {
                info = client.Instance.GetGoodsBarcodeInfo(goodscode);
            }
            if (info != null)
            {
                var ids = new List<Guid> { info.GoodsId };
                var priceDict = GetGoodsSellPrice(ids);
                //var originDict = GetGoodsAttrValues(ids, Configuration.AppSettings["MadeInIndex"].ToInt());
                if ((priceDict != null) && priceDict.Count > 0)
                {
                    info.SellPrice = priceDict.ContainsKey(info.GoodsId) ? priceDict[info.GoodsId] : "-";
                }
            }
            return info;
        }

        public IList<GoodsBarcodeInfo> GetGoodsBarcodeListByOutStock(string tradeNo)
        {
            IList<GoodsBarcodeInfo> list=new List<GoodsBarcodeInfo>();
            using (var client = new ServiceClient<IService>(CMS_ENDPOINT))
            {
               //list = client.Instance.GetOutStockGoodsBarcode(tradeNo);
            }
            if ((list != null) && (list.Count > 0))
            {
                List<Guid> ids = list.Select(w => w.GoodsId).Distinct().ToList();
                IDictionary<Guid, string> priceDict = GetGoodsSellPrice(ids);
                //IDictionary<Guid, string> originDict = GetGoodsAttrValues(ids, Configuration.AppSettings["MadeInIndex"].ToInt());
                if (priceDict == null)
                {
                    return list;
                }
                foreach (GoodsBarcodeInfo info in list)
                {
                    info.SellPrice = priceDict.ContainsKey(info.GoodsId) ? priceDict[info.GoodsId] : "-";
                }
            }
            return list;
        }

        /// <summary>
        /// 获取销售价格
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public IDictionary<Guid, string> GetGoodsSellPrice(List<Guid> goodsIds)
        {
            var result = _b2CGoodsServerClient.GetGroupGoodsModelListByGoodsIds(goodsIds, B2CGoodsModelType.GoodsSaleModel) as ListResult<GoodsSaleModel>;
            if (result != null && result.IsSuccess)
            {
                var goodsList = result.Data;
                return goodsIds.ToDictionary(id => id, id =>
                {
                    var groupGoodsBaseInfo = goodsList.FirstOrDefault(d => d.GoodsId == id);
                    return groupGoodsBaseInfo != null ? groupGoodsBaseInfo.SellPrice.ToString(CultureInfo.InvariantCulture) : "0";
                });
            }
            return new Dictionary<Guid, string>();
        }

        /// <summary>
        /// 获取商品高级属性值
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="attrGroupId"></param>
        /// <returns></returns>
        public Dictionary<Guid, String> GetGoodsAttrValues(List<Guid> goodsIdList, Int32 attrGroupId)
        {
            var result = _b2CGoodsServerClient.GetGoodsAttrValues(goodsIdList, attrGroupId);
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            return new Dictionary<Guid, string>();
        }
    }
}

