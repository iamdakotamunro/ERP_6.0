using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IOrder
{
    /// <summary> 订单接口
    /// </summary>
    public interface IGoodsOrder
    {
        /// <summary>删除订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool Delete(Guid orderId);

        /// <summary> 插入订单
        /// </summary>
        bool Insert(GoodsOrderInfo goodsOrder, out string errorMsg);

        /// <summary>订单量分析
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChart(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatform);

        /// <summary>单按状态与时间柱状图（退单数目）
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChart(DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary> 获得全部或地区(省级）| 货到付款、款到发货订单金额分析表
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, double>> GetOrderAmountRecord(Guid provinceId, Guid cityId, DateTime startTime, DateTime endTime, int payMode, int[] orderState, int result, int showMode, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>按月 订单按状态与时间柱状图
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="datetime"></param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="saleFilialeID"> </param>
        /// <param name="salePlatformID"> </param>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChartDay(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeID, Guid salePlatformID);

        /// <summary>按月（每天）定单按状态与时间柱状图（退单数目）
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChartDay(DateTime datetime, int keepyear, Guid saleFilialeID, Guid salePlatformID);

        /// <summary>每半个小时订单的统计
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetOrderHalfHour(DateTime datetime, int[] orderStates, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>获取指定发票中包含的订单
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetInvoiceGoodsOrderList(Guid invoiceId);

        IList<GoodsOrderInfo> GetOrderList(List<Guid> authWarehouseIds, DateTime startTime, DateTime endTime, Guid goodsId, string identifyCode, List<OrderState> orderStates, int pageIndex, int pageSize, out int recordCount);

        /// <summary> 获取指定的订单号是否存在
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns></returns>
        bool IsOrderNo(string orderNo);

        /// <summary>订单额月排行
        /// </summary>
        IList<KeyValuePair<int, decimal>> GetQueryOrderByFinancial(int nonceYear, OrderState orderstate, int statisticsType, Guid countryId, Guid provinceid, Guid cityId, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>获取订单信息
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(Guid orderId);

        /// <summary>获取订单信息
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(String orderNo);

        /// <summary> 根据商品ID获取特定条件下的单据是否存在
        /// </summary>
        /// <returns></returns>
        bool SelectSemiStockAtOneYearByGoodsId(Guid goodsId, List<Guid> realGoodsIds, List<int> stockTypes, int days, List<int> stockStates);

        /// <summary>会员订单（搜索订单列表）
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetOrderList(DateTime start, DateTime end, OrderState state, string orderNo, Guid memberId, string mobile, string consignee, Guid warehouseId, string expressNo, Guid saleFilialeId, Guid salePlatformId, int keepyear, int startPage, int pageSize, out long recordCount);


        /// <summary>根据年份和指定条件查询订单
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, Guid orderId, string orderNo, string consignee,
                                            string expressNo, string mobile, Guid memberId, int year, int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 插入待完成订单表
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        bool InsertWaitConsignmentOrder(Guid orderId, int orderState, string operatorName);

        /// <summary>
        /// 获取待完成发货的订单ID
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<WaitConsignmentOrderInfo> GetWaitConsignmentOrder(int top);

        /// <summary>
        /// 删除待完成发货的订单ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool DeleteWaitConsignmentOrder(Guid orderId);

        /// <summary> 根据商品ID获取会员ID集合
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        IList<Guid> GetMemberIdListByRealGoodsIds(int keepyear, DateTime startTime, DateTime endTime, List<Guid> realGoodsIds);

        /// <summary>
        /// 根据订单号获取订单简单信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleGoodsOrderInfo GetOrderBasic(string orderNo);

        /// <summary>
        /// 根据订单号 获取订单信息 如果是货到付款的则返回金额，如果不是则返回默认值
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice);

        /// <summary>
        /// 根据订单号集合，是否打印发票来获取订单信息 
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        List<GoodsOrderInvoiceInfo> GetGoodsOrderInfoByorderNos(List<string> orderNos);

        /// <summary>
        /// 根据发票号获得订单id和订单号
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        GoodsOrderInfo GetGoodsOrderByInvoiceId(Guid invoiceId);

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToRedeploy(List<String> orderNos);

        /// <summary>
        /// 订单转移发货仓更新状态为需调拨
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToRedeployNew(List<String> orderNos,Guid warehouseId,byte storageType,Guid hostingFilialeId,Guid expressId);

        /// <summary>更新订单状态为需采购
        /// </summary>
        /// <returns></returns>
        bool SetGoodsOrderToPurchase(List<string> orderNos);

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderStateToWaitOutbound(List<String> orderNos);

        /// <summary>
        /// 订单转移发货仓更新状态为出货中
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderStateToWaitOutboundNew(List<String> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId);

        /// <summary>设置订单快递号
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        Boolean SetGoodsOrderExpressNo(List<String> orderNos, String expressNo);

        /// <summary>更新订单状态为完成
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToConsignmented(String orderNo);

        /// <summary>更新订单状态为作废
        /// </summary>
        /// <returns></returns>
        Boolean SetGoodsOrderToCancellation(String orderNo);

        /// <summary>获取订单ID和订单号字典
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, String> GetOrderNoDicAndWarehouseIdsByOrderIds(List<Guid> orderIds, out Guid warehouseIds);

        /// <summary>获取会员历史订单记录
        /// </summary>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime);

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime);

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime);


        /// <summary>获取会员订单金额，积分列表
        /// </summary>
        /// <returns></returns>
        IList<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex, out long recordCount);

        /// <summary>
        /// 可以从数据库中取出订单，不需要有仓库权限
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

        /// <summary>第三方订单直接完成     ADD   陈重文
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressId">订单快递Id</param>
        /// <param name="expressNo">快递号</param>
        /// <returns></returns>
        Boolean ThirdPartyOrderDirectlyToComplete(String orderNo, Guid expressId, String expressNo);

        /// <summary>
        /// 为处理作废订单销量使用
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-03-13
        IList<GoodsOrderInfo> GetGoodsOrderInfoListForHistory(int pageIndex, int pageSize);

        /// <summary>
        /// 更新OrderIdTable表中的状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool UpdateOrderIdTable(Guid orderId);
    }
}