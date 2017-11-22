using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
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
using CodeType = ERP.Enum.CodeType;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 添加内部采购
    /// </summary>
    public partial class InnerPurchaseForm : Page
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly RealTimeGrossSettlementManager _realTimeGrossSettlementManager = new RealTimeGrossSettlementManager();
        static readonly CodeManager _code = new CodeManager();
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly IInternalPriceSetDao _internalPriceSetDao = new InternalPriceSetDao(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent=new CompanyCussent(GlobalConfig.DB.FromType.Write);
        public DateTime dt = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_DateCreated.Text = dt.ToString("yyyy/MM/dd");
                txt_Transactor.Text = CurrentSession.Personnel.Get().RealName;
                //绑定仓库
                BindOutWarehouse();
                BindInWarehouse();
                BindGoodsClass();
            }
            else
            {
                GetRgGoods();
            }
        }

        #region 下拉框选择事件
        /// <summary>
        /// 出库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbOutStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteSelectGoodsList();
            Guid warehouseId = string.IsNullOrEmpty(RCB_OutWarehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_OutWarehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null && warehouseAuth.Storages != null)
            {
                list.AddRange(warehouseAuth.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L || act.StorageType == (Byte)StorageAuthType.S));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_OutStorageAuth.DataSource = list;
            RCB_OutStorageAuth.DataBind();


            //清空物流配送公司下拉框
            RCB_OutHostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_OutHostingFilialeAuth.DataBind();

            var details = GetRgGoodsData(Guid.Empty);
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }

        /// <summary>
        /// 出库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbOutStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteSelectGoodsList();
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_OutWarehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_OutWarehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_OutStorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_OutStorageAuth.SelectedValue);

            //绑定物流配送公司
            var list = new List<HostingFilialeAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (storageAuth != null && storageAuth.Filiales != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_OutHostingFilialeAuth.DataSource = list;
            RCB_OutHostingFilialeAuth.DataBind();

            var details = GetRgGoodsData(Guid.Empty);
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }


        /// <summary>
        /// 出库物流配送公司Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbOutHostingFilialeAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteSelectGoodsList();
            var details = GetRgGoodsData(Guid.Empty);
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }


        /// <summary>
        /// 入库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_InWarehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_InWarehouse.SelectedValue);

            //绑定入库储
            var list = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null && warehouseAuth.Storages != null)
            {
                list.AddRange(warehouseAuth.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L || act.StorageType == (Byte)StorageAuthType.S));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_InStorageAuth.DataSource = list;
            RCB_InStorageAuth.DataBind();


            //清空物流配送公司下拉框
            RCB_InHostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_InHostingFilialeAuth.DataBind();

            var details = GetRgGoodsData(Guid.Empty);
            if (details.Count > 0)
            {
                RGGoods.Rebind();
            }
        }

        /// <summary>
        /// 出库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_InWarehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_InWarehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_InStorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_InStorageAuth.SelectedValue);

            //绑定物流配送公司
            var list = new List<HostingFilialeAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (storageAuth != null && storageAuth.Filiales != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_InHostingFilialeAuth.DataSource = list;
            RCB_InHostingFilialeAuth.DataBind();

            var details = GetRgGoodsData(Guid.Empty);
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
        protected void RcbInHostingFilialeAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var details = GetRgGoodsData(Guid.Empty);
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
            var strWarehouseId = RCB_OutWarehouse.SelectedValue;
            if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            IList<StorageRecordDetailInfo> goodsStockList = GetRgGoodsData(Guid.NewGuid());
            if (RGSelectGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            var goodsIds = new List<Guid>();
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (goodsIds.Count(w => w == goodsId) == 0)
                    goodsIds.Add(goodsId);
            }
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
            if (goodsList.Count == 0)
            {
                RAM.Alert("商品中心异常，请稍候再试！");
                return;
            }
            var listGoodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(RCB_OutWarehouse.SelectedItem.Value),
                 new Guid(RCB_OutHostingFilialeAuth.SelectedItem.Value));
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                if (!listGoodsTypes.Contains(goodsInfo.GoodsType))
                {
                    RAM.Alert(goodsInfo.GoodsName + ":" + "不在当前物流配送公司代发范围");
                    return;
                }
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsInfo.GoodsId);
                if (current == null) continue;
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
                    var internalPriceSet = _internalPriceSetDao.GetInternalPriceSetInfoList(goodsInfo.GoodsType, new Guid(RCB_OutHostingFilialeAuth.SelectedItem.Value));
                    var goodsStockInfo = new StorageRecordDetailInfo
                    {
                        StockId = Guid.Empty,
                        ThirdCompanyID = Guid.Empty,
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = childGoodsInfo.RealGoodsId,
                        GoodsName = goodsInfo.GoodsName,
                        GoodsCode = goodsInfo.GoodsCode,
                        GoodsType = goodsInfo.GoodsType,
                        Specification = childGoodsInfo.Specification,
                        Units = goodsInfo.Units,
                        UnitPrice = _realTimeGrossSettlementManager.GetLatestUnitPrice(new Guid(RCB_OutHostingFilialeAuth.SelectedItem.Value), goodsInfo.GoodsId) * (internalPriceSet == null ? 0 : Convert.ToDecimal(internalPriceSet.ReserveProfitRatio)),//利润占比 * 结算价
                        Quantity = 0,
                        NonceWarehouseGoodsStock = 0,
                        Description = string.Empty
                    };
                    if (goodsStockList.Count(w => w.RealGoodsId == goodsStockInfo.RealGoodsId) == 0)
                    {
                        goodsStockList.Add(goodsStockInfo);
                    }
                }
            }
            InDetailList = goodsStockList;
            RGGoods.Rebind();
        }

        //价格表单元格绑定
        protected void RGSelectGoods_ItemDataBound(object sender, GridItemEventArgs e)
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
                    var goodsStockInfo = InDetailList.FirstOrDefault(w => w.RealGoodsId == realGoodsId);
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

        private void DeleteSelectGoodsList()
        {
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                var goodsStockInfo = InDetailList.FirstOrDefault(w => w.RealGoodsId == realGoodsId);
                if (goodsStockInfo != null)
                    InDetailList.Remove(goodsStockInfo);
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
                var warehouseIdSelected = new Guid(RCB_OutWarehouse.SelectedValue);
                if (String.IsNullOrWhiteSpace(RCB_OutStorageAuth.SelectedValue))
                {
                    return;
                }
                var storageType = byte.Parse(RCB_OutStorageAuth.SelectedValue);
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
                                if (!string.IsNullOrWhiteSpace(RCB_OutHostingFilialeAuth.SelectedValue) && RCB_OutHostingFilialeAuth.SelectedValue != Guid.Empty.ToString())
                                {
                                    stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, new Guid(RCB_OutHostingFilialeAuth.SelectedValue));
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
        protected void RGSelectGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
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
            if (string.IsNullOrEmpty(RCB_OutWarehouse.SelectedValue) ||
                RCB_OutWarehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择出库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_OutStorageAuth.SelectedValue) ||
                RCB_OutStorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择出库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_OutHostingFilialeAuth.SelectedValue) ||
                RCB_OutHostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择出库物流配送公司！");
                return;
            }

            if (string.IsNullOrEmpty(RCB_InWarehouse.SelectedValue) ||
                RCB_InWarehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择入库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_InStorageAuth.SelectedValue) ||
                RCB_InStorageAuth.SelectedValue == "0")
            {
                RAM.Alert("请选择入库仓储！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_InHostingFilialeAuth.SelectedValue) ||
                RCB_InHostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择入库物流配送公司！");
                return;
            }
            if (RCB_OutWarehouse.SelectedValue == RCB_InWarehouse.SelectedValue)
            {
                RAM.Alert("请选择不同的仓库！");
                return;
            }
            if (RCB_OutHostingFilialeAuth.SelectedValue == RCB_InHostingFilialeAuth.SelectedValue)
            {
                RAM.Alert("请选择不同的公司！");
                return;
            }

            var stockId = Guid.NewGuid();
            //出入库详细
            var goodsStockList = GetRgGoodsData(stockId);//stockId
            var listGoodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(RCB_InWarehouse.SelectedItem.Value),
                new Guid(RCB_InHostingFilialeAuth.SelectedItem.Value));
            foreach (var goodsList in goodsStockList)
            {
                if (!listGoodsTypes.Contains(goodsList.GoodsType))
                {
                    RAM.Alert(goodsList.GoodsName + ":" + "入库物流配送公司不包含该商品类型");
                    return;
                }
            }
            //总金额
            decimal accountReceivable = 0;
            //总数量
            double subtotalQuantity = 0;
            if (goodsStockList.Count>0)
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
            string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text) ? txt_Description.Text.Trim() : "无";
            var personnelInfo = CurrentSession.Personnel.Get();
            string description = string.Format("[内部采购;出库人:{0};出库单备注:{1};{2}]", personnelInfo.RealName, storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


            string transactor = txt_Transactor.Text;
            //出入库

            var purchasingId = Guid.Empty;
            String linkTradeCode = String.Empty;
            var thirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(new Guid(RCB_InHostingFilialeAuth.SelectedItem.Value));
            if (thirdCompanyId == Guid.Empty)
            {
                RAM.Alert("入库物流配送公司未关联往来单位");
                return;
            }
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {

                    #region 出库单
                    var outStockInfo = new StorageRecordInfo
                    {
                        StockId = stockId,
                        FilialeId = new Guid(RCB_OutHostingFilialeAuth.SelectedValue),//出库
                        WarehouseId = new Guid(RCB_OutWarehouse.SelectedValue),
                        ThirdCompanyID = thirdCompanyId,
                        RelevanceFilialeId = Guid.Empty,//入库
                        RelevanceWarehouseId = Guid.Empty,
                        AccountReceivable = accountReceivable,
                        DateCreated = DateTime.Now,
                        Description = description,
                        LinkTradeCode = linkTradeCode,
                        LinkTradeID = purchasingId,
                        StockState = (int)StorageRecordState.WaitAudit,
                        StockType = (int)StorageRecordType.InnerPurchase,
                        StockValidation = false,
                        SubtotalQuantity = (decimal)subtotalQuantity,
                        TradeCode = _code.GetCode(CodeType.CC),
                        Transactor = transactor,
                        //IsOut = true,
                        StorageType = int.Parse(RCB_OutStorageAuth.SelectedValue),
                        LinkTradeType = (int)StorageRecordLinkTradeType.Other,
                        TradeBothPartiesType = (int)TradeBothPartiesType.HostingToHosting
                    };
                    var outResult = _storageManager.NewInsertStockAndGoods(outStockInfo, goodsStockList);
                    if (outResult)
                    {
                        //购买入库添加操作记录添加
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, outStockInfo.StockId,
                            outStockInfo.TradeCode,
                            OperationPoint.StorageInManager.BuyInto.GetBusinessInfo(), string.Empty);
                    }
                    else
                    {
                        RAM.ResponseScripts.Add("alert('保存失败！')");
                    }
                    #endregion
                    

                    //插入关联
                    _storageRecordDao.InsertInnerPurchaseRelationInfo(outStockInfo.StockId, Guid.Empty, Guid.Empty, new Guid(RCB_OutWarehouse.SelectedValue), new Guid(RCB_OutHostingFilialeAuth.SelectedValue), int.Parse(RCB_OutStorageAuth.SelectedValue), new Guid(RCB_InWarehouse.SelectedValue), new Guid(RCB_InHostingFilialeAuth.SelectedValue), int.Parse(RCB_InStorageAuth.SelectedValue));
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    RAM.Alert(ex.Message);
                    return;
                }
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>商品明细获得入库商品详细
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


                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                var goodsType = RGGoods.Items[i].GetDataKeyValue("GoodsType").ToString();
                var unitPrice = RGGoods.Items[i].GetDataKeyValue("UnitPrice").ToString();

                info.StockId = stockId;
                info.GoodsId = new Guid(goodsId);
                info.Specification = specification;
                info.Quantity = quantity;
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode;
                info.RealGoodsId = realGoodsId;
                info.Description = string.Empty;
                info.NonceWarehouseGoodsStock = 0;
                info.GoodsType = Convert.ToByte(goodsType);
                info.UnitPrice = Convert.ToDecimal(unitPrice);
                list.Add(info);
            }
            return list;
        }

        /// <summary> 建立 Dictionary 存放每行选择的商品属性
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

        public class OutWarehouse
        {
            public string OutWarehouseName { get; set; }
            public Guid OutWarehouseId { get; set; }
        }
        public class InWarehouse
        {
            public string InWarehouseName { get; set; }
            public Guid InWarehouseId { get; set; }
        }
        #endregion

        private void GetRgGoods()
        {
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                //出库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;

                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                foreach (StorageRecordDetailInfo goodsStockInfo in InDetailList)
                {
                    if (realGoodsId == goodsStockInfo.RealGoodsId)
                    {
                        goodsStockInfo.Quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                    }
                }
            }
        }

        #region 下拉框绑定
        /// <summary>
        /// 绑定出库仓储
        /// </summary>
        private void BindOutWarehouse()
        {
            var outWarehouseList = CurrentSession.Personnel.WarehouseList.Select(list => new OutWarehouse
            {
                OutWarehouseName = list.WarehouseName,
                OutWarehouseId = list.WarehouseId
            }).ToList();
            RCB_OutWarehouse.DataSource = outWarehouseList;
            RCB_OutWarehouse.DataTextField = "OutWarehouseName";
            RCB_OutWarehouse.DataValueField = "OutWarehouseId";
            RCB_OutWarehouse.DataBind();
            RCB_OutWarehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindInWarehouse()
        {
            var inWarehouseList = CurrentSession.Personnel.WarehouseList.Select(list => new InWarehouse
            {
                InWarehouseName = list.WarehouseName,
                InWarehouseId = list.WarehouseId
            }).ToList();
            RCB_InWarehouse.DataSource = inWarehouseList;
            RCB_InWarehouse.DataTextField = "InWarehouseName";
            RCB_InWarehouse.DataValueField = "InWarehouseId";
            RCB_InWarehouse.DataBind();
            RCB_InWarehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
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