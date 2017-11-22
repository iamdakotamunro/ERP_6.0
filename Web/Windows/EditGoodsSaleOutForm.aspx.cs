using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 编辑/重送销售出库
    /// </summary>
    public partial class EditGoodsSaleOutForm : Page
    {
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly RealTimeGrossSettlementManager _realTimeGrossSettlementManager = new RealTimeGrossSettlementManager();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定仓库
                BindWarehouse();
                var bindShop = !string.IsNullOrEmpty(Request.QueryString["TradeType"]) &&
                               Convert.ToInt32(Request.QueryString["TradeType"]) ==
                               (int)StorageRecordLinkTradeType.Allot;
                LbTitle.Text = bindShop ? "单据来源：" : "销 售 商：";
                if (!bindShop)
                    BindCompanyId();
                BindGoodsClass();
                if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
                {
                    btnSave.Text = "重送";
                }
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]), bindShop);
                }
            }
            else
            {
                GetRgGoods();
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
        }

        private void RcbInStockChanged()
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var slist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                .FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (slist != null)
            {
                list = slist.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L || act.StorageType == (Byte)StorageAuthType.S).ToList();
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();

            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();

            var details = GetRgGoodsData(new Guid(Request.QueryString["StockId"]));
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }

        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            RcbStorageAuthChanged(true);
        }

        private void RcbStorageAuthChanged(bool sel)
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
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (storageAuth != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_HostingFilialeAuth.DataSource = list;
            RCB_HostingFilialeAuth.DataBind();
            var details = GetRgGoodsData(new Guid(Request.QueryString["StockId"]));
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }

        /// <summary>
        /// 物流配送公司Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbHostingFilialeAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var details = GetRgGoodsData(new Guid(Request.QueryString["StockId"]));
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }

        //选择分类
        protected void GoodsClass_SelectedIndexChanged(object source, EventArgs e)
        {
            HFSonGoods.Value = "";
            Guid goodsClass = Guid.Empty;
            if (!string.IsNullOrEmpty(RCB_GoodsClass.SelectedValue))
            {
                goodsClass = new Guid(RCB_GoodsClass.SelectedValue);
            }

            //根据分类包含产品生成表行
            IList<GoodsInfo> goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(goodsClass, string.Empty);
            var tempList = GoodsList;
            foreach (GoodsInfo goodsInfo in goodsInfoList)
            {
                var fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsInfo.GoodsId).ToList();
                var atigmiaDic = new List<FieldInfo>();
                var axialsDic = new List<FieldInfo>();
                var luminosityDic = new List<FieldInfo>();

                foreach (FieldInfo fieldInfo in fieldList)
                {
                    var childFieldList = new List<FieldInfo> { new FieldInfo { FieldId = fieldInfo.FieldId, FieldValue = "全部" } };
                    childFieldList.AddRange(fieldInfo.ChildFields.OrderBy(w => w.FieldValue).ToList());
                    switch (fieldInfo.FieldName)
                    {
                        case "光度":
                            luminosityDic = childFieldList;
                            break;
                        case "散光":
                            atigmiaDic = childFieldList;
                            break;
                        case "轴位":
                            axialsDic = childFieldList;
                            break;
                    }
                }
                if (tempList.All(act => act.GoodsId != goodsInfo.GoodsId))
                {
                    tempList.Add(new SearchGoodsInfo
                    {
                        Astigmias = atigmiaDic,
                        Axialss = axialsDic,
                        GoodsId = goodsInfo.GoodsId,
                        GoodsCode = goodsInfo.GoodsCode,
                        GoodsName = goodsInfo.GoodsName,
                        Luminositys = luminosityDic
                    });
                }
            }
            GoodsList = tempList;
            RGSelectGoods.Rebind();
        }

        //下拉选择商品
        protected void Goods_SelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RCB_Goods.SelectedValue))
            {
                HFSonGoods.Value = "";
                var goodsId = new Guid(RCB_Goods.SelectedValue);
                GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                if (goodsBaseInfo != null && !string.IsNullOrEmpty(goodsBaseInfo.GoodsName))
                {
                    //获取产品属性，根据分类属性生成表列
                    IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();
                    var tempList = GoodsList;
                    var selectGoods = tempList.FirstOrDefault(act => act.GoodsId == goodsId);
                    var atigmiaDic = new List<FieldInfo>();
                    var axialsDic = new List<FieldInfo>();
                    var luminosityDic = new List<FieldInfo>();
                    foreach (FieldInfo fieldInfo in fieldList)
                    {
                        var childFieldList = new List<FieldInfo> { new FieldInfo { FieldId = fieldInfo.FieldId, FieldValue = "全部" } };
                        childFieldList.AddRange(fieldInfo.ChildFields.OrderBy(act => act.OrderIndex).ToList());
                        switch (fieldInfo.FieldName)
                        {
                            case "光度":
                                luminosityDic = childFieldList;
                                break;
                            case "散光":
                                atigmiaDic = childFieldList;
                                break;
                            case "轴位":
                                axialsDic = childFieldList;
                                break;
                        }
                    }
                    if (selectGoods == null)
                    {
                        tempList.Add(new SearchGoodsInfo
                        {
                            Astigmias = atigmiaDic,
                            Axialss = axialsDic,
                            GoodsId = goodsBaseInfo.GoodsId,
                            GoodsCode = goodsBaseInfo.GoodsCode,
                            GoodsName = goodsBaseInfo.GoodsName,
                            Luminositys = luminosityDic
                        });
                        GoodsList = tempList;
                    }
                }
            }
            RGSelectGoods.Rebind();
        }

        #endregion

        /// <summary>搜索商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            //此处商品搜索有待更新，需要 过滤是否下架商品。
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key,
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        /// <summary>选择并添加商品 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ||
               RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择出库储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ||
                RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }

            if (RGSelectGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            //goodsStockList 选择后产品列表
            IList<StorageRecordDetailInfo> goodsStockList = GetRgGoodsData(new Guid(Request.QueryString["StockId"]));
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            var compGoodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (compGoodsIdList.Count(w => w == selectedGoodsId) == 0)
                    compGoodsIdList.Add(selectedGoodsId);
            }
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(compGoodsIdList);
            var goodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(RCB_Warehouse.SelectedValue), new Guid(RCB_HostingFilialeAuth.SelectedValue));
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsInfo.GoodsId);
                if (current == null) continue;
                if (!goodsTypes.Contains(goodsInfo.GoodsType))
                {
                    RAM.Alert(string.Format("{0}({1})不在仓库物流公司的采购范围内", goodsInfo.GoodsName, goodsInfo.GoodsCode));
                    return;
                }
                IList<ChildGoodsInfo> childGoodsList = new List<ChildGoodsInfo>();
                if (current.Luminositys.Count == 0 && current.Astigmias.Count == 0 && current.Axialss.Count == 0)
                {
                    var childGoodsInfo = new ChildGoodsInfo
                    {
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = goodsInfo.GoodsId,
                        Specification = string.Empty
                    };
                    childGoodsList.Add(childGoodsInfo);
                }
                else if (dataItem.Cells.Count >= 7)
                {
                    var dicSelectedField = new Dictionary<Guid, List<Guid>>();
                    for (int i = 5; i < dataItem.Cells.Count; i++)
                    {
                        var fieldInfoList = i == 5
                                ? current.Luminositys
                                : i == 6 ? current.Astigmias : current.Axialss;
                        if (i == 5 && current.Luminositys.Count == 0) break;
                        if (i == 6 && current.Astigmias.Count == 0) break;
                        if (i == 7 && current.Axialss.Count == 0) break;
                        var fieldId = fieldInfoList.First().FieldId;
                        string key = dataItemClientId + "_RCB_Field" + i;
                        foreach (var keyValuePair in dicFiled)
                        {
                            if (key == keyValuePair.Key)
                            {
                                var list = keyValuePair.Value.Select(str => new Guid(str)).ToList();
                                if (list.Count(w => w == fieldId) > 0)
                                {
                                    //该属性选择了全部
                                    FieldInfo fieldInfo = fieldInfoList.FirstOrDefault(w => w.FieldId == fieldId);
                                    if (fieldInfo != null)
                                        dicSelectedField.Add(fieldId, fieldInfoList.Where(act => act.FieldId != fieldId).Select(w => w.FieldId).ToList());
                                }
                                else
                                {
                                    dicSelectedField.Add(fieldId, list);
                                }
                                break;
                            }
                        }
                    }
                    if (dicSelectedField.Count > 0)
                    {
                        childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsIdAndFields(selectedGoodsId, dicSelectedField);
                        if (childGoodsList.Count == 0)
                        {
                            RAM.Alert(string.Format("商品{0}属性选择不完整！", goodsInfo.GoodsCode));
                            return;
                        }
                    }
                    else
                    {
                        RAM.Alert(string.Format("商品{0}请选择子商品！", goodsInfo.GoodsCode));
                        return;
                    }
                }

                foreach (var childGoodsInfo in childGoodsList)
                {
                    var goodsStockInfo = new StorageRecordDetailInfo
                    {
                        StockId = Guid.Empty,
                        ThirdCompanyID = Guid.Empty,
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = childGoodsInfo.RealGoodsId,
                        GoodsName = goodsInfo.GoodsName,
                        GoodsCode = goodsInfo.GoodsCode,
                        Specification = childGoodsInfo.Specification,
                        Units = goodsInfo.Units,
                        UnitPrice = 0,
                        Quantity = 0,
                        NonceWarehouseGoodsStock = 0,
                        Description = string.Empty
                    };
                    if (goodsStockList.Count(w => w.RealGoodsId == goodsStockInfo.RealGoodsId && w.UnitPrice == goodsStockInfo.UnitPrice) == 0)
                    {
                        goodsStockList.Add(goodsStockInfo);
                    }
                }
            }
            List<Guid> goodsIdList = goodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdList);
            foreach (var info in goodsStockList)
            {
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        info.UnitPrice =
                            _realTimeGrossSettlementManager.GetLatestUnitPrice(
                                new Guid(RCB_HostingFilialeAuth.SelectedItem.Value), info.GoodsId);//结算价
                    }
                }
            }
            InDetailList = goodsStockList;
            InDetailAllList = goodsStockList;
            RGGoods.Rebind();
        }

        /// <summary>价格表单元格绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgSelectGoodsItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsId);
                if (current != null)
                {
                    foreach (GridTableCell cell in e.Item.Cells)
                    {
                        int cellIndex = e.Item.Cells.GetCellIndex(cell);
                        if (cellIndex >= 5)
                        {
                            var fieldList = cellIndex == 5
                                ? current.Luminositys
                                : cellIndex == 6 ? current.Astigmias : current.Axialss;
                            if (fieldList.Count > 0)
                            {
                                var rads = new RadComboBox();
                                cell.Controls.Add(rads);
                                rads.ID = "RCB_Field" + cellIndex;
                                rads.Height = Unit.Pixel(300);
                                rads.Width = Unit.Pixel(100);
                                rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                rads.DataSource = fieldList;
                                rads.DataTextField = "FieldValue";
                                rads.DataValueField = "FieldId";
                                rads.DataBind();
                                rads.HighlightTemplatedItems = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>删除一条记录
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
                    var unitPrice = editedItem.GetDataKeyValue("UnitPrice").ToString();
                    var goodsStockInfo =
                        InDetailList.FirstOrDefault(
                            w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice));
                    if (goodsStockInfo != null)
                        InDetailList.Remove(goodsStockInfo);
                }
                RGGoods.Rebind();
            }
            catch
            {
                RAM.Alert("商品信息删除失败！");
            }
        }

        /// <summary>商品明细
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = string.Format("{0}", InDetailList.Sum(p => Math.Abs(p.Quantity)));
            if (InDetailList.Count > 0)
            {
                List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
                var warehouseIdSelected = new Guid(RCB_Warehouse.SelectedValue);
                var storageType = byte.Parse(RCB_StorageAuth.SelectedValue);
                var stockQuantitys = new Dictionary<Guid, int>();
                if (storageType > 0)
                {
                    var warehouse = CurrentSession.Personnel.WarehouseList.FirstOrDefault(act => act.WarehouseId == warehouseIdSelected);
                    if (warehouse != null)
                    {
                        var storage = warehouse.Storages.FirstOrDefault(p => p.StorageType == storageType);
                        if (storage != null)
                        {
                            if (storage.IsReal)
                            {
                                if (!string.IsNullOrWhiteSpace(RCB_HostingFilialeAuth.SelectedValue) && RCB_HostingFilialeAuth.SelectedValue != Guid.Empty.ToString())
                                {
                                    stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, new Guid(RCB_HostingFilialeAuth.SelectedValue));
                                }
                            }
                            else
                            {
                                stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, null);
                            }
                        }
                    }
                }

                foreach (var info in InDetailList)
                {
                    if (string.IsNullOrWhiteSpace(info.Units))
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                        info.Units = goodsInfo != null ? goodsInfo.Units : "";
                    }
                    info.NonceWarehouseGoodsStock = stockQuantitys.ContainsKey(info.RealGoodsId) ? stockQuantitys[info.RealGoodsId] : 0;
                }
            }
            RGGoods.DataSource = InDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        /// <summary>选择商品框的商品绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSelectGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.MasterTableView.Columns[3].Visible = GoodsList.Any(act => act.Luminositys.Count > 0);
            RGSelectGoods.MasterTableView.Columns[4].Visible = GoodsList.Any(act => act.Astigmias.Count > 0);
            RGSelectGoods.MasterTableView.Columns[5].Visible = GoodsList.Any(act => act.Axialss.Count > 0);
            RGSelectGoods.DataSource = GoodsList;
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
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ||
                RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择出库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ||
                RCB_StorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择出库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ||
                RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
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
            //总金额
            decimal accountReceivable = 0;
            //总数量
            double subtotalQuantity = 0;
            if (goodsStockList != null)
            {
                string numString = String.Empty; ;
                foreach (StorageRecordDetailInfo goodsStockInfo in goodsStockList)
                {
                    if (goodsStockInfo.Quantity == 0)
                    {
                        numString += goodsStockInfo.GoodsName + ":" + goodsStockInfo.Specification + ",";
                    }
                    accountReceivable += Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice;
                    subtotalQuantity += Convert.ToDouble(goodsStockInfo.Quantity);
                    if (!GetPowerOperationPoint("SettlementPrice"))
                    {
                        if (InDetailAllList.Any(ent => ent.RealGoodsId == goodsStockInfo.RealGoodsId))
                        {
                            var unitPrice = InDetailAllList.First(ent => ent.RealGoodsId == goodsStockInfo.RealGoodsId).UnitPrice;
                            if (goodsStockInfo.UnitPrice < unitPrice)
                            {
                                RAM.Alert("修改的单价不能低于当前单价!");
                                return;
                            }
                        }
                    }
                    
                }
                if (!String.IsNullOrWhiteSpace(numString))
                {
                    RAM.Alert(numString.Trim(',') + " 出库数不能为0！");
                    return;
                }
            }
            if ((decimal)subtotalQuantity == 0)
            {
                RAM.Alert("商品数量不能为0！");
                return;
            }

            //入库单备注
            string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text)
                ? txt_Description.Text.Trim()
                : "无";
            var personnelInfo = CurrentSession.Personnel.Get();
            string description = string.Format("[修改销售出库;出库人:{0};出库单备注:{1};{2}]", personnelInfo.RealName,
                storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //出入库
            var stockInfo = new StorageRecordInfo
            {
                StockId = stockId,
                WarehouseId = new Guid(RCB_Warehouse.SelectedValue),
                ThirdCompanyID = new Guid(RCB_CompanyId.SelectedValue),
                AccountReceivable = accountReceivable,
                Description = HFDescription.Value + description,
                StockState = (int)StorageRecordState.WaitAudit,
                SubtotalQuantity = (decimal)subtotalQuantity,
                StorageType = int.Parse(RCB_StorageAuth.SelectedValue),
                FilialeId = new Guid(RCB_HostingFilialeAuth.SelectedValue),
                LinkTradeCode = Hid_LinkTradeCode.Value,
                LinkTradeID = new Guid(Hid_LinkTradeID.Value),
                StockType=oldstockInfo.StockType,
                TradeCode=oldstockInfo.TradeCode
            };

            _storageManager.UpdateStorageRecordAndStorageRecordDetail(stockInfo, goodsStockList);

            //购买入库添加操作记录添加
            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName,
                stockInfo.StockId, stockInfo.TradeCode,
                OperationPoint.StorageInManager.BuyInto.GetBusinessInfo(), string.Empty);

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
                //出库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                //单价
                var unitPricelbl = RGGoods.Items[i]["UnitPrice"].FindControl("TB_UnitPrice") as TextBox;


                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var unitPrice = unitPricelbl == null ? 0 : decimal.Parse(unitPricelbl.Text);
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());

                info.StockId = stockId;
                info.GoodsId = new Guid(goodsId);
                info.Specification = specification;
                info.Quantity = quantity;
                info.UnitPrice = unitPrice;
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode;
                info.RealGoodsId = realGoodsId;
                info.Description = string.Empty;
                info.NonceWarehouseGoodsStock = 0;
                list.Add(info);
            }
            return list;
        }

        /// <summary>建立 Dictionary 存放每行选择的商品属性
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> CreateFiledGoods(string str)
        {
            var dic = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] goodsArr = str.Split('@');
                foreach (string combGoods in goodsArr)
                {
                    string regStr = Regex.Match(combGoods, @"([^_]*_){4}([^_]*)", RegexOptions.IgnoreCase).Value;
                    if (dic.Count > 0)
                    {
                        bool isExist = false;
                        foreach (var keyValuePair in dic)
                        {
                            if (keyValuePair.Key == regStr)
                            {
                                keyValuePair.Value.Add(combGoods.Split('|')[1]);
                                isExist = true;
                            }
                        }
                        if (isExist == false)
                        {
                            var fieldList = new List<string> { combGoods.Split('|')[1] };
                            dic.Add(regStr, fieldList);
                        }
                    }
                    else
                    {
                        var fieldList = new List<string> { combGoods.Split('|')[1] };
                        dic.Add(regStr, fieldList);
                    }
                }
            }
            return dic;
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

        /// <summary>
        /// 商品清单数据源未修改之前的数据
        /// </summary>
        private IList<StorageRecordDetailInfo> InDetailAllList
        {
            get
            {
                if (ViewState["InDetailAllList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["InDetailAllList"];
            }
            set { ViewState["InDetailAllList"] = value; }
        }

        /// <summary>
        /// 商品选择属性 RGSelectGoods
        /// </summary>
        public List<SearchGoodsInfo> GoodsList
        {
            get
            {
                if (ViewState["GoodsList"] == null)
                {
                    return new List<SearchGoodsInfo>();
                }
                return (List<SearchGoodsInfo>)ViewState["GoodsList"];
            }
            set
            {
                ViewState["GoodsList"] = value;
            }
        }

        #endregion

        private void GetRgGoods()
        {
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                //出库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                //单价
                var unitPricelbl = RGGoods.Items[i]["UnitPrice"].FindControl("TB_UnitPrice") as TextBox;

                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                foreach (StorageRecordDetailInfo goodsStockInfo in InDetailList)
                {
                    if (realGoodsId == goodsStockInfo.RealGoodsId)
                    {
                        goodsStockInfo.Quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                        goodsStockInfo.UnitPrice = unitPricelbl == null ? 0 : decimal.Parse(unitPricelbl.Text);
                    }
                }
            }
        }

        private void BindData(Guid stockId, bool bindShop)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            InDetailList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            if (bindShop)
                BindApplyShops(storageRecordinfo);
            foreach (var info in InDetailList)
            {
                if (string.IsNullOrWhiteSpace(info.Units))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                    info.Units = goodsInfo != null ? goodsInfo.Units : "";
                }
            }
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RCB_Warehouse.Enabled = false;
            RcbInStockChanged();
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged(false);
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
            RCB_CompanyId.SelectedValue = storageRecordinfo.ThirdCompanyID.ToString();
            HFDescription.Value = storageRecordinfo.Description;
            txt_TradeCode.Text = storageRecordinfo.TradeCode;
            Hid_LinkTradeCode.Value = storageRecordinfo.LinkTradeCode;
            Hid_LinkTradeID.Value = storageRecordinfo.LinkTradeID.ToString();
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "StorageRecordApplyOut.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
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
        private void BindCompanyId()
        {
            var companyCussentInfoList = _companyCussent.GetCompanyCussentList(new[] { CompanyType.Vendors }, State.Enable).ToList();
            RCB_CompanyId.DataSource = companyCussentInfoList;
            RCB_CompanyId.DataTextField = "CompanyName";
            RCB_CompanyId.DataValueField = "CompanyId";
            RCB_CompanyId.DataBind();
            RCB_CompanyId.Items.Insert(0, new RadComboBoxItem(""));
        }

        private void BindApplyShops(StorageRecordInfo storageRecord)
        {
            var filiales = MISService.GetAllFiliales().ToList();
            var shops = filiales.Where(p => p.ID == storageRecord.ThirdCompanyID);
            RCB_CompanyId.DataSource = shops;
            RCB_CompanyId.DataTextField = "Name";
            RCB_CompanyId.DataValueField = "ID";
            RCB_CompanyId.DataBind();
            RCB_CompanyId.Enabled = false;
        }


        //商品分类
        private void BindGoodsClass()
        {
            RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
            RCB_GoodsClass.DataBind();
            RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        #endregion
    }
}