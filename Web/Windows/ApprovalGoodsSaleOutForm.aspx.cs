using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using Framework.Common;
using Telerik.Web.UI;
using ERP.Enum.ShopFront;
using ERP.BLL.Implement.Shop;
using ERP.BLL.Implement.Organization;
using AllianceShop.Contract.DataTransferObject;
using AllianceShop.Enum;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model.ShopFront;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;


namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 审批销售出库
    /// </summary>
    public partial class ApprovalGoodsSaleOutForm : System.Web.UI.Page
    {
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();
        private readonly IApplyStockDAL _applyStockDal = new ApplyStockDAL(GlobalConfig.DB.FromType.Write);
        private readonly IShopExchangedApply _shopExchangedApply = new ShopExchangedApplyDal(GlobalConfig.DB.FromType.Read);

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
                else
                {
                    LbTitle.Text = "销 售 商：";
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
            //获得出入库记录
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                RAM.Alert("当前单据状态已改变，审核失败！");
                return;
            }

            string errorMsg = "";
            var isValid = _storageManager.ApproveSellStockOut(stockId, lbl_CompanyId.Text, personnel.PersonnelId, personnel.RealName, out errorMsg);
            if (isValid)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                RAM.ResponseScripts.Add("alert('" + errorMsg + "');");
            }
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
            var result = _storageManager.RejectStockOutFromWmsRejectOutGoods(storageRecordinfo.TradeCode, description);
            RAM.ResponseScripts.Add(result ? "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");" : "alert('审批失败！')");
        }

        #region 模型

        /// <summary>
        /// 商品清单数据源
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
        #endregion

        private void BindData(Guid stockId)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            InDetailList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            var goodsStockQuantityList = new Dictionary<Guid, int>();
            //出库仓储
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                    .FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    Guid? hostingFilialeId = null;
                    if (slist.IsReal) hostingFilialeId = storageRecordinfo.FilialeId;
                    //根据储位判断是否根据物流配送公司获取库存
                    goodsStockQuantityList = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, storageRecordinfo.WarehouseId, (Byte)storageRecordinfo.StorageType,
                        hostingFilialeId);
                }
            }


            foreach (var info in InDetailList)
            {
                if (string.IsNullOrWhiteSpace(info.Units))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                    info.Units = goodsInfo != null ? goodsInfo.Units : "";
                }
                if (goodsStockQuantityList != null)
                {
                    info.NonceWarehouseGoodsStock = goodsStockQuantityList.ContainsKey(info.RealGoodsId) ? goodsStockQuantityList[info.RealGoodsId] : 0;
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
            if (storageRecordinfo.ThirdCompanyID == Guid.Empty)
            {
                lbl_CompanyId.Text = "订单";
                LbTitle.Text = "单据来源：";
            }
            else
            {
                var filialeInfo=MISService.GetFiliaeInfo(storageRecordinfo.ThirdCompanyID);
                if (filialeInfo!=null)
                {
                    lbl_CompanyId.Text = filialeInfo.Name;
                    LbTitle.Text = "单据来源：";
                    lbl_CompanyId.Enabled = true;
                }
                else
                {
                    var company = _companyCussent.GetCompanyCussent(storageRecordinfo.ThirdCompanyID);
                    lbl_CompanyId.Text = company != null ? company.CompanyName : "";
                    LbTitle.Text = "销 售 商：";
                }
            }
        }
    }
}