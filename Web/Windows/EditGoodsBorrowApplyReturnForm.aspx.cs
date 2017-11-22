using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Goods;
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
    /// 重送借入返回
    /// </summary>
    public partial class EditGoodsBorrowApplyReturnForm : System.Web.UI.Page
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
                BindWarehouse();
                BindCompany();
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    var storageRecordInfo = _storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"]));
                    HF_Description.Value = storageRecordInfo.Description;
                    BindData(storageRecordInfo.LinkTradeID);

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
                }
            }
        }

        private void BindData(Guid stockId)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_CompanyId.SelectedValue = storageRecordinfo.ThirdCompanyID.ToString();
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RcbInStockChanged();
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged();
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
            var list = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            var units = _goodsCenterSao.GetGoodsListByGoodsIds(list.Select(ent => ent.GoodsId).Distinct().ToList());
            foreach (var item in list)
            {
                var unit = units.FirstOrDefault(ent => ent.GoodsId == item.GoodsId);
                item.Units = unit != null ? unit.Units : "";
            }
            InDetailList = list;

            Lab_TotalNumber.Text = InDetailList.Sum(p => Math.Abs(p.Quantity)).ToString();

            var borrowLendInfo = _borrowLendDao.GetBorrowLendInfo(stockId);
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

                var detailUnits = _goodsCenterSao.GetGoodsListByGoodsIds(OutDetailList.Select(ent => ent.GoodsId).Distinct().ToList());
                foreach (var detailunit in detailUnits)
                {
                    foreach (var info in OutDetailList.Where(p => p.GoodsId == detailunit.GoodsId))
                    {
                        info.Units = detailunit.Units;
                    }
                }
            }
        }

        #region 下拉框选择事件
        private void RcbInStockChanged()
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var slist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (slist != null)
            {
                list = slist.Storages;
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();

            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();
        }
       
        private void RcbStorageAuthChanged()
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue)
                ? default(byte)
                : byte.Parse(RCB_StorageAuth.SelectedValue);

            //绑定物流配送公司
            var list = new List<HostingFilialeAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var wlist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (wlist != null)
            {
                if (wlist.Storages != null)
                {
                    var flist = wlist.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (flist != null)
                    {
                        list.AddRange(flist.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_HostingFilialeAuth.DataSource = list;
            RCB_HostingFilialeAuth.DataBind();
        }
        #endregion

        #region 模型
       

        /// <summary>
        /// 借入单明细
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
        /// 商品选择属性 RGSelectGoods
        /// </summary>
        protected DataTable GoodsTable
        {
            get
            {
                if (Session["GoodsTable"] == null)
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("GoodsId", typeof(Guid));
                    dataTable.Columns.Add("IsRealGoods", typeof(int));
                    dataTable.Columns.Add("商品编号", typeof(string));
                    dataTable.Columns.Add("商品名称", typeof(string));
                    dataTable.PrimaryKey = new[] { dataTable.Columns[0] };
                    return dataTable;
                }
                return (DataTable)Session["GoodsTable"];
            }
            set { Session["GoodsTable"] = value; }
        }

        /// <summary>
        /// 借入返还单明细
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
      
        #region --> RG_GoodsBack

        protected void RgGoodsBack_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            foreach (var info in OutDetailList)
            {
                var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                info.Units = goodsInfo.Units;
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
                var rcbSpecification = (RadComboBox)dataItem.FindControl("rcbSpecification");
                if (DicGoodsAndChilds.ContainsKey(goodsId))
                {
                    rcbSpecification.Visible = true;
                    rcbSpecification.DataSource = DicGoodsAndChilds[goodsId].OrderBy(p => p.Specification);
                    rcbSpecification.DataTextField = "Specification";
                    rcbSpecification.DataValueField = "RealGoodsId";
                    rcbSpecification.DataBind();
                    if (realGoodsId == Guid.Empty)
                    {
                        rcbSpecification.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
                    }
                    rcbSpecification.SelectedValue = realGoodsId.ToString();
                }
                else
                {
                    var tbQuantity = (TextBox)dataItem.FindControl("TB_Quantity");
                    tbQuantity.ReadOnly = true;
                }
            }
        }

        protected void rgGoodsOrderDetail_OnItemCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var dataItem = item;
                if (e.CommandName == "QuantityOut")
                {
                    var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                    var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
                    var unitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString());
                    var rcbSpecification = (RadComboBox)dataItem.FindControl("rcbSpecification");
                    var tbQuantity = (TextBox)dataItem.FindControl("TB_Quantity");

                    if (string.IsNullOrEmpty(tbQuantity.Text.Trim()) || tbQuantity.ReadOnly)
                        return;
                    var strRealGoodsId = rcbSpecification.SelectedValue;
                    if (strRealGoodsId == Guid.Empty.ToString())
                    {
                        RAM.Alert("请先选择SKU！");
                    }
                    else
                    {
                        IList<StorageRecordDetailInfo> list = (List<StorageRecordDetailInfo>)OutDetailList.DeepCopy();
                        var info =
                            list.FirstOrDefault(
                                w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice);
                        if (info != null)
                        {
                            if (!string.IsNullOrEmpty(strRealGoodsId))
                            {
                                info.RealGoodsId = new Guid(strRealGoodsId);
                                info.Specification = rcbSpecification.Text;
                            }
                            int quantity = int.Parse(tbQuantity.Text.Trim());
                            if (quantity == 0)
                            {
                                tbQuantity.Text = string.Format("{0}", info.Quantity);
                                RAM.Alert(string.Format("商品：{0} {1}不能为0！", info.GoodsName, info.Specification));
                            }
                            else if (info.Quantity < quantity)
                            {
                                tbQuantity.Text = string.Format("{0}", info.Quantity);
                                RAM.Alert(string.Format("商品：{0} {1}不能大于{2}！", info.GoodsName, info.Specification,
                                    info.Quantity));
                            }
                            else
                            {
                                if (info.Quantity > quantity)
                                {
                                    var newInfo = (StorageRecordDetailInfo)info.DeepCopy();
                                    newInfo.RealGoodsId = Guid.Empty;
                                    newInfo.Quantity = info.Quantity - quantity;
                                    list.Add(newInfo);
                                }
                                info.Quantity = quantity;
                                OutDetailList = list;
                                RG_GoodsBack.Rebind();
                            }
                        }
                    }
                }
                RAM.ResponseScripts.Add("flag = '1'");
            }
        }

        protected void RcbSpecification_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var rcbSpecification = (RadComboBox)sender;
            var dataItem = (GridDataItem)rcbSpecification.Parent.Parent;
            var strRealGoodsId = rcbSpecification.SelectedValue;

            var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
            var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
            var unitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString());

            IList<StorageRecordDetailInfo> list = (List<StorageRecordDetailInfo>)OutDetailList.DeepCopy();
            var info =
                list.FirstOrDefault(
                    w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice);
            if (info != null)
            {
                var newInfo =
                    list.FirstOrDefault(
                        w =>
                            w.GoodsId == goodsId && w.RealGoodsId == new Guid(strRealGoodsId) &&
                            w.UnitPrice == unitPrice);
                if (newInfo == null)
                {
                    info.RealGoodsId = new Guid(strRealGoodsId);
                }
                else
                {
                    newInfo.Quantity += info.Quantity;
                    list.Remove(info);
                }
                OutDetailList = list;
                RG_GoodsBack.Rebind();
            }
        }

        #endregion

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            #region 验证验证数据
            var oldstockInfo = _storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"]));
            if (oldstockInfo == null) return;

            if (oldstockInfo.StockState != (int)StorageRecordState.Refuse)
            {
                RAM.Alert("当前单据状态已改变，重送失败！");
                return;
            }

            var strWarehouseId = RCB_Warehouse.SelectedValue;
            if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择入库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ||
             RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择入库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ||
                RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }
            if (RCB_CompanyId.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择供应商！");
                return;
            }
            if (RG_GoodsBack.Visible)
            {
                GetRgGoodsBackData();
                if (OutDetailList.Count(w => w.RealGoodsId == Guid.Empty) > 0)
                {
                    RAM.Alert("请选择商品SKU！");
                    return;
                }
            }
            else
            {
                GetRgGoodsBackData();
                if (OutDetailList.Count == 0)
                    OutDetailList = InDetailList;
            }
            if (InDetailList.Count == 0)
            {
                RAM.Alert("请添加商品！");
                return;
            }

            if (InDetailList.Count(w => w.Quantity == 0) > 0)
            {
                var strb = new StringBuilder();
                foreach (var info in InDetailList.Where(w => w.Quantity == 0))
                {
                    strb.Append(info.GoodsName);
                    if (!string.IsNullOrEmpty(info.Specification))
                        strb.Append("[").Append(info.Specification).Append("]");
                    strb.Append("\n");

                }
                RAM.Alert("借入单中商品列表中数量不能为零！\n" + strb);
                return;
            }
            #endregion


            var stockId = new Guid(Request.QueryString["StockId"]);
            var storageRecordInfo = _storageRecordDao.GetStorageRecord(stockId);


            //借入返还单
            var borrowLendInfo = new BorrowLendInfo
            {
                BorrowLendId = Guid.NewGuid(),
                StockId = storageRecordInfo.LinkTradeID,
                AccountReceivable = OutDetailList.Sum(w => w.Quantity * w.UnitPrice),
                SubtotalQuantity = OutDetailList.Sum(w => w.Quantity),
                DateCreated = DateTime.Now
            };
            //借入返还单明细
            List<BorrowLendDetailInfo> borrowLendDetailList =
                OutDetailList.Select(detailInfo => new BorrowLendDetailInfo
                {
                    BorrowLendId = borrowLendInfo.BorrowLendId,
                    GoodsId = detailInfo.GoodsId,
                    RealGoodsId = detailInfo.RealGoodsId,
                    GoodsName = detailInfo.GoodsName,
                    GoodsCode = detailInfo.GoodsCode,
                    Specification = detailInfo.Specification,
                    UnitPrice = detailInfo.UnitPrice,
                    Quantity = detailInfo.Quantity,
                    Description = detailInfo.Description
                }).ToList();

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                string errorMessage;
                bool isSuccess;
                if (stockId != Guid.Empty)
                {
                    #region --> 更新，删除老数据
                    var oldBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(storageRecordInfo.LinkTradeID);
                    if (oldBorrowLendInfo != null)
                    {
                        var result = _borrowLendDao.DeleteBorrowLendAndDetailList(oldBorrowLendInfo.BorrowLendId,
                            out errorMessage);
                        if (result <= 0)
                        {
                            RAM.Alert("录入借入返还单前异常！" + errorMessage);
                            return;
                        }
                    }
                    #endregion
                }
                isSuccess = _borrowLendDao.AddBorrowLendAndDetailList(borrowLendInfo, borrowLendDetailList,
                    out errorMessage);
                if (!isSuccess)
                {
                    RAM.Alert("录入借入返还单异常！" + errorMessage);
                    return;
                }


                //在出入库记录中添加借入返还单
                var personnelInfo = CurrentSession.Personnel.Get();
                string tbDes = !string.IsNullOrWhiteSpace(tbDescription.Text) ? tbDescription.Text.Trim() : "无";
                string description = string.Format("[重送人:{0};重送备注:{1};{2}]", personnelInfo.RealName, tbDes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var addStockInfo = _storageRecordDao.GetStorageRecord(storageRecordInfo.LinkTradeID);
                var addBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(storageRecordInfo.LinkTradeID);
                var addBorrowLendDetailList = _borrowLendDao.GetBorrowLendDetailList(addBorrowLendInfo.BorrowLendId);

                //出入库
                var addstorageRecordInfo = new StorageRecordInfo
                {
                    StockId = stockId,
                    WarehouseId = addStockInfo.WarehouseId,
                    ThirdCompanyID = addStockInfo.ThirdCompanyID,
                    AccountReceivable = addBorrowLendDetailList.Sum(w => w.Quantity * w.UnitPrice),
                    Description = HF_Description.Value + description,
                    LinkTradeCode = addStockInfo.TradeCode,
                    LinkTradeID = addStockInfo.StockId,
                    StockState = (int)StorageRecordState.WaitAudit,
                    SubtotalQuantity = addBorrowLendDetailList.Sum(w => w.Quantity),
                    StorageType = addStockInfo.StorageType,
                    FilialeId = addStockInfo.FilialeId,
                    StockType= (int)StorageRecordType.BorrowOut
                };
                IList<StorageRecordDetailInfo> storageRecordDetailList =
                    addBorrowLendDetailList.Select(borrowLendDetailInfo => new StorageRecordDetailInfo
                    {
                        StockId = addstorageRecordInfo.StockId,
                        GoodsId = borrowLendDetailInfo.GoodsId,
                        RealGoodsId = borrowLendDetailInfo.RealGoodsId,
                        GoodsName = borrowLendDetailInfo.GoodsName,
                        GoodsCode = borrowLendDetailInfo.GoodsCode,
                        Specification = borrowLendDetailInfo.Specification,
                        UnitPrice = borrowLendDetailInfo.UnitPrice,
                        Quantity = borrowLendDetailInfo.Quantity,
                        Description = borrowLendDetailInfo.Description
                    }).ToList();

                _storageManager.UpdateStorageRecordAndStorageRecordDetail(addstorageRecordInfo,
                 storageRecordDetailList);
                ts.Complete();
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }


        /// <summary>获取借入返还单数据源
        /// </summary>
        private void GetRgGoodsBackData()
        {
            IList<StorageRecordDetailInfo> goodsStockList = new List<StorageRecordDetailInfo>();
            foreach (GridDataItem dataItem in RG_GoodsBack.Items)
            {
                var tbQuantity = (TextBox)dataItem.FindControl("TB_Quantity");
                var rcbSpecification = (RadComboBox)dataItem.FindControl("rcbSpecification");
                var info = new StorageRecordDetailInfo
                {
                    GoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString()),
                    RealGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString()),
                    GoodsCode = dataItem["GoodsCode"].Text,
                    Specification =
                        string.IsNullOrEmpty(rcbSpecification.Text)
                            ? string.Empty
                            : rcbSpecification.Text.Replace("&nbsp;", ""),
                    Units = dataItem["Units"].Text,
                    Quantity = string.IsNullOrEmpty(tbQuantity.Text) ? 0 : int.Parse(tbQuantity.Text),
                    UnitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString())
                };

                if (rcbSpecification.Visible)
                {
                    info.RealGoodsId = new Guid(rcbSpecification.SelectedValue);
                }
                if (goodsStockList.Count(w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice) == 0)
                {
                    var inInfo = InDetailList.FirstOrDefault(w => w.GoodsId == info.GoodsId);
                    if (inInfo != null)
                    {
                        info.GoodsName = inInfo.GoodsName;
                        info.Description = inInfo.Description;
                    }
                    goodsStockList.Add(info);
                }
                else
                {
                    var goodsStockInfo = goodsStockList.FirstOrDefault(w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice);
                    if (goodsStockInfo != null)
                    {
                        goodsStockInfo.Quantity += info.Quantity;
                    }
                }
            }
            OutDetailList = goodsStockList;
        }

        #region 下拉框绑定

        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var wList = CurrentSession.Personnel.WarehouseList;
            RCB_Warehouse.DataSource = wList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 供应商
        /// </summary>
        private void BindCompany()
        {
            var companyData = new List<CompanyCussentInfo>
            {
                new CompanyCussentInfo {CompanyId = Guid.Empty, CompanyName = ""}
            };
            var companyCussentList =
                _companyCussent.GetCompanyCussentList(
                    new[] { CompanyType.Suppliers, CompanyType.Other, CompanyType.MemberGeneralLedger }, State.Enable);
            companyData.AddRange(companyCussentList);
            var filialeList =
                CacheCollection.Filiale.GetList().Where(w => w.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            companyData.AddRange(
                filialeList.Select(
                    filialeInfo => new CompanyCussentInfo { CompanyId = filialeInfo.ID, CompanyName = filialeInfo.Name }));
            RCB_CompanyId.DataSource = companyData;
            RCB_CompanyId.DataBind();
        }

        #endregion
    }
}