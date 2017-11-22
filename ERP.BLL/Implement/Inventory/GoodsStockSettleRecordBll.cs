using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IStorage;
using ERP.Enum;
using ERP.Model;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Inventory
{
    public class GoodsStockSettleRecordBll
    {
        private readonly IStorageRecordDao _storageRecordDao;
        readonly IGoodsStockRecord _goodsStockRecord;
        private readonly IGoodsCenterSao _goodsInfoSao;

        public GoodsStockSettleRecordBll(IGoodsStockRecord goodsStockRecord,IGoodsCenterSao goodsCenterSao)
        {
            _goodsStockRecord = goodsStockRecord;
            _goodsInfoSao = goodsCenterSao;

        }

        public GoodsStockSettleRecordBll(IGoodsStockRecord goodsStockRecord,IStorageRecordDao storageRecordDao,IGoodsCenterSao goodsInfoSao)
        {
            _goodsStockRecord =goodsStockRecord;
            _storageRecordDao = storageRecordDao;
            _goodsInfoSao = goodsInfoSao;
        }


        #region   商品库存、结算价存档

        /// <summary>
        /// 添加结算价与商品库存存档数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool AddSettlePriceAndStockRecord(int year, int month)
        {
            var startTime = new DateTime(year, month, 1);  //10月
            //判断9月份是否存在结算价记录
            if (!_goodsStockRecord.IsExistsSettlePriceRecord(startTime.AddMonths(-1)))
            {
                IList<GoodsStockPriceRecordInfo> goodsStockPriceRecordInfos = new List<GoodsStockPriceRecordInfo>();
                //Key: 商品主键ID  Value: 当月进货均价
                var totalDics = new List<Guid>();
                //8月月末库存
                var stockNums = _storageRecordDao.SelectMonthGoodsStockRecordInfos(startTime.AddMonths(-2));
                if (stockNums == null || stockNums.Count == 0) return true;
                var settlePriceDics = _goodsStockRecord.GetGoodsSettlePriceDicts(startTime.AddMonths(-2));
                var stockTypeList = new List<int>
                    {
                        (int) StorageRecordType.BuyStockIn,
                        (int) StorageRecordType.BuyStockOut
                    };
                //获取9月份的出入库数
                var storageList = _storageRecordDao.SelectGoodsStockInfos(stockTypeList, startTime.AddMonths(-1), startTime.AddSeconds(-1));
                foreach (var monthGoodsStockRecordInfo in storageList)
                {
                    var info = monthGoodsStockRecordInfo;
                    var preSettlePrice = settlePriceDics.ContainsKey(info.GoodsId) ? settlePriceDics[info.GoodsId] : 0;

                    //采购进货、退货、拆分组合入库、出库列表
                    var buyInAndOutList = storageList.Where(act => act.GoodsId == info.GoodsId).ToList();
                    //月末库存商品列表
                    var stockList = stockNums.Where(act => act.GoodsId == info.GoodsId).ToList();
                    var stockNum =stockList.Count>0?stockList.Sum(act => act.NonceGoodsStock):0;  //总库存数
                    if (!totalDics.Contains(info.GoodsId))
                    {
                        totalDics.Add(info.GoodsId);
                        var monthNum = buyInAndOutList.Sum(act => act.Quantity);  //月采购、拆分组合出入库总数
                        var monthAvgPrice = monthNum != 0 ? buyInAndOutList.Sum(act => act.TotalPrice) / monthNum : 0;  //每月平均价
                        var totalPrice = stockNum * preSettlePrice + monthNum * monthAvgPrice;
                        decimal settlePrice = stockNum + monthNum != 0 && totalPrice > 0 ? totalPrice / (stockNum + monthNum) : preSettlePrice; // 每月结算价
                        goodsStockPriceRecordInfos.Add(new GoodsStockPriceRecordInfo
                        {
                            AvgSettlePrice = Math.Abs(settlePrice),
                            DayTime = startTime.AddMonths(-1),
                            GoodsId = info.GoodsId,
                            MonthAvgPrice = Math.Abs(monthAvgPrice)
                        });
                    }
                }
                foreach (var settlePriceDic in settlePriceDics.Where(act => totalDics.All(a => a != act.Key)))
                {
                    goodsStockPriceRecordInfos.Add(new GoodsStockPriceRecordInfo
                    {
                        AvgSettlePrice = settlePriceDic.Value,
                        DayTime = startTime.AddMonths(-1),
                        GoodsId = settlePriceDic.Key,
                        MonthAvgPrice = 0
                    });
                }
                //将当月的结算价、库存记录保存到数据库
                return _goodsStockRecord.BatchInsert(goodsStockPriceRecordInfos);
            }
            return true;
        }

        /// <summary>
        /// 商品库存金额明细
        /// </summary>
        /// <param name="goodsType"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<MonthGoodsStockRecordInfo> SelectMonthGoodsStockRecordInfos(int goodsType, int year, int month)
        {
            var dataList = _goodsStockRecord.SelectMonthGoodsStockRecordInfos(goodsType, year, month);
            var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(dataList.Select(act => act.GoodsId).Distinct().ToList());
            foreach (var monthGoodsStockRecordInfo in dataList)
            {
                var info = goodsInfos.FirstOrDefault(act => act.GoodsId == monthGoodsStockRecordInfo.GoodsId);
                if (info != null)
                {
                    monthGoodsStockRecordInfo.GoodsName = info.GoodsName;
                    monthGoodsStockRecordInfo.GoodsCode = info.GoodsCode;
                }
            }
            return dataList;
        }
        #endregion

        /// <summary>
        /// 备份月末库存
        /// </summary>
        /// <returns></returns>
        public bool CopyMonthGoodsStockInfos(DateTime dayTime)
        {
            if (_goodsStockRecord.IsExistsGoodsStockRecord(dayTime))
            {
                return true;
            }
            var stockNums = _storageRecordDao.SelectMonthGoodsStockRecordInfos(dayTime);
            if (stockNums == null || stockNums.Count == 0) return true;
            var goodsIds = stockNums.Select(act => act.GoodsId).Distinct().ToList();
            var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
            if (goodsInfos == null || goodsInfos.Count == 0) return true; 
            var dics = goodsInfos.ToDictionary(k => k.GoodsId, v => v.GoodsType);
            IList<MonthGoodsStockRecordInfo> monthGoodsStockRecordInfos = stockNums.Select(info => new MonthGoodsStockRecordInfo
            {
                ID = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                DayTime = dayTime,
                FilialeId = info.FilialeId,
                GoodsId = info.GoodsId,
                RealGoodsId = info.RealGoodsId,
                NonceGoodsStock = info.NonceGoodsStock,
                GoodsType = dics.ContainsKey(info.GoodsId) ? dics[info.GoodsId] : 0,
                WarehouseId = info.WarehouseId
            }).ToList();
            return _goodsStockRecord.CopyMonthGoodsStockInfos(monthGoodsStockRecordInfos);
        }

        public bool AddNewGoodsSettlePrice(int year, int month)
        {
            var startTime = new DateTime(year, month, 1);
            var stockTypeList = new List<int>
                    {
                        (int) StorageRecordType.BuyStockIn,
                        (int) StorageRecordType.BuyStockOut
                    };
            //获取当月采购进出入库记录
            var storageList = _storageRecordDao.SelectGoodsStockInfos(stockTypeList, startTime, startTime.AddMonths(1).AddSeconds(-1));
            //获取当月结算价记录
            var settlePriceDics = _goodsStockRecord.GetGoodsSettlePriceDicts(startTime);

            IList<GoodsStockPriceRecordInfo> goodsStockPriceRecordInfos = (from info in storageList.Where(act => !settlePriceDics.Keys.Contains(act.GoodsId))
                                                                           let buyInAndOutList = storageList.Where(act => act.GoodsId == info.GoodsId).ToList()
                                                                           let monthNum = buyInAndOutList.Sum(act => act.Quantity)
                                                                           let monthAvgPrice = monthNum != 0 ? buyInAndOutList.Sum(act => act.TotalPrice) / monthNum : 0
                                                                           select new GoodsStockPriceRecordInfo
                                                                           {
                                                                               AvgSettlePrice = Math.Abs(monthAvgPrice),
                                                                               DayTime = startTime,
                                                                               GoodsId = info.GoodsId,
                                                                               MonthAvgPrice = Math.Abs(monthAvgPrice)
                                                                           }).ToList();

            //将当月的结算价、库存记录保存到数据库
            return _goodsStockRecord.BatchInsert(goodsStockPriceRecordInfos);
        }
    }
}
