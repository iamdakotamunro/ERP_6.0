using System;
using System.Collections.Generic;
using System.Linq;
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
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 审批借出返回
    /// </summary>
    public partial class ApprovalGoodsLendApplyReturnForm : System.Web.UI.Page
    {
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao =
            new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Read);
        static readonly IPersonnelSao _personnelManager = new PersonnelSao();

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

        /// <summary>商品列表数据源
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
            RGGoods.DataSource = GoodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
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
                    //修改单据状态
                    var result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Approved,
                        description);
                    if (result)
                    {
                        //新增进货单据
                        string billNo;
                        var wmsResult = WMSSao.InsertInGoodsBill(_storageManager.ConvertToWMSInGoodsBill(storageRecordinfo, goodsStockList, lbl_ThirdCompanyID.Text, personnel.PersonnelId, personnel.RealName),out billNo);
                        if (!wmsResult.IsSuccess)
                        {
                            RAM.ResponseScripts.Add("alert('核准失败！');");
                            return;
                        }
                        if (!string.IsNullOrEmpty(billNo))
                        {
                            if (_storageRecordDao.SetBillNo(stockId, billNo))
                                ts.Complete();
                            else
                            {
                                RAM.ResponseScripts.Add("alert('入库单进货单号更新失败！');");
                                return;
                            }
                        }
                        else
                        {
                            RAM.ResponseScripts.Add("alert('仓储反馈进货单号为空！');");
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
            GoodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            lbl_TradeCode.Text = storageRecordinfo.TradeCode;
            lbl_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            lbl_Transactor.Text = storageRecordinfo.Transactor;
            var company = _companyCussent.GetCompanyCussent(storageRecordinfo.ThirdCompanyID);
            if (company != null)
            {
                lbl_ThirdCompanyID.Text = company.CompanyName;
            }
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

            lbl_Description.Text = storageRecordinfo.Description;
            Lab_TotalNumber.Text = GoodsStockList.Sum(p => p.Quantity).ToString();
            decimal TotalAmount = GoodsStockList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
        }

        /// <summary>存储商品列表
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
    }
}