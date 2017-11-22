using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(Environment.GlobalConfig.DB.FromType.Write);

        /// <summary>订单导入
        /// </summary>
        /// <param name="pushDataId">推送数据ID</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="orderDetailList">订单明细信息</param>
        /// <param name="invoiceInfo">发票信息</param>
        /// <returns></returns>
        public WCFReturnInfo AddOrderAndInvoice(Guid pushDataId, GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> orderDetailList, InvoiceInfo invoiceInfo)
        {
            if (pushDataId == Guid.Empty)
            {
                SAL.LogCenter.LogService.LogError("方法：AddOrderAndInvoice，推送数据ID不能为空!", "ERP.Service.AddOrderAndInvoice", null);
                return new WCFReturnInfo(false, false, pushDataId, null, "方法：AddOrderAndInvoice，推送数据ID不能为空!");
            }
            lock (this)
            {
                //如果commandid存在，且已执行成功，则直接返回true，表示该命令已经被执行；否则操作
                if (PUSH.Core.Instance.ExistExecuted(pushDataId))
                {
                    return new WCFReturnInfo(true, true, pushDataId, null, "服务验证方法已经执行!");
                }
            }
            string errorMsg;
            try
            {
                if (orderDetailList != null && orderDetailList.Count > 0)
                {
                    var success = _goodsOrder.AddOrderAndInvoice(orderInfo, orderDetailList, invoiceInfo, out errorMsg);
                    if (!success)
                    {
                        SAL.LogCenter.LogService.LogError(string.Format("订单号 {0} 处理失败：{1}", orderInfo.OrderNo, errorMsg), "ERP.Service.AddOrderAndInvoice", null);
                        return new WCFReturnInfo(true, false, pushDataId, false, errorMsg);
                    }

                    PUSH.Core.Instance.AddExecuted(pushDataId);
                    return new WCFReturnInfo(true, true, pushDataId, true, "操作成功");
                }
                return new WCFReturnInfo(true, false, pushDataId, false, "订单详细中，没有下单商品信息，无法添加订单");
            }
            catch (Exception exp)
            {
                errorMsg = exp.Message;
                if (exp.InnerException != null)
                {
                    errorMsg += @"\r\n" + exp.InnerException.Message;
                }
                SAL.LogCenter.LogService.LogError(errorMsg, "ERP.Service.AddOrderAndInvoice", exp);
                return new WCFReturnInfo(false, false, pushDataId, null, errorMsg);
            }
        }

        /// <summary> 根据年份和指定条件查询订单
        /// </summary>
        /// <returns></returns>
        public DataListInfo<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, string orderNo, string consignee, string expressNo, string mobile, Guid memberId, int startPage, int pageSize)
        {
            try
            {
                long recordCount;
                IList<GoodsOrderInfo> list = _goodsOrderWrite.GetGoodsOrder(salePlatformId, startTime, endTime, Guid.Empty, orderNo, consignee, expressNo, mobile, memberId, Environment.GlobalConfig.KeepYear, startPage, pageSize, out recordCount);
                return new DataListInfo<GoodsOrderInfo>
                {
                    AllRecordCount = (int)recordCount,
                    Items = list
                };
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(ex.Message, "ERP.Service.GetGoodsOrder", ex);
                return new DataListInfo<GoodsOrderInfo>
                {
                    AllRecordCount = 0,
                    Items = new List<GoodsOrderInfo>()
                };
            }
        }

        /// <summary>插入发票数据
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo InsertGoodsOrderInvoice(Guid pushDataId, InvoiceInfo invoice, Guid[] orderIds)
        {
            return Execute(pushDataId, () =>
            {
                try
                {
                    if (invoice.InvoiceId != Guid.Empty && string.IsNullOrEmpty(invoice.InvoiceName))
                    {
                        _invoiceDao.SetInvoiceState(invoice.InvoiceId, InvoiceState.Waste, "");
                    }
                    else
                    {
                        Guid warehouseId;
                        var orderNoDic = _goodsOrderWrite.GetOrderNoDicAndWarehouseIdsByOrderIds(orderIds.ToList(), out warehouseId);
                        invoice.DeliverWarehouseId = warehouseId;
                        if (orderNoDic.Count > 0)
                        {
                            _invoiceDao.Insert(invoice, orderNoDic);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAL.LogCenter.LogService.LogError(ex.Message, "ERP.Service.InsertGoodsOrderInvoice", ex);
                }
            });
        }

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        public Boolean SetGoodsOrderToRedeploy(List<String> orderNos)
        {
            if (orderNos.Count == 0)
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderToRedeploy", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderToRedeploy(orderNos);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToRedeploy", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToRedeploy", ex);
                return false;
            }
        }

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        public Boolean SetGoodsOrderToRedeployNew(List<String> orderNos,Guid warehouseId, byte storageType, Guid hostingFilialeId,Guid expressId)
        {
            if (orderNos.Count == 0)
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderToRedeployNew", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderToRedeployNew(orderNos,warehouseId,storageType,hostingFilialeId,expressId);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToRedeployNew", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToRedeployNew", ex);
                return false;
            }
        }

        /// <summary>更新订单状态为需采购
        /// </summary>
        /// <returns></returns>
        public Boolean SetGoodsOrderToPurchase(List<String> orderNos)
        {
            if (orderNos.Count == 0)
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderToPurchase", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderToPurchase(orderNos);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToPurchase", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToPurchase", ex);
                return false;
            }
        }

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        public bool SetGoodsOrderStateToWaitOutbound(List<string> orderNos)
        {
            if (orderNos.Count == 0)
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderStateToWaitOutbound", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderStateToWaitOutbound(orderNos);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderStateToWaitOutbound", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderStateToWaitOutbound", ex);
                return false;
            }
        }

        public bool SetGoodsOrderStateToWaitOutboundNew(List<string> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId)
        {
            if (orderNos.Count == 0)
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderStateToWaitOutboundNew", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderStateToWaitOutboundNew(orderNos, warehouseId, storageType, hostingFilialeId, expressId);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderStateToWaitOutboundNew", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderStateToWaitOutboundNew", ex);
                return false;
            }
        }

        /// <summary>设置订单快递号
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        public bool SetGoodsOrderExpressNo(List<string> orderNos, string expressNo)
        {
            if (orderNos.Count == 0 || String.IsNullOrWhiteSpace(expressNo))
            {
                SAL.LogCenter.LogService.LogError("无传入的订单列表 或 快递单号", "ERP.Service.SetGoodsOrderExpressNo", null);
                return false;
            }
            try
            {
                var result = _goodsOrderWrite.SetGoodsOrderExpressNo(orderNos, expressNo);
                if (!result)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}，快递号：{1}", string.Join(",", orderNos), expressNo), "ERP.Service.SetGoodsOrderExpressNo", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单列表：{1}，快递单号 {2}", ex.Message, string.Join(",", orderNos), expressNo), "ERP.Service.SetGoodsOrderStateToWaitOutbound", ex);
                return false;
            }
        }

        /// <summary>更新订单状态为完成
        /// </summary>
        /// <returns></returns>
        public Boolean SetGoodsOrderToConsignmented(Guid operatorId, string operatorName, List<String> orderNos)
        {
            if (orderNos.Count == 0)
            {
                ERP.SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderToConsignmented", null);
                return false;
            }
            string errorMessage;
            try
            {
                var result = _orderManager.JoinWaitConsignmentOrder(operatorId, operatorName, orderNos, out errorMessage);
                if (!result)
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，订单列表：{1}", errorMessage, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToConsignmented", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToConsignmented", ex);
                return false;
            }
        }

        /// <summary>更新订单状态为作废
        /// </summary>
        /// <returns></returns>
        public Boolean SetGoodsOrderToCancellation(Guid operatorId, string operatorName, List<String> orderNos)
        {
            if (orderNos.Count == 0)
            {
                ERP.SAL.LogCenter.LogService.LogError("无传入的订单列表", "ERP.Service.SetGoodsOrderToCancellation", null);
                return false;
            }
            try
            {
                var result = _orderManager.CancelOrder(operatorId, operatorName, orderNos);
                if (!result)
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("更新失败，订单列表：{0}", string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToCancellation", null);
                }
                return result;
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，订单列表：{1}", ex.Message, string.Join(",", orderNos)), "ERP.Service.SetGoodsOrderToCancellation", ex);
                return false;
            }
        }

        /// <summary>获取商品备货信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, String> GetGoodsIdsIsStockUp(Guid warehouseId, List<Guid> goodsIds)
        {
            var goodsStockUpDic = new Dictionary<Guid, String>();
            if (goodsIds.Count > 0 && warehouseId != default(Guid))
            {
                try
                {
                    var goodsPurchaseSetDic = _purchaseSet.GetGoodsIsReadyByWarehouseId(warehouseId,goodsIds);

                    foreach (var goodsId in goodsIds.Distinct())
                    {
                        goodsStockUpDic.Add(goodsId, goodsPurchaseSetDic.ContainsKey(goodsId) ? "备货" : "每日进货");
                    }
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，商品列表：{1}，仓库：{2}", ex.Message, string.Join(",", goodsIds), warehouseId), "ERP.Service.GetGoodsIdsIsStockUp", ex);
                }
            }
            return goodsStockUpDic;
        }

        /// <summary>获取会员历史订单记录
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime)
        {
            if (memberId == default(Guid) || orderTime == default(DateTime))
            {
                return new List<GoodsOrderInfo>();
            }
            try
            {
                return _goodsOrderWrite.GetHistoryGoodsOrderByMemberIdAndOrderTime(memberId, orderTime);
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，会员ID：{1}，订单时间：{2}", ex.Message, memberId, orderTime), "ERP.Service.GetHistoryGoodsOrderByMemberIdAndOrderTime", ex);
                return new List<GoodsOrderInfo>();
            }
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime)
        {
            if (orderId == default(Guid) || orderTime == default(DateTime))
            {
                return null;
            }
            return _goodsOrderWrite.GetHistoryOrderInfoByOrderId(orderId, orderTime);
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime)
        {
            if (String.IsNullOrWhiteSpace(orderNo) || orderTime == default(DateTime))
            {
                return null;
            }
            return _goodsOrderWrite.GetHistoryOrderInfoByOrderNo(orderNo, orderTime);
        }

        /// <summary>获取历史订单明细信息
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderDetailInfo> GetHistoryGoodsOrderDetailListByOrderId(Guid orderId, DateTime orderTime)
        {
            if (orderId == default(Guid) || orderTime == default(DateTime))
            {
                return new List<GoodsOrderDetailInfo>();
            }
            return _goodsOrderDetailWrite.GetGoodsOrderDetailList(orderId, orderTime);
        }

        /// <summary>申请发票作废
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceStateToWasteRequest(Guid invoiceId, string cancelPersonel)
        {
            if (invoiceId == default(Guid) || String.IsNullOrWhiteSpace(cancelPersonel))
            {
                return false;
            }
            return _invoiceDao.SetInvoiceState(invoiceId, InvoiceState.WasteRequest, cancelPersonel);
        }

        /// <summary>
        /// 批量申请发票作废
        /// </summary>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        public bool BatchSetInvoiceStateToWasteRequest(List<Guid> invoiceIdList, string cancelPersonel)
        {
            if (invoiceIdList.Count == 0 || string.IsNullOrWhiteSpace(cancelPersonel))
            {
                return false;
            }
            return _invoiceDao.BatchSetInvoiceState(invoiceIdList, InvoiceState.WasteRequest, cancelPersonel);
        }

        /// <summary>取消发票
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceStateToCancel(Guid invoiceId, string cancelPersonel)
        {
            if (invoiceId == default(Guid) || String.IsNullOrWhiteSpace(cancelPersonel))
            {
                return false;
            }
            return _invoiceDao.SetInvoiceState(invoiceId, InvoiceState.Cancel, cancelPersonel);
        }

        /// <summary>
        /// 批量取消发票
        /// </summary>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        public bool BatchSetInvoiceStateToCancel(List<Guid> invoiceIdList, string cancelPersonel)
        {
            if (invoiceIdList.Count == 0 || string.IsNullOrWhiteSpace(cancelPersonel))
            {
                return false;
            }
            return _invoiceDao.BatchSetInvoiceState(invoiceIdList, InvoiceState.Cancel, cancelPersonel);
        }

        /// <summary>更新发票抬头和发票内容
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceNameAndInvoiceContent(Guid invoiceId, string invoiceName, string invoiceContent)
        {
            if (invoiceId == default(Guid) || String.IsNullOrWhiteSpace(invoiceName) || String.IsNullOrWhiteSpace(invoiceContent))
            {
                return false;
            }
            return _invoiceDao.SetInvoiceNameAndInvoiceContent(invoiceId, invoiceName, invoiceContent);
        }

        /// <summary>通过订单ID获取发票号码和发票是否报税        (Key:发票号码，Value:是否报税)
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<long, bool> GetInvoiceNoAndIsCommitByOrderId(Guid orderId)
        {
            if (orderId == default(Guid))
            {
                return new KeyValuePair<long, bool>(default(long), default(Boolean));
            }
            return _invoiceDao.GetInvoiceNoAndIsCommitByOrderId(orderId);
        }


        #region [到款通知]

        /// <summary> 插入一条到款通知
        /// </summary>
        /// <param name="pnInfo"></param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        public WCFReturnInfo PayNoticeInsert(Guid pushDataId, PaymentNoticeInfo pnInfo)
        {
            return Execute(pushDataId, () => _paymentNotice.Insert(pnInfo));
        }

        /// <summary>是否存在待确认的到款通知
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public bool IsExistByOrderNoAndPaystate(string orderNo)
        {
            return _paymentNotice.IsExistByOrderNoAndPaystate(orderNo);
        }

        /// <summary>获取到款通知
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IList<PaymentNoticeInfo> GetPayNoticeInfoListByOrderNo(string orderNo)
        {
            return _paymentNotice.GetListByOrderNo(orderNo);
        }

        /// <summary>更新到款通知
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="pninfo"></param>
        /// <returns></returns>
        public WCFReturnInfo UpdatePayNoticeInfo(Guid pushDataId, PaymentNoticeInfo pninfo)
        {
            return Execute(pushDataId, () => _paymentNotice.UpdatePayNoticeInfo(pninfo));
        }

        /// <summary>删除到款通知 
        /// </summary>
        /// <param name="payid"></param>
        /// <returns></returns>
        public WCFReturnInfo DeletePayNoticeByPayId(Guid payid)
        {
            return Execute(() => _paymentNotice.DeletePayNoticeByPayId(payid));
        }


        #endregion
    }
}
