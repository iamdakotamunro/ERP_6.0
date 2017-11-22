using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IShop;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model.ShopFront;
using OperationLog.Core;
using Telerik.Web.UI;
using CodeType = ERP.Enum.CodeType;
using GoodsInfo = ERP.Model.Goods.GoodsInfo;
using StorageManager = ERP.BLL.Implement.Inventory.StorageManager;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class ShopFrontApplyStockOut : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IApplyStockDAL _applyStockDal = new ApplyStockDAL(GlobalConfig.DB.FromType.Write);
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private readonly RealTimeGrossSettlementManager _realTimeGrossSettlementManager = new RealTimeGrossSettlementManager(GlobalConfig.DB.FromType.Write);

        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                LB_SemiStockOutCode.Text = new CodeManager().GetCode(CodeType.SL);
            }
        }

        protected Guid ApplyId
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["applyid"]))
                {
                    return new Guid(Request.QueryString["applyid"]);
                }
                return Guid.Empty;
            }
        }

        protected ApplyStockInfo CurrentStockInfo
        {
            get
            {
                if (ViewState["ApplyStockInfo"] == null)
                {
                    ViewState["ApplyStockInfo"] = _applyStockDal.FindById(ApplyId);
                }
                return ViewState["ApplyStockInfo"] as ApplyStockInfo;
            }
        }

        protected IList<GoodsSemiStockStateInfo> GoodsSemiStockStateList
        {
            get
            {
                if (ViewState["GoodsSemiStockStateList"] == null)
                {
                    ViewState["GoodsSemiStockStateList"] = _storageRecordDao.GetSemiGoodsQuanityWithState(CurrentStockInfo.TradeCode, CurrentPageGoodsIdList);
                }
                return ViewState["GoodsSemiStockStateList"] as IList<GoodsSemiStockStateInfo>;
            }
        }

        protected IList<ApplyStockDetailInfo> ApplyStockDetailList
        {
            get
            {
                if (ViewState["ApplyStockDetailList"] == null)
                {
                    var applyStockBll = new ApplyStockBLL(_applyStockDal);
                    ViewState["ApplyStockDetailList"] = applyStockBll.FindDetailList(ApplyId);
                }
                return ViewState["ApplyStockDetailList"] as IList<ApplyStockDetailInfo>;
            }
        }

        protected IList<Guid> CurrentPageGoodsIdList
        {
            get
            {
                if (ViewState["CurrentPageGoodsIdList"] == null)
                {
                    ViewState["CurrentPageGoodsIdList"] = ApplyStockDetailList.Select(ent => ent.GoodsId).ToList();
                }
                return ViewState["CurrentPageGoodsIdList"] as IList<Guid>;
            }
        }

        protected void RG_ApplyStockDetail_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RG_ApplyStockDetail.DataSource = ApplyStockDetailList.ToList();
        }


        protected bool IsNeedSemiStockGoods(Guid goodsId)
        {
            var goodsInfo = ApplyStockDetailList.FirstOrDefault(ent => ent.GoodsId == goodsId);
            if (goodsInfo != null)
            {
                var sm = GoodsSemiStockStateList.Where(ent => ent.GoodsId == goodsId && (ent.State == (int)StorageRecordState.WaitAudit || ent.State == (int)StorageRecordState.Finished)).ToList();
                if (sm.Count > 0)
                {
                    var sum = sm.Sum(ent => Math.Abs(ent.Quantity));
                    if (sum < goodsInfo.Quantity)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return true;
        }

        //确认出库
        protected void AffirmSemiStockGoods(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                RAM.Alert("程序正在处理中，请稍候...");
                return;
            }
            if (CurrentStockInfo != null)
            {
                if (String.IsNullOrWhiteSpace(LB_SemiStockOutCode.Text))
                {
                    RAM.Alert("单据号为空！");
                    return;
                }
                if (_storageRecordDao.IsExistNormalStorageRecord(LB_SemiStockOutCode.Text.Trim()))
                {
                    RAM.Alert(string.Format("门店要货出库单{0}已生成", LB_SemiStockOutCode.Text));
                    return;
                }

                var goodsIdOrRealGoodsIdList = new List<Guid>();
                var infoMsg = new List<String>();
                foreach (GridDataItem item in RG_ApplyStockDetail.MasterTableView.Items)
                {
                    var goodsId = (Guid)item.GetDataKeyValue("GoodsId");
                    var goodsName = (String)item.GetDataKeyValue("GoodsName");
                    var specification = (String)item.GetDataKeyValue("Specification");
                    var goodsStock = (int)item.GetDataKeyValue("GoodsStock");
                    var quantitytxt = item["Quantity"].FindControl("TB_ApplyQuantity") as TextBox;
                    if (goodsIdOrRealGoodsIdList.Count(w => w == goodsId) == 0)
                        goodsIdOrRealGoodsIdList.Add(goodsId);
                    if (quantitytxt != null && int.Parse(quantitytxt.Text) > goodsStock)
                    {
                        infoMsg.Add(goodsName + " " + specification);
                    }
                }
                if (infoMsg.Count > 0)
                {
                    RAM.Alert(String.Join(",", infoMsg) + " 需调拨数大于可用库存数！");
                    return;
                }
                var stock = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(CurrentStockInfo.WarehouseId,
                    null, goodsIdOrRealGoodsIdList, CurrentStockInfo.FilialeId);
                Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
                if (dicGoods == null || dicGoods.Count != goodsIdOrRealGoodsIdList.Count)
                {
                    RAM.Alert("商品中心数据异常");
                    return;
                }
                var personnel = CurrentSession.Personnel.Get();
                var stockId = Guid.NewGuid();
                var hostingFilialeId =
                    WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(CurrentStockInfo.CompanyWarehouseId, CurrentStockInfo.CompanyId,
                        dicGoods.Values.Select(ent => ent.GoodsType).Distinct());
                if (hostingFilialeId == Guid.Empty)
                {
                    RAM.Alert("物流公司ID不能为空！");
                    return;
                }
                Guid thirdCompanyId;
                bool direct = hostingFilialeId == CurrentStockInfo.CompanyId;
                if (!direct)
                {
                    thirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(CurrentStockInfo.CompanyId);
                    if (thirdCompanyId == Guid.Empty)
                    {
                        RAM.Alert("销售公司未关联往来单位！");
                        return;
                    }
                }
                else //如果等于那么相当于销售公司直接出货给门店
                {
                    thirdCompanyId = CurrentStockInfo.FilialeId;
                }
                var semiInfo = new StorageRecordInfo
                {
                    StockId = stockId,
                    FilialeId = hostingFilialeId,
                    //出库
                    ThirdCompanyID = thirdCompanyId,
                    WarehouseId = CurrentStockInfo.CompanyWarehouseId,
                    //入库
                    RelevanceFilialeId = CurrentStockInfo.FilialeId,
                    RelevanceWarehouseId = CurrentStockInfo.WarehouseId,
                    DateCreated = DateTime.Now,
                    Description = RTB_Remark.Text.Trim(),
                    LinkTradeCode = CurrentStockInfo.TradeCode,
                    StockState = (int)StorageRecordState.WaitAudit,
                    StockType = (int)StorageRecordType.SellStockOut,
                    StockValidation = false,
                    TradeCode = LB_SemiStockOutCode.Text,
                    Transactor = personnel.RealName,
                    SubtotalQuantity = 0,
                    AccountReceivable = 0,
                    LinkTradeID = CurrentStockInfo.ApplyId,
                    LinkTradeType = direct ? (int)StorageRecordLinkTradeType.Allot : (int)StorageRecordLinkTradeType.GoodsOrder,
                    //IsOut = true,
                    StorageType = (Byte)StorageAuthType.L,
                    TradeBothPartiesType = direct ? (int)TradeBothPartiesType.Other : (int)TradeBothPartiesType.HostingToSale
                };
                IDictionary<Guid, decimal> dics = new Dictionary<Guid, decimal>();
                if (hostingFilialeId != CurrentStockInfo.CompanyId)
                {
                    dics = _realTimeGrossSettlementManager.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(hostingFilialeId, dicGoods.Keys, DateTime.Now);
                }
                IList<StorageRecordDetailInfo> stockGoodsList = new List<StorageRecordDetailInfo>();
                var msg = string.Empty;
                foreach (GridDataItem item in RG_ApplyStockDetail.MasterTableView.Items)
                {
                    var realGoodsId = (Guid)item.GetDataKeyValue("GoodsId");
                    var tbApplyQuantity = item.FindControl("TB_ApplyQuantity") as TextBox;
                    if (tbApplyQuantity != null && Convert.ToDouble(tbApplyQuantity.Text) > 0)
                    {
                        int quantity = Convert.ToInt32(tbApplyQuantity.Text);
                        var stockDetailInfo = new StorageRecordDetailInfo
                        {
                            StockId = stockId,
                            RealGoodsId = realGoodsId,
                            GoodsName = item["GoodsName"].Text,
                            GoodsCode = string.Empty,
                            Specification = item.GetDataKeyValue("Specification").ToString(),
                            UnitPrice = (decimal)item.GetDataKeyValue("Price"),
                            Description = CurrentStockInfo.WarehouseName,
                            Quantity = -quantity,
                            ShelfType = 1
                        };

                        if (dicGoods.Count > 0)
                        {
                            bool hasKey = dicGoods.ContainsKey(realGoodsId);
                            if (hasKey)
                            {
                                var goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == realGoodsId).Value;
                                if (goodsBaseInfo != null)
                                {
                                    stockDetailInfo.GoodsId = goodsBaseInfo.GoodsId;
                                    stockDetailInfo.GoodsCode = goodsBaseInfo.GoodsCode;
                                    stockDetailInfo.Units = goodsBaseInfo.Units;
                                    if (dics.ContainsKey(goodsBaseInfo.GoodsId))
                                        stockDetailInfo.UnitPrice = dics[goodsBaseInfo.GoodsId];
                                    else
                                    {
                                        if (goodsBaseInfo.ExpandInfo != null)
                                        {
                                            if (goodsBaseInfo.ExpandInfo.JoinPrice == 0)
                                            {
                                                if (!msg.Contains(goodsBaseInfo.GoodsCode))
                                                    msg += "[" + stockDetailInfo.GoodsCode + "]" + stockDetailInfo.GoodsName + "\n";
                                            }
                                            else
                                            {
                                                stockDetailInfo.UnitPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                                                stockDetailInfo.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                                            }
                                        }
                                        else
                                        {
                                            if (!msg.Contains(stockDetailInfo.GoodsCode))
                                                msg += "[" + stockDetailInfo.GoodsCode + "]" + stockDetailInfo.GoodsName + "\n";
                                        }
                                    }
                                }
                            }
                        }
                        var applyStockDetailInfo = ApplyStockDetailList.FirstOrDefault(ent => ent.GoodsId == stockDetailInfo.RealGoodsId);
                        if (applyStockDetailInfo != null)
                        {
                            var tip = string.IsNullOrEmpty(stockDetailInfo.Specification) ? stockDetailInfo.GoodsName : stockDetailInfo.Specification;
                            if (quantity < 0)
                            {
                                RAM.Alert("提交失败：" + tip + "\\n申请数量不能填写负数！");
                                return;
                            }
                            if (quantity > applyStockDetailInfo.Quantity)
                            {
                                RAM.Alert("提交失败：" + tip + "\\n申请数量超过需求数量！");
                                return;
                            }
                        }
                        stockGoodsList.Add(stockDetailInfo);
                    }
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    RAM.Alert("不允许出库！以下商品未设置加盟价：\n" + msg);
                    return;
                }

                semiInfo.SubtotalQuantity = stockGoodsList.Sum(ent => ent.Quantity);
                semiInfo.AccountReceivable = stockGoodsList.Sum(ent => ent.UnitPrice * Math.Abs(ent.Quantity));

                //验证
                if (semiInfo.SubtotalQuantity == 0)
                {
                    RAM.Alert("提交失败：调拨合计总数不能为0！");
                    return;
                }

                //插入调拨
                var error = new StringBuilder();
                try
                {
                    var storageManager = new StorageManager();

                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        storageManager.NewInsertStockAndGoods(semiInfo, stockGoodsList);
                        ts.Complete();
                    }

                    WebControl.AddOperationLog(personnel.PersonnelId, personnel.RealName, semiInfo.StockId, semiInfo.TradeCode,
                         OperationPoint.StoreApplyList.ApplyOut.GetBusinessInfo(), "门店申请单号：" + CurrentStockInfo.TradeCode);

                    RAM.Alert("申请调拨成功！");
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    if (error.Length > 0)
                    {
                        RAM.Alert(error + "，" + ex.Message);
                    }
                }
            }
            else
            {
                RAM.Alert("发货主仓库未找到！");
            }
            _submitController.Submit();
        }


        protected int GetWaitSemiStockQuantity(Guid goodsId)
        {
            var sm = GoodsSemiStockStateList.Where(ent => ent.GoodsId == goodsId && (ent.State == (Int32)StorageRecordState.WaitAudit || ent.State == (Int32)StorageRecordState.Approved) || ent.State == (Int32)StorageRecordState.Finished).ToList();
            if (sm.Count > 0)
            {
                return sm.Sum(ent => Math.Abs(ent.Quantity));
            }
            return 0;
        }

        protected int NeedSemiQuantity(int allQuantity, int waitSemiQuantity, int semiQuantity)
        {
            return allQuantity - waitSemiQuantity - semiQuantity;
        }

        protected void RG_ApplyStockList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (e.Item.RowIndex > 1)
                {
                    var applygoodsinfo = (ApplyStockDetailInfo)e.Item.DataItem;

                    if ((applygoodsinfo.GoodsStock - applygoodsinfo.Quantity) < 0)
                    {
                        e.Item.Style.Add("background-color", "#FF6666");
                    }
                }
            }
        }


    }
}
