using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Interface;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.DAL.Interface.IStorage;
using ERP.Enum;
using ERP.Enum.ASYN;
using ERP.Environment;
using ERP.Model;
using ERP.Model.ASYN;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using ERP.DAL.Implement.FinanceModule;

namespace ERP.BLL.Implement.Order
{
    public class OrderManager : BllInstance<OrderManager>
    {
        static IGoodsOrder _goodsOrderDal;
        static IGoodsOrderDetail _goodsOrderDetail;
        static IReckoning _reckoning;
        static IGoodsCenterSao _goodsCenterSao;
        static IASYNStorageRecordDao _asynStorageRecordDao;
        static IOperationLogManager _operationLogManager;
        static IPromotionSao _promotionSao;
        static IGoodsStockRecord _goodsStockSettleRecordBll;
        static IInvoice _invoice;
        static IRealTimeGrossSettlementDal _realTimeGrossSettlementDal;

        public OrderManager(GlobalConfig.DB.FromType fromType = GlobalConfig.DB.FromType.Write)
        {
            _operationLogManager = new OperationLogManager();
            _goodsOrderDal = new DAL.Implement.Order.GoodsOrder(fromType);
            _goodsOrderDetail = OrderInstance.GetGoodsOrderDetailDao(fromType);
            _goodsCenterSao = new GoodsCenterSao();
            _promotionSao = new PromotionSao();
            _reckoning = new Reckoning(fromType);
            _asynStorageRecordDao = new ASYNStorageRecordDao(fromType);
            _goodsStockSettleRecordBll = new GoodsStockRecordDao();
            _invoice = new Invoice(fromType);
            _realTimeGrossSettlementDal = new RealTimeGrossSettlementDal(fromType);
        }

        /// <summary> 加入到待完成订单列表里（适合批量完成订单操作）
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorId"></param>
        /// <param name="operatorName"></param>
        public bool JoinWaitConsignmentOrder(Guid operatorId, string operatorName, List<String> orderNos, out string errorMessage)
        {
            errorMessage = string.Empty;
            var orders = new List<GoodsOrderInfo>();
            foreach (var orderNo in orderNos)
            {
                var orderInfo = _goodsOrderDal.GetGoodsOrder(orderNo);
                if (orderInfo == null)
                {
                    errorMessage = ("无法完成操作,没找到订单信息！订单号:" + orderNo);

                    ERP.SAL.LogCenter.LogService.LogWarn(string.Format("订单未找到{0}", orderNo), "JoinWaitConsignmentOrder");
                    continue;
                }
                orders.Add(orderInfo);
            }
            if (orders.Count == 0) return true;
            using (var tran = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    foreach (var order in orders)
                    {
                        var isSuccess = _goodsOrderDal.SetGoodsOrderToConsignmented(order.OrderNo);
                        if (!isSuccess)
                        {
                            errorMessage = "更新订单状态到完成失败！";
                            return false;
                        }

                        var successInsertWait = _goodsOrderDal.InsertWaitConsignmentOrder(order.OrderId, (Int32)OrderState.Consignmented, "系统自动");
                        if (!successInsertWait)
                        {
                            errorMessage = "插入异步完成订单表数据失败！";
                            return false;
                        }
                    }
                    tran.Complete();
                    return true;
                }
                catch (Exception exp)
                {
                    errorMessage = "发生异常操作，异常消息：" + exp.Message + "\r\n" + exp.StackTrace;
                    throw exp;
                }
            }
        }

        /// <summary>
        /// 系统自动执行订单完成发货的最后数据操作（适合批量完成订单操作）
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="operatorName"></param>
        /// <param name="errorMessage"></param>
        public bool FinishConsignmentOrder(GoodsOrderInfo orderInfo, string operatorName, out string errorMessage)
        {
            if (orderInfo.OrderState != (int)OrderState.Consignmented)
            {
                errorMessage = "当前订单：" + orderInfo.OrderNo + "，状态：" + orderInfo.OrderState + "，无法完成！";
                return false;
            }
            using (var tran = new TransactionScope(TransactionScopeOption.Required))
            {
                errorMessage = String.Empty;
                #region -- 插入出入库待执行异步表
                var asyncStorageInfo = new ASYNStorageRecordInfo
                {
                    CreateTime = DateTime.Now,
                    ID = Guid.NewGuid(),
                    IdentifyId = orderInfo.OrderId,
                    IdentifyKey = orderInfo.OrderNo,
                    IsValidateStock = false,
                    StorageState = (int)StorageRecordState.Finished,
                    StorageType = (int)StorageRecordType.SellStockOut,
                    WarehouseId = orderInfo.DeliverWarehouseId
                };
                var successStorage = _asynStorageRecordDao.Insert(asyncStorageInfo);
                if (!successStorage)
                {
                    errorMessage = "添加出入库待执行异步表失败";
                    return false;
                }
                #endregion

                #region -- 插入往来帐待执行异步表
                var asynReckoningInfo = new ASYNReckoningInfo
                {
                    CreateTime = DateTime.Now,
                    ID = Guid.NewGuid(),
                    IdentifyId = orderInfo.OrderId,
                    IdentifyKey = orderInfo.OrderNo,
                    ReckoningFromType = ASYNReckoningFromType.CompleteOrder.ToString()
                };
                var successReckoning = _reckoning.InsertAsyn(asynReckoningInfo);
                if (!successReckoning)
                {
                    errorMessage = ("添加往来帐待执行异步表失败！");
                    return false;
                }
                #endregion

                //删除待完成订单列表数据
                var successDelete = _goodsOrderDal.DeleteWaitConsignmentOrder(orderInfo.OrderId);
                if (!successDelete)
                {
                    errorMessage = ("删除待完成订单列表数据发生错误！");
                    return false;
                }

                tran.Complete();
                return true;
            }
        }

        /// <summary>作废订单
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="operatorName"></param>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        public Boolean CancelOrder(Guid operatorId, string operatorName, List<String> orderNos)
        {
            using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(3)))
            {
                foreach (var orderNo in orderNos)
                {
                    GoodsOrderInfo orderInfo = null;
                    IList<GoodsOrderDetailInfo> orderDetails = null;
                    InvoiceInfo invoiceinfo = null;
                    using (var tss = new TransactionScope(TransactionScopeOption.Suppress, TimeSpan.FromMinutes(3)))
                    {
                        orderInfo = _goodsOrderDal.GetGoodsOrder(orderNo);
                        if (orderInfo == null) continue;
                        orderDetails = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(orderInfo.OrderId);
                        invoiceinfo = _invoice.GetInvoiceByGoodsOrder(orderInfo.OrderId);
                    }
                    var result = _goodsOrderDal.SetGoodsOrderToCancellation(orderNo);
                    if (invoiceinfo != null && invoiceinfo.InvoiceId != Guid.Empty)
                    {
                        //已开发票，发票状态更改为作废申请
                        if (invoiceinfo.InvoiceState == (int)InvoiceState.Success)
                        {
                            result = _invoice.UpdateInvoiceStateByinvoiceId(orderInfo.OrderId, InvoiceState.WasteRequest);
                        }
                        //申请发票，发票状态更改为取消
                        else if (invoiceinfo.InvoiceState == (int)InvoiceState.Request)
                        {
                            result = _invoice.UpdateInvoiceStateByinvoiceId(orderInfo.OrderId, InvoiceState.Cancel);
                        }
                    }
                    if (!result) return false;
                }
                ts.Complete();
                return true;
            }
        }

        /// <summary>添加日销售量
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="detailList"></param>
        /// <param name="dicAvgSettlePrice"></param>
        public void SaveGoodsDaySalesStatistics(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> detailList, IDictionary<Guid, decimal> dicAvgSettlePrice)
        {
            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, detailList.Select(ent => ent.GoodsType).Distinct());
            }
            var goodsSeriesList = _goodsCenterSao.GetGoodsSeriesList(orderInfo.SaleFilialeId, detailList.Select(w => w.GoodsID).Distinct().ToList());
            //根据订单Id获取实际商品价格(参加促销活动的商品要抵扣相应的促销价，从而得到新的实际商品价格)
            Dictionary<Guid, decimal> dicGoodsPriceDict = _promotionSao.GetGoodsPriceDict(orderInfo.OrderId);

            var list = new List<GoodsDaySalesStatisticsInfo>();
            foreach (var goodsOrderDetailInfo in detailList)
            {
                var gdsinfo = new GoodsDaySalesStatisticsInfo
                {
                    DeliverWarehouseId = orderInfo.DeliverWarehouseId,
                    SaleFilialeId = orderInfo.SaleFilialeId,
                    SalePlatformId = orderInfo.SalePlatformId,
                    GoodsId = goodsOrderDetailInfo.GoodsID,
                    RealGoodsId = goodsOrderDetailInfo.RealGoodsID,
                    GoodsSales = goodsOrderDetailInfo.Quantity,
                    DayTime = orderInfo.OrderTime,
                    Specification = goodsOrderDetailInfo.PurchaseSpecification,
                    SellPrice = ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)goodsOrderDetailInfo.Quantity,
                    GoodsName = goodsOrderDetailInfo.GoodsName,
                    GoodsCode = goodsOrderDetailInfo.GoodsCode,
                    ClassId = Guid.Empty,
                    AvgSettlePrice = ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)goodsOrderDetailInfo.Quantity,
                    HostingFilialeId = orderInfo.HostingFilialeId
                };
                if (goodsSeriesList != null && goodsSeriesList.Count > 0)
                {
                    var result = goodsSeriesList.FirstOrDefault(p => p.GoodsID.Equals(goodsOrderDetailInfo.GoodsID));
                    if (result != null)
                    {
                        gdsinfo.GoodsName = result.GoodsName;
                        gdsinfo.GoodsCode = result.GoodsCode;
                        gdsinfo.ClassId = result.ClassID;
                        gdsinfo.BrandId = result.BrandId;
                        gdsinfo.SeriesId = result.SeriesId;
                    }
                }
                list.Add(gdsinfo);
            }
            _goodsOrderDetail.SaveGoodsSales(list);
        }

        /// <summary>修改日销量
        /// </summary>
        /// <param name="orderInfo">订单</param>
        /// <param name="goodsOrderDetailInfos"></param>
        /// <param name="isUpdate">false、作废，true、修改</param>
        ///<param name="oldDetailInfos"></param>
        /// <param name="dicAvgSettlePrice"></param>
        public void UpdateGoodsDaySalesStatistics(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> goodsOrderDetailInfos, bool isUpdate, IList<GoodsOrderDetailInfo> oldDetailInfos, IDictionary<Guid, decimal> dicAvgSettlePrice)
        {
            var list = new List<GoodsDaySalesStatisticsInfo>();
            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, goodsOrderDetailInfos.Select(ent => ent.GoodsType).Distinct());
            }
            var goodsSeriesList = _goodsCenterSao.GetGoodsSeriesList(orderInfo.SaleFilialeId, goodsOrderDetailInfos.Select(w => w.GoodsID).Distinct().ToList());
            //根据订单Id获取实际商品价格(参加促销活动的商品要抵扣相应的促销价，从而得到新的实际商品价格)
            Dictionary<Guid, decimal> dicGoodsPriceDict = _promotionSao.GetGoodsPriceDict(orderInfo.OrderId);
            #region 作废
            if (!isUpdate)  //作废
            {
                foreach (var goodsOrderDetailInfo in goodsOrderDetailInfos)
                {
                    var goodsDaySalesStatisticsInfo = new GoodsDaySalesStatisticsInfo
                    {
                        DeliverWarehouseId = orderInfo.DeliverWarehouseId,
                        SaleFilialeId = orderInfo.SaleFilialeId,
                        SalePlatformId = orderInfo.SalePlatformId,
                        GoodsId = goodsOrderDetailInfo.GoodsID,
                        RealGoodsId = goodsOrderDetailInfo.RealGoodsID,
                        GoodsSales = -goodsOrderDetailInfo.Quantity,
                        DayTime = orderInfo.OrderTime,
                        Specification = goodsOrderDetailInfo.PurchaseSpecification,
                        SellPrice = ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)-goodsOrderDetailInfo.Quantity,
                        GoodsName = goodsOrderDetailInfo.GoodsName,
                        GoodsCode = goodsOrderDetailInfo.GoodsCode,
                        HostingFilialeId = orderInfo.HostingFilialeId,
                        AvgSettlePrice = ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)-goodsOrderDetailInfo.Quantity
                    };
                    if (goodsSeriesList != null && goodsSeriesList.Count > 0)
                    {
                        var result = goodsSeriesList.FirstOrDefault(p => p.GoodsID.Equals(goodsOrderDetailInfo.GoodsID));
                        if (result != null)
                        {
                            goodsDaySalesStatisticsInfo.GoodsName = result.GoodsName;
                            goodsDaySalesStatisticsInfo.GoodsCode = result.GoodsCode;
                            goodsDaySalesStatisticsInfo.ClassId = result.ClassID;
                            goodsDaySalesStatisticsInfo.BrandId = result.BrandId;
                            goodsDaySalesStatisticsInfo.SeriesId = result.SeriesId;
                        }
                    }
                    list.Add(goodsDaySalesStatisticsInfo);
                }
            }
            #endregion
            #region [修改]
            else  //修改
            {
                if (orderInfo.OrderState == (int)OrderState.StockUp)
                {
                    var oldGoodsDaySalesStatistics = new List<GoodsDaySalesStatisticsInfo>();  //修改前订单销量明细
                    var newGoodsDaySalesStatistics = new List<GoodsDaySalesStatisticsInfo>();  //修改后的订单销量明细
                    foreach (var goodsOrderDetailInfo in oldDetailInfos)  //原始订单明细
                    {
                        var goodsDaySalesInfo = oldGoodsDaySalesStatistics.FirstOrDefault(act => act.RealGoodsId == goodsOrderDetailInfo.RealGoodsID);
                        if (goodsDaySalesInfo != null)
                        {
                            goodsDaySalesInfo.GoodsSales += goodsOrderDetailInfo.Quantity;
                            goodsDaySalesInfo.SellPrice += ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)goodsOrderDetailInfo.Quantity;
                            goodsDaySalesInfo.AvgSettlePrice += ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)goodsOrderDetailInfo.Quantity;
                        }
                        else
                        {
                            var gdsinfo = new GoodsDaySalesStatisticsInfo
                            {
                                DeliverWarehouseId = orderInfo.DeliverWarehouseId,
                                SaleFilialeId = orderInfo.SaleFilialeId,
                                SalePlatformId = orderInfo.SalePlatformId,
                                GoodsId = goodsOrderDetailInfo.GoodsID,
                                RealGoodsId = goodsOrderDetailInfo.RealGoodsID,
                                GoodsSales = goodsOrderDetailInfo.Quantity,
                                DayTime = orderInfo.OrderTime,
                                Specification = goodsOrderDetailInfo.PurchaseSpecification,
                                SellPrice = ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)goodsOrderDetailInfo.Quantity,
                                GoodsName = goodsOrderDetailInfo.GoodsName,
                                GoodsCode = goodsOrderDetailInfo.GoodsCode,
                                ClassId = Guid.Empty,
                                HostingFilialeId = orderInfo.HostingFilialeId,
                                AvgSettlePrice = ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)goodsOrderDetailInfo.Quantity
                            };
                            oldGoodsDaySalesStatistics.Add(gdsinfo);
                        }
                    }
                    foreach (var goodsOrderDetailInfo in goodsOrderDetailInfos)  //当前销量
                    {
                        var goodsDaySalesInfo = newGoodsDaySalesStatistics.FirstOrDefault(act => act.RealGoodsId == goodsOrderDetailInfo.RealGoodsID);
                        if (goodsDaySalesInfo != null)
                        {
                            goodsDaySalesInfo.GoodsSales += goodsOrderDetailInfo.Quantity;
                            goodsDaySalesInfo.SellPrice += ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)goodsOrderDetailInfo.Quantity;
                            goodsDaySalesInfo.AvgSettlePrice += ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)goodsOrderDetailInfo.Quantity;
                        }
                        else
                        {
                            goodsDaySalesInfo = new GoodsDaySalesStatisticsInfo
                            {
                                DeliverWarehouseId = orderInfo.DeliverWarehouseId,
                                SaleFilialeId = orderInfo.SaleFilialeId,
                                SalePlatformId = orderInfo.SalePlatformId,
                                GoodsId = goodsOrderDetailInfo.GoodsID,
                                RealGoodsId = goodsOrderDetailInfo.RealGoodsID,
                                GoodsSales = goodsOrderDetailInfo.Quantity,
                                DayTime = orderInfo.OrderTime,
                                Specification = goodsOrderDetailInfo.PurchaseSpecification,
                                SellPrice = ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicGoodsPriceDict[goodsOrderDetailInfo.GoodsID] : goodsOrderDetailInfo.SellPrice) * (decimal)goodsOrderDetailInfo.Quantity,
                                GoodsName = goodsOrderDetailInfo.GoodsName,
                                GoodsCode = goodsOrderDetailInfo.GoodsCode,
                                ClassId = Guid.Empty,
                                HostingFilialeId = orderInfo.HostingFilialeId,
                                AvgSettlePrice = ((dicAvgSettlePrice != null && dicAvgSettlePrice.Any() && dicAvgSettlePrice.ContainsKey(goodsOrderDetailInfo.GoodsID)) ? dicAvgSettlePrice[goodsOrderDetailInfo.GoodsID] : 0) * (decimal)goodsOrderDetailInfo.Quantity
                            };
                            newGoodsDaySalesStatistics.Add(goodsDaySalesInfo);
                        }
                    }
                    //修改后删除的商品
                    foreach (var goodsDaySalesStatisticsInfo in oldGoodsDaySalesStatistics)
                    {
                        var info = newGoodsDaySalesStatistics.All(act => act.RealGoodsId != goodsDaySalesStatisticsInfo.RealGoodsId);
                        if (info)
                        {
                            goodsDaySalesStatisticsInfo.GoodsSales = -goodsDaySalesStatisticsInfo.GoodsSales;
                            goodsDaySalesStatisticsInfo.SellPrice = -goodsDaySalesStatisticsInfo.SellPrice;
                            goodsDaySalesStatisticsInfo.AvgSettlePrice = -goodsDaySalesStatisticsInfo.AvgSettlePrice;
                            //_goodsOrderDetail.SaveGoodsSales(goodsDaySalesStatisticsInfo);
                            list.Add(goodsDaySalesStatisticsInfo);
                            //newGoodsDaySalesStatistics.Add(goodsDaySalesStatisticsInfo);
                        }
                    }

                    foreach (var goodsDaySalesStatisticsInfo in newGoodsDaySalesStatistics)
                    {
                        var info = oldGoodsDaySalesStatistics.FirstOrDefault(act => act.RealGoodsId == goodsDaySalesStatisticsInfo.RealGoodsId);
                        if (info != null)
                        {
                            goodsDaySalesStatisticsInfo.GoodsSales -= info.GoodsSales;
                            goodsDaySalesStatisticsInfo.SellPrice -= info.SellPrice;
                            goodsDaySalesStatisticsInfo.AvgSettlePrice -= info.AvgSettlePrice;
                        }
                        if (Math.Abs(goodsDaySalesStatisticsInfo.GoodsSales) > 0 || Math.Abs(goodsDaySalesStatisticsInfo.SellPrice) > 0)
                        {
                            if (oldGoodsDaySalesStatistics.All(act => act.RealGoodsId != goodsDaySalesStatisticsInfo.RealGoodsId))
                            {
                                if (goodsSeriesList != null && goodsSeriesList.Count > 0)
                                {
                                    var result = goodsSeriesList.FirstOrDefault(p => p.GoodsID.Equals(goodsDaySalesStatisticsInfo.GoodsId));
                                    if (result != null)
                                    {
                                        goodsDaySalesStatisticsInfo.GoodsName = result.GoodsName;
                                        goodsDaySalesStatisticsInfo.GoodsCode = result.GoodsCode;
                                        goodsDaySalesStatisticsInfo.ClassId = result.ClassID;
                                        goodsDaySalesStatisticsInfo.BrandId = result.BrandId;
                                        goodsDaySalesStatisticsInfo.SeriesId = result.SeriesId;
                                    }
                                }
                            }
                            //_goodsOrderDetail.SaveGoodsSales(goodsDaySalesStatisticsInfo);
                            list.Add(goodsDaySalesStatisticsInfo);
                        }
                    }
                }
            }
            #endregion
            _goodsOrderDetail.SaveGoodsSales(list);
        }

        /// <summary> 售后更新日销量
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="afterSaleDetailList"></param>
        /// zal 2016-03-04
        /// 售后的商品在日销量表中单独插入一条负销量的数据,且插入销量的日期是退换货当天(需求1883)
        public void AfterSaleUpdateGoodsDaySalesStatistics(Guid orderId, IList<AfterSaleDetailInfo> afterSaleDetailList)
        {
            return;
            var list = new List<GoodsDaySalesStatisticsInfo>();
            var goodsOrderInfo = _goodsOrderDal.GetGoodsOrder(orderId);
            if (goodsOrderInfo != null && goodsOrderInfo.OrderId != Guid.Empty)
            {
                var goodsOrderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(orderId);
                if (goodsOrderInfo.HostingFilialeId == Guid.Empty)
                {
                    goodsOrderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(goodsOrderInfo.DeliverWarehouseId, goodsOrderInfo.SaleFilialeId, goodsOrderDetailList.Select(ent => ent.GoodsType).Distinct());
                }
                #region 售后商品的价格，不在使用B2C传过来的价格，改从ERP这边取(原因：当商品做二次售后时，价格应该是0，而B2C每次传过来的价格都是原单的价格) zal 2016-03-18
                foreach (var item in afterSaleDetailList)
                {
                    var goodsOrderDetailInfo = goodsOrderDetailList.FirstOrDefault(p => p.RealGoodsID.Equals(item.RealGoodsId));
                    if (goodsOrderDetailInfo != null)
                    {
                        item.SellPrice = goodsOrderDetailInfo.SellPrice;
                    }
                }
                #endregion

                var goodsSeriesList = _goodsCenterSao.GetGoodsSeriesList(goodsOrderInfo.SaleFilialeId, afterSaleDetailList.Select(w => w.GoodsId).Distinct().ToList());
                //根据订单Id获取实际商品价格(参加促销活动的商品要抵扣相应的促销价，从而得到新的实际商品价格)
                Dictionary<Guid, decimal> dicGoodsPriceDict = _promotionSao.GetGoodsPriceDict(orderId);

                //获取指定时间下某公司的某商品的最近结算价
                var dicFilialeIdAndGoodsIdAvgSettlePrice = _realTimeGrossSettlementDal.GetFilialeIdGoodsIdAvgSettlePrice(goodsOrderInfo.OrderTime);

                foreach (var afterSaleDetailInfo in afterSaleDetailList)
                {
                    if (afterSaleDetailInfo.AgainCount == 0) continue;
                    var gdsinfo = new GoodsDaySalesStatisticsInfo
                    {
                        DeliverWarehouseId = goodsOrderInfo.DeliverWarehouseId,
                        SaleFilialeId = goodsOrderInfo.SaleFilialeId,
                        SalePlatformId = goodsOrderInfo.SalePlatformId,
                        GoodsId = afterSaleDetailInfo.GoodsId,
                        RealGoodsId = afterSaleDetailInfo.RealGoodsId,
                        GoodsSales = -afterSaleDetailInfo.AgainCount,
                        DayTime = DateTime.Now,
                        Specification = afterSaleDetailInfo.Specification,
                        SellPrice = ((dicGoodsPriceDict != null && dicGoodsPriceDict.ContainsKey(afterSaleDetailInfo.GoodsId)) ? dicGoodsPriceDict[afterSaleDetailInfo.GoodsId] : afterSaleDetailInfo.SellPrice) * (-afterSaleDetailInfo.AgainCount),
                        GoodsName = afterSaleDetailInfo.GoodsName,
                        GoodsCode = afterSaleDetailInfo.GoodsCode,
                        ClassId = Guid.Empty,
                        HostingFilialeId = goodsOrderInfo.HostingFilialeId,
                        AvgSettlePrice = ((dicFilialeIdAndGoodsIdAvgSettlePrice != null && dicFilialeIdAndGoodsIdAvgSettlePrice.Any() && dicFilialeIdAndGoodsIdAvgSettlePrice.ContainsKey(goodsOrderInfo.SaleFilialeId)) ? (dicFilialeIdAndGoodsIdAvgSettlePrice[goodsOrderInfo.SaleFilialeId].ContainsKey(afterSaleDetailInfo.GoodsId) ? dicFilialeIdAndGoodsIdAvgSettlePrice[goodsOrderInfo.SaleFilialeId][afterSaleDetailInfo.GoodsId] : 0) : 0) * (-afterSaleDetailInfo.AgainCount)
                    };
                    if (goodsSeriesList != null && goodsSeriesList.Count > 0)
                    {
                        var result = goodsSeriesList.FirstOrDefault(p => p.GoodsID.Equals(afterSaleDetailInfo.GoodsId));
                        if (result != null)
                        {
                            gdsinfo.ClassId = result.ClassID;
                            gdsinfo.GoodsName = result.GoodsName;
                            gdsinfo.GoodsCode = result.GoodsCode;
                            gdsinfo.BrandId = result.BrandId;
                            gdsinfo.SeriesId = result.SeriesId;
                        }
                    }
                    list.Add(gdsinfo);
                }
            }
            _goodsOrderDetail.SaveGoodsSales(list, true);
        }
    }
}
