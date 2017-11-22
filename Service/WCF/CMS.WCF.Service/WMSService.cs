using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AllianceShop.Contract.DataTransferObject;
using AllianceShop.Enum;
using ERP.BLL.Implement.Organization;
using ERP.Enum;
using ERP.Enum.ShopFront;
using ERP.Model;
using ERP.Model.ShopFront;
using ERP.Model.WMS;
using ERP.SAL;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.ShopFront;
using AuditingState = ERP.Enum.AuditingState;
using CodeType = ERP.Enum.CodeType;
using ReckoningType = ERP.Enum.ReckoningType;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        /// <summary>
        /// 获取往来单位字典
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public IDictionary<Guid, String> GetCompanyDic()
        {
            return _companyCussent.GetCompanyDic();
        }

        /// <summary>
        /// 进货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">入库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public bool RefuseInGoodsBill(string no, string description)
        {
            if (string.IsNullOrEmpty(no))
            {
                SAL.LogCenter.LogService.LogError("未传入入库单号", "ERP.Service.RefuseInGoodsBill", null);
                return false;
            }
            try
            {
                var storageRecord = _storageRecordDao.GetStorageRecord(no);
                if (storageRecord == null) return false;
                if (storageRecord.StockType == (int)StorageRecordType.InnerPurchase)
                    return _storageRecordDao.RefuseInorOutGoodsBill(no, description);
                string errorMsg;
                return _storageRecordDao.UpdateStorageState(no, (int)StorageRecordState.Refuse, description, out errorMsg);
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 入库单号：{1}", ex.Message, no), "ERP.Service.RefuseInGoodsBill", ex);
                return false;
            }
        }

        /// <summary>
        /// 出货单据核退（更新ERP入库单据状态）。
        /// </summary>
        /// <param name="no">出库单号</param>
        /// <param name="description">描述</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public bool RefuseOutGoodsBill(string no, string description)
        {
            if (string.IsNullOrEmpty(no))
            {
                SAL.LogCenter.LogService.LogError("未传入出库单号", "ERP.Service.RefuseOutGoodsBill", null);
                return false;
            }
            try
            {
                return _storageManager.RejectStockOutFromWmsRejectOutGoods(no, description);
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 出库单号：{1}", ex.Message, no), "ERP.Service.RefuseOutGoodsBill", ex);
                return false;
            }
        }

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime)
        {
            return _salesGoodsRanking.GetAllRealGoodsSaleNumber(startTime, endTime);
        }

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品日均销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<GoodsAvgDaySalesInfo> GetAvgRealGoodsSaleNumber(DateTime startTime, DateTime endTime)
        {
            return _salesGoodsRanking.GetAvgRealGoodsSaleNumber(startTime, endTime);
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
        /// ww 2016-06-28
        public int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId, Guid salePlatformId)
        {
            if (realGoodsId == Guid.Empty || salePlatformId == Guid.Empty)
            {
                return 0;
            }
            return _salesGoodsRanking.GetRealGoodsSaleNumber(startTime, endTime, realGoodsId, salePlatformId);
        }

        /// <summary>
        /// 根据订单号，发票状态，订单是否完成发货，发票是否提交，开始时间，截止时间获取发票集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit,
            DateTime fromTime, DateTime toTime)
        {
            return _invoiceDao.GetInvoiceList(orderNo, invoiceState, isFinished, isCommit, fromTime, toTime);
        }

        /// <summary>
        /// 根据发票ID获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleInvoiceDetailInfo GetInvoice(Guid invoiceId)
        {
            return invoiceId == Guid.Empty ? null : _invoiceDao.GetInvoiceInfo(invoiceId);
        }

        /// <summary>
        /// 根据发票号码获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleInvoiceDetailInfo GetInvoice(long invoiceNo)
        {
            return _invoiceDao.GetInvoiceInfo(invoiceNo);
        }

        /// <summary>
        /// 根据订单号获取订单简单信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleGoodsOrderInfo GetOrderBasic(string orderNo)
        {
            return string.IsNullOrEmpty(orderNo) ? null : _goodsOrderWrite.GetOrderBasic(orderNo);
        }

        /// <summary>
        /// 获取发票品名的名称集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public List<string> GetInvoiceItem()
        {
            return _invoiceDao.GetInvoiceItem();
        }

        /// <summary>
        /// 根据订单号更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public bool UpdateInvoiceStateWithInvoiceNo(string orderNo, byte invoiceState, long invoiceno,
            string invoicecode)
        {
            try
            {
                var invoiceId = _invoiceDao.GetInvoiceIdByOrderNo(orderNo);
                if (invoiceId != Guid.Empty)
                {
                    var result = _invoiceDao.UpdateInvoiceStateWithInvoiceNo(invoiceId, invoiceState, invoiceno, invoicecode);
                    if (!result)
                    {
                        SAL.LogCenter.LogService.LogError(string.Format("更新失败. 订单号：{0}", orderNo), "ERP.Service.UpdateInvoiceStateWithInvoiceNo", null);
                    }
                    return result;
                }
                else
                {
                    SAL.LogCenter.LogService.LogError(string.Format("按订单号未找到发票ID. 订单号：{0}", orderNo), "ERP.Service.UpdateInvoiceStateWithInvoiceNo", null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError(string.Format("{0}. 订单号：{1}", ex.Message, orderNo), "ERP.Service.UpdateInvoiceStateWithInvoiceNo", ex);
                return false;
            }
        }

        /// <summary>
        /// 根据参数发票卷开始和截止号码获取（返回模型字段：发票卷代码，发票卷所属公司ID，当前发票卷使用到的最大发票号）。
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public InvoiceRollInfo GetInvoiceCodeAndCurInvoiceNo(long invoiceStartNo, long invoiceEndNo)
        {
            var info = _invoiceDao.GetInvoiceRollByStartNoandEndNo(invoiceStartNo, invoiceEndNo);
            long maxInvoiceNo = _invoiceDao.GetInvoiceMaxInvoiceNoByInvoiceNo(invoiceStartNo, invoiceEndNo);
            if (info == null)
            {
                return null;
            }
            var invoiceRollInfo = new InvoiceRollInfo
            {
                InvoiceCode = info.InvoiceCode,
                FilialeId = info.FilialeId,
                MaxInvoiceNo = maxInvoiceNo
            };
            return invoiceRollInfo;
        }

        /// <summary>
        /// 根据发票ID获取发票打印所属数据
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId)
        {
            return invoiceId == Guid.Empty ? null : _invoiceDao.GetInvoicePrintData(invoiceId);
        }

        /// <summary>
        /// 根据订单号 获取订单信息 如果是货到付款的则返回金额，如果不是则返回默认值
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice)
        {
            return _goodsOrderWrite.GetOrderCodRealTotalPrice(orderNo, out realtotalPrice);
        }

        /// <summary>
        /// 根据订单号集合，是否打印发票来获取订单信息。
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="isPrintInvoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public List<GoodsOrderInvoiceInfo> GetOrder(List<string> orderNos, bool isPrintInvoice)
        {
            if (orderNos.Count <= 0)
            {
                return null;
            }
            var goodsOrderInvoiceInfo = _goodsOrderWrite.GetGoodsOrderInfoByorderNos(orderNos);
            if (goodsOrderInvoiceInfo.Count <= 0)
            {
                return null;
            }
            foreach (var info in goodsOrderInvoiceInfo)
            {
                info.GoodsOrderDetailList = _goodsOrderDetailWrite.GetGoodsOrderDetailsByOrderId(info.OrderId);
                info.SimpleInvoiceList = isPrintInvoice ? _invoiceDao.GetInvoiceByOrderId(info.OrderId) : null;
            }
            return goodsOrderInvoiceInfo;
        }


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
        public SimpleInvoiceInfo ChInvoice(Guid invoiceId, string invoiceChCode, long invoiceChNo)
        {
            if (invoiceId == Guid.Empty)
            {
                SAL.LogCenter.LogService.LogError("未传入发票ID", "ERP.Service.ChInvoice", null);
                return null;
            }
            //1.根据原发票ID查询发票信息
            var invoiceInfo = invoiceId == Guid.Empty
                ? null
                : _invoiceDao.GetInvoiceByInvoiceId(invoiceId);
            if (invoiceInfo == null)
            {
                return null;
            }

            //2.根据原发票ID获得订单ID和订单号
            var goodsOrderinfo = _goodsOrderWrite.GetGoodsOrderByInvoiceId(invoiceId);
            if (goodsOrderinfo == null) return null;

            var newInvoiceId = Guid.NewGuid();
            invoiceInfo.InvoiceId = newInvoiceId;
            invoiceInfo.InvoiceSum = invoiceInfo.InvoiceSum - (invoiceInfo.InvoiceSum * 2);
            invoiceInfo.InvoiceCode = invoiceChCode;
            invoiceInfo.InvoiceNo = invoiceChNo;
            invoiceInfo.RequestTime = DateTime.Now;
            invoiceInfo.AcceptedTime = DateTime.Now;

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    //3.增加一条新的发票信息
                    var insertInvoiceresult = _invoiceDao.InsertInvoice(invoiceInfo);
                    if (insertInvoiceresult)
                    {
                        //4.添加发票订单关系表
                        var insertOrderInvoiceresult = _invoiceDao.InsertOrderInvoice(newInvoiceId, goodsOrderinfo.OrderId, goodsOrderinfo.OrderNo);
                        if (insertOrderInvoiceresult)
                        {
                            //5.修改原发票InvoiceChCode和InvoiceChNo字段
                            _invoiceDao.UpdateInvoiceChCodeAndInvoiceChNoByinvoiceId(invoiceId, invoiceChCode, invoiceChNo);
                        }
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    SAL.LogCenter.LogService.LogError(ex.Message, "ERP.Service.ChInvoice", ex);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //释放资源
                    ts.Dispose();
                }
            }
            return _invoiceDao.GetInvoicePrintData(invoiceId);
        }


        /// <summary>
        /// 新增发票信息并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        ///  For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public SimpleInvoiceInfo AddInvoice(AddInvoiceInfo invoiceAdd)
        {
            if (invoiceAdd == null)
            {
                return null;
            }
            var goodsOrderInfo = _goodsOrderWrite.GetGoodsOrder(invoiceAdd.OrderId);

            if (goodsOrderInfo == null) return null;

            var info = new InvoiceInfo();
            var invoiceId = Guid.NewGuid();
            info.InvoiceId = invoiceId;
            info.MemberId = goodsOrderInfo.MemberId;
            info.InvoiceName = invoiceAdd.InvoiceName;
            info.InvoiceContent = invoiceAdd.InvoiceContent;
            info.Receiver = goodsOrderInfo.Consignee;
            info.PostalCode = invoiceAdd.PostalCode;
            info.Address = invoiceAdd.Address;
            info.RequestTime = DateTime.Now;
            info.InvoiceSum = (double)goodsOrderInfo.RealTotalPrice;
            info.InvoiceState = 2;
            info.AcceptedTime = DateTime.Now;
            info.PurchaserType = invoiceAdd.IsPurchaserType
                ? InvoicePurchaserType.Individual
                : InvoicePurchaserType.None;
            info.InvoiceCode = invoiceAdd.InvoiceCode;
            info.InvoiceNo = invoiceAdd.InvoiceNo;
            info.IsCommit = false;
            info.NoteType = InvoiceNoteType.Effective;
            info.SaleFilialeId = goodsOrderInfo.SaleFilialeId;
            info.SalePlatformId = goodsOrderInfo.SalePlatformId;
            info.IsShopInvoice = false;



            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    //增加一条发票信息
                    var insertInvoiceresult = _invoiceDao.InsertInvoice(info);
                    if (insertInvoiceresult)
                    {
                        //添加发票订单关系表
                        _invoiceDao.InsertOrderInvoice(invoiceId, goodsOrderInfo.OrderId,
                            goodsOrderInfo.OrderNo);
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //释放资源
                    ts.Dispose();
                }
            }
            return _invoiceDao.GetInvoicePrintData(invoiceId);
        }

        /// <summary>
        /// 重开发票或重开并作废原发票并返回发票打印所属数据
        /// </summary>
        /// <param name="invoiceAdd"></param>
        /// <param name="isCancelOriginalInvoice">是否作废原发票</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public SimpleInvoiceInfo AgainInvoice(AgainInvoiceInfo invoiceAdd, bool isCancelOriginalInvoice)
        {
            if (invoiceAdd == null)
            {
                return null;
            }
            //1.根据原发票ID查询发票信息
            var invoiceInfo = invoiceAdd.InvoiceId == Guid.Empty
                ? null
                : _invoiceDao.GetInvoiceByInvoiceId(invoiceAdd.InvoiceId);
            if (invoiceInfo == null) return null;

            //2.根据原发票ID获得订单ID和订单号
            var goodsOrderinfo = _goodsOrderWrite.GetGoodsOrderByInvoiceId(invoiceAdd.InvoiceId);
            if (goodsOrderinfo == null) return null;

            var invoiceId = Guid.NewGuid();
            invoiceInfo.InvoiceId = invoiceId;
            invoiceInfo.Address = invoiceAdd.Address;
            invoiceInfo.PostalCode = invoiceAdd.PostalCode;
            invoiceInfo.InvoiceName = invoiceAdd.InvoiceName;
            invoiceInfo.InvoiceContent = invoiceAdd.InvoiceContent;
            invoiceInfo.InvoiceCode = invoiceAdd.InvoiceCode;
            invoiceInfo.InvoiceNo = invoiceAdd.InvoiceNo;
            invoiceInfo.RequestTime = DateTime.Now;
            invoiceInfo.AcceptedTime = DateTime.Now;

            //发票状态
            var invoiceState = (InvoiceState)invoiceInfo.InvoiceState;
            //是否作废原发票
            if (isCancelOriginalInvoice)
            {
                if (invoiceInfo.InvoiceState == (int)InvoiceState.Request)
                {
                    invoiceState = InvoiceState.Cancel;
                }
                else if (invoiceInfo.InvoiceState == (int)InvoiceState.Success)
                {
                    invoiceState = InvoiceState.WasteRequest;
                }
            }


            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    //3.增加一条新的发票信息
                    var insertInvoiceresult = _invoiceDao.InsertInvoice(invoiceInfo);
                    if (insertInvoiceresult)
                    {
                        //4.插入订单发票关系表  
                        var insertOrderInvoiceresult = _invoiceDao.InsertOrderInvoice(invoiceId, goodsOrderinfo.OrderId,
                            goodsOrderinfo.OrderNo);
                        if (insertOrderInvoiceresult)
                        {
                            if (isCancelOriginalInvoice)
                            {
                                //作废原发票
                                _invoiceDao.UpdateInvoiceStateByinvoiceId(invoiceAdd.InvoiceId, invoiceState);
                            }
                        }
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //释放资源
                    ts.Dispose();
                }
            }

            return _invoiceDao.GetInvoicePrintData(invoiceId);
        }


        /// <summary>
        /// 根据发票id和起始号更新发票卷为启用
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-04
        public bool UpdateInvoiceRollStateInvocation(Guid rollId, long startNo)
        {
            return _invoiceDao.UpdateInvoiceStateByinvoiceId(rollId, startNo, (int)InvoiceRollState.Invocation);
        }

        /// <summary>入库单完成
        /// </summary>
        /// <param name="inTradeCode"></param>
        /// <param name="description"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>  
        public Boolean FinishStorageRecordIn(String inTradeCode, String description, Dictionary<Guid, int> stockQuantitys)
        {
            var storageRecordInfo = _storageRecordDao.GetStorageRecord(inTradeCode);
            if (storageRecordInfo == null)
            {
                return false;
            }
            if (storageRecordInfo.StockState == (int)StorageRecordState.Finished)
            {
                return true;
            }
            var storageRecordDetailList = _storageManager.GetStorageRecordDetailListByStockId(storageRecordInfo.StockId);
            if (storageRecordDetailList.Count == 0)
            {
                return false;
            }
            else
            {
                //当一个单据多次修改时，单据总额≠Sum(UnitPrice * Quantity)，故做此处理
                storageRecordInfo.AccountReceivable = Math.Round(storageRecordDetailList.Sum(p => p.UnitPrice * p.Quantity), 2);
            }

            #region 往来账处理
            var accountReceivable = -storageRecordInfo.AccountReceivable;
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> tuples = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            List<ReckoningInfo> reckoningInfoList = new List<ReckoningInfo>();
            if (accountReceivable != 0)
            {
                var isShop = storageRecordInfo.RelevanceFilialeId != Guid.Empty && FilialeManager.IsEntityShopFiliale(storageRecordInfo.RelevanceFilialeId); //门店要货申请
                if (!isShop)
                {
                    var isChecked = (int)CheckType.NotCheck;

                    #region --> 采购入库  对账

                    if (storageRecordInfo.StockType == (int)StorageRecordType.BuyStockIn)
                    {
                        //按采购单付款生成的往来账收款
                        var totalAccountReceivable = _reckoning.GetTotalledAccountReceivable(storageRecordInfo.FilialeId, storageRecordInfo.ThirdCompanyID,
                                storageRecordInfo.LinkTradeCode, (int)CheckType.IsChecked, (int)ReckoningType.Income,
                                new List<int>
                                {
                                    (int) ReckoningLinkTradeType.PurchasingNo,
                                    (int) ReckoningLinkTradeType.CompanyFundReceipt
                                });

                        if (totalAccountReceivable != 0)
                        {
                            //已对账的成功对账的入库往来账
                            decimal stockInSum = _reckoning.GetTotalledAccountReceivableByLinkTradeId(storageRecordInfo.FilialeId, storageRecordInfo.ThirdCompanyID,
                                            storageRecordInfo.LinkTradeID, (int)CheckType.IsChecked, (int)ReckoningType.Defray,
                                            new List<int> { (int)ReckoningLinkTradeType.StockIn });

                            var userSum = Math.Abs(totalAccountReceivable) - Math.Abs(stockInSum);
                            if (userSum > 0)
                            {
                                if (userSum >= Math.Abs(accountReceivable))
                                {
                                    isChecked = (int)CheckType.IsChecked;
                                }
                                else
                                {
                                    reckoningInfoList.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId,
                                        storageRecordInfo.ThirdCompanyID, _codeManager.GetCode(CodeType.PY),
                                        storageRecordInfo.Description + description, -userSum,
                                        (int)ReckoningType.Defray, (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked, (int)AuditingState.Yes, storageRecordInfo.TradeCode,
                                        storageRecordInfo.WarehouseId)
                                    {
                                        LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                                        IsOut = storageRecordInfo.IsOut,
                                        IsAllow = true
                                    });
                                    accountReceivable += userSum;
                                }
                            }
                        }
                    }

                    #endregion

                    if (accountReceivable != 0)
                    {
                        reckoningInfoList.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId,
                        storageRecordInfo.ThirdCompanyID,
                        _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                        accountReceivable, (int)ReckoningType.Defray,
                        (int)ReckoningStateType.Currently,
                        isChecked, (int)AuditingState.Yes,
                        storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                        {
                            LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                            IsOut = storageRecordInfo.IsOut,
                            IsAllow = true
                        });
                    }
                }
                else
                {
                    var goodsIds = storageRecordDetailList.Select(ent => ent.GoodsId).Distinct().ToList();
                    var goodsInfos = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
                    if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
                    {
                        ERP.SAL.LogCenter.LogService.LogError("GMS获取商品信息列表", "入库单完成", new Exception(""));
                        return false;
                    }

                    Guid saleFilialeId = storageRecordInfo.FilialeId;
                    Guid hostingCompanyId = storageRecordInfo.ThirdCompanyID;
                    var isDirect = storageRecordInfo.ThirdCompanyID == storageRecordInfo.RelevanceFilialeId;
                    if (!isDirect)
                    {
                        //销售公司销售给门店
                        saleFilialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(storageRecordInfo.ThirdCompanyID);
                        if (saleFilialeId == Guid.Empty)
                        {
                            ERP.SAL.LogCenter.LogService.LogError("往来单位未找到对应的关联公司", "入库单完成", new Exception(""));
                            return false;
                        }
                        hostingCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(storageRecordInfo.FilialeId);
                        if (hostingCompanyId == Guid.Empty)
                        {
                            ERP.SAL.LogCenter.LogService.LogError("物流配送公司未关联往来单位", "入库单完成", new Exception(""));
                            return false;
                        }

                        var joinPrices = goodsInfos.ToDictionary(k => k.GoodsId, v => v.ExpandInfo.JoinPrice);

                        tuples = _storageManager.CreateShopReturnIn(storageRecordInfo, storageRecordDetailList, saleFilialeId, hostingCompanyId, joinPrices, "WMS同步");
                        
                    }

                    var exchangeInfo = _shopExchangedApply.GetShopExchangedApplyInfo(storageRecordInfo.LinkTradeID);
                    if (exchangeInfo != null && !exchangeInfo.IsBarter)
                    {
                        if (Math.Abs(storageRecordInfo.AccountReceivable) > 0)
                        {
                            if (!isDirect)
                            {
                                //如果门店退货、添加往来帐  销售公司对物流公司应收款  物流公司对销售公司应付款
                                reckoningInfoList.Add(new ReckoningInfo(Guid.NewGuid(), saleFilialeId, hostingCompanyId,
                                                             _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                                                             Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Income,
                                                             (int)ReckoningStateType.Currently, (int)CheckType.NotCheck
                                                             , (int)AuditingState.Yes,
                                                             storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                                {
                                    LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                                    IsOut = storageRecordInfo.IsOut
                                });
                            }

                            reckoningInfoList.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId, storageRecordInfo.ThirdCompanyID,
                                                         _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                                                         -Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Defray,
                                                         (int)ReckoningStateType.Currently,
                                                         (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                         storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                            {
                                LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                                IsOut = storageRecordInfo.IsOut
                            });
                        }
                    }
                }
            }
            #endregion

            #region 如果是采购进货单据记录最后一次进货价格
            var goodsPurchaseLastPriceInfos = new List<GoodsPurchaseLastPriceInfo>();
            if (storageRecordInfo.StockType == (int)StorageRecordType.BuyStockIn)
            {
                goodsPurchaseLastPriceInfos.AddRange(from info in storageRecordDetailList
                                                     where info.UnitPrice > 0
                                                     select new GoodsPurchaseLastPriceInfo
                                                     {
                                                         Id = Guid.NewGuid(),
                                                         GoodsId = info.GoodsId,
                                                         RealGoodsId = info.RealGoodsId,
                                                         ThirdCompanyId = storageRecordInfo.ThirdCompanyID,
                                                         WarehouseId = storageRecordInfo.WarehouseId,
                                                         UnitPrice = info.UnitPrice,
                                                         LastPriceDate = DateTime.Now
                                                     });
            }
            #endregion

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    string errorMessage;
                    bool result;
                    if (reckoningInfoList.Count > 0)
                    {
                        foreach (var info in reckoningInfoList)
                        {
                            result = _reckoning.Insert(info, out errorMessage);
                            if (!result)
                            {
                                ERP.SAL.LogCenter.LogService.LogError("插入往来账错误", "入库单完成", new Exception(errorMessage));
                                return false;
                            }
                        }
                    }

                    foreach (var tuple in tuples)
                    {
                        result = _storageRecordDao.Insert(tuple.Item1, tuple.Item2, out errorMessage);
                        if (!result)
                        {
                            ERP.SAL.LogCenter.LogService.LogError("插入出入库单据错误", "入库单完成", new Exception(errorMessage));
                            return false;
                        }
                    }
                    foreach (var goodsPurchaseLastPriceInfo in goodsPurchaseLastPriceInfos)
                    {
                        result = _storageRecordDao.InsertLastPrice(goodsPurchaseLastPriceInfo);
                        if (!result)
                        {
                            ERP.SAL.LogCenter.LogService.LogError("新增或修改最后一次进货价", "入库单完成", new Exception(""));
                            return false;
                        }
                    }
                    result = _storageManager.SetStorageRecordToFinished(storageRecordInfo.StockId, StorageRecordState.Finished, storageRecordInfo.AccountReceivable, description, stockQuantitys);
                    if (!result)
                    {
                        ERP.SAL.LogCenter.LogService.LogError("设置入库单状态为“完成”", "入库单完成", new Exception(""));
                        return false;
                    }

                    #region 判断是否采购进货，修改采购单状态为完成或部分完成

                    if (storageRecordInfo.StockType == (int)StorageRecordType.BuyStockIn || storageRecordInfo.StockType == (int)StorageRecordType.InnerPurchase)
                    {
                        //借记单完成
                        var purchasingId = _debitNote.GetPurchasingIdByNewPurchasingId(storageRecordInfo.LinkTradeID);
                        var flag = UpdatePurchasingDetail(storageRecordInfo, storageRecordDetailList, purchasingId != Guid.Empty ? _debitNote.GetDebitNoteDetailList(purchasingId) : null);

                        result = flag ? _purchasingRead.PurchasingUpdateStateByPartComplete(storageRecordInfo.LinkTradeID, PurchasingState.PartComplete) :
                                 _purchasingRead.PurchasingUpdateStateByAllComplete(storageRecordInfo.LinkTradeID, PurchasingState.AllComplete);

                        if (purchasingId != Guid.Empty)
                        {
                            if (flag)
                            {
                                _debitNote.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.PartComplete);
                            }
                            else
                            {
                                _debitNote.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.AllComplete);
                            }
                        }

                        if (!result)
                        {
                            ERP.SAL.LogCenter.LogService.LogError("修改采购单状态为“完成”或“部分完成”", "入库单完成", new Exception(""));
                            return false;
                        }

                        #region 采购进货入库的，计算一次结算价
                        var items = _grossSettlementManager.CreateByPurchaseStockIn(storageRecordInfo.StockId, DateTime.Now).ToList();
                        items.ForEach(m => _grossSettlementManager.Calculate(m));
                        #endregion
                    }
                    #endregion

                    #region 销售公司采购退货出库给物流配送公司，计算一次结算价
                    foreach (var tuple in tuples)
                    {
                        if (tuple.Item1.StockType == (int)StorageRecordType.BuyStockOut)
                        {
                            var items = _grossSettlementManager.CreateByPurchaseStockIn(tuple.Item1.StockId, DateTime.Now).ToList();
                            items.ForEach(m => _grossSettlementManager.Calculate(m));
                        }
                    }
                    #endregion

                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("捕获异常", "入库单完成", new Exception(ex.Message));
                    return false;
                }
            }
        }

        /// <summary>
        /// 入库完成时修改采购单明细完成状态
        /// </summary>
        /// <param name="storageRecordInfo">入库信息</param>
        /// <param name="storageRecordDetailList">入库详细</param>
        /// <param name="debitNoteDetailInfos">借计单明细</param>
        /// <returns>是否为部分完成</returns>
        private bool UpdatePurchasingDetail(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList, IList<DebitNoteDetailInfo> debitNoteDetailInfos)
        {
            IList<PurchasingDetailInfo> pList = _purchasingDetail.Select(storageRecordInfo.LinkTradeID);
            bool flag = false;
            foreach (var gsInfo in pList)
            {
                if (gsInfo.PurchasingGoodsType == (int)PurchasingGoodsType.Gift)
                {
                    var storageRecordDetailInfo = storageRecordDetailList.FirstOrDefault(p => p.RealGoodsId == gsInfo.GoodsID && p.UnitPrice == 0);
                    if (storageRecordDetailInfo != null)
                    {
                        var debitDetailInfo = debitNoteDetailInfos != null && debitNoteDetailInfos.Count > 0
                            ? debitNoteDetailInfos.FirstOrDefault(act => act.GoodsId == gsInfo.GoodsID)
                            : null;
                        if (gsInfo.PlanQuantity - gsInfo.RealityQuantity > Convert.ToInt32(storageRecordDetailInfo.Quantity) && gsInfo.State == (int)YesOrNo.No)
                        {
                            flag = true;
                            if (debitDetailInfo != null && debitDetailInfo.State == (int)YesOrNo.No)
                            {
                                _debitNote.UpdateDebitNoteDetail(debitDetailInfo.PurchasingId, debitDetailInfo.GoodsId, (int)YesOrNo.No, debitDetailInfo.ArrivalCount + storageRecordDetailInfo.Quantity, true);
                            }
                        }
                        else
                        {
                            if (debitDetailInfo != null && debitDetailInfo.State == (int)YesOrNo.No)
                            {
                                _debitNote.UpdateDebitNoteDetail(debitDetailInfo.PurchasingId, debitDetailInfo.GoodsId, (int)YesOrNo.Yes, debitDetailInfo.GivingCount, true);
                            }
                            _purchasingDetail.UpdateGoodState(YesOrNo.Yes, gsInfo.PurchasingGoodsID);
                        }
                        gsInfo.RealityQuantity = gsInfo.RealityQuantity + Convert.ToDouble(storageRecordDetailInfo.Quantity);
                        _purchasingDetail.UpdateRealQuantity(gsInfo, gsInfo.PurchasingGoodsID);
                    }
                    else
                    {
                        if (gsInfo.PlanQuantity - gsInfo.RealityQuantity > 0 && gsInfo.State == (int)YesOrNo.No)
                        {
                            flag = true;
                        }
                        else
                        {
                            _purchasingDetail.UpdateGoodState(YesOrNo.Yes, gsInfo.PurchasingGoodsID);
                        }
                    }
                }
                else
                {
                    var storageRecordDetailInfo = storageRecordDetailList.FirstOrDefault(p => p.RealGoodsId == gsInfo.GoodsID && p.UnitPrice != 0);
                    if (storageRecordDetailInfo != null)
                    {
                        if (gsInfo.PlanQuantity - gsInfo.RealityQuantity > Convert.ToDouble(storageRecordDetailInfo.Quantity) && gsInfo.State == (int)YesOrNo.No)
                        {
                            flag = true;
                        }
                        else
                        {
                            gsInfo.State = (int)YesOrNo.Yes;
                        }
                        gsInfo.Price = storageRecordDetailInfo.UnitPrice; //修改采购单中入库的价格
                        gsInfo.RealityQuantity = gsInfo.RealityQuantity + Convert.ToInt32(storageRecordDetailInfo.Quantity);
                        _purchasingDetail.UpdatePurchasingDetail(gsInfo, gsInfo.PurchasingGoodsID);
                    }
                    else
                    {
                        if (gsInfo.PlanQuantity - gsInfo.RealityQuantity > 0 && gsInfo.State == (int)YesOrNo.No)
                        {
                            flag = true;
                        }
                        else
                        {
                            _purchasingDetail.UpdateGoodState(YesOrNo.Yes, gsInfo.PurchasingGoodsID);
                        }
                    }
                }
            }
            return flag;
        }


        /// <summary>出库单完成
        /// </summary>
        /// <param name="outTradeCode"></param>
        /// <param name="description"></param>
        /// <param name="operatorName"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public Boolean FinishStorageRecordOut(String outTradeCode, String description, string operatorName, Dictionary<Guid, int> stockQuantitys)
        {
            var storageRecordInfo = _storageRecordDao.GetStorageRecord(outTradeCode);
            if (storageRecordInfo == null)
                return false;
            if (storageRecordInfo.StockState == (int)StorageRecordState.Finished)
                return true;
            var storageRecordDetailList = _storageManager.GetStorageRecordDetailListByStockId(storageRecordInfo.StockId);
            if (storageRecordDetailList.Count == 0)
            {
                return false;
            }
            else
            {
                //当一个单据多次修改时，单据总额≠Sum(UnitPrice * Quantity)，故做此处理
                storageRecordInfo.AccountReceivable = Math.Round(storageRecordDetailList.Sum(p => p.UnitPrice * p.Quantity), 2);
            }

            #region 往来账处理
            ApplyStockInfo applyStockInfo = null;
            ShopExchangedApplyInfo exchangedApplyInfo = null;
            var accountReceivable = Math.Abs(storageRecordInfo.AccountReceivable);
            var shopInfo = storageRecordInfo.RelevanceFilialeId != Guid.Empty ? FilialeManager.Get(storageRecordInfo.RelevanceFilialeId) : null;
            List<ReckoningInfo> reckoningInfos = new List<ReckoningInfo>();
            if (accountReceivable != 0)
            {
                if (shopInfo==null)
                {
                    reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId, storageRecordInfo.ThirdCompanyID,
                                                     _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                                                     accountReceivable, (int)ReckoningType.Income,
                                                     (int)ReckoningStateType.Currently,
                                                     (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                     storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                    {
                        LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                        IsOut = storageRecordInfo.IsOut
                    });
                }
                else
                {
                    applyStockInfo = _applyStock.GetApplyInfoByTradeCode(storageRecordInfo.LinkTradeCode);
                    if (applyStockInfo == null)
                        exchangedApplyInfo = _shopExchangedApply.GetShopExchangedApplyInfoByApplyNo(storageRecordInfo.LinkTradeCode);
                }
            }

            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> tuples = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            Tuple<StockDTO, IList<StockDetailDTO>> shopTuples =null;
            bool isPurchase = false;//是否为采购入库
            Guid purchaseId=Guid.Empty;//要货单Id
            var isShop = shopInfo != null && FilialeManager.IsShop(shopInfo);
            if (isShop)
            {
                #region 设置要货单Id
                if (applyStockInfo != null && applyStockInfo.ApplyId != Guid.Empty)//采购出库
                {
                    purchaseId = applyStockInfo.ApplyId;
                    isPurchase = true;
                }
                else
                {
                    purchaseId = exchangedApplyInfo != null ? exchangedApplyInfo.ApplyID : Guid.Empty;
                }
                #endregion

                #region 
                var goodsIds = storageRecordDetailList.Select(ent => ent.GoodsId).Distinct().ToList();
                var goodsInfos = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
                {
                    ERP.SAL.LogCenter.LogService.LogError("GMS获取商品信息列表", "出库单完成", new Exception(""));
                    return false;
                }
                var settles = goodsInfos.ToDictionary(k => k.GoodsId, v => v.ExpandInfo.JoinPrice);
                tuples = _storageManager.CreateShopSellOut(storageRecordInfo, storageRecordDetailList, shopInfo.ParentId, settles, "WMS同步");
                if (isPurchase || (exchangedApplyInfo != null && exchangedApplyInfo.IsBarter))
                {
                    if (Math.Abs(storageRecordInfo.AccountReceivable) > 0 && !CreateReckoningInfos(storageRecordInfo,shopInfo,description,reckoningInfos))
                    {
                        return false;
                    }
                }
                #endregion

                #region 门店入库申请
                var shopStockDto = new StockDTO
                {
                    StockID = Guid.NewGuid(),
                    PurchaseID = purchaseId,
                    DateCreated = DateTime.Now,
                    OriginalTradeCode = storageRecordInfo.TradeCode,
                    StockState = (int)StockState.Wait,
                    StockType = (int)StockType.PurchaseStockIn,
                    SubtotalQuantity = Math.Abs((int)storageRecordInfo.SubtotalQuantity),
                    SubtotalPrice = Math.Abs(storageRecordInfo.AccountReceivable),
                    TradeCode = string.Empty, //联盟店生成
                    Transactor = operatorName,
                    Description = "联盟店调拨入库",
                    //入库公司
                    ShopID = storageRecordInfo.RelevanceFilialeId,
                    WarehouseID = storageRecordInfo.RelevanceWarehouseId,
                    //出库公司
                    CompanyID = shopInfo.ParentId,
                    AuditingTime = DateTime.Now
                };

                var shopStockDetailDtoList = storageRecordDetailList.Select(gsi => new StockDetailDTO
                {
                    StockID = shopStockDto.StockID,
                    GoodsID = gsi.GoodsId,
                    RealGoodsID = gsi.RealGoodsId,
                    GoodsName = gsi.GoodsName,
                    Price = gsi.UnitPrice,
                    GoodsCode = gsi.GoodsCode,
                    Quantity = Math.Abs(gsi.Quantity),
                    Specification = gsi.Specification,
                    RealQuantity = 0,
                    NonceGoodsStock = 0
                }).ToList();

                shopTuples=new Tuple<StockDTO, IList<StockDetailDTO>>(shopStockDto, shopStockDetailDtoList);
                #endregion
            }
            #endregion
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    bool result;
                    if (reckoningInfos.Count > 0)
                    {
                        foreach (var reck in reckoningInfos)
                        {
                            string errorMessage;
                            result = _reckoning.Insert(reck, out errorMessage);
                            if (!result)
                            {
                                ERP.SAL.LogCenter.LogService.LogError("插入往来账错误", "出库单完成", new Exception(errorMessage));
                                return false;
                            }
                        }
                    }
                    result = _storageManager.SetStorageRecordToFinished(storageRecordInfo.StockId, StorageRecordState.Finished, storageRecordInfo.AccountReceivable, description, stockQuantitys);
                    if (!result)
                    {
                        ERP.SAL.LogCenter.LogService.LogError("设置出库单状态为“完成”", "出库单完成", new Exception(""));
                        return false;
                    }

                    if (isShop && !AboutShop(isPurchase,purchaseId,applyStockInfo,shopInfo,shopTuples,tuples))
                    {
                        return false;
                    }

                    #region 销售公司向物流配送公司采购进货入库，计算一次结算价
                    foreach (var tuple in tuples)
                    {
                        if (tuple.Item1.StockType == (int)StorageRecordType.BuyStockIn)
                        {
                            var items = _grossSettlementManager.CreateByPurchaseStockIn(tuple.Item1.StockId, DateTime.Now).ToList();
                            items.ForEach(m => _grossSettlementManager.Calculate(m));
                        }
                    }
                    #endregion

                    #region 采购退货出库，计算一次结算价
                    if (storageRecordInfo.StockType == (int)StorageRecordType.BuyStockOut)
                    {
                        var items = _grossSettlementManager.CreateByPurchaseReturnStockOut(storageRecordInfo.StockId, DateTime.Now).ToList();
                        items.ForEach(m => _grossSettlementManager.Calculate(m));
                    }
                    #endregion

                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    ERP.SAL.LogCenter.LogService.LogError("捕获异常", "出库单完成", new Exception(ex.Message));
                    return false;
                }
            }
        }

        /// <summary>
        /// 门店相关数据执行  状态同步 往来帐添加 出入库添加
        /// </summary>
        /// <param name="isPurchase"></param>
        /// <param name="purchaseId"></param>
        /// <param name="applyStockInfo"></param>
        /// <param name="shopInfo"></param>
        /// <param name="shopTuples"></param>
        /// <param name="tuples"></param>
        /// <returns></returns>
        private bool AboutShop(bool isPurchase,Guid purchaseId,ApplyStockInfo applyStockInfo,FilialeInfo shopInfo, Tuple<StockDTO, IList<StockDetailDTO>> shopTuples, List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> tuples)
        {
            string msg;
            if (applyStockInfo != null)
            {
                #region 更新门店要货单状态，同时向门店插入要货申请数据
                var result = _applyStock.UpdateApplyStockState(applyStockInfo.ApplyId, (int)ApplyStockState.Finishing);
                if (result)
                {
                    result = ShopSao.UpdatePurchaseState(shopInfo.ParentId, applyStockInfo.ApplyId, (int)ApplyStockState.Finishing, "", out msg);
                    if (!result)
                    {
                        ERP.SAL.LogCenter.LogService.LogError("设置联门店采购单状态为“等待收货”", "出库单完成", new Exception(msg));
                        return false;
                    }
                }

                foreach (var tuple in tuples)
                {
                    result = _storageRecordDao.Insert(tuple.Item1, tuple.Item2, out msg);
                    if (!result)
                    {
                        ERP.SAL.LogCenter.LogService.LogError("插入出入库单据错误", "出库单完成", new Exception(msg));
                        return false;
                    }
                }
                #endregion
            }
            if (!isPurchase && purchaseId != Guid.Empty) //换货出库
            {
                if (_shopExchangedApply.UpdateExchangeState(purchaseId, (Int32)ExchangedState.Bartering, "换货出库完成") <= 0)
                {
                    ERP.SAL.LogCenter.LogService.LogError("设置联门店退换货申请状态为“换货中”", "出库单完成", new Exception(""));
                    return false;
                }
            }

            //添加门店入库申请记录
            ShopSao.InsertStock(shopInfo.ParentId, shopTuples.Item1, shopTuples.Item2, isPurchase, null, 0, out msg);
            if (!string.IsNullOrEmpty(msg))
            {
                ERP.SAL.LogCenter.LogService.LogError("添加联门店入库申请记录", "出库单完成", new Exception(msg));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 如果：销售公司=物流配送公司 生成销售公司对门店的应收款
        /// 销售公司!=物流配送公司 生成销售公司对门店的销售出库单 物流配送公司对销售公司的应收款 销售公司对门店的应收款
        /// 至于销售公司对物流配送公司的应付款由每日任务自动创建采购单、入库单、应付款往来帐
        /// </summary>
        /// <param name="storageRecordInfo"></param>
        /// <param name="shopInfo"></param>
        /// <param name="description"></param>
        /// <param name="reckoningInfos"></param>
        /// <returns></returns>
        private bool CreateReckoningInfos(StorageRecordInfo storageRecordInfo,FilialeInfo shopInfo,string description, List<ReckoningInfo> reckoningInfos)
        {
            #region //历史数据 thirdCompanyId为门店Id 或者 销售公司直接发货给门店
            if (storageRecordInfo.RelevanceFilialeId == storageRecordInfo.ThirdCompanyID)
            {
                if (storageRecordInfo.FilialeId != shopInfo.ParentId)  //销售公司不等于物流配送公司   反之 销售公司直接发货给门店 
                {
                    var thirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(storageRecordInfo.FilialeId);
                    if (thirdCompanyId == Guid.Empty)
                    {
                        ERP.SAL.LogCenter.LogService.LogError(string.Format("出库单出库公司{0}未关联往来单位", storageRecordInfo.FilialeId), "出库单完成", new Exception(""));
                        return false;
                    }

                    var parentThirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(shopInfo.ParentId);
                    if (parentThirdCompanyId == Guid.Empty)
                    {
                        ERP.SAL.LogCenter.LogService.LogError(string.Format("出库单往来单位{0}未关联公司", shopInfo.ParentId), "出库单完成", new Exception(""));
                        return false;
                    }
                    //物流出给销售公司  
                    reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId,
                        parentThirdCompanyId, _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                        Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Income,
                        (int)ReckoningStateType.Currently, (int)CheckType.NotCheck
                        , (int)AuditingState.Yes,
                        storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                    {
                        LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                        IsOut = storageRecordInfo.IsOut
                    });

                    //销售公司采购物流公司  
                    reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), shopInfo.ParentId,
                        thirdCompanyId, _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                        -Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Defray,
                        (int)ReckoningStateType.Currently, (int)CheckType.NotCheck
                        , (int)AuditingState.Yes,
                        storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                    {
                        LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                        IsOut = storageRecordInfo.IsOut
                    });
                }

                ////销售公司出给门店 
                //reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), shopInfo.ParentId,
                //    storageRecordInfo.ThirdCompanyID, _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                //    Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Income,
                //    (int)ReckoningStateType.Currently, (int)CheckType.NotCheck
                //    , (int)AuditingState.Yes,
                //    storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                //{
                //    LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                //    IsOut = storageRecordInfo.IsOut
                //});
            }
            #endregion

            #region //物流公司对可得应收款  可得对物流公司应付款
            else
            {
                reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), storageRecordInfo.FilialeId,
                    storageRecordInfo.ThirdCompanyID, _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                    Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Income,
                    (int)ReckoningStateType.Currently, (int)CheckType.NotCheck
                    , (int)AuditingState.Yes,
                    storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                {
                    LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                    IsOut = storageRecordInfo.IsOut
                });

                //var filialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(storageRecordInfo.ThirdCompanyID);
                //if (filialeId == Guid.Empty)
                //{
                //    ERP.SAL.LogCenter.LogService.LogError(string.Format("出库单往来单位{0}未关联公司", storageRecordInfo.ThirdCompanyID), "出库单完成", new Exception(""));
                //    return false;
                //}

                //reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), filialeId,
                //    storageRecordInfo.RelevanceFilialeId, _codeManager.GetCode(CodeType.PY), storageRecordInfo.Description + description,
                //    Math.Abs(storageRecordInfo.AccountReceivable), (int)ReckoningType.Income,
                //    (int)ReckoningStateType.Currently,
                //    (int)CheckType.NotCheck, (int)AuditingState.Yes,
                //    storageRecordInfo.TradeCode, storageRecordInfo.WarehouseId)
                //{
                //    LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                //    IsOut = storageRecordInfo.IsOut
                //});
            }
            #endregion
            return true;
        }

        /// <summary>
        /// 更新出入库明细商品当前库存
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public Boolean UpdateNonceWarehouseGoodsStock(String tradeCode, Dictionary<Guid, int> stockQuantitys)
        {
            var storageRecordInfo = _storageRecordDao.GetStorageRecord(tradeCode);
            if (storageRecordInfo != null)
            {
                return _storageRecordDao.UpdateNonCurrentStockByStockId(storageRecordInfo.StockId, stockQuantitys);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="payToKede"></param>
        /// <param name="badMoney"></param>
        /// <returns></returns>
        public bool InsertReckoningForLostBack(string orderNo, decimal payToKede, decimal badMoney)
        {
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var orderInfo = _goodsOrderWrite.GetGoodsOrder(orderNo);
                    if (orderInfo != null)
                    {
                        string errorMessage;
                        if (payToKede != 0)
                        {
                            var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), orderInfo.SaleFilialeId, orderInfo.ExpressId, _codeManager.GetCode(CodeType.GT),
                                                                    "[丢件找回(赔偿后找回)][快递应收减少]", -Math.Abs(payToKede), (int)ReckoningType.Defray,
                                                                    (int)ReckoningStateType.Currently, (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                                    orderInfo.ExpressNo, orderInfo.DeliverWarehouseId)
                            {
                                ReckoningCheckType = (int)ReckoningCheckType.Collection,
                                LinkTradeType = (int)ReckoningLinkTradeType.GoodsOrder,
                                IsOut = orderInfo.IsOut
                            };
                            var result = _reckoning.Insert(reckoningInfo, out errorMessage);
                            if (!result)
                            {
                                return false;
                            }
                        }
                        if (badMoney != 0)
                        {
                            var badReckoningInfo = new ReckoningInfo(Guid.NewGuid(), orderInfo.SaleFilialeId, orderInfo.ExpressId, _codeManager.GetCode(CodeType.PY),
                                                                    "[丢件找回][快递损坏]", Math.Abs(badMoney), (int)ReckoningType.Income,
                                                                    (int)ReckoningStateType.Currently, (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                                    orderInfo.ExpressNo, orderInfo.DeliverWarehouseId)
                            {
                                ReckoningCheckType = (int)ReckoningCheckType.Collection,
                                LinkTradeType = (int)ReckoningLinkTradeType.GoodsOrder,
                                IsOut = orderInfo.IsOut
                            };
                            var result = _reckoning.Insert(badReckoningInfo, out errorMessage);
                            if (!result)
                            {
                                return false;
                            }
                        }
                    }
                    ts.Complete();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IList<SimpleInvoiceDetailInfo> GetInvoiceListByOrderNos(List<string> orderNos)
        {
            var data = new List<SimpleInvoiceDetailInfo>();
            foreach (var orderNo in orderNos)
            {
                data.AddRange(_invoiceDao.GetInvoiceByOrderNo(orderNo));
            }
            return data;
        }

        /// <summary>
        /// 判断入库单对应的往来单位是否被搁置
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public Boolean IsAbeyanced(string tradeCode)
        {
            return _storageRecordDao.IsAbeyancedThirdComapny(tradeCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public WMSResultInfo AddBySellReturnIn(WMSReturnGoodsRequest request)
        {
            List<ReckoningInfo> reckoningInfos;
            string errorMsg;
            var result = _storageManager.AddBySellReturnIn(request.OrderNo, request.BillNo, request.OutGoodsBillNo, request.OperatorName, request.Details, request.StockQuantitys, out reckoningInfos, out errorMsg);
            if (result.Count == 0) return new WMSResultInfo(false, errorMsg);
            bool isSuccess = true;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var tuple in result)
                {
                    isSuccess = _storageRecordDao.Insert(tuple.Item1, tuple.Item2, out errorMsg);
                    if (!isSuccess) break;
                }
                if (isSuccess)
                {
                    foreach (var reckoningInfo in reckoningInfos)
                    {
                        isSuccess = _reckoning.Insert(reckoningInfo, out errorMsg);
                        if (!isSuccess) break;
                    }

                    if (isSuccess)
                    {
                        #region 采购退货出库，计算一次结算价
                        foreach (var stockRecord in result.Where(m => m.Item1.StockType == (int)StorageRecordType.BuyStockOut))
                        {
                            var items = _grossSettlementManager.CreateByPurchaseReturnStockOut(stockRecord.Item1.StockId, DateTime.Now).ToList();
                            items.ForEach(m => _grossSettlementManager.Calculate(m));
                        }
                        #endregion

                        ts.Complete();
                    }
                }
            }
            return new WMSResultInfo(isSuccess, isSuccess ? "" : errorMsg);
        }

        /// <summary>
        /// 取消/作废订单 删除订单相关出库单据
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public WMSResultInfo CancelStorageRecord(string orderNo)
        {
            string message;
            var result = _storageRecordDao.CancelStorageRecordByLinkTradeCode(orderNo, out message);
            if (!result)
                return new WMSResultInfo(false, message);
            return new WMSResultInfo(true, "");
        }

        /// <summary>
        /// 丢件找回商品处理
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public WMSResultInfo AddByLostBackReturnIn(WMSLostBackReturnDTO request)
        {
            List<ReckoningInfo> reckoningInfos;
            string errorMsg;
            var result = _storageManager.AddByLostBackReturnIn(request, out reckoningInfos, out errorMsg);
            if (result.Count == 0) return new WMSResultInfo(false, errorMsg);
            bool isSuccess = true;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var tuple in result)
                {
                    isSuccess = _storageRecordDao.Insert(tuple.Item1, tuple.Item2, out errorMsg);
                    if (!isSuccess) break;
                }
                if (isSuccess)
                {
                    foreach (var reckoningInfo in reckoningInfos)
                    {
                        isSuccess = _reckoning.Insert(reckoningInfo, out errorMsg);
                        if (!isSuccess) break;
                    }

                    if (isSuccess)
                    {
                        #region 采购退货出库，计算一次结算价
                        foreach (var stockRecord in result.Where(m => m.Item1.StockType == (int)StorageRecordType.BuyStockOut))
                        {
                            var items = _grossSettlementManager.CreateByPurchaseReturnStockOut(stockRecord.Item1.StockId, DateTime.Now).ToList();
                            items.ForEach(m => _grossSettlementManager.Calculate(m));
                        }
                        #endregion

                        ts.Complete();
                    }
                }
            }
            return new WMSResultInfo(isSuccess, isSuccess ? "" : errorMsg);
        }

        /// <summary>
        /// 验证商品是否存在采购设置
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseIds"></param>
        /// <returns></returns>
        public List<KeyValuePair<Guid, Guid>> HasPurchseSetGoodsIds(IEnumerable<Guid> goodsIds, IEnumerable<Guid> warehouseIds)
        {
            var result = new List<KeyValuePair<Guid, Guid>>();
            if (goodsIds == null || !goodsIds.Any()
                || warehouseIds == null || !warehouseIds.Any())
            {
                return result;
            }
            var allKeys = Cache.PurchseSet.Instance.GetAllKeys();
            var matchedKeys = allKeys.Where(m => warehouseIds.Contains(m.Key)).ToList();
            matchedKeys.ForEach((key) => {
                var goodsIdsInCache = Cache.PurchseSet.Instance.Get(key);
                bool goodsIdAllExists = true;
                foreach(var goodsId in goodsIds)
                {
                    goodsIdAllExists = goodsIdAllExists && goodsIdsInCache.Contains(goodsId);
                }
                if(goodsIdAllExists)
                {
                    result.Add(key);
                }
            });
            return result;
        }

        /// <summary>
        /// 验证商品是否存在采购设置
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <returns>Key物流公司,Value 存在采购设置的商品列表</returns>
        public Dictionary<Guid,List<Guid>> HasPurchseSetGoodsIdsGroup(IEnumerable<Guid> goodsIds, Guid warehouseId)
        {
            var result = new Dictionary<Guid,List<Guid>>();
            if (goodsIds == null || !goodsIds.Any()
                || warehouseId ==Guid.Empty)
            {
                return result;
            }
            var purchasets=_purchaseSet.GetPurchaseSetList(goodsIds.ToList(), warehouseId);
            if (purchasets.Count>0)
            {
                result = purchasets.GroupBy(ent => ent.HostingFilialeId)
                    .ToDictionary(k => k.Key, v => v.Select(ent => ent.GoodsId).ToList());
            }
            //var allKeys = Cache.PurchseSet.Instance.GetAllKeys();
            //var matchedKeys = allKeys.Where(m => m.Key==warehouseId).ToList();
            //matchedKeys.ForEach((key) => {
            //    var goodsIdsInCache = Cache.PurchseSet.Instance.Get(key);
            //    var existsIds = goodsIds.Where(ent => goodsIdsInCache.Contains(ent)).ToList();
            //    if (existsIds.Count>0)
            //    {
            //        result.Add(key.Value,existsIds);
            //    }
            //});
            return result;
        }


        /// <summary>
        /// 验证商品是否存在采购设置
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseIds"></param>
        /// <returns></returns>
        public Dictionary<KeyValuePair<Guid, Guid>,List<Guid>> HasPurchseSetGoodsIdssss()
        {
            return _purchaseSet.GetKeyAndValueGuids();
        }

        /// <summary>
        /// 拆分组合更新结算价
        /// </summary>
        /// <param name="isSplit"></param>
        /// <param name="billNo"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="combineSplitGoodsId"></param>
        /// <param name="combineSplitGoodsQuantity"></param>
        /// <param name="combineSplitDetailGoodsIds"></param>
        /// <param name="occerTime"></param>
        /// <param name="stockQuantitys"></param>
        /// <returns></returns>
        public WMSResultInfo CreateByCombineSplit(bool isSplit, string billNo, Guid hostingFilialeId, Guid combineSplitGoodsId, int combineSplitGoodsQuantity,
            Dictionary<Guid, int> combineSplitDetailGoodsIds, DateTime occerTime, Dictionary<Guid, int> stockQuantitys)
        {
            var details = _grossSettlementManager.CreateByCombineSplit(new CombineSplitBillDTO
            {
                BillNo = billNo,
                GoodsId = combineSplitGoodsId,
                Quantity = combineSplitGoodsQuantity,
                HostingFilialeId = hostingFilialeId,
                IsSplit = isSplit,
                Details = combineSplitDetailGoodsIds.Count > 0 ? combineSplitDetailGoodsIds.Select(ent => new CombineSplitBillDetailDTO
                {
                    GoodsId = ent.Key,
                    Quantity = ent.Value
                }).ToList() : new List<CombineSplitBillDetailDTO>()
            }, occerTime);
            if (!details.Any())
                return new WMSResultInfo(false, "拆分组合添加结算价计算任务失败");
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var item in details)
                    {
                        _grossSettlementManager.Calculate(item);
                    }
                    ts.Complete();
                }
                return new WMSResultInfo(true, "");
            }
            catch (Exception ex)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}，拆分组合单号：{1}", ex.Message, billNo), "ERP.Service.CreateByCombineSplit", ex);
                return new WMSResultInfo(false, "数据库保存失败");
            }
        }
    }
}
