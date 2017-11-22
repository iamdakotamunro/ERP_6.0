using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class PurchasingManager : BllInstance<PurchasingManager>
    {
        private readonly IPurchasing _purchasing;
        private readonly IGoodsCenterSao _goodsInfoSao;
        private readonly IPurchasingDetail _purchasingDetail;
        private readonly ICompanyCussent _companyCussent;
        private readonly ProcurementTicketLimitDAL _procurementTicketLimitDal;

        public PurchasingManager(IPurchasing iPurchasing, IGoodsCenterSao goodsInfoSao,
            IPurchasingDetail purchasingDetail, ICompanyCussent companyCussent, ProcurementTicketLimitDAL procurementTicketLimitDal)
        {
            _purchasing = iPurchasing;
            _goodsInfoSao = goodsInfoSao;
            _purchasingDetail = purchasingDetail;
            _companyCussent = companyCussent;
            _procurementTicketLimitDal = procurementTicketLimitDal;
        }

        public PurchasingManager(GlobalConfig.DB.FromType fromType)
        {
            _purchasing = new DAL.Implement.Inventory.Purchasing(fromType);
            _purchasingDetail = new PurchasingDetail(fromType);
            _companyCussent=new CompanyCussent(fromType);
            _procurementTicketLimitDal = new ProcurementTicketLimitDAL(fromType);
        }

        /// <summary>
        /// 修改采购单状态
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="purchasingState"></param>
        public void PurchasingUpdate(Guid purchasingID, PurchasingState purchasingState)
        {
            PurchasingInfo pInfo = _purchasing.GetPurchasingById(purchasingID);
            if (purchasingState == PurchasingState.Deleted)//如果变删除状态
            {
                //只允许删除采购中以前的状态和调价拒绝
                if (pInfo.PurchasingState <= (int)PurchasingState.Purchasing || pInfo.PurchasingState == (int)PurchasingState.Refusing)
                {
                    _purchasing.PurchasingUpdate(purchasingID, purchasingState, pInfo.PurchasingFilialeId);
                }
            }
            else
            {
                _purchasing.PurchasingUpdate(purchasingID, purchasingState, pInfo.PurchasingFilialeId);
            }
        }

        /// <summary>
        /// 删除采购单记录
        /// </summary>
        /// <param name="purchasingId"></param>
        public string Delete(Guid purchasingId)
        {
            PurchasingInfo pInfo = _purchasing.GetPurchasingById(purchasingId);
            if (pInfo.PurchasingState <= 1)
            {
                using (var ts = new TransactionScope())
                {
                    _purchasingDetail.Delete(purchasingId);
                    _purchasing.DeleteById(purchasingId);
                    ts.Complete();
                }
            }
            else
                return pInfo.PurchasingNo;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="realGoodsIds">null(取所有子商品)</param>
        /// <returns></returns>
        public bool SelectPurchasingNoCompleteByGoodsId(Guid goodsId, List<Guid> realGoodsIds)
        {
            if (realGoodsIds == null || realGoodsIds.Count == 0)
                realGoodsIds = _goodsInfoSao.GetRealGoodsIdsByGoodsId(goodsId).ToList();
            return _purchasing.SelectPurchasingNoCompleteByGoodsId(goodsId, realGoodsIds);
        }

        /// <summary>
        /// 新增采购单
        /// </summary>
        /// <param name="purchasingInfo"></param>
        /// <param name="purchasingDetailInfoList"></param>
        /// <returns></returns>
        public bool Insert(PurchasingInfo purchasingInfo, IList<PurchasingDetailInfo> purchasingDetailInfoList)
        {
            if (purchasingInfo == null || purchasingDetailInfoList == null || purchasingDetailInfoList.Count == 0)
            {
                return false;
            }
            _purchasing.PurchasingInsertWithDetails(purchasingInfo, purchasingDetailInfoList);
            return true;
        }

        #region [采购中分配采购公司]

        /// <summary>计算采购单为采购中（分配采购公司）
        /// </summary>
        /// <param name="purchasingId">采购单Id</param>
        public void PurchaseInProcess(Guid purchasingId)
        {
            //获取采购单信息
            var purchasingInfo = _purchasing.GetPurchasingById(purchasingId);
            //获取此采购单供应商绑定我方公司
            var filialeIds = _companyCussent.GetCompanyBindingFiliale(purchasingInfo.CompanyID);
            //获取供应商信息（是否必开票）
            var cinfo = _companyCussent.GetCompanyCussent(purchasingInfo.CompanyID);

            //获取当前采购单供应商当前年月分配的具体采购额度（含剩余额度）
            var dateTime = DateTime.Now;
            IList<ProcurementTicketLimitInfo> ptlList = _procurementTicketLimitDal.GetProcurementTicketLimitDetailByCompanyId(purchasingInfo.CompanyID, dateTime.Year, dateTime.Month);

            //获取采购单商品分组金额集合
            var goodsAmountList = _purchasing.GetGoodsAmountList(purchasingInfo.PurchasingID);
            //采购单商品总金额
            var goodsSumAmount = goodsAmountList.Sum(ent => ent.AmountPrice);
            //采购单分组商品集合
            IList<PurchasingGoodsSplitInfo> purchasingGoodsSplitList = new List<PurchasingGoodsSplitInfo>();

            // 1.该采购单供应商没有绑定我方公司，则获取任意一总公司作为采购公司
            if (filialeIds.Count == 0)
            {
                var filialeId = FilialeManager.GetHeadList().First(ent => ent.Name.Contains("可得")).ID;
                _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, filialeId, false);
                return;
            }
            // 1.未设置采购额度但已绑定我方公司（获取其中一家公司采购）
            if (ptlList.Count == 0 && filialeIds.Count > 0)
            {
                _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, filialeIds[0], false);
                return;
            }
            //1.只绑定一个采购公司
            if (filialeIds.Count == 1 && ptlList.Count == 1)
            {
                if (cinfo.IsNeedInvoices)
                {
                    _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, filialeIds[0], true);
                }
                else
                {
                    var item = ptlList[0];
                    _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, filialeIds[0], goodsSumAmount <= item.SurplusLimit);
                }
                return;
            }

            //采购公司ID
            IList<Guid> filialeIdGuids = new List<Guid>();
            foreach (var item in ptlList)
            {
                if (item.SurplusLimit > 0 || cinfo.IsNeedInvoices)
                {
                    var info = new PurchasingGoodsSplitInfo
                    {
                        FilialeId = item.FilialeId,
                        PurchasingLimit = item.SurplusLimit,
                        TotalPurchasingLimit = item.TakerTicketLimit,
                        IsElseFilialeId = false,
                        PurchasingGoodsItemList = new List<PurchasingGoodsItemInfo>()
                    };
                    purchasingGoodsSplitList.Add(info);
                }
                if (item.SurplusLimit < goodsSumAmount)
                {
                    filialeIdGuids.Add(item.FilialeId);
                }
            }
            //所有采购公司剩余额度全部满足当前采购单，此采购单分配给剩余比例最大的采购公司
            if (filialeIdGuids.Count == 0)
            {
                var maxItem = ptlList.OrderByDescending(ent => ent.SurplusProportion).ThenBy(ent => ent.SurplusLimit).ToList()[0];
                _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, maxItem.FilialeId, true);
                return;
            }
            //如果剩余额度某家不满足当前采购单金额，此采购单分配给剩余比例最大且满足此采购单金额的采购公司
            foreach (var item in ptlList.OrderByDescending(ent => ent.SurplusProportion).ThenBy(ent => ent.SurplusLimit).Where(item => item.SurplusLimit >= goodsSumAmount))
            {
                _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, item.FilialeId, true);
                return;
            }
            //所有采购公司不满足当前采购单
            if (filialeIdGuids.Count == ptlList.Count)
            {
                //如果采购单商品只有1个
                if (goodsAmountList.Count == 1)
                {
                    var item = ptlList.OrderByDescending(ent => ent.SurplusProportion).First();
                    _purchasing.PurchasingUpdate(purchasingInfo.PurchasingID, item.FilialeId, cinfo.IsNeedInvoices);
                    return;
                }
                //循环采购商品从大到小
                foreach (var gInfo in goodsAmountList.OrderByDescending(ent => ent.AmountPrice))
                {
                    //获取满足当前商品的采购公司，并将此商品添加到此采购公司
                    var tempgInfo = gInfo;
                    var item = purchasingGoodsSplitList.Where(ent => ent.SurplusPurchasingLimit >= tempgInfo.AmountPrice).OrderBy(ent => ent.SurplusPurchasingLimit);
                    if (item.Any())
                    {
                        var temp = item.First();
                        var info = new PurchasingGoodsItemInfo
                        {
                            TotalGoodsAmount = gInfo.AmountPrice,
                            IsOut = true,
                            GoodsCode = gInfo.GoodsCode
                        };
                        temp.PurchasingGoodsItemList.Add(info);
                    }
                    else
                    {
                        //如果是必开票
                        if (cinfo.IsNeedInvoices)
                        {
                            var item1 = purchasingGoodsSplitList.OrderByDescending(ent => ent.TotalPurchasingLimit).First();
                            var info1 = new PurchasingGoodsItemInfo
                            {
                                TotalGoodsAmount = gInfo.AmountPrice,
                                IsOut = true,
                                GoodsCode = gInfo.GoodsCode
                            };
                            item1.PurchasingGoodsItemList.Add(info1);
                        }
                        else //非必开票
                        {
                            //var item1 = purchasingGoodsSplitList.FirstOrDefault(ent => ent.IsElseFilialeId);
                            //if (item1 != null)
                            //{
                            //    var info1 = new PurchasingGoodsItemInfo
                            //    {
                            //        TotalGoodsAmount = gInfo.AmountPrice,
                            //        IsOut = true, //cinfo.IsNeedInvoices
                            //        GoodsCode = gInfo.GoodsCode
                            //    };
                            //    item1.PurchasingGoodsItemList.Add(info1);
                            //}
                            //else
                            //{
                                
                            //}
                            //添加一条其他公司的记录
                            var info1 = new PurchasingGoodsItemInfo
                            {
                                TotalGoodsAmount = gInfo.AmountPrice,
                                IsOut = false,
                                GoodsCode = gInfo.GoodsCode
                            };
                            IList<PurchasingGoodsItemInfo> items = new List<PurchasingGoodsItemInfo>();
                            items.Add(info1);
                            //获取最大额度的采购公司
                            var maxItem = ptlList.OrderByDescending(ent => ent.TakerTicketLimit).First();
                            purchasingGoodsSplitList.Add(new PurchasingGoodsSplitInfo
                            {
                                FilialeId = maxItem.FilialeId,
                                TotalPurchasingLimit = maxItem.TakerTicketLimit,
                                PurchasingLimit = maxItem.SurplusLimit,
                                IsElseFilialeId = true,
                                PurchasingGoodsItemList = items
                            });
                        }
                    }
                }
                SplitPurchasing(purchasingId, purchasingGoodsSplitList);
            }
        }

        /// <summary>拆分采购单并插入新的采购单
        /// </summary>
        /// <param name="purchasingId">原始采购单Id</param>
        /// <param name="list">新采购单信息</param>
        private void SplitPurchasing(Guid purchasingId, IList<PurchasingGoodsSplitInfo> list)
        {
            var purchasingInfo = _purchasing.GetPurchasingById(purchasingId);
            IList<PurchasingDetailInfo> purchasingDetailInfoList = _purchasingDetail.Select(purchasingInfo.PurchasingID);
            var addedPurchasingList = new List<PurchasingInfo>();
            var addedPurchasingDetailList = new List<PurchasingDetailInfo>();
            var newList = list.Where(ent => ent.PurchasingGoodsItemList.Count > 0).ToList();
            for (var i = 0; i < newList.Count; i++)
            {
                var info = newList[i];
                var newPurchasingId = Guid.NewGuid();
                addedPurchasingList.Add(new PurchasingInfo
                {
                    PurchasingID = newPurchasingId,
                    ArrivalTime = purchasingInfo.ArrivalTime,
                    CompanyID = purchasingInfo.CompanyID,
                    CompanyName = purchasingInfo.CompanyName,
                    Description = purchasingInfo.Description,
                    Director = purchasingInfo.Director,
                    EndTime = DateTime.MaxValue,
                    FilialeID = purchasingInfo.FilialeID,
                    IsException = purchasingInfo.IsException,
                    NextPurchasingDate = purchasingInfo.NextPurchasingDate,
                    PersonResponsible = purchasingInfo.PersonResponsible,
                    PersonResponsibleName = purchasingInfo.PersonResponsibleName,
                    PmId = purchasingInfo.PmId,
                    PmName = purchasingInfo.PmName,
                    PurchasingFilialeId = info.FilialeId,
                    PurchasingNo = i > 0 ? purchasingInfo.PurchasingNo + "V" + i : purchasingInfo.PurchasingNo,
                    PurchasingState = (int)PurchasingState.Purchasing,
                    PurchasingToDate = purchasingInfo.PurchasingToDate,
                    PurchasingType = purchasingInfo.PurchasingType,
                    StartTime = purchasingInfo.StartTime,
                    WarehouseID = purchasingInfo.WarehouseID,
                    //IsOut = info.PurchasingGoodsItemList[0].IsOut
                });
                foreach (var item in info.PurchasingGoodsItemList)
                {
                    var item1 = item;
                    var pgInfos = purchasingDetailInfoList.Where(ent => ent.GoodsCode == item1.GoodsCode);
                    foreach (var pgInfo in pgInfos)
                    {
                        addedPurchasingDetailList.Add(new PurchasingDetailInfo
                        {
                            PurchasingID = newPurchasingId,
                            GoodsID = pgInfo.GoodsID,
                            GoodsName = pgInfo.GoodsName,
                            GoodsCode = pgInfo.GoodsCode,
                            Specification = pgInfo.Specification,
                            CompanyID = purchasingInfo.CompanyID,
                            Price = pgInfo.Price,
                            PlanQuantity = pgInfo.PlanQuantity,
                            RealityQuantity = 0,
                            State = 0,
                            Units = pgInfo.Units,
                            PurchasingGoodsID = pgInfo.PurchasingGoodsID,
                            SixtyDaySales = pgInfo.SixtyDaySales,
                            ThirtyDaySales = pgInfo.ThirtyDaySales,
                            ElevenDaySales = pgInfo.ElevenDaySales,
                            CPrice = pgInfo.Price,
                            PurchasingGoodsType = pgInfo.PurchasingGoodsType
                        });
                    }
                }
            }
            using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(2)))
            {
                try
                {
                    foreach (var item in addedPurchasingList)
                    {
                        _purchasing.PurchasingInsert(item);
                        _purchasing.PurchasingUpdate(item.PurchasingID, item.PurchasingFilialeId, item.IsOut);
                    }
                    foreach (var purchasingDetailInfo in addedPurchasingDetailList)
                    {
                        _purchasingDetail.Insert(purchasingDetailInfo);
                    }
                    var returnStr = Delete(purchasingInfo.PurchasingID);
                    if (string.IsNullOrWhiteSpace(returnStr))
                    {
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    SAL.LogCenter.LogService.LogError(string.Format("采购单拆单异常,PurchasingId={0}", purchasingId), "采购管理", ex);
                }
            }
        }

        #endregion
    }
}
