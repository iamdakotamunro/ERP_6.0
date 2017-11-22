using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Model.Report;
using ERP.SAL.Interface;
using ERP.SAL.Goods;
using System.Transactions;
using System.Configuration;

namespace ERP.BLL.Implement.Inventory
{
    public class GoodsGrossProfitBll
    {
        private readonly IGoodsGrossProfit _goodsGrossProfit;
        private readonly IGoodsGrossProfitRecordDetail _goodsGrossProfitRecordDetail;
        private readonly IGoodsCenterSao _goodsInfoSao;
        private readonly IGoodsOrderDetail _goodsOrderDetail;
        private readonly IPromotionSao _promotionSao;
        readonly int _grossProfitPageSize = string.IsNullOrEmpty(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitPageSize")) ? 1000 : int.Parse(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitPageSize"));
        readonly bool _grossProfitExecute = string.IsNullOrEmpty(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitExecute")) ? false : Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitExecute").Equals("True", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 当前月份数据查询、添加历史毛利记录构造函数
        /// </summary>
        /// <param name="goodsInfoSao"></param>
        /// <param name="goodsGrossProfit"></param>
        /// <param name="goodsOrderDetail"></param>
        /// <param name="promotionSao"></param>
        /// <param name="goodsGrossProfitRecordDetail"></param>
        public GoodsGrossProfitBll(IGoodsCenterSao goodsInfoSao, IGoodsGrossProfit goodsGrossProfit, IGoodsOrderDetail goodsOrderDetail, IPromotionSao promotionSao, IGoodsGrossProfitRecordDetail goodsGrossProfitRecordDetail)
        {
            _goodsGrossProfit = goodsGrossProfit;
            _goodsInfoSao = goodsInfoSao;
            _goodsOrderDetail = goodsOrderDetail;
            _promotionSao = promotionSao;
            _goodsGrossProfitRecordDetail = goodsGrossProfitRecordDetail;
        }

        /// <summary>
        /// 生成历史月份的商品毛利记录
        /// </summary>
        /// <param name="goodsGrossProfit"></param>
        /// <param name="goodsOrderDetail"></param>
        /// <param name="promotionSao"></param>
        /// <param name="goodsGrossProfitRecordDetail"></param>
        public GoodsGrossProfitBll(IGoodsGrossProfit goodsGrossProfit, IGoodsOrderDetail goodsOrderDetail, IPromotionSao promotionSao, IGoodsGrossProfitRecordDetail goodsGrossProfitRecordDetail)
        {
            _goodsGrossProfit = goodsGrossProfit;
            _goodsOrderDetail = goodsOrderDetail;
            _promotionSao = promotionSao;
            _goodsGrossProfitRecordDetail = goodsGrossProfitRecordDetail;
        }

        /// <summary>
        /// 添加每月商品毛利记录存档
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool AddGoodsGrossProfitRecord(DateTime dayTime, out string errorMsg)
        {
            errorMsg = string.Empty;
            var recordTime = new DateTime(dayTime.Year, dayTime.Month, dayTime.Day);
            if (recordTime > DateTime.Now)//用于处理历史数据或遗漏数据时，防止计算当天的数据以及合计当前月的前一个月的数据【此处理用于老的毛利计算程序，当前程序不需要单独处理遗漏数据】
            {
                return true;
            }
            bool isCurrentDay = false;//当前天是否执行失败
            bool result = true;

            #region 重新计算执行失败的日期的数据
            var executeFailedDayTimeList = _goodsOrderDetail.GetExecuteFailedDayTime(new List<int> { 3, 4 });
            foreach (var item in executeFailedDayTimeList)
            {
                var executeDayTime = item.Split(',')[0];
                var executeType = item.Split(',')[1];
                isCurrentDay = DateTime.Parse(executeDayTime).AddDays(1).Equals(recordTime);

                #region 计算每天的数据(完成时间是今天的数据)
                result = GoodsGrossProfitForEveryDay(DateTime.Parse(executeDayTime).AddDays(1), int.Parse(executeType));
                #endregion

                #region 将生成的商品毛利明细数据转移到Report库中的商品毛利明细表中
                AddGoodsGrossProfitToReport(DateTime.Parse(executeDayTime).AddDays(1));
                #endregion
            }
            #endregion

            if (!isCurrentDay)
            {
                #region 计算每天的数据(完成时间是今天的数据)
                result = GoodsGrossProfitForEveryDay(recordTime, -1);
                #endregion

                #region 将生成的商品毛利明细数据转移到Report库中的商品毛利明细表中
                AddGoodsGrossProfitToReport(recordTime);
                #endregion
            }

            if (_grossProfitExecute)
            {
                #region 处理完成时间超过一个自然月或一个自然月以上的数据
                GoodsGrossProfitMoreMonth(recordTime);
                #endregion
            }

            #region 计算每月的数据
            result = GoodsGrossProfitForEveryMonth(recordTime, out errorMsg);
            #endregion

            return result;
        }

        /// <summary>
        /// 计算每天的数据(完成时间是今天的数据)
        /// </summary>
        /// <param name="recordTime"></param>
        /// <param name="executeType">1:公司毛利(订单)；2:公司毛利(出入库单据)；3:商品毛利(订单)；4:商品毛利(出入库单据)</param>
        /// <returns></returns>
        public bool GoodsGrossProfitForEveryDay(DateTime recordTime, int executeType)
        {
            DateTime startTime = recordTime.AddDays(-1);
            if (!_goodsGrossProfitRecordDetail.Exists(startTime) || !executeType.Equals(-1))
            {
                #region 计算每天的数据(完成时间是今天的数据)
                return GoodsGrossProfitForEveryDayExecute(startTime, recordTime, executeType);
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 将生成的商品毛利明细数据转移到Report库中的商品毛利明细表中
        /// </summary>
        /// <returns></returns>
        public void AddGoodsGrossProfitToReport(DateTime recordTime)
        {
            DateTime dayTime = recordTime.AddDays(-1);
            List<GoodsGrossProfitRecordDetailInfo> goodsGrossProfitRecordDetailList = new List<GoodsGrossProfitRecordDetailInfo>();
            int i = 0;
            while (true)
            {
                i++;
                #region 订单
                var goodsGrossProfitGoodsOrderList = _goodsOrderDetail.GetGoodsGrossProfit_GoodsOrder(dayTime, i, _grossProfitPageSize);
                if (goodsGrossProfitGoodsOrderList.Count > 0)
                {
                    goodsGrossProfitRecordDetailList.AddRange(goodsGrossProfitGoodsOrderList);
                }
                #endregion

                #region 门店采购出库单
                var goodsGrossProfitStorageRecordList = _goodsOrderDetail.GetGoodsGrossProfit_StorageRecord(dayTime, i, _grossProfitPageSize);
                if (goodsGrossProfitStorageRecordList.Count > 0)
                {
                    var goodsIdList = goodsGrossProfitStorageRecordList.Select(act => act.GoodsId).Distinct().ToList();
                    var dicGoodsIdAndGoodsType = new GoodsCenterSao().GetGoodsListByGoodsIds(goodsIdList).ToDictionary(p => p.GoodsId, p => p.GoodsType);
                    foreach (var item in goodsGrossProfitStorageRecordList)
                    {
                        item.GoodsType = dicGoodsIdAndGoodsType.ContainsKey(item.GoodsId) ? dicGoodsIdAndGoodsType[item.GoodsId] : 0;
                    }
                    goodsGrossProfitRecordDetailList.AddRange(goodsGrossProfitStorageRecordList);
                }
                #endregion

                if (goodsGrossProfitGoodsOrderList.Count == 0 && goodsGrossProfitStorageRecordList.Count == 0)
                {
                    break;
                }
            }

            if (goodsGrossProfitRecordDetailList.Count > 0)
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
                {
                    try
                    {
                        int j = 0;
                        while (true)
                        {
                            var companyGrossProfitRecordDetailInfoList = goodsGrossProfitRecordDetailList.Skip(j * _grossProfitPageSize).Take(_grossProfitPageSize).ToList();
                            if (companyGrossProfitRecordDetailInfoList.Count > 0)
                            {
                                _goodsGrossProfitRecordDetail.AddDataDetail(companyGrossProfitRecordDetailInfoList);
                            }
                            else
                            {
                                break;
                            }
                            j++;
                        }
                        ts.Complete();
                    }
                    finally
                    {
                        //释放资源
                        ts.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 处理完成时间超过一个自然月或一个自然月以上的数据
        /// </summary>
        /// <param name="recordTime"></param>
        public void GoodsGrossProfitMoreMonth(DateTime recordTime)
        {
            #region 处理完成时间超过一个自然月或一个自然月以上的数据
            //合计商品毛利中超过一个自然月或一个自然月以上未完成的数据
            var goodsGrossProfitRecordList = _goodsGrossProfitRecordDetail.GetGoodsGrossProfitRecordDetailInfosForMoreMonth(recordTime);
            if (goodsGrossProfitRecordList.Any())
            {
                foreach (var item in goodsGrossProfitRecordList)
                {
                    _goodsGrossProfit.UpdateGoodsGrossProfitInfo(item);
                }
            }
            #endregion
        }

        /// <summary>
        /// 计算每月的数据
        /// </summary>
        /// <param name="recordTime"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool GoodsGrossProfitForEveryMonth(DateTime recordTime, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (recordTime.Day == 1)
            {
                //每月1号计算上上个月的公司毛利
                /*
                例如：当前时间是6月1号，5月份的数据暂不合计(原因：5月份的数据是不准确的，因为可能有些订单在5月份还没有完成，即完成时间可能是在6月份的，如果此时合计5月份的数据，会遗漏完成时间是6月份的订单；) 
                故此时只合计4月份的数据(原因：4月份的数据是相对准确的，因为大部分的订单在一个自然月内应该都已经完成了；)
                 */

                #region 4月份数据处理(合计订单时间是4月份的数据)
                DateTime startTime = recordTime.AddMonths(-2);
                DateTime endTime = recordTime.AddMonths(-1);
                if (_goodsGrossProfit.Exists(startTime))
                {
                    return true;
                }

                var data = _goodsGrossProfitRecordDetail.SumGoodsGrossProfitRecordDetailInfos(startTime, endTime, string.Empty, Guid.Empty, string.Empty, string.Empty);
                if (data.Any())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
                    {
                        try
                        {
                            int i = 0;
                            while (true)
                            {
                                var goodsGrossProfitRecordList = data.Skip(i * _grossProfitPageSize).Take(_grossProfitPageSize).ToList();
                                if (goodsGrossProfitRecordList.Count > 0)
                                {
                                    _goodsGrossProfit.AddData(goodsGrossProfitRecordList);
                                }
                                else
                                {
                                    break;
                                }
                                i++;
                            }

                            _goodsGrossProfitRecordDetail.UpdateState(startTime, endTime);
                            ts.Complete();
                        }
                        catch (Exception ex)
                        {
                            errorMsg = startTime.ToString("yyyy-MM") + "商品毛利存档失败！" + ex.Message;
                            return false;
                        }
                        finally
                        {
                            //释放资源
                            ts.Dispose();
                        }
                    }
                }
                else
                {
                    errorMsg = startTime.ToString("yyyy-MM") + "商品毛利存档失败！(当月没有数据)";
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的商品毛利
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="executeType">1:公司毛利(订单)；2:公司毛利(出入库单据)；3:商品毛利(订单)；4:商品毛利(出入库单据)</param>
        /// <returns></returns>
        public bool GoodsGrossProfitForEveryDayExecute(DateTime startTime, DateTime endTime, int executeType)
        {
            bool result1 = true;
            bool result2 = true;
            switch (executeType)
            {
                case 3:
                    //订单
                    result1 = _goodsOrderDetail.GoodsGrossProfitForEveryDay_GoodsOrder(startTime, endTime);
                    break;
                case 4:
                    //门店采购出库单
                    result2 = _goodsOrderDetail.GoodsGrossProfitForEveryDay_StorageRecord(startTime, endTime);
                    break;
                default:
                    //订单
                    result1 = _goodsOrderDetail.GoodsGrossProfitForEveryDay_GoodsOrder(startTime, endTime);
                    //门店采购出库单
                    result2 = _goodsOrderDetail.GoodsGrossProfitForEveryDay_StorageRecord(startTime, endTime);
                    break;
            }
            return result1 && result2;
        }

        #region 页面查询
        /// <summary>
        /// 获取历史月份的商品毛利列表
        /// </summary>
        /// <param name="startTIme"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformIds"></param>
        /// <param name="orderTypes"></param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SelectGoodsGrossProfitInfos(DateTime startTIme, DateTime endTime, string goodsTypes, string goodsNameOrCode, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            var goodsGrossProfitInfo = _goodsGrossProfit.SelectGoodsGrossProfitInfos(startTIme, endTime, goodsTypes, saleFilialeId, salePlatformIds, orderTypes);
            return ShowGoodsGrossProfitInfos(goodsGrossProfitInfo, goodsNameOrCode);
        }

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTIme"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformIds"></param>
        /// <param name="orderTypes"></param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitFromMonthByGoodsIdAndSaleFilialeId(DateTime startTIme, DateTime endTime, string goodsTypes, string goodsNameOrCode, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            var goodsGrossProfitInfo = _goodsGrossProfit.SumGoodsGrossProfitFromMonthByGoodsIdAndSaleFilialeId(startTIme, endTime, goodsTypes, saleFilialeId, salePlatformIds, orderTypes);
            return ShowGoodsGrossProfitInfos(goodsGrossProfitInfo, goodsNameOrCode);
        }

        /// <summary>
        /// 数据组装、商品名称和code赋值
        /// </summary>
        /// <param name="goodsGrossProfitInfos"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        private IList<GoodsGrossProfitInfo> ShowGoodsGrossProfitInfos(IList<GoodsGrossProfitInfo> goodsGrossProfitInfos, string goodsNameOrCode)
        {
            var dataList = new List<GoodsGrossProfitInfo>();
            if (goodsGrossProfitInfos != null && goodsGrossProfitInfos.Count > 0)
            {
                var goodsIds = goodsGrossProfitInfos.Select(act => act.GoodsId).Distinct().ToList();
                var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsInfos != null && goodsInfos.Count > 0)
                {
                    if (!string.IsNullOrEmpty(goodsNameOrCode))
                    {
                        goodsInfos = goodsInfos.Where(act => act.GoodsName.Contains(goodsNameOrCode) || act.GoodsCode.Contains(goodsNameOrCode) || act.OldGoodsCode.Contains(goodsNameOrCode)).ToList();
                    }
                    foreach (var goodsGrossProfitInfo in goodsGrossProfitInfos)
                    {
                        var info = goodsInfos.FirstOrDefault(act => act.GoodsId == goodsGrossProfitInfo.GoodsId);
                        if (info == null) continue;
                        goodsGrossProfitInfo.GoodsName = info.GoodsName;
                        goodsGrossProfitInfo.GoodsCode = info.GoodsCode;
                        dataList.Add(goodsGrossProfitInfo);
                    }
                }
            }
            return dataList;
        }
        #endregion
    }
}
