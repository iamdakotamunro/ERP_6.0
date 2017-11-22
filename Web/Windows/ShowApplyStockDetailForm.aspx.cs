using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IShop;
using ERP.BLL.Implement.Organization;
using ERP.SAL;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model.ShopFront;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.Enum;
using ERP.Model;

namespace ERP.UI.Web.Windows
{
    public partial class ShowApplyStockDetailForm : Page
    {
        private static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private static readonly IApplyStockDAL _applyStockDal = new ApplyStockDAL(GlobalConfig.DB.FromType.Write);
        private static readonly StorageManager _storageManager = new StorageManager();

        /// <summary>
        /// 当前采购申请
        /// </summary>
        private ApplyStockInfo ApplyStock
        {
            get
            {
                if (ViewState["ApplyStock"] == null)
                    return _applyStockDal.FindById(new Guid(Request.QueryString["ApplyId"]));
                return (ApplyStockInfo)ViewState["ApplyStock"];
            }
            set { ViewState["ApplyStock"] = value; }
        }

        protected Guid ApplyId
        {
            get
            {
                return string.IsNullOrEmpty(Request["ApplyId"])
                           ? Guid.Empty
                           : new Guid(Request["ApplyId"]);
            }
        }

        /// <summary>
        /// 判断是否为审核操作
        /// </summary>
        protected bool IsAudit
        {
            get
            {
                return !string.IsNullOrEmpty(Request["IsAudit"]) && Request.QueryString["IsAudit"] == "1";
            }
        }

        /// <summary>
        /// 采购明细
        /// </summary>
        protected IList<TempApplyStockDetail> StockDetailInfos
        {
            get
            {
                if (ViewState["StockDetailInfos"] == null)
                    return new List<TempApplyStockDetail>();
                return (List<TempApplyStockDetail>)ViewState["StockDetailInfos"];
            }
            set { ViewState["StockDetailInfos"] = value; }
        }

        /// <summary>
        /// 是否展示
        /// </summary>
        protected Dictionary<Guid, bool> DicExpanded
        {
            get
            {
                if (ViewState["DicExpanded"] == null)
                    return new Dictionary<Guid, bool>();
                return (Dictionary<Guid, bool>)ViewState["DicExpanded"];
            }
            set { ViewState["DicExpanded"] = value; }
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var applyId = Request.QueryString["ApplyId"];
                ShowAudit.Visible = IsAudit;
                if (!string.IsNullOrEmpty(applyId))
                {
                    var applyStockInfo = _applyStockDal.FindById(new Guid(applyId));
                    if (applyStockInfo != null)
                    {
                        ApplyStock = applyStockInfo;
                        LbApplyNo.Text = applyStockInfo.TradeCode;
                        LbCreateTime.Text = applyStockInfo.DateCreated.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
        }

        /// <summary>
        /// 加载数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgApplyGoodsListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            //实际收货数 出库数-异常入库数
            var states = new List<int> { (int)StorageRecordState.WaitAudit, (int)StorageRecordState.Approved, (int)StorageRecordState.Finished };
            var receiverDic = ApplyStock.StockState >= (int)ApplyStockState.Finishing
                ? _storageRecordDao.GetSendQuantityByLinkTradeCode(ApplyStock.TradeCode, ApplyStock.CompanyId, ApplyStock.FilialeId, - 1, states)
                : new Dictionary<Guid, int>();

            //实际发货数由ERP出库单提供
            var sendQuantityDic = new Dictionary<Guid, int>();
            if (ApplyStock.StockState >= (int)ApplyStockState.Delivering)
            {
                var outList = _storageRecordDao.GetStorageRecordByLinkTradeCode(ApplyStock.TradeCode).OrderBy(act => act.DateCreated).ToList();
                if (outList.Count != 0)
                {
                    var first = outList.FirstOrDefault(ent => ent.FilialeId == ApplyStock.CompanyId && ent.ThirdCompanyID == ApplyStock.FilialeId
                      && states.Contains(ent.StockState) && ent.StockType == (int)StorageRecordType.SellStockOut);
                    if(first!=null)
                        sendQuantityDic = _storageRecordDao.GetStorageRecordDetailListByStockId(first.StockId).ToDictionary(act => act.RealGoodsId, ac => Math.Abs(ac.Quantity));
                }
            }

            //获取采购申请明细
            var quantityDic = ApplyStockBLL.ReadInstance.FindDetailList(ApplyId);

            var dataList = new List<TempApplyStockDetail>(); //主商品显示数据集合
            var actuallydataList = new List<TempApplyStockDetail>(); //真实数据集合
            if (quantityDic.Count != 0)
            {
                foreach (var tempApplyStockDetail in quantityDic)
                {
                    var sendQuantity = sendQuantityDic.Any(act => act.Key == tempApplyStockDetail.GoodsId)
                                             ? sendQuantityDic[tempApplyStockDetail.GoodsId] : 0;
                    var receiveQuantity = receiverDic != null
                            && receiverDic.Any(act => act.Key == tempApplyStockDetail.GoodsId)
                                            ? receiverDic[tempApplyStockDetail.GoodsId] : 0;
                    if (IsAudit && (sendQuantity == 0 || sendQuantity == receiveQuantity)) continue;
                    var info = dataList.FirstOrDefault(act => act.CompGoodsID == tempApplyStockDetail.CompGoodsID);
                    var acuallyDataInfo = StockDetailInfos.FirstOrDefault(act => act.GoodsId == tempApplyStockDetail.GoodsId);
                    if (info != null)
                    {
                        info.Quantity += tempApplyStockDetail.Quantity;
                        info.SendQuantity += sendQuantity;
                        info.ReceiveQuantity += receiveQuantity;
                    }
                    else
                    {
                        var detailInfo = new TempApplyStockDetail
                        {
                            CompGoodsID = tempApplyStockDetail.CompGoodsID,
                            GoodsId = tempApplyStockDetail.CompGoodsID == tempApplyStockDetail.GoodsId ? tempApplyStockDetail.GoodsId : Guid.Empty,
                            GoodsName = tempApplyStockDetail.GoodsName,
                            Specification = string.Empty,
                            Price = tempApplyStockDetail.Price,
                            Quantity = tempApplyStockDetail.Quantity,
                            SendQuantity = sendQuantity,
                            ReceiveQuantity = receiveQuantity
                        };
                        dataList.Add(detailInfo);
                    }
                    if (acuallyDataInfo != null) continue;
                    actuallydataList.Add(new TempApplyStockDetail
                    {
                        CompGoodsID = tempApplyStockDetail.CompGoodsID,
                        GoodsId = tempApplyStockDetail.GoodsId,
                        GoodsName = tempApplyStockDetail.GoodsName,
                        Specification = tempApplyStockDetail.Specification,
                        Price = tempApplyStockDetail.Price,
                        Quantity = tempApplyStockDetail.Quantity,
                        SendQuantity = sendQuantity,
                        ReceiveQuantity = receiveQuantity
                    });
                }
            }
            if (actuallydataList.Count > 0)
            {
                StockDetailInfos = actuallydataList;
            }
            //获取商品中心数据
            RgApplyGoodsList.DataSource = dataList;
        }

        private int _reciveQuantity;  //实际收货总数
        private int _quantity;  //采购申请采购数
        private int _sendQuantity;  //已发货数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgApplyGoodsListItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = (GridDataItem)e.Item;
                //var goodsId = item.GetDataKeyValue("GoodsId");
                var compGoodsId = item.GetDataKeyValue("CompGoodsID");
                var mainGoodsId = compGoodsId == null
                                      ? Guid.Empty
                                      : string.IsNullOrEmpty(compGoodsId.ToString())
                                            ? Guid.Empty
                                            : new Guid(compGoodsId.ToString());
                item.Expanded = DicExpanded.ContainsKey(mainGoodsId) && DicExpanded[mainGoodsId];
                var txtbox = item["ReceiveQuantity"].FindControl("TbReceive") as TextBox;
                if (txtbox != null)
                    _reciveQuantity += Convert.ToInt32(txtbox.Text);
                _sendQuantity += Convert.ToInt32(item["SendQuantity"].Text);
                _quantity += Convert.ToInt32(item["Quantity"].Text);
            }
            else if (e.Item is GridFooterItem)
            {
                var item = (GridFooterItem)e.Item;
                var labal = item["ReceiveQuantity"].FindControl("LbReceive") as Label;
                if (labal != null)
                    labal.Text = string.Format("{0}", _reciveQuantity);
                item["Quantity"].Text = string.Format("{0}", _quantity);
                item["SendQuantity"].Text = string.Format("{0}", _sendQuantity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IbAffirmOnClick(object sender, ImageClickEventArgs e)
        {
            var applyId = Request.QueryString["ApplyId"];
            var applyStockInfo = _applyStockDal.FindById(new Guid(applyId));
            if (applyStockInfo.StockState != (int)ApplyStockState.CheckPending)
            {
                RAM.Alert("只有异常待审核的单据才能进行审核操作！");
                return;
            }
            var storageRecords = _storageRecordDao.GetStorageRecordByLinkTradeCode(applyStockInfo.TradeCode);
            if (storageRecords == null || storageRecords.Count == 0)
            {
                RAM.Alert("该采购申请对应的出库单据信息未找到！");
                return;
            }

            var storageInfo = storageRecords.FirstOrDefault(act =>
              (act.StockType == (Int32)StorageRecordType.SellStockOut ||
              //下面三个是兼容老数据 zal 2017-03-02
              act.StockType == (Int32)StorageRecordType.TransferStockOut) && act.ThirdCompanyID==act.RelevanceFilialeId);
            if (storageInfo == null)
            {
                RAM.Alert("该采购申请对应的出库单据信息未找到！");
                return;
            }
            decimal totalPrice = 0;

            var stockDetails = _storageRecordDao.GetStorageRecordDetailListByStockId(storageInfo.StockId);
            foreach (var tempApplyStockDetail in StockDetailInfos)
            {
                var info = stockDetails.FirstOrDefault(act => act.RealGoodsId == tempApplyStockDetail.GoodsId);
                totalPrice += (tempApplyStockDetail.SendQuantity - tempApplyStockDetail.ReceiveQuantity) * info.UnitPrice;
            }
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    var result = _applyStockDal.UpdateApplyStockState(ApplyId, (int)ApplyStockState.Finished);
                    if (!result)
                    {
                        RAM.Alert("要货申请状态更新失败！");
                        return;
                    }
                    string msg;
                    if (totalPrice != 0)
                    {
                        var parentId = FilialeManager.GetShopHeadFilialeId(storageInfo.ThirdCompanyID);
                        result = ShopSao.ReturnMoney(parentId, storageInfo.ThirdCompanyID, totalPrice, ApplyId, "[ERP异常入库审核,退回金额]", out msg);
                        if (!result)
                        {
                            RAM.Alert(msg);
                            return;
                        }
                    }
                    ts.Complete();
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert(ex.Message);
            }
        }

        /// <summary>
        /// 商品子商品列表绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgApplyGoodsListDetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            var dic = DicExpanded;
            if (dataItem != null)
            {
                var compGoodsId = new Guid(dataItem.GetDataKeyValue("CompGoodsID").ToString());
                if (dic.ContainsKey(compGoodsId))
                    dic[compGoodsId] = true;
                else
                {
                    dic.Add(compGoodsId, true);
                }
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var dataList = goodsId == Guid.Empty && compGoodsId != goodsId
                                   ? StockDetailInfos.Where(act => act.CompGoodsID == compGoodsId).ToList()
                                   : new List<TempApplyStockDetail>();
                e.DetailTableView.DataSource = dataList;
            }
            DicExpanded = dic;
        }

        /// <summary>
        /// 填写确认数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TbReceiveCountChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            var item = tb.BindingContainer as GridDataItem;

            int receiveCount = Convert.ToInt32(tb.Text);
            if (item != null)
            {
                //var compGoodsId = new Guid(item.GetDataKeyValue("CompGoodsID").ToString());
                var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                var quantity = Convert.ToInt32(item.GetDataKeyValue("SendQuantity"));
                if (receiveCount > quantity)
                {
                    tb.Text = string.Format("{0}", quantity);
                    RAM.Alert("收货数量大于发货数!");
                    return;
                }
                if (goodsId == Guid.Empty) return;
                //var flag = goodsId == realGoodsId || StockDetailInfos.Any(act => act.GoodsId == realGoodsId);
                var tempDetails = StockDetailInfos;
                var info = tempDetails.FirstOrDefault(act => act.GoodsId == goodsId);
                if (info != null)
                {
                    info.ReceiveQuantity = receiveCount;
                    StockDetailInfos = tempDetails;
                }
                RgApplyGoodsList.MasterTableView.DetailTables[0].Rebind();
                //RgApplyGoodsList.Rebind();
            }
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgApplyGoodsList, e);
        }
    }

    /// <summary>
    /// 采购详细展示模型
    /// </summary>
    [Serializable]
    public class TempApplyStockDetail : ApplyStockDetailInfo
    {
        /// <summary>
        /// 发货数
        /// </summary>
        public int SendQuantity { get; set; }

        /// <summary>
        /// 收货数
        /// </summary>
        public int ReceiveQuantity { get; set; }

        /// <summary>
        /// 待确认描述
        /// </summary>
        public string ConfirmRemark { get; set; }
    }
}