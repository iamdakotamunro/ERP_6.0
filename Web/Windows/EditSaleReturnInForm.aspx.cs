using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using NPOI.HSSF.Record.Formula.Functions;
using OperationLog.Core;
using Telerik.Web.UI;


namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 编辑/重送销售退货入库
    /// </summary>
    public partial class EditSaleReturnInForm : System.Web.UI.Page
    {

        static readonly CodeManager _code = new CodeManager();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定仓库
                BindWarehouse();
                BindCompany();
                if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
                {
                    btnSave.Text = "重送";
                }
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]));
                }
            }
        }



        #region 下拉框选择事件

        /// <summary>
        /// 入库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            RcbInStockChanged();
            GoodsStockList = new List<StorageRecordDetailInfo>();
            RGGoods.Rebind();
            Rcb_CompanyChanged();
        }

        private void RcbInStockChanged()
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var warehouse = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouse != null && warehouse.Storages != null)
            {
                list.AddRange(warehouse.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.S));
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
        }

        //物流配送公司
        protected void RCB_HostingFilialeAuth_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Rcb_CompanyChanged();
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
            var warehouse = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouse != null)
            {
                if (warehouse.Storages != null)
                {
                    var flist = warehouse.Storages.FirstOrDefault(p => p.StorageType == storageType);
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

        /// <summary>供应商Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rcb_CompanyOnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            GoodsStockList = new List<StorageRecordDetailInfo>();
            RGGoods.Rebind();
            Rcb_CompanyChanged();
        }

        private void Rcb_CompanyChanged()
        {
            Lab_TotalNumber.Text = "0";
            Lab_TotalAmount.Text = "0";
            RCB_CreateNo.Text = "";
            var companyId = RCB_CompanyId.SelectedValue;
            var warehouseId = RCB_Warehouse.SelectedValue;
            var hostingFilialeId = RCB_HostingFilialeAuth.SelectedValue;

            if (!string.IsNullOrEmpty(companyId) && 
                !string.IsNullOrEmpty(warehouseId) && warehouseId!=Guid.Empty.ToString() &&
                !string.IsNullOrEmpty(hostingFilialeId) && hostingFilialeId != Guid.Empty.ToString()
                )
            {
                var list = _storageRecordDao.GetDicStockIdAndTradeCode(new Guid(warehouseId),new Guid(companyId),new Guid(hostingFilialeId) , string.Empty);
                RCB_CreateNo.DataSource = list;
                RCB_CreateNo.DataBind();
                RCB_CreateNo.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
                RCB_CreateNo.SelectedValue = Guid.Empty.ToString();
            }
        }

        /// <summary>
        /// 销售出库单号Changed事件
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void Rcb_CreateNoOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            Guid stockId = string.IsNullOrEmpty(RCB_CreateNo.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_CreateNo.SelectedValue);
            var sum = 0;
            IList<StorageRecordDetailInfo> dataList = new List<StorageRecordDetailInfo>();
            if (stockId != Guid.Empty)
            {
                var stockInfo = _storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"]));
                var dics = _storageRecordDao.GetEffictiveSellReturn(RCB_CreateNo.SelectedItem.Text, (int)StorageRecordType.SellReturnIn, stockInfo.TradeCode);
                if (dics.Count > 0)
                {
                    IList<StorageRecordDetailInfo> list = _storageManager.GetStorageRecordDetailListByStockId(stockId).ToList();
                    foreach (var item in dics)
                    {
                        var info = list.FirstOrDefault(act => act.RealGoodsId == item.Key);
                        info.Quantity = Math.Abs(item.Value);
                        sum += info.Quantity;
                        info.NonceWarehouseGoodsStock = 0;
                        dataList.Add(info);
                    }
                }
            }
            
            Lab_TotalNumber.Text = string.Format("{0}", sum);
            decimal TotalAmount = dataList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
            GoodsStockList = dataList;
            RGGoods.Rebind();
        }

        #endregion

        /// <summary>搜索销售单
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void Rcb_CreateNoOnItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Trim().Length >= 2)
            {
                var companyId = RCB_CompanyId.SelectedValue;
                var warehouseId = RCB_Warehouse.SelectedValue;
                var hostingFilialeId = RCB_HostingFilialeAuth.SelectedValue;

                if (!string.IsNullOrEmpty(companyId) &&
                !string.IsNullOrEmpty(warehouseId) && warehouseId != Guid.Empty.ToString() &&
                !string.IsNullOrEmpty(hostingFilialeId) && hostingFilialeId != Guid.Empty.ToString()
                )
                {
                    var list = _storageRecordDao.GetDicStockIdAndTradeCode(new Guid(warehouseId), new Guid(companyId), new Guid(hostingFilialeId), e.Text.Trim());
                    RCB_CreateNo.DataSource = list;
                    RCB_CreateNo.DataBind();
                }
            }
        }

        /// <summary>删除商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgGoodsDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            try
            {
                if (editedItem != null)
                {
                    var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
                    var unitPrice = editedItem.GetDataKeyValue("UnitPrice").ToString();
                    IList<StorageRecordDetailInfo> storageRecordDetailList = GoodsStockList;
                    var goodsStockInfo =
                        storageRecordDetailList.FirstOrDefault(
                            w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice));
                    if (goodsStockInfo != null)
                    {
                        storageRecordDetailList.Remove(goodsStockInfo);
                        GoodsStockList = storageRecordDetailList;
                    }
                }
                RGGoods.Rebind();
            }
            catch
            {
                RAM.Alert("删除失败！");
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

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_InsterStock(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_CompanyId.SelectedValue))
            {
                RAM.Alert("请选择供应商！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || new Guid(RCB_Warehouse.SelectedValue) == Guid.Empty)
            {
                RAM.Alert("请选择入库仓储！");
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

            var oldstockInfo = _storageRecordDao.GetStorageRecord(stockId);
            if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
            {
                if (oldstockInfo.StockState != (int)StorageRecordState.Refuse)
                {
                    RAM.Alert("当前单据状态已改变，重送失败！");
                    return;
                }
            }

            //出入库详细
            var goodsStockList = GetRgGoodsData(stockId);

            //储位id
            int storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue)
                ? default(int)
                : int.Parse(RCB_StorageAuth.SelectedValue);
            //物流配送公司
            Guid hostingFilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_HostingFilialeAuth.SelectedValue);

            //var filialeId = new Guid(HF_FilialeId.Value);
            var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
            var companyId = string.IsNullOrEmpty(RCB_CompanyId.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_CompanyId.SelectedValue);
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

            //入库单备注
            string description = !string.IsNullOrWhiteSpace(txt_Description.Text)
                ? txt_Description.Text.Trim()
                : "无";
            var personnelInfo = CurrentSession.Personnel.Get();



            Guid sellId = string.IsNullOrEmpty(RCB_CreateNo.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_CreateNo.SelectedValue);
            var originalCode = RCB_CreateNo.Text;

            //出入库
            var stockInfo = new StorageRecordInfo
            {
                StockId = stockId,
                WarehouseId = warehouseId,
                ThirdCompanyID = companyId,
                AccountReceivable = accountReceivable,
                Description = description,
                LinkTradeCode =string.IsNullOrEmpty(originalCode)?oldstockInfo.LinkTradeCode: originalCode,
                LinkTradeID = sellId==Guid.Empty?oldstockInfo.LinkTradeID:sellId,
                StockState = (int)StorageRecordState.WaitAudit,
                SubtotalQuantity = (decimal)subtotalQuantity,
                TradeCode = oldstockInfo.TradeCode,
                //Transactor = transactor,
                StorageType = storageType,
                FilialeId = hostingFilialeId,
                StockType= oldstockInfo.StockType
            };


            //修改
            _storageManager.UpdateStorageRecordAndStorageRecordDetail(stockInfo, goodsStockList);

            //入库修改操作记录添加
            Common.WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, stockInfo.StockId, stockInfo.TradeCode,
                  OperationPoint.StorageInManager.Edit.GetBusinessInfo(), "销售退货入库");

            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>获得入库商品详细
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        private List<StorageRecordDetailInfo> GetRgGoodsData(Guid stockId)
        {
            var list = new List<StorageRecordDetailInfo>();
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var info = new StorageRecordDetailInfo();
                //入库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                //单价
                var unitPricelbl = RGGoods.Items[i]["UnitPrice"].FindControl("TB_UnitPrice") as TextBox;
                //批文号
                var approvalNOlbl = RGGoods.Items[i]["ApprovalNO"].FindControl("lbApprovalNO") as Label;
                //备注
                var descriptiontxt = RGGoods.Items[i]["Description"].FindControl("TB_Description") as TextBox;

                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var unitPrice = unitPricelbl == null ? 0 : decimal.Parse(unitPricelbl.Text);
                var approvalNo = approvalNOlbl == null ? "" : approvalNOlbl.Text;
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                var description = descriptiontxt == null ? "" : descriptiontxt.Text;

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
                info.ApprovalNO = approvalNo;
                list.Add(info);
            }
            return list;
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
        /// 绑定供应商
        /// </summary>
        private void BindCompany()
        {
            //供应商
            RCB_CompanyId.DataSource =
                _companyCussent.GetCompanyCussentList(new[] { CompanyType.Vendors }, State.Enable).ToList();
            RCB_CompanyId.DataBind();
            RCB_CompanyId.Items.Insert(0, new RadComboBoxItem(""));
        }

        #endregion

        private void BindData(Guid stockId)
        {
            GoodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;

            var isShop = FilialeManager.IsEntityShopFiliale(storageRecordinfo.ThirdCompanyID);
            if (isShop)
            {
                var shop = CacheCollection.Filiale.Get(storageRecordinfo.ThirdCompanyID);
                if (shop != null)
                {

                    RCB_CompanyId.Items.Add(new RadComboBoxItem(shop.Name, shop.ID.ToString()));
                    RCB_CompanyId.SelectedValue = shop.ID.ToString();
                    RCB_CompanyId.SelectedItem.Text = shop.Name;
                    RCB_CompanyId.SelectedItem.Selected = true;
                }
            }
            else
            {
                RCB_CompanyId.SelectedValue = storageRecordinfo.ThirdCompanyID.ToString();
            }
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RcbInStockChanged();
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged();
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
            Rcb_CompanyChanged();
            RCB_CreateNo.SelectedValue = storageRecordinfo.LinkTradeID.ToString();
            txt_Description.Text = storageRecordinfo.Description;
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