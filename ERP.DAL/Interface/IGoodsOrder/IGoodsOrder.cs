using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoodsOrder
{
    public interface IGoodsOrder
    {
        /// <summary>删除订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool Delete(Guid orderId);

        /// <summary> 插入订单
        /// </summary>
        bool Insert(GoodsOrderInfo goodsOrder);

        /// <summary>订单量分析
        /// </summary>
        /// <returns></returns>
        IList<KeyValuePair<int, int>> GetGoodOrderChart(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId);

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

        /// <summary> 获取指定的订单号是否存在
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns></returns>
        bool IsOrderNo(string orderNo);

        /// <summary>订单额月排行
        /// </summary>
        IList<KeyValuePair<int, decimal>> GetQueryOrderByFinancial(int nonceYear, OrderState orderstate, int statisticsType, Guid countryId, Guid provinceid, Guid cityId, int keepyear, Guid saleFilialeId, Guid salePlatformId);

        /// <summary>可以从数据库中取出订单，不需要有仓库权限
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        GoodsOrderInfo GetGoodsOrder(Guid orderId);

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
        /// <param name="salePlatformId"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="consignee"></param>
        /// <param name="expressNo"></param>
        /// <param name="mobile"></param>
        /// <param name="year"></param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        IList<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, Guid orderId, string orderNo, string consignee,
                                            string expressNo, string mobile, int year, int startPage, int pageSize, out long recordCount);

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

        /// <summary>
        /// 根据订单ID集合获取订单信息
        /// </summary>
        /// For WMS
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        /// ww 2016-07-01
        GoodsOrderInfo GetGoodsOrderInfoByOrderId(Guid orderId);
    }
}
