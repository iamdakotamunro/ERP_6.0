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
using NPOI.HSSF.Record.Formula.Functions;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 重送采购进货入库
    /// </summary>
    public partial class EditGoodsPurchaseInForm : System.Web.UI.Page
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
                //绑定仓库
                BindWarehouse();
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]));
                }
            }
        }

        #region 下拉框选择事件

        private void RcbInStockChanged()
        {
            RGGoods.Rebind();
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定采购单
            RCB_Purchasing.DataSource = BindInPurchase(warehouseId);
            RCB_Purchasing.DataBind();
            RCB_Purchasing.Text = string.Empty;

            //绑定入库储
            var list = new List<StorageAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var slist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (slist != null)
            {
                list = slist.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L).ToList();
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();


            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();
        }

        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            RcbStorageAuthChanged();
            RCB_HostingFilialeAuth.SelectedValue = HF_FilialeId.Value;
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

        /// <summary>
        /// 根据仓库ID绑定采购单(状态为采购中和部分完成)
        /// </summary>
        /// <param name="wareHouseId"></param>
        /// <returns></returns>
        private IList<PurchasingInfo> BindInPurchase(Guid wareHouseId)
        {
            IList<PurchasingInfo> dList = _purchasing.GetPurchasingList(wareHouseId);
            IList<PurchasingInfo> pList = dList.Select(pinfo => new PurchasingInfo
            {
                PurchasingNo = pinfo.PurchasingNo + " " + pinfo.CompanyName,
                PurchasingID = pinfo.PurchasingID
            }).ToList();
            if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
            {
                PurchasingInfo dInfo = _purchasing.GetPurchasingById(_storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"])).LinkTradeID);
                if (null != dInfo)
                {
                    if (Guid.Empty != dInfo.PurchasingID && dInfo.WarehouseID == wareHouseId)
                    {
                        var newInfo = new PurchasingInfo
                        {
                            PurchasingID = dInfo.PurchasingID,
                            PurchasingNo = dInfo.PurchasingNo + " " + dInfo.CompanyName
                        };
                        pList.Insert(0, newInfo);
                    }
                }
            }
            var pInfo = new PurchasingInfo { PurchasingID = Guid.Empty, PurchasingNo = "", CompanyName = "" };
            pList.Insert(0, pInfo);
            return pList;
        }
        #endregion

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
            RGGoods.DataSource = GoodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        /// <summary>重送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_InsterStock(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || new Guid(RCB_Warehouse.SelectedValue) == Guid.Empty)
            {
                RAM.Alert("请选择入库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_Purchasing.SelectedValue))
            {
                RAM.Alert("请选择采购单！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) || RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择入库储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) || new Guid(RCB_HostingFilialeAuth.SelectedValue) == Guid.Empty)
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }
            var stockId = new Guid(Request.QueryString["StockId"]);
            //出入库详细
            var goodsStockList = GetRgGoodsData(stockId);

            //储位id
            int storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(int) : int.Parse(RCB_StorageAuth.SelectedValue);
            //物流配送公司
            Guid hostingFilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ? Guid.Empty : new Guid(RCB_HostingFilialeAuth.SelectedValue);

            //总金额
            decimal accountReceivable = 0;
            //总数量
            double subtotalQuantity = 0;
            if (goodsStockList != null)
            {
                foreach (StorageRecordDetailInfo goodsStockInfo in goodsStockList)
                {
                    accountReceivable += Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice;
                    subtotalQuantity += Convert.ToDouble(goodsStockInfo.Quantity);
                }
            }
            if ((decimal)subtotalQuantity == 0)
            {
                RAM.Alert("商品数量不能为0！");
                return;
            }

            //重送备注
            string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text) ? txt_Description.Text.Trim() : "无";
            var personnelInfo = CurrentSession.Personnel.Get();
            string description = string.Format("[重送人:{0};重送备注:{1};{2}]", personnelInfo.RealName, storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var oldstockInfo = _storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"]));

            if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
            {
                if (oldstockInfo.StockState != (int)StorageRecordState.Refuse)
                {
                    RAM.Alert("当前单据状态已改变，重送失败！");
                    return;
                }
            }

            //出入库
            var stockInfo = new StorageRecordInfo
            {
                StockId = stockId,
                FilialeId = hostingFilialeId,
                WarehouseId = oldstockInfo.WarehouseId,
                ThirdCompanyID = oldstockInfo.ThirdCompanyID,
                AccountReceivable = accountReceivable,
                Description = description,
                LinkTradeCode = oldstockInfo.LinkTradeCode,
                LinkTradeID = oldstockInfo.LinkTradeID,
                StockState = (int)StorageRecordState.WaitAudit,
                SubtotalQuantity = (decimal)subtotalQuantity,
                StorageType = storageType,
                TradeCode = oldstockInfo.TradeCode,
                Transactor = oldstockInfo.Transactor,
                StockType = oldstockInfo.StockType
            };

            var companyCussent = _companyCussent.GetCompanyCussent(stockInfo.ThirdCompanyID);
            if (companyCussent == null)
            {
                RAM.Alert("未找到往来单位信息！");
                return;
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    //修改
                    _storageManager.UpdateStorageRecordAndStorageRecordDetail(stockInfo, goodsStockList);
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    RAM.Alert(ex.Message);
                    return;
                }
            }

            //入库修改操作记录添加
            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, stockInfo.StockId, stockInfo.TradeCode,
                  OperationPoint.StorageInManager.Edit.GetBusinessInfo(), "采购进货入库");
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        protected void RgGoods_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var quantitytxt = dataItem["Quantity"].FindControl("TB_Quantity") as TextBox;
                //quantitytxt.Enabled = false;
            }
        }

        /// <summary>创建价格表时设定列宽 删除一条记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGGoods_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            try
            {
                if (editedItem != null)
                {
                    var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
                    var goodsStockInfo = GoodsStockList.FirstOrDefault(w => w.RealGoodsId == realGoodsId);
                    if (goodsStockInfo != null)
                        GoodsStockList.Remove(goodsStockInfo);
                }
                RGGoods.Rebind();
            }
            catch
            {
                RAM.Alert("商品信息删除失败！");
            }
        }

        /// <summary>获得入库商品详细
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        private List<StorageRecordDetailInfo> GetRgGoodsData(Guid stockId)
        {
            var innPurch = _storageRecordDao.GetInnerPurchaseRelationInfoIn(stockId);
            var flag = false || innPurch.InStorageType != 0;
            var list = new List<StorageRecordDetailInfo>();
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var info = new StorageRecordDetailInfo();
                //入库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                if (flag)
                {
                    quantitytxt.Enabled = false;
                }
                //单价
                var unitPricelbl = RGGoods.Items[i]["UnitPrice"].FindControl("lbUnitPrice") as Label;
                //批文号
                var approvalNOlbl = RGGoods.Items[i]["ApprovalNO"].FindControl("lbApprovalNO") as Label;
                //备注
                var descriptiontxt = RGGoods.Items[i]["Description"].FindControl("TB_Description") as TextBox;



                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var units = RGGoods.Items[i]["Units"].Text;
                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var unitPrice = unitPricelbl == null ? 0 : decimal.Parse(unitPricelbl.Text);
                var approvalNo = approvalNOlbl == null ? "" : approvalNOlbl.Text;
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                var description = descriptiontxt == null ? "" : descriptiontxt.Text;

                //如果数量等于0并且单价不等于0则默认删除此商品入库
                if (quantity <= 0)
                {
                    continue;
                }

                info.StockId = stockId;
                info.GoodsId = new Guid(goodsId);
                info.Specification = specification;
                info.Quantity = quantity;
                info.UnitPrice = unitPrice;
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode;
                info.RealGoodsId = realGoodsId;
                info.Description = description;
                info.NonceWarehouseGoodsStock = 0;
                info.Units = units;
                info.ApprovalNO = approvalNo;
                list.Add(info);
            }
            return list;
        }

        private void BindData(Guid stockId)
        {
            GoodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RcbInStockChanged();
            GetRgGoodsData(stockId);
            RCB_Purchasing.SelectedValue = storageRecordinfo.LinkTradeID.ToString();
            var company = _companyCussent.GetCompanyCussentList(new[] { CompanyType.Suppliers }, State.Enable).FirstOrDefault(p => p.CompanyId == storageRecordinfo.ThirdCompanyID);
            if (company != null)
            {
                txt_CompanyId.Text = company.CompanyName;
                HF_CompanyId.Value = storageRecordinfo.ThirdCompanyID.ToString();
            }
            else
            {
                HF_CompanyId.Value = storageRecordinfo.ThirdCompanyID.ToString();
                txt_CompanyId.Text = CacheCollection.Filiale.GetName(storageRecordinfo.ThirdCompanyID);
            }
            var filiale = CacheCollection.Filiale.GetHeadList().FirstOrDefault(p => p.ID == storageRecordinfo.FilialeId);
            if (filiale != null)
            {
                txt_Filiale.Text = filiale.Name;
                HF_FilialeId.Value = storageRecordinfo.FilialeId.ToString();
            }
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged();
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
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
                TB_Personnel.Text = _personnelManager.GetName(personResponsibleId);
            }
            txt_OriginalCode.Text = storageRecordinfo.LinkTradeCode;
            txt_Description.Text = storageRecordinfo.Description;
            Lab_TotalNumber.Text = GoodsStockList.Sum(p => p.Quantity).ToString();
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

        #endregion
    }
}