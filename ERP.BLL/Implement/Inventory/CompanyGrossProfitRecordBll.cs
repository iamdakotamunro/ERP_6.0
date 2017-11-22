using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using ERP.Model.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 公司毛利
    /// </summary>
    /// zal 2016-05-20
    public class CompanyGrossProfitRecordBll
    {
        readonly IGoodsOrderDetail _goodsOrderDetail;
        readonly ICompanyGrossProfitRecord _companyGrossProfitRecord;
        readonly ICompanyGrossProfitRecordDetail _companyGrossProfitRecordDetail;
        readonly IWasteBookReport _wasteBookReport;
        readonly IWasteBook _wasteBookDao = new WasteBook(GlobalConfig.DB.FromType.Write);
        readonly int _grossProfitPageSize = string.IsNullOrEmpty(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitPageSize")) ? 1000 : int.Parse(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitPageSize"));
        readonly bool _grossProfitExecute = string.IsNullOrEmpty(Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitExecute")) ? false : Config.Keede.Library.ConfManager.GetAppsetting("GrossProfitExecute").Equals("True", StringComparison.OrdinalIgnoreCase);

        public CompanyGrossProfitRecordBll(IGoodsOrderDetail goodsOrderDetail, ICompanyGrossProfitRecord companyGrossProfitRecord, ICompanyGrossProfitRecordDetail companyGrossProfitRecordDetail, IWasteBookReport wasteBookReport)
        {
            _goodsOrderDetail = goodsOrderDetail;
            _companyGrossProfitRecord = companyGrossProfitRecord;
            _companyGrossProfitRecordDetail = companyGrossProfitRecordDetail;
            _wasteBookReport = wasteBookReport;
        }

        /// <summary>
        /// 添加每月公司毛利记录存档
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool AddCompanyGrossProfitRecord(DateTime dayTime, out string errorMsg)
        {
            errorMsg = string.Empty;
            var recordTime = new DateTime(dayTime.Year, dayTime.Month, dayTime.Day);
            if (recordTime > DateTime.Now)//用于处理历史数据或遗漏数据时，防止计算当天的数据以及合计当前月的前一个月的数据【此处理用于老的毛利计算程序，当前程序不需要单独处理遗漏数据】
            {
                return true;
            }
            bool isCurrentDay = false;//当前天是否执行失败
            bool result = true;

            #region 重新计算执行失败的日期数据
            var executeFailedDayTimeList = _goodsOrderDetail.GetExecuteFailedDayTime(new List<int> { 1, 2 });
            foreach (var item in executeFailedDayTimeList)
            {
                var executeDayTime = item.Split(',')[0];
                var executeType = item.Split(',')[1];
                isCurrentDay = DateTime.Parse(executeDayTime).AddDays(1).Equals(recordTime);

                #region 计算每天的数据(完成时间是今天的数据)
                result = CompanyGrossProfitForEveryDay(DateTime.Parse(executeDayTime).AddDays(1), int.Parse(executeType));
                #endregion

                #region 将生成的公司毛利明细数据转移到Report库中的公司毛利明细表中
                AddCompanyGrossProfitToReport(DateTime.Parse(executeDayTime).AddDays(1));
                #endregion
            }
            #endregion

            if (!isCurrentDay)
            {
                #region 计算每天的数据(完成时间是今天的数据)
                result = CompanyGrossProfitForEveryDay(recordTime, -1);
                #endregion

                #region 将生成的公司毛利明细数据转移到Report库中的公司毛利明细表中
                AddCompanyGrossProfitToReport(recordTime);
                #endregion
            }

            if (_grossProfitExecute)
            {
                #region 将ERP库中lmShop_WasteBook表中的交易佣金数据根据订单号合并转移到Report库中WasteBook表
                //CompanyGrossProfitCommission(recordTime);
                #endregion

                #region 处理完成时间超过一个自然月或一个自然月以上的数据
                CompanyGrossProfitMoreMonth(recordTime);
                #endregion
            }

            #region 计算每月的数据
            result = CompanyGrossProfitForEveryMonth(recordTime, out errorMsg);
            #endregion

            return result;
        }

        /// <summary>
        /// 计算每天的数据(完成时间是今天的数据)
        /// </summary>
        /// <param name="recordTime"></param>
        /// <param name="executeType">1:公司毛利(订单)；2:公司毛利(出入库单据)；3:商品毛利(订单)；4:商品毛利(出入库单据)</param>
        /// <returns></returns>
        public bool CompanyGrossProfitForEveryDay(DateTime recordTime, int executeType)
        {
            DateTime startTime = recordTime.AddDays(-1);
            if (!_companyGrossProfitRecordDetail.Exists(startTime) || !executeType.Equals(-1))
            {
                #region 计算每天的数据(完成时间是今天的数据)
                return CompanyGrossProfitForEveryDayExecute(startTime, recordTime, executeType);
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 将生成的公司毛利明细数据转移到Report库中的公司毛利明细表中
        /// </summary>
        /// <returns></returns>
        public void AddCompanyGrossProfitToReport(DateTime recordTime)
        {
            DateTime dayTime = recordTime.AddDays(-1);
            List<CompanyGrossProfitRecordDetailInfo> companyGrossProfitRecordDetailList = new List<CompanyGrossProfitRecordDetailInfo>();
            int i = 0;
            while (true)
            {
                i++;
                var companyGrossProfitList = _goodsOrderDetail.GetCompanyGrossProfit(dayTime, i, _grossProfitPageSize);
                if (companyGrossProfitList.Count > 0)
                {
                    companyGrossProfitRecordDetailList.AddRange(companyGrossProfitList);
                }
                else
                {
                    break;
                }
            }

            if (companyGrossProfitRecordDetailList.Count > 0)
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
                {
                    try
                    {
                        int j = 0;
                        while (true)
                        {
                            var companyGrossProfitRecordDetailInfoList = companyGrossProfitRecordDetailList.Skip(j * _grossProfitPageSize).Take(_grossProfitPageSize).ToList();
                            if (companyGrossProfitRecordDetailInfoList.Count > 0)
                            {
                                _companyGrossProfitRecordDetail.AddDataDetail(companyGrossProfitRecordDetailInfoList);
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
        /// 将产生的订单佣金更新到相应的订单中
        /// </summary>
        /// <param name="recordTime"></param>
        public void CompanyGrossProfitCommission(DateTime recordTime)
        {
            #region 将ERP库中lmShop_WasteBook表中的交易佣金数据根据订单号合并转移到Report库中WasteBook表
            var dateCreated = new DateTime(recordTime.AddDays(-1).Year, recordTime.AddDays(-1).Month, recordTime.AddDays(-1).Day);
            var wasteBookInfoList = _wasteBookDao.GetWasteBookByDateCreatedForProfits();
            if (wasteBookInfoList.Count > 0)
            {
                var wasteBookList = wasteBookInfoList.Select(item => new WasteBookInfo
                {
                    Id = Guid.NewGuid(),
                    OrderNo = item.LinkTradeCode,
                    Income = item.Income,
                    DateCreated = dateCreated,
                    State = 0
                }).ToList();

                bool resultAdd = _wasteBookReport.AddWasteBook(wasteBookList);
                if (resultAdd)
                {
                    bool resultUpdate = _wasteBookDao.UpdateOperateState();
                    if (!resultUpdate)
                    {
                        _wasteBookReport.DelWasteBook(dateCreated);
                    }
                    else
                    {
                        #region 将产生的订单佣金更新到相应的订单中
                        _companyGrossProfitRecordDetail.UpdateCatCommission();
                        #endregion
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 处理完成时间超过一个自然月或一个自然月以上的数据
        /// </summary>
        /// <param name="recordTime"></param>
        public void CompanyGrossProfitMoreMonth(DateTime recordTime)
        {
            #region 处理完成时间超过一个自然月或一个自然月以上的数据
            //合计公司毛利中超过一个自然月或一个自然月以上未完成的数据
            var companyGrossProfitRecordList = _companyGrossProfitRecordDetail.GetCompanyGrossProfitDetailInfosForMoreMonth(recordTime);
            if (companyGrossProfitRecordList.Count > 0)
            {
                foreach (var item in companyGrossProfitRecordList)
                {
                    _companyGrossProfitRecord.UpdateCompanyGrossProfitRecordInfo(item);
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
        public bool CompanyGrossProfitForEveryMonth(DateTime recordTime, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (recordTime.Day == 1)
            {
                //每月1号计算上上个月的公司毛利
                /*
                例如：当前时间是6月1号，5月份的数据暂不合计(原因：5月份的数据是不准确的，一是因为可能有些订单在5月份还没有完成，即完成时间可能是在6月份的，如果此时合计5月份的数据，会遗漏完成时间是6月份的订单；二是可能有些订单还没有天猫佣金) 
                故此时只合计4月份的数据(原因：4月份的数据是相对准确的，一是因为大部分的订单在一个自然月内应该都已经完成了；二是因为天猫佣金是在用户确认收货的时候才生成的，为兼容天猫大型活动(如双十一活动)时收货期延长，故隔月计算时的数据是相对准确的)
                此处佣金生成都按照用户未确认收货，系统自动确认收货来考虑;
                */

                #region 4月份数据处理(合计订单时间是4月份的数据)
                DateTime startTime = recordTime.AddMonths(-2);
                DateTime endTime = recordTime.AddMonths(-1);
                if (_companyGrossProfitRecord.Exists(startTime))
                {
                    return true;
                }

                //合计公司毛利明细
                var data = _companyGrossProfitRecordDetail.SumCompanyGrossProfitDetailInfos(startTime, endTime, Guid.Empty, string.Empty, string.Empty);
                if (data.Any())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
                    {
                        try
                        {
                            int i = 0;
                            while (true)
                            {
                                var companyGrossProfitRecordList = data.Skip(i * _grossProfitPageSize).Take(_grossProfitPageSize).ToList();
                                if (companyGrossProfitRecordList.Count > 0)
                                {
                                    _companyGrossProfitRecord.AddData(companyGrossProfitRecordList);
                                }
                                else
                                {
                                    break;
                                }
                                i++;
                            }

                            _companyGrossProfitRecordDetail.UpdateState(startTime, endTime);
                            ts.Complete();
                        }
                        catch (Exception ex)
                        {
                            errorMsg = startTime.ToString("yyyy-MM") + "公司毛利存档失败！" + ex.Message;
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
                    errorMsg = startTime.ToString("yyyy-MM") + "公司毛利存档失败！(当月没有数据)";
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的公司毛利
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="executeType">1:公司毛利(订单)；2:公司毛利(出入库单据)；3:商品毛利(订单)；4:商品毛利(出入库单据)</param>
        /// <returns></returns>
        public bool CompanyGrossProfitForEveryDayExecute(DateTime startTime, DateTime endTime, int executeType)
        {
            bool result1 = true;
            bool result2 = true;
            switch (executeType)
            {
                case 1:
                    //订单
                    result1 = _goodsOrderDetail.CompanyGrossProfitForEveryDay_GoodsOrder(startTime, endTime);
                    break;
                case 2:
                    //门店采购出库单
                    result2 = _goodsOrderDetail.CompanyGrossProfitForEveryDay_StorageRecord(startTime, endTime);
                    break;
                default:
                    //订单
                    result1 = _goodsOrderDetail.CompanyGrossProfitForEveryDay_GoodsOrder(startTime, endTime);
                    //门店采购出库单
                    result2 = _goodsOrderDetail.CompanyGrossProfitForEveryDay_StorageRecord(startTime, endTime);
                    break;
            }
            return result1 && result2;
        }
    }
}
