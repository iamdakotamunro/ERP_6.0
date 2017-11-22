using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Purchasing
{
    /// <summary>
    /// 进货申报业务管理类
    /// </summary>
    public class StockDeclareManager : BllInstance<StockDeclareManager>
    {
        private readonly IPurchaseSet _purchaseSet;
        private readonly IPurchasing _purchasing;
        private readonly IPurchasingDetail _purchasingDetailManager;
        private readonly IPurchasePromotion _purchasePromotionManager;
        private readonly ICompanyCussent _companyCussent;
        private readonly IGoodsCenterSao _goodsCenterSao;

        public StockDeclareManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _purchaseSet = new PurchaseSet(fromType);
            _purchasing = new DAL.Implement.Inventory.Purchasing(fromType);
            _companyCussent = new CompanyCussent(fromType);
            _purchasingDetailManager = new PurchasingDetail(fromType);
            _purchasePromotionManager = new PurchasePromotion(fromType);
            _goodsCenterSao = new GoodsCenterSao();
        }

        public StockDeclareManager(IPurchasing purchasing,IPurchasingDetail purchasingDetail,IPurchaseSet purchaseSet,
            IGoodsCenterSao goodsCenterSao,ICompanyCussent companyCussent)
        {
            _purchaseSet = purchaseSet;
            _purchasing = purchasing;
            _companyCussent = companyCussent;
            _purchasingDetailManager = purchasingDetail;
            _goodsCenterSao = goodsCenterSao;
            _purchasePromotionManager = new PurchasePromotion(Environment.GlobalConfig.DB.FromType.Read);
        }

        /// <summary>
        /// 注意：此方法是服务自动调用，WEB页面请用Declare
        /// </summary>
        public void AutoDeclare(DateTime endTime, ref StringBuilder failMessage)
        {
            var canUseWarehouses = WMSSao.GetAllCanUseWarehouseDics();
            foreach (var warehouseInfo in canUseWarehouses)
            {
                Guid warehouseId = warehouseInfo.Key;
                if (warehouseId != Guid.Empty)
                {
                    //获取该仓库下的全部进货申报数据
                    var data = WMSSao.GetStockDeclareDtos(warehouseId, new List<Guid>());
                    if (data==null || data.Count == 0)
                    {
                        failMessage.AppendLine(warehouseInfo.Value + "没有可生成的数据!");
                        continue;
                    }
                    var purchasingSets = _purchaseSet.GetPurchaseSetListByWarehouseId(warehouseId);
                    var realGoodsInfos = _goodsCenterSao.GetStockDeclareGridList(data.Keys.ToList()).ToDictionary(k => k.RealGoodsId, v => v);
                    var purchasingGoodsDics = _purchasingDetailManager.GetStockDeclarePursingGoodsDicsWithFiliale(warehouseId,new List<Guid>(),
                        new[] { PurchasingState.NoSubmit, PurchasingState.Purchasing, PurchasingState.PartComplete, PurchasingState.StockIn, PurchasingState.WaitingAudit, PurchasingState.Refusing }, data.Keys.ToList());
                    var details = GetDataList(warehouseId, data, purchasingGoodsDics, realGoodsInfos,purchasingSets);
                    if (details.Count == 0)
                    {
                        failMessage.AppendLine(warehouseInfo.Value + "没有可生成的数据!");
                        continue;
                    }
                    failMessage.AppendLine(warehouseInfo.Value + "开始生成采购单!");
                    BuilderPurchasing(details, purchasingSets, warehouseId, null,  "进货申报服务", ref failMessage);
                    failMessage.AppendLine(warehouseInfo.Value + "结束生成采购单!");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purDics"></param>
        /// <param name="dicGoods"></param>
        /// <param name="purchaseSetList"></param>
        /// <param name="msg"></param>
        public bool CreatePurchasing(Dictionary<PurchasingInfo, IList<PurchasingDetailInfo>> purDics, Dictionary<string,Guid> dicGoods,
            IList<PurchaseSetInfo> purchaseSetList, out string msg)
        {
            if (dicGoods == null || dicGoods.Count == 0)
            {
                msg = string.Empty;
                return true;
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (KeyValuePair<PurchasingInfo, IList<PurchasingDetailInfo>> keyValue in purDics)
                {
                    var pInfo = keyValue.Key;
                    IList<PurchasingDetailInfo> plist = keyValue.Value;

                    IList<PurchasingDetailInfo> plist2 = plist.Where(w => w.PurchasingGoodsType != (int)PurchasingGoodsType.Gift).ToList();
                    //如果赠送方式为总数量赠送时使用  key 主商品ID  value 额外赠送 
                    var dics = new Dictionary<string, int>();   //需赠送
                    var sendDics = new Dictionary<string, int>(); //现返已赠送
                    var debitExtraDics = new Dictionary<string, int>(); //借计单已送
                    var addDetailsList = new List<PurchasingDetailInfo>(); //添加赠品
                    #region [现返]
                    //处理原理：
                    //赠品数量=原采购数量/(买几个+送几个)*送几个
                    //购买数量=原采购数量-赠品数量
                    foreach (var pdInfo in plist2)
                    {
                        if(!dicGoods.ContainsKey(pdInfo.GoodsName))continue;
                        var goodsBaseInfo = dicGoods[pdInfo.GoodsName];
                        PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsBaseInfo && pInfo.WarehouseID==w.WarehouseId && pInfo.PurchasingFilialeId==w.HostingFilialeId);
                        if (purchaseSetInfo == null) continue;
                        IList<PurchasePromotionInfo> ppList = _purchasePromotionManager.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId,purchaseSetInfo.WarehouseId, purchaseSetInfo.HostingFilialeId,
                            (int)PurchasePromotionType.Back);
                        PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                        if (ppInfo != null)
                        {
                            #region  新增
                            if (!ppInfo.IsSingle)  //按商品总数量进行赠送
                            {
                                if (!dics.ContainsKey(pdInfo.GoodsName))
                                {
                                    var dataList = plist2.Where(act => act.GoodsName == pdInfo.GoodsName).OrderByDescending(act => act.PlanQuantity).ToList();
                                    //余数
                                    var extra = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) % (ppInfo.BuyCount + ppInfo.GivingCount);
                                    if (extra == ppInfo.BuyCount)
                                    {
                                        dataList[0].PlanQuantity += ppInfo.GivingCount;
                                    }
                                    else
                                    {
                                        //应赠送次数
                                        int actquantity = extra % (ppInfo.BuyCount+ppInfo.GivingCount);
                                        if (actquantity >= ((ppInfo.BuyCount + ppInfo.GivingCount) / float.Parse("2.0")))
                                        {
                                            dataList[0].PlanQuantity = dataList[0].PlanQuantity + (ppInfo.BuyCount + ppInfo.GivingCount) - actquantity;
                                        }
                                    }
                                    //总商品赠送商品总数
                                    var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) * ppInfo.GivingCount / (ppInfo.BuyCount + ppInfo.GivingCount);
                                    dics.Add(pdInfo.GoodsName, sumTotal);
                                    if (dics[pdInfo.GoodsName] > 0)
                                    {
                                        
                                        foreach (var item in dataList.OrderByDescending(act => act.PlanQuantity))
                                        {
                                            if (dics[item.GoodsName] == 0) continue;
                                            var count = dics[item.GoodsName] - Convert.ToInt32(item.PlanQuantity);
                                            if (count > 0)  //赠品数>购买数
                                            {
                                                item.Price = 0;
                                                item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                                item.RealityQuantity = 0;
                                                dics[item.GoodsName] = count;
                                            }
                                            else if (count < 0)  //赠送数小于采购数
                                            {
                                                addDetailsList.Add(new PurchasingDetailInfo
                                                {
                                                    PurchasingGoodsID = Guid.NewGuid(),
                                                    PurchasingID = item.PurchasingID,
                                                    GoodsID = item.GoodsID,
                                                    GoodsName = item.GoodsName,
                                                    GoodsCode = item.GoodsCode,
                                                    Specification = item.Specification,
                                                    CompanyID = item.CompanyID,
                                                    Price = 0,
                                                    PlanQuantity = dics[item.GoodsName],
                                                    PurchasingGoodsType = (int)PurchasingGoodsType.Gift,
                                                    RealityQuantity = 0,
                                                    State = item.State,
                                                    Description = "",
                                                    Units = item.Units,
                                                    SixtyDaySales = item.SixtyDaySales,
                                                    ThirtyDaySales = item.ThirtyDaySales,
                                                    ElevenDaySales = item.ElevenDaySales,
                                                    CPrice = 0
                                                });
                                                item.RealityQuantity = 0;
                                                item.PlanQuantity = Math.Abs(count);
                                                dics[item.GoodsName] = 0;
                                            }
                                            else   //赠送数=采购数
                                            {
                                                item.Price = 0;
                                                item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                                item.RealityQuantity = 0;
                                                item.CPrice = 0;
                                                dics[item.GoodsName] = 0;
                                            }
                                        }
                                    }
                                }
                            }
                            else  //按单光度
                            {
                                //应赠余数
                                var actquantity = pdInfo.PlanQuantity % (ppInfo.BuyCount + ppInfo.GivingCount);
                                if (actquantity > 0)
                                {
                                    if (actquantity >= ((ppInfo.BuyCount+ppInfo.GivingCount) / float.Parse("2.0")))
                                    {
                                        pdInfo.PlanQuantity = pdInfo.PlanQuantity + (ppInfo.BuyCount + ppInfo.GivingCount) - actquantity;
                                    }
                                }
                                int pQuantity = int.Parse(pdInfo.PlanQuantity.ToString(CultureInfo.InvariantCulture));
                                //赠品数量=原采购数量/(买几个+送几个)*送几个
                                int quantity = pQuantity / (ppInfo.BuyCount + ppInfo.GivingCount) * ppInfo.GivingCount;
                                if (quantity > 0)
                                {
                                    var oldPurchasingDetailInfo = plist.FirstOrDefault(w => w.PurchasingGoodsID == pdInfo.PurchasingGoodsID);
                                    if (oldPurchasingDetailInfo != null)
                                    {

                                        //购买数量=原采购数量-赠品数量
                                        oldPurchasingDetailInfo.PlanQuantity -= (quantity);
                                    }

                                    var purchasingDetailInfo = plist.FirstOrDefault(w => w.GoodsID == pdInfo.GoodsID && w.PurchasingGoodsType == (int)PurchasingGoodsType.Gift);
                                    if (purchasingDetailInfo != null)
                                    {
                                        //在原赠品数量累加
                                        purchasingDetailInfo.PlanQuantity += (quantity);
                                    }
                                    else
                                    {
                                        purchasingDetailInfo = new PurchasingDetailInfo
                                        {
                                            PurchasingGoodsID = Guid.NewGuid(),
                                            PurchasingID = pInfo.PurchasingID,
                                            GoodsID = pdInfo.GoodsID,
                                            GoodsName = pdInfo.GoodsName,
                                            GoodsCode = pdInfo.GoodsCode,
                                            Specification = pdInfo.Specification,
                                            CompanyID = pdInfo.CompanyID,
                                            Price = 0,
                                            PlanQuantity = (quantity),
                                            PurchasingGoodsType = (int)PurchasingGoodsType.Gift,
                                            RealityQuantity = 0,
                                            State = 0,
                                            Description = "",
                                            Units = pdInfo.Units,
                                            SixtyDaySales = pdInfo.SixtyDaySales,
                                            ThirtyDaySales = pdInfo.ThirtyDaySales,
                                            ElevenDaySales = pdInfo.ElevenDaySales
                                        };
                                        addDetailsList.Add(purchasingDetailInfo);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    if (addDetailsList.Count > 0)
                    {
                        foreach (var purchasingDetailInfo in addDetailsList)
                        {
                            purchasingDetailInfo.Price = 0;
                            plist.Add(purchasingDetailInfo);
                        }
                    }

                    #endregion

                    #region [非现返生成借记单]
                    var debitNoteDetailList = new List<DebitNoteDetailInfo>();
                    foreach (var pdInfo in plist2)
                    {
                        if (!dicGoods.ContainsKey(pdInfo.GoodsName)) continue;
                        var goodsBaseInfo = dicGoods[pdInfo.GoodsName];
                        PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsBaseInfo && w.HostingFilialeId==pInfo.PurchasingFilialeId);
                        if (purchaseSetInfo != null)
                        {
                            IList<PurchasePromotionInfo> ppList = _purchasePromotionManager.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId,purchaseSetInfo.WarehouseId,purchaseSetInfo.HostingFilialeId,
                                (int)PurchasePromotionType.NoBack);
                            PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                            if (ppInfo != null)
                            {
                                int pQuantity = Convert.ToInt32(pdInfo.PlanQuantity);
                                //赠品数量=原采购数量/买几个*送几个
                                int quantity = pQuantity / ppInfo.BuyCount * ppInfo.GivingCount;
                                #region  新增
                                //按商品总数量进行赠送
                                if (!ppInfo.IsSingle)
                                {
                                    if (!dics.ContainsKey(pdInfo.GoodsName))
                                    {
                                        var dataList = plist2.Where(act => act.GoodsName == pdInfo.GoodsName).ToList();
                                        //单光度赠送商品总数
                                        var total = dataList.Sum(act => (Convert.ToInt32(act.PlanQuantity) / (ppInfo.BuyCount + ppInfo.GivingCount)));
                                        //总商品赠送商品总数
                                        var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) / (ppInfo.BuyCount + ppInfo.GivingCount);
                                        if (sumTotal > total)
                                            dics.Add(pdInfo.GoodsName, sumTotal);
                                    }
                                }
                                #endregion
                                if (quantity > 0)
                                {
                                    var debitNoteDetailInfo = new DebitNoteDetailInfo
                                    {
                                        PurchasingId = pInfo.PurchasingID,
                                        GoodsId = pdInfo.GoodsID,
                                        GoodsName = pdInfo.GoodsName,
                                        Specification = pdInfo.Specification,
                                        GivingCount = quantity,
                                        ArrivalCount = 0,
                                        Price = pdInfo.Price,
                                        State = 0,
                                        Amount = quantity * pdInfo.Price,
                                        Memo = "",
                                        Id = Guid.NewGuid()
                                    };
                                    if (debitExtraDics.ContainsKey(pdInfo.GoodsName))
                                    {
                                        debitExtraDics[pdInfo.GoodsName] = debitExtraDics[pdInfo.GoodsName] + quantity;
                                    }
                                    else
                                    {
                                        debitExtraDics.Add(pdInfo.GoodsName, quantity);
                                    }
                                    debitNoteDetailList.Add(debitNoteDetailInfo);
                                }
                            }
                        }
                    }

                    #endregion

                    #region 处理额外赠送商品
                    foreach (var dic in dics)
                    {
                        KeyValuePair<string, int> dic1 = dic;
                        if (sendDics.ContainsKey(dic.Key))
                        {
                            var total = dic1.Value - sendDics[dic.Key];
                            if (total > 0)
                            {
                                var data = plist.Where(act => act.GoodsName == dic1.Key && act.PurchasingGoodsType != (int)PurchasingGoodsType.Gift)
                                .OrderByDescending(act => act.PlanQuantity).ToList();
                                var data2 = plist.Where(act => act.GoodsName == dic1.Key && act.PurchasingGoodsType == (int)PurchasingGoodsType.Gift)
                                    .OrderByDescending(act => act.PlanQuantity);
                                for (int i = 0; i < total; i++)
                                {
                                    var purchasingDetailInfo = data2.FirstOrDefault(w => w.GoodsID == plist[i].GoodsID);
                                    if (purchasingDetailInfo != null)
                                    {
                                        data[i].PlanQuantity -= 1;
                                        purchasingDetailInfo.PlanQuantity += 1;
                                    }
                                }
                            }
                        }
                        if (debitExtraDics.ContainsKey(dic.Key))
                        {
                            var total = dic1.Value - debitExtraDics[dic.Key];
                            if (total > 0)
                            {
                                var data = debitNoteDetailList.Where(act => act.GoodsName == dic1.Key).OrderByDescending(act => act.GivingCount).ToList();
                                for (int i = 0; i < total; i++)
                                {
                                    data[i].GivingCount += 1;
                                    data[i].Amount = data[i].Price * data[i].GivingCount;
                                }
                            }
                        }
                    }
                    #endregion

                    var originalDetails = _purchasingDetailManager.Select(pInfo.PurchasingID);

                    _purchasingDetailManager.Delete(pInfo.PurchasingID);
                    _purchasing.DeleteById(pInfo.PurchasingID);

                    //pInfo.IsOut = true;
                    //插入采购单
                    _purchasing.PurchasingInsert(pInfo);
                    //保存采购单明细
                    foreach (var item in plist.Where(act=>act.Price==0 && act.PurchasingGoodsType!=(int)PurchasingGoodsType.Gift))
                    {
                        var info = plist.FirstOrDefault(act => act.GoodsName == item.GoodsName && act.Price > 0);
                        item.Price = info!=null?info.Price:0;
                    }

                    var ids=new List<Guid>();
                    if (originalDetails.Count>0)
                    {
                        foreach (var purchasingDetailInfo in originalDetails)
                        {
                            var info = plist.FirstOrDefault(act =>
                                act.GoodsID == purchasingDetailInfo.GoodsID &&
                                act.PurchasingGoodsType == purchasingDetailInfo.PurchasingGoodsType);
                            if (info != null)
                            {
                                purchasingDetailInfo.PlanQuantity += info.PlanQuantity;
                                ids.Add(info.PurchasingGoodsID);
                            }
                            _purchasingDetailManager.Insert(purchasingDetailInfo);
                        }
                    }
                    foreach (var item in plist.Where(act => !ids.Contains(act.PurchasingGoodsID)))
                    {
                        _purchasingDetailManager.Insert(item);
                    }
                    
                    //添加借记单
                    if (debitNoteDetailList.Count > 0)
                    {
                        IDebitNote debitNoteManager = new DebitNote(Environment.GlobalConfig.DB.FromType.Write);
                        var debitNoteInfo = new DebitNoteInfo
                        {
                            PurchasingId = pInfo.PurchasingID,
                            PurchasingNo = pInfo.PurchasingNo,
                            CompanyId = pInfo.CompanyID,
                            PresentAmount = debitNoteDetailList.Sum(w => w.Amount),
                            CreateDate = DateTime.Now,
                            FinishDate = DateTime.MinValue,
                            State = (int)DebitNoteState.ToPurchase,
                            WarehouseId = pInfo.WarehouseID,
                            Memo = "",
                            PersonResponsible = pInfo.PersonResponsible,
                            NewPurchasingId = Guid.Empty
                        };
                        debitNoteManager.AddPurchaseSetAndDetail(debitNoteInfo, debitNoteDetailList);
                    }
                }
                ts.Complete();
            }
            msg = string.Empty;
            return true;
        }

        #region WMS进货申报

        public void BuilderPurchasing(List<PurchaseDeclarationDTO> stockDeclareDtos,IList<PurchaseSetInfo> purchaseSetList, Guid warehouseId, DateTime? arrivalTime, string operationer, ref StringBuilder failMessage)
        {
            var goodsIdList = stockDeclareDtos.Select(w => w.GoodsId).Distinct().ToList();
            var goodsBaseInfos=_goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList);
            if (goodsBaseInfos == null || goodsBaseInfos.Count == 0 || goodsIdList.Count!=goodsBaseInfos.Count)
            {
                failMessage=new StringBuilder("获取商品信息失败!");
                return;
            }
            var goodsBaseDics = goodsBaseInfos.ToDictionary(k => k.GoodsId, v => v);
            if (purchaseSetList == null)
            {
                purchaseSetList =_purchaseSet.GetPurchaseSetList(goodsIdList, warehouseId);
            }
            var purDics = new Dictionary<PurchasingInfo, IList<PurchasingDetailInfo>>();
            var purchasingLists=new List<PurchasingInfo>(); 
            var goodsDics = new Dictionary<string, Guid>();
            
            foreach (var gdiInfoGroup in stockDeclareDtos.GroupBy(ent=>new {ent.FilialeId,ent.CompanyId,ent.PersonResponsible}))
            {
                var dataList = _purchasing.GetPurchasingList(DateTime.MinValue, DateTime.MinValue, gdiInfoGroup.Key.CompanyId, warehouseId, gdiInfoGroup.Key.FilialeId, PurchasingState.NoSubmit,
                    PurchasingType.StockDeclare, string.Empty, Guid.Empty, gdiInfoGroup.Key.PersonResponsible);
                if (dataList != null && dataList.Count > 0)
                    purchasingLists.AddRange(dataList);

                foreach (var item in gdiInfoGroup.Where(ent=>goodsBaseDics.ContainsKey(ent.GoodsId)))
                {
                    if (!goodsDics.ContainsKey(item.GoodsName))
                    {
                        goodsDics.Add(item.GoodsName, item.GoodsId);
                    }
                    var goodsBaseInfo = goodsBaseDics[item.GoodsId];
                    Guid pGuid = Guid.NewGuid();
                    var existPurchsingInfo = purchasingLists.FirstOrDefault(ent => ent.CompanyID==gdiInfoGroup.Key.CompanyId && ent.PurchasingFilialeId==gdiInfoGroup.Key.FilialeId && ent.PersonResponsible== gdiInfoGroup.Key.PersonResponsible);
                    var tempPurchasingInfo = purDics.Keys.FirstOrDefault(ent => ent.CompanyID == gdiInfoGroup.Key.CompanyId && ent.PurchasingFilialeId == gdiInfoGroup.Key.FilialeId && ent.PersonResponsible == gdiInfoGroup.Key.PersonResponsible);
                    var flag = tempPurchasingInfo == null || tempPurchasingInfo.PurchasingID == Guid.Empty;
                    
                    if (existPurchsingInfo == null)
                    {
                        IList<PurchasingDetailInfo> pdList;
                        if (flag)
                        {
                            tempPurchasingInfo = new PurchasingInfo(pGuid, new CodeManager().GetCode(CodeType.PH),
                                                                  item.CompanyId, item.CompanyName, item.FilialeId,
                                                                  warehouseId, (int)PurchasingState.NoSubmit
                                                                  , (int)PurchasingType.StockDeclare, DateTime.Now,
                                                                  DateTime.MaxValue
                                                                  , string.Format("[采购类别:{0};采购人:{1}]", Enum.Attribute.EnumAttribute.GetKeyName(PurchasingType.StockDeclare), operationer),
                                                                  Guid.Empty, string.Empty, operationer)
                            {
                                Director = item.PersonResponsibleName,
                                PersonResponsible = item.PersonResponsible,
                                PurchasingFilialeId = item.FilialeId
                            };
                            pdList = new List<PurchasingDetailInfo>();
                        }
                        else
                        {
                            pGuid = tempPurchasingInfo.PurchasingID;
                            pdList = purDics[tempPurchasingInfo];
                        }
                        if (arrivalTime != null)
                        {
                            tempPurchasingInfo.ArrivalTime = (DateTime)arrivalTime;
                        }
                        
                        var detailInfo = new PurchasingDetailInfo(pGuid, item.RealGoodsId,
                                                                                   item.GoodsName,
                                                                                   goodsBaseInfo.Units,
                                                                                   goodsBaseInfo.GoodsCode,
                                                                                   item.Sku, item.CompanyId
                                                                                   , item.PurchasePrice<=0?-1: item.PurchasePrice, item.Quantity, 0,
                                                                                   0, string.Empty, Guid.NewGuid(),
                                                                                   (int)PurchasingGoodsType.NoGift)
                        {
                            CPrice = item.PurchasePrice == 0 ? -1 : item.PurchasePrice
                        };
                        //获取商品的60、30、11天销量
                        var pdInfo = _purchasingDetailManager.GetChildGoodsSale(detailInfo.GoodsID, warehouseId, DateTime.Now,item.FilialeId);
                        if (pdInfo != null)
                        {
                            detailInfo.SixtyDaySales = pdInfo.SixtyDaySales;
                            detailInfo.ThirtyDaySales = pdInfo.ThirtyDaySales;
                            detailInfo.ElevenDaySales = pdInfo.ElevenDaySales / 11;//日均销量(11天)
                        }
                        else
                        {
                            detailInfo.SixtyDaySales = 0;
                            detailInfo.ThirtyDaySales = 0;
                            detailInfo.ElevenDaySales = 0;
                        }
                        if (flag)
                        {
                            pdList.Add(detailInfo);
                            purDics.Add(tempPurchasingInfo, pdList);
                        }
                        else
                        {
                            var tempDetailInfo = pdList.FirstOrDefault(act => act.GoodsID == item.RealGoodsId && act.PurchasingGoodsType == (int)PurchasingGoodsType.NoGift);
                            if (tempDetailInfo == null)
                            {
                                pdList.Add(detailInfo);
                            }
                            else
                            {
                                tempDetailInfo.PlanQuantity += item.PurchaseQuantity;
                            }
                            purDics[tempPurchasingInfo] = pdList;
                        }
                        purchasingLists.Add(tempPurchasingInfo);
                    }
                    else 
                    {
                        IList<PurchasingDetailInfo> pdList = new List<PurchasingDetailInfo>();
                        var detailInfo = new PurchasingDetailInfo(existPurchsingInfo.PurchasingID,
                                                                  item.RealGoodsId, item.GoodsName,
                                                                  goodsBaseInfo.Units,
                                                                  goodsBaseInfo.GoodsCode,
                                                                  item.Sku, item.CompanyId
                                                                  , item.PurchasePrice <= 0 ? -1 : item.PurchasePrice, item.Quantity, 0,
                                                                  0, "", Guid.NewGuid(),
                                                                  (int)PurchasingGoodsType.NoGift)
                        {
                            CPrice = item.PurchasePrice == 0 ? -1 : item.PurchasePrice
                        };

                        // 获取商品的60、30、11天销量
                        var purchasingDetailInfo = _purchasingDetailManager.GetChildGoodsSale(detailInfo.GoodsID,warehouseId, DateTime.Now,item.FilialeId);
                        if (purchasingDetailInfo != null)
                        {
                            detailInfo.SixtyDaySales = purchasingDetailInfo.SixtyDaySales;
                            detailInfo.ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales;
                            detailInfo.ElevenDaySales = purchasingDetailInfo.ElevenDaySales / 11;//日均销量(11天)
                        }
                        else
                        {
                            detailInfo.SixtyDaySales = 0;
                            detailInfo.ThirtyDaySales = 0;
                            detailInfo.ElevenDaySales = 0;
                        }
                        if (!flag) //已经添加采购单
                        {
                            pdList = purDics[tempPurchasingInfo];
                        }
                        else
                        {
                            tempPurchasingInfo = existPurchsingInfo;
                            purDics.Add(tempPurchasingInfo, pdList);
                        }
                        var tempDetailInfo = pdList.FirstOrDefault(act => act.GoodsID == detailInfo.GoodsID
                                && act.PurchasingGoodsType == (int)PurchasingGoodsType.NoGift);
                        if (tempDetailInfo == null)
                        {
                            pdList.Add(detailInfo);
                        }
                        else
                        {
                            tempDetailInfo.PlanQuantity += detailInfo.PlanQuantity;
                        }
                        purDics[tempPurchasingInfo] = pdList;
                    }
                }
            }

            #region  采购单重新分配

            string msg;
            var result = CreatePurchasing(purDics, goodsDics, purchaseSetList, out msg);
            if (!result)
            {
                failMessage = new StringBuilder(msg);
            }

            #endregion
        }

        public List<PurchaseDeclarationDTO> GetDataList(Guid warehouseId,Dictionary<Guid, List<PurchaseDeclarationDTO>> dics,
            Dictionary<Guid, Dictionary<Guid, int>> purchasingGoodsDics, Dictionary<Guid,ChildGoodsInfo> goodsInfos,IList<PurchaseSetInfo> purchaseSetInfos)
        {
            var purchasingSetDics = purchaseSetInfos.GroupBy(ent => ent.HostingFilialeId).ToDictionary(k => k.Key, v => v.ToDictionary(kk => kk.GoodsId, vv => vv));
            var allDeclaretionInfos = dics.Where(ent => goodsInfos.ContainsKey(ent.Key)).SelectMany(ent => ent.Value);
            List<PurchaseDeclarationDTO> newDatasource = new List<PurchaseDeclarationDTO>();

            var companyCussents = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable).ToDictionary(k => k.CompanyId, v => v.CompanyName);
            foreach (var filiaeAndGoodsGroup in allDeclaretionInfos.Where(ent => goodsInfos.ContainsKey(ent.RealGoodsId) && purchasingSetDics.ContainsKey(ent.FilialeId) && purchasingSetDics[ent.FilialeId].ContainsKey(ent.GoodsId)).GroupBy(ent => new { ent.FilialeId, ent.GoodsId }))
            {
                var purchaseSetInfo = purchasingSetDics[filiaeAndGoodsGroup.Key.FilialeId][filiaeAndGoodsGroup.Key.GoodsId];
                foreach (var item in filiaeAndGoodsGroup)
                {
                    var goodsInfo = goodsInfos[item.RealGoodsId];
                    item.GoodsCode = goodsInfo.GoodsCode;
                    item.Units = goodsInfo.Units;
                    item.GoodsName = goodsInfo.GoodsName;
                    item.Sku = goodsInfo.Specification;
                    item.CompanyName = companyCussents.ContainsKey(purchaseSetInfo.CompanyId) ? companyCussents[purchaseSetInfo.CompanyId] : "";
                    item.CompanyId = purchaseSetInfo.CompanyId;
                    item.PersonResponsible = purchaseSetInfo.PersonResponsible;
                    item.PersonResponsibleName = purchaseSetInfo.PersonResponsibleName;
                    item.PurchasePrice = purchaseSetInfo.PurchasePrice;
                    if (purchasingGoodsDics.ContainsKey(item.FilialeId) &&
                        purchasingGoodsDics[item.FilialeId].ContainsKey(item.RealGoodsId))
                    {
                        item.Quantity -= purchasingGoodsDics[item.FilialeId][item.RealGoodsId];
                    }
                    if (item.Quantity > 0)
                        newDatasource.Add(item);
                }
            }
            return newDatasource;
        }
        #endregion
    }
}
