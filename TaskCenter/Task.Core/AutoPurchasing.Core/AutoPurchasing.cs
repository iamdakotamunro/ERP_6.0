using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoPurchasing.Core.Model;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;

namespace AutoPurchasing.Core
{
    public class AutoPurchasing
    {

        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private static readonly IPurchasing _purchasing = new Purchasing(ERP.Environment.GlobalConfig.DB.FromType.Write);
        private static readonly PurchasingDetailManager _purchasingDetailBll = new PurchasingDetailManager(ERP.Environment.GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasePromotion _purchasePromotionBll = new PurchasePromotion(ERP.Environment.GlobalConfig.DB.FromType.Write);
        private static readonly IDebitNote _debitNoteBll = new DebitNote(ERP.Environment.GlobalConfig.DB.FromType.Write);


        public static void RunTask()
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            int week = GetWeek(now);
            if (week == 1 || week == 3)
            {
                #region --> 今天周一或周三
                var pg = DataAccessor.GetPurchasingGoodsList(TaskType.All);
                var goodsIdList = pg.Select(w => w.GoodsId).Distinct().ToList();
                IList<GoodsInfo> goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList).ToList();
                if (goodsList.Count == 0) return;

                var dictPurchase = new Dictionary<PurchasingInfo, List<PurchasingDetailInfo>>();
                var purchasingSets = new List<PurchasingGoods>();
                pg = pg.Where(w => w.StockUpDay == week && w.HostingFilialeId!=Guid.Empty).ToList();
                var goodsDics = goodsList.ToDictionary(k => k.GoodsId, v => v);
                var realGoodsDics = new Dictionary<Guid,GoodsInfo>();

                Dictionary<Guid,List<Guid>> goodsWithRealGoods=new Dictionary<Guid, List<Guid>>();
                foreach (var warehouseIdGroup in pg.GroupBy(act => new { act.WarehouseId,act.HostingFilialeId }))
                {
                    var warehouseId = warehouseIdGroup.Key.WarehouseId;
                    var hostingFilialeId = warehouseIdGroup.Key.HostingFilialeId;
                   var stockStatistics = WMSSao.GetStockStatisticsDtosForAuto(warehouseId, hostingFilialeId);
                    var planPurchasingGoods1 = DataAccessor.GetAllSumPurchasingQuantity(warehouseId, hostingFilialeId).ToDictionary(k=>k.GoodsID,v=>v.PurchasingQuantity); 
                    foreach (var companyIdGroup in warehouseIdGroup.GroupBy(act => act.CompanyId))
                    {
                        var purchaseGroupIds = companyIdGroup.Select(w => w.PurchaseGroupId).Distinct().ToList();
                        foreach (var purchaseGroupId in purchaseGroupIds)
                        {
                            //采购分组
                            var pgList = companyIdGroup.Where(w => w.PurchaseGroupId == purchaseGroupId).ToList();
                            var personResponsibles = pgList.Select(w => w.PersonResponsible).Distinct().ToList();
                            foreach (var personResponsible in personResponsibles)
                            {
                                var prList = pgList.Where(w => w.PersonResponsible == personResponsible).ToList();
                                if (prList.Count > 0)
                                {
                                    foreach (var prInfo in prList.Where(ent=>goodsDics.ContainsKey(ent.GoodsId)))
                                    {
                                        GoodsInfo goodsInfo = goodsDics[prInfo.GoodsId];
                                        if(goodsInfo==null)continue;
                                        prInfo.GoodsName = goodsInfo.GoodsName;
                                        prInfo.GoodsCode = goodsInfo.GoodsCode;
                                        prInfo.Units = goodsInfo.Units;
                                        if (goodsInfo.ExpandInfo != null)
                                            prInfo.PackQuantity = goodsInfo.ExpandInfo.PackCount;
                                        List<Guid> realGoodsIdList;
                                        if (goodsWithRealGoods.ContainsKey(prInfo.GoodsId))
                                        {
                                            realGoodsIdList = goodsWithRealGoods[prInfo.GoodsId];
                                        }
                                        else
                                        {
                                            realGoodsIdList = _goodsCenterSao.GetRealGoodsIdsByGoodsId(prInfo.GoodsId).ToList();
                                            if (realGoodsIdList.Count == 0)
                                                realGoodsIdList.Add(prInfo.GoodsId);
                                            goodsWithRealGoods.Add(prInfo.GoodsId,realGoodsIdList);
                                        }
                                        
                                        int stockUpDays = prInfo.FilingForm == 1 ? GetStockUpDays(prInfo) : prInfo.FilingTrigger;
                                        if (stockUpDays == 0) continue;
                                        var childGoodsSaleAll = GetChildGoodsSaleTotalByDays(realGoodsIdList, new List<Guid> { prInfo.WarehouseId }, stockUpDays, now);

                                        if (childGoodsSaleAll.Count > 0)
                                        {
                                            foreach (var sale in childGoodsSaleAll)
                                            {
                                                sale.PlanPurchasingquantity = sale.WeightedAverageSaleQuantity * stockUpDays;
                                                if (sale.PlanPurchasingquantity > 0)
                                                {
                                                    //个位数0<x<=5向上取为5，个位数为6<x<=9向上取整为10
                                                    char[] temp = sale.PlanPurchasingquantity.ToString(CultureInfo.InvariantCulture).ToArray();
                                                    int unitsDigit = Convert.ToInt32(temp[temp.Length - 1]);
                                                    if (unitsDigit > 0 && unitsDigit < 5)
                                                    {
                                                        sale.PlanPurchasingquantity += 5 - unitsDigit;
                                                    }
                                                    else if (unitsDigit > 5)
                                                    {
                                                        sale.PlanPurchasingquantity += 10 - unitsDigit;
                                                    }
                                                }

                                                //计算当前的采购商品数量,包含扣除的已经采购完成和部分采购完成和赠品类型
                                                sale.SubtractPurchasingQuantity = planPurchasingGoods1.ContainsKey(sale.GoodsId)? planPurchasingGoods1[sale.GoodsId] : 0;

                                                //计算当前仓库存货数量
                                                sale.NonceWarehouseStockQuantity = stockStatistics.Where(ent => ent.RealGoodsId == sale.GoodsId).Sum(info => info.CurrentStock + info.UppingQuantity - info.RequireQuantity -info.SubtotalQuantity); 
                                                if (prInfo.FilingForm == 2)
                                                {
                                                    //2触发报备
                                                    //计算是否满足不足数量
                                                    double planQuantity = sale.WeightedAverageSaleQuantity * prInfo.Insufficient;
                                                    double realityQuantity = Math.Ceiling(planQuantity - sale.SubtractPurchasingQuantity - sale.NonceWarehouseStockQuantity);
                                                    if (realityQuantity <= 0)
                                                    {
                                                        //无需报备
                                                        continue;
                                                    }
                                                }
                                                if (sale.RealityNeedPurchasingQuantity > 0)
                                                {
                                                    var salesinfo = DataAccessor.GetChildGoodsSale(sale.GoodsId, warehouseId, sale.HostingFilialeId, DateTime.Now);
                                                    PurchasingInfo existingPurchasingInfo = null;
                                                    bool isExist = false;
                                                    if (dictPurchase.Keys.Count > 0)
                                                        existingPurchasingInfo = dictPurchase.Keys.FirstOrDefault(act => act.PersonResponsible == personResponsible && act.PurchasingFilialeId== hostingFilialeId && act.WarehouseID == warehouseId && companyIdGroup.Key == act.CompanyID);
                                                    var detailList = new List<PurchasingDetailInfo>();
                                                    if (existingPurchasingInfo == null)
                                                    {
                                                        existingPurchasingInfo = new PurchasingInfo
                                                        {
                                                            PurchasingID = Guid.NewGuid(),
                                                            CompanyID = companyIdGroup.Key,
                                                            CompanyName = prList[0].CompanyName,
                                                            WarehouseID = warehouseId,
                                                            PurchasingState = (int)PurchasingState.NoSubmit,
                                                            PurchasingType = (int)PurchasingType.AutoStock,
                                                            PersonResponsible = personResponsible,
                                                            PurchaseGroupId = purchaseGroupId,
                                                            StartTime = DateTime.Now,
                                                            EndTime = DateTime.MaxValue,
                                                            Description = string.Format("[采购类别:{0};系统自动报备]", EnumAttribute.GetKeyName(PurchasingType.Custom)),
                                                            FilialeID = hostingFilialeId,
                                                            PurchasingFilialeId = hostingFilialeId
                                                        };
                                                    }
                                                    else
                                                    {
                                                        isExist = true;
                                                        detailList = dictPurchase[existingPurchasingInfo];
                                                    }
                                                    var purchasingDetailInfo = new PurchasingDetailInfo
                                                    {
                                                        PurchasingID = existingPurchasingInfo.PurchasingID,
                                                        GoodsID = sale.GoodsId,
                                                        GoodsName = prInfo.GoodsName,
                                                        GoodsCode = prInfo.GoodsCode,
                                                        Specification = sale.Specification,
                                                        CompanyID = companyIdGroup.Key,
                                                        Price = prInfo.Price,
                                                        PlanQuantity = sale.RealityNeedPurchasingQuantity,
                                                        RealityQuantity = 0,
                                                        State = 0,
                                                        Units = prInfo.Units,
                                                        PurchasingGoodsID = Guid.NewGuid(),
                                                        SixtyDaySales = salesinfo.SixtyDaySales,
                                                        ThirtyDaySales = salesinfo.ThirtyDaySales,
                                                        ElevenDaySales = salesinfo.ElevenDaySales,
                                                        CPrice = prInfo.Price
                                                    };
                                                    detailList.Add(purchasingDetailInfo);

                                                    if (!realGoodsDics.ContainsKey(sale.GoodsId))
                                                    {
                                                        realGoodsDics.Add(sale.GoodsId,goodsInfo);
                                                    }
                                                    if (detailList.Count > 0 && !isExist)
                                                    {
                                                        dictPurchase.Add(existingPurchasingInfo, detailList);
                                                    }

                                                }
                                            }
                                            if (!purchasingSets.Any(act => act.WarehouseId == warehouseId && act.HostingFilialeId==hostingFilialeId && act.GoodsId == prInfo.GoodsId))
                                            {
                                                purchasingSets.Add(prInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                PurchasingPromotion(dictPurchase, realGoodsDics, purchasingSets);
                #endregion
            }
        }

        #region  采购促销处理

        private static void PurchasingPromotion(Dictionary<PurchasingInfo, List<PurchasingDetailInfo>> dictPurchase, Dictionary<Guid, GoodsInfo> dicGoods, IList<PurchasingGoods> pg)
        {
            if (dictPurchase.Count > 0)
            {
                //IList<PurchaseSetInfo> purchaseSetList = _purchaseSetBll.GetPurchaseSetList();
                foreach (KeyValuePair<PurchasingInfo, List<PurchasingDetailInfo>> keyValuePair in dictPurchase)
                {
                    keyValuePair.Key.PurchasingNo = DataAccessor.GetCode(BaseInfo.CodeType.PH);
                    var pInfo = keyValuePair.Key;
                    IList<PurchasingDetailInfo> plist = keyValuePair.Value;
                    //pInfo.IsOut = true;
                    _purchasing.PurchasingInsert(pInfo);

                    //非赠品采购商品
                    IList<PurchasingDetailInfo> plist2 = plist.Where(w => w.PurchasingGoodsType != (int)PurchasingGoodsType.Gift).ToList();

                    #region [现返]
                    //如果赠送方式为总数量赠送时使用  key 主商品ID  value 额外赠送 
                    var dics = new Dictionary<string, int>();   //需赠送
                    var debitExtraDics = new Dictionary<string, int>(); //借计单已送
                    var addDetailsList = new List<PurchasingDetailInfo>(); //添加赠品
                                                                           //处理原理：
                                                                           //赠品数量=原采购数量/(买几个+送几个)*送几个
                                                                           //购买数量=原采购数量-赠品数量
                    foreach (var pdInfo in plist2)
                    {
                        var goodsBaseInfo = new GoodsInfo();
                        if (dicGoods != null)
                        {
                            bool hasKey = dicGoods.ContainsKey(pdInfo.GoodsID);
                            if (hasKey)
                            {
                                goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == pdInfo.GoodsID).Value;
                            }
                        }
                        PurchasingGoods purchaseSetInfo = pg.FirstOrDefault(w => w.GoodsId == goodsBaseInfo.GoodsId && w.WarehouseId == pInfo.WarehouseID);
                        if (purchaseSetInfo == null) continue;
                        IList<PurchasePromotionInfo> ppList = _purchasePromotionBll.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId, purchaseSetInfo.WarehouseId, pInfo.PurchasingFilialeId, (int)PurchasePromotionType.Back);
                        PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                        if (ppInfo != null)
                        {
                            #region  新增
                            if (!ppInfo.IsSingle)  //按商品总数量进行赠送
                            {
                                if (!dics.ContainsKey(goodsBaseInfo.GoodsName))
                                {
                                    var dataList = plist2.Where(act => act.GoodsName == goodsBaseInfo.GoodsName).OrderByDescending(act => act.PlanQuantity).ToList();
                                    //余数
                                    var extra = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) % (ppInfo.BuyCount + ppInfo.GivingCount);
                                    if (extra == ppInfo.BuyCount)
                                    {
                                        dataList[0].PlanQuantity += ppInfo.GivingCount;
                                    }
                                    else
                                    {
                                        //应赠送次数
                                        int actquantity = extra % (ppInfo.BuyCount + ppInfo.GivingCount);
                                        if (actquantity >= (ppInfo.BuyCount / float.Parse("2.0")))
                                        {
                                            dataList[0].PlanQuantity = dataList[0].PlanQuantity + (ppInfo.BuyCount + ppInfo.GivingCount) - actquantity;
                                        }
                                    }
                                    //总商品赠送商品总数
                                    var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) / (ppInfo.BuyCount + ppInfo.GivingCount);
                                    dics.Add(goodsBaseInfo.GoodsName, sumTotal);
                                    if (dics[goodsBaseInfo.GoodsName] > 0)
                                    {
                                        foreach (var item in dataList.OrderByDescending(act => act.PlanQuantity))
                                        {
                                            var count = dics[item.GoodsName] - Convert.ToInt32(item.PlanQuantity);
                                            if (count > 0)  //赠品数>购买数
                                            {
                                                item.Price = 0;
                                                item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                                item.RealityQuantity = 0;
                                                dics[item.GoodsName] = count;
                                            }
                                            else if (count < 0)
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
                                            else
                                            {
                                                item.Price = 0;
                                                item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                                item.RealityQuantity = 0;
                                                item.CPrice = 0;
                                                dics[item.GoodsName] = 0;
                                            }
                                            if (dics[item.GoodsName] == 0) break;
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
                                    if (actquantity >= ((ppInfo.BuyCount + ppInfo.GivingCount) / float.Parse("2.0")))
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
                    #endregion

                    #region [非现返生成借记单]

                    var debitNoteDetailList = new List<DebitNoteDetailInfo>();
                    foreach (var pdInfo in plist2)
                    {
                        var goodsBaseInfo = new GoodsInfo();
                        if (dicGoods != null)
                        {
                            bool hasKey = dicGoods.ContainsKey(pdInfo.GoodsID);
                            if (hasKey)
                            {
                                goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == pdInfo.GoodsID).Value;
                            }
                        }
                        PurchasingGoods purchaseSetInfo = pg.FirstOrDefault(w => w.GoodsId == goodsBaseInfo.GoodsId && w.WarehouseId == pInfo.WarehouseID);
                        if (purchaseSetInfo != null)
                        {
                            IList<PurchasePromotionInfo> ppList = _purchasePromotionBll.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId, purchaseSetInfo.WarehouseId,pInfo.PurchasingFilialeId, (int)PurchasePromotionType.NoBack);
                            PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                            if (ppInfo != null)
                            {
                                int pQuantity = Convert.ToInt32(pdInfo.PlanQuantity);
                                //赠品数量=原采购数量/买几个*送几个
                                #region  新增
                                //按商品总数量进行赠送
                                if (!ppInfo.IsSingle && dicGoods != null)
                                {
                                    if (!dics.ContainsKey(goodsBaseInfo.GoodsName))
                                    {
                                        var dataList = plist2.Where(act => act.GoodsName == goodsBaseInfo.GoodsName).ToList();
                                        //单光度赠送商品总数
                                        var total = dataList.Sum(act => (Convert.ToInt32(act.PlanQuantity) / (ppInfo.BuyCount + ppInfo.GivingCount)));
                                        //总商品赠送商品总数
                                        var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) / (ppInfo.BuyCount + ppInfo.GivingCount);
                                        if (sumTotal > total)
                                            dics.Add(goodsBaseInfo.GoodsName, sumTotal);
                                    }
                                }
                                #endregion
                                int quantity = pQuantity / ppInfo.BuyCount * ppInfo.GivingCount;
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
                                    debitNoteDetailList.Add(debitNoteDetailInfo);
                                }
                            }
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

                    #region 处理额外赠送商品
                    foreach (var dic in dics)
                    {
                        KeyValuePair<string, int> dic1 = dic;
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

                    foreach (var item in plist.Where(act => act.Price == 0 && act.PurchasingGoodsType != (int)PurchasingGoodsType.Gift))
                    {
                        var info = plist.FirstOrDefault(act => act.GoodsName == item.GoodsName && act.Price > 0);
                        item.Price = info != null ? info.Price : 0;
                    }
                    //保存采购单明细
                    _purchasingDetailBll.Save(plist);

                    //添加借记单
                    if (debitNoteDetailList.Count > 0)
                    {
                        var debitNoteInfo = new DebitNoteInfo
                        {
                            PurchasingId = pInfo.PurchasingID,
                            PurchaseGroupId = pInfo.PurchaseGroupId,
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
                        _debitNoteBll.AddPurchaseSetAndDetail(debitNoteInfo, debitNoteDetailList);
                    }
                }

                if (dicGoods != null && dicGoods.Count > 0)
                {
                    var completeGoodsIds = dicGoods.Select(keyValuePair => keyValuePair.Value.GoodsId).ToList();
                    foreach (var item in pg.Where(ent=>completeGoodsIds.Contains(ent.GoodsId)))
                    {
                        DataAccessor.UpdateLastPurchasingDate(item.WarehouseId,item.HostingFilialeId,item.GoodsId,DateTime.Now);
                    }
                }
            }
        }
        #endregion

        /// <summary> 第几周
        /// </summary>
        /// <returns></returns>
        public static int GetWeekOfMonth(DateTime day)
        {
            DateTime firstofMonth = Convert.ToDateTime(day.Date.Year + "-" + day.Date.Month + "-" + 1);
            var i = (int)firstofMonth.Date.DayOfWeek;
            if (i == 0)
            {
                i = 7;
            }
            return (day.Date.Day + i - 2) / 7 + 1;
            //if (WEEK_START == 1)
            //{
            //    //周一至周日 为一周
            //    return (day.Date.Day + i - 2) / 7 + 1;
            //}
            //if (WEEK_START == 2)
            //{
            //    //周日至周六 为一周
            //    return (day.Date.Day + i - 1) / 7;
            //}
            //return 0;//错误返回值0
        }

        /// <summary> 今天星期几
        /// </summary>
        /// <returns></returns>
        public static int GetWeek(DateTime dateTime)
        {
            //string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            int[] weekdays = { 7, 1, 2, 3, 4, 5, 6 };
            int week = weekdays[Convert.ToInt32(dateTime.DayOfWeek)];

            return week;
        }

        /// <summary>
        /// 获得常规报备天数
        /// </summary>
        /// <returns></returns>
        public static int GetStockUpDays(PurchasingGoods pg)
        {
            int stockUpDay = 0;
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);  //该月第一天
            int startDateWeek = GetWeek(startDate);//该月第一天星期几
            int nowDateWeek = GetWeek(DateTime.Now);//今天星期几
            int weekOfMonth = GetWeekOfMonth(DateTime.Now);//当前第几周
            if (nowDateWeek == 1 || nowDateWeek == 3)
            {
                if (nowDateWeek == 1)
                {
                    //今天周一
                    if (startDateWeek != 1)
                    {
                        //该月第一天不是周一，当前第几周 - 1
                        weekOfMonth = weekOfMonth - 1;
                    }
                }
                else if (nowDateWeek == 3)
                {
                    //今天周三
                    if (startDateWeek > 3)
                    {
                        //该月第一天不是周三以及以上，当前第几周 - 1  
                        weekOfMonth = weekOfMonth - 1;
                    }
                }
                switch (weekOfMonth)
                {
                    case 1:
                        stockUpDay = pg.FirstWeek;
                        break;
                    case 2:
                        stockUpDay = pg.SecondWeek;
                        break;
                    case 3:
                        stockUpDay = pg.ThirdWeek;
                        break;
                    case 4:
                        stockUpDay = pg.FourthWeek;
                        break;
                }
            }
            return stockUpDay;
        }


        #region -- 获取商品的总销售统计信息 >> GetChildGoodsSaleTotalByDays

        /// <summary>
        /// 获取商品的总销售统计信息
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseIds"></param>
        /// <param name="stockUpDays"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static List<ChildGoodsSalePurchasing> GetChildGoodsSaleTotalByDays(List<Guid> goodsIdList, List<Guid> warehouseIds, int stockUpDays, DateTime dateTime)
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var totalSales = new List<GoodsDaySalesInfo>();
            var saleDays = new List<GoodsSaleDaysInfo>();

            foreach (var warehouseId in warehouseIds)
            {
                totalSales.AddRange(DataAccessor.GetGoodsDaySalesInfos(goodsIdList, warehouseId, now.AddDays(-110), now));
                saleDays.AddRange(DataAccessor.GetSaleDays(goodsIdList, warehouseId, now));
            }

            //计算前第一个备货周期的销售额
            var childSaleList1 = totalSales.Where(act => act.DayTime >= now.AddDays(-30) && act.DayTime < now);

            //计算前第二个备货周期的销售额
            var childSaleList2 = totalSales.Where(act => act.DayTime >= now.AddDays(-60) && act.DayTime < now.AddDays(-30));

            //计算前第三个备货周期的销售额
            var childSaleList3 = totalSales.Where(act => act.DayTime >= now.AddDays(-90) && act.DayTime < now.AddDays(-60));

            //合并这三个销售额的信息
            var childGoodsSaleAll = MergeSaleList(goodsIdList, childSaleList1, childSaleList2, childSaleList3, totalSales, dateTime, saleDays);

            return childGoodsSaleAll;
        }
        #endregion

        #region -- 合并三个备货周期的销售信息 >> MergeSaleList

        /// <summary>
        /// 合并三个备货周期的销售信息
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="sales1"></param>
        /// <param name="sales2"></param>
        /// <param name="sales3"></param>
        /// <param name="totalSales"></param>
        /// <param name="dateTime"></param>
        /// <param name="saleDays"></param>
        /// <returns></returns>
        public static List<ChildGoodsSalePurchasing> MergeSaleList(List<Guid> goodsIdList, IEnumerable<GoodsDaySalesInfo> sales1, IEnumerable<GoodsDaySalesInfo> sales2, IEnumerable<GoodsDaySalesInfo> sales3,
            IList<GoodsDaySalesInfo> totalSales, DateTime dateTime, List<GoodsSaleDaysInfo> saleDays)
        {
            var childGoodsSaleAll = new List<ChildGoodsSalePurchasing>();
            foreach (var guid in goodsIdList)
            {
                var totals = new List<GoodsDaySalesInfo>();
                var tempSaleList1 = sales1.Where(act => act.RealGoodsID == guid);
                var tempSaleList2 = sales2.Where(act => act.RealGoodsID == guid);
                var tempSaleList3 = sales3.Where(act => act.RealGoodsID == guid);
                totals.AddRange(tempSaleList1);
                totals.AddRange(tempSaleList2);
                totals.AddRange(tempSaleList3);
                childGoodsSaleAll.AddRange(totals.GroupBy(act => act.HostingFilialeId).Select(goodsDaySalesInfoGroup => new ChildGoodsSalePurchasing
                {
                    GoodsId = guid,
                    FirstNumberOneStockUpSale = tempSaleList1.Where(act => act.HostingFilialeId == goodsDaySalesInfoGroup.Key).Sum(act => act.GoodsSales),
                    FirstNumberTwoStockUpSale = tempSaleList2.Where(act => act.HostingFilialeId == goodsDaySalesInfoGroup.Key).Sum(act => act.GoodsSales),
                    FirstNumberThreeStockUpSale = tempSaleList3.Where(act => act.HostingFilialeId == goodsDaySalesInfoGroup.Key).Sum(act => act.GoodsSales),
                    Specification = goodsDaySalesInfoGroup.First().Specification,
                    HostingFilialeId = goodsDaySalesInfoGroup.Key,
                    PerStepSales = GetPreStepSales(totalSales.Where(act => act.RealGoodsID == guid && act.HostingFilialeId == goodsDaySalesInfoGroup.Key), dateTime,
                    saleDays.Where(act => act.RealGoodsId == guid && act.HostingFilialeId == goodsDaySalesInfoGroup.Key).Max(act => act.Days))
                }));
            }
            //返回合并的销售记录信息
            return childGoodsSaleAll;
        }

        private static Dictionary<int, double> GetPreStepSales(IEnumerable<GoodsDaySalesInfo> realGoodsSales, DateTime dateTime, int days)
        {
            var preStepSales = new Dictionary<int, double>();
            if (realGoodsSales == null || !realGoodsSales.Any()) return preStepSales;
            if (days > 0)
            {
                if (days < 90)
                {
                    if (days <= 2 || realGoodsSales.Count() <= 2)
                    {
                        preStepSales.Add(1, realGoodsSales.Sum(act => act.GoodsSales) / realGoodsSales.Count());
                        return preStepSales;
                    }
                    var max = realGoodsSales.Max(act => act.GoodsSales);
                    var min = realGoodsSales.Min(act => act.GoodsSales);
                    var totalSalesNum = realGoodsSales.Sum(act => act.GoodsSales) - max - min;
                    if (totalSalesNum > 0)
                        preStepSales.Add(1, totalSalesNum / (days - 2));
                }
                else
                {
                    var effictiveDays = days / 10;
                    effictiveDays = effictiveDays > 11 ? 11 : effictiveDays;
                    var effitiveSales = effictiveDays == 11 ? realGoodsSales
                        : realGoodsSales.Where(act => act.DayTime >= dateTime.AddDays(-effictiveDays * 10) && act.DayTime < dateTime);
                    var dics = new Dictionary<int, double>();
                    var dayTime = dateTime;
                    for (int i = 1; i <= effictiveDays; i++)
                    {
                        var salesNum = effitiveSales.Where(act =>
                            act.DayTime >= dayTime.AddDays(-10) &&
                            act.DayTime < dayTime).Sum(act => act.GoodsSales);
                        dics.Add(i, salesNum);
                        dayTime = dayTime.AddDays(-10);
                    }

                    if (effictiveDays == 10)
                    {
                        var latelyMax = dics.OrderBy(act => act.Value).ThenByDescending(act => act.Key).Last();
                        dics.Remove(latelyMax.Key);
                    }
                    else
                    {
                        var latelyMax = dics.OrderBy(act => act.Value).ThenByDescending(act => act.Key).Last();
                        var latelyMin = dics.OrderBy(act => act.Value).ThenBy(act => act.Key).First();
                        dics.Remove(latelyMax.Key);
                        dics.Remove(latelyMin.Key);
                    }
                    preStepSales = dics;
                }
            }
            return preStepSales;
        }
        #endregion
    }
}
