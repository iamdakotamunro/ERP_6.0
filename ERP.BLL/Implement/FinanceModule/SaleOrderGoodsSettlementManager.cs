using ERP.BLL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.FinanceModule;
using ERP.Enum;
using ERP.Environment;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Implement.FinanceModule;
using System.Threading;

namespace ERP.BLL.Implement.FinanceModule
{
    /// <summary>
    /// 销售订单商品结算价 业务层 
    /// </summary>
    /// <remarks>按订单记录当时的结算价，用于毛利计算</remarks>
    public class SaleOrderGoodsSettlementManager : BllInstance<SaleOrderGoodsSettlementManager>, ISaleOrderGoodsSettlementManager
    {
        private ISaleOrderGoodsSettlementDal _saleOrderGoodsSettlementDal = null;
        private IRealTimeGrossSettlementDal _realTimeGrossSettlementDal = null;
        private const string LOG_TAG = "销售订单商品结算价";

        public SaleOrderGoodsSettlementManager(GlobalConfig.DB.FromType fromType = GlobalConfig.DB.FromType.Write)
        {
            _saleOrderGoodsSettlementDal = new SaleOrderGoodsSettlementDal(fromType);
            _realTimeGrossSettlementDal = new RealTimeGrossSettlementDal(fromType);
        }

        public void Generate(DateTime statisticDate, int maxRowCount)
        {
            if (maxRowCount <= 0)
            {
                maxRowCount = 100;
            }
            try
            {
                var orderList = _saleOrderGoodsSettlementDal.GetUnsavedB2COrderList(statisticDate, maxRowCount);
                foreach (var orderInfo in orderList)
                {
                    var items = Create(orderInfo);
                    foreach (var item in items)
                    {
                        Save(item);
                    }
                }
                ERP.SAL.LogCenter.LogService.LogInfo(string.Format("此次生成来自B2C订单，销售订单商品结算价已完成！统计日期 {0}，此次取的数据量为 {1}", statisticDate.ToString("yyyy-MM-dd"), orderList.Count), LOG_TAG, null);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("生成来自B2C订单，销售订单商品结算价失败！统计日期 {0}", statisticDate.ToString("yyyy-MM-dd")), LOG_TAG, ex);
            }
            Thread.Sleep(1000);
            
            try
            {
                var orderList = _saleOrderGoodsSettlementDal.GetUnsavedSaleStockOutForSaleFilialeList(statisticDate, maxRowCount);
                foreach (var orderInfo in orderList)
                {
                    var items = Create(orderInfo);
                    foreach (var item in items)
                    {
                        Save(item);
                    }
                }
                ERP.SAL.LogCenter.LogService.LogInfo(string.Format("此次生成来自销售出库给销售公司，销售订单商品结算价已完成！统计日期 {0}，此次取的数据量为 {1}", statisticDate.ToString("yyyy-MM-dd"), orderList.Count), LOG_TAG, null);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("生成来自销售出库给销售公司，销售订单商品结算价失败！统计日期 {0}", statisticDate.ToString("yyyy-MM-dd")), LOG_TAG, ex);
            }
            Thread.Sleep(1000);

            try
            {
                var orderList = _saleOrderGoodsSettlementDal.GetUnsavedSaleStockOutForHostingFilialeList(statisticDate, maxRowCount);
                foreach (var orderInfo in orderList)
                {
                    var items = Create(orderInfo);
                    foreach (var item in items)
                    {
                        Save(item);
                    }
                }
                ERP.SAL.LogCenter.LogService.LogInfo(string.Format("此次生成来自销售出库给物流配送公司，销售订单商品结算价已完成！统计日期 {0}，此次取的数据量为 {1}", statisticDate.ToString("yyyy-MM-dd"), orderList.Count), LOG_TAG, null);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("生成来自销售出库给物流配送公司，销售订单商品结算价失败！统计日期 {0}", statisticDate.ToString("yyyy-MM-dd")), LOG_TAG, ex);
            }
        }

        /// <summary>
        /// 创建销售订单商品结算价
        /// </summary>
        /// <param name="orderInfo">关联的订单</param>
        /// <returns></returns>
        public IList<SaleOrderGoodsSettlementInfo> Create(SampleSaleOrderInfo orderInfo)
        {
            List<SaleOrderGoodsSettlementInfo> result = new List<SaleOrderGoodsSettlementInfo>();
            if(orderInfo == null || string.IsNullOrEmpty(orderInfo.TradeNo) || orderInfo.FilialeId == Guid.Empty || orderInfo.GoodsIds == null || orderInfo.GoodsIds.Count == 0)
            {
                return result;
            }
            var settlementList = _realTimeGrossSettlementDal.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(orderInfo.FilialeId, orderInfo.GoodsIds, orderInfo.OccurTime);
            result = orderInfo.GoodsIds.Select(m => new SaleOrderGoodsSettlementInfo
            {
                FilialeId = orderInfo.FilialeId,
                GoodsId = m,
                SettlementPrice = settlementList.ContainsKey(m) ? settlementList[m] : 0,
                RelatedTradeNo = orderInfo.TradeNo,
                RelatedTradeType = (int)orderInfo.TradeType,
                OccurTime = orderInfo.OccurTime,
                CreateTime = DateTime.Now
            }).ToList();
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="item"></param>
        public void Save(SaleOrderGoodsSettlementInfo item)
        {
            if (item == null) return;
            try
            {
                _saleOrderGoodsSettlementDal.Save(item);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("保存销售订单商品结算价失败！公司ID {0}，主商品ID {1}，关联单据号 {2}", item.FilialeId, item.GoodsId, item.RelatedTradeNo), LOG_TAG, ex);
            }
        }
    }
}
