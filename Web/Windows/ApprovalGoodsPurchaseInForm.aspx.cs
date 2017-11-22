using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
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
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class ApprovalGoodsPurchaseInForm : System.Web.UI.Page
    {
        static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Read);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly IPersonnelSao _personnelManager = new PersonnelSao();
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]));
                }
            }
        }

        /// <summary>采购商品列表数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<StorageRecordDetailInfo> goodsMergeStockList = new List<StorageRecordDetailInfo>();
            if (GoodsStockList.Count > 0)
            {
                var realGoodsIds = GoodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
                //根据商品id获得商品信息
                var dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIds);
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    var goodsIds = GoodsStockList.Select(w => w.GoodsId).Distinct().ToList();
                    var dicGoodsInformationInfo = _goodsCenterSao.GetGoodsInformationList(goodsIds);
                    foreach (StorageRecordDetailInfo goodsStockInfo in GoodsStockList)
                    {
                        bool hasKey = dicGoods.ContainsKey(goodsStockInfo.RealGoodsId);
                        if (hasKey)
                        {
                            string str = string.Empty;
                            var goodsInfo = dicGoods.FirstOrDefault(w => w.Key == goodsStockInfo.RealGoodsId).Value;
                            var dicGoodsInformationInfoList = dicGoodsInformationInfo.FirstOrDefault(p => p.Key.Equals(goodsStockInfo.GoodsId)).Value;
                            if (dicGoodsInformationInfoList != null)
                            {
                                var goodsInformationInfoList = dicGoodsInformationInfoList.Where(p => p.QualificationType.Equals((int)GoodsQualificationType.MedicalDeviceRegistrationNumber));

                                foreach (var item in goodsInformationInfoList)
                                {
                                    str += "," + item.Number;
                                }
                                if (str.Length > 0)
                                {
                                    str = str.Substring(1);
                                }
                            }

                            goodsStockInfo.Units = goodsInfo.Units;
                            goodsStockInfo.ApprovalNO = str;
                        }
                        var isHas = false;
                        if (goodsMergeStockList.Count > 0)
                        {
                            foreach (StorageRecordDetailInfo t in goodsMergeStockList)
                            {
                                if (goodsStockInfo.RealGoodsId == t.RealGoodsId &&
                                    goodsStockInfo.UnitPrice == t.UnitPrice)
                                {
                                    t.Quantity += goodsStockInfo.Quantity;
                                    isHas = true;
                                    break;
                                }
                            }
                        }
                        if (isHas == false)
                        {
                            goodsMergeStockList.Add(goodsStockInfo);
                        }
                    }
                    GoodsStockList = goodsMergeStockList;
                }
            }

            var units = _goodsCenterSao.GetGoodsListByGoodsIds(GoodsStockList.Select(ent => ent.GoodsId).Distinct().ToList());
            foreach (var item in GoodsStockList)
            {
                var unit = units.FirstOrDefault(ent => ent.GoodsId == item.GoodsId);
                item.Units = unit != null ? unit.Units : "";
            }
            RGGoods.DataSource = GoodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        private void BindData(Guid stockId)
        {
            GoodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.TradeBothPartiesType == (int)TradeBothPartiesType.HostingToHosting)
            {
                btn_Return.Visible = false;
            }
            lbl_TradeCode.Text = storageRecordinfo.TradeCode;
            lbl_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            lbl_Transactor.Text = storageRecordinfo.Transactor;

            var personinfo = CurrentSession.Personnel.Get();

            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (wList != null)
            {
                lbl_Warehouse.Text = wList.WarehouseName;
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    lbl_Warehouse.Text += "-" + slist.StorageTypeName;
                    var hlist = slist.Filiales.FirstOrDefault(p => p.HostingFilialeId == storageRecordinfo.FilialeId);
                    lbl_HostingFilialeAuth.Text = hlist == null ? "" : hlist.HostingFilialeName;
                }
            }

            var company = _companyCussent.GetCompanyCussent(storageRecordinfo.ThirdCompanyID);
            if (company != null)
            {
                lbl_CompanyId.Text = company.CompanyName;
            }
            else
            {
                lbl_CompanyId.Text = CacheCollection.Filiale.GetName(storageRecordinfo.ThirdCompanyID);
            }

            lbl_Filiale.Text = CacheCollection.Filiale.GetName(storageRecordinfo.FilialeId);
            //根据id查询采购单
            PurchasingInfo purchasinginfo = _purchasing.GetPurchasingById(storageRecordinfo.LinkTradeID);
            if (purchasinginfo == null)
            {
                return;
            }
            //采购负责人
            var personResponsibleId = _purchasing.GetRealNameByPurchasingID(storageRecordinfo.LinkTradeID);
            if (personResponsibleId != default(Guid))
            {
                lbl_Personnel.Text = _personnelManager.GetName(personResponsibleId);
            }
            lbl_OriginalCode.Text = storageRecordinfo.LinkTradeCode;
            lbl_Description.Text = storageRecordinfo.Description;
            Lab_TotalNumber.Text = GoodsStockList.Sum(p => p.Quantity).ToString();
            lbl_LogisticsCode.Text = storageRecordinfo.LogisticsCode;
            decimal TotalAmount = GoodsStockList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
        }

        /// <summary>
        /// 存储商品列表
        /// </summary>
        private IList<StorageRecordDetailInfo> GoodsStockList
        {
            get
            {
                if (ViewState["GoodsStockList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["GoodsStockList"];
            }
            set { ViewState["GoodsStockList"] = value; }
        }

        /// <summary>核准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApproval_Click(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            var innerPurchase = _storageRecordDao.GetInnerPurchaseRelationInfoIn(stockId);
            if (innerPurchase.OutStockId != Guid.Empty)
            {
                //查询出库单
                var outStorageRecordinfo = _storageRecordDao.GetStorageRecord(innerPurchase.OutStockId);
                if (outStorageRecordinfo != null)
                {
                    if (outStorageRecordinfo.StockState != (int)StorageRecordState.Finished)
                    {
                        RAM.Alert("此单据为来源为内部采购，需出货完成才能进行入库审核！");
                        return;
                    }
                }
            }
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
            var list = new List<HostingFilialeAuth>();
            var listStorage = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    listStorage.AddRange(warehouseAuth.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L || act.StorageType == (Byte)StorageAuthType.S));

                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                    if (storageAuth != null && storageAuth.Filiales != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                    }
                }
            }
            StringBuilder errorMsg = new StringBuilder();
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    bool result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Approved, description);
                    if (result)
                    {
                        storageRecordinfo.Description = string.Format("{0}{1}", storageRecordinfo.Description, description);
                        //新增出货单据
                        string billNo;
                        var resultAdd = WMSSao.InsertInGoodsBill(_storageManager.ConvertToWMSInGoodsBill(storageRecordinfo, goodsStockList, string.Format("{0}", list.First(ent => ent.HostingFilialeId == storageRecordinfo.FilialeId).HostingFilialeName), personnel.PersonnelId, personnel.RealName), out billNo);
                        if (!resultAdd.IsSuccess)
                        {
                            errorMsg.Append(string.Format("核准失败！{0}", resultAdd.Msg));
                        }
                        if (string.IsNullOrEmpty(billNo))
                        {
                            errorMsg.Append("仓储反馈进货单号为空！");
                        }
                        result = _storageRecordDao.SetBillNo(stockId, billNo);
                        if (!result)
                        {
                            errorMsg.Append("入库单进货单号更新失败！");
                        }
                    }
                    else
                    {
                        errorMsg.Append("更新出入库单据失败！");
                    }

                    if (string.IsNullOrEmpty(errorMsg.ToString()))
                    {
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    errorMsg.Append(ex.Message);
                }
                finally
                {
                    ts.Dispose();
                }
            }
            if (string.IsNullOrEmpty(errorMsg.ToString()))
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
            string description = string.Format("[入库审核(审核人:{0};审核备注:{1});{2}]", realName, "核退", dateTime);
            //修改单据状态
            var result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Refuse, description);
            RAM.ResponseScripts.Add(result ? "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");" : "alert('审批失败！')");
        }
    }
}