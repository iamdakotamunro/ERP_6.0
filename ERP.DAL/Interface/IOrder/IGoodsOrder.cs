using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IOrder
{
    /// <summary> �����ӿ�
    /// </summary>
    public interface IGoodsOrder
    {
        /// <summary>ɾ������
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool Delete(Guid orderId);

        /// <summary> ���붩��
        /// </summary>
        bool Insert(GoodsOrderInfo goodsOrder, out string errorMsg);

        /// <summary>����������
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChart(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatform);

        /// <summary>����״̬��ʱ����״ͼ���˵���Ŀ��
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChart(DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary> ���ȫ�������(ʡ����| ������������������������
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, double>> GetOrderAmountRecord(Guid provinceId, Guid cityId, DateTime startTime, DateTime endTime, int payMode, int[] orderState, int result, int showMode, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>���� ������״̬��ʱ����״ͼ
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="datetime"></param>
        /// <param name="keepyear">������������</param>
        /// <param name="saleFilialeID"> </param>
        /// <param name="salePlatformID"> </param>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChartDay(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeID, Guid salePlatformID);

        /// <summary>���£�ÿ�죩������״̬��ʱ����״ͼ���˵���Ŀ��
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChartDay(DateTime datetime, int keepyear, Guid saleFilialeID, Guid salePlatformID);

        /// <summary>ÿ���Сʱ������ͳ��
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetOrderHalfHour(DateTime datetime, int[] orderStates, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>��ȡָ����Ʊ�а����Ķ���
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetInvoiceGoodsOrderList(Guid invoiceId);

        IList<GoodsOrderInfo> GetOrderList(List<Guid> authWarehouseIds, DateTime startTime, DateTime endTime, Guid goodsId, string identifyCode, List<OrderState> orderStates, int pageIndex, int pageSize, out int recordCount);

        /// <summary> ��ȡָ���Ķ������Ƿ����
        /// </summary>
        /// <param name="orderNo">�������</param>
        /// <returns></returns>
        bool IsOrderNo(string orderNo);

        /// <summary>������������
        /// </summary>
        IList<KeyValuePair<int, decimal>> GetQueryOrderByFinancial(int nonceYear, OrderState orderstate, int statisticsType, Guid countryId, Guid provinceid, Guid cityId, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>��ȡ������Ϣ
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(Guid orderId);

        /// <summary>��ȡ������Ϣ
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(String orderNo);

        /// <summary> ������ƷID��ȡ�ض������µĵ����Ƿ����
        /// </summary>
        /// <returns></returns>
        bool SelectSemiStockAtOneYearByGoodsId(Guid goodsId, List<Guid> realGoodsIds, List<int> stockTypes, int days, List<int> stockStates);

        /// <summary>��Ա���������������б�
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetOrderList(DateTime start, DateTime end, OrderState state, string orderNo, Guid memberId, string mobile, string consignee, Guid warehouseId, string expressNo, Guid saleFilialeId, Guid salePlatformId, int keepyear, int startPage, int pageSize, out long recordCount);


        /// <summary>������ݺ�ָ��������ѯ����
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, Guid orderId, string orderNo, string consignee,
                                            string expressNo, string mobile, Guid memberId, int year, int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// �������ɶ�����
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        bool InsertWaitConsignmentOrder(Guid orderId, int orderState, string operatorName);

        /// <summary>
        /// ��ȡ����ɷ����Ķ���ID
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<WaitConsignmentOrderInfo> GetWaitConsignmentOrder(int top);

        /// <summary>
        /// ɾ������ɷ����Ķ���ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool DeleteWaitConsignmentOrder(Guid orderId);

        /// <summary> ������ƷID��ȡ��ԱID����
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        IList<Guid> GetMemberIdListByRealGoodsIds(int keepyear, DateTime startTime, DateTime endTime, List<Guid> realGoodsIds);

        /// <summary>
        /// ���ݶ����Ż�ȡ��������Ϣ
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleGoodsOrderInfo GetOrderBasic(string orderNo);

        /// <summary>
        /// ���ݶ����� ��ȡ������Ϣ ����ǻ���������򷵻ؽ���������򷵻�Ĭ��ֵ
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice);

        /// <summary>
        /// ���ݶ����ż��ϣ��Ƿ��ӡ��Ʊ����ȡ������Ϣ 
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        List<GoodsOrderInvoiceInfo> GetGoodsOrderInfoByorderNos(List<string> orderNos);

        /// <summary>
        /// ���ݷ�Ʊ�Ż�ö���id�Ͷ�����
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        GoodsOrderInfo GetGoodsOrderByInvoiceId(Guid invoiceId);

        /// <summary>���¶���״̬Ϊ�����
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToRedeploy(List<String> orderNos);

        /// <summary>
        /// ����ת�Ʒ����ָ���״̬Ϊ�����
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToRedeployNew(List<String> orderNos,Guid warehouseId,byte storageType,Guid hostingFilialeId,Guid expressId);

        /// <summary>���¶���״̬Ϊ��ɹ�
        /// </summary>
        /// <returns></returns>
        bool SetGoodsOrderToPurchase(List<string> orderNos);

        /// <summary>���¶���״̬Ϊ������
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderStateToWaitOutbound(List<String> orderNos);

        /// <summary>
        /// ����ת�Ʒ����ָ���״̬Ϊ������
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderStateToWaitOutboundNew(List<String> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId);

        /// <summary>���ö�����ݺ�
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        Boolean SetGoodsOrderExpressNo(List<String> orderNos, String expressNo);

        /// <summary>���¶���״̬Ϊ���
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToConsignmented(String orderNo);

        /// <summary>���¶���״̬Ϊ����
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToCancellation(String orderNo);

        /// <summary>��ȡ����ID�Ͷ������ֵ�
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, String> GetOrderNoDicAndWarehouseIdsByOrderIds(List<Guid> orderIds, out Guid warehouseIds);

        /// <summary>��ȡ��Ա��ʷ������¼
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime);

        /// <summary>��ȡ��ʷ������Ϣ
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime);

        /// <summary>��ȡ��ʷ������Ϣ
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime);


        /// <summary>��ȡ��Ա�����������б�
        /// </summary>
        /// <returns></returns>
        IList<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex, out long recordCount);

        /// <summary>
        /// ���Դ����ݿ���ȡ������������Ҫ�вֿ�Ȩ��
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="keepyear"></param>
        /// <param name="orderTime"></param>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(string orderNo, int keepyear, DateTime orderTime);

        List<TempOrderInfo> TempOrderList(Int32 top, DateTime fromOrderTime, DateTime toOrderTime);

        Boolean InsertGoodsTempOrder(Guid orderId);

        Boolean IsCalculated(Guid orderId);

        void RenewGoodsDaySalesStatisticsOfSellPrice(Decimal subtractSellPrice, Guid deliverFilialeId, Guid deliverWarehouseId, DateTime dayTime, Guid realGoodsId, Guid salePlatformId);

        /// <summary>����������ֱ�����     ADD   ������
        /// </summary>
        /// <param name="orderNo">������</param>
        /// <param name="expressId">�������Id</param>
        /// <param name="expressNo">��ݺ�</param>
        /// <returns></returns>
        Boolean ThirdPartyOrderDirectlyToComplete(String orderNo, Guid expressId, String expressNo);

        /// <summary>
        /// Ϊ�������϶�������ʹ��
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-03-13
        IList<GoodsOrderInfo> GetGoodsOrderInfoListForHistory(int pageIndex, int pageSize);

        /// <summary>
        /// ����OrderIdTable���е�״̬
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool UpdateOrderIdTable(Guid orderId);
    }
}