using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.SAL.WMS;

namespace ERP.UI.Web.Windows
{
    /// <summary>新建采购单
    /// </summary>
    public partial class PurchingForm : WindowsPage
    {
        //其他公司
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        GoodsClassManager _goodsClassManager;

        #region -- 初始化
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            RCB_Goods.ItemsRequested += (Rcb_Goods_ItemsRequested);
        }
        #endregion

        SubmitController _submitController;
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 180);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!Page.IsPostBack)
            {
                _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
                RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
                RCB_GoodsClass.DataBind();
                RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));

                var personnelInfo = CurrentSession.Personnel.Get();
                RCB_Warehouse.DataSource = GetWarehouseList(personnelInfo);
                RCB_Warehouse.DataBind();

                LB_Director.Text = personnelInfo.RealName;

                RCB_Filiale.DataSource = new List<HostingFilialeAuth> { new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "物流配送公司列表" } };
                RCB_Filiale.DataBind();
            }
        }

        private Dictionary<Guid,WarehouseFilialeAuth> WarehouseAuth
        {
            get
            {
                if (ViewState["WarehouseAuth"] == null)
                {
                    return new Dictionary<Guid, WarehouseFilialeAuth>();
                }
                return (Dictionary<Guid, WarehouseFilialeAuth>)ViewState["WarehouseAuth"];
            }
            set { ViewState["WarehouseAuth"] = value; }
        }

        private IList<PurchasingDetailInfo> PurchasingDetailList
        {
            get
            {
                if (ViewState["PurchasingDetailInfo"] == null)
                {
                    return new List<PurchasingDetailInfo>();
                }
                return (List<PurchasingDetailInfo>)ViewState["PurchasingDetailInfo"];
            }
            set { ViewState["PurchasingDetailInfo"] = value; }
        }

        public List<SearchGoodsInfo> GoodsList
        {
            get
            {
                if (Session["GoodsList"] == null)
                {
                    return new List<SearchGoodsInfo>();
                }
                return (List<SearchGoodsInfo>)Session["GoodsList"];
            }
            set
            {
                Session["GoodsList"] = value;
            }
        } 

        /// <summary> 获取(采购部)所有员工的信息
        /// </summary>
        /// <returns></returns>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }

        #region -- 绑定要采购明细商品
        /// <summary>绑定要采购明细商品
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Rgd_PurchasingDetail.DataSource = PurchasingDetailList.OrderBy(w => w.GoodsName).ThenBy(ent => ent.Specification).ToList();
        }
        #endregion

        #region  [搜索商品]
        //初始化产品选择表
        //private void InitializeGoodsTable()
        //{
        //    DataTable dataTable = GoodsTable;
        //    dataTable.Clear();
        //    if (dataTable.Columns.Count > 3)
        //    {
        //        for (int cols = dataTable.Columns.Count; cols > 3; cols--)
        //            dataTable.Columns.RemoveAt(cols - 1);
        //    }
        //    GoodsTable = dataTable;
        //}

        //选择分类
        /// <summary>搜索商品
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
                if (tempList.All(act=>act.GoodsId!=goodsInfo.GoodsId))
                {
                    tempList.Add(new SearchGoodsInfo
                    {
                        Astigmias = atigmiaDic,
                        Axialss = axialsDic,
                        GoodsId = goodsInfo.GoodsId,
                        GoodsCode = goodsInfo.GoodsCode,
                        GoodsName = goodsInfo.GoodsName,
                        Luminositys= luminosityDic
                    });
                }
            }
            GoodsList = tempList;
            RgGoods.Rebind();
        }

        private void Rcb_Goods_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
         {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(e.Text);
                Int32 totalCount = dic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var keyValuePair in dic)
                    {
                        var item = new RadComboBoxItem { Text = keyValuePair.Value, Value = keyValuePair.Key };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>选择具体商品
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
                        childFieldList.AddRange(fieldInfo.ChildFields.OrderBy(act=>act.OrderIndex).ToList());
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
                            Axialss= axialsDic,
                            GoodsId = goodsBaseInfo.GoodsId,
                            GoodsCode = goodsBaseInfo.GoodsCode,
                            GoodsName = goodsBaseInfo.GoodsName,
                            Luminositys= luminosityDic
                        });
                        GoodsList = tempList;
                    }
                }
            }
            RgGoods.Rebind();
        }
        #endregion

        #region [添加商品]
        /// <summary>添加商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
            if (warehouseId == Guid.Empty)
            {
                RAM.Alert("请先选择“仓库”");
                return;
            }

            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);

            if (RgGoods.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
                return;
            }
            if (string.IsNullOrEmpty(RCB_Filiale.SelectedValue) || RCB_Filiale.SelectedValue==Guid.Empty.ToString())
            {
                RAM.Alert("请选择物流配送公司!");
                return;
            }
            var goodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in RgGoods.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (goodsIdList.Count(w => w == goodsId) == 0)
                    goodsIdList.Add(goodsId);
            }
            var hostingFilialeId = new Guid(RCB_Filiale.SelectedValue);
            var purchaseGoodsTypes=WMSSao.GetPurchaseGoodsTypes(warehouseId, hostingFilialeId);
            IPurchaseSet purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
            var purchaseSetList = purchaseSet.GetPurchaseSetInfoList(goodsIdList, warehouseId,hostingFilialeId).ToDictionary(k=>k.GoodsId,v=>v);
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList);
            IList<PurchasingDetailInfo> detailList = (from GridDataItem dataItem in Rgd_PurchasingDetail.Items
                                                                                             let goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString())
                                                                                             let planQuantity = ((TextBox)dataItem.FindControl("tbx_quantity")).Text
                                                                                             let goodsName = dataItem.GetDataKeyValue("GoodsName").ToString()
                                                                                             let goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString()
                                                                                             let specification = dataItem.GetDataKeyValue("Specification").ToString()
                                                                                             let price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString())
                                                                                             let units = dataItem.GetDataKeyValue("Units").ToString()
                                                                                             let purchasingGoodsType = Convert.ToInt32(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString())
                                                                                             select new PurchasingDetailInfo(Guid.NewGuid(), goodsId, goodsName, units, goodsCode, specification, 
                                                                                             Guid.Empty,  price, Convert.ToDouble(planQuantity), 0, 0, "", 
                                                                                             Guid.NewGuid(), purchasingGoodsType)
                                                                                             {
                                                                                                 SixtyDaySales = Convert.ToInt32(dataItem["SixtyDaySales"].Text),
                                                                                                 ThirtyDaySales = Convert.ToInt32(dataItem["ThirtyDaySales"].Text),
                                                                                                 ElevenDaySales = Convert.ToInt32(dataItem["ElevenDaySales"].Text),
                                                                                                 CPrice = price
                                                                                             }).ToList(); ;
            IList<CompanyCussentInfo> companyList = Rcb_CommanyDataSource();
            foreach (GridDataItem dataItem in RgGoods.SelectedItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                if (!purchaseGoodsTypes.Contains(goodsInfo.GoodsType))
                {
                    RAM.Alert(string.Format("商品{0}不在公司的代发范围内！", goodsInfo.GoodsName));
                    return;
                }
                if (!purchaseSetList.ContainsKey(selectedGoodsId))
                {
                    RAM.Alert("“" + goodsInfo.GoodsName + "”未添加商品采购设置");
                    return;
                }
                if (purchaseSetList[selectedGoodsId].PurchasePrice<=0)
                {
                    RAM.Alert(string.Format("“{0} ”采购价为：【{1}】",goodsInfo.GoodsName, purchaseSetList[selectedGoodsId].PurchasePrice));
                    return;
                }
                var purchaseSetInfo = purchaseSetList[selectedGoodsId];
                var companyInfo = companyList.FirstOrDefault(w => w.CompanyId == purchaseSetInfo.CompanyId) ?? new CompanyCussentInfo();
                if (companyInfo.CompanyId == Guid.Empty)
                {
                    RAM.Alert("“" + goodsInfo.GoodsName + "”的往来单位被“搁置”");
                    return;
                }
                
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsInfo.GoodsId);
                if(current==null)continue;
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
                        if(i==5 && current.Luminositys.Count==0)break;
                        if (i == 6 && current.Astigmias.Count == 0) break;
                        if (i == 7&& current.Axialss.Count == 0) break;
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
                                        dicSelectedField.Add(fieldId, fieldInfoList.Where(act=>act.FieldId!=fieldId).Select(w => w.FieldId).ToList());
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
                    decimal price = purchaseSetInfo.PurchasePrice <= 0 ? 0 : purchaseSetInfo.PurchasePrice;
                    var detailInfo = new PurchasingDetailInfo(Guid.NewGuid(), childGoodsInfo.RealGoodsId, goodsInfo.GoodsName, goodsInfo.Units, goodsInfo.GoodsCode, childGoodsInfo.Specification, Guid.Empty, price, 
                        1, 0, (int)YesOrNo.No, "", Guid.NewGuid(), Cbx_GoodsType.Checked ? (int)PurchasingGoodsType.Gift : (int)PurchasingGoodsType.NoGift);
                    // 获取商品的60、30、11天销量
                    //TODO   确定该页面是否使用，新建采购单必须选择采购公司 
                    var purchasingDetailInfo = _purchasingDetail.GetChildGoodsSale(childGoodsInfo.RealGoodsId, warehouseId, DateTime.Now, hostingFilialeId);
                    if (purchasingDetailInfo != null)
                    {
                        detailInfo.SixtyDaySales = purchasingDetailInfo.SixtyDaySales;
                        detailInfo.ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales;
                        detailInfo.ElevenDaySales = purchasingDetailInfo.ElevenDaySales / 11;//日均销量(11天)
                    }
                    else
                    {
                        detailInfo.SixtyDaySales = 0;
                        detailInfo.ThirtyDaySales = 0;
                        detailInfo.ElevenDaySales = 0;
                    }
                    //保留输入记录的采购数量
                    foreach (GridDataItem item in Rgd_PurchasingDetail.Items)
                    {
                        var goodsId = (Guid)item.GetDataKeyValue("GoodsID");
                        if (goodsId == childGoodsInfo.RealGoodsId)
                        {
                            detailInfo.PlanQuantity = double.Parse(((TextBox)item.FindControl("tbx_quantity")).Text);
                            break;
                        }
                    }
                    if (PurchasingDetailList.Count >= 1)
                    {
                        ChildGoodsInfo info = childGoodsInfo;
                        if (PurchasingDetailList.Where(p => p.GoodsID == info.RealGoodsId)
                            .Count(p => p.PurchasingGoodsType == (Cbx_GoodsType.Checked ? (int)PurchasingGoodsType.Gift : (int)PurchasingGoodsType.NoGift)) == 0)
                        {
                            detailList.Add(detailInfo);
                        }
                        else
                        {
                            detailList.First(d => d.GoodsID == childGoodsInfo.RealGoodsId).PlanQuantity = detailInfo.PlanQuantity;
                        }
                    }
                    else
                    {
                        detailList.Add(detailInfo);
                    }
                }
            }
            PurchasingDetailList = detailList;
            Rgd_PurchasingDetail.Rebind();
        }
        #endregion

        /// <summary>仓库列表
        /// </summary>
        /// <returns></returns>
        protected List<WarehouseFilialeAuth> GetWarehouseList(PersonnelInfo personnelInfo)
        {
            var wList = WMSSao.GetWarehouseAndFilialeAuth(personnelInfo.PersonnelId);
            WarehouseAuth = wList.ToDictionary(k=>k.WarehouseId,v=>v);
            if (wList.Count(p => p.WarehouseId == Guid.Empty) == 0)
            {
                var winfo = new WarehouseFilialeAuth { WarehouseId = Guid.Empty, WarehouseName = "所有仓库", FilialeAuths = new List<HostingFilialeAuth>() };
                wList.Insert(0, winfo);
            }
            return wList;
        }

        #region 获取供应商集合
        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> Rcb_CommanyDataSource()
        {
            return _companyCussent.GetCompanyCussentList(CompanyType.Suppliers);
        }
        #endregion

        /// <summary>生成采购单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_InsterStock(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                return;
            }

            #region [验证]
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || RCB_Warehouse.SelectedValue == string.Format("{0}", Guid.Empty))
            {
                RAM.Alert("仓库必须选择!");
                return;
            }
            if (string.IsNullOrEmpty(RCB_Filiale.SelectedValue) || RCB_Filiale.SelectedValue == string.Format("{0}", Guid.Empty))
            {
                RAM.Alert("采购公司必须选择!");
                return;
            }
            if (string.IsNullOrEmpty(RDP_ArrivalTime.DateInput.Text) || RDP_ArrivalTime.SelectedDate == null)
            {
                RAM.Alert("到货时间必须选择!");
                return;
            }
            if (Rgd_PurchasingDetail.Items.Count <= 0)
            {
                RAM.Alert("请添加商品!");
                return;
            }
            #endregion

            IList<CompanyCussentInfo> companyList = Rcb_CommanyDataSource();

            var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
            var filialeId = new Guid(RCB_Filiale.SelectedValue);
            var pmid = Guid.Empty;
            string pmName = string.Empty;

            var goodsIdOrRealGoodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in Rgd_PurchasingDetail.Items)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                if (goodsIdOrRealGoodsIdList.Count(w => w == goodsId) == 0)
                    goodsIdOrRealGoodsIdList.Add(goodsId);
            }
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
            if (dicGoods == null) return;
            IList<PurchaseSetInfo> purchaseSetList = new List<PurchaseSetInfo>();
            if (dicGoods.Count > 0)
            {
                var goodsIdList = dicGoods.Select(keyValuePair => keyValuePair.Value.GoodsId).ToList();
                IPurchaseSet purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
                purchaseSetList = purchaseSet.GetPurchaseSetInfoList(goodsIdList, warehouseId, filialeId);
            }
            var purchasingDic = new Dictionary<PurchasingInfo, IList<PurchasingDetailInfo>>();
            IPurchasePromotion purchasePromotionDal = new PurchasePromotion(GlobalConfig.DB.FromType.Read);
            var codeManager = new CodeManager();
            foreach (GridDataItem dataItem in Rgd_PurchasingDetail.Items)
            {
                IList<PurchasingDetailInfo> plist = new List<PurchasingDetailInfo>();
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var tbQuantity = (TextBox)dataItem.FindControl("tbx_quantity");
                string planQuantity = tbQuantity.Text; //采购数量
                string goodsName = dataItem.GetDataKeyValue("GoodsName").ToString();
                string goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString();
                string specification = dataItem.GetDataKeyValue("Specification").ToString();
                decimal price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString());
                string units = dataItem.GetDataKeyValue("Units").ToString();
                int purchasingGoodsType = Convert.ToInt32(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString());

                var goodsInfo = new GoodsInfo();
                if (dicGoods.Count > 0)
                {
                    bool hasKey = dicGoods.ContainsKey(goodsId);
                    if (hasKey)
                    {
                        goodsInfo = dicGoods.FirstOrDefault(w => w.Key == goodsId).Value;
                    }
                }
                if (goodsInfo.GoodsId == Guid.Empty) continue;
                PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsInfo.GoodsId) ?? new PurchaseSetInfo();

                var purchId = Guid.NewGuid();
                //采购单明细
                var dInfo = new PurchasingDetailInfo(purchId, goodsId, goodsName, units, goodsCode, specification, purchaseSetInfo.CompanyId,
                                                     purchasingGoodsType == (int)PurchasingGoodsType.Gift ? 0 : price,
                                                     Convert.ToDouble(planQuantity), 0, 0, "", Guid.NewGuid(), purchasingGoodsType)
                                {
                                    SixtyDaySales = Convert.ToInt32(dataItem["SixtyDaySales"].Text),
                                    ThirtyDaySales = Convert.ToInt32(dataItem["ThirtyDaySales"].Text),
                                    ElevenDaySales = Convert.ToInt32(dataItem["ElevenDaySales"].Text),
                                    CPrice = purchaseSetInfo.PurchasePrice
                                };
                var companyInfo = companyList.FirstOrDefault(w => w.CompanyId == purchaseSetInfo.CompanyId) ?? new CompanyCussentInfo();

                //采购单
                var purchasingInfo = new PurchasingInfo(purchId, "", companyInfo.CompanyId, companyInfo.CompanyName,
                                                filialeId, warehouseId, (int)PurchasingState.NoSubmit,
                                               (int)PurchasingType.Custom, DateTime.Now, DateTime.MaxValue,
                                               string.Format("[采购类别:{0};采购人:{1}]", EnumAttribute.GetKeyName(PurchasingType.Custom), CurrentSession.Personnel.Get().RealName), pmid, pmName)
                                {
                                    Director = purchaseSetInfo.PersonResponsibleName,
                                    PersonResponsible = purchaseSetInfo.PersonResponsible,
                                    ArrivalTime = RDP_ArrivalTime.SelectedDate.Value,
                                    PurchasingFilialeId = filialeId,
                                    PurchasingPersonName= CurrentSession.Personnel.Get().RealName
                };
                bool isHave = false;
                if (purchasingDic.Count > 0)
                {
                    foreach (KeyValuePair<PurchasingInfo, IList<PurchasingDetailInfo>> keyValue in purchasingDic)
                    {
                        if (keyValue.Key.PersonResponsible == purchasingInfo.PersonResponsible && keyValue.Key.CompanyID == purchasingInfo.CompanyID)
                        {
                            dInfo.PurchasingID = keyValue.Key.PurchasingID;
                            keyValue.Value.Add(dInfo);
                            isHave = true;
                            break;
                        }
                    }
                }
                if (isHave == false)
                {
                    purchasingInfo.PurchasingNo = codeManager.GetCode(CodeType.PH);
                    plist.Add(dInfo);
                    purchasingDic.Add(purchasingInfo, plist);
                }
            }

            foreach (KeyValuePair<PurchasingInfo, IList<PurchasingDetailInfo>> keyValue in purchasingDic)
            {
                var pInfo = keyValue.Key;
                IList<PurchasingDetailInfo> plist = keyValue.Value;
                //存在商品采购价大于绑定价格，采购单为调价待审核
                if (plist.Any(act => act.Price != 0 && act.Price > act.CPrice))
                {
                    pInfo.PurchasingState = (int)PurchasingState.WaitingAudit;
                }
                //判断是否选择采购公司
                if (pInfo.PurchasingFilialeId == Guid.Empty)
                {
                    pInfo.PurchasingFilialeId = filialeId;
                    //pInfo.IsOut = true;
                }
                _purchasing.PurchasingInsert(pInfo);
                //添加采购单添加操作记录添加
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pInfo.PurchasingID, pInfo.PurchasingNo,
                    OperationPoint.PurchasingManager.CreatePurchaseList.GetBusinessInfo(), string.Empty);


                IList<PurchasingDetailInfo> plist2 = plist.Where(w => w.PurchasingGoodsType != (int)PurchasingGoodsType.Gift).ToList();
                //如果赠送方式为总数量赠送时使用  key 主商品ID  value 额外赠送 
                var dics = new Dictionary<string, int>();   //需赠送
                var debitExtraDics = new Dictionary<string, int>(); //借计单已送
                #region [现返]
                //处理原理：
                //赠品数量=原采购数量/(买几个+送几个)*送几个
                //购买数量=原采购数量-赠品数量
                foreach (var pdInfo in plist2)
                {
                    var goodsBaseInfo = new GoodsInfo();
                    if (dicGoods != null)
                    {
                        bool hasKey = dicGoods.ContainsKey(pdInfo.GoodsID);
                        if (hasKey)
                        {
                            goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == pdInfo.GoodsID).Value;
                        }
                    }
                    PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsBaseInfo.GoodsId && w.WarehouseId == pInfo.WarehouseID);
                    if (purchaseSetInfo == null) continue;
                    IList<PurchasePromotionInfo> ppList = purchasePromotionDal.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId, pInfo.WarehouseID, purchaseSetInfo.HostingFilialeId,(int)PurchasePromotionType.Back);
                    PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                    if (ppInfo != null)
                    {
                        #region  新增
                        if (!ppInfo.IsSingle)  //按商品总数量进行赠送
                        {
                            if (!dics.ContainsKey(goodsBaseInfo.GoodsName))
                            {
                                var dataList = plist2.Where(act => act.GoodsName == goodsBaseInfo.GoodsName).OrderByDescending(act => act.PlanQuantity).ToList();
                                //余数
                                var extra = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) % (ppInfo.BuyCount + ppInfo.GivingCount);
                                if (extra == ppInfo.BuyCount)
                                {
                                    dataList[0].PlanQuantity += ppInfo.GivingCount;
                                }
                                else
                                {
                                    //应赠送次数
                                    int actquantity = extra % (ppInfo.BuyCount + ppInfo.GivingCount);
                                    if (actquantity >= ((ppInfo.BuyCount + ppInfo.GivingCount) / float.Parse("2.0")))
                                    {
                                        dataList[0].PlanQuantity = dataList[0].PlanQuantity + (ppInfo.BuyCount + ppInfo.GivingCount) - actquantity;
                                    }
                                }
                                //总商品赠送商品总数
                                var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) * ppInfo.GivingCount / (ppInfo.BuyCount + ppInfo.GivingCount);
                                dics.Add(goodsBaseInfo.GoodsName, sumTotal);
                                if (dics[goodsBaseInfo.GoodsName] > 0)
                                {
                                    foreach (var item in dataList.OrderByDescending(act => act.PlanQuantity))
                                    {
                                        var count = dics[item.GoodsName] - Convert.ToInt32(item.PlanQuantity);
                                        if (count > 0)  //赠品数>购买数
                                        {
                                            item.Price = 0;
                                            item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                            item.RealityQuantity = 0;
                                            dics[item.GoodsName] = count;
                                        }
                                        else if (count < 0)
                                        {
                                            plist.Add(new PurchasingDetailInfo
                                            {
                                                PurchasingGoodsID = Guid.NewGuid(),
                                                PurchasingID = item.PurchasingID,
                                                GoodsID = item.GoodsID,
                                                GoodsName = item.GoodsName,
                                                GoodsCode = item.GoodsCode,
                                                Specification = item.Specification,
                                                CompanyID = item.CompanyID,
                                                Price = item.Price,
                                                PlanQuantity = Math.Abs(count),
                                                PurchasingGoodsType = (int)PurchasingGoodsType.NoGift,
                                                RealityQuantity = 0,
                                                State = item.State,
                                                Description = "",
                                                Units = pdInfo.Units,
                                                SixtyDaySales = item.SixtyDaySales,
                                                ThirtyDaySales = item.ThirtyDaySales,
                                                ElevenDaySales = item.ElevenDaySales,
                                                CPrice = item.CPrice
                                            });
                                            item.Price = 0;
                                            item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                            item.RealityQuantity = 0;
                                            item.PlanQuantity = dics[item.GoodsName];
                                            item.CPrice = 0;
                                            dics[item.GoodsName] = 0;
                                        }
                                        else
                                        {
                                            item.Price = 0;
                                            item.PurchasingGoodsType = (int)PurchasingGoodsType.Gift;
                                            item.RealityQuantity = 0;
                                            item.CPrice = 0;
                                            dics[item.GoodsName] = 0;
                                        }
                                        if (dics[item.GoodsName] == 0) break;
                                    }
                                }
                            }
                        }
                        else  //按单光度
                        {
                            //应赠余数
                            var actquantity = pdInfo.PlanQuantity % (ppInfo.BuyCount + ppInfo.GivingCount);
                            if (actquantity > 0)
                            {
                                if (actquantity >= ((ppInfo.BuyCount + ppInfo.GivingCount) / float.Parse("2.0")))
                                {
                                    pdInfo.PlanQuantity = pdInfo.PlanQuantity + (ppInfo.BuyCount + ppInfo.GivingCount) - actquantity;
                                }
                            }
                            int pQuantity = int.Parse(pdInfo.PlanQuantity.ToString(CultureInfo.InvariantCulture));
                            //赠品数量=原采购数量/(买几个+送几个)*送几个
                            int quantity = pQuantity / (ppInfo.BuyCount + ppInfo.GivingCount) * ppInfo.GivingCount;
                            if (quantity > 0)
                            {
                                var oldPurchasingDetailInfo = plist.FirstOrDefault(w => w.PurchasingGoodsID == pdInfo.PurchasingGoodsID);
                                if (oldPurchasingDetailInfo != null)
                                {
                                    //购买数量=原采购数量-赠品数量
                                    oldPurchasingDetailInfo.PlanQuantity -= (quantity);
                                }

                                var purchasingDetailInfo = plist.FirstOrDefault(w => w.GoodsID == pdInfo.GoodsID && w.PurchasingGoodsType == (int)PurchasingGoodsType.Gift);
                                if (purchasingDetailInfo != null)
                                {
                                    //在原赠品数量累加
                                    purchasingDetailInfo.PlanQuantity += (quantity);
                                }
                                else
                                {
                                    purchasingDetailInfo = new PurchasingDetailInfo
                                    {
                                        PurchasingGoodsID = Guid.NewGuid(),
                                        PurchasingID = pInfo.PurchasingID,
                                        GoodsID = pdInfo.GoodsID,
                                        GoodsName = pdInfo.GoodsName,
                                        GoodsCode = pdInfo.GoodsCode,
                                        Specification = pdInfo.Specification,
                                        CompanyID = pdInfo.CompanyID,
                                        Price = 0,
                                        PlanQuantity = (quantity),
                                        PurchasingGoodsType = (int)PurchasingGoodsType.Gift,
                                        RealityQuantity = 0,
                                        State = 0,
                                        Description = "",
                                        Units = pdInfo.Units,
                                        SixtyDaySales = pdInfo.SixtyDaySales,
                                        ThirtyDaySales = pdInfo.ThirtyDaySales,
                                        ElevenDaySales = pdInfo.ElevenDaySales
                                    };
                                    plist.Add(purchasingDetailInfo);
                                }
                            }
                        }
                        #endregion
                    }
                }

                #endregion

                #region [非现返生成借记单]
                var debitNoteDetailList = new List<DebitNoteDetailInfo>();
                foreach (var pdInfo in plist2)
                {
                    var goodsBaseInfo = new GoodsInfo();
                    if (dicGoods != null)
                    {
                        bool hasKey = dicGoods.ContainsKey(pdInfo.GoodsID);
                        if (hasKey)
                        {
                            goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == pdInfo.GoodsID).Value;
                        }
                    }
                    PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsBaseInfo.GoodsId && w.WarehouseId == pInfo.WarehouseID);
                    if (purchaseSetInfo != null)
                    {
                        IList<PurchasePromotionInfo> ppList = purchasePromotionDal.GetPurchasePromotionList(purchaseSetInfo.PromotionId, purchaseSetInfo.GoodsId, pInfo.WarehouseID, purchaseSetInfo.HostingFilialeId,(int)PurchasePromotionType.NoBack);
                        PurchasePromotionInfo ppInfo = ppList.FirstOrDefault(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now);
                        if (ppInfo != null)
                        {
                            int pQuantity = int.Parse(pdInfo.PlanQuantity.ToString(CultureInfo.InvariantCulture));
                            //赠品数量=原采购数量/买几个*送几个
                            int quantity = pQuantity / ppInfo.BuyCount * ppInfo.GivingCount;
                            #region  新增
                            //按商品总数量进行赠送
                            if (!ppInfo.IsSingle && dicGoods != null)
                            {
                                if (!dics.ContainsKey(goodsBaseInfo.GoodsName))
                                {
                                    var dataList = plist2.Where(act => act.GoodsName == goodsBaseInfo.GoodsName).ToList();
                                    //单光度赠送商品总数
                                    var total = dataList.Sum(act => (Convert.ToInt32(act.PlanQuantity) / (ppInfo.BuyCount + ppInfo.GivingCount)));
                                    //总商品赠送商品总数
                                    var sumTotal = dataList.Sum(act => Convert.ToInt32(act.PlanQuantity)) / (ppInfo.BuyCount + ppInfo.GivingCount);
                                    if (sumTotal > total)
                                        dics.Add(goodsBaseInfo.GoodsName, sumTotal);
                                }
                            }
                            #endregion
                            if (quantity > 0)
                            {
                                var debitNoteDetailInfo = new DebitNoteDetailInfo
                                {
                                    PurchasingId = pInfo.PurchasingID,
                                    GoodsId = pdInfo.GoodsID,
                                    GoodsName = pdInfo.GoodsName,
                                    Specification = pdInfo.Specification,
                                    GivingCount = quantity,
                                    ArrivalCount = 0,
                                    Price = pdInfo.Price,
                                    State = 0,
                                    Amount = quantity * pdInfo.Price,
                                    Memo = "",
                                    Id = Guid.NewGuid()
                                };
                                if (debitExtraDics.ContainsKey(goodsBaseInfo.GoodsName))
                                {
                                    debitExtraDics[goodsBaseInfo.GoodsName] = debitExtraDics[goodsBaseInfo.GoodsName] + quantity;
                                }
                                else
                                {
                                    debitExtraDics.Add(goodsBaseInfo.GoodsName, quantity);
                                }
                                debitNoteDetailList.Add(debitNoteDetailInfo);
                            }
                        }
                    }
                }

                #endregion

                #region 处理额外赠送商品
                foreach (var dic in dics)
                {
                    KeyValuePair<string, int> dic1 = dic;
                    if (debitExtraDics.ContainsKey(dic.Key))
                    {
                        var total = dic1.Value - debitExtraDics[dic.Key];
                        if (total > 0)
                        {
                            var data = debitNoteDetailList.Where(act => act.GoodsName == dic1.Key).OrderByDescending(act => act.GivingCount).ToList();
                            for (int i = 0; i < total; i++)
                            {
                                data[i].GivingCount += 1;
                                data[i].Amount = data[i].Price * data[i].GivingCount;
                            }
                        }
                    }
                }
                #endregion

                var purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasing);
                //保存采购单明细
                purchasingDetailManager.Save(plist);
                //添加借记单
                if (debitNoteDetailList.Count > 0)
                {
                    var debitNote = new DebitNote(GlobalConfig.DB.FromType.Write);
                    var debitNoteInfo = new DebitNoteInfo
                    {
                        PurchasingId = pInfo.PurchasingID,
                        PurchasingNo = pInfo.PurchasingNo,
                        CompanyId = pInfo.CompanyID,
                        PresentAmount = debitNoteDetailList.Sum(w => w.Amount),
                        CreateDate = DateTime.Now,
                        FinishDate = DateTime.MinValue,
                        State = (int)DebitNoteState.ToPurchase,
                        WarehouseId = pInfo.WarehouseID,
                        Memo = "",
                        PersonResponsible = pInfo.PersonResponsible,
                        NewPurchasingId = Guid.Empty
                    };
                    debitNote.AddPurchaseSetAndDetail(debitNoteInfo, debitNoteDetailList);
                }
            }

            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            _submitController.Submit();
        }

        /// <summary>删除商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_Delete(object sender, EventArgs e)
        {
            if (Rgd_PurchasingDetail.Items.Count > 0)
            {
                if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
                {
                    RAM.Alert("没有选择要删除的商品!");
                    return;
                }
                PurchasingDetailList = (from GridDataItem dataItem in Rgd_PurchasingDetail.Items where !dataItem.Selected
                                                          let goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString())
                                                          let planQuantity = ((TextBox)dataItem.FindControl("tbx_quantity")).Text
                                                          let goodsName = dataItem.GetDataKeyValue("GoodsName").ToString()
                                                          let goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString()
                                                          let specification = dataItem.GetDataKeyValue("Specification").ToString()
                                                          let price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString())
                                                          let units = dataItem.GetDataKeyValue("Units").ToString()
                                                          let purchasingGoodsType = Convert.ToInt32(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString())
                                                          select new PurchasingDetailInfo(Guid.NewGuid(), goodsId, goodsName, units, goodsCode, specification,
                                                          Guid.Empty, price, Convert.ToDouble(planQuantity), 0, 0, "",
                                                          Guid.NewGuid(), purchasingGoodsType)
                                                          {
                                                              SixtyDaySales = Convert.ToInt32(dataItem["SixtyDaySales"].Text),
                                                              ThirtyDaySales = Convert.ToInt32(dataItem["ThirtyDaySales"].Text),
                                                              ElevenDaySales = Convert.ToInt32(dataItem["ElevenDaySales"].Text),
                                                              CPrice = price
                                                          }).ToList(); ;
                
                Rgd_PurchasingDetail.Rebind();
            }
            
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

        protected void RcbWarehouseSelectedChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox)o;
            if (cb != null)
            {
                var warehouseId = new Guid(cb.SelectedItem.Value);
                var filiales = new List<HostingFilialeAuth> { new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "采购公司列表" } };
                if (WarehouseAuth.ContainsKey(warehouseId))
                {
                    filiales.AddRange(WarehouseAuth[warehouseId].FilialeAuths);
                }
                RCB_Filiale.DataSource = filiales;
                RCB_Filiale.DataBind();

                if (Rgd_PurchasingDetail.Items.Count > 0)
                {
                    var temp = PurchasingDetailList;
                    foreach (var detailInfo in temp)
                    {
                        detailInfo.SixtyDaySales = 0;
                        detailInfo.ThirtyDaySales = 0;
                        detailInfo.ElevenDaySales = 0 / 11;//日均销量(11天)
                    }
                    PurchasingDetailList = temp;
                    Rgd_PurchasingDetail.Rebind();
                }
            }
        }

        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RgGoods.MasterTableView.Columns[3].Visible = GoodsList.Any(act=>act.Luminositys.Count>0);
            RgGoods.MasterTableView.Columns[4].Visible = GoodsList.Any(act => act.Astigmias.Count > 0);
            RgGoods.MasterTableView.Columns[5].Visible = GoodsList.Any(act => act.Axialss.Count > 0);
            RgGoods.DataSource = GoodsList;
        }

        protected void RgGoodsItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var current = GoodsList.FirstOrDefault(act => act.GoodsId == goodsId);
                if (current!=null)
                {
                    foreach (GridTableCell cell in e.Item.Cells)
                    {
                        int cellIndex = e.Item.Cells.GetCellIndex(cell);
                        if (cellIndex >= 5)
                        {
                            var fieldList = cellIndex == 5
                                ? current.Luminositys
                                : cellIndex == 6 ? current.Astigmias : current.Axialss;
                            if (fieldList.Count>0)
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

        protected void RcbFilialeSelectedChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox)o;
            if (cb!=null)
            {
                if (Rgd_PurchasingDetail.Items.Count > 0)
                {
                    var temp = PurchasingDetailList;
                    var filialeId = new Guid(cb.SelectedValue);
                    foreach (var detailInfo in temp)
                    {
                        var purchasingDetailInfo =filialeId==Guid.Empty? null: _purchasingDetail.GetChildGoodsSale(detailInfo.GoodsID, new Guid(RCB_Warehouse.SelectedValue), DateTime.Now, filialeId);
                        if (purchasingDetailInfo != null)
                        {
                            detailInfo.SixtyDaySales = purchasingDetailInfo.SixtyDaySales;
                            detailInfo.ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales;
                            detailInfo.ElevenDaySales = purchasingDetailInfo.ElevenDaySales / 11;
                        }
                        else
                        {
                            detailInfo.SixtyDaySales = 0;
                            detailInfo.ThirtyDaySales = 0;
                            detailInfo.ElevenDaySales = 0;
                        }
                    }
                    PurchasingDetailList = temp;
                    Rgd_PurchasingDetail.Rebind();
                }
            }
            
        }
    }
}