using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.Model.Report;
using Keede.Ecsoft.Model;
using ERP.Model;

namespace ERP.DAL.Interface.IOrder
{
    /// <summary>订单明细
    /// </summary>
    public interface IGoodsOrderDetail
    {
        /// <summary>
        /// 添加订单明细
        /// </summary>
        /// <param name="goodsOrderDetailList"></param>
        /// <param name="goodsOrder"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool Insert(IList<GoodsOrderDetailInfo> goodsOrderDetailList, GoodsOrderInfo goodsOrder, out string errorMsg);

        /// <summary>获取订单明细
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(Guid orderId);

        /// <summary>获取订单明细
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="orderTime"> </param>
        /// <returns></returns>
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(Guid orderId, DateTime orderTime);

        /// <summary> 保存商品日销售量
        /// </summary>
        /// <param name="gdsInfo"></param>
        /// <param name="flag">是否是售后(True:是;False:否;)</param>
        void SaveGoodsSales(List<GoodsDaySalesStatisticsInfo> gdsInfo, bool flag = false);

        /// <summary> 根据订单ID查询订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailByOrderId(Guid orderId);

        /// <summary>
        /// 根据OrderId集合查询GoodsId
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        /// zal 2016-05-20
        List<Guid> GetGoodsIdByOrderId(List<Guid> orderIds);

        /// <summary>
        /// 根据OrderId集合获取订单明细数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        /// zal 2016-07-08
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailByOrderIds(List<Guid> orderIds);

        /// <summary>根据订单时间获取日销量表中的goodsId
        /// </summary>
        /// <param name="startTime">订单时间</param>
        /// <param name="endTime">订单时间</param>
        /// <returns></returns>
        /// zal 2016-06-06
        IList<Guid> GetGoodsDaySalesStatisticsList(DateTime startTime, DateTime endTime);

        /// <summary> 根据goodsId修改指定时间内的“月平均结算价”
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="startTime">订单时间</param>
        /// <param name="endTime">订单时间</param>
        /// <param name="avgSettlePrice">月平均结算价</param>
        /// <returns></returns>
        /// zal 2016-06-20
        bool UpdateGoodsDaySalesStatisticsAvgSettlePrice(Guid goodsId, decimal avgSettlePrice, DateTime startTime, DateTime endTime);

        Boolean InsertASYN_GoodsDaySalesStatisticsInfo(ASYN_GoodsDaySalesStatisticsInfo asynGoodsDaySalesStatisticsInfo);

        List<ASYN_GoodsDaySalesStatisticsInfo> GetAsynGoodsDaySalesStatisticsList();

        void SetAsynGoodsDaySalesStatisticsExecuted(Guid id);

        /// <summary> 根据订单号获得订单明细数据
        /// </summary>
        /// <param name="orderId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        List<GoodsOrderDetails> GetGoodsOrderDetailsByOrderId(Guid orderId);

        /// <summary>
        /// 获取指定时间段内订单及对应商品和数量
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<GoodsOrderDetailBaseInfo> GetGoodsQuantityDics(Guid warehouseId, DateTime startTime, DateTime endTime, Guid personResponsible);

        List<GoodsOrderDemandInfo> GetGoodsOrderDemandInfos(Guid warehouseId, Guid realGoodsId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderTime"></param>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="keepyear"></param>
        /// <returns></returns>
        IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(DateTime orderTime, Guid orderId, string orderNo, int keepyear);

        List<NeedPurchasingGoods> GetNeedPurchasingGoodses(Guid warehouseId, Guid personResponsible, DateTime startTime,
            DateTime endTime, IEnumerable<int> orderStates);

        #region 公司毛利、商品毛利
        #region 公司毛利
        /// <summary> 
        /// 计算指定时间段(指定时间段：目前为每天)内的公司毛利【订单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        bool CompanyGrossProfitForEveryDay_GoodsOrder(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的公司毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        bool CompanyGrossProfitForEveryDay_StorageRecord(DateTime startTime, DateTime endTime);

        /// <summary> 
        /// 获取生成的公司毛利
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        IList<CompanyGrossProfitRecordDetailInfo> GetCompanyGrossProfit(DateTime dayTime,int pageIndex, int pageSize);
        #endregion

        #region 商品毛利
        /// <summary> 
        /// 计算指定时间段(指定时间段：目前为每天)内的商品毛利【订单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        bool GoodsGrossProfitForEveryDay_GoodsOrder(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 计算指定时间段(指定时间段：目前为每天)内的商品毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        bool GoodsGrossProfitForEveryDay_StorageRecord(DateTime startTime, DateTime endTime);

        /// <summary> 
        /// 获取生成的商品毛利【订单】
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        IList<GoodsGrossProfitRecordDetailInfo> GetGoodsGrossProfit_GoodsOrder(DateTime dayTime, int pageIndex, int pageSize);

        /// <summary>
        /// 获取生成的商品毛利【门店采购单对应的ERP出库单】
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name = "pageIndex"></param >
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        IList<GoodsGrossProfitRecordDetailInfo> GetGoodsGrossProfit_StorageRecord(DateTime dayTime, int pageIndex, int pageSize);
        #endregion

        /// <summary> 
        /// 获取执行失败的日期【公司毛利、商品毛利】
        /// </summary>
        /// <param name="executeTypeList"></param>
        /// <returns></returns>
        /// zal 2017-07-13
        IList<string> GetExecuteFailedDayTime(List<int> executeTypeList);

        #endregion

        List<SourceBindGoods> GetSourceBindGoodsesByOrderNo(string orderNo);
    }
}
