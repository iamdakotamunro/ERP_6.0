using System;
using System.Collections.Generic;
using System.ServiceModel;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;

namespace ERP.Service.Contract
{
    public partial interface IService
    {
        /// <summary>订单导入
        /// </summary>
        /// <param name="pushDataId">推送数据ID</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="orderDetailList">订单明细信息</param>
        /// <param name="invoiceInfo">发票信息</param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo AddOrderAndInvoice(Guid pushDataId, GoodsOrderInfo orderInfo,
            IList<GoodsOrderDetailInfo> orderDetailList, InvoiceInfo invoiceInfo);

        /// <summary>根据年份和指定条件查询订单
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataListInfo<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime,
            string orderNo, string consignee, string expressNo, string mobile, Guid memberId, int startPage, int pageSize);


        /// <summary>插入发票数据
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoice">发票模型</param>
        /// <param name="orderIds">订单详细</param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo InsertGoodsOrderInvoice(Guid pushDataId, InvoiceInfo invoice, Guid[] orderIds);

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderToRedeploy(List<String> orderNos);

        /// <summary>
        /// 订单转移发货仓更新到需调拨
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderToRedeployNew(List<String> orderNos,Guid warehouseId,byte storageType,Guid hostingFilialeId,Guid expressId);

        /// <summary>更新订单状态为需采购
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderToPurchase(List<String> orderNos);

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderStateToWaitOutbound(List<String> orderNos);

        /// <summary>
        /// 订单转移发货仓更新到出货中
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderStateToWaitOutboundNew(List<String> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId);

        /// <summary>设置订单快递号
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderExpressNo(List<String> orderNos, String expressNo);

        /// <summary>更新订单状态为完成
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderToConsignmented(Guid operatorId, string operatorName, List<String> orderNos);

        /// <summary>更新订单状态为作废
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetGoodsOrderToCancellation(Guid operatorId, string operatorName, List<String> orderNos);

        /// <summary>获取商品备货信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<Guid, String> GetGoodsIdsIsStockUp(Guid warehouseId, List<Guid> goodsIds);

        /// <summary>获取会员历史订单记录
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime);

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime);

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime);

        /// <summary>获取历史订单明细信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<GoodsOrderDetailInfo> GetHistoryGoodsOrderDetailListByOrderId(Guid orderId, DateTime orderTime);

        /// <summary>申请发票作废
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetInvoiceStateToWasteRequest(Guid invoiceId, String cancelPersonel);

        /// <summary>取消发票
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetInvoiceStateToCancel(Guid invoiceId, String cancelPersonel);

        /// <summary>更新发票抬头和发票内容
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean SetInvoiceNameAndInvoiceContent(Guid invoiceId, String invoiceName, String invoiceContent);

        /// <summary>通过订单ID获取发票号码和发票是否报税        (Key:发票号码，Value:是否报税)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        KeyValuePair<long, Boolean> GetInvoiceNoAndIsCommitByOrderId(Guid orderId);

        #region [到款通知]

        /// <summary> 插入一条到款通知
        /// </summary>
        /// <param name="pnInfo"></param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo PayNoticeInsert(Guid pushDataId, PaymentNoticeInfo pnInfo);

        /// <summary>是否存在待确认的到款通知
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsExistByOrderNoAndPaystate(string orderNo);

        /// <summary>获取到款通知
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [OperationContract]
        IList<PaymentNoticeInfo> GetPayNoticeInfoListByOrderNo(string orderNo);

        /// <summary>更新到款通知
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="pninfo"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo UpdatePayNoticeInfo(Guid pushDataId, PaymentNoticeInfo pninfo);

        /// <summary>删除到款通知 
        /// </summary>
        /// <param name="payid"></param>
        /// <returns></returns>
        [OperationContract]
        WCFReturnInfo DeletePayNoticeByPayId(Guid payid);

        #endregion
    }
}
