using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
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
    /// 编辑/重送采购退货出库
    /// </summary>
    public partial class EditPurchaseReturnOutForm : Page
    {
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定仓库
                BindWarehouse();
                BindCompanyId();
                BindGoodsClass();
                if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
                {
                    btnSave.Text = "重送";
                }
                if (!string.IsNullOrEmpty(Request.QueryString["StockId"]))
                {
                    BindData(new Guid(Request.QueryString["StockId"]));
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
                list.AddRange(slist.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.Z || storages.StorageType == (int)StorageAuthType.L));
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
            var personinfo = CurrentSession.Personnel.Get();
            var wlist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                .FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (wlist != null)
            {
                if (wlist.Storages != null)
                {
                    var flist = wlist.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (flist != null)
                    {
                        list.AddRange(flist.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });

                        if (sel && flist.IsReal == false)
                        {
                            BindStockSearch(flist.IsReal);
                        }
                    }
                }
            }
            RCB_HostingFilialeAuth.DataSource = list;
            RCB_HostingFilialeAuth.DataBind();
        }

        /// <summary>
        /// 物流配送公司Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbHostingFilialeAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_StorageAuth.SelectedValue);

            var personinfo = CurrentSession.Personnel.Get();
            var wlist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (wlist != null)
            {
                if (wlist.Storages != null)
                {
                    var flist = wlist.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (flist != null)
                    {
                        if (flist.IsReal)
                        {
                            BindStockSearch(flist.IsReal);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 单据号Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbCreateNoOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_CreateNo.SelectedValue))
            {
                return;
            }
            var stockId = new Guid(RCB_CreateNo.SelectedValue);
            if (stockId != Guid.Empty)
            {
                var warehouseIdSelected = new Guid(RCB_Warehouse.SelectedValue);
                var storageType = byte.Parse(RCB_StorageAuth.SelectedValue);
                var list = _storageManager.GetStorageRecordDetailListByStockId(stockId);
                List<Guid> goodsIdOrRealGoodsIdList = list.Select(w => w.RealGoodsId).Distinct().ToList();
                var dicRealGoodsIdAndStockQuantity = new Dictionary<Guid, int>();
                //出库仓储
                var personinfo = CurrentSession.Personnel.Get();
                var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                        .FirstOrDefault(p => p.WarehouseId == warehouseIdSelected);
                if (wList != null)
                {
                    var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (slist != null)
                    {
                        Guid hostingFilialeId = Guid.Empty;
                        if (slist.IsReal) hostingFilialeId = new Guid(RCB_HostingFilialeAuth.SelectedValue);
                        //根据储位判断是否根据物流配送公司获取库存
                        dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseIdSelected,null, goodsIdOrRealGoodsIdList,
                            hostingFilialeId);
                    }
                }

                foreach (var info in list)
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                    info.Units = goodsInfo.Units;
                    if (dicRealGoodsIdAndStockQuantity != null)
                    {
                        //可出库数
                        var goodsStockKeyValuePair =
                            dicRealGoodsIdAndStockQuantity.FirstOrDefault(w => w.Key == info.RealGoodsId);
                        info.NonceWarehouseGoodsStock = goodsStockKeyValuePair.Value;
                    }
                    else
                    {
                        info.NonceWarehouseGoodsStock = 0;
                    }
                }
                InDetailList = list;
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

            DataTable dataTable = GoodsTable;
            dataTable.Clear();
            if (dataTable.Columns.Count > 4)
            {
                for (int cols = dataTable.Columns.Count; cols > 4; cols--)
                    dataTable.Columns.RemoveAt(cols - 1);
            }

            //获取分类属性，根据分类属性生成表列
            IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldListByGoodsClassId(goodsClass).ToList();
            foreach (FieldInfo fieldInfo in fieldList)
            {
                dataTable.Columns.Add(fieldInfo.FieldName, typeof(Guid));
            }
            //根据分类包含产品生成表行
            IList<GoodsInfo> goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(goodsClass, string.Empty);
            foreach (GoodsInfo goodsInfo in goodsInfoList)
            {
                DataRow dataRow = dataTable.NewRow();
                fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsInfo.GoodsId).ToList();

                dataRow[0] = goodsInfo.GoodsId;
                dataRow[1] = fieldList.Count == 0 ? 0 : 1;
                dataRow[2] = goodsInfo.GoodsCode;
                dataRow[3] = goodsInfo.GoodsName;

                foreach (FieldInfo fieldInfo in fieldList)
                {
                    dataRow[fieldInfo.FieldName] = fieldInfo.FieldId;
                }
                dataTable.Rows.Add(dataRow);
            }
            GoodsTable = dataTable;
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
                    DataTable dataTable = GoodsTable;

                    //获取产品属性，根据分类属性生成表列
                    IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();

                    foreach (FieldInfo fieldInfo in fieldList)
                    {
                        if (!dataTable.Columns.Contains(fieldInfo.FieldName))
                            dataTable.Columns.Add(fieldInfo.FieldName, typeof(Guid));
                    }
                    if (!dataTable.Rows.Contains(goodsBaseInfo.GoodsId))
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow[0] = goodsBaseInfo.GoodsId;
                        dataRow[1] = fieldList.Count == 0 ? 0 : 1;
                        dataRow[2] = goodsBaseInfo.GoodsCode;
                        dataRow[3] = goodsBaseInfo.GoodsName;
                        foreach (FieldInfo fieldInfo in fieldList)
                        {
                            dataRow[fieldInfo.FieldName] = fieldInfo.FieldId;
                        }
                        dataTable.Rows.InsertAt(dataRow, dataTable.Rows.Count);
                        GoodsTable = dataTable;
                    }
                }
            }
            RGSelectGoods.Rebind();
        }

        /// <summary>
        /// 搜索单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbCreateNoItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var warehouseId = RCB_Warehouse.SelectedValue;
            var storageType = RCB_StorageAuth.SelectedValue;
            var hostingFilialeId = RCB_HostingFilialeAuth.SelectedValue;
            var companyId = RCB_CompanyId.SelectedValue;

            if (!string.IsNullOrEmpty(warehouseId) && !string.IsNullOrEmpty(storageType) &&
                !string.IsNullOrEmpty(hostingFilialeId) && !string.IsNullOrEmpty(companyId))
            {
                var combo = (RadComboBox)sender;
                combo.Items.Clear();
                if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
                {
                    var list = _storageRecordDao.GetStorageRecordListByWarehouseIdAndCompanyId(new Guid(warehouseId),
                        int.Parse(storageType), new Guid(hostingFilialeId), new Guid(companyId),
                        (int)StorageRecordType.BuyStockIn, (int)StorageRecordState.Finished, e.Text);

                    var totalCount = list.Count;
                    if (e.NumberOfItems >= totalCount)
                        e.EndOfItems = true;
                    else
                    {
                        foreach (var item in list)
                        {
                            var rcb = new RadComboBoxItem { Value = item.StockId.ToString(), Text = item.TradeCode, };
                            combo.Items.Add(rcb);
                        }
                    }
                }
            }
        }

        #endregion

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


        /// <summary>商品明细
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = InDetailList.Sum(p => Math.Abs(p.Quantity)).ToString();
            RGGoods.DataSource = InDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        /// <summary>创建价格表时设定列宽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            var uniqueName = e.Column.UniqueName;
            if (String.Compare(uniqueName, "GoodsId", StringComparison.Ordinal) == 0)
                e.Column.Visible = false;
            else if (String.Compare(uniqueName, "IsRealGoods", StringComparison.Ordinal) == 0)
                e.Column.Visible = false;
            else if (String.Compare(uniqueName, "商品编号", StringComparison.Ordinal) == 0)
            {
                e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Column.HeaderStyle.Width = 100;
                e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            }
            else if (String.Compare(uniqueName, "商品名称", StringComparison.Ordinal) == 0)
            {
                e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Column.HeaderStyle.Width = 255;
                e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            }
            else if (String.Compare(uniqueName, "CheckBoxColumn", StringComparison.Ordinal) != 0)
            {
                e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Column.HeaderStyle.Width = 100;
                e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        /// <summary>价格表单元格绑定 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                bool isRealGoods = dataItem.GetDataKeyValue("IsRealGoods").ToString() == "1";
                if (isRealGoods)
                {
                    var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                    var fieldInfoList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();
                    foreach (GridTableCell cell in e.Item.Cells)
                    {
                        int cellIndex = e.Item.Cells.GetCellIndex(cell);
                        if (cellIndex >= 7)
                        {
                            foreach (FieldInfo fieldInfo in fieldInfoList)
                            {
                                if (String.Compare(fieldInfo.FieldId.ToString(), cell.Text, StringComparison.Ordinal) ==
                                    0)
                                {
                                    if (fieldInfo.ParentFieldId == Guid.Empty)
                                    {
                                        var rads = new RadComboBox();
                                        cell.Controls.Add(rads);
                                        rads.ID = string.Format("{0}{1}", "RCB_Field", cellIndex);
                                        rads.Height = Unit.Pixel(300);
                                        rads.Width = Unit.Pixel(100);
                                        rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                        rads.DataSource = fieldInfo.ChildFields.OrderBy(act => act.OrderIndex);
                                        rads.DataTextField = "FieldValue";
                                        rads.DataValueField = "FieldId";
                                        rads.DataBind();
                                        rads.HighlightTemplatedItems = true;
                                    }
                                    else
                                    {
                                        var rads = new RadComboBox();
                                        cell.Controls.Add(rads);
                                        rads.Height = Unit.Pixel(300);
                                        rads.Width = Unit.Pixel(100);
                                        rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                        rads.Items.Add(new RadComboBoxItem(fieldInfo.FieldValue,
                                            fieldInfo.FieldId.ToString()));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>选择商品框的商品绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.DataSource = GoodsTable;
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

            IList<GridDataItem> dataItems = RGSelectGoods.Items.Cast<GridDataItem>().Where(dataItem =>
            {
                var checkBox = dataItem.FindControl("CheckGoods") as CheckBox;
                return checkBox != null && checkBox.Checked;
            }).ToList();
            if (dataItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            //goodsStockList 选择后产品列表
            IList<StorageRecordDetailInfo> goodsStockList = InDetailList;
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            var compGoodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in dataItems)
            {
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (compGoodsIdList.Count(w => w == selectedGoodsId) == 0)
                    compGoodsIdList.Add(selectedGoodsId);
            }
            var goodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(RCB_Warehouse.SelectedValue), new Guid(RCB_HostingFilialeAuth.SelectedValue));
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(compGoodsIdList);
            foreach (GridDataItem dataItem in dataItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                bool isRealGoods = dataItem.GetDataKeyValue("IsRealGoods").ToString() == "1";
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                if (!goodsTypes.Contains(goodsInfo.GoodsType))
                {
                    RAM.Alert("“" + goodsInfo.GoodsName + "”不在该仓库物流公司采购范围内");
                    return;
                }
                IList<ChildGoodsInfo> childGoodsList = new List<ChildGoodsInfo>();
                if (!isRealGoods)
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
                    for (int i = 7; i < dataItem.Cells.Count; i++)
                    {
                        var txt = dataItem.Cells[i].Text.Trim().Replace("&nbsp;", "");
                        if (!string.IsNullOrEmpty(txt))
                        {
                            var fieldId = new Guid(txt);
                            string key = string.Format("{0}{1}{2}", dataItemClientId, "_RCB_Field", i);
                            foreach (var keyValuePair in dicFiled)
                            {
                                if (key == keyValuePair.Key)
                                {
                                    var list = keyValuePair.Value.Select(str => new Guid(str)).ToList();
                                    dicSelectedField.Add(fieldId, list);
                                    break;
                                }
                            }
                        }
                    }
                    if (dicSelectedField.Count > 0)
                    {
                        childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsIdAndFields(selectedGoodsId,
                            dicSelectedField);
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

                var warehouseIdSelected = new Guid(RCB_Warehouse.SelectedValue);
                var storageType = byte.Parse(RCB_StorageAuth.SelectedValue);
                List<Guid> goodsIdOrRealGoodsIdList = childGoodsList.Select(w => w.RealGoodsId).Distinct().ToList();
                var dicRealGoodsIdAndStockQuantity = new Dictionary<Guid, int>();
                //出库仓储
                var personinfo = CurrentSession.Personnel.Get();
                var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                        .FirstOrDefault(p => p.WarehouseId == warehouseIdSelected);
                if (wList != null)
                {
                    var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (slist != null)
                    {
                        Guid hostingFilialeId = Guid.Empty;
                        if (slist.IsReal) hostingFilialeId = new Guid(RCB_HostingFilialeAuth.SelectedValue);
                        //根据储位判断是否根据物流配送公司获取库存
                        dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseIdSelected,null, goodsIdOrRealGoodsIdList,
                            hostingFilialeId);
                    }
                }

                var goodsIdList = childGoodsList.Select(p => p.GoodsId).Distinct().ToList();
                //根据仓库Id获取供应商Id
                var dicGoodsIdAndCompanyId = _purchaseSet.GetCompanyIdByWarehouseId(warehouseIdSelected, new Guid(RCB_HostingFilialeAuth.SelectedValue));
                var companyIdList = dicGoodsIdAndCompanyId.Where(p => goodsIdList.Contains(p.Key)).Select(p => p.Value).ToList();
                //获取商品的最后一次进货价信息
                var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(warehouseIdSelected);
                goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIdList.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

                foreach (var childGoodsInfo in childGoodsList)
                {
                    //根据商品id获取供应商
                    var companyId = dicGoodsIdAndCompanyId.ContainsKey(childGoodsInfo.GoodsId) ? dicGoodsIdAndCompanyId[childGoodsInfo.GoodsId] : Guid.Empty;

                    decimal unitPrice = 0;
                    if (goodsPurchaseLastPriceInfoList.Count > 0)
                    {
                        var goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(childGoodsInfo.GoodsId) && p.ThirdCompanyId.Equals(companyId));
                        unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                    }
                    var goodsStockInfo = new StorageRecordDetailInfo
                    {
                        StockId = Guid.Empty,
                        ThirdCompanyID = companyId,
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = childGoodsInfo.RealGoodsId,
                        GoodsName = goodsInfo.GoodsName,
                        GoodsCode = goodsInfo.GoodsCode,
                        Units = goodsInfo.Units,
                        Specification = childGoodsInfo.Specification,
                        UnitPrice = unitPrice,
                        Quantity = 0,
                        NonceWarehouseGoodsStock =
                            (dicRealGoodsIdAndStockQuantity != null &&
                             dicRealGoodsIdAndStockQuantity.ContainsKey(childGoodsInfo.RealGoodsId))
                                ? dicRealGoodsIdAndStockQuantity[childGoodsInfo.RealGoodsId]
                                : 0,
                        Description = string.Empty
                    };

                    if (goodsStockList.Count == 0 ||
                        goodsStockList.Count(
                            w =>
                                w.RealGoodsId == goodsStockInfo.RealGoodsId &&
                                w.Specification == goodsStockInfo.Specification) == 0)
                    {
                        goodsStockList.Add(goodsStockInfo);
                    }
                }
            }
            InDetailList = goodsStockList;
            RGGoods.Rebind();
        }

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

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_InsterStock(object sender, EventArgs e)
        {
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
            if (string.IsNullOrEmpty(RCB_CompanyId.SelectedValue))
            {
                RAM.Alert("请选择供应商！");
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
            string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text)
                ? txt_Description.Text.Trim()
                : "无";
            var personnelInfo = CurrentSession.Personnel.Get();
            string description = string.Format("[修改采购退货出库;操作人:{0};备注:{1};{2}]", personnelInfo.RealName,
                storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //出入库
            var stockInfo = new StorageRecordInfo
            {
                StockId = stockId,
                WarehouseId = warehouseId,
                ThirdCompanyID = companyId,
                AccountReceivable = accountReceivable,
                Description = HFDescription.Value + description,
                LinkTradeCode = oldstockInfo.LinkTradeCode,
                LinkTradeID = oldstockInfo.LinkTradeID,
                StockState = (int)StorageRecordState.WaitAudit,
                SubtotalQuantity = (decimal)subtotalQuantity,
                StorageType = storageType,
                FilialeId = hostingFilialeId,
                StockType = oldstockInfo.StockType,
                TradeCode = oldstockInfo.TradeCode
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
                    dicRealGoodsIdAndStockQuantity = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList,storageRecordinfo.WarehouseId, (byte)storageRecordinfo.StorageType, storageRecordinfo.FilialeId);
                }
            }

            foreach (var info in InDetailList)
            {
                var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                info.Units = goodsInfo.Units;
                if (dicRealGoodsIdAndStockQuantity != null)
                {
                    //可出库数
                    var goodsStockKeyValuePair = dicRealGoodsIdAndStockQuantity.FirstOrDefault(w => w.Key == info.RealGoodsId);
                    info.NonceWarehouseGoodsStock = goodsStockKeyValuePair.Value;
                }
                else
                {
                    info.NonceWarehouseGoodsStock = 0;
                }
            }
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RcbInStockChanged();
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged(false);
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
            RCB_CompanyId.SelectedValue = storageRecordinfo.ThirdCompanyID.ToString();
            HFDescription.Value = storageRecordinfo.Description;
        }
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


        #region 下拉框绑定

        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId);

            //var wList =
            //    WarehouseManager.ReadInstance.GetWarehouseIsPermission(personinfo.FilialeId, personinfo.BranchId,
            //        personinfo.PositionId).ToList();
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
            var companyCussentInfoList =
                _companyCussent.GetCompanyCussentList(new[] { CompanyType.Suppliers }, State.Enable).ToList();
            RCB_CompanyId.DataSource = companyCussentInfoList;
            RCB_CompanyId.DataBind();
            RCB_CompanyId.Items.Insert(0, new RadComboBoxItem(""));
        }

        //商品分类
        private void BindGoodsClass()
        {
            RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
            RCB_GoodsClass.DataBind();
            RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        #endregion

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

        /// <summary>
        ///  更改入库仓储时重新查询库存数
        /// </summary>
        /// <param name="isReal">判断是否根据物流配送公司查询库存</param>
        private void BindStockSearch(bool isReal)
        {
            //仓库id
            var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
            if (isReal)
            {
                var hostingFilialeId = new Guid(RCB_HostingFilialeAuth.SelectedValue);
                var dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseId, null, InDetailList.Select(ent => ent.RealGoodsId).Distinct(),
                        hostingFilialeId);
                foreach (var inDetail in InDetailList)
                {
                    if (dicRealGoodsIdAndStockQuantity != null && dicRealGoodsIdAndStockQuantity.Count > 0)
                    {
                        inDetail.NonceWarehouseGoodsStock = dicRealGoodsIdAndStockQuantity.FirstOrDefault().Value;
                    }
                    else
                    {
                        inDetail.NonceWarehouseGoodsStock = 0;
                    }
                }
            }
            else
            {
                var dicRealGoodsIdAndStockQuantity = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(warehouseId, null, InDetailList.Select(ent=>ent.RealGoodsId).Distinct(), Guid.Empty);
                foreach (var inDetail in InDetailList)
                {
                    if (dicRealGoodsIdAndStockQuantity != null && dicRealGoodsIdAndStockQuantity.Count > 0)
                    {
                        inDetail.NonceWarehouseGoodsStock = dicRealGoodsIdAndStockQuantity.FirstOrDefault().Value;
                    }
                    else
                    {
                        inDetail.NonceWarehouseGoodsStock = 0;
                    }
                }
            }
            RGGoods.Rebind();
        }
    }
}