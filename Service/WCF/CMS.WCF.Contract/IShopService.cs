using System;
using System.Collections.Generic;
using System.ServiceModel;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.ShopFront;
using ERP.Model;
using Framework.Data;

namespace ERP.Service.Contract
{
    public partial interface IService
    {
        #region 加盟店


        /// <summary>
        /// 根据id获得一条
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [OperationContract]
        ShopActivityNoticeInfo SelectActivityNoticeInfo(Guid noteId);

        /// <summary>
        /// 查询加盟店首页广告图片
        /// </summary>
        /// <returns>空-返回null  注意判断</returns>
        [OperationContract]
        ShopActivityImageInfo SelectShopActivityImageInfo(int shopActivityImageType);

        /// <summary>
        /// 获得所有的加盟店广告
        /// 请自行筛选
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<ShopActivityNoticeInfo> GetAllShopActivityNoticeList();

        /// <summary>
        /// 根据条件查询广告---分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isShow">是否显示</param>
        /// <param name="isNotice">是否是广告-显示在首页</param>
        /// <returns></returns>
        [OperationContract]
        PageItems<ShopActivityNoticeInfo> SelectNoticeListByPage(int pageIndex, int pageSize, bool isShow, bool? isNotice);
        #endregion

        #region   联盟店采购申请

        /// <summary>
        /// 更新门店采购申请状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="stockState"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateShopApplyStockState(Guid applyId, int stockState, string remark);

        /// <summary>
        /// 确认收获异常，原采购完成，生成新采购
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="storageRecord"></param>
        /// <param name="storageRecordDetail"></param>
        /// <param name="applyStock"></param>
        /// <param name="stockDetailInfos"></param>
        /// <param name="list">往来帐列表</param>
        /// <returns></returns>
        [OperationContract]
        int ShopConfirmPurchaseOrder(Guid applyId, StorageRecordInfo storageRecord, IList<StorageRecordDetailInfo> storageRecordDetail,
            ApplyStockInfo applyStock, IList<ApplyStockDetailInfo> stockDetailInfos, IList<ReckoningInfo> list);
        #endregion

        #region  退换货申请相关

        /// <summary>
        /// 获取退换货单的状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [OperationContract]
        int GetExchangeState(Guid applyId);

        /// <summary>
        /// 是否存在未审核的退货留言
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsExistNoAuditMessage(Guid shopId);

        /// <summary>
        /// 添加退货留言
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddRefundMessage(ShopRefundMessageInfo messageInfo);

        /// <summary>
        /// 添加换货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddBarterApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo,
            IList<ShopExchangedApplyDetailInfo> detailInfos);


        /// <summary>
        /// 修改换货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateBarterApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo,
            IList<ShopExchangedApplyDetailInfo> detailInfos);

        /// <summary>
        /// 修改退货检查
        /// </summary>
        /// <param name="checkRefundInfo"></param>
        /// <param name="checkRefundDetailInfos"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateCheckRefundList(CheckRefundInfo checkRefundInfo, IList<CheckRefundDetailInfo> checkRefundDetailInfos);

        /// <summary>
        /// 添加退货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddRefundApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo, IList<ShopApplyDetailInfo> detailInfos);

        /// <summary>
        /// 修改退货申请列表
        /// </summary>
        /// <param name="applyDetailInfo"></param>
        /// <param name="detailInfos"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateRefundApplyGoodsList(ShopExchangedApplyInfo applyDetailInfo, IList<ShopApplyDetailInfo> detailInfos);

        /// <summary>
        /// 更新退换货申请状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="exchangeState"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateExchangeApplyState(Guid applyId, int exchangeState, string remark);

        /// <summary>
        /// 获取当月的退货记录
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-03-15
        [OperationContract]
        Dictionary<string, decimal> SelectOneMonthReturnedApplyList(Guid shopId, DateTime dateTime);

        /// <summary>
        /// 获取退换货申请列表
        /// </summary>
        /// <param name="isBarter">false 换货，true 退货 </param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="applyNo">申请编号</param>
        /// <param name="state"></param>
        /// <param name="goodsNameOrGoodsCode">商品名称或者商品Code</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">第几页</param>
        /// <returns></returns>
        [OperationContract]
        PageItems<ShopExchangedApplyInfo> GetExchangedApplyList(bool isBarter, Guid shopId, DateTime startTime,
            DateTime endTime, string applyNo, int state, string goodsNameOrGoodsCode, int pageSize, int pageIndex);

        /// <summary>
        /// 获取换货明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<ShopExchangedApplyDetailInfo> GetBarterGoodsListByApplyId(Guid applyId);

        /// <summary>
        /// 获取退货明细
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<ShopApplyDetailInfo> GetRefundGoodsListByApplyId(Guid applyId);

        /// <summary>
        /// 判断店铺下某商品是否允许采购
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsAllowPurchase(Guid shopId, Guid goodsId);

        /// <summary>
        /// 获取未审核及已审核的退还换货记录
        /// </summary>
        /// <param name="type">type:-1,全部，0：换货,1退货</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="goodsId">商品Id</param>
        /// <returns>key:子商品Id,value:退换货数</returns>
        [OperationContract]
        Dictionary<Guid, int> GetNoAuditedGoodsQuantity(int type, Guid shopId, Guid goodsId);
        #endregion

        #region  联盟店采购申请

        /// <summary>
        /// 联盟店添加采购申请
        /// </summary>
        /// <param name="applyStock"></param>
        /// <param name="list"></param>
        /// <returns>采购申请状态</returns>
        [OperationContract]
        int AddApplyStock(ApplyStockInfo applyStock, IList<ApplyStockDetailInfo> list);

        #endregion

        #region  退货商品检查

        /// <summary>
        /// 是否成功生成退货商品检查
        /// </summary>
        /// <param name="applyId">申请单号</param>
        /// <param name="expressNo">快递单号</param>
        /// <param name="expressName">快递公司</param>
        /// <returns></returns>
        [OperationContract]
        bool IsSuccessCreateCheck(Guid applyId, Guid warehouseId, string expressNo, string expressName);

        #endregion

        #region


        /// <summary>
        /// 根据店铺获取帐户(用于联盟店往来单位收付款)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<ShopBankAccountsInfo> GetBankAccountsByShopId();

        /// <summary>
        /// 添加往来单位收付款到ERP
        /// </summary>
        /// <param name="fundReceiptInfo"></param>
        /// <returns>收付款单据号</returns>
        [OperationContract]
        string InsertCompanyFundReceipt(CompanyFundReceiptInfo fundReceiptInfo);

        #endregion

        #region 获取所有的资金帐号

        /// <summary>
        /// 获取资金帐号列表不关联BankAccountBinding
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<BankAccountInfo> GetBankAccountsList();
        #endregion

        #region 获取平均月结算价
        /// <summary>
        /// 获取商品特定时间下最近的结算价存档，如果最近结算价没有(即表示是新添加的商品)，则取该商品的采购价
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2016-05-19
        [OperationContract]
        Dictionary<Guid, Dictionary<Guid, decimal>> GetFilialeIdGoodsIdAvgSettlePrice(DateTime dateTime);
        #endregion

        /// <summary> 根据订单号，获取指定的发票索取记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        InvoiceInfo GetInvoiceByOrderId(Guid orderId);

        /// <summary>获取会员Id订单金额，积分列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PageItems<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex);

        /// <summary>获取商品的采购设置（用于门店自己采购，供应商直接发货至门店） 2016年10月27日   陈重文
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        [OperationContract]
        PurchaseSetInfo GetPurchaseSetInfo(Guid goodsId, Guid warehouseId);

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="isDelete"></param>
        /// <returns>0:禁用；1:启用；2:全部</returns>
        /// zal 2017-03-16
        [OperationContract]
        IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId, int isDelete);

        /// <summary>
        /// 获取所有商品采购设置
        /// </summary>
        /// <returns></returns>
        /// zal 2017-03-07
        [OperationContract]
        List<PurchaseSetInfo> GetPurchaseSetListByWarehouseIdAndCompanyId(Guid warehouseId, Guid companyId);

        /// <summary>获取往来单位绑定公司信息   ADD 2016年12月7日  陈重文
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        [OperationContract]
        List<Guid> GetCompanyBindFilialeIds(Guid companyId);
    }
}
