using System;
using System.Collections.Generic;
using System.ServiceModel;
using ERP.Model;
using ERP.Model.WMS;

namespace ERP.Service.Contract
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/28 14:14:22 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/6/28 14:14:22 
     * 修改人  ：  
     * 描述    ：
     */
    public partial interface IService
    {
        /// <summary>
        /// 获取往来单位字典
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        [OperationContract]
        IDictionary<Guid, String> GetCompanyDic();


        /// <summary>
        /// 进货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">入库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        [OperationContract]
        bool RefuseInGoodsBill(string no, string description);

        /// <summary>
        /// 出货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">出库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        [OperationContract]
        bool RefuseOutGoodsBill(string no, string description);

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        [OperationContract]
        IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品日均销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        IList<GoodsAvgDaySalesInfo> GetAvgRealGoodsSaleNumber(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 根据开始时间,截止时间，子商品ID，销售平台 获取时间段内具体销售平台某个子商品的销量。
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="salePlatformId">销售平台</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        [OperationContract]
        int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId,
            Guid salePlatformId);


        /// <summary>
        /// 根据订单号，发票状态，订单是否完成发货，发票是否提交，开始时间，截止时间获取发票集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract]
        IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit,
            DateTime fromTime, DateTime toTime);

        /// <summary>
        /// 根据发票ID获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract(Name = "GetInvoiceByInvoiceId")]

        SimpleInvoiceDetailInfo GetInvoice(Guid invoiceId);

        /// <summary>
        /// 根据发票号码获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract(Name = "GetInvoiceByInvoiceNo")]
        SimpleInvoiceDetailInfo GetInvoice(long invoiceNo);

        /// <summary>
        /// 根据订单号获取订单简单信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract]
        SimpleGoodsOrderInfo GetOrderBasic(string orderNo);

        /// <summary>
        /// 获取发票品名的名称集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract]
        List<string> GetInvoiceItem();

        /// <summary>
        /// 根据订单号更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract]
        bool UpdateInvoiceStateWithInvoiceNo(string orderNo, byte invoiceState, long invoiceno,
            string invoicecode);

        /// <summary>
        /// 根据参数发票卷开始和截止号码获取（返回模型字段：发票卷代码，发票卷所属公司ID，当前发票卷使用到的最大发票号）。
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        [OperationContract]
        InvoiceRollInfo GetInvoiceCodeAndCurInvoiceNo(long invoiceStartNo, long invoiceEndNo);

        /// <summary>
        /// 根据发票ID获取发票打印所属数据
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        [OperationContract]
        SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId);

        /// <summary>
        /// 根据订单号 获取订单信息 如果是货到付款的则返回金额，如果不是则返回默认值
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        [OperationContract]
        bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice);

        /// <summary>
        /// 根据订单号集合，是否打印发票来获取订单信息。
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="isPrintInvoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        [OperationContract]
        List<GoodsOrderInvoiceInfo> GetOrder(List<string> orderNos, bool isPrintInvoice);

        /// <summary>
        /// 红冲发票并返回发票打印所属数据
        /// </summary>
        /// 1.根据原发票ID查询发票信息
        /// 2.增加一条新的发票信息，金额等于原发票信息的金额的负数，InvoiceCode，InvoiceNo的值等于 参数（红冲后的发票代码，红冲后的发票号码），RequestTime和AcceptedTime为当前时间
        /// 3.修改原发票InvoiceChCode和InvoiceChNo字段为（红冲后的发票代码，红冲后的发票号码）
        /// 4.插入lmshop_OrderInvoice表
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        [OperationContract]
        SimpleInvoiceInfo ChInvoice(Guid invoiceId, string invoiceChCode, long invoiceChNo);

        /// <summary>
        /// 新增发票信息并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        ///  For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        [OperationContract]
        SimpleInvoiceInfo AddInvoice(AddInvoiceInfo invoiceAdd);

        /// <summary>
        /// 重开发票或重开并作废原发票并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        /// <param name="isCancelOriginalInvoice">是否作废原发票</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        [OperationContract]
        SimpleInvoiceInfo AgainInvoice(AgainInvoiceInfo invoiceAdd, bool isCancelOriginalInvoice);

        /// <summary>
        /// 根据发票id和起始号更新发票卷为启用
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-04
        [OperationContract]
        bool UpdateInvoiceRollStateInvocation(Guid rollId, long startNo);


        /// <summary>入库单完成
        /// </summary>
        /// <param name="inTradeCode"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean FinishStorageRecordIn(String inTradeCode, String description, Dictionary<Guid, int> stockQuantitys);

        /// <summary>出库单完成
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Boolean FinishStorageRecordOut(String outTradeCode, String description,String operatorName, Dictionary<Guid, int> stockQuantitys);

        /// <summary>
        /// 更新出入库明细商品当前库存
        /// </summary>
        /// <param name="inTradeCode"></param>
        /// <param name="stockQuantitys"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean UpdateNonceWarehouseGoodsStock(String inTradeCode, Dictionary<Guid, int> stockQuantitys);

        /// <summary>
        /// 丢件找回插入相关往来帐
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="payToKede"></param>
        /// <param name="badMoney"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean InsertReckoningForLostBack(string orderNo, decimal payToKede, decimal badMoney);

        /// <summary>
        /// 根据订单号列表获取发票信息
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [OperationContract]
        IList<SimpleInvoiceDetailInfo> GetInvoiceListByOrderNos(List<string> orders);

        /// <summary>
        /// 判断进货单关联的入库单对应的往来单位是否搁置
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean IsAbeyanced(string tradeCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        WMSResultInfo AddBySellReturnIn(WMSReturnGoodsRequest request);

        /// <summary>
        /// 取消/作废订单 删除订单相关出库单据
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [OperationContract]
        WMSResultInfo CancelStorageRecord(string orderNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        WMSResultInfo AddByLostBackReturnIn(WMSLostBackReturnDTO request);

        /// <summary>
        /// 存在采购设置的商品集合
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseIds"></param>
        /// <returns></returns>
        [OperationContract]
        List<KeyValuePair<Guid,Guid>> HasPurchseSetGoodsIds(IEnumerable<Guid> goodsIds,IEnumerable<Guid> warehouseIds);

        /// <summary>
        /// 验证商品是否存在采购设置
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <returns>Key物流公司,Value 存在采购设置的商品列表</returns>
        [OperationContract]
        Dictionary<Guid, List<Guid>> HasPurchseSetGoodsIdsGroup(IEnumerable<Guid> goodsIds, Guid warehouseId);

        /// <summary>
        /// 拆分组合更新结算价
        /// </summary>
        /// <param name="isSplit">是否为拆分</param>
        /// <param name="billNo"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="combineSplitGoodsId">主商品ID</param>
        /// <param name="combineSplitGoodsQuantity">主商品数量</param>
        /// <param name="combineSplitDetailGoodsIds">组合或拆分的商品明细，主商品ID、数量</param>
        /// <param name="occerTime"></param>
        /// <param name="stockQuantitys">各主商品库存</param>
        /// <returns></returns>
        [OperationContract]
        WMSResultInfo CreateByCombineSplit(bool isSplit,string billNo,Guid hostingFilialeId,Guid combineSplitGoodsId, int combineSplitGoodsQuantity, Dictionary<Guid,int> combineSplitDetailGoodsIds,DateTime occerTime ,Dictionary<Guid, int> stockQuantitys);
    }
}
