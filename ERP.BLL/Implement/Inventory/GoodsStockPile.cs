using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IGoods;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Inventory
{
    public class GoodsStockPile : BllInstance<GoodsStockPile>
    {
        private readonly IGoodsStockPile _goodsStockPileDao;
        private readonly IGoodsCenterSao _goodsCenterSao;
        private readonly IStorageRecordDao _storageRecordDao;
        readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        public GoodsStockPile(GlobalConfig.DB.FromType fromType)
        {
            _goodsStockPileDao = InventoryInstance.GetGoodsStockPileDao(fromType);
            _goodsCenterSao = new GoodsCenterSao();
            _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        }

        /// <summary>根据主商品ID查询其当前仓库子商品的库存信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public IList<GoodsStockPileInfo> GetChildGoodsStockPileList(Guid goodsId, Guid warehouseId, Guid hostingFilialeId)
        {
            if (goodsId == Guid.Empty) return new List<GoodsStockPileInfo>();
            var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
            if (goodsInfo == null) return new List<GoodsStockPileInfo>();
            var goodsStockPileList = new List<GoodsStockPileInfo>();

            //根据主商品获取所有子商品信息
            var childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId }).ToList();
            if (childGoodsList.Count > 0)
            {
                var goodsIds = childGoodsList.Select(w => w.RealGoodsId).ToList();
                var dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseId, null, goodsIds, hostingFilialeId);
                if (dicRealGoodsIdAndStockQuantity != null && dicRealGoodsIdAndStockQuantity.Count > 0)
                {
                    var goodsIdList = new List<Guid> { goodsId };
                    //根据仓库Id获取供应商Id
                    var dicGoodsIdAndCompanyId = _purchaseSet.GetCompanyIdByWarehouseId(warehouseId, hostingFilialeId);
                    var companyIdList = dicGoodsIdAndCompanyId.Where(p => goodsIdList.Contains(p.Key)).Select(p => p.Value).ToList();
                    //获取商品的最后一次进货价信息
                    var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(warehouseId);
                    goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIdList.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

                    //返回库存>0的子商品
                    foreach (var childGoodsInfo in childGoodsList)
                    {
                        //可出库数
                        var goodsStockKeyValuePair = dicRealGoodsIdAndStockQuantity.FirstOrDefault(w => w.Key == childGoodsInfo.RealGoodsId);
                        //根据商品id获取供应商
                        var companyId = dicGoodsIdAndCompanyId.ContainsKey(childGoodsInfo.GoodsId) ? dicGoodsIdAndCompanyId[childGoodsInfo.GoodsId] : Guid.Empty;

                        decimal unitPrice = 0;
                        GoodsPurchaseLastPriceInfo goodsPurchaseLastPriceInfo =null;
                        if (goodsPurchaseLastPriceInfoList.Count > 0)
                        {
                            goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(childGoodsInfo.GoodsId) && p.ThirdCompanyId.Equals(companyId));
                            unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                        }
                        var goodsStockPileInfo = new GoodsStockPileInfo
                        {
                            GoodsId = childGoodsInfo.RealGoodsId,
                            GoodsName = goodsInfo.GoodsName,
                            GoodsCode = goodsInfo.GoodsCode,
                            Specification = childGoodsInfo.Specification,
                            UnitPrice = unitPrice,
                            NonceWarehouseGoodsStock = goodsStockKeyValuePair.Value,
                            RecentInDate = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.LastPriceDate : DateTime.MinValue
                        };
                        goodsStockPileList.Add(goodsStockPileInfo);
                    }
                }
            }
            return goodsStockPileList;
            //IList<GoodsStockQuantityInfo> stockQuantityList = _stockCenterManager.GetChildGoodsQuantity(goodsId);
            //var result = from item in stockQuantityList.Where(ent => ent.FilialeId == warehouseInfo.FilialeId && ent.WarehouseId == warehouseId)
            //             select new GoodsStockPileInfo
            //             {
            //                 GoodsId = item.RealGoodsId,
            //                 GoodsName = item.GoodsName,
            //                 GoodsCode = item.GoodsCode,
            //                 Specification = item.Specification,
            //                 UnitPrice = item.RecentInPrice,
            //                 NonceWarehouseGoodsStock = item.CurrentQuantity,
            //                 WaitConsignmentedGoodsStock = item.OutboundQuantity,
            //                 RecentInDate = item.RecentCDate
            //             };
            //return result.ToList();
        }

        /// <summary>得到平均库存周转 2015-04-29  陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="goodsIds">商品ID集合</param>
        /// <param name="state">0全部，1下架或缺货且有库存，2无销售商品</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetAvgStockTurnOver(DateTime startTime, DateTime endTime, Guid warehouseId, List<Guid> goodsIds, int state)
        {
            return _goodsStockPileDao.GetAvgStockTurnOver(startTime, endTime, warehouseId, goodsIds, state);
        }

        /// <summary>获取商品的库存周转率  2015-04-30 陈重文
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetGoodsStockTurnOverByGoodsId(DateTime startTime, DateTime endTime, Guid goodsId, Guid warehouseId)
        {
            return _goodsStockPileDao.GetGoodsStockTurnOverByGoodsId(startTime, endTime, goodsId, warehouseId);
        }

        /// <summary>获取商品近3个月内销量情况  2015-06-16  陈重文
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public IList<SalesVolumeInfo> GetGoodsSalesVolume(Guid warehouseId)
        {
            return _goodsStockPileDao.GetGoodsSalesVolume(warehouseId);
        }
    }
}
