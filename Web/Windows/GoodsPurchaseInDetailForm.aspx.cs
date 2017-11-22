using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
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
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 查看采购入库详细
    /// </summary>
    public partial class GoodsPurchaseInDetailForm : System.Web.UI.Page
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
                    lbl_HostingFilialeAuth.Text = CacheCollection.Filiale.GetName(storageRecordinfo.FilialeId);
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
            var filiale = CacheCollection.Filiale.GetHeadList().FirstOrDefault(p => p.ID == storageRecordinfo.FilialeId);
            if (filiale != null)
            {
                lbl_Filiale.Text = filiale.Name;
            }
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

    
    }
}