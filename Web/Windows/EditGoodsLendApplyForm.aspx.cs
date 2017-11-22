using System;
using System.Collections.Generic;
using System.Data;
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
using WMSEnum= KeedeGroup.WMS.Infrastructure.CrossCutting.Enum;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 编辑/重送借出申请
    /// </summary>
    public partial class EditGoodsLendApplyForm : System.Web.UI.Page
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao =new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly CodeManager _codeManager = new CodeManager();
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly IBorrowLendDao _borrowLendDao = OrderInstance.GetBorrowLendDao(GlobalConfig.DB.FromType.Write);
        readonly WMSSao _wmsSao=new WMSSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWarehouse();
                BindCompany();
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
        }

        public IDictionary<byte, string> ShelfTypeDic
        {
            get
            {
                if(ViewState["ShelfTypeDic"]==null)return new Dictionary<byte, string>();
                return (Dictionary<byte, string>) ViewState["ShelfTypeDic"];
            }
            set { ViewState["ShelfTypeDic"] = value; }
        }

        public IDictionary<Guid, Dictionary<byte,int>> GoodsStockDics
        {
            get
            {
                if (ViewState["GoodsStockDics"] == null) return new Dictionary<Guid, Dictionary<byte, int>>();
                return (IDictionary<Guid, Dictionary<byte, int>>)ViewState["GoodsStockDics"];
            }
            set { ViewState["GoodsStockDics"] = value; }
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
            var slist = WarehouseAuths.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (slist != null)
            {
                list.AddRange(slist.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.Z || storages.StorageType == (int)StorageAuthType.L || storages.StorageType == (int)StorageAuthType.S));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();

            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();

            if (warehouseId != Guid.Empty)
            {
                GetRgGoodsData();
                if (InDetailList.Count > 0)
                {
                    var builder = new StringBuilder();
                    var purchaseSetList = _purchaseSet.GetPurchaseSetList(InDetailList.Select(act => act.GoodsId).Distinct().ToList(), warehouseId);
                    var inDetails = new List<StorageRecordDetailInfo>();
                    var outDetail = new List<StorageRecordDetailInfo>();
                    GetRgGoodsBackData();
                    foreach (var goodsGroup in InDetailList.GroupBy(act => act.GoodsId))
                    {
                        if (purchaseSetList.All(act => act.GoodsId != goodsGroup.Key))
                        {
                            var detail = goodsGroup.First();
                            builder.AppendFormat("{0}({1})/r/n", detail.GoodsName, detail.GoodsCode);
                        }
                        else
                        {
                            inDetails.AddRange(goodsGroup);
                            if (OutDetailList != null && OutDetailList.Count > 0)
                            {
                                outDetail.AddRange(OutDetailList.Where(act=>act.GoodsId==goodsGroup.Key));
                            }
                        }
                    }
                    if (builder.ToString().Length > 0)
                    {
                        RAM.Alert(string.Format("商品：{0}在该仓库下没有添加采购设置被移除！", builder));
                    }
                    InDetailList = inDetails;
                    OutDetailList = outDetail;
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
            var warehouse = WarehouseAuths .FirstOrDefault(p => p.WarehouseId == warehouseId);
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
                    BindStockSearch();
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
            BindStockSearch();
        }

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
            RgSelectGoods.Rebind();
        }

        /// <summary>
        /// 搜索商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox) sender;
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
                        var rcb = new RadComboBoxItem {Value = item.Key, Text = item.Value,};
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
            RgSelectGoods.Rebind();
        }

        #endregion

        #region --> 添加商品

        // 建立 Dictionary 存放每行选择的商品属性
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
                            var fieldList = new List<string> {combGoods.Split('|')[1]};
                            dic.Add(regStr, fieldList);
                        }
                    }
                    else
                    {
                        var fieldList = new List<string> {combGoods.Split('|')[1]};
                        dic.Add(regStr, fieldList);
                    }
                }
            }
            return dic;
        }

        protected void RgSelectGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RgSelectGoods.MasterTableView.Columns[3].Visible = GoodsList.Any(act => act.Luminositys.Count > 0);
            RgSelectGoods.MasterTableView.Columns[4].Visible = GoodsList.Any(act => act.Astigmias.Count > 0);
            RgSelectGoods.MasterTableView.Columns[5].Visible = GoodsList.Any(act => act.Axialss.Count > 0);
            RgSelectGoods.DataSource = GoodsList;
        }

        //价格表单元格绑定
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

        //选择并添加商品
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            var strWarehouseId = RCB_Warehouse.SelectedValue;
            var strHostingFilialeId = RCB_HostingFilialeAuth.SelectedValue;
            if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            GetRgGoodsData();
            IList<StorageRecordDetailInfo> goodsStockList = InDetailList;
            if (RgSelectGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            if (string.IsNullOrEmpty(strHostingFilialeId) ||
               strHostingFilialeId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }
            var goodsIds = new List<Guid>();
            foreach (GridDataItem dataItem in RgSelectGoods.SelectedItems)
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
            var supportGoodsTypes = WMSSao.GetPurchaseGoodsTypes(new Guid(strWarehouseId), new Guid(strHostingFilialeId));
            var purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(goodsIds, new Guid(strWarehouseId), new Guid(strHostingFilialeId));
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            foreach (GridDataItem dataItem in RgSelectGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                var purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == selectedGoodsId);
                if (purchaseSetInfo == null)
                {
                    RAM.Alert(string.Format("{0}({1})未添加商品采购设置",goodsInfo.GoodsName,goodsInfo.GoodsCode));
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
                        UnitPrice =  purchaseSetInfo.PurchasePrice,
                        Quantity = 0,
                        NonceWarehouseGoodsStock = 0,
                        Description = string.Empty,
                        ShelfType = (Byte)WMSEnum.ShelfType.Normal
                    };
                    if (goodsStockList.Count(w => w.RealGoodsId == goodsStockInfo.RealGoodsId && w.ShelfType== (Byte)WMSEnum.ShelfType.Normal && w.UnitPrice == goodsStockInfo.UnitPrice) ==
                        0)
                    {
                        goodsStockList.Add(goodsStockInfo);
                    }
                }
            }
            InDetailList = goodsStockList;
            RG_Goods.Rebind();
        }

        #endregion

        #region --> RG_GoodsBack

        protected void RgGoodsBack_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? byte.MinValue : byte.Parse(RCB_StorageAuth.SelectedValue);
            if (OutDetailList.Count > 0)
            {
                List<Guid> goodsIdOrRealGoodsIdList = OutDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
                var warehouseIdSelected = new Guid(RCB_Warehouse.SelectedValue);
                
                var hostingFilialeId = !string.IsNullOrWhiteSpace(RCB_HostingFilialeAuth.SelectedValue)
                    ? new Guid(RCB_HostingFilialeAuth.SelectedValue)
                    : Guid.Empty;
                var stockQuantitys=new Dictionary<Guid, int>();
                if (storageType>0)
                {
                    stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, hostingFilialeId);
                }

                foreach (var info in OutDetailList)
                {
                    if (string.IsNullOrWhiteSpace(info.Units))
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                        info.Units =goodsInfo!=null?goodsInfo.Units:"";
                    }
                    info.NonceWarehouseGoodsStock = stockQuantitys.ContainsKey(info.RealGoodsId)?stockQuantitys[info.RealGoodsId]:0;
                }
            }
            RG_GoodsBack.DataSource = OutDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
            RG_GoodsBack.MasterTableView.Columns.FindByUniqueName("ShelfType").Display = storageType ==
                                                                                         (Byte) WMSEnum.StorageType.S;
        }

        protected void RgGoodsBack_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem) e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
                var rcbSpecification = (RadComboBox) dataItem.FindControl("rcbSpecification");
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
                    var tbQuantity = (TextBox) dataItem.FindControl("TB_Quantity");
                    tbQuantity.ReadOnly = true;
                }
            }
        }

        protected void RG_GoodsBack_OnItemCommand(object sender, GridCommandEventArgs e)
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
                    var shelfType = Convert.ToByte(dataItem.GetDataKeyValue("ShelfType"));
                    var rcbSpecification = (RadComboBox) dataItem.FindControl("rcbSpecification");
                    var tbQuantity = (TextBox) dataItem.FindControl("TB_Quantity");
                    var strWarehouseId = RCB_Warehouse.SelectedValue;
                    if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
                    {
                        RAM.Alert("请选择仓库！");
                        return;
                    }
                    if (string.IsNullOrEmpty(tbQuantity.Text.Trim()) || tbQuantity.ReadOnly)
                        return;
                    var strRealGoodsId = rcbSpecification.SelectedValue;
                    if (strRealGoodsId == Guid.Empty.ToString())
                    {
                        RAM.Alert("请先选择SKU！");
                    }
                    else
                    {
                        IList<StorageRecordDetailInfo> list = (List<StorageRecordDetailInfo>) OutDetailList.DeepCopy();
                        var info =
                            list.FirstOrDefault(
                                w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice && w.ShelfType== shelfType);
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
                                    var newInfo = (StorageRecordDetailInfo) info.DeepCopy();
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
            var rcbSpecification = (RadComboBox) sender;
            var dataItem = (GridDataItem) rcbSpecification.Parent.Parent;
            var strRealGoodsId = rcbSpecification.SelectedValue;

            var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
            var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
            var unitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString());
            var shelfType = Convert.ToByte(dataItem.GetDataKeyValue("ShelfType"));

            IList<StorageRecordDetailInfo> list = (List<StorageRecordDetailInfo>) OutDetailList.DeepCopy();
            var info =list.FirstOrDefault( w => w.GoodsId == goodsId && w.RealGoodsId == realGoodsId && w.UnitPrice == unitPrice && w.ShelfType== shelfType);
            if (info != null) 
            {
                var newInfo =list.FirstOrDefault(w => w.GoodsId == goodsId && w.RealGoodsId == new Guid(strRealGoodsId) && w.UnitPrice == unitPrice && w.ShelfType== shelfType);
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

        /// <summary>切换借出单
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
                    RAM.Alert("请添加借出单商品！");
                    cbStockBack.Checked = false;
                }
                else if (InDetailList.Count(w => w.Quantity == 0) > 0)
                {
                    RAM.Alert("借出商品中数量必须大于0！");
                    cbStockBack.Checked = false;
                }
                else
                {
                    lbTitle.Text = "借出返还单";
                    btnShowAddGoods.Visible = false;
                    RG_Goods.Visible = false;
                    RG_GoodsBack.Visible = true;
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
                    //判断主商品数量是否一致
                    if (OutDetailList.Count == 0 ||
                        goodsIds.Any(act => InDetailList.Where(k => k.GoodsId == act).Sum(v => v.Quantity)
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
                lbTitle.Text = "借出单";
                btnShowAddGoods.Visible = true;
                RG_Goods.Visible = true;
                RG_GoodsBack.Visible = false;
                RG_Goods.Rebind();
            }
        }

        protected void RgGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? byte.MinValue : byte.Parse(RCB_StorageAuth.SelectedValue);
            bool show = storageType == (Byte)WMSEnum.StorageType.S;
            if (InDetailList.Count > 0)
            {
                List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
                var stockQuantityDtos = new List<ERP.Model.WMS.UpDownGoodsQuantityLendDTO>();
                var warehouseIdSelected = new Guid(RCB_Warehouse.SelectedValue);
                var hostingFilialeId = !string.IsNullOrWhiteSpace(RCB_HostingFilialeAuth.SelectedValue)
                    ? new Guid(RCB_HostingFilialeAuth.SelectedValue)
                    : Guid.Empty;
                if (storageType>0 )
                {
                    var warehouse = WarehouseAuths.FirstOrDefault(p => p.WarehouseId == warehouseIdSelected);
                    if (warehouse != null)
                    {
                        stockQuantityDtos = WMSSao.GetGoodsStockByStorageTypeLend(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, hostingFilialeId);
                    }
                }

                foreach (var info in InDetailList)
                {
                    if (string.IsNullOrWhiteSpace(info.Units))
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                        info.Units = goodsInfo.Units;
                    }
                    info.NonceWarehouseGoodsStock =
                    show? stockQuantityDtos.Where(ent => ent.RealGoodsId == info.RealGoodsId && ent.ShelfType == info.ShelfType).Sum(ent => ent.Quantity):
                    stockQuantityDtos.Where(ent => ent.RealGoodsId == info.RealGoodsId).Sum(ent => ent.Quantity);
                }
            }
            Lab_TotalNumber.Text = InDetailList.Sum(p => Math.Abs(p.Quantity)).ToString();
            decimal TotalAmount = InDetailList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();
            RG_Goods.DataSource = InDetailList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
            RG_Goods.MasterTableView.Columns.FindByUniqueName("ShelfType").Display = show;
        }

        protected void RgGoods_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem) e.Item;
            var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
            var unitPrice = editedItem.GetDataKeyValue("UnitPrice").ToString();
            var shelfType = Convert.ToByte(editedItem.GetDataKeyValue("ShelfType"));
            var goodsStockInfo =
                InDetailList.FirstOrDefault(
                    w => w.RealGoodsId == realGoodsId && w.UnitPrice == Convert.ToDecimal(unitPrice) && w.ShelfType== shelfType);
            if (goodsStockInfo != null)
            {
                InDetailList.Remove(goodsStockInfo);
                RG_Goods.Rebind();
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            var oldstockInfo = _storageRecordDao.GetStorageRecord(new Guid(Request.QueryString["StockId"]));
            if (!string.IsNullOrEmpty(Request.QueryString["IsAgain"]) && Request.QueryString["IsAgain"] == "1")
            {
                if (oldstockInfo.StockState != (int)StorageRecordState.Refuse)
                {
                    RAM.Alert("当前单据状态已改变，重送失败！");
                    return;
                }
            }
            var strWarehouseId = RCB_Warehouse.SelectedValue;
            if (string.IsNullOrEmpty(strWarehouseId) || strWarehouseId == Guid.Empty.ToString())
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
            if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) || RCB_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_CompanyId.SelectedValue) || RCB_CompanyId.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择供应商！");
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
                return;
            }
            if (OutDetailList.Count(w => w.RealGoodsId == Guid.Empty) > 0)
            {
                RAM.Alert("请选择商品SKU！");
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
                RAM.Alert("借出单中商品列表中数量不能为零！\n" + strb);
                return;
            }

            if (OutDetailList.Sum(act => act.Quantity) != InDetailList.Sum(act => act.Quantity))
            {
                RAM.Alert("借出单和借出返还单商品总数不一致！\n");
                return;
            }
            var inGoodsNames = InDetailList.Select(act => act.GoodsName).Distinct();
            var outGoodsNames = OutDetailList.Select(act => act.GoodsName).Distinct();
            if (inGoodsNames.Any(act => !outGoodsNames.Contains(act)))
            {
                RAM.Alert("借出单和借出返还单商品存在商品名称不一致！\n");
                return;
            }
            //出入库记录
            var inStorageRecordInfo = new StorageRecordInfo
            {
                StockId = StockId,// 本应该仅修改，既然是删除后重新插入，那么StockId还是沿用原有的 By Jerry Bai 2017/09/25
                FilialeId = 
                    string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue)
                        ? Guid.Empty
                        : new Guid(RCB_HostingFilialeAuth.SelectedValue),
                StorageType =
                    string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue)
                        ? default(int)
                        : int.Parse(RCB_StorageAuth.SelectedValue),
                ThirdCompanyID = new Guid(RCB_CompanyId.SelectedValue),
                WarehouseId = new Guid(strWarehouseId),
                TradeCode = HF_TradeCode.Value,
                LinkTradeCode = string.Empty,
                DateCreated = DateTime.Now,
                Transactor = CurrentSession.Personnel.Get().RealName,
                Description = tbDescription.Text.Trim(),
                AccountReceivable = -InDetailList.Sum(w => w.Quantity*w.UnitPrice),
                SubtotalQuantity = -InDetailList.Sum(w => w.Quantity),
                StockType = (int) StorageRecordType.LendOut,
                StockState = (int) StorageRecordState.WaitAudit,
                LinkTradeType = (int)StorageRecordLinkTradeType.Other,
                IsOut = true
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
                AccountReceivable = OutDetailList.Sum(w => w.Quantity*w.UnitPrice),
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
                    BatchNo = string.Empty,
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
                        RAM.Alert("录入借出单前异常！" + errorMessage);
                        return;
                    }
                    var oldBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(StockId);
                    if (oldBorrowLendInfo != null)
                    {
                        var result = _borrowLendDao.DeleteBorrowLendAndDetailList(oldBorrowLendInfo.BorrowLendId,
                            out errorMessage);
                        if (result <= 0)
                        {
                            RAM.Alert("录入借出返还单前异常！" + errorMessage);
                            return;
                        }

                        //删除借入返还单
                        isSuccess = _storageRecordDao.DeleteStorageRecord(oldBorrowLendInfo.BorrowLendId, out errorMessage);
                        if (!isSuccess)
                        {
                            RAM.Alert("录入借出返还单前异常！" + errorMessage);
                            return;
                        }
                    }
                    #endregion
                }
                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var personnelInfo = CurrentSession.Personnel.Get();
                string tbDes = !string.IsNullOrWhiteSpace(tbDescription.Text) ? tbDescription.Text.Trim() : "无";
                var description = string.Format("[借出单,(申请备注:{0}),申请人:{1};{2}]", tbDes, personnelInfo.RealName, dateTime);
                inStorageRecordInfo.Description = description;
                isSuccess = _storageManager.NewAddStorageRecordAndDetailList(inStorageRecordInfo, inGoodsStockList,
                    out errorMessage);
                if (!isSuccess)
                {
                    RAM.Alert("录入借出单异常！" + errorMessage);
                    return;
                }
                isSuccess = _borrowLendDao.AddBorrowLendAndDetailList(borrowLendInfo, borrowLendDetailList,
                    out errorMessage);
                if (!isSuccess)
                {
                    RAM.Alert("录入借出返还单异常！" + errorMessage);
                    return;
                }

                //在出入库记录中添加借入返还单
                var description1 = string.Format("[借出单申请成功生成借出返还单,申请人:{0};{1}]", personnelInfo.RealName, dateTime);
                //var addStockInfo = _storageRecordDao.GetStorageRecord(inStorageRecordInfo.StockId);
                //var addBorrowLendInfo = _borrowLendDao.GetBorrowLendInfo(inStorageRecordInfo.StockId);
                //var addBorrowLendDetailList = _borrowLendDao.GetBorrowLendDetailList(addBorrowLendInfo.BorrowLendId);
                //var outStorageDetails = _storageRecordDao.GetStorageRecordDetailListByStockId();
                var addstorageRecordInfo = new StorageRecordInfo
                {
                    StockId = borrowLendInfo.BorrowLendId,
                    FilialeId = inStorageRecordInfo.FilialeId,
                    ThirdCompanyID = inStorageRecordInfo.ThirdCompanyID,
                    WarehouseId = inStorageRecordInfo.WarehouseId,
                    TradeCode = _codeManager.GetCode(CodeType.LI),
                    LinkTradeCode = inStorageRecordInfo.TradeCode,
                    DateCreated = DateTime.Now,
                    Transactor = CurrentSession.Personnel.Get().RealName,
                    Description = description1,
                    AccountReceivable = -borrowLendDetailList.Sum(w => w.Quantity*w.UnitPrice),
                    SubtotalQuantity = -borrowLendDetailList.Sum(w => w.Quantity),
                    StockType = (int) StorageRecordType.LendIn,
                    StockState = (int) StorageRecordState.WaitAudit,
                    LinkTradeID = inStorageRecordInfo.StockId,
                    StorageType = inStorageRecordInfo.StorageType,
                    LinkTradeType = (int) StorageRecordLinkTradeType.Other,
                    IsOut = true
                };
                IList<StorageRecordDetailInfo> storageRecordDetailList =
                    OutDetailList.Select(item => new StorageRecordDetailInfo
                    {
                        StockId = addstorageRecordInfo.StockId,
                        GoodsId = item.GoodsId,
                        RealGoodsId = item.RealGoodsId,
                        GoodsName = item.GoodsName,
                        GoodsCode = item.GoodsCode,
                        Specification = item.Specification,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Description = item.Description,
                        ShelfType = item.ShelfType,
                        Units = item.Units
                    }).ToList();
                isSuccess = _storageManager.NewAddStorageRecordAndDetailList(addstorageRecordInfo,
                    storageRecordDetailList, out errorMessage);
                if (!isSuccess)
                {
                    RAM.Alert("录入借入返还单异常！" + errorMessage);
                    return;
                }
                ts.Complete();
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>获得借出单数据列表
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
                    Specification =
                        string.IsNullOrEmpty(dataItem["Specification"].Text)
                            ? string.Empty
                            : dataItem["Specification"].Text.Replace("&nbsp;", ""),
                    Units = dataItem["Units"].Text,
                    UnitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString()),
                    Quantity = string.IsNullOrEmpty(tbQuantity.Text) ? 0 : int.Parse(tbQuantity.Text),
                    Description = string.Empty,
                    ShelfType = Convert.ToByte(dataItem.GetDataKeyValue("ShelfType"))
                };
                var goodsStockInfo = goodsStockList.FirstOrDefault(w => w.RealGoodsId == info.RealGoodsId && w.ShelfType == info.ShelfType && w.UnitPrice == info.UnitPrice);
                if (goodsStockInfo==null)
                    goodsStockList.Add(info);
                else
                {
                    goodsStockInfo.Quantity += info.Quantity;
                }
            }
            InDetailList = goodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification).ToList();
        }

        /// <summary>获得借出返还单数据列表
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
                    UnitPrice = decimal.Parse(dataItem.GetDataKeyValue("UnitPrice").ToString()),
                    ShelfType = Convert.ToByte(dataItem.GetDataKeyValue("ShelfType"))
                };

                if (rcbSpecification.Visible)
                {
                    info.RealGoodsId = new Guid(rcbSpecification.SelectedValue);
                }

                var goodsStockInfo =goodsStockList.FirstOrDefault(w => w.RealGoodsId == info.RealGoodsId && w.UnitPrice == info.UnitPrice && w.ShelfType==info.ShelfType);
                if (goodsStockInfo==null)
                {
                    var inInfo = InDetailList.FirstOrDefault(w => w.GoodsId == info.GoodsId);
                    if (inInfo != null)
                        info.GoodsName = inInfo.GoodsName;
                    goodsStockList.Add(info);
                }
                else
                {
                    goodsStockInfo.Quantity += info.Quantity;
                }
            }
            OutDetailList = goodsStockList.OrderBy(ent=>ent.GoodsName).ThenBy(ent=>ent.Specification).ToList();
        }

        #region 下拉框绑定

        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId);
            WarehouseAuths = wList;
            RCB_Warehouse.DataSource = wList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }

        public List<WarehouseAuth> WarehouseAuths
        {
            get
            {
                if(ViewState["WarehouseAuths"]==null)return new List<WarehouseAuth>();
                return (List<WarehouseAuth>) ViewState["WarehouseAuths"];
            }
            set { ViewState["WarehouseAuths"] = value; }
        }

        public List<Int32> BindGoodsTypes
        {
            get
            {
                if (ViewState["BindGoodsTypes"] == null) return new List<Int32>();
                return (List<Int32>)ViewState["BindGoodsTypes"];
            }
            set { ViewState["BindGoodsTypes"] = value; }
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
        /// 借出单明细
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
        /// 借出返还单明细
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

        private void BindData(Guid stockId)
        {
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            HF_TradeCode.Value = storageRecordinfo.TradeCode;
            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_CompanyId.SelectedValue = storageRecordinfo.ThirdCompanyID.ToString();
            RCB_Warehouse.SelectedValue = storageRecordinfo.WarehouseId.ToString();
            RcbInStockChanged();
            RCB_StorageAuth.SelectedValue = storageRecordinfo.StorageType.ToString();
            RcbStorageAuthChanged(false);
            RCB_HostingFilialeAuth.SelectedValue = storageRecordinfo.FilialeId.ToString();
            var list = _storageManager.GetStorageRecordDetailListByStockId(StockId);
            var units = _goodsCenterSao.GetGoodsListByGoodsIds(list.Select(ent => ent.GoodsId).Distinct().ToList());
            foreach (var item in list)
            {
                var unit = units.FirstOrDefault(ent => ent.GoodsId == item.GoodsId);
                item.Units = unit != null ? unit.Units : "";
            }
            InDetailList = list;

            var dics = WMSEnum.Attribute.DescriptionAttribute.GetDict<WMSEnum.ShelfType>();
            ShelfTypeDic = dics.Where(ent=>ent.Key== (Byte)WMSEnum.ShelfType.Good || ent.Key == (Byte)WMSEnum.ShelfType.Inferior || ent.Key == (Byte)WMSEnum.ShelfType.Bad).ToDictionary(k=>k.Key,v=>v.Value);

            var borrowLendInfo = _borrowLendDao.GetBorrowLendInfo(StockId);
            if (borrowLendInfo != null)
            {
                OutDetailList=_storageRecordDao.GetStorageRecordDetailListByStockId(borrowLendInfo.BorrowLendId);
                //OutDetailList = list.Select(act => new StorageRecordDetailInfo
                //{
                //    Description = act.Description,
                //    GoodsCode = act.GoodsCode,
                //    GoodsId = act.GoodsId,
                //    GoodsName = act.GoodsName,
                //    Quantity = act.Quantity,
                //    RealGoodsId = act.RealGoodsId,
                //    Specification = act.Specification,
                //    UnitPrice = act.UnitPrice,
                //    ShelfType =act.ShelfType
                //}).ToList();

                //var detailUnits = _goodsCenterSao.GetGoodsListByGoodsIds(OutDetailList.Select(ent => ent.GoodsId).Distinct().ToList());
                //foreach (var detailunit in detailUnits)
                //{
                //    foreach (var info in OutDetailList.Where(p => p.GoodsId == detailunit.GoodsId))
                //    {
                //        info.Units = detailunit.Units;
                //    }
                //}
            }

        }

        /// <summary>
        /// 更改入库仓储时重新查询库存数
        /// </summary>
        private void BindStockSearch()
        {
            GetRgGoodsData();
            GetRgGoodsBackData();
            if (InDetailList.Count > 0 && RG_Goods.Visible)
            {
                RG_Goods.Rebind();
            }
            else if (OutDetailList.Count > 0 && RG_GoodsBack.Visible)
            {
                RG_GoodsBack.Rebind();
            }
        }

        public string ShowShelfType(object shelfType)
        {
            var type = Convert.ToByte(shelfType);
            if (!ShelfTypeDic.ContainsKey(type)) return "-";
            return ShelfTypeDic[type];
        }

        protected void DdlShelfTypeSelectedChanged(object sender, EventArgs e)
        {
            var ddlShelfType = (DropDownList)sender;
            var gridDataItem = (GridDataItem) ddlShelfType.Parent.Parent;
            var realGoodsId = new Guid(gridDataItem.GetDataKeyValue("RealGoodsId").ToString());
            var shelfType = Convert.ToByte(gridDataItem.GetDataKeyValue("ShelfType"));
            var unitPrice = Convert.ToDecimal(gridDataItem.GetDataKeyValue("UnitPrice"));
            List<StorageRecordDetailInfo> details=new List<StorageRecordDetailInfo>();
            foreach (var detail in InDetailList)
            {
                if (detail.RealGoodsId != realGoodsId)
                    details.Add(detail);
                else
                {
                    if (detail.ShelfType == shelfType && detail.UnitPrice == unitPrice)
                    {
                        detail.ShelfType = Convert.ToByte(ddlShelfType.SelectedValue);
                    }
                    var exists = details.FirstOrDefault(ent => ent.RealGoodsId == realGoodsId && ent.ShelfType == Convert.ToByte(ddlShelfType.SelectedValue) && ent.UnitPrice == unitPrice);
                    if (exists != null)
                        exists.Quantity += detail.Quantity;
                    else
                        details.Add(detail);
                }
            }
            InDetailList = details;
            RG_Goods.Rebind();
        }

        protected void RgGoodsItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var ddlShelfType = (DropDownList)e.Item.FindControl("DdlShelfType");
                ddlShelfType.Items.Clear();
                ddlShelfType.Text = "";
                ddlShelfType.DataValueField = "Value";
                ddlShelfType.DataTextField = "Key";
                var storageType = RCB_StorageAuth.SelectedItem != null &&
                                  !string.IsNullOrEmpty(RCB_StorageAuth.SelectedItem.Value)
                    ? Convert.ToByte(RCB_StorageAuth.SelectedItem.Value)
                    : 0;
                if (storageType == (Byte)WMSEnum.StorageType.S)
                {
                    ddlShelfType.Items.Add(new ListItem("--请选择--", "1"));
                    var shelfTypeValue = Convert.ToByte(((GridDataItem)e.Item).GetDataKeyValue("ShelfType"));
                    foreach (var shelfTypeItem in ShelfTypeDic)
                    {
                        ddlShelfType.Items.Add(new ListItem(shelfTypeItem.Value, string.Format("{0}", shelfTypeItem.Key)));
                    }
                    ddlShelfType.SelectedValue = string.Format("{0}", shelfTypeValue==0?(Byte)WMSEnum.ShelfType.Normal: shelfTypeValue);
                }  
            }
        }
    }
}