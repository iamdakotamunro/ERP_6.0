using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using AllianceShop.Contract.DataTransferObject;
using AllianceShop.Enum;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.DAL.Interface.IShop;
using ERP.Enum;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.Model;
using ERP.Model.ShopFront;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using CodeType = ERP.Enum.CodeType;
using StorageManager = ERP.BLL.Implement.Inventory.StorageManager;
using WMSEnum = KeedeGroup.WMS.Infrastructure.CrossCutting.Enum;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ShopReturnGoodsCheckForm : Page
    {
        private readonly CodeManager _codeBll = new CodeManager();
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private static readonly IShopExchangedApplyDetail _shopApplyDetail = new ShopExchangedApplyDetailDal(GlobalConfig.DB.FromType.Write);
        private static readonly IShopExchangedApply _shopExchangedApply = new ShopExchangedApplyDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICheckRefund _refundDal = new CheckRefund(GlobalConfig.DB.FromType.Write);
        readonly ShopExchangedApplyBll _applyBll = new ShopExchangedApplyBll(_shopExchangedApply, _shopApplyDetail, _refundDal);
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly StorageManager _storageManager = new StorageManager();
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        #region prorperty
        SubmitController _submitController;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        /// <summary>
        /// 退换货号
        /// </summary>
        protected Guid RefundId
        {
            get { return ViewState["RefundId"] == null ? Guid.Empty : new Guid(ViewState["RefundId"].ToString()); }
            set { ViewState["RefundId"] = value; }
        }

        /// <summary>
        /// 退换货检查记录
        /// </summary>
        protected CheckRefundInfo RefundInfo
        {
            get
            {
                return ViewState["RefundInfo"] == null ? new CheckRefundInfo()
                    : (CheckRefundInfo)ViewState["RefundInfo"];
            }
            set { ViewState["RefundInfo"] = value; }
        }

        /// <summary>
        /// 初始退货检查明细记录
        /// </summary>
        protected List<CheckRefundDetailInfo> DetailInfos
        {
            get
            {
                return ViewState["DetailInfos"] == null ? new List<CheckRefundDetailInfo>()
                    : (List<CheckRefundDetailInfo>)ViewState["DetailInfos"];
            }
            set { ViewState["DetailInfos"] = value; }
        }

        #endregion

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                RefundId = string.IsNullOrEmpty(Request.QueryString["RefundId"])
                    ? Guid.Empty : new Guid(Request.QueryString["RefundId"]);
                if (RefundId == Guid.Empty)
                {
                    var applyId = string.IsNullOrEmpty(Request.QueryString["ApplyId"])
                        ? Guid.Empty
                        : new Guid(Request.QueryString["ApplyId"]);
                    var shopId = string.IsNullOrEmpty(Request.QueryString["ShopID"])
                        ? Guid.Empty
                        : new Guid(Request.QueryString["ShopID"]);
                    RefundInfo = _refundDal.GetShopCheckRefundList(shopId, applyId, string.Empty).FirstOrDefault();
                    RefundId = RefundInfo != null ? RefundInfo.RefundId : Guid.Empty;
                }
                else
                {
                    RefundInfo = _refundDal.GetShopCheckRefundInfo(RefundId);
                    Dictionary<Guid,List<byte>> dics = new Dictionary<Guid, List<byte>>();
                    if (RefundInfo!=null && RefundInfo.CheckState==(int)CheckState.Pass)
                    {
                        dics = _storageRecordDao.GetStorageRecordDetailListByLinkTradeCode(RefundInfo.RefundNo).GroupBy(ent=>ent.RealGoodsId).ToDictionary(k=>k.Key,v=>v.Select(act=>act.ShelfType).ToList());
                    }
                    var data = _refundDal.GetCheckRefundDetails(RefundId);
                    var mergeList = new List<CheckRefundDetailInfo>();
                    foreach (var checkRefundDetailInfo in data)
                    {
                        var info = mergeList.FirstOrDefault(act => act.RealGoodsId == checkRefundDetailInfo.RealGoodsId);
                        if (info != null)
                        {
                            info.Quantity += checkRefundDetailInfo.Quantity;
                            info.ReturnCount += checkRefundDetailInfo.ReturnCount;
                            info.DamageCount += checkRefundDetailInfo.DamageCount;
                        }
                        else
                        {
                            checkRefundDetailInfo.ShelfType = dics.ContainsKey(checkRefundDetailInfo.RealGoodsId)
                                ? dics[checkRefundDetailInfo.RealGoodsId].First()
                                : byte.MinValue;
                            mergeList.Add(checkRefundDetailInfo);
                        }
                    }
                    DetailInfos = mergeList;
                }
                if (RefundInfo == null) return;
                BindWarehouse();
                if (RefundInfo.CheckState == (int)CheckState.Pass || RefundInfo.CheckState == (int)CheckState.Checking)
                {
                    RbPass.Enabled = RefundInfo.CheckState == (int)CheckState.Checking;
                    RbBack.Enabled = RefundInfo.CheckState == (int)CheckState.Checking;
                    RtxReturnReason.Enabled = false;
                    RtxReStart.Enabled = false;
                    BtnSubmit.Enabled = RefundInfo.CheckState == (int)CheckState.Checking;
                    DdlWarehouse.EnableTextSelection = false;
                    if (!string.IsNullOrEmpty(RefundInfo.Remark))
                    {
                        RtxReturnReason.Text = RefundInfo.Remark;
                    }
                    if (!string.IsNullOrEmpty(RefundInfo.ReStartReason))
                    {
                        RtxReStart.Text = RefundInfo.ReStartReason;
                        DivReStart.Visible = true;
                    }
                    else
                    {
                        DivReStart.Visible = false;
                    }
                }
                else
                {
                    if (RefundInfo.CheckState == (int)CheckState.Refuse) //不通过的退换货单号
                    {
                        RbPass.Checked = false;
                        RbBack.Checked = true;
                        RtxReturnReason.Text = RefundInfo.Remark;
                        RtxReturnReason.Enabled = false;
                        BtnSubmit.Enabled = false;
                    }
                    if (!string.IsNullOrEmpty(RefundInfo.ReStartReason))
                    {
                        RtxReStart.Text = RefundInfo.ReStartReason;
                        RtxReStart.Enabled = false;
                        DivReStart.Visible = true;
                    }
                    else
                    {
                        DivReStart.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// 仓库绑定
        /// </summary>
        protected void BindWarehouse()
        {
            var warehouseList = WarehouseManager.GetContainsCustomerServiceStorageWarehouseList();
            DdlWarehouse.Items.Add(new RadComboBoxItem("--请选择仓库--", Guid.Empty.ToString()));
            foreach (var t in warehouseList)
            {
                DdlWarehouse.Items.Add(new RadComboBoxItem(t.WarehouseName, t.WarehouseId.ToString()));
            }
            bool flag = false;
            Guid warehouseId = RefundInfo.WarehouseId;
            if (RefundInfo.CheckState == (int)CheckState.Pass)
            {

                var storageRecrodInfos = _storageRecordDao.GetStorageRecordByLinkTradeCode(RefundInfo.OrderNo);
                var storageRecrodInfo =
                    storageRecrodInfos.FirstOrDefault(
                        act =>
                            act.LinkTradeType == (Int32)StorageRecordLinkTradeType.Allot &&
                            act.StockType == (Int32)StorageRecordType.SellReturnIn);
                if (storageRecrodInfo != null)
                {
                    warehouseId = storageRecrodInfo.WarehouseId;
                }
                flag = true;
            }
            DdlWarehouse.SelectedValue = warehouseId.ToString();
            DdlWarehouse.Enabled = false;
            RtxReturnReason.Enabled = flag;
        }


        /// <summary>
        /// modify by liangcanren at 2015-03-12 17:24
        /// 绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgReturnGoodsListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {

            IList<CheckRefundDetailInfo> tempList = new List<CheckRefundDetailInfo>();
            foreach (var checkRefundDetailInfo in DetailInfos)
            {
                if (checkRefundDetailInfo.GoodsId == checkRefundDetailInfo.RealGoodsId)
                {
                    tempList.Add(checkRefundDetailInfo);
                    continue;
                }
                var info = tempList.FirstOrDefault(act => act.GoodsId == checkRefundDetailInfo.GoodsId);
                if (info != null)
                {
                    info.Quantity += checkRefundDetailInfo.Quantity;
                    info.ReturnCount += checkRefundDetailInfo.ReturnCount;
                }
                else
                {
                    var temp = checkRefundDetailInfo.DeepCopy() as CheckRefundDetailInfo;
                    if (temp != null)
                    {
                        temp.GoodsId = checkRefundDetailInfo.GoodsId;
                        temp.Specification = string.Empty;
                        tempList.Add(temp);
                    }
                }
            }
            RgReturnGoodsList.DataSource = tempList;
        }

        /// <summary>
        /// 获取售后状态
        /// </summary>
        /// <returns></returns>
        protected string GetState(string state)
        {
            switch (state)
            {
                case "0":
                    return "换货";
                case "1":
                    return "退货";
                default:
                    return "-";
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSubmitClick(object sender, EventArgs e)
        {
            #region 数据验证
            if (!_submitController.Enabled)
            {
                RAM.Alert("程序正在处理中，请稍候...");
                return;
            }
            if (RbPass.Checked && (DdlWarehouse.SelectedItem == null || DdlWarehouse.SelectedValue == Guid.Empty.ToString()))
            {
                RAM.Alert("请选择退货仓库！");
                return;
            }
            if (DetailInfos.Count == 0)
            {
                RAM.Alert("未找到退换货商品明细！");
                return;
            }
            var checkRefundInfo = _refundDal.GetShopCheckRefundInfo(RefundId);
            if (checkRefundInfo.CheckState == (int)CheckState.Pass)
            {
                RAM.Alert("退回商品检查审核已通过！");
                return;
            }
            var exchangedInfo = _shopExchangedApply.GetShopExchangedApplyInfo(RefundInfo.OrderId);
            if (exchangedInfo == null)
            {
                RAM.Alert("未找到与该检查相对应的退换货申请！");
                return;
            }
            #endregion

            StorageRecordInfo semiStock = null;//退换货商品入库
            var semiStockDetails = new List<StorageRecordDetailInfo>();
            StockDTO allianceInStorage = null;//联盟店生成调拨入库
            var allianceInStorageDetails = new List<StockDetailDTO>();
            var dics = new Dictionary<Guid, int>();
            Guid parentId = FilialeManager.GetShopHeadFilialeId(exchangedInfo.ShopID);
            Guid warehouseId = new Guid(DdlWarehouse.SelectedValue);
            var exchangedDetails = _shopApplyDetail.GetShopExchangedApplyDetailList(exchangedInfo.ApplyID).ToList();
            List<Guid> goodsIds = new List<Guid>();
            goodsIds.AddRange(exchangedDetails.Select(ent => ent.GoodsID).Distinct());
            var goodsInfos = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
            if (goodsInfos == null || goodsInfos.Count != goodsIds.Count)
            {
                RAM.Alert("获取GMS商品信息失败！");
                return;
            }
            var dicGoods = goodsInfos.ToDictionary(k => k.GoodsId, v => v);
            var hostingFilialeId = Guid.Empty;
            if (RbPass.Checked)
            {
                hostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(warehouseId, parentId, goodsInfos.Select(ent => ent.GoodsType).Distinct());
                if (hostingFilialeId == Guid.Empty)
                {
                    RAM.Alert("该仓库所有物流配送公司都不能同时支持退回商品类型的发货！");
                    return;
                }
            }
            var thirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(parentId);
            if (thirdCompanyId == Guid.Empty)
            {
                RAM.Alert("销售公司未关联往来单位！");
                return;
            }
            PersonnelInfo personnelInfo = CurrentSession.Personnel.Get();

            var isFlag = DetailInfos.Any(act => act.Quantity - act.ReturnCount > 0);//购买数量>退回数量

            if (RbBack.Checked || (RbPass.Checked && isFlag))
            {
                #region 退回或者购买数量>退回数量时，商品退回到联盟店
                //联盟店生成调拨入库 异常差额  已审核
                allianceInStorage = new StockDTO
                {
                    CompanyID = parentId,
                    DateCreated = DateTime.Now,
                    Description = "退回商品检查退回联盟店商品入库",
                    OriginalTradeCode = checkRefundInfo.OrderNo,
                    PurchaseID = Guid.Empty,
                    ShopID = exchangedInfo.ShopID,
                    StockID = Guid.NewGuid(),
                    StockState = (int)StockState.Finish,
                    StockType = (int)StockType.PurchaseStockIn,
                    SubtotalPrice = 0,
                    SubtotalQuantity = 0
                };
                #endregion
            }
            if (RbPass.Checked)
            {
                #region 检查通过,商品生成入库单据
                //生成入库   1、销售公司销售退货入库 物流公司不等于销售公司  2、物流公司销售退货入库   物流公司不等于销售公司
                semiStock = new StorageRecordInfo
                {
                    StockId = Guid.NewGuid(),
                    FilialeId = hostingFilialeId,
                    ThirdCompanyID = hostingFilialeId != parentId ? thirdCompanyId : exchangedInfo.ShopID,
                    RelevanceFilialeId = exchangedInfo.ShopID,
                    RelevanceWarehouseId = exchangedInfo.ShopID,
                    WarehouseId = new Guid(DdlWarehouse.SelectedValue),
                    TradeCode = _codeBll.GetCode(CodeType.SI),
                    LinkTradeCode = RefundInfo.OrderNo,
                    DateCreated = DateTime.Now,
                    Transactor = personnelInfo.RealName,
                    Description = string.Format("[联盟店采购{0}货][{1}][{0}货单号][{2}]", exchangedInfo.IsBarter ? "退" : "换", personnelInfo.RealName, RefundInfo.OrderNo),
                    StockType = (int)StorageRecordType.SellReturnIn,
                    StockState = (int)StorageRecordState.Approved,
                    StorageType = (int)StorageAuthType.S,
                    LinkTradeID = RefundId,
                    StockValidation = false,
                    LinkTradeType = (int)StorageRecordLinkTradeType.Allot
                };
                #endregion
            }
            foreach (var info in DetailInfos)
            {
                if (RbBack.Checked || (RbPass.Checked && isFlag))
                {
                    #region 退换货商品明细(退回或者购买数量>退回数量时商品，退回到联盟店)
                    if (allianceInStorage == null) continue;//联盟店生成调拨入库
                    var detailDto = new StockDetailDTO
                    {
                        StockID = allianceInStorage.StockID,
                        GoodsCode = info.GoodsCode,
                        GoodsID = info.GoodsId,
                        RealGoodsID = info.RealGoodsId,
                        GoodsName = info.GoodsName,
                        Price = info.SellPrice,
                        RealQuantity = RbBack.Checked ? info.Quantity : info.Quantity - info.ReturnCount,
                        Specification = info.Specification,
                        Quantity = RbBack.Checked ? info.Quantity : info.Quantity - info.ReturnCount,
                        NonceGoodsStock = 0
                    };
                    allianceInStorageDetails.Add(detailDto);
                    if (isFlag)
                    {
                        dics.Add(info.GoodsId, info.Quantity - info.ReturnCount);
                    }
                    #endregion
                }
                if (semiStock != null)
                {
                    var detail = exchangedDetails.FirstOrDefault(act => act.ID == info.Id);
                    #region 退换货商品明细(检查通过,商品生成入库单据)
                    var goodsStockInfo = new StorageRecordDetailInfo
                    {
                        StockId = semiStock.StockId,
                        GoodsName = info.GoodsName,
                        GoodsCode = info.GoodsCode,
                        RealGoodsId = info.RealGoodsId,
                        Quantity = info.ReturnCount,
                        UnitPrice = info.SellPrice,
                        Specification = info.Specification,
                        Description = string.Format("{0}", exchangedInfo.IsBarter ? "退货" : "换货"),
                        NonceWarehouseGoodsStock = 0,
                        GoodsId = info.GoodsId,
                        BatchNo = detail != null ? detail.BatchNo : "",
                        EffectiveDate = detail != null ? detail.EffectiveDate ?? DateTime.MinValue : DateTime.Now,
                        ShelfType = info.ShelfType
                    };
                    semiStockDetails.Add(goodsStockInfo);
                    #endregion
                }
            }

            StringBuilder errMsg = new StringBuilder();
            int state = RbPass.Checked ? (int)CheckState.Pass : (int)CheckState.Refuse;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var isSuccess = _refundDal.UpdateCheckRefund(RefundId, state, "", personnelInfo.FilialeId) > 0;  //修改检查状态
                    //同时修改退换货申请状态
                    if (isSuccess)
                    {
                        #region 检查通过
                        if (RbPass.Checked)//通过
                        {
                            if (exchangedInfo.IsBarter)//退货通过
                            {
                                #region 退货
                                var returnMoney = DetailInfos.Where(act => act.GoodsId != Guid.Empty).Sum(act => act.ReturnCount * act.SellPrice);
                                string errorMsg;
                                isSuccess = ShopSao.ReturnMoney(parentId, exchangedInfo.ShopID, returnMoney, Guid.Empty, string.Format("[退回检查退回金额][单据号:{0}]", exchangedInfo.ApplyNo), out errorMsg);
                                if (!isSuccess)
                                {
                                    errMsg.Append(errorMsg);
                                }
                                #endregion
                            }
                            else
                            {
                                #region 换货，生成出库单 
                                var detailList = _shopApplyDetail.GetShopExchangedApplyDetailList(RefundInfo.OrderId).ToList();
                                if (detailList.Count == 0)
                                {
                                    isSuccess = false;
                                    errMsg.Append("未找到对应的换货明细！");
                                }
                                else
                                {
                                    string errorMsg;
                                    //生成调拨出库
                                    var createResult = CreateApplyStockOut(exchangedInfo, detailList, hostingFilialeId, dics, personnelInfo.RealName, parentId, thirdCompanyId, out errorMsg);
                                    if (!createResult)
                                    {
                                        isSuccess = false;
                                        errMsg.Append(errorMsg);
                                    }
                                }
                                #endregion
                            }

                            if (isSuccess)
                            {
                                if (semiStock != null && semiStockDetails.Count != 0)
                                {
                                    #region 新增出入库记录
                                    semiStock.AccountReceivable = Convert.ToDecimal(semiStockDetails.Sum(d => d.Quantity * d.UnitPrice));
                                    semiStock.SubtotalQuantity = semiStockDetails.Sum(w => w.Quantity);

                                    foreach (var outOfStorageDetailInfo in semiStockDetails)
                                    {
                                        if (dicGoods.ContainsKey(outOfStorageDetailInfo.RealGoodsId))
                                        {
                                            outOfStorageDetailInfo.JoinPrice = dicGoods[outOfStorageDetailInfo.RealGoodsId].ExpandInfo.JoinPrice;
                                        }
                                    }


                                    //增加准库存，添加入库单
                                    isSuccess = _storageManager.NewInsertStockAndGoods(semiStock, semiStockDetails);
                                    if (!isSuccess)
                                    {
                                        RAM.Alert("新增入库失败");
                                        return;
                                    }

                                    //新增进货单据
                                    string billNo;
                                    var wmsResult = WMSSao.InsertInGoodsBill(_storageManager.ConvertToWMSInGoodsBill(semiStock, semiStockDetails,
                                        exchangedInfo.ShopName, personnelInfo.PersonnelId, personnelInfo.RealName, parentId), out billNo);
                                    if (!wmsResult.IsSuccess)
                                    {
                                        RAM.Alert(wmsResult.Msg);
                                        return;
                                    }

                                    isSuccess = _storageRecordDao.SetBillNo(semiStock.StockId, billNo);
                                    if (!isSuccess)
                                    {
                                        RAM.Alert("更新进货单号失败");
                                        return;
                                    }
                                    #endregion
                                }
                                string errorMsg;
                                isSuccess = _applyBll.SetShopExchangedState(RefundInfo.OrderId, exchangedInfo.IsBarter ? (int)ExchangedState.ReturnEnd : (int)ExchangedState.Checked, exchangedInfo.IsBarter ? "商品退货完成" : "商品换货检查通过", out errorMsg) > 0;
                            }
                        }
                        #endregion


                        if (isSuccess && allianceInStorage != null)
                        {
                            string errorMsg;
                            isSuccess = _applyBll.SetShopExchangedState(RefundInfo.OrderId, (int)ExchangedState.GoodsReturn, "商品退回", out errorMsg) > 0;
                            if (isSuccess)
                            {
                                allianceInStorage.SubtotalPrice = allianceInStorageDetails.Sum(act => act.Quantity * act.Price);
                                allianceInStorage.SubtotalQuantity = allianceInStorageDetails.Sum(act => act.Quantity);
                                string errorMessage;
                                ShopSao.InsertStock(parentId, allianceInStorage, allianceInStorageDetails, false, null, 0, out errorMessage);
                                if (!string.IsNullOrEmpty(errorMessage))
                                {
                                    isSuccess = false;
                                    errMsg.Append(errorMessage);
                                }
                            }
                            else
                            {
                                errMsg.Append(errorMsg);
                            }
                        }
                    }
                    if (isSuccess)
                    {
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    errMsg.Append(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(errMsg.ToString()))
            {
                RAM.Alert(errMsg.ToString());
            }
            else
            {
                _submitController.Submit();
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        /// <summary>
        /// 通过/退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RbtnCheckedChanged(object sender, EventArgs e)
        {
            DdlWarehouse.Enabled = RbPass.Checked;
            if (RbPass.Checked)
            {
                //通过
                if (DdlWarehouse.Items.Count > 1)
                {
                    //DdlWarehouse.Items[1].Selected = true;
                    DdlWarehouse.SelectedValue = string.Format("{0}", RefundInfo.WarehouseId);
                }
                RtxReturnReason.Text = "";
                RtxReturnReason.Enabled = false;
            }
            else
            {
                //退回
                DdlWarehouse.Items[0].Selected = true;
                RtxReturnReason.Enabled = true;
                RtxReturnReason.Text = "";
            }
        }

        /// <summary>
        /// 选择仓库
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void DdlWarehouseSelectedChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (DdlWarehouse.SelectedItem != null && DdlWarehouse.SelectedValue != Guid.Empty.ToString())
            {
                RtxReStart.Enabled = false;
                RtxReturnReason.Enabled = false;
            }
            else
            {
                RtxReturnReason.Enabled = true;
            }
        }

        /// <summary>
        /// 子商品列表绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgReturnGoodsListDetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            if (dataItem != null)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                //var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                e.DetailTableView.DataSource = DetailInfos.Where(act => act.GoodsId == goodsId
                        && act.GoodsId != Guid.Empty && act.GoodsId != act.RealGoodsId);
            }
        }

        /// <summary>
        /// 损坏数量修改  (去除)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TbDageCountTextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            var item = tb.BindingContainer as GridDataItem;
            int dageCount = Convert.ToInt32(tb.Text);
            if (item != null)
            {
                var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                //var compGoodsId = new Guid(item.GetDataKeyValue("CompGoodsID").ToString());
                var quantity = Convert.ToInt32(item.GetDataKeyValue("Quantity"));
                if (dageCount > quantity)
                {
                    tb.Text = "0";
                    RAM.Alert("损坏数量大于采购数!");
                    return;
                }
                if (goodsId == Guid.Empty) return;
                var tempDetails = DetailInfos;
                var info = tempDetails.FirstOrDefault(act => act.GoodsId == goodsId);
                if (info != null)
                {
                    info.DamageCount = dageCount;
                    DetailInfos = tempDetails;

                }
            }
        }

        /// <summary>
        /// 退货数量修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TbReturnCountTextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            var item = tb.BindingContainer as GridDataItem;
            int returnCount = Convert.ToInt32(tb.Text);
            if (item != null)
            {
                var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                //var compGoodsId = new Guid(item.GetDataKeyValue("CompGoodsID").ToString());
                var quantity = Convert.ToInt32(item.GetDataKeyValue("Quantity"));
                if (returnCount > quantity)
                {
                    tb.Text = string.Format("{0}", quantity);
                    RAM.Alert("退回数量大于商品检查数!");
                    return;
                }
                if (goodsId == Guid.Empty) return;
                var tempDetails = DetailInfos;
                var info = tempDetails.FirstOrDefault(act => act.GoodsId == goodsId);
                if (info != null)
                {
                    info.ReturnCount = returnCount;
                    DetailInfos = tempDetails;
                    RgReturnGoodsList.Rebind();
                }
            }
        }

        /// <summary>
        /// 根据换货申请，生成调拨出库单据
        /// </summary>
        protected bool CreateApplyStockOut(ShopExchangedApplyInfo applyInfo, IList<ShopExchangedApplyDetailInfo> applyDetailInfos, Guid hostingFilialeId, Dictionary<Guid, int> dics, string realName, Guid parentId, Guid thridCompanyId, out string msg)
        {
            msg = string.Empty;
            var stockId = Guid.NewGuid();
            var isSame = hostingFilialeId == parentId;
            var semiInfo = new StorageRecordInfo
            {
                StockId = stockId,
                FilialeId = hostingFilialeId,
                //出库
                ThirdCompanyID = isSame ? applyInfo.ShopID : thridCompanyId,
                WarehouseId = RefundInfo.WarehouseId,
                //入库  联盟店公司/仓库
                RelevanceFilialeId = applyInfo.ShopID,
                RelevanceWarehouseId = Guid.Empty,
                DateCreated = DateTime.Now,
                Description = string.Format("{0}货商品调拨出库", applyInfo.IsBarter ? "退" : "换"),
                LinkTradeCode = RefundInfo.OrderNo,  //换货申请单号
                StockState = (int)StorageRecordState.WaitAudit,
                StockType = (int)StorageRecordType.SellStockOut,
                StockValidation = false,
                TradeCode = _codeBll.GetCode(CodeType.TSO),
                Transactor = realName,
                SubtotalQuantity = 0,
                AccountReceivable = 0,
                LinkTradeID = RefundInfo.OrderId,
                LinkTradeType = isSame ? (int)StorageRecordLinkTradeType.Allot : (int)StorageRecordLinkTradeType.GoodsOrder,
                //IsOut = false,
                StorageType = (Byte)StorageAuthType.L,
                TradeBothPartiesType = isSame ? (int)TradeBothPartiesType.Other : (int)TradeBothPartiesType.HostingToSale
            };
            IList<StorageRecordDetailInfo> stockGoodsList = new List<StorageRecordDetailInfo>();
            foreach (var storageRecordDetailInfo in applyDetailInfos)
            {
                var tempInfo =
                    stockGoodsList.FirstOrDefault(act => act.RealGoodsId == storageRecordDetailInfo.BarterRealGoodsID);
                if (tempInfo != null)
                {
                    tempInfo.Quantity -= storageRecordDetailInfo.Quantity;
                }
                else
                {
                    stockGoodsList.Add(new StorageRecordDetailInfo
                    {
                        StockId = stockId,
                        RealGoodsId = storageRecordDetailInfo.BarterRealGoodsID,
                        GoodsName = storageRecordDetailInfo.BarterGoodsName,
                        GoodsCode = storageRecordDetailInfo.BarterGoodsCode,
                        Specification = storageRecordDetailInfo.BarterSpecification,
                        UnitPrice = storageRecordDetailInfo.Price,
                        Description = string.Format("联盟店{0}货出库", applyInfo.IsBarter ? "退" : "换"),
                        Quantity = dics.ContainsKey(storageRecordDetailInfo.RealGoodsID)
                                            ? -(storageRecordDetailInfo.Quantity - dics[storageRecordDetailInfo.RealGoodsID])
                                            : -storageRecordDetailInfo.Quantity,
                        GoodsId = storageRecordDetailInfo.BarterGoodsID,
                        Units = storageRecordDetailInfo.Units
                    });
                }
            }
            semiInfo.SubtotalQuantity = stockGoodsList.Sum(ent => ent.Quantity);
            semiInfo.AccountReceivable = stockGoodsList.Sum(ent => ent.UnitPrice * Math.Abs(ent.Quantity));
            //验证
            if (semiInfo.SubtotalQuantity == 0)
            {
                msg = "提交失败：调拨合计总数不能为0！";
                return false;
            }
            _storageManager.NewInsertStockAndGoodsNoTrans(semiInfo, stockGoodsList);
            return true;
        }

        protected void BtnCancelOnClick(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        protected void RgReturnGoodsListItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var ddlShelfType = (DropDownList)e.Item.FindControl("DdlShelfType");
                bool isLoad = true;
                if (ddlShelfType == null)
                {
                    ddlShelfType = (DropDownList)e.Item.FindControl("DdlcShelfType");
                }
                else
                {
                    var goodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("GoodsId").ToString());
                    var realGoodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("RealGoodsId").ToString());
                    isLoad = goodsId == realGoodsId;
                }

                ddlShelfType.Items.Clear();
                ddlShelfType.Text = "";
                ddlShelfType.DataValueField = "Value";
                ddlShelfType.DataTextField = "Key";

                ddlShelfType.Enabled = RefundInfo.CheckState != (int) CheckState.Pass;

                if (isLoad)
                {
                    var dics = WMSEnum.Attribute.DescriptionAttribute.GetDict<WMSEnum.ShelfType>();
                    var shelfDics = dics.Where(ent => ent.Key == (Byte)WMSEnum.ShelfType.Good || ent.Key == (Byte)WMSEnum.ShelfType.Inferior || ent.Key == (Byte)WMSEnum.ShelfType.Bad).ToDictionary(k => k.Key, v => v.Value);
                    ddlShelfType.Items.Add(new ListItem("--请选择--", "0"));
                    var shelfTypeValue = Convert.ToByte(((GridDataItem)e.Item).GetDataKeyValue("ShelfType"));
                    foreach (var shelfTypeItem in shelfDics)
                    {
                        ddlShelfType.Items.Add(new ListItem(shelfTypeItem.Value, string.Format("{0}", shelfTypeItem.Key)));
                    }
                    ddlShelfType.SelectedValue = string.Format("{0}", shelfTypeValue);
                }
                else
                {
                    ddlShelfType.Visible = false;
                }
            }
        }

        protected void DdlShelfTypeSelectedChanged(object sender, EventArgs e)
        {
            var ddlShelfType = (DropDownList)sender;
            var gridDataItem = (GridDataItem)ddlShelfType.Parent.Parent;
            var realGoodsId = new Guid(gridDataItem.GetDataKeyValue("RealGoodsId").ToString());
            var shelfType = Convert.ToByte(gridDataItem.GetDataKeyValue("ShelfType"));
            List<CheckRefundDetailInfo> details = new List<CheckRefundDetailInfo>();
            foreach (var detail in DetailInfos)
            {
                if (detail.RealGoodsId == realGoodsId && detail.ShelfType == shelfType)
                {
                    detail.ShelfType = Convert.ToByte(ddlShelfType.SelectedValue);
                }
                details.Add(detail);
            }
            DetailInfos = details;
        }

        protected void DdlcShelfTypeSelectedChanged(object sender, EventArgs e)
        {
            var ddlShelfType = (DropDownList)sender;
            var gridDataItem = (GridDataItem)ddlShelfType.Parent.Parent;
            var realGoodsId = new Guid(gridDataItem.GetDataKeyValue("RealGoodsId").ToString());
            var shelfType = Convert.ToByte(gridDataItem.GetDataKeyValue("ShelfType"));
            List<CheckRefundDetailInfo> details = new List<CheckRefundDetailInfo>();
            foreach (var detail in DetailInfos)
            {
                if (detail.RealGoodsId == realGoodsId && detail.ShelfType == shelfType)
                {
                    detail.ShelfType = Convert.ToByte(ddlShelfType.SelectedValue);
                }
                details.Add(detail);
            }
            DetailInfos = details;
        }
    }
}