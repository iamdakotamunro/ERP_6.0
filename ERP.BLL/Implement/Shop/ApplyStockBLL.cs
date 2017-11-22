using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.FinanceModule;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IShop;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.ShopFront;
using KeedeGroup.GoodsManageSystem.Public.Enum;
using ERP.DAL.Interface.FinanceModule;
using ERP.Model;

namespace ERP.BLL.Implement.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplyStockBLL : BllInstance<ApplyStockBLL>
    {

        private readonly IApplyStockDAL _applyStockDao;
        private readonly IPurchaseSet _purchaseSet;
        private readonly IShopExchangedApplyDetail _applyDetailBll;
        private readonly IGoodsCenterSao _goodsInfoSao;
        private readonly StorageManager _storageManager;
        readonly IReckoning _reckoning;
        readonly IGoodsDaySalesStatistics _daySalesStatistics = new GoodsDaySalesStatistics(GlobalConfig.DB.FromType.Read);
        readonly IRealTimeGrossSettlementDal _grossSettlementDal = new RealTimeGrossSettlementDal(GlobalConfig.DB.FromType.Read);

        /// <summary>
        /// for 门店确认异常入库
        /// </summary>
        /// <param name="fromType"></param>
        public ApplyStockBLL(GlobalConfig.DB.FromType fromType)
        {
            _goodsInfoSao = new GoodsCenterSao();
            _reckoning = new Reckoning(fromType);
            _applyStockDao = new ApplyStockDAL(fromType);
            _applyDetailBll = new ShopExchangedApplyDetailDal(fromType);
            _purchaseSet = new PurchaseSet(fromType);
            _storageManager = new StorageManager(fromType);
        }

        /// <summary>
        /// 获取采购明细
        /// </summary>
        /// <param name="applyStockDao"></param>
        public ApplyStockBLL(IApplyStockDAL applyStockDao)
        {
            _applyStockDao = applyStockDao;
        }

        /// <summary>
        /// 添加申请、修改状态
        /// </summary>
        /// <param name="applyStockDao"></param>
        /// <param name="goodsCenterSao"></param>
        /// <param name="purchaseSet"></param>
        /// <param name="applyDetail"></param>
        public ApplyStockBLL(IApplyStockDAL applyStockDao, IGoodsCenterSao goodsCenterSao, IPurchaseSet purchaseSet, IShopExchangedApplyDetail applyDetail)
        {
            _goodsInfoSao = goodsCenterSao;
            _applyStockDao = applyStockDao;
            _purchaseSet = purchaseSet;
            _applyDetailBll = applyDetail;
        }

        /// <summary>
        /// 新增门店采购单
        /// </summary>
        /// <param name="applyStockInfo"></param>
        /// <param name="applyStockDetailInfoList"></param>
        /// <param name="errorMsg"> </param>
        /// <returns>采购申请状态</returns>
        public int Add(ApplyStockInfo applyStockInfo, IList<ApplyStockDetailInfo> applyStockDetailInfoList, out string errorMsg)
        {
            int state = -1;
            try
            {
                var isalliance = FilialeManager.IsAllianceShopFiliale(applyStockInfo.FilialeId, out errorMsg);
                if (isalliance)//联盟店
                {
                    if (applyStockInfo.CompanyWarehouseId == Guid.Empty)
                    {
                        applyStockInfo.PurchaseType = (int)PurchaseType.FromPurchase;
                    }

                    var filialeInfo = FilialeManager.Get(applyStockInfo.CompanyId);
                    applyStockInfo.CompanyName = filialeInfo != null && filialeInfo.ID != Guid.Empty ? filialeInfo.Name : "-";

                    //新增加盟店采购申请添加时待确认
                    if (applyStockInfo.StockState >= (int)ApplyStockState.Delivering)
                    {
                        var goodsIds = applyStockDetailInfoList.Select(ent => ent.CompGoodsID).Distinct().ToList();
                        var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                        if (goodsInfos == null || goodsInfos.Count!=goodsIds.Count)
                        {
                            errorMsg = "GMS商品信息获取失败";
                            return state;
                        }
                        var hostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(applyStockInfo.CompanyWarehouseId, applyStockInfo.CompanyId, goodsInfos.Select(ent => ent.GoodsType).Distinct());
                        if (hostingFilialeId == Guid.Empty)
                        {
                            errorMsg = "获取仓库对应的物流公司失败";
                            return state;
                        }
                        var settleDics = _grossSettlementDal.GetLatestUnitPriceListByMultiGoods(hostingFilialeId, goodsIds);
                        var flag = IsMatchSendCondition(applyStockInfo, applyStockDetailInfoList,hostingFilialeId, goodsInfos.ToDictionary(k=>k.GoodsId,v=>v), settleDics, out errorMsg);
                        //确认不通过修改采购申请状态为等待确认
                        if (!flag)
                            applyStockInfo.StockState = (int)ApplyStockState.Confirming;
                    }
                }
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    bool isSuccess = _applyStockDao.Insert(applyStockInfo);
                    if (isSuccess)
                    {
                        if (_applyStockDao.InsertDetail(applyStockDetailInfoList) > 0)
                        {
                            state = applyStockInfo.StockState;
                        }
                        else
                        {
                            isSuccess = false;
                            errorMsg = "添加申请明细时失败";
                        }
                    }
                    else
                    {
                        errorMsg = "添加申请记录失败";
                    }
                    if (isSuccess)
                    {
                        ts.Complete();
                    }
                }
                return state;
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(string.Format("新增采购申请报错: applyStockInfo={0}, applyStockDetailInfoList={1}", new Framework.Core.Serialize.JsonSerializer().Serialize(applyStockInfo), new Framework.Core.Serialize.JsonSerializer().Serialize(applyStockDetailInfoList)), "采购管理", exp);
                errorMsg = exp.Message;
                return -1;
            }
        }

        public int AddNewApplyStock(ApplyStockInfo applyStockInfo, IList<ApplyStockDetailInfo> applyStockDetailInfoList, IDictionary<Guid, decimal> settledics, out string errorMsg)
        {
            var isalliance = FilialeManager.IsAllianceShopFiliale(applyStockInfo.FilialeId, out errorMsg);
            if (isalliance)//联盟店
            {
                if (applyStockInfo.CompanyWarehouseId == Guid.Empty)
                {
                    applyStockInfo.PurchaseType = (int)PurchaseType.FromPurchase;
                }

                var filialeInfo = FilialeManager.Get(applyStockInfo.CompanyId);
                applyStockInfo.CompanyName = filialeInfo != null && filialeInfo.ID != Guid.Empty ? filialeInfo.Name : "-";

                //新增加盟店采购申请添加时待确认
                if (applyStockInfo.StockState >= (int)ApplyStockState.Delivering)
                {
                    var goodsIds = applyStockDetailInfoList.Select(ent => ent.CompGoodsID).Distinct().ToList();
                    var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                    if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
                    {
                        errorMsg = "GMS商品信息获取失败";
                        return -1;
                    }
                    var hostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(applyStockInfo.CompanyWarehouseId, applyStockInfo.CompanyId, goodsInfos.Select(ent => ent.GoodsType).Distinct());
                    if (hostingFilialeId == Guid.Empty)
                    {
                        errorMsg = "获取仓库对应的物流公司失败";
                        return -1;
                    }
                    var flag = IsMatchSendCondition(applyStockInfo, applyStockDetailInfoList, hostingFilialeId, goodsInfos.ToDictionary(k => k.GoodsId, v => v), settledics, out errorMsg);
                    //确认不通过修改采购申请状态为等待确认
                    if (!flag)
                        applyStockInfo.StockState = (int)ApplyStockState.Confirming;
                }
            }
            if (_applyStockDao.Insert(applyStockInfo) && _applyStockDao.InsertDetail(applyStockDetailInfoList) > 0)
            {
                errorMsg = "";
                return applyStockInfo.StockState;
            }
            errorMsg = "添加申请明细时失败";
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public IList<ApplyStockDetailInfo> FindDetailList(Guid applyId)
        {
            var infoList = _applyStockDao.FindDetailList(applyId);
            var applyInfo = _applyStockDao.FindById(applyId);
            var quantityDic = new Dictionary<Guid, int>();
            if (applyInfo != null)
            {
                quantityDic = WMSSao.GoodsEffitiveStockBySaleFilialeId(applyInfo.CompanyWarehouseId, null,
                    infoList.GroupBy(ent=>new {ent.GoodsId,ent.CompGoodsID}).ToDictionary(k=>k.Key.GoodsId,v=>v.Key.CompGoodsID), applyInfo.CompanyId);
                if (quantityDic == null)
                {
                    throw new ApplicationException("库存中心异常FindDetailList");
                }
            }
            foreach (var info in infoList)
            {
                info.GoodsStock = quantityDic.ContainsKey(info.GoodsId) ? quantityDic[info.GoodsId] : 0;
            }
            return infoList;
        }

        /// <summary>
        /// 更新申请单状态
        /// modify by liangcanren at 2015-04-15 添加事务且修改状态时修改采购申请和明细需确认提示信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <param name="alliance">判断是否同步到联盟店</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool UpdateApplyStockState(Guid applyId, int state, bool alliance,out string msg)
        {
            msg = string.Empty;
            ApplyStockInfo applyInfo;
            var dics = new Dictionary<Guid, string>();
            if (state == (int)ApplyStockState.Delivering && !alliance)
            {
                applyInfo = _applyStockDao.FindById(applyId);
                var detailList = FindDetailList(applyId);
                var goodsIds = detailList.Select(ent => ent.CompGoodsID).Distinct().ToList();
                var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
                {
                    msg = "GMS商品信息获取失败";
                    return false;
                }
                var hostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(applyInfo.CompanyWarehouseId, applyInfo.CompanyId, goodsInfos.Select(ent => ent.GoodsType).Distinct());
                if (hostingFilialeId == Guid.Empty)
                {
                    msg = "获取仓库对应的物流公司失败";
                    return false;
                }
                var settleDics = _grossSettlementDal.GetLatestUnitPriceListByMultiGoods(hostingFilialeId, goodsIds);
                var result = IsMatchSendCondition(applyInfo, detailList, hostingFilialeId, goodsInfos.ToDictionary(k => k.GoodsId, v => v), settleDics, out msg);
                if (!result)
                {
                    //更新采购申请信息
                    state = (int)ApplyStockState.Confirming;
                    dics = detailList.Where(act => act.IsComfirmed).ToDictionary(k => k.GoodsId, v => v.ComfirmTips);
                }
            }
            bool flag = _applyStockDao.UpdateApplyStockState(applyId, state);
            //修改明细中商品需确认标识
            if (flag && dics.Count > 0)
            {
                if (dics.Select(dic => _applyStockDao.UpdateDetailTips(applyId, dic.Key, dic.Value, true)).Any(result => !result))
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                msg = "修改对应门店采购申请状态失败!";
            }
            else
            {
                if (alliance)
                {
                    applyInfo = _applyStockDao.FindById(applyId);
                    if (applyInfo == null)
                    {
                        msg = "未找到相对应的采购申请记录!";
                        flag = false;
                    }
                    else
                    {
                        string message;
                        var parenId = FilialeManager.GetShopHeadFilialeId(applyInfo.FilialeId);
                        //联盟店备注格式：[XX]
                        var result = ShopSao.UpdatePurchaseState(parenId, applyId, state, string.Empty, out message);
                        if (!result)
                        {
                            msg = message;
                            flag = false;
                        }
                    }
                }
            }
            return flag;

        }

        public bool UpdateApplyStockState(ApplyStockInfo applyInfo, int state, bool alliance,out IDictionary<Guid,decimal> settleDics,out string msg)
        {
            msg = string.Empty;
            var dics = new Dictionary<Guid, string>();
            settleDics=new Dictionary<Guid, decimal>();
            if (state == (int)ApplyStockState.Delivering && !alliance)
            {
                var detailList = FindDetailList(applyInfo.ApplyId);
                var goodsIds = detailList.Select(ent => ent.CompGoodsID).Distinct().ToList();
                var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
                {
                    msg = "GMS商品信息获取失败";
                    return false;
                }
                var hostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(applyInfo.CompanyWarehouseId, applyInfo.CompanyId, goodsInfos.Select(ent => ent.GoodsType).Distinct());
                if (hostingFilialeId == Guid.Empty)
                {
                    msg = "获取仓库对应的物流公司失败";
                    return false;
                }
                settleDics = _grossSettlementDal.GetLatestUnitPriceListByMultiGoods(hostingFilialeId, goodsIds);
                var result = IsMatchSendCondition(applyInfo, detailList, hostingFilialeId, goodsInfos.ToDictionary(k => k.GoodsId, v => v), settleDics, out msg);
                if (!result)
                {
                    //更新采购申请信息
                    state = (int)ApplyStockState.Confirming;
                    dics = detailList.Where(act => act.IsComfirmed).ToDictionary(k => k.GoodsId, v => v.ComfirmTips);
                }
            }
            bool flag = _applyStockDao.UpdateApplyStockState(applyInfo.ApplyId, state);
            //修改明细中商品需确认标识
            if (flag && dics.Count > 0)
            {
                if (dics.Select(dic => _applyStockDao.UpdateDetailTips(applyInfo.ApplyId, dic.Key, dic.Value, true)).Any(result => !result))
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                msg = "修改对应门店采购申请状态失败!";
            }
            else
            {
                if (alliance)
                {
                    string message;
                    var parenId = FilialeManager.GetShopHeadFilialeId(applyInfo.FilialeId);
                    //联盟店备注格式：[XX]
                    var result = ShopSao.UpdatePurchaseState(parenId, applyInfo.ApplyId, state, string.Empty, out message);
                    if (!result)
                    {
                        msg = message;
                        flag = false;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 根据单号更新申请单状态
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateApplyStockStateByTradeCode(string tradeCode, int state)
        {
            var flag = _applyStockDao.UpdateApplyStockStateByTradeCode(tradeCode, state);
            return flag;
        }

        /// <summary>
        /// 更新联盟店采购申请状态且同步到联盟总店
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool UpdateStockStateErp(Guid applyId, int state, out string errorMsg)
        {
            IDictionary<Guid, decimal> settleDics;
            try
            {
                var applyInfo = _applyStockDao.FindById(applyId);
                using (var scope = new TransactionScope())
                {

                    //修改采购申请状态
                    var isScuess = UpdateApplyStockState(applyInfo, state, true, out settleDics ,out errorMsg);
                    if (isScuess)
                    {
                        scope.Complete();
                    }
                    else
                    {
                        errorMsg = "同步到联盟店申请状态时失败!";
                        return false;
                    }
                }
                errorMsg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 判断是否满足发货条件
        /// </summary>
        /// <param name="applyStockInfo"></param>
        /// <param name="applyStockDetailInfoList"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsDics"></param>
        /// <param name="dicAvgSettlePrice"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool IsMatchSendCondition(ApplyStockInfo applyStockInfo, IList<ApplyStockDetailInfo> applyStockDetailInfoList,
            Guid hostingFilialeId,Dictionary<Guid,Model.Goods.GoodsInfo> goodsDics,IDictionary<Guid, decimal> dicAvgSettlePrice, out string errorMsg)
        {
            errorMsg = string.Empty;

            //获取子商品列表(for获取商品库存和销量)
            var realGoodsIds = applyStockDetailInfoList.Select(act => act.GoodsId).Distinct().ToList();
            var flag = false;

            //2、发货量限制在可得15天外库存，如果采购数大于该库存手动确认
            //当前库存-获取最近15天每日销量
            var start = DateTime.Now.AddDays(-15);
            var end = DateTime.Now.AddDays(-1);
            var stockCurrent = WMSSao.GoodsEffitiveStock(applyStockInfo.CompanyWarehouseId,null, realGoodsIds, hostingFilialeId);
            var saleGoodsIds = _daySalesStatistics.GetRealGoodsDaySales(applyStockInfo.CompanyWarehouseId, applyStockInfo.CompanyId, new DateTime(start.Year, start.Month, start.Day),
                new DateTime(end.Year, end.Month, end.Day), realGoodsIds);
            if (stockCurrent != null && stockCurrent.Count > 0)
            {
                bool isNeedeConfirm = false;
                foreach (var realGoodsId in realGoodsIds)
                {
                    var dataList = applyStockDetailInfoList.Where(act => act.GoodsId == realGoodsId).ToList();
                    var stock = stockCurrent.Count > 0 && stockCurrent.ContainsKey(realGoodsId)
                        ? stockCurrent[realGoodsId]
                        : 0;
                    var sale = saleGoodsIds.FirstOrDefault(act => act.Key == realGoodsId);
                    var tempFlag = stock - (sale!=null ? sale.Value : 0) > dataList.Sum(act => act.Quantity);
                    if (!isNeedeConfirm || flag)
                        flag = tempFlag;
                    isNeedeConfirm = true;
                    if (!tempFlag)
                    {
                        foreach (var applyStockDetailInfo in dataList)
                        {
                            applyStockDetailInfo.IsComfirmed = true;
                            applyStockDetailInfo.ComfirmTips = "当前库存<可得15天外销量";
                        }
                    }
                }
            }

            if (flag)
            {
                //获取商品采购设置列表
                var purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(goodsDics.Keys, applyStockInfo.CompanyWarehouseId, hostingFilialeId);
                if (purchaseSetList == null || purchaseSetList.Count == 0)
                {
                    errorMsg = "采购商品全部未绑定商品采购设置!";
                    return false;
                }
                foreach (var goodsInfo in goodsDics.Values)
                {
                    var dataList = applyStockDetailInfoList.Where(act => act.CompGoodsID == goodsInfo.GoodsId && !act.IsComfirmed).ToList();
                    string content = string.Empty;
                    //1、判断是否存在护理液等特殊商品
                    if (goodsInfo.GoodsType == (int)GoodsType.LensSolution
                    || goodsInfo.GoodsType == (int)GoodsType.CareProducts
                    || goodsInfo.GoodsType == (int)GoodsType.NoSet)
                    {
                        flag = false;
                        content = "护理液/用品或未设置等特殊商品";
                    }
                    if (flag)
                    {
                        var setInfo = purchaseSetList.FirstOrDefault(act => act.GoodsId == goodsInfo.GoodsId);
                        if (goodsInfo.ExpandInfo != null && setInfo != null)
                        {
                            //3、加盟价<=成本价*5% && 加盟价<=采购价/年返点 需手动确认，采购状态为待确认
                            if (goodsInfo.ExpandInfo.JoinPrice > (dicAvgSettlePrice.ContainsKey(goodsInfo.GoodsId) ? dicAvgSettlePrice[goodsInfo.GoodsId] : 0) * decimal.Parse("0.05"))
                            {
                                if (goodsInfo.ExpandInfo.YearDiscount == 0) continue;
                                if (goodsInfo.ExpandInfo.JoinPrice > (setInfo.PurchasePrice / goodsInfo.ExpandInfo.YearDiscount)) continue;
                                content = "加盟价<=采购价/年返点";
                            }
                            else
                            {
                                content = "加盟价<=成本价*5%";
                            }
                        }
                        flag = false;
                    }
                    if (content.Length > 0)
                    {
                        foreach (var applyStockDetailInfo in dataList)
                        {
                            applyStockDetailInfo.IsComfirmed = true;
                            applyStockDetailInfo.ComfirmTips = content;
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 判断商品是否允许采购
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public bool IsAllowPurchase(Guid shopId, Guid goodsId)
        {
            //获取已退商品
            var dataList = _applyDetailBll.IsAllowPurchase(shopId, true, new List<Guid> { goodsId }, DateTime.MinValue, DateTime.MinValue);
            if (dataList != null && dataList.Count > 0)
            {
                if (dataList.ContainsKey(goodsId) && !dataList[goodsId])
                    return false;
            }
            var goodsInfo = _goodsInfoSao.GetGoodsBaseInfoById(goodsId);
            if (goodsInfo == null) return false;
            var goodsIds = _goodsInfoSao.GetGoodsIDListByBrandID(goodsInfo.BrandId);
            if (goodsIds != null && goodsIds.Count > 0)
            {
                //获取已退商品
                var brandsList = _applyDetailBll.IsAllowPurchase(shopId, true, new List<Guid>(), DateTime.Now.AddMonths(-3), DateTime.Now);
                if (brandsList != null && brandsList.Count > 0)
                {
                    if (brandsList.Any(act => goodsIds.Contains(act.Key) && !brandsList[act.Key]))
                        return false;
                }
            }
            return true;
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
        /// <param name="msg"> </param>
        /// <returns></returns>
        public int ShopConfirmPurchaseOrder(Guid applyId, StorageRecordInfo storageRecord, IList<StorageRecordDetailInfo> storageRecordDetail,
            ApplyStockInfo applyStock, IList<ApplyStockDetailInfo> stockDetailInfos, IList<ReckoningInfo> list, out string msg)
        {
            int state = 0;
            //获取商品特定时间下最近的结算价存档
            var applyInfo = _applyStockDao.FindById(applyId);
            if (applyInfo == null)
            {
                msg = "门店要货申请记录未找到！";
                return -1;
            }
            IDictionary<Guid, decimal> dicAvgSettlePrice;
            using (var scope = new TransactionScope())
            {
                var isError = storageRecord != null && storageRecordDetail != null && storageRecordDetail.Count != 0;
                var stockState = isError ? (int)ApplyStockState.CheckPending : (int)ApplyStockState.Finished;
                var flag = UpdateApplyStockState(applyInfo, stockState, false, out dicAvgSettlePrice, out msg);
                if (flag)
                {
                    if (isError)
                    {
                        try
                        {
                            _storageManager.NewAddStorageRecordAndDetailList(storageRecord, storageRecordDetail, out msg);
                        }
                        catch (Exception ex)
                        {
                            state = -1;
                            SAL.LogCenter.LogService.LogError(string.Format("ERP服务门店确认收获添加库存记录失败，applyID={0},storageRecord={1},storageRecordDetail={2},applyStock={3},stockDetailInfos={4},reckoningList={5}", applyId, new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecord), new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecordDetail), new Framework.Core.Serialize.JsonSerializer().Serialize(applyStock), new Framework.Core.Serialize.JsonSerializer().Serialize(stockDetailInfos), new Framework.Core.Serialize.JsonSerializer().Serialize(list)), "门店管理", ex);
                        }
                    }
                    if (state == 0 && applyStock != null && stockDetailInfos != null && stockDetailInfos.Count != 0)
                    {
                        state = AddNewApplyStock(applyStock, stockDetailInfos, dicAvgSettlePrice, out msg);
                    }
                    if (state >= 0)
                    {
                        bool result = true;
                        if (list != null && list.Count > 0)
                        {
                            foreach (var reckoningInfo in list)
                            {
                                string error;
                                var sucess = _reckoning.Insert(reckoningInfo, out error);
                                if (!sucess)
                                {
                                    result = false;
                                    msg = "插入往来帐失败!";
                                    break;
                                }
                            }
                        }
                        if (result)
                        {
                            scope.Complete();
                        }
                        else
                        {
                            state = -1;
                            SAL.LogCenter.LogService.LogError(string.Format("ERP服务门店确认收获更改状态失败，applyID={0},storageRecord={1},storageRecordDetail={2},applyStock={3},stockDetailInfos={4},reckoningList={5}", applyId, new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecord), new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecordDetail), new Framework.Core.Serialize.JsonSerializer().Serialize(applyStock), new Framework.Core.Serialize.JsonSerializer().Serialize(stockDetailInfos), new Framework.Core.Serialize.JsonSerializer().Serialize(list)), "门店管理");
                        }
                    }
                }
                else
                {
                    state = -1;
                    SAL.LogCenter.LogService.LogError(string.Format("ERP服务门店确认收获更改状态失败，applyID={0},storageRecord={1},storageRecordDetail={2},applyStock={3},stockDetailInfos={4},reckoningList={5}", applyId, new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecord), new Framework.Core.Serialize.JsonSerializer().Serialize(storageRecordDetail), new Framework.Core.Serialize.JsonSerializer().Serialize(applyStock), new Framework.Core.Serialize.JsonSerializer().Serialize(stockDetailInfos), new Framework.Core.Serialize.JsonSerializer().Serialize(list)), "门店管理");
                }
            }
            return state;
        }
    }
}
