using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Order;
using ERP.Model;
using ERP.Model.Goods;

namespace ERP.Service.Implement
{
    public partial class Service
    {

        private static readonly FrameProcessManager _frameProcessManager = new FrameProcessManager();

        public FrameProcessCertificateInfo GetFrameProcessCertificateInfo(string processNo)
        {
            return _frameProcessManager.GetCertificateInfo(processNo);
        }

        /// <summary> 获取出库商品条码信息(框架相关)
        /// </summary>
        /// <param name="tracodeNo"></param>
        /// <returns></returns>
        public IList<GoodsBarcodeInfo> GetOutStockGoodsBarcode(string tracodeNo)
        {
            var frameGoodsTypes = new[] { 4, 6, 7, 8 };
            IList<GoodsBarcodeInfo> list = new List<GoodsBarcodeInfo>();
            IList<Guid> goodsIdList = _storageRecordDao.GetGoodsIdListFromStorageRecordDetail(tracodeNo);
            if (goodsIdList.Count > 0)
            {
                var dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdList.ToList());
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    foreach (var keyValuePair in dicGoods)
                    {
                        if (frameGoodsTypes.Any(a => a.Equals(keyValuePair.Value.GoodsType)))
                        {
                            list.Add(new GoodsBarcodeInfo
                            {
                                GoodsId = keyValuePair.Value.GoodsId,
                                GoodsCode = keyValuePair.Value.GoodsCode,
                                GoodsName = keyValuePair.Value.GoodsName
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
