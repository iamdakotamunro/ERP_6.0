using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 审批采购退货出库
    /// </summary>
    public partial class ApprovalPurchaseReturnOutForm : System.Web.UI.Page
    {
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao =new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["IsSel"]))
                {
                    btn_Approval.Visible = false;
                    btn_Return.Visible = false;
                }
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]));
                }
            }
        }

        /// <summary>商品明细
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = InDetailList.Sum(p => Math.Abs(p.Quantity)).ToString();
            RGGoods.DataSource = InDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }


        /// <summary>核准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApproval_Click(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            var personnel = CurrentSession.Personnel.Get();
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string description = string.Format("[出库审核(审核人:{0};审核备注:{1});{2}]", personnel.RealName, "核准", dateTime);
            //获得出入库记录
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                RAM.Alert("当前单据状态已改变，审核失败！");
                return;
            }
            var goodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    bool result;
                    //修改单据状态
                    result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Approved, description);
                    if (result)
                    {
                        storageRecordinfo.Description = string.Format("{0}{1}", storageRecordinfo.Description, description);
                        string billNo, msg;
                        //新增进货单据
                        result = WMSSao.InsertOutGoodsBill(_storageManager.ConvertToWMSOutGoodsBill(storageRecordinfo, goodsStockList, lbl_CompanyId.Text, personnel.PersonnelId, personnel.RealName),out billNo,out msg);
                        if (!result)
                        {
                            RAM.ResponseScripts.Add("alert('核准失败！("+ msg + ")');");
                            return;
                        }
                        if (!string.IsNullOrEmpty(billNo))
                        {
                            if (_storageRecordDao.SetBillNo(stockId, billNo))
                                ts.Complete();
                            else
                            {
                                RAM.ResponseScripts.Add("alert('出库单出货单号更新失败！');");
                                return;
                            }
                        }
                        else
                        {
                            RAM.ResponseScripts.Add("alert('仓储反馈出货单号为空！');");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    RAM.ResponseScripts.Add(ex.Message);
                    return;
                }
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>核退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            //获得出入库记录
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                RAM.Alert("当前单据状态已改变，审核失败！");
                return;
            }
            var realName = CurrentSession.Personnel.Get().RealName;
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string description = string.Format("[出库审核(审核人:{0};审核备注:{1});{2}]", realName, "核退", dateTime);
            //修改单据状态
            var result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Refuse, description);
            RAM.ResponseScripts.Add(result ? "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");" : "alert('审批失败！')");
        }

        private void BindData(Guid stockId)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            InDetailList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            var dicRealGoodsIdAndStockQuantity = new Dictionary<Guid, int>();
            //出库仓储
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                    .FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    //根据储位判断是否根据物流配送公司获取库存
                    dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(storageRecordinfo.WarehouseId, new List<Byte> { (Byte)storageRecordinfo.StorageType }, goodsIdOrRealGoodsIdList,
                        slist.IsReal ? storageRecordinfo.FilialeId : Guid.Empty);
                }
            }

            foreach (var info in InDetailList)
            {
                if (string.IsNullOrWhiteSpace(info.Units))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                    info.Units = goodsInfo!=null?goodsInfo.Units:"";
                }
                if (dicRealGoodsIdAndStockQuantity != null)
                {
                    //可出库数
                    var goodsStockKeyValuePair = dicRealGoodsIdAndStockQuantity.FirstOrDefault(w => w.Key == info.RealGoodsId);
                    info.NonceWarehouseGoodsStock = goodsStockKeyValuePair.Value;
                }
            }

            lbl_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            lbl_Transactor.Text = storageRecordinfo.Transactor;
            lbl_TradeCode.Text = storageRecordinfo.TradeCode;
            //入库仓储，物流配送公司
            if (wList != null)
            {
                lbl_Warehouse.Text = wList.WarehouseName;
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    lbl_Warehouse.Text += "-" + slist.StorageTypeName;
                    lbl_HostingFilialeAuth.Text = FilialeManager.GetName(storageRecordinfo.FilialeId);
                }
            }
            lbl_Description.Text = storageRecordinfo.Description;

            var companyCussentInfo =_companyCussent.GetCompanyCussent(storageRecordinfo.ThirdCompanyID);
            lbl_CompanyId.Text = companyCussentInfo == null ? "" : companyCussentInfo.CompanyName;
        }


        /// <summary>商品清单数据源
        /// </summary>
        private IList<StorageRecordDetailInfo> InDetailList
        {
            get
            {
                if (ViewState["InDetailList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["InDetailList"];
            }
            set { ViewState["InDetailList"] = value; }
        }
    }
}