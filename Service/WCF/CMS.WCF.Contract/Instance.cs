using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using ERP.Model.Invoice;
using ERP.Model.ShopFront;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;
using Framework.Data;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 
    /// </summary>
    public class Instance
    {

        #region lmShop_PaymentNotice

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static bool IsExistByOrderNoAndPaystate(string orderNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.IsExistByOrderNoAndPaystate(orderNo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static IList<PaymentNoticeInfo> GetPayNoticeInfoListByOrderNo(string orderNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPayNoticeInfoListByOrderNo(orderNo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pninfo"></param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        public static WCFReturnInfo UpdatePayNoticeInfo(PaymentNoticeInfo pninfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdatePayNoticeInfo(pushDataId, pninfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payid"></param>
        /// <returns></returns>
        public static WCFReturnInfo DeletePayNoticeByPayId(Guid payid)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.DeletePayNoticeByPayId(payid);
            }
        }

        #endregion

        /// <summary>插入发票数据
        /// </summary>
        /// <param name="invoice">发票模型</param>
        /// <param name="orderIds">订单详细</param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        public static WCFReturnInfo InsertGoodsOrderInvoice(InvoiceInfo invoice, Guid[] orderIds, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertGoodsOrderInvoice(pushDataId, invoice, orderIds);
            }
        }

        /// <summary>
        /// 获取商品类型对应的税率比例(包含：商品类型、类型编码、比例)
        /// </summary>
        /// <param name="goodsTypes"></param>
        /// <returns></returns>
        public List<TaxrateProportionInfo> GeTaxrateProportionInfos(IEnumerable<int> goodsTypes)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GeTaxrateProportionInfos(goodsTypes);
            }
        }

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderToRedeploy(List<String> orderNos)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderToRedeploy(orderNos);
            }
        }

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderToRedeployNew(List<String> orderNos,Guid warehouseId,byte storageType,Guid hostingFilialeId,Guid expressId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderToRedeployNew(orderNos,warehouseId, storageType, hostingFilialeId, expressId);
            }
        }

        /// <summary>更新订单状态为需采购
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderToPurchase(List<String> orderNos)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderToPurchase(orderNos);
            }
        }

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderStateToWaitOutbound(List<String> orderNos)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderStateToWaitOutbound(orderNos);
            }
        }

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderStateToWaitOutboundNew(List<String> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderStateToWaitOutboundNew(orderNos,warehouseId,storageType,hostingFilialeId,expressId);
            }
        }

        /// <summary>设置订单快递号
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderExpressNo(List<String> orderNos, String expressNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderExpressNo(orderNos, expressNo);
            }
        }

        /// <summary>更新订单状态为完成
        /// </summary>
        /// <returns></returns>
        public static Boolean SetGoodsOrderToConsignmented(Guid operatorId, string operatorName, List<String> orderNos)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderToConsignmented(operatorId, operatorName, orderNos);
            }
        }

        /// <summary>更新订单状态为作废
        /// </summary>
        /// <returns></returns>  
        public static Boolean SetGoodsOrderToCancellation(Guid operatorId, string operatorName, bool systemCancel, List<String> orderNos)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsOrderToCancellation(operatorId, operatorName, orderNos);
            }
        }

        /// <summary>获取商品备货信息
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, String> GetGoodsIdsIsStockUp(Guid warehouseId, List<Guid> goodsIds)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsIdsIsStockUp(warehouseId, goodsIds);
            }
        }

        /// <summary>获取会员历史订单记录
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetHistoryGoodsOrderByMemberIdAndOrderTime(memberId, orderTime);
            }
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetHistoryOrderInfoByOrderId(orderId, orderTime);
            }
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetHistoryOrderInfoByOrderNo(orderNo, orderTime);
            }
        }

        /// <summary>获取历史订单明细信息
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderDetailInfo> GetHistoryGoodsOrderDetailListByOrderId(Guid orderId, DateTime orderTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetHistoryGoodsOrderDetailListByOrderId(orderId, orderTime);
            }
        }

        /// <summary>申请发票作废
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceStateToWasteRequest(Guid invoiceId, string cancelPersonel)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetInvoiceStateToWasteRequest(invoiceId, cancelPersonel);
            }
        }

        /// <summary>申请发票作废
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceStateToCancel(Guid invoiceId, string cancelPersonel)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetInvoiceStateToCancel(invoiceId, cancelPersonel);
            }
        }

        /// <summary>更新发票抬头和发票内容
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceNameAndInvoiceContent(Guid invoiceId, string invoiceName, string invoiceContent)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetInvoiceNameAndInvoiceContent(invoiceId, invoiceName, invoiceContent);
            }
        }

        /// <summary>通过订单ID获取发票号码和发票是否报税     (Key:发票号码，Value:是否报税)
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<long, Boolean> GetInvoiceNoAndIsCommitByOrderId(Guid orderId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceNoAndIsCommitByOrderId(orderId);
            }
        }

        /// <summary>
        /// 获取商品特定时间下最近的结算价存档，如果最近结算价没有(即表示是新添加的商品)，则取该商品的采购价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2016-05-19
        public Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetFilialeIdGoodsIdAvgSettlePrice(dateTime);
            }
        }

        /// <summary> 根据订单号，获取指定的发票索取记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByOrderId(Guid orderId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceByOrderId(orderId);
            }
        }

        /// <summary>获取会员Id订单金额，积分列表
        /// </summary>
        /// <returns></returns>
        public PageItems<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetOrderScoreListToPage(memeberId, year, pageSize, pageIndex);
            }
        }


        #region[设置商品上下架]

        /// <summary>
        /// 设置商品上下架
        /// Add by liucaijun at 2011-March-25th
        /// </summary>
        /// <param name="personnelId"> </param>
        /// <param name="goodsList">商品列表</param>
        /// <param name="state">上下架状态</param>
        /// <param name="pushDataId">命令ID</param>
        public static WCFReturnInfo SetGoodsState(Guid personnelId, List<Guid> goodsList, int state, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsState(personnelId, pushDataId, goodsList, state);
            }
        }
        #endregion

        #region[设置商品是否缺货]
        /// <summary>
        /// 设置商品是否缺货
        /// </summary>
        /// <param name="goodsList">商品列表</param>
        /// <param name="state">缺货</param>
        /// <param name="pushDataId">命令ID</param>
        public static WCFReturnInfo SetGoodsIsScarcityState(List<Guid> goodsList, int state, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SetGoodsIsScarcityState(pushDataId, goodsList, state);
            }
        }
        #endregion

        #region -- 获取商品状态

        /// <summary>
        /// 根据商品标识获取商品上下架状态 add jiang 2011-08-01
        /// </summary>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        public static int GetGoodsState(Guid goodsid)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsState(goodsid);
            }
        }

        #endregion


        /// <summary>
        /// 根据商品ID设置商品是否缺货
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="isScarcity">是否缺货</param>
        /// <param name="state"></param>
        public static WCFReturnInfo UpdateGoodsISScarcity(Guid pushDataId, Guid goodsId, int isScarcity, int state)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateGoodsISScarcity(pushDataId, goodsId, isScarcity, state);
            }
        }

        /// <summary>
        /// 设置子商品是否缺货
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="isScarcity"></param>
        /// <returns></returns>
        public static WCFReturnInfo UpdateRealGoodsISScarcity(Guid pushDataId, Guid realGoodsId, int isScarcity)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateRealGoodsISScarcity(pushDataId, realGoodsId, isScarcity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsSales(startTime, endTime);
            }
        }

        /// <summary>
        /// 添加帐务记录数据
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pushDataId"> </param>
        public static WCFReturnInfo InsertReckoningInfo(ReckoningInfo info, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertReckoningInfo(pushDataId, info);
            }
        }

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public static string GetCompanyCussentList(int companyType)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyCussentList(companyType);
            }
        }


        /// <summary>
        /// 获取所有仓库集合
        /// </summary>
        /// <returns></returns>
        public static IList<BankAccountInfo> GetBankAccountList(Guid filialeId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetBankAccountList(filialeId);
            }
        }

        /// <summary>
        /// 对帐
        /// </summary>
        /// <param name="lstModify"></param>
        /// <param name="lstAdd"></param>
        /// <param name="wastBookinfo"></param>
        public static void CheckReckoning(IList<ReckoningInfo> lstModify, IList<ReckoningInfo> lstAdd, WasteBookInfo wastBookinfo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                client.Instance.Checking(lstModify, lstAdd, wastBookinfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="originalTradeCode"></param>
        /// <param name="checkType"></param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        public static ReckoningInfo GetReckoningInfo(Guid companyId, string originalTradeCode, CheckType checkType, int reckoningCheckType = 2)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetReckoningInfo(companyId, originalTradeCode, checkType, reckoningCheckType);
            }
        }

        // 增加往来单位类型字段搜索
        /// <summary>
        /// 按是否对账,日期,账单类型获取往来账（未对账）
        /// 用于列表显示
        /// </summary>
        /// <param name="companyClass">往来单位类型</param>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="filialeId">公司ID </param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cType"></param>
        /// <param name="auditingState"></param>
        /// <param name="receiptType"></param>
        /// <param name="tradeCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="money"></param>
        /// <param name="start"> </param>
        /// <param name="limit"> </param>
        /// <returns></returns>
        public static DataListInfo<ReckoningInfo> GetValidateDataPage(Guid companyClass, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType,
            AuditingState auditingState, ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, params int[] money)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetValidateDataPage(companyClass, companyId, filialeId, startDate,
                                                                        endDate, cType, auditingState, receiptType,
                                                                        tradeCode, warehouseId, keepyear, start, limit, money);
            }
        }

        /// <summary>根据年份和指定条件查询订单
        /// </summary>
        /// <param name="salePlatformId"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderNo"></param>
        /// <param name="consignee"></param>
        /// <param name="expressNo"></param>
        /// <param name="mobile"></param>
        /// <param name="memberId"></param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DataListInfo<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, string orderNo, string consignee, string expressNo, string mobile, Guid memberId, int startPage, int pageSize)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsOrder(salePlatformId, startTime, endTime, orderNo, consignee, expressNo, mobile, memberId, startPage, pageSize);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<BankAccountInfo> GetBankAccountsListFromERP()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetBankAccountsListFromERP();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filialeId"> </param>
        /// <param name="branchId"> </param>
        /// <param name="positionId"> </param>
        /// <returns></returns>
        public static IList<BankAccountInfo> GetBankAccountsListByFBP(Guid filialeId, Guid branchId, Guid positionId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetBankAccountsListByFBP(filialeId, branchId, positionId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetAllCompanyBankAccountsList();
            }
        }

        /// <summary>
        /// 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        public static double GetBankAccountsNonce(Guid bankAccountsId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetBankAccountsNonce(bankAccountsId);
            }
        }

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="startDate"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <returns></returns>
        public static IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, String tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookList(bankAccountsId, startDate, endDate, receiptType, auditingState, minIncome, maxIncome, tradeCode, filialeId, branchId, positionId, keepyear);
            }
        }

        /// <summary>
        /// 根据条件获取账目信息(分页)
        /// </summary>
        /// <param name="bankAccountsId">银行帐号Id</param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="startDate"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Dictionary<int, IList<WasteBookInfo>> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, String tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int startPage, int pageSize)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookListToPage(bankAccountsId, startDate, endDate, receiptType, auditingState, minIncome, maxIncome, tradeCode, filialeId, branchId, positionId, keepyear, startPage, pageSize);
            }
        }

        /// <summary>
        /// 根据条件获取账目信息
        /// </summary>
        /// <param name="saleFilialeId">公司Id</param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="startDate"></param>
        /// <param name="auditingState"></param>
        /// <param name="minIncome"></param>
        /// <param name="maxIncome"></param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <param name="keepyear"></param>
        /// <param name="isCheck"> </param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Dictionary<int, IList<WasteBookInfo>> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookListBySaleFilialeIdToPage(saleFilialeId, startDate, endDate, receiptType, auditingState, minIncome, maxIncome, tradeCode, filialeId, branchId, positionId, keepyear, isCheck, startPage, pageSize);
            }
        }

        /// <summary>
        /// 获取指定tradecode在表中的记录数
        /// </summary>
        /// <param name="tradecode"></param>
        /// <returns></returns>
        public static decimal GetTradeCodeNum(string tradecode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetTradeCodeNum(tradecode);
            }
        }

        /// <summary>
        /// 获取手续费（为reckoning表修改记录用）
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public static decimal GetPoundageForReckoning(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPoundageForReckoning(tradeCode);
            }
        }

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public static string GetWasteBookId(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookId(tradeCode);
            }
        }

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public static string GetWasteBookIdForUpdate(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookIdForUpdate(tradeCode);
            }
        }

        /// <summary>
        /// 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public static string GetWasteBookIdForReckoning(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookIdForReckoning(tradeCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookCheck(wasteBookId);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WasteBookInfo GetWasteBook(Guid wasteBookId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBook(wasteBookId);
            }

        }

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="linkTradeCode">关联单据编号</param>
        /// <param name="wasteSource">1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额</param>
        /// <returns></returns>
        /// zal 2016-06-15
        public static WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookByLinkTradeCodeAndWasteSource(linkTradeCode, wasteSource);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WasteTypeInfo GetWasteBookInfo(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetWasteBookInfo(tradeCode);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<CompanyClassInfo> GetChildCompanyClassList(Guid companyClassId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetChildCompanyClassList(companyClassId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<CompanyCussentInfo> GetCompanyCussentListByClass(Guid companyClassId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyCussentListByClass(companyClassId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<CompanyCussentInfo> GetAllCompanyCussentList()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetAllCompanyCussentList();
            }
        }

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        public static CompanyCussentInfo GetCompanyCussent(Guid companyId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyCussent(companyId);
            }
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public static double GetNonceReckoningTotalled(Guid companyId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetNonceReckoningTotalled(companyId);
            }
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <param name="filialeId"> </param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public static double GetNonceReckoningTotalledDetail(Guid companyId, Guid filialeId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetNonceReckoningTotalledDetail(companyId, filialeId);
            }
        }


        /// <summary> 根据账单编号查询往来账信息
        /// </summary>
        /// <returns></returns>
        public static ReckoningInfo GetReckoning(Guid reckoningId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetReckoning(reckoningId);
            }
        }

        /// <summary>
        /// 获取指定reckoningId的ReckoningType
        /// </summary>
        /// <param name="reckoningId"></param>
        /// <returns></returns>
        public static int GetReckoningType(Guid reckoningId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetReckoningType(reckoningId);
            }
        }

        /// <summary>
        /// 获取指定往来单位的往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        public static decimal GetTotalled(Guid companyId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetTotalled(companyId);
            }
        }

        /// <summary>
        /// 获取手续费
        /// </summary>
        /// <param name="tradeCode">往来单位编号</param>
        public static decimal GetPoundage(string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPoundage(tradeCode);
            }
        }

        /// <summary>
        /// 获取指定reckoningId的ReckoningType
        /// </summary>
        /// <param name="sum"> </param>
        /// <param name="poundage"> </param>
        /// <returns></returns>
        public static bool CheckPoundage(decimal sum, decimal poundage)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.CheckPoundage(sum, poundage);
            }
        }

        /// <summary> 根据账单编号查询往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateReckoning(ReckoningInfo reckoningInfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateReckoning(pushDataId, reckoningInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wasteBookId"></param>
        /// <param name="description"></param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBookDescription(Guid wasteBookId, String description, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBookDescription(pushDataId, wasteBookId, description);
            }
        }

        /// <summary> 修改往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateDescriptionForAuditing(string tradeCode, String description, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateDescriptionForAuditing(pushDataId, tradeCode, description);
            }
        }

        /// <summary> 修改往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateReckoningDescription(Guid reckongId, String description, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateReckoningDescription(pushDataId, reckongId, description);
            }
        }

        /// <summary> 根据账单编号查询往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo AuditingReckoning(string tradeCode, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.AuditingReckoning(pushDataId, tradeCode);
            }
        }

        /// <summary> 根据账单编号删除往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo DeleteReckoning(string tradeCode, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.DeleteReckoning(pushDataId, tradeCode);
            }
        }

        /// <summary> 获取编码
        /// </summary>
        /// <returns></returns>
        public static string GetCode(CodeType codeType)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCode(codeType);
            }
        }

        /// <summary> 修改账目记录
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBook(WasteBookInfo wasteBookInfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBook(pushDataId, wasteBookInfo);
            }
        }

        /// <summary> 修改账目记录
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBookWithBank(WasteBookInfo wasteBookInfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBookWithBank(pushDataId, wasteBookInfo);
            }
        }

        /// <summary> 修改账目记录
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBookDateTime(string tradeCode, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBookDateTime(pushDataId, tradeCode);
            }
        }

        /// <summary>
        /// 在reckoning中更改wastebook中的手续费
        /// </summary>
        public static WCFReturnInfo UpdatePoundageForReckoning(Guid wastebookId, DateTime dateCreated, decimal poundage, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdatePoundageForReckoning(pushDataId, wastebookId, dateCreated, poundage);
            }
        }

        /// <summary> 删除账目记录
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo DeleteWasteBook(string tradeCode, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.DeleteWasteBook(pushDataId, tradeCode);
            }
        }

        /// <summary>
        /// 删除手续费
        /// </summary>
        public static WCFReturnInfo DeleteWasteBookPoundage(string tradeCode, decimal poundage, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.DeleteWasteBookPoundage(pushDataId, tradeCode, poundage);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBookDescriptionForAuditing(string tradeCode, String description, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBookDescriptionForAuditing(pushDataId, tradeCode, description);
            }
        }

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo AuditingWasteBook(string tradeCode, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.AuditingWasteBook(pushDataId, tradeCode);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateWasteBookCheck(WasteBookCheckInfo checkInfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateWasteBookCheck(pushDataId, checkInfo);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo InsertWasteBookCheck(WasteBookCheckInfo checkInfo, Guid pushDataId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertWasteBookCheck(pushDataId, checkInfo);
            }
        }

        ///<summary>
        ///更改一笔账目
        /// </summary>
        public static WCFReturnInfo UpdateBll(Guid pushDataId, Guid outWasteBookId, string description, decimal income, string tradecode, decimal poundage, Guid bankAccountsId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateBll(pushDataId, outWasteBookId, description, income, tradecode, poundage, bankAccountsId);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdateBankAccountsId(Guid pushDataId, Guid wasteBookId, Guid bankAccountsId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateBankAccountsId(pushDataId, wasteBookId, bankAccountsId);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo InsertPoundage(Guid pushDataId, Guid outBankAccountsId, string tradeCode, decimal poundage, Guid filialeId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertPoundage(pushDataId, outBankAccountsId, tradeCode, poundage, filialeId);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo UpdatePoundage(Guid pushDataId, string tradeCode, decimal poundage)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdatePoundage(pushDataId, tradeCode, poundage);
            }
        }

        /// <summary>
        /// 转帐
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="inBankAccountsId">转入帐号Id</param>
        /// <param name="outBankAccountsId">转出帐号Id</param>
        /// <param name="sum">金额</param>
        /// <param name="poundage"></param>
        /// <param name="description">说明</param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"> </param>
        public static WCFReturnInfo Virement(Guid pushDataId, Guid inBankAccountsId, Guid outBankAccountsId, decimal sum, decimal poundage, string description, string tradeCode, Guid filialeId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.Virement(pushDataId, inBankAccountsId, outBankAccountsId, sum, poundage, description, tradeCode, filialeId);
            }
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public static WCFReturnInfo AuditingWaste(Guid pushDataId, string tradeCode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.AuditingWaste(pushDataId, tradeCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        public DateTime GetGoodsPredictArrivalTime(Guid realGoodsId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsPredictArrivalTime(realGoodsId);
            }
        }

        /// <summary> 根据商品ID获取会员ID集合
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public static IList<Guid> GetMemberIdListByRealGoodsIds(DateTime startTime, DateTime endTime, List<Guid> realGoodsIds)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetMemberIdListByRealGoodsIds(startTime, endTime, realGoodsIds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="orderId"></param>
        /// <param name="afterSaleDetailList"></param>
        public static void UpdateGoodsDaySalesStatistics(Guid pushDataId, Guid orderId, IList<AfterSaleDetailInfo> afterSaleDetailList)
        {
            using (var client = ClientFactory.CreateClient())
            {
                client.Instance.UpdateGoodsDaySalesStatistics(pushDataId, orderId, afterSaleDetailList);
            }
        }

        #region 往来单位收付款
        /// <summary> 
        /// 往来单位收付款添加,修改
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        public static CompanyFundReceiptInfo InsertCompanyFundReceiptForShop(CompanyFundReceiptInfo receipt)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertCompanyFundReceiptForShop(receipt);
            }
        }


        /// <summary>
        /// 往来单位收付款查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <param name="receiptType"></param>
        /// <param name="searchKey"> </param>
        /// <returns></returns>
        public static IList<CompanyFundReceiptInfo> GetCompanyFundReceiptForShop(DateTime? startTime, DateTime? endTime, Guid? companyId, Guid filialeId, int? receiptType, string searchKey)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyFundReceiptForShop(startTime, endTime, companyId, filialeId, receiptType, searchKey);
            }
        }

        /// <summary>
        /// 更改往来单位收付款状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="receiptId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static bool UpdateCompanyFundReceiptStatus(Guid pushDataId, Guid receiptId, int status, string remark)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateCompanyFundReceiptStatus(pushDataId, receiptId, status, remark);
            }
        }


        /// <summary> 
        /// 根据id获取往来帐收付款状态
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public static int GetCompanyFundReceiptStatus(Guid receiptId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyFundReceiptStatus(receiptId);
            }
        }
        #endregion



        /// <summary>针对支付宝订单IsOut为False且事后申请发票更新订单和资金流字段IsOut为True（其他地方慎用）
        /// </summary>
        /// <param name="orderIds">订单Ids </param>
        /// <param name="paidNo">交易流水号</param>
        /// <returns></returns>
        public Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.RenewalWasteBookByIsOut(orderIds, paidNo);
            }
        }

        /// <summary>
        /// 获取往来单位字典
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public IDictionary<Guid, String> GetCompanyDic()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyDic();
            }
        }

        /// <summary>
        /// 进货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">入库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public bool RefuseInGoodsBill(string no, string description)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.RefuseInGoodsBill(no, description);
            }
        }
        /// <summary>
        /// 出货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">出库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public bool RefuseOutGoodsBill(string no, string description)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.RefuseOutGoodsBill(no, description);
            }
        }

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetAllRealGoodsSaleNumber(startTime, endTime);
            }
        }

        /// <summary>
        /// 根据开始时间,截止时间，子商品ID，销售平台 获取时间段内具体销售平台某个子商品的销量。
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="salePlatformId">销售平台</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId,
            Guid salePlatformId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetRealGoodsSaleNumber(startTime, endTime, realGoodsId, salePlatformId);
            }
        }

        /// <summary>
        /// 根据订单号，发票状态，订单是否完成发货，发票是否提交，开始时间，截止时间获取发票集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit,
            DateTime fromTime, DateTime toTime)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceList(orderNo, invoiceState, isFinished, isCommit, fromTime, toTime);
            }
        }

        /// <summary>
        /// 根据发票ID获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceDetailInfo GetInvoice(Guid invoiceId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoice(invoiceId);
            }
        }

        /// <summary>
        /// 根据发票号码获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceDetailInfo GetInvoice(long invoiceNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoice(invoiceNo);
            }
        }
        /// <summary>
        /// 根据订单号获取订单简单信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleGoodsOrderInfo GetOrderBasic(string orderNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetOrderBasic(orderNo);
            }
        }

        /// <summary>
        /// 获取发票品名的名称集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public List<string> GetInvoiceItem()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceItem();
            }
        }

        /// <summary>
        /// 根据发票ID更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public bool UpdateInvoiceStateWithInvoiceNo(string orderNo, byte invoiceState, long invoiceno,
            string invoicecode)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateInvoiceStateWithInvoiceNo(orderNo, invoiceState, invoiceno, invoicecode);
            }
        }

        /// <summary>
        /// 根据参数发票卷开始和截止号码获取（返回模型字段：发票卷代码，发票卷所属公司ID，当前发票卷使用到的最大发票号）。
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public InvoiceRollInfo GetInvoiceCodeAndCurInvoiceNo(long invoiceStartNo, long invoiceEndNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceCodeAndCurInvoiceNo(invoiceStartNo, invoiceEndNo);
            }
        }

        /// <summary>
        /// 根据发票ID获取发票打印所属数据
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoicePrintData(invoiceId);
            }
        }

        /// <summary>
        /// 根据订单号 获取订单信息 如果是货到付款的则返回金额，如果不是则返回默认值
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetOrderCodRealTotalPrice(orderNo, out realtotalPrice);
            }
        }

        /// <summary>
        /// 根据订单号集合，是否打印发票来获取订单信息。
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="isPrintInvoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public List<GoodsOrderInvoiceInfo> GetOrder(List<string> orderNos, bool isPrintInvoice)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetOrder(orderNos, isPrintInvoice);
            }
        }

        /// <summary>
        /// 红冲发票并返回发票打印所属数据
        /// </summary>
        /// 1.根据原发票ID查询发票信息
        /// 2.增加一条新的发票信息，金额等于原发票信息的金额的负数，InvoiceCode，InvoiceNo的值等于 参数（红冲后的发票代码，红冲后的发票号码），RequestTime和AcceptedTime为当前时间
        /// 3.修改原发票InvoiceChCode和InvoiceChNo字段为（冲红后的发票代码，红冲后的发票号码）
        /// 4.插入lmshop_OrderInvoice表
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceInfo ChInvoice(Guid invoiceId, string invoiceChCode, long invoiceChNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.ChInvoice(invoiceId, invoiceChCode, invoiceChNo);
            }
        }

        /// <summary>
        /// 新增发票信息并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        ///  For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceInfo AddInvoice(AddInvoiceInfo invoiceAdd)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.AddInvoice(invoiceAdd);
            }
        }

        /// <summary>
        /// 重开发票或重开并作废原发票并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        /// <param name="isCancelOriginalInvoice">是否作废原发票</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public SimpleInvoiceInfo AgainInvoice(AgainInvoiceInfo invoiceAdd, bool isCancelOriginalInvoice)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.AgainInvoice(invoiceAdd, isCancelOriginalInvoice);
            }
        }

        /// <summary>
        /// 根据发票id和起始号更新发票卷为启用
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-13
        public bool UpdateInvoiceRollStateInvocation(Guid rollId, long startNo)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateInvoiceRollStateInvocation(rollId, startNo);
            }
        }

        /// <summary>入库单完成
        /// </summary>
        /// <param name="inTradeCode"></param>
        /// <param name="description"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public bool FinishStorageRecordIn(string inTradeCode, String description, Dictionary<Guid, int> stockQuantitys)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.FinishStorageRecordIn(inTradeCode, description, stockQuantitys);
            }
        }

        /// <summary>出库单完成
        /// </summary>
        /// <param name="outTradeCode"></param>
        /// <param name="description"></param>
        /// <param name="operatorName"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public bool FinishStorageRecordOut(string outTradeCode, string description, string operatorName, Dictionary<Guid, int> stockQuantitys)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.FinishStorageRecordOut(outTradeCode, description, operatorName, stockQuantitys);
            }
        }

        /// <summary>
        /// 更新出入库明细商品当前库存
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public Boolean UpdateNonceWarehouseGoodsStock(String tradeCode, Dictionary<Guid, int> stockQuantitys)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.UpdateNonceWarehouseGoodsStock(tradeCode, stockQuantitys);
            }
        }

        /// <summary>
        /// 丢件找回插入相关往来帐
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="payToKede"></param>
        /// <param name="badMoney"></param>
        /// <returns></returns>
        public Boolean InsertReckoningForLostBack(string orderNo, decimal payToKede, decimal badMoney)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.InsertReckoningForLostBack(orderNo, payToKede, badMoney);
            }
        }

        /// <summary>
        /// 根据订单号列表获取发票信息
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public IList<SimpleInvoiceDetailInfo> GetInvoiceListByOrderNos(List<string> orders)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetInvoiceListByOrderNos(orders);
            }
        }


        /// <summary>获取商品的采购设置（用于门店自己采购，供应商直接发货至门店） 2016年10月27日   陈重文
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public PurchaseSetInfo GetPurchaseSetInfo(Guid goodsId, Guid warehouseId)
        {
            if (goodsId == default(Guid) || warehouseId == default(Guid))
            {
                return null;
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPurchaseSetInfo(goodsId, warehouseId);
            }
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="isDelete"></param>
        /// <returns>0:禁用；1:启用；2:全部</returns>
        /// zal 2017-03-16
        public IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId, int isDelete)
        {
            if (goodsIdList == null || warehouseId.Equals(Guid.Empty))
            {
                return new List<PurchaseSetInfo>();
            }
            if (goodsIdList.Count == 0)
            {
                return new List<PurchaseSetInfo>();
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPurchaseSetList(goodsIdList, warehouseId, isDelete);
            }
        }

        /// <summary>
        /// 获取所有商品采购设置
        /// </summary>
        /// <returns></returns>
        /// zal 2017-03-07
        public List<PurchaseSetInfo> GetPurchaseSetListByWarehouseIdAndCompanyId(Guid warehouseId, Guid companyId)
        {
            if (companyId.Equals(Guid.Empty) || warehouseId.Equals(Guid.Empty))
            {
                return new List<PurchaseSetInfo>();
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetPurchaseSetListByWarehouseIdAndCompanyId(warehouseId, companyId);
            }
        }

        /// <summary>第三方订单直接完成发货【四维】        ADD  2016年12月6日  陈重文
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressId">订单快递Id</param>
        /// <param name="expressNo">订单快递号</param>
        /// <param name="companyId">配置的往来单位ID</param>
        /// <returns></returns>
        public Boolean ThirdPartyOrderComplete(String orderNo, Guid expressId, String expressNo, Guid companyId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.ThirdPartyOrderComplete(orderNo, expressId, expressNo, companyId);
            }
        }

        /// <summary>获取往来单位绑定公司信息   ADD 2016年12月7日  陈重文
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public List<Guid> GetCompanyBindFilialeIds(Guid companyId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetCompanyBindFilialeIds(companyId);
            }
        }

        /// <summary>
        /// 获取当月的退货记录
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-03-15
        public Dictionary<string, decimal> SelectOneMonthReturnedApplyList(Guid shopId, DateTime dateTime)
        {
            if (shopId.Equals(Guid.Empty))
            {
                return new Dictionary<string, decimal>();
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.SelectOneMonthReturnedApplyList(shopId, dateTime);
            }
        }


        /// <summary>根据销售公司获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        public Dictionary<Guid, int> GetGoodsSaleBySaleFilialeId(DateTime fromTime, DateTime toTime, Guid saleFilialeId)
        {
            if (saleFilialeId == default(Guid))
            {
                return new Dictionary<Guid, int>();
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsSaleBySaleFilialeId(fromTime, toTime, saleFilialeId);
            }
        }

        /// <summary>根据销售平台获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="salePlatformIdList">销售平台ID集合</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        /// zal 2017-07-27
        public Dictionary<Guid, int> GetGoodsSaleBySalePlatformIdList(DateTime fromTime, DateTime toTime, List<Guid> salePlatformIdList)
        {
            if (salePlatformIdList == null || salePlatformIdList.Count == 0)
            {
                return new Dictionary<Guid, int>();
            }
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsSaleBySalePlatformIdList(fromTime, toTime, salePlatformIdList);
            }
        }

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <returns></returns>
        /// zal 2017-08-08
        public List<VocabularyInfo> GetVocabularyListByState()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetVocabularyListByState();
            }
        }

        /// <summary>
        /// 获取违禁词(State=1)
        /// </summary>
        /// <returns></returns>
        /// zal 2017-10-25
        public List<string> GetVocabularyNameList()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetVocabularyNameList();
            }
        }

        #region 海外购相关接口
        /// <summary>
        /// 根据商品IDS，批量获取对应的商品 供应商关系（供应商ID，商品ID）
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public Dictionary<Guid, Guid> GetGoodsAndCompanyList(IList<Guid> goodsId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetGoodsAndCompanyList(goodsId);
            }
        }

        /// <summary>
        /// 获取境外供应商列表（返回值：供应商ID，名称）
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, String> GetAbroadCompanyList()
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetAbroadCompanyList();
            }
        }

        /// <summary>
        /// 根据登录账号、销售公司Id获取已授权的境外供应商列表
        /// </summary>
        /// <returns></returns>
        public List<AuthorizeCompanyDTO> GetAuthorizeCompanyDtos(string accountNo, Guid saleFilialeId)
        {
            using (var client = ClientFactory.CreateClient())
            {
                return client.Instance.GetAuthorizeCompanyDtos(accountNo, saleFilialeId);
            }
        }
        #endregion

    }
}
