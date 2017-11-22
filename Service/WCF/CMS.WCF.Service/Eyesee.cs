using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using System.Xml;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Order;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Goods;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IGoods;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.Model.Invoice;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Framework;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.ShopFront;
using Newtonsoft.Json;
using CompanyClass = ERP.DAL.Implement.Inventory.CompanyClass;
using GoodsInfo = ERP.Model.Goods.GoodsInfo;
using GoodsOrder = ERP.DAL.Implement.Order.GoodsOrder;
using GoodsOrderDetail = ERP.DAL.Implement.Order.GoodsOrderDetail;
using Invoice = ERP.DAL.Implement.Inventory.Invoice;
using ReckoningManager = ERP.BLL.Implement.Inventory.ReckoningManager;
using ERP.Model.SubsidyPayment;
using ERP.Model.RefundsMoney;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Implement.FinanceModule;

namespace ERP.Service.Implement
{
    /// <summary>
    ///
    /// </summary>
    public partial class Service
    {
        private static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private static readonly ICostReport _costReport = InventoryInstance.GetCostReportDao(GlobalConfig.DB.FromType.Write);
        private static readonly IGoodsOrder _goodsOrderWrite = new GoodsOrder(GlobalConfig.DB.FromType.Write);
        private static readonly BLL.Implement.Order.GoodsOrder _goodsOrder = new BLL.Implement.Order.GoodsOrder(GlobalConfig.DB.FromType.Write);
        private static readonly IGoodsOrderDetail _goodsOrderDetailWrite = new GoodsOrderDetail(GlobalConfig.DB.FromType.Write);
        private static readonly IWasteBook _wasteBookDaoWrite = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private static readonly OrderManager _orderManager = OrderManager.WriteInstance;
        private static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasing _purchasingRead = new Purchasing(GlobalConfig.DB.FromType.Write);
        private static readonly BankAccountManager _bankAccountManager = BankAccountManager.WriteInstance;
        private static readonly IWasteBookCheck _wasteBookCheckDao = new WasteBookCheck(GlobalConfig.DB.FromType.Write);
        private static readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyClass _companyClass = new CompanyClass();
        private static readonly IInvoice _invoiceDao = new Invoice(GlobalConfig.DB.FromType.Write);
        private static readonly IPaymentNotice _paymentNotice = new PaymentNotice(GlobalConfig.DB.FromType.Write);
        private static readonly IGoodsDemand _goodsDemandDao = new GoodsDemand(GlobalConfig.DB.FromType.Write);
        private static readonly ISalesGoodsRanking _salesGoodsRanking = new SalesGoodsRanking(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private static readonly IDebitNote _debitNote = new DebitNote(GlobalConfig.DB.FromType.Write);
        private static readonly RealTimeGrossSettlementManager _grossSettlementManager = new RealTimeGrossSettlementManager(GlobalConfig.DB.FromType.Write);
        private static readonly ITaxrateProportion _taxrateProportion = new TaxrateProportionDao();
        private static readonly IVocabulary _vocabulary = new VocabularyDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyCussentRelation _companyCussentRelation = new CompanyCussentRelation(GlobalConfig.DB.FromType.Write);
        private static readonly InvoiceApplySerivce _invoiceApplySerivce = new InvoiceApplySerivce();
        private static readonly IRefundsMoneyDal _refundsMoneyDal = new RefundsMoneyDal();
        private static readonly ISubsidyPaymentDal _subsidyPaymentDal = new SubsidyPaymentDal();

        #region[插入一条资金流数据]

        /// <summary>
        /// 插入一条资金流数据
        /// </summary>
        /// <param name="wasteBookInfo">资金流参数</param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        public bool InsertWastebook(Guid pushDataId, WasteBookInfo wasteBookInfo)
        {
            string tradeCode = _codeManager.GetCode(CodeType.VI);
            if (string.IsNullOrEmpty(tradeCode)) return false;
            wasteBookInfo.TradeCode = tradeCode;

            var result = _wasteBookDaoWrite.Insert(wasteBookInfo);
            if (result == -1 || result == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入一条资金流数据
        /// </summary>
        /// <param name="wasteBookInfo">资金流参数</param>
        /// <param name="pushDataId"></param>
        /// <returns></returns>
        /// zal 2016-06-15
        public bool InsertWastebookForB2C(Guid pushDataId, WasteBookInfo wasteBookInfo)
        {
            string tradeCode = _codeManager.GetCode(CodeType.VI);
            if (string.IsNullOrEmpty(tradeCode)) return false;
            wasteBookInfo.TradeCode = tradeCode;

            if (wasteBookInfo.WasteSource.Equals((int)WasteSource.OrderBusinessMoney))
            {
                var exist = GetWasteBookByLinkTradeCodeAndWasteSource(wasteBookInfo.LinkTradeCode, (int)WasteSource.OrderBusinessMoney);
                if (exist == null)
                {
                    var result = _wasteBookDaoWrite.Insert(wasteBookInfo);
                    if (result == -1 || result == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var result = _wasteBookDaoWrite.Insert(wasteBookInfo);
                if (result == -1 || result == 1)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region[设置商品上下架]

        /// <summary>
        /// 设置商品上下架
        /// Add by liucaijun at 2011-March-25th
        /// </summary>
        /// <param name="pushDataId">命令ID</param>
        /// <param name="goodsList">商品列表</param>
        /// <param name="state">上下架状态(0下架，1上架)</param>
        /// <param name="personnelId"> </param>
        public WCFReturnInfo SetGoodsState(Guid personnelId, Guid pushDataId, List<Guid> goodsList, int state)
        {
            return Execute(pushDataId, () =>
                                           {
                                               foreach (Guid goodsId in goodsList)
                                               {
                                                   string errorMessage;
                                                   _goodsCenterSao.SetPurchaseState(goodsId, state == 1, "服务下架", personnelId, out errorMessage);
                                               }
                                           });
        }

        #endregion

        #region[设置商品是否缺货]

        /// <summary>
        /// 设置商品是否缺货
        /// </summary>
        /// <param name="pushDataId">命令ID</param>
        /// <param name="realGoodsList">商品列表</param>
        /// <param name="state">缺货</param>
        public WCFReturnInfo SetGoodsIsScarcityState(Guid pushDataId, List<Guid> realGoodsList, int state)
        {
            return Execute(pushDataId, () =>
            {
                foreach (Guid goodsId in realGoodsList)
                {
                    _goodsCenterSao.SetRealGoodsIsScarcity(goodsId, state == 1, "服务自动", Guid.Empty);
                }
            });
        }

        #endregion

        /// <summary> 根据商品标识获取商品上下架状态 add jiang 2011-08-01
        /// </summary>
        /// <param name="goodsid"></param>
        /// <returns></returns>
        public int GetGoodsState(Guid goodsid)
        {
            var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsid);
            return goodsInfo != null && goodsInfo.IsOnShelf ? 0 : 1;
        }

        /// <summary> 获取商品销量列表
        /// zhangfan created at 2012-July-12th
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime)
        {
            return _goodsDemandDao.GetGoodsSales(startTime, endTime);
        }

        /// <summary> 新增门店采购申请单
        /// </summary>
        /// <param name="applyStockContent"></param>
        /// <param name="applyStockDetailContent"></param>
        /// <returns></returns>
        public bool AddShopFrontApplyStock(string applyStockContent, string applyStockDetailContent)
        {
            var info = JsonConvert.DeserializeObject<ApplyStockInfo>(applyStockContent);
            var detailList = JsonConvert.DeserializeObject<IList<ApplyStockDetailInfo>>(applyStockDetailContent);
            string errorMsg;
            var applyStockBll = new ApplyStockBLL(GlobalConfig.DB.FromType.Write);
            return applyStockBll.Add(info, detailList, out errorMsg) > 0;
        }

        /// <summary> 根据商品ID设置商品是否缺货
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="isScarcity">是否缺货</param>
        /// <param name="upOrDownstate"></param>
        public WCFReturnInfo UpdateGoodsISScarcity(Guid pushDataId, Guid goodsId, int isScarcity, int upOrDownstate)
        {
            //如果commandid存在，且已执行成功，则直接返回true，表示该命令已经被执行；否则操作
            return Execute(pushDataId, () => _goodsCenterSao.SetRealGoodsIsScarcity(goodsId, isScarcity == 1, "服务自动", Guid.Empty));
        }

        /// <summary>设置子商品是否缺货
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="isScarcity"></param>
        /// <returns></returns>
        public WCFReturnInfo UpdateRealGoodsISScarcity(Guid pushDataId, Guid realGoodsId, int isScarcity)
        {
            //如果commandid存在，且已执行成功，则直接返回true，表示该命令已经被执行；否则操作
            return Execute(pushDataId, () => _goodsCenterSao.SetRealGoodsIsScarcity(realGoodsId, isScarcity == 1, "服务自动", Guid.Empty));
        }

        /// <summary> 更新申请单状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateApplyStockState(Guid applyId, int state)
        {
            try
            {
                string msg;
                var applyStockBll = new ApplyStockBLL(GlobalConfig.DB.FromType.Write);
                var info = _applyStock.FindById(applyId);
                IDictionary<Guid, decimal> settleDics;
                return applyStockBll.UpdateApplyStockState(info, state, false, out settleDics, out msg);
            }
            catch (FaultException fexp)
            {
                throw new FaultException(fexp.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reckoningContent"></param>
        /// <returns></returns>
        public bool AddReckoningInfo(string reckoningContent)
        {
            try
            {
                string errorMsg;
                var reck = JsonConvert.DeserializeObject<ReckoningInfo>(reckoningContent);
                var reckoning = new ReckoningInfo(reck.ReckoningId, reck.FilialeId, reck.ThirdCompanyID,
                                                            reck.TradeCode, reck.Description ?? string.Empty,
                                                            reck.AccountReceivable,
                                                            reck.ReckoningType, reck.State, reck.IsChecked,
                                                            reck.AuditingState, reck.LinkTradeCode, reck.WarehouseId);
                _reckoning.Insert(reckoning, out errorMsg);
                return true;
            }
            catch (FaultException fexp)
            {
                throw new FaultException(fexp.Message);
            }
        }

        /// <summary> 添加帐务记录数据
        /// </summary>
        /// <param name="pushDataId"> </param>
        /// <param name="info"></param>
        public WCFReturnInfo InsertReckoningInfo(Guid pushDataId, ReckoningInfo info)
        {
            return Execute(pushDataId, () =>
            {
                string errorMsg;
                info.TradeCode = _codeManager.GetCode(CodeType.PY);
                if (info.ReckoningType == (int)ReckoningType.Defray)
                {
                    info.AccountReceivable = -Math.Abs(info.AccountReceivable);
                }
                else
                {
                    info.AccountReceivable = Math.Abs(info.AccountReceivable);
                }
                if (info.Description.Contains("[损坏快递赔偿]")
                || info.Description.Contains("[快递损坏赔偿]")
                || info.Description.Contains("[快递丢件赔偿]")
                || info.Description.Contains("[丢件快递赔偿]"))
                {
                    if (!_reckoning.IsExist(info, out errorMsg))
                        info.IsAllow = true;
                }
                _reckoning.Insert(info, out errorMsg);
            });
        }

        /// <summary>
        /// 修改往来账对账类型
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="linkTradeCode">原始单据号</param>
        /// <param name="linkTradeType">对应单据类型</param>
        /// <param name="reckoningType">账单类型：0收入，1支出</param>
        /// <param name="reckoningCheckType">往来账对账类型</param>
        /// <param name="isChecked">对账类型 1 已对账 0 未对账 2 异常对账</param>
        /// zal 2016-06-05
        public bool UpdateCheckState(Guid pushDataId, string linkTradeCode, int linkTradeType, int reckoningType, int reckoningCheckType, int isChecked)
        {
            return _reckoning.UpdateCheckState(linkTradeCode, linkTradeType, reckoningType, reckoningCheckType, isChecked);
        }

        /// <summary> 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public string GetCompanyCussentList(int companyType)
        {
            return JsonConvert.SerializeObject(_companyCussent.GetCompanyCussentList((CompanyType)companyType));
        }

        public IList<BankAccountInfo> GetBankAccountList(Guid filialeId)
        {
            var list = _bankAccounts.GetBankAccountsList().Where(ent => ent.TargetId == filialeId && ent.IsUse).ToList();
            return list;
        }

        /// <summary> 对帐
        /// </summary>
        /// <param name="lstModify"></param>
        /// <param name="lstAdd"></param>
        /// <param name="wastBookinfo"></param>
        public void Checking(IList<ReckoningInfo> lstModify, IList<ReckoningInfo> lstAdd, WasteBookInfo wastBookinfo)
        {
            var reckoningManager = new ReckoningManager(_reckoning, _wasteBookDaoWrite);
            reckoningManager.Checking(lstModify, lstAdd, wastBookinfo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="originalTradeCode"></param>
        /// <param name="checkType"></param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        public ReckoningInfo GetReckoningInfo(Guid companyId, string originalTradeCode, CheckType checkType, int reckoningCheckType = 2)
        {
            return _reckoning.GetReckoningInfo(companyId, originalTradeCode, checkType, reckoningCheckType);
        }

        // 增加往来单位类型字段搜索
        /// <summary>
        /// 按是否对账,日期,账单类型获取往来账（未对账）
        /// 用于列表显示
        /// </summary>
        /// <param name="companyClassId">往来单位类型</param>
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
        public DataListInfo<ReckoningInfo> GetValidateDataPage(Guid companyClassId, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType,
            AuditingState auditingState, ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, params int[] money)
        {
            int recordCount;
            IList<ReckoningInfo> list = _reckoning.GetValidateDataPage(companyClassId, companyId, filialeId, startDate,
                                                                        endDate, cType, auditingState, receiptType,
                                                                        tradeCode, warehouseId, keepyear, start, limit, out recordCount, money);
            return new DataListInfo<ReckoningInfo>
            {
                AllRecordCount = recordCount,
                Items = list
            };
        }

        /// <summary> 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsListFromERP()
        {
            return _bankAccounts.GetBankAccountsList();
        }

        /// <summary> 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsListByFBP(Guid filialeId, Guid branchId, Guid positionId)
        {
            return _bankAccounts.GetBankAccountsList(filialeId, branchId, positionId);
        }

        public IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList()
        {
            return new BindingList<CompanyBankAccountsInfo>();
        }

        /// <summary> 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        public double GetBankAccountsNonce(Guid bankAccountsId)
        {
            return _bankAccounts.GetBankAccountsNonce(bankAccountsId);
        }

        /// <summary> 根据条件获取账目信息
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
        public IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, String tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear)
        {
            return _wasteBookDaoWrite.GetWasteBookList(bankAccountsId, startDate, endDate, receiptType, auditingState, minIncome, maxIncome, tradeCode, filialeId, branchId, positionId, keepyear);
        }

        /// <summary> 根据条件获取账目信息(分页)
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
        public Dictionary<int, IList<WasteBookInfo>> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate, ReceiptType receiptType,
            AuditingState auditingState, double minIncome, double maxIncome, String tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int startPage, int pageSize)
        {
            long recordCount;
            IList<WasteBookInfo> list = _wasteBookDaoWrite.GetWasteBookListToPage(bankAccountsId, startDate, endDate, receiptType, auditingState, minIncome, maxIncome, tradeCode, filialeId, branchId, positionId, keepyear, startPage, pageSize, out recordCount);
            var dic = new Dictionary<int, IList<WasteBookInfo>> { { (int)recordCount, list } };
            return dic;
        }

        /// <summary> 根据条件获取账目信息(分页)
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
        public Dictionary<int, IList<WasteBookInfo>> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate, ReceiptType receiptType, AuditingState auditingState, double minIncome, double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck, int startPage, int pageSize)
        {
            long recordCount;
            IList<WasteBookInfo> list = _wasteBookDaoWrite.GetWasteBookListBySaleFilialeIdToPage(saleFilialeId, startDate, endDate, receiptType,
                                                               auditingState, minIncome, maxIncome, tradeCode, filialeId,
                                                               branchId, positionId, keepyear, isCheck, startPage, pageSize, out recordCount);
            var dic = new Dictionary<int, IList<WasteBookInfo>> { { (int)recordCount, list } };
            return dic;
        }

        /// <summary> 获取指定tradecode在表中的记录数
        /// </summary>
        /// <param name="tradecode"></param>
        /// <returns></returns>
        public decimal GetTradeCodeNum(string tradecode)
        {
            return _wasteBookDaoWrite.GetTradeCodeNum(tradecode);
        }

        /// <summary> 获取手续费（为reckoning表修改记录用）
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public decimal GetPoundageForReckoning(string tradeCode)
        {
            return _wasteBookDaoWrite.GetPoundageForReckoning(tradeCode);
        }

        /// <summary> 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public string GetWasteBookId(string tradeCode)
        {
            return _wasteBookDaoWrite.GetWasteBookId(tradeCode);
        }

        /// <summary> 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public string GetWasteBookIdForUpdate(string tradeCode)
        {
            return _wasteBookDaoWrite.GetWasteBookIdForUpdate(tradeCode);
        }

        /// <summary> 获取wastebookid
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public string GetWasteBookIdForReckoning(string tradeCode)
        {
            return _wasteBookDaoWrite.GetWasteBookIdForReckoning(tradeCode);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId)
        {
            return _wasteBookCheckDao.GetWasteBookCheck(wasteBookId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public WasteBookInfo GetWasteBook(Guid wasteBookId)
        {
            return _wasteBookDaoWrite.GetWasteBook(wasteBookId);
        }

        /// <summary>根据单据编号获取资金流
        /// </summary>
        /// <param name="linkTradeCode">关联单据编号</param>
        /// <param name="wasteSource">1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额</param>
        /// <returns></returns>
        /// zal 2016-06-15
        public WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource)
        {
            return _wasteBookDaoWrite.GetWasteBookByLinkTradeCodeAndWasteSource(linkTradeCode, wasteSource);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public WasteTypeInfo GetWasteBookInfo(string tradeCode)
        {
            return _wasteBookDaoWrite.GetWasteBookInfo(tradeCode);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IList<CompanyClassInfo> GetChildCompanyClassList(Guid companyClassId)
        {
            return _companyClass.GetChildCompanyClassList(companyClassId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentListByClass(Guid companyClassId)
        {
            return _companyCussent.GetCompanyCussentList(companyClassId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetAllCompanyCussentList()
        {
            return _companyCussent.GetCompanyCussentList();
        }

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        public CompanyCussentInfo GetCompanyCussent(Guid companyId)
        {
            return _companyCussent.GetCompanyCussent(companyId);
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public double GetNonceReckoningTotalled(Guid companyId)
        {
            return _companyCussent.GetNonceReckoningTotalled(companyId);
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <param name="filialeId"> </param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public double GetNonceReckoningTotalledDetail(Guid companyId, Guid filialeId)
        {
            return _companyCussent.GetNonceReckoningTotalled(companyId, filialeId);
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public ReckoningInfo GetReckoning(Guid reckoningId)
        {
            return _reckoning.GetReckoning(reckoningId);
        }

        /// <summary> 获取指定reckoningId的ReckoningType
        /// </summary>
        /// <param name="reckoningId"></param>
        /// <returns></returns>
        public int GetReckoningType(Guid reckoningId)
        {
            return _reckoning.GetReckoningType(reckoningId);
        }

        /// <summary> 获取指定往来单位的往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        public decimal GetTotalled(Guid companyId)
        {
            return _reckoning.GetTotalled(companyId);
        }

        /// <summary> 获取手续费
        /// </summary>
        /// <param name="tradeCode">单据编号</param>
        public decimal GetPoundage(string tradeCode)
        {
            return _wasteBookDaoWrite.GetPoundage(tradeCode);
        }

        /// <summary> 获取指定reckoningId的ReckoningType
        /// </summary>
        /// <returns></returns>
        public bool CheckPoundage(decimal sum, decimal poundage)
        {
            if ((poundage <= 6) || (poundage / sum <= (decimal)0.02))
            {
                return true;
            }
            return false;
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateReckoning(Guid pushDataId, ReckoningInfo reckoningInfo)
        {
            return Execute(pushDataId, () => _reckoning.Update(reckoningInfo));
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBookDescription(Guid pushDataId, Guid wasteBookId, String description)
        {
            return Execute(pushDataId, () => _reckoning.UpdateDescription(wasteBookId, description));
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateDescriptionForAuditing(Guid pushDataId, string tradeCode, String description)
        {
            return Execute(pushDataId, () => _reckoning.UpdateDescriptionForAuditing(tradeCode, description));
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateReckoningDescription(Guid pushDataId, Guid reckongId, String description)
        {
            return Execute(pushDataId, () => _reckoning.UpdateDescription(reckongId, description));
        }

        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo AuditingReckoning(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _reckoning.Auditing(tradeCode));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo DeleteReckoning(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _reckoning.Delete(tradeCode));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBook(Guid pushDataId, WasteBookInfo wasteBookInfo)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.Update(wasteBookInfo));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBookWithBank(Guid pushDataId, WasteBookInfo wasteBookInfo)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.Update(wasteBookInfo));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBookDateTime(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.UpdateDateTime(tradeCode));
        }

        /// <summary> 在reckoning中更改wastebook中的手续费
        /// </summary>
        public WCFReturnInfo UpdatePoundageForReckoning(Guid pushDataId, Guid wastebookId, DateTime dateCreated, decimal poundage)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.UpdatePoundageForReckoning(wastebookId, dateCreated, poundage));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo DeleteWasteBook(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.DeleteWasteBook(tradeCode));
        }

        /// <summary> 根据账单编号删除往来账核对信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo DeleteWasteBookPoundage(Guid pushDataId, string tradeCode, decimal poundage)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.DeleteWasteBookPoundage(tradeCode, poundage));
        }

        public string GetCode(CodeType codeType)
        {
            return _codeManager.GetCode(codeType);
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBookDescriptionForAuditing(Guid pushDataId, string tradeCode, String description)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.UpdateDescriptionForAuditing(tradeCode, description));
        }

        /// <summary> 审核往来账信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo AuditingWasteBook(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.Auditing(tradeCode));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateWasteBookCheck(Guid pushDataId, WasteBookCheckInfo checkInfo)
        {
            return Execute(pushDataId, () => _wasteBookCheckDao.Update(checkInfo));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo InsertWasteBookCheck(Guid pushDataId, WasteBookCheckInfo checkInfo)
        {
            return Execute(pushDataId, () => _wasteBookCheckDao.Insert(checkInfo));
        }

        ///<summary> 更改一笔账目
        /// </summary>
        public WCFReturnInfo UpdateBll(Guid pushDataId, Guid outWasteBookId, string description, decimal income, string tradecode, decimal poundage, Guid bankAccountsId)
        {
            return Execute(pushDataId, () => _bankAccountManager.UpdateBll(outWasteBookId, description, income, tradecode, poundage, bankAccountsId));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdateBankAccountsId(Guid pushDataId, Guid wasteBookId, Guid bankAccountsId)
        {
            return Execute(pushDataId, () => _wasteBookDaoWrite.UpdateBankAccountsId(wasteBookId, bankAccountsId));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo InsertPoundage(Guid pushDataId, Guid outBankAccountsId, string tradeCode, decimal poundage, Guid filialeId)
        {
            return Execute(pushDataId, () => _bankAccountManager.InsertPoundage(outBankAccountsId, tradeCode, poundage, filialeId));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo UpdatePoundage(Guid pushDataId, string tradeCode, decimal poundage)
        {
            return Execute(pushDataId, () => _bankAccountManager.UpdatePoundage(tradeCode, poundage));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo Virement(Guid pushDataId, Guid inBankAccountsId, Guid outBankAccountsId, decimal sum, decimal poundage, string description, string tradeCode, Guid filialeId)
        {
            return Execute(pushDataId, () => _bankAccountManager.Virement(inBankAccountsId, outBankAccountsId, sum, poundage, description, tradeCode, filialeId));
        }

        /// <summary> 修改账单信息
        /// </summary>
        /// <returns></returns>
        public WCFReturnInfo AuditingWaste(Guid pushDataId, string tradeCode)
        {
            return Execute(pushDataId, () => _bankAccountManager.Auditing(tradeCode));
        }

        public DateTime GetGoodsPredictArrivalTime(Guid realGoodsId)
        {
            return _purchasingRead.GetGoodsPredictArrivalTime(realGoodsId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        public GoodsBarcodeInfo GetGoodsBarcodeInfo(string goodsCode)
        {
            GoodsBarcodeInfo goodsBarcodeInfo = null;
            GoodsInfo goodsInfo = _goodsCenterSao.GetGoodsBaseInfoByCode(goodsCode);
            if (goodsInfo != null)
            {
                goodsBarcodeInfo = new GoodsBarcodeInfo
                {
                    GoodsId = goodsInfo.GoodsId,
                    GoodsCode = goodsInfo.GoodsCode,
                    GoodsName = goodsInfo.GoodsName
                };
            }
            return goodsBarcodeInfo;
        }

        /// <summary> 根据商品ID获取会员ID集合
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public IList<Guid> GetMemberIdListByRealGoodsIds(DateTime startTime, DateTime endTime, List<Guid> realGoodsIds)
        {
            return _goodsOrderWrite.GetMemberIdListByRealGoodsIds(GlobalConfig.KeepYear, startTime, endTime, realGoodsIds);
        }

        /// <summary>
        /// 售后更新日销量
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="orderId"></param>
        /// <param name="afterSaleDetailList"></param>
        public WCFReturnInfo UpdateGoodsDaySalesStatistics(Guid pushDataId, Guid orderId, IList<AfterSaleDetailInfo> afterSaleDetailList)
        {
            return Execute(pushDataId, () => _orderManager.AfterSaleUpdateGoodsDaySalesStatistics(orderId, afterSaleDetailList));
        }

        #region  往来单位收付款(门店)

        /// <summary>
        /// 往来单位收付款添加,修改
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        public CompanyFundReceiptInfo InsertCompanyFundReceiptForShop(CompanyFundReceiptInfo receipt)
        {
            CompanyFundReceiptInfo companyFundReceiptInfo = null;
            if (receipt.ReceiptID == Guid.Empty) //添加
            {
                receipt.ReceiptNo =
                    _codeManager.GetCode(receipt.ReceiptType == (int)CompanyFundReceiptType.Receive
                                                  ? CodeType.GT
                                                  : CodeType.PY);
                var companyCussentInfo = _companyCussent.GetCompanyCussent(receipt.CompanyID);
                if (companyCussentInfo != null)
                    receipt.HasInvoice = companyCussentInfo.IsNeedInvoices;//是否要开发票
                if (receipt.SettleStartDate == DateTime.MinValue)
                    receipt.SettleStartDate = Convert.ToDateTime("1999-09-09");
                if (receipt.SettleStartDate == DateTime.MinValue)
                    receipt.SettleEndDate = Convert.ToDateTime("1999-09-09");
                receipt.OtherDiscountCaption += "[门店生成的收付款]";
                if (receipt.ReceiptType == (int)CompanyFundReceiptType.Receive)
                {
                    receipt.ReceiptStatus = receipt.HasInvoice ? Convert.ToInt32(CompanyFundReceiptState.WaitInvoice) : Convert.ToInt32(CompanyFundReceiptState.Audited);
                }
                if (receipt.RealityBalance != 0)
                {
                    var result = _companyFundReceipt.Insert(receipt);
                    if (result)
                    {
                        companyFundReceiptInfo = _companyFundReceipt.GetFundReceiptInfoByReceiptNo(receipt.ReceiptNo);
                    }
                }
            }
            else  //修改
            {
                _companyFundReceipt.Update(receipt);
            }
            return receipt.ReceiptID == Guid.Empty ? companyFundReceiptInfo : receipt;
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
        public IList<CompanyFundReceiptInfo> GetCompanyFundReceiptForShop(DateTime? startTime, DateTime? endTime, Guid? companyId, Guid filialeId, int? receiptType, string searchKey)
        {
            return _companyFundReceipt.GetFundReceiptList(startTime, endTime, companyId, filialeId, receiptType, searchKey);
        }

        /// <summary>
        /// 更改往来单位收付款状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="receiptId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateCompanyFundReceiptStatus(Guid pushDataId, Guid receiptId, int status, string remark)
        {
            var info = Execute(pushDataId, () => _companyFundReceipt.UpdateCompanyFundReceiptStatus(receiptId, status, remark));
            return info != null && (bool)info.ReturnValue;
        }

        /// <summary>
        /// 根据id获取往来帐收付款状态
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public int GetCompanyFundReceiptStatus(Guid receiptId)
        {
            var info = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId);
            if (info != null)
            {
                return info.ReceiptStatus;
            }
            return -1;
        }

        #endregion

        /// <summary>针对支付宝订单IsOut为False且事后申请发票更新订单和资金流字段IsOut为True（其他地方慎用）
        /// </summary>
        /// <param name="orderIds">订单Ids  </param>
        /// <param name="paidNo">交易流水号</param>
        /// <returns></returns>
        public Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo)
        {
            return _wasteBookDaoWrite.RenewalWasteBookByIsOut(orderIds, paidNo);
        }

        public ReturnResult IsEnterFaxAudit(bool isEnable)
        {
            return ReturnResult.Fault("此方法已废弃");
        }

        public ReturnResult<bool> GetEnterFaxAuditState()
        {
            return ReturnResult<bool>.Fault("此方法已废弃");
        }

        /// <summary>
        /// 根据员工ID获取对应的预借款预警信息
        /// 财务查询全部可输入Guid.Empty
        /// </summary>
        /// <returns></returns>
        public int GetPreloanWarningsCount(Guid personnelId, Guid systemBranchId)
        {
            DateTime now = DateTime.Now;
            var endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).AddDays(-60);
            IList<int> states = new List<int>
            {
                (int)CostReportState.Complete,(int)CostReportState.Cancel
            };
            string where = string.Join(",", states.ToArray());
            if (systemBranchId == new Guid("3500A995-68C3-49F4-AD9C-4CBA53A44834")) //财务部
            {
                return _costReport.GetPreloanWarningsCount((int)CostReportKind.Before, endTime, Guid.Empty, where);
            }
            return _costReport.GetPreloanWarningsCount((int)CostReportKind.Before, endTime, personnelId, where);
        }

        /// <summary>
        /// 获取商品类型对应的税率比例(包含：商品类型、类型编码、比例)
        /// </summary>
        /// <param name="goodsTypes"></param>
        /// <returns></returns>
        public List<TaxrateProportionInfo> GeTaxrateProportionInfos(IEnumerable<int> goodsTypes)
        {
            return _taxrateProportion.GetNewPercentages(goodsTypes);
        }

        /// <summary>
        /// 修改发票状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="clew">备注</param>
        /// <returns></returns>
        public WCFReturnInfo SetGoodsOrderInvoiceState(Guid pushDataId, Guid invoiceId, InvoiceState invoiceState, string clew)
        {
            return Execute(pushDataId, () => invoiceId != Guid.Empty && _invoiceDao.SetInvoiceState(invoiceId, invoiceState, clew));
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        public WCFReturnInfo BatchSetInvoiceState(Guid pushDataId, List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel)
        {
            return Execute(pushDataId, () => invoiceIdList.Count > 0 && _invoiceDao.BatchSetInvoiceState(invoiceIdList, invoiceState, cancelPersonel));
        }

        /// <summary> 修改发票信息
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="invoiceInfo"></param>
        /// <returns></returns>
        public WCFReturnInfo SetGoodsOrderInvoice(Guid pushDataId, InvoiceInfo invoiceInfo)
        {
            return Execute(pushDataId, () => _invoiceDao.Update(invoiceInfo));
        }

        /// <summary> 根据年份和指定条件查询订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="orderTime"></param>
        /// <returns></returns>
        public GoodsOrderInfo GetOrderInfoByNo(string orderNo, DateTime orderTime)
        {
            return _goodsOrderWrite.GetGoodsOrder(orderNo, GlobalConfig.KeepYear, orderTime);
        }

        /// <summary> 根据年份和指定条件查询订单明细
        /// </summary>
        /// <param name="orderTime"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IList<GoodsOrderDetailInfo> GetGoodsOrderDetailList(DateTime orderTime, string orderNo)
        {
            return _goodsOrderDetailWrite.GetGoodsOrderDetailList(orderTime, Guid.Empty, orderNo, GlobalConfig.KeepYear);
        }

        /// <summary>第三方订单直接完成发货【四维】 ADD  2016年12月6日  陈重文
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressId">订单快递Id</param>
        /// <param name="expressNo">订单快递号</param>
        /// <param name="companyId">配置的往来单位ID</param>
        /// <returns></returns>
        public bool ThirdPartyOrderComplete(string orderNo, Guid expressId, string expressNo, Guid companyId)
        {
            if (String.IsNullOrWhiteSpace(orderNo) || companyId == default(Guid) || expressId == default(Guid)) return false;

            var orderInfo = _goodsOrderWrite.GetGoodsOrder(orderNo);
            if (orderInfo == null || orderInfo.OrderId == default(Guid)) return false;
            var companyInfo = _companyCussent.GetCompanyCussent(companyId);
            if (companyInfo == null || companyInfo.CompanyId == default(Guid)) return false;

            var des = String.Format("[完成订单,订单金额应付款]");
            var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), orderInfo.SaleFilialeId, companyId, _codeManager.GetCode(CodeType.PY), des,
                                -(orderInfo.RealTotalPrice + orderInfo.PaymentByBalance), (int)ReckoningType.Defray,
                                (int)ReckoningStateType.Currently,
                                (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                orderInfo.OrderNo, orderInfo.DeliverWarehouseId)
            {
                LinkTradeType = (int)ReckoningLinkTradeType.GoodsOrder,
                IsOut = orderInfo.IsOut
            };

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var result = _goodsOrderWrite.ThirdPartyOrderDirectlyToComplete(orderNo, expressId, expressNo);
                    if (result)
                    {
                        string msg;
                        result = _reckoning.Insert(reckoningInfo, out msg);
                        if (result)
                        {
                            ts.Complete();
                        }
                    }
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
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
            return _salesGoodsRanking.GetGoodsSaleBySaleFilialeId(fromTime, toTime, saleFilialeId);
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
            return _salesGoodsRanking.GetGoodsSaleBySalePlatformIdList(fromTime, toTime, salePlatformIdList);
        }

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <returns></returns>
        /// zal 2017-08-08
        public List<VocabularyInfo> GetVocabularyListByState()
        {
            return _vocabulary.GetVocabularyListByState((int)VocabularyState.Enable);
        }

        /// <summary>
        /// 获取违禁词(State=1)
        /// </summary>
        /// <returns></returns>
        /// zal 2017-10-25
        public List<string> GetVocabularyNameList()
        {
            return _vocabulary.GetVocabularyNameListByState((int)VocabularyState.Enable);
        }

        /// <summary>
        /// 根据商品IDS，批量获取对应的商品 供应商关系（供应商ID，商品ID）
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, Guid> GetGoodsAndCompanyList(IList<Guid> goodsId)
        {
            return _companyCussent.GetGoodsAndCompanyDic(goodsId);
        }

        /// <summary>
        /// 获取境外供应商列表（返回值：供应商ID，名称）
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, String> GetAbroadCompanyList()
        {
            return _companyCussent.GetAbroadCompanyList();
        }

        /// <summary>
        /// 根据登录账号、销售公司Id获取已授权的境外供应商列表
        /// </summary>
        /// <returns></returns>
        public List<AuthorizeCompanyDTO> GetAuthorizeCompanyDtos(string accountNo, Guid saleFilialeId)
        {
            return _companyCussentRelation.GetAuthorizeCompanyDtos(accountNo, saleFilialeId);
        }

        #region 补贴审核、补贴打款

        /// <summary>
        /// 新增补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo AddSubsidyPayment(SubsidyPaymentInfo_Add model)
        {
            return Execute(model.ID, () => _subsidyPaymentDal.AddSubsidyPayment(model));
        }

        /// <summary>
        /// 修改补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo UpdateSubsidyPayment(SubsidyPaymentInfo_Edit model)
        {
            return Execute(model.ID, () => _subsidyPaymentDal.UpdateSubsidyPayment(model));
        }
        /// <summary>
        /// 审核补贴审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo ApprovalSubsidyPayment(SubsidyPaymentInfo_Approval model)
        {
            return Execute(model.ID, () => _subsidyPaymentDal.ApprovalSubsidyPayment(model));
        }

        /// <summary>
        /// 删除补贴审核（作废）
        /// </summary>
        /// <param name="ID">补贴审核编号</param>
        /// <param name="ModifyUser">修改人</param>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo DeleteSubsidyPayment(Guid ID, string ModifyUser)
        {
            return Execute(ID, () => _subsidyPaymentDal.DeleteSubsidyPayment(ID, ModifyUser));
        }

        /// <summary>
        /// 获取补贴审核列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public IList<SubsidyPaymentInfo> GetSubsidyPaymentList(SubsidyPaymentInfo_SeachModel seachModel, out int Totalcount)
        {
            return _subsidyPaymentDal.GetSubsidyPaymentList(seachModel, out Totalcount);
        }

        /// <summary>
        /// 获取补贴审核详细信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public SubsidyPaymentInfo GetSubsidyPaymentByID(Guid ID)
        {
            return _subsidyPaymentDal.GetSubsidyPaymentByID(ID);
        }

        /// <summary>
        /// 根据第三方订单号 判断（1：有没有进行中（待审核、待财务审核、不通过）的费用补贴，2：补贴次数不超过2次）
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public ResultModel IsExistSubsidyPayment(string ThirdPartyOrderNumber)
        {
            return _subsidyPaymentDal.IsExistSubsidyPayment(ThirdPartyOrderNumber);
        }

        #endregion 补贴审核、补贴打款

        #region 退款打款

        /// <summary>
        /// 新增退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo AddRefundsMoney(RefundsMoneyInfo_Add model)
        {
            return Execute(model.ID, () => _refundsMoneyDal.AddRefundsMoney(model));
        }

        /// <summary>
        /// 修改退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo UpdateRefundsMoney(RefundsMoneyInfo_Edit model)
        {
            return Execute(model.ID, () => _refundsMoneyDal.UpdateRefundsMoney(model));
        }
        /// <summary>
        /// 审核退款打款
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo ApprovalRefundsMoney(RefundsMoneyInfo_Approval model)
        {
            return Execute(model.ID, () => _refundsMoneyDal.ApprovalRefundsMoney(model));
        }

        /// <summary>
        /// 删除退款打款（作废）
        /// </summary>
        /// <param name="ID">退款打款编号</param>
        /// <param name="ModifyUser">修改人</param>
        /// <returns></returns>
        [OperationContract]
        public WCFReturnInfo DeleteRefundsMoney(Guid ID, string ModifyUser)
        {
            return Execute(ID, () => _refundsMoneyDal.DeleteRefundsMoney(ID, ModifyUser));
        }

        /// <summary>
        /// 获取退款打款列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public IList<RefundsMoneyInfo> GetRefundsMoneyList(RefundsMoneyInfo_SeachModel seachModel, out int Totalcount)
        {
            return _refundsMoneyDal.GetRefundsMoneyList(seachModel, out Totalcount);
        }

        /// <summary>
        /// 获取退款打款详细信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public RefundsMoneyInfo GetRefundsMoneyByID(Guid ID)
        {
            return _refundsMoneyDal.GetRefundsMoneyByID(ID);
        }

        #endregion 退款打款
    }
}