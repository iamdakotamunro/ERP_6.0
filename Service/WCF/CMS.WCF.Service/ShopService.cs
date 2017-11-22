using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Order;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.BLL.Interface;
using ERP.DAL.Implement.FinanceModule;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IShop;
using ERP.DAL.Interface.IStorage;
using ERP.Enum;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.Model;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.ShopFront;
using CompanyFundReceipt = ERP.DAL.Implement.Inventory.CompanyFundReceipt;
using Framework.Data;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        private static readonly CodeManager _codeManager = new CodeManager();
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Write);
        private static readonly ShopActivityNoticeDal _activityNoticeDal = new ShopActivityNoticeDal(GlobalConfig.DB.FromType.Write);
        private static readonly ShopActivityImageDal _shopActivityImageDal = new ShopActivityImageDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyFundReceipt _companyFundReceipt = new CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private static readonly CheckRefund _refundDalWrite = new CheckRefund(GlobalConfig.DB.FromType.Write);
        private static readonly ShopExchangedApplyBll _shopExchangedApplyBllWrite = new ShopExchangedApplyBll(GlobalConfig.DB.FromType.Write);
        private static readonly IShopRefundMessage _shopRefundMessage = new ShopRefundMessageDal(GlobalConfig.DB.FromType.Write);
        private static readonly IShopExchangedApplyDetail _shopExchangedApplyDetailWrite = new ShopExchangedApplyDetailDal(GlobalConfig.DB.FromType.Write);
        private static readonly IShopExchangedApply _shopExchangedApply = new ShopExchangedApplyDal(GlobalConfig.DB.FromType.Write);
        private static readonly IApplyStockDAL _applyStock = new ApplyStockDAL(GlobalConfig.DB.FromType.Write);
        private static readonly StorageManager _storageManager = new StorageManager(GlobalConfig.DB.FromType.Write);
        private static readonly IInternalPriceSetManager _internalPriceSetManager = new InternalPriceSetManager(GlobalConfig.DB.FromType.Write);
        readonly IGoodsStockRecord _goodsStockSettleRecordBll = new GoodsStockRecordDao();
        readonly IRealTimeGrossSettlementDal _realTimeGrossSettlement = new RealTimeGrossSettlementDal(GlobalConfig.DB.FromType.Write);

        #region  联盟店广告

        /// <summary>
        /// 根据id获得一条
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public ShopActivityNoticeInfo SelectActivityNoticeInfo(Guid noteId)
        {
            return _activityNoticeDal.SelectActivityNoticeInfo(noteId);
        }

        /// <summary>
        /// 获取店铺广告图片
        /// </summary>
        /// <returns></returns>
        public ShopActivityImageInfo SelectShopActivityImageInfo(int shopActivityImageType)
        {
            var result = _shopActivityImageDal.SelectShopActivityImageInfo();
            if (result != null && result.Count > 0)
            {
                var tempInfo = result.FirstOrDefault(ent => ent.ShopActivityImageType == shopActivityImageType);
                if (tempInfo != null && !string.IsNullOrWhiteSpace(tempInfo.ShopActivityImage))
                {
                    return tempInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得所有的加盟店广告
        /// 请自行筛选
        /// </summary>
        /// <returns></returns>
        public IList<ShopActivityNoticeInfo> GetAllShopActivityNoticeList()
        {
            return _activityNoticeDal.SelectNoticeList().ToList();
        }

        /// <summary>
        /// 根据条件查询广告---分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isShow">是否显示</param>
        /// <param name="isNotice">是否是广告-显示在首页</param>
        /// <returns></returns>
        public PageItems<ShopActivityNoticeInfo> SelectNoticeListByPage(int pageIndex, int pageSize, bool isShow, bool? isNotice)
        {
            var result = _activityNoticeDal.SelectNoticeListByPage(pageIndex, pageSize, isShow, isNotice);
            return new PageItems<ShopActivityNoticeInfo>(pageSize, result.RecordCount, result.Items);
        }
        #endregion

        #region   联盟店采购申请
        /// <summary>  pass
        /// 添加联盟店采购申请
        /// </summary>
        /// <param name="applyStock"></param>
        /// <param name="stockDetailInfos"> </param>
        /// <returns></returns>
        public int AddApplyStock(ApplyStockInfo applyStock, IList<ApplyStockDetailInfo> stockDetailInfos)
        {
            string msg;
            var applyStockBll = new ApplyStockBLL(GlobalConfig.DB.FromType.Write);
            return applyStockBll.Add(applyStock, stockDetailInfos, out msg);
        }

        /// <summary>  pass
        /// 更新门店采购申请状态  
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="stockState"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateShopApplyStockState(Guid applyId, int stockState, string remark)
        {
            string msg;
            var applyStockBll = new ApplyStockBLL(GlobalConfig.DB.FromType.Write);
            var applyInfo = _applyStock.FindById(applyId);
            IDictionary<Guid, decimal> settleDics;
            return applyStockBll.UpdateApplyStockState(applyInfo, stockState, false,out settleDics, out msg);
        }

        /// <summary>
        /// 确认收获异常，原采购完成，生成新采购
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="storageRecord"></param>
        /// <param name="storageRecordDetail"></param>
        /// <param name="applyStock"></param>
        /// <param name="stockDetailInfos"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int ShopConfirmPurchaseOrder(Guid applyId, StorageRecordInfo storageRecord, IList<StorageRecordDetailInfo> storageRecordDetail,
            ApplyStockInfo applyStock, IList<ApplyStockDetailInfo> stockDetailInfos, IList<ReckoningInfo> list)
        {
            string msg;
            return ApplyStockBLL.WriteInstance.ShopConfirmPurchaseOrder(applyId, storageRecord, storageRecordDetail, applyStock,
                                                      stockDetailInfos, list, out msg);
        }
        #endregion

        #region  退换货申请相关

        /// <summary>  pass
        /// 获取退换货单的状态  
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public int GetExchangeState(Guid applyId)
        {
            return _shopExchangedApply.GetExchangeState(applyId, string.Empty);
        }

        /// <summary> pass
        /// 添加退货留言  
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool AddRefundMessage(ShopRefundMessageInfo messageInfo)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var msgId = _shopRefundMessage.GetLastedRefundMsgId(messageInfo.ShopID, (int)ReturnMsgState.CheckPending);
                    if (msgId != Guid.Empty)
                    {
                        var row = _shopRefundMessage.DeleteShopRefundMessage(msgId);
                        if (row < 0)
                            return false;
                    }
                    _shopRefundMessage.InsertShopRefundMessage(messageInfo);
                    scope.Complete();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否存在未审核的退货留言
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public bool IsExistNoAuditMessage(Guid shopId)
        {
            return _shopRefundMessage.GetMessageCount(shopId, (int)ReturnMsgState.CheckPending) > 0;
        }

        /// <summary> pass
        /// 添加换货申请列表
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        public bool AddBarterApplyGoodsList(ShopExchangedApplyInfo applyInfo,
            IList<ShopExchangedApplyDetailInfo> detailInfos)
        {
            string errorMsg;
            return _shopExchangedApplyBllWrite.AddShopExchangedApply(applyInfo, detailInfos, out errorMsg);
        }

        /// <summary>
        /// 修改换货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        public bool UpdateBarterApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo,
            IList<ShopExchangedApplyDetailInfo> detailInfos)
        {
            string errorMsg;
            return _shopExchangedApplyBllWrite.UpdateShopExchangedApply(applyDetailInfo, detailInfos, out errorMsg);
        }


        /// <summary> pass
        /// 添加退货申请列表
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        public bool AddRefundApplyGoodsList(ShopExchangedApplyInfo applyInfo, IList<ShopApplyDetailInfo> detailInfos)
        {
            string errorMsg;
            if (applyInfo.MsgID == Guid.Empty)//因为直营店不需要走退货申请，故如果门店是直营店的话，MsgID字段门店前台会赋上一个随机的值，所以不会走此分支 zal 2016-04-21
            {
                var msgId = _shopRefundMessage.GetLastedRefundMsgId(applyInfo.ShopID, (byte)ReturnMsgState.Pass);
                if (msgId == Guid.Empty)
                    return false;
                applyInfo.MsgID = msgId;
            }
            return _shopExchangedApplyBllWrite.AddShopRefundApply(applyInfo, detailInfos, out errorMsg);
        }


        /// <summary>
        /// 修改退货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        public bool UpdateRefundApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo, IList<ShopApplyDetailInfo> detailInfos)
        {
            string errorMsg;
            return _shopExchangedApplyBllWrite.UpdateShopRefundApply(applyDetailInfo, detailInfos, out errorMsg);
        }

        /// <summary>  pass
        /// 更新退换货申请状态  
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="exchangeState"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateExchangeApplyState(Guid applyId, int exchangeState, string remark)
        {
            string errorMsg;
            var result = _shopExchangedApplyBllWrite.SetShopExchangedState(applyId, exchangeState, remark, out errorMsg);
            return result > 0;
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
            return _shopExchangedApplyBllWrite.SelectOneMonthReturnedApplyList(shopId, dateTime);
        }

        /// <summary>  pass
        /// 获取退换货申请列表
        /// </summary>
        /// <param name="isBarter"> </param>
        /// <param name="shopId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="applyNo"></param>
        /// <param name="state"></param>
        /// <param name="goodsNameOrGoodsCode"> </param>
        /// <param name="pageSize"> </param>
        /// <param name="pageIndex"> </param>
        /// <returns></returns>
        public PageItems<ShopExchangedApplyInfo> GetExchangedApplyList(bool isBarter, Guid shopId, DateTime startTime,
            DateTime endTime, string applyNo, int state, string goodsNameOrGoodsCode, int pageSize, int pageIndex)
        {
            var totalList = _shopExchangedApplyBllWrite.GetShopExchangedApplyList(isBarter, applyNo, startTime, endTime, null, shopId,
                                                                 state, goodsNameOrGoodsCode).ToList();
            var tempList = totalList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PageItems<ShopExchangedApplyInfo>(pageIndex, pageSize, totalList.Count, tempList);
        }

        /// <summary>  pass
        /// 获取换货明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyDetailInfo> GetBarterGoodsListByApplyId(Guid applyId)
        {
            return _shopExchangedApplyDetailWrite.GetShopExchangedApplyDetailList(applyId);
        }

        /// <summary>  pass
        /// 获取退货明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IEnumerable<ShopApplyDetailInfo> GetRefundGoodsListByApplyId(Guid applyId)
        {
            return _shopExchangedApplyDetailWrite.GetShopApplyDetailList(applyId);
        }

        /// <summary>  pass
        /// 判断商品是否允许采购 条件：三月内无同品牌下商品退货  该商品无
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public bool IsAllowPurchase(Guid shopId, Guid goodsId)
        {
            var applyStockBll = new ApplyStockBLL(null, _goodsCenterSao, null, _shopExchangedApplyDetailWrite);
            return applyStockBll.IsAllowPurchase(shopId, goodsId);
        }

        /// <summary>
        /// 获取未审核及已审核的换货记录
        /// </summary>
        /// <param name="type">type:-1,全部，0：换货,1退货</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="goodsId">商品Id</param>
        /// <returns>key:子商品Id,value:退换货数</returns>
        public Dictionary<Guid, int> GetNoAuditedGoodsQuantity(int type, Guid shopId, Guid goodsId)
        {
            var dics = _shopExchangedApplyDetailWrite.GetExchangedApplyGoodsQuantity(shopId, type, new DateTime(2014, 9, 1),
                DateTime.Now, new List<int> { (int)ExchangedState.CheckPending, (int)ExchangedState.Pass }, new List<Guid> { goodsId });
            return dics != null && dics.ContainsKey(goodsId) ? dics[goodsId] : null;
        }
        #endregion

        #region  联盟店退货商品检查修改 OK
        /// <summary>
        /// 修改退货检查
        /// </summary>
        /// <param name="checkRefundInfo"></param>
        /// <param name="checkRefundDetailInfos"></param>
        /// <returns></returns>
        public bool UpdateCheckRefundList(CheckRefundInfo checkRefundInfo, IList<CheckRefundDetailInfo> checkRefundDetailInfos)
        {
            string msg;
            return _refundDalWrite.UpdateCheckRefund(checkRefundInfo, checkRefundDetailInfos, out msg);
        }
        #endregion

        /// <summary>
        /// 是否成功生成退货商品检查
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="expressNo"> </param>
        /// <param name="expressName"> </param>
        /// <returns></returns>
        public bool IsSuccessCreateCheck(Guid applyId, Guid warehouseId, string expressNo, string expressName)
        {
            return _shopExchangedApplyBllWrite.IsSuccessCreateCheck(applyId, warehouseId, expressNo, expressName);
        }

        /// <summary>
        /// 根据店铺获取帐户(用于联盟店往来单位收付款)
        /// </summary>
        /// <returns></returns>
        public IList<ShopBankAccountsInfo> GetBankAccountsByShopId()
        {
            return _bankAccounts.GetBankAccountsByShopId();
        }

        /// <summary>
        /// 添加往来单位收付款到ERP 返回生成的单据号
        /// modifyed by liangcanren 2015-03-13
        /// </summary>
        /// <param name="fundReceiptInfo"></param>
        /// <returns></returns>
        public string InsertCompanyFundReceipt(CompanyFundReceiptInfo fundReceiptInfo)
        {
            string tradeCode =
                _codeManager.GetCode(fundReceiptInfo.ReceiptType == (int)CompanyFundReceiptType.Receive ? CodeType.GT : CodeType.PY);
            if (string.IsNullOrEmpty(tradeCode)) return string.Empty;
            fundReceiptInfo.ReceiptNo = string.Format("{0}{1}", "S", tradeCode);
            fundReceiptInfo.IsOut = false;
            var result = _companyFundReceipt.Insert(fundReceiptInfo);
            if (result)
                return fundReceiptInfo.ReceiptNo;
            return string.Empty;
        }
        #region 获取所有的资金帐号 OK

        /// <summary>
        /// 获取资金帐号列表不关联BankAccountBinding
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList()
        {
            return _bankAccounts.GetList();
        }
        #endregion

        #region 获取平均月结算价
        /// <summary>
        /// 获取商品特定时间下最近的结算价存档，如果最近结算价没有(即表示是新添加的商品)，则取该商品的采购价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2016-05-19
        public Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime)
        {
            return _realTimeGrossSettlement.GetFilialeIdGoodsIdAvgSettlePrice(dateTime);
        }

        /// <summary> 根据订单号，获取指定的发票索取记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByOrderId(Guid orderId)
        {
            return orderId == default(Guid) ? null : _invoiceDao.GetInvoiceByGoodsOrder(orderId);
        }

        /// <summary>获取会员Id订单金额，积分列表
        /// </summary>
        /// <returns></returns>
        public PageItems<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex)
        {
            long recordCount;
            var result = _goodsOrderWrite.GetOrderScoreListToPage(memeberId, year, pageSize, pageIndex, out recordCount);
            return new PageItems<OrderScoreInfo>(pageSize, recordCount, result);
        }
        #endregion

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
            var infos= _purchaseSet.GetPurchaseSetInfo(goodsId, warehouseId);
            return infos.FirstOrDefault();
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
            return _purchaseSet.GetPurchaseSetList(goodsIdList, warehouseId, isDelete);
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
            return _purchaseSet.GetPurchaseSetListByWarehouseIdAndCompanyId(warehouseId, companyId);
        }

        /// <summary>获取往来单位绑定公司信息   ADD 2016年12月7日  陈重文
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public List<Guid> GetCompanyBindFilialeIds(Guid companyId)
        {
            return companyId == default(Guid) ? new List<Guid>() : FilialeManager.GetAllHostingAndSaleFilialeList().Select(ent=>ent.ID).ToList();
            //: _companyCussent.GetCompanyBindingFiliale(companyId).ToList();
        }
    }
}
