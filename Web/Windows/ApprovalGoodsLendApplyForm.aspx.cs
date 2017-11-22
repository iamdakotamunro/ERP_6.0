using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IStorage;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    ///  审批借出申请
    /// </summary>
    public partial class ApprovalGoodsLendApplyForm : System.Web.UI.Page
    {

        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly IBorrowLendDao _borrowLendDao = OrderInstance.GetBorrowLendDao(GlobalConfig.DB.FromType.Write);

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

        #region --> RG_GoodsBack

        protected void RgGoodsBack_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var warehouseIdSelected = new Guid(HF_Warehouse.Value);
            var storageType = byte.Parse(HF_StorageType.Value);
            List<Guid> goodsIdOrRealGoodsIdList = OutDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            var goodsStockQuantityList = new Dictionary<Guid, int>();
            //出库仓储
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                .FirstOrDefault(p => p.WarehouseId == warehouseIdSelected);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageType);
                if (slist != null)
                {
                    Guid? hostingFilialeId = null;
                    if (slist.IsReal) hostingFilialeId = new Guid(HF_Filiale.Value);
                    //根据储位判断是否根据物流配送公司获取库存
                    goodsStockQuantityList = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, hostingFilialeId);
                }
            }

            foreach (var info in OutDetailList)
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
            RG_GoodsBack.DataSource = OutDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
        }

        protected void RgGoodsBack_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
                var lblSpecification = (Label)dataItem.FindControl("lbl_Specification");
                if (DicGoodsAndChilds.ContainsKey(goodsId))
                {
                    var goods = DicGoodsAndChilds[goodsId].FirstOrDefault(p => p.RealGoodsId == realGoodsId);
                    lblSpecification.Text = goods == null ? "" : goods.Specification;
                }
            }
        }

        #endregion

        /// <summary>切换借出单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void CbStockBack_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStockBack.Checked)
            {
                lbTitle.Text = "借出返还单";
                RG_Goods.Visible = false;
                RG_GoodsBack.Visible = true;
                var goodsIds = InDetailList.Select(w => w.GoodsId).Distinct().ToList();
                var childList =
                    _goodsCenterSao.GetRealGoodsListByGoodsId(goodsIds)
                        .Where(w => w.GoodsId != w.RealGoodsId)
                        .ToList();
                var dict = new Dictionary<Guid, List<ChildGoodsInfo>>();
                foreach (var id in goodsIds)
                {
                    var list = childList.Where(w => w.GoodsId == id).ToList();
                    if (list.Count > 0)
                        dict.Add(id, list);
                }
                DicGoodsAndChilds = dict;
                //判断主商品数量是否一致
                if (OutDetailList.Count == 0 ||
                    goodsIds.Any(act => InDetailList.Where(k => k.GoodsId == act).Sum(v => v.Quantity)
                                        != OutDetailList.Where(k => k.GoodsId == act).Sum(v => v.Quantity)))
                {
                    OutDetailList = InDetailList;
                }
                RG_GoodsBack.Rebind();

            }
            else
            {
                lbTitle.Text = "借出单";
                RG_Goods.Visible = true;
                RG_GoodsBack.Visible = false;
            }
        }

        protected void RgGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var warehouseIdSelected = new Guid(HF_Warehouse.Value);
            var storageType = byte.Parse(HF_StorageType.Value);
            var isShow = storageType == (Byte) KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.StorageType.S;
            RG_Goods.MasterTableView.Columns.FindByUniqueName("ShelfType").Display = isShow;
            List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            var stockQuantityDtos = new List<ERP.Model.WMS.UpDownGoodsQuantityLendDTO>();
            //出库仓储
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                .FirstOrDefault(p => p.WarehouseId == warehouseIdSelected);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageType);
                if (slist != null)
                {
                    stockQuantityDtos = WMSSao.GetGoodsStockByStorageTypeLend(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, new Guid(HF_Filiale.Value));
                }
            }

            foreach (var info in InDetailList)
            {
                if (string.IsNullOrWhiteSpace(info.Units))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                    info.Units = goodsInfo != null ? goodsInfo.Units : "";
                }
                info.NonceWarehouseGoodsStock = isShow ? stockQuantityDtos.Where(ent=>ent.RealGoodsId==info.RealGoodsId && ent.ShelfType==info.ShelfType).Sum(ent=>ent.Quantity):
                    stockQuantityDtos.Where(ent => ent.RealGoodsId == info.RealGoodsId).Sum(ent => ent.Quantity);
            }
            Lab_TotalNumber.Text = InDetailList.Sum(p => Math.Abs(p.Quantity)).ToString();
            decimal TotalAmount = InDetailList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
            RG_Goods.DataSource = InDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
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
            string description = string.Format("[入库审核(审核人:{0};审核备注:{1});{2}]", personnel.RealName, "核准", dateTime);
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
                    bool result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Approved, description);
                    if (result)
                    {
                        storageRecordinfo.Description = string.Format("{0}{1}", storageRecordinfo.Description, description);
                        //新增进货单据
                        string billNo, msg;
                        result = WMSSao.InsertOutGoodsBill(_storageManager.ConvertToWMSOutGoodsBill(storageRecordinfo, goodsStockList, lbl_CompanyId.Text, personnel.PersonnelId, personnel.RealName), out billNo, out msg);
                        if (!result)
                        {
                            RAM.ResponseScripts.Add("alert('核准失败！(" + msg + ")');");
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
            string description = string.Format("[入库审核(审核人:{0};审核备注:{1});{2}]", realName, "核退", dateTime);
            //修改单据状态
            var result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Refuse, description);
            RAM.ResponseScripts.Add(result ? "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");" : "alert('审批失败！')");
        }

        private void BindData(Guid stockId)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            lbl_TradeCode.Text = storageRecordinfo.TradeCode;
            lbl_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            lbl_Transactor.Text = storageRecordinfo.Transactor;
            //供应商
            var companyData = new List<CompanyCussentInfo>();
            var companyCussentList =
                _companyCussent.GetCompanyCussentList(
                    new[] { CompanyType.Suppliers, CompanyType.Other, CompanyType.MemberGeneralLedger }, State.Enable);
            companyData.AddRange(companyCussentList);
            var filialeList =
                CacheCollection.Filiale.GetList().Where(w => w.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            companyData.AddRange(
                filialeList.Select(
                    filialeInfo => new CompanyCussentInfo { CompanyId = filialeInfo.ID, CompanyName = filialeInfo.Name }));
            var company = companyData.FirstOrDefault(p => p.CompanyId == storageRecordinfo.ThirdCompanyID);
            lbl_CompanyId.Text = company == null ? "" : company.CompanyName;
            //入库仓储，物流配送公司
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                    .FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (wList != null)
            {
                lbl_Warehouse.Text = wList.WarehouseName;
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    lbl_Warehouse.Text += "-" + slist.StorageTypeName;
                    var hlist =
                        slist.Filiales.FirstOrDefault(p => p.HostingFilialeId == storageRecordinfo.FilialeId);
                    lbl_HostingFilialeAuth.Text = hlist == null ? "" : hlist.HostingFilialeName;
                }
            }

            HF_Warehouse.Value = storageRecordinfo.WarehouseId.ToString();
            HF_StorageType.Value = storageRecordinfo.StorageType.ToString();
            HF_Filiale.Value = storageRecordinfo.FilialeId.ToString();
            lbl_Description.Text = storageRecordinfo.Description;

            var list = _storageManager.GetStorageRecordDetailListByStockId(StockId);
            var units = _goodsCenterSao.GetGoodsListByGoodsIds(list.Select(ent => ent.GoodsId).Distinct().ToList());
            foreach (var item in list)
            {
                var unit = units.FirstOrDefault(ent => ent.GoodsId == item.GoodsId);
                item.Units = unit != null ? unit.Units : "";
            }
            InDetailList = list;



            var borrowLendInfo = _borrowLendDao.GetBorrowLendInfo(StockId);
            if (borrowLendInfo != null)
            {
                var details = _borrowLendDao.GetBorrowLendDetailList(borrowLendInfo.BorrowLendId);
                OutDetailList = details.Select(act => new StorageRecordDetailInfo
                {
                    Description = act.Description,
                    GoodsCode = act.GoodsCode,
                    GoodsId = act.GoodsId,
                    GoodsName = act.GoodsName,
                    Quantity = act.Quantity,
                    RealGoodsId = act.RealGoodsId,
                    Specification = act.Specification,
                    UnitPrice = act.UnitPrice
                }).ToList();
            }
        }

        #region 模型

        /// <summary>
        /// 
        /// </summary>
        private Guid StockId
        {
            get
            {
                if (Request.QueryString["StockId"] == null ||
                    string.IsNullOrEmpty(Request.QueryString["StockId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["StockId"].Trim());
            }
        }

        /// <summary>
        /// 借出单明细
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

        /// <summary>
        /// 借出返还单明细
        /// </summary>
        private IList<StorageRecordDetailInfo> OutDetailList
        {
            get
            {
                if (ViewState["OutDetailList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["OutDetailList"];
            }
            set { ViewState["OutDetailList"] = value; }
        }


        private Dictionary<Guid, List<ChildGoodsInfo>> DicGoodsAndChilds
        {
            get
            {
                if (ViewState["DicGoodsAndChilds"] == null)
                {
                    return new Dictionary<Guid, List<ChildGoodsInfo>>();
                }
                return (Dictionary<Guid, List<ChildGoodsInfo>>)ViewState["DicGoodsAndChilds"];
            }
            set { ViewState["DicGoodsAndChilds"] = value; }
        }

        #endregion

        public string ShowShelfType(object shelfType)
        {
            var type = Convert.ToByte(shelfType);
            if (type == 0) return "-";
            switch (type)
            {
                case (Byte)KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.ShelfType.Good:
                    return "良品";
                case (Byte)KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.ShelfType.Inferior:
                    return "次品";
                case (Byte)KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.ShelfType.Bad:
                    return "坏品";
                default:
                    return "";
            }
        }
    }
}