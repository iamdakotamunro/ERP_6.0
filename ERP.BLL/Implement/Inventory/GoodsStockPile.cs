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

        /// <summary>��������ƷID��ѯ�䵱ǰ�ֿ�����Ʒ�Ŀ����Ϣ
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

            //��������Ʒ��ȡ��������Ʒ��Ϣ
            var childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId }).ToList();
            if (childGoodsList.Count > 0)
            {
                var goodsIds = childGoodsList.Select(w => w.RealGoodsId).ToList();
                var dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseId, null, goodsIds, hostingFilialeId);
                if (dicRealGoodsIdAndStockQuantity != null && dicRealGoodsIdAndStockQuantity.Count > 0)
                {
                    var goodsIdList = new List<Guid> { goodsId };
                    //���ݲֿ�Id��ȡ��Ӧ��Id
                    var dicGoodsIdAndCompanyId = _purchaseSet.GetCompanyIdByWarehouseId(warehouseId, hostingFilialeId);
                    var companyIdList = dicGoodsIdAndCompanyId.Where(p => goodsIdList.Contains(p.Key)).Select(p => p.Value).ToList();
                    //��ȡ��Ʒ�����һ�ν�������Ϣ
                    var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(warehouseId);
                    goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIdList.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

                    //���ؿ��>0������Ʒ
                    foreach (var childGoodsInfo in childGoodsList)
                    {
                        //�ɳ�����
                        var goodsStockKeyValuePair = dicRealGoodsIdAndStockQuantity.FirstOrDefault(w => w.Key == childGoodsInfo.RealGoodsId);
                        //������Ʒid��ȡ��Ӧ��
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

        /// <summary>�õ�ƽ�������ת 2015-04-29  ������
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <param name="goodsIds">��ƷID����</param>
        /// <param name="state">0ȫ����1�¼ܻ�ȱ�����п�棬2��������Ʒ</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetAvgStockTurnOver(DateTime startTime, DateTime endTime, Guid warehouseId, List<Guid> goodsIds, int state)
        {
            return _goodsStockPileDao.GetAvgStockTurnOver(startTime, endTime, warehouseId, goodsIds, state);
        }

        /// <summary>��ȡ��Ʒ�Ŀ����ת��  2015-04-30 ������
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <returns></returns>
        public IList<StockTurnOverInfo> GetGoodsStockTurnOverByGoodsId(DateTime startTime, DateTime endTime, Guid goodsId, Guid warehouseId)
        {
            return _goodsStockPileDao.GetGoodsStockTurnOverByGoodsId(startTime, endTime, goodsId, warehouseId);
        }

        /// <summary>��ȡ��Ʒ��3�������������  2015-06-16  ������
        /// </summary>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <returns></returns>
        public IList<SalesVolumeInfo> GetGoodsSalesVolume(Guid warehouseId)
        {
            return _goodsStockPileDao.GetGoodsSalesVolume(warehouseId);
        }
    }
}
