using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 添加借入申请
    /// </summary>
    public partial class GoodsBorrowApplyForm : WindowsPage
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly IBorrowLendDao _borrowLendDao = OrderInstance.GetBorrowLendDao(GlobalConfig.DB.FromType.Write);
        static readonly CodeManager _codeManager = new CodeManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_DateCreated.Text = DateTime.Now.ToString("yyyy/MM/dd");
                txt_Transactor.Text = CurrentSession.Personnel.Get().RealName;
                BindWarehouse();
                BindCompany();
                BindGoodsClass();
            }
        }

        #region 仓库、储位、物流配送公司选择事件
        /// <summary>
        /// 入库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);
            var list = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (warehouseAuth != null && warehouseAuth.Storages!=null)
            {
                list.AddRange(warehouseAuth.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.Z || storages.StorageType == (int)StorageAuthType.L));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }
            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();

            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();

            if (warehouseId!=Guid.Empty)
            {
                GetRgGoodsData();
                if (InDetailList.Count > 0)
                {
                    GetRgGoodsBackData();
                    var builder = new StringBuilder();
                    var inDetailsList=new List<StorageRecordDetailInfo>();
                    var outDetailsList = new List<StorageRecordDetailInfo>();
                    var purchasingSets =  _purchaseSet.GetPurchaseSetList(InDetailList.Select(act => act.GoodsId).Distinct().ToList(),warehouseId);
                    foreach (var goodsGroup in InDetailList.GroupBy(act => act.GoodsId))
                    {
                        if (purchasingSets.Any(act => act.GoodsId == goodsGroup.Key))
                        {
                            inDetailsList.AddRange(goodsGroup);
                            if (OutDetailList.Count > 0)
                            {
                                outDetailsList.AddRange(OutDetailList.Where(act=>act.GoodsId==goodsGroup.Key));
                            }
                        }
                        else
                        {
                            var detail = goodsGroup.FirstOrDefault();
                            if (detail != null)
                            {
                                builder.AppendFormat("{0}({1})/r/n",detail.GoodsName,detail.GoodsCode);
                            }
                        }
                    }
                    if (builder.ToString().Length > 0)
                    {
                        RAM.Alert(string.Format("商品：{0}在当前仓库未添加采购设置被移除！",builder));
                    }
                    InDetailList = inDetailsList;
                    OutDetailList = OutDetailList;
                    RG_Goods.Rebind();
                    RG_GoodsBack.Rebind();
                }
            }
        }

        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_StorageAuth.SelectedValue);

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
        }
        #endregion

        #region 商品 下拉框选择事件
        /// <summary>
        /// 选择分类
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 搜索商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
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
                        var rcb = new RadComboBoxItem { Value = item.Key, Text = item.Value, };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        /// <summary>
        /// 下拉选择商品
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
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

        #region 模型
        
        /// <summary>
        /// 
        /// </summary>
        private Guid StockId
        {
            get
            {
                if (Request.QueryString["StockId"] == null || string.IsNullOrEmpty(Request.QueryString["StockId"].Trim()))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["StockId"].Trim());
            }
        }

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

        /// <summary>切换显示单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbStockBack_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStockBack.Checked)
            {
                GetRgGoodsData();
                if (InDetailList.Count == 0)
                {
                    RAM.Alert("请添加借入单商品！");
                    cbStockBack.Checked = false;
                }
                else if (InDetailList.Count(w => w.Quantity == 0) > 0)
                {
                    RAM.Alert("借入商品中数量必须大于0！");
                    cbStockBack.Checked = false;
                }
                else
                {
                    lbTitle.Text = "借入返还单";
                    btnShowAddGoods.Visible = false;
                    RG_Goods.Visible = false;
                    RG_GoodsBack.Visible = true;
                    var goodsIds = InDetailList.Select(w => w.GoodsId).Distinct().ToList();
                    var childList = _goodsCenterSao.GetRealGoodsListByGoodsId(goodsIds).Where(w => w.GoodsId != w.RealGoodsId).ToList();
                    var dict = new Dictionary<Guid, List<ChildGoodsInfo>>();
                    foreach (var id in goodsIds)
                    {
                        var list = childList.Where(w => w.GoodsId == id).ToList();
                        if (list.Count > 0)
                            dict.Add(id, list);
                    }
                    DicGoodsAndChilds = dict;
                    //判断主商品数量是否一致
                    if (OutDetailList.Count == 0 || goodsIds.Any(act => InDetailList.Where(k => k.GoodsId == act).Sum(v => v.Quantity)
                        != OutDetailList.Where(k => k.GoodsId == act).Sum(v => v.Quantity)))
                    {
                        OutDetailList = InDetailList;
                    }
                    RG_GoodsBack.Rebind();
                }
            }
            else
            {
                GetRgGoodsBackData();
                lbTitle.Text = "借入单";
                btnShowAddGoods.Visible = true;
                RG_Goods.Visible = true;
                RG_GoodsBack.Visible = false;
            }
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

        /// <summary>选择并添加商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            var strWarehouseId = RCB_Warehouse.SelectedValue;
            if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            GetRgGoodsData();
            IList<StorageRecordDetailInfo> goodsStockList = InDetailList;
            if (RGSelectGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            var strHostingFilialeId = RCB_HostingFilialeAuth.SelectedValue;
            if (string.IsNullOrEmpty(strHostingFilialeId) || strHostingFilialeId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
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
            var purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(goodsIds, new Guid(strWarehouseId),new Guid(strHostingFilialeId));
            if (purchaseSetList.Count == 0)
            {
                RAM.Alert("选择商品在当前仓库未曾添加采购设置！");
                return;
            }
            var supportGoodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(strWarehouseId), new Guid(strHostingFilialeId));
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            foreach (GridDataItem dataItem in RGSelectGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                var purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == selectedGoodsId);
                if (purchaseSetInfo == null)
                {
                    RAM.Alert(string.Format("{0}({1})未添加商品采购设置", goodsInfo.GoodsName, goodsInfo.GoodsCode));
                    return;
                }
                if (!supportGoodsTypes.Contains(goodsInfo.GoodsType))
                {
                    RAM.Alert(string.Format("{0}({1})不在仓库物流公司的采购范围内", goodsInfo.GoodsName, goodsInfo.GoodsCode));
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
                        UnitPrice = purchaseSetInfo.PurchasePrice,
                        Quantity = 0,
                        NonceWarehouseGoodsStock = 0,
                        Description = string.Empty
                    };
                    if (
                        goodsStockList.Count(w => w.RealGoodsId == goodsStockInfo.RealGoodsId && w.UnitPrice == goodsStockInfo.UnitPrice) ==
                        0)
                    {
                        goodsStockList.Add(goodsStockInfo);
                    }
                }
            }
            InDetailList = goodsStockList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList(); ;
            RG_Goods.Rebind();
        }
        

        /// <summary>删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgGoods_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
            var unitPrice = editedItem.GetDataKeyValue("UnitPrice").ToString();
            var goodsStockInfo = InDetailList.FirstOrDefault(w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice));
            if (goodsStockInfo != null)
            {
                InDetailList.Remove(goodsStockInfo);
                RG_Goods.Rebind();
            }
        }


        #region --> RG_GoodsBack
        protected void RgGoodsBack_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
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
                        var info = list.FirstOrDefault(w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice);
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
                                RAM.Alert(string.Format("商品：{0} {1}不能大于{2}！", info.GoodsName, info.Specification, info.Quantity));
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
            var info = list.FirstOrDefault(w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice);
            if (info != null)
            {
                var newInfo = list.FirstOrDefault(w => w.GoodsId == goodsId && w.RealGoodsId == new Guid(strRealGoodsId) && w.UnitPrice == unitPrice);
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


        /// <summary>选择商品列表
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


        protected void RgGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = string.Format("{0}", InDetailList.Sum(p => p.Quantity));
            decimal TotalAmount = InDetailList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
            RG_Goods.DataSource = InDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
        }

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                var strWarehouseId = RCB_Warehouse.SelectedValue;
                if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择仓库！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) || RCB_StorageAuth.SelectedValue == "0")
                {
                    RAM.Alert("请选择入库储！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) || new Guid(RCB_HostingFilialeAuth.SelectedValue) == Guid.Empty)
                {
                    RAM.Alert("请选择物流配送公司！");
                    ctx.SetFail();
                    return;
                }
                if (RCB_CompanyId.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择供应商！");
                    ctx.SetFail();
                    return;
                }
                if (RG_GoodsBack.Visible)
                {
                    GetRgGoodsBackData();
                }
                else
                {
                    GetRgGoodsData();
                    GetRgGoodsBackData();
                    if (OutDetailList.Count == 0)
                        OutDetailList = InDetailList;
                }
                if (InDetailList.Count == 0)
                {
                    RAM.Alert("请添加商品！");
                    ctx.SetFail();
                    return;
                }
                if (OutDetailList.Count(w => w.RealGoodsId == Guid.Empty) > 0)
                {
                    RAM.Alert("请选择商品SKU！");
                    ctx.SetFail();
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
                    ctx.SetFail();
                    return;
                }

                //出入库记录
                var inStorageRecordInfo = new StorageRecordInfo
                {
                    StockId = Guid.NewGuid(),
                    FilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ? Guid.Empty : new Guid(RCB_HostingFilialeAuth.SelectedValue),
                    StorageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(int) : int.Parse(RCB_StorageAuth.SelectedValue),
                    ThirdCompanyID = new Guid(RCB_CompanyId.SelectedValue),
                    WarehouseId = new Guid(strWarehouseId),
                    TradeCode = _codeManager.GetCode(CodeType.BI),
                    LinkTradeCode = string.Empty,
                    DateCreated = DateTime.Now,
                    Transactor = CurrentSession.Personnel.Get().RealName,
                    Description = tbDescription.Text.Trim(),
                    AccountReceivable = InDetailList.Sum(w => w.Quantity * w.UnitPrice),
                    SubtotalQuantity = InDetailList.Sum(w => w.Quantity),
                    StockType = (int)StorageRecordType.BorrowIn,
                    StockState = (int)StorageRecordState.WaitAudit,
                    LinkTradeType = (int)StorageRecordLinkTradeType.Other,
                    //IsOut = true
                };

                //出入库明细
                IList<StorageRecordDetailInfo> inGoodsStockList = InDetailList;
                foreach (var detailInfo in inGoodsStockList)
                {
                    detailInfo.StockId = inStorageRecordInfo.StockId;
                }

                //借入返还单
                var borrowLendInfo = new BorrowLendInfo
                {
                    BorrowLendId = Guid.NewGuid(),
                    StockId = inStorageRecordInfo.StockId,
                    AccountReceivable = OutDetailList.Sum(w => w.Quantity * w.UnitPrice),
                    SubtotalQuantity = OutDetailList.Sum(w => w.Quantity),
                    DateCreated = DateTime.Now
                };

                //借入返还单明细
                List<BorrowLendDetailInfo> borrowLendDetailList = OutDetailList.Select(detailInfo => new BorrowLendDetailInfo
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
                    if (StockId != Guid.Empty)
                    {
                        #region --> 更新，删除老数据

                        isSuccess = _storageRecordDao.DeleteStorageRecord(StockId, out errorMessage);
                        if (!isSuccess)
                        {
                            RAM.Alert("录入借入单前异常！" + errorMessage);
                            ctx.SetFail();
                            return;
                        }
                        var oldBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(StockId);
                        if (oldBorrowLendInfo != null)
                        {
                            var result = _borrowLendDao.DeleteBorrowLendAndDetailList(oldBorrowLendInfo.BorrowLendId, out errorMessage);
                            if (result <= 0)
                            {
                                RAM.Alert("录入借入返还单前异常！" + errorMessage);
                                ctx.SetFail();
                                return;
                            }
                        }

                        #endregion
                    }
                    var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var personnelInfo = CurrentSession.Personnel.Get();
                    string tbDes = !string.IsNullOrWhiteSpace(tbDescription.Text) ? tbDescription.Text.Trim() : "无";
                    var description = string.Format("[借入单,(申请备注:{0}),申请人:{1};{2}]", tbDes, personnelInfo.RealName, dateTime);
                    inStorageRecordInfo.Description = description;
                    isSuccess = _storageManager.NewAddStorageRecordAndDetailList(inStorageRecordInfo, inGoodsStockList, out errorMessage);

                    if (!isSuccess)
                    {
                        RAM.Alert("录入借入单异常！" + errorMessage);
                        ctx.SetFail();
                        return;
                    }
                    isSuccess = _borrowLendDao.AddBorrowLendAndDetailList(borrowLendInfo, borrowLendDetailList, out errorMessage);
                    if (!isSuccess)
                    {
                        RAM.Alert("录入借入返还单异常！" + errorMessage);
                        ctx.SetFail();
                        return;
                    }


                    //在出入库记录中添加借入返还单
                    var description1 = string.Format("[借入单申请成功生成借入返还单,申请人:{0};{1}]", personnelInfo.RealName, dateTime);
                    var addStockInfo = _storageRecordDao.GetStorageRecord(inStorageRecordInfo.StockId);
                    var addBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(inStorageRecordInfo.StockId);
                    var addBorrowLendDetailList = _borrowLendDao.GetBorrowLendDetailList(addBorrowLendInfo.BorrowLendId);
                    var addstorageRecordInfo = new StorageRecordInfo
                    {
                        StockId = addBorrowLendInfo.BorrowLendId,
                        FilialeId = addStockInfo.FilialeId,
                        ThirdCompanyID = addStockInfo.ThirdCompanyID,
                        WarehouseId = addStockInfo.WarehouseId,
                        TradeCode = _codeManager.GetCode(CodeType.BO),
                        LinkTradeCode = addStockInfo.TradeCode,
                        DateCreated = DateTime.Now,
                        Transactor = CurrentSession.Personnel.Get().RealName,
                        Description = description1,
                        AccountReceivable = -addBorrowLendDetailList.Sum(w => w.Quantity * w.UnitPrice),
                        SubtotalQuantity = -addBorrowLendDetailList.Sum(w => w.Quantity),
                        StockType = (int)StorageRecordType.BorrowOut,
                        StockState = (int)StorageRecordState.WaitAudit,
                        LinkTradeID = addStockInfo.StockId,
                        StorageType = addStockInfo.StorageType,
                        LinkTradeType = (int)StorageRecordLinkTradeType.Other,
                        //IsOut = true
                    };
                    IList<StorageRecordDetailInfo> storageRecordDetailList = addBorrowLendDetailList.Select(borrowLendDetailInfo => new StorageRecordDetailInfo
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
                    isSuccess = _storageManager.NewAddStorageRecordAndDetailList(addstorageRecordInfo, storageRecordDetailList, out errorMessage);
                    if (!isSuccess)
                    {
                        RAM.Alert("录入借入返还单异常！" + errorMessage);
                        ctx.SetFail();
                        return;
                    }
                    ts.Complete();
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            });
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

        /// <summary>获取出入库数据源
        /// </summary>
        private void GetRgGoodsData()
        {
            IList<StorageRecordDetailInfo> goodsStockList = new List<StorageRecordDetailInfo>();
            foreach (GridDataItem dataItem in RG_Goods.Items)
            {
                var tbQuantity = (TextBox)dataItem.FindControl("TB_Quantity");

                var info = new StorageRecordDetailInfo
                {
                    GoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString()),
                    RealGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString()),
                    GoodsCode = dataItem["GoodsCode"].Text,
                    GoodsName = dataItem["GoodsName"].Text,
                    Specification = string.IsNullOrEmpty(dataItem["Specification"].Text) ? string.Empty : dataItem["Specification"].Text.Replace("&nbsp;", ""),
                    Units = dataItem["Units"].Text,
                    UnitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString()),
                    Quantity = string.IsNullOrEmpty(tbQuantity.Text) ? 0 : int.Parse(tbQuantity.Text)
                };
                if (goodsStockList.Count(w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice) == 0)
                    goodsStockList.Add(info);
                else
                {
                    var goodsStockInfo = goodsStockList.FirstOrDefault(w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice);
                    if (goodsStockInfo != null)
                    {
                        goodsStockInfo.Quantity += info.Quantity;
                    }
                }
            }
            InDetailList = goodsStockList;
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
                    Specification = string.IsNullOrEmpty(rcbSpecification.Text) ? string.Empty : rcbSpecification.Text.Replace("&nbsp;", ""),
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
                    var goodsStockInfo =
                        goodsStockList.FirstOrDefault(
                            w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice);
                    if (goodsStockInfo != null)
                    {
                        goodsStockInfo.Quantity += info.Quantity;
                    }
                }
            }
            OutDetailList = goodsStockList;
        }

        private void ReloadGrid()
        {
            if (RG_Goods.Visible)
            {
                GetRgGoodsData();
                if(InDetailList.Count>0)
                    RG_Goods.Rebind();
            }
            else
            {
                GetRgGoodsBackData();
                if (OutDetailList.Count > 0)
                    RG_GoodsBack.Rebind();
            }
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
                    new CompanyCussentInfo{CompanyId = Guid.Empty,CompanyName = ""}
                };
            var companyCussentList = _companyCussent.GetCompanyCussentList(new[] { CompanyType.Suppliers, CompanyType.Other, CompanyType.MemberGeneralLedger }, State.Enable);
            companyData.AddRange(companyCussentList);
            var filialeList = CacheCollection.Filiale.GetList().Where(w => w.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            companyData.AddRange(filialeList.Select(filialeInfo => new CompanyCussentInfo { CompanyId = filialeInfo.ID, CompanyName = filialeInfo.Name }));
            RCB_CompanyId.DataSource = companyData;
            RCB_CompanyId.DataBind();
        }

        /// <summary>
        /// 商品分类
        /// </summary>
        private void BindGoodsClass()
        {
            RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
            RCB_GoodsClass.DataBind();
            RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        #endregion
    }
}