using System;
using System.Collections.Generic;
using System.Linq;
using StorageTask.Core.Model;

namespace StorageTask.Core
{
    public class StorageBalanceTaskManager
    {
        /// <summary>
        /// 降序计算库存余额流水
        /// </summary>
        public static int CalculateStorageDetailBalance(bool isDebug)
        {
            IList<NonceGoodsStockInfo> noBalanceGoodsIdList;
            try
            {
                //先获取没有库存流水的商品信息
                noBalanceGoodsIdList = DAL.StorageRecordDetailDao.GetNoStorageRecordBalanceGoodsId(100);
                ERP.SAL.LogCenter.LogService.LogInfo(string.Format("读取库存流水，isDebug={0}, noBalanceGoodsIdList.Count={1}", isDebug, noBalanceGoodsIdList.Count), "任务中心出入库", null);
            }
            catch (Exception exp)
            {
                if (isDebug)
                {
                    Console.WriteLine("没有读取到需要计算商品列表！" + exp.Message);
                }
                else
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("读取没有库存流水报错，isDebug={0}", isDebug), "任务中心出入库", exp);
                }
                return 0;
            }

            foreach (var noBalanceInfo in noBalanceGoodsIdList)
            {
                //统计成功记录
                int sumSuccess = 0;

                //降序还是升序
                bool isDesc = false;

                //遍历每一个商品获取最近库存信息，如果没有就去当前库存信息
                NonceGoodsStockInfo recentBalanceInfo =
                    DAL.StorageRecordBalanceDetailDao.GetRecentBalanceInfo(noBalanceInfo.RealGoodsId,
                        noBalanceInfo.WarehouseId);
                if (recentBalanceInfo == null)
                {
                    isDesc = true;
                    NonceGoodsStockInfo currentBalanceInfo =
                        DAL.GoodsStockCurrentDao.GetNonceStockInfo(noBalanceInfo.RealGoodsId,
                            noBalanceInfo.WarehouseId) ??
                        new NonceGoodsStockInfo
                        {
                            NonceBalance = 0,
                            RealGoodsId = noBalanceInfo.RealGoodsId,
                            WarehouseId = noBalanceInfo.WarehouseId
                        };
                    recentBalanceInfo = new NonceGoodsStockInfo
                    {
                        NonceBalance = currentBalanceInfo.NonceBalance,
                        RealGoodsId = currentBalanceInfo.RealGoodsId,
                        WarehouseId = currentBalanceInfo.WarehouseId
                    };
                }

                //读取当前商品没有库存流水的所有出入库记录
                var allNeedCalculateList = DAL.StorageRecordDetailDao.GetNeedCalculateList(
                    noBalanceInfo.RealGoodsId,
                    noBalanceInfo.WarehouseId);
                if (allNeedCalculateList.Count == 0)
                {
                    continue;
                }
                if (isDesc)
                {
                    //如果是降序要算出初始库存
                    var sumQuantity = allNeedCalculateList.Sum(ent => ent.Quantity);
                    recentBalanceInfo.NonceBalance = recentBalanceInfo.NonceBalance - sumQuantity;
                }
                foreach (var item in allNeedCalculateList)
                {
                    var insertItem = item;
                    recentBalanceInfo.NonceBalance = recentBalanceInfo.NonceBalance + insertItem.Quantity;
                    insertItem.NonceBalance = recentBalanceInfo.NonceBalance;
                    var success = DAL.StorageRecordBalanceDetailDao.Insert(insertItem);
                    if (!success)
                    {
                        break;
                    }
                    ++sumSuccess;
                }
                if (isDebug)
                {
                    Console.WriteLine("商品ID：{0},读取:{1},执行:{2}", noBalanceInfo.RealGoodsId,
                        allNeedCalculateList.Count, sumSuccess);
                }
                else
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("读取库存流水异常,isDebug={0}, noBalanceInfo.RealGoodsId={1}, allNeedCalculateList.Count={2}", isDebug, noBalanceInfo.RealGoodsId, allNeedCalculateList.Count), "任务中心出入库", null);
                    //    string.Format("商品ID：{0},读取:{1},执行:{2}", noBalanceInfo.RealGoodsId,
                    //        allNeedCalculateList.Count, sumSuccess), string.Empty);
                }
            }
            return noBalanceGoodsIdList.Count;
        }
    }
}
