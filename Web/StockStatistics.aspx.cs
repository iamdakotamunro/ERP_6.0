using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Purchasing;
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
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;
using StockWarning = ERP.BLL.Implement.Inventory.StockWarning;

namespace ERP.UI.Web
{
    public partial class StockStatistics : BasePage
    {
        private static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private int _stockDay = 10;

        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly StockWarning _stockWarning = new StockWarning(new DAL.Implement.Inventory.StockWarning(GlobalConfig.DB.FromType.Read), _goodsCenterSao);
        private readonly CodeManager _codeManager = new CodeManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            _stockDay = Convert.ToInt32(TB_StockDay.Text);
            if (!Page.IsPostBack)
            {
                BindInStock();
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                {
                    ViewState["WarehouseId"] = Guid.Empty.ToString();
                }
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set { ViewState["WarehouseId"] = value.ToString(); }
        }

        protected Dictionary<Guid, WarehouseFilialeAuth> WarehouseAuths
        {
            get
            {
                if (ViewState["WarehouseAuths"] == null)
                {
                    return new Dictionary<Guid, WarehouseFilialeAuth>();
                }
                return (Dictionary<Guid, WarehouseFilialeAuth>)ViewState["WarehouseAuths"];
            }
            set { ViewState["WarehouseAuths"] = value; }
        }

        protected Guid SFilialeId
        {
            get
            {
                if (ViewState["SFilialeId"] == null)
                {
                    ViewState["SFilialeId"] = Guid.Empty;
                }
                return new Guid(ViewState["SFilialeId"].ToString());
            }
            set { ViewState["SFilialeId"] = value.ToString(); }
        }
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set { ViewState["StartTime"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>采购单数据绑定
        /// </summary>
        private static bool _isTrue;
        protected void Rgss_GoodsNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsInfo> goodsInfoList = new List<GoodsInfo>();
            if (lbx_goodslist2.Items.Count != 0 && _isTrue)
            {
                foreach (RadListBoxItem item in lbx_goodslist2.Items)
                {
                    var gbinfo = new GoodsInfo { GoodsId = new Guid(item.Value), GoodsName = item.Text };
                    goodsInfoList.Add(gbinfo);
                }
            }
            RGSS.DataSource = goodsInfoList;
        }

        /// <summary>采购单商品详情数据绑定
        /// </summary>
        protected void Rgss_NeedDataSource(object source, GridDetailTableDataBindEventArgs e)
        {
            try
            {
                var totalWarnings = new List<StockWarningInfo>();
                var dataSource = new List<StockWarningInfo>();
                GridDataItem dataItem = e.DetailTableView.ParentItem;
                var parentGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var selectFilialeId = string.IsNullOrEmpty(RCB_Filile.SelectedValue)
                    ? Guid.Empty
                    : new Guid(RCB_Filile.SelectedValue);
                var realGoodsIds = new List<Guid>();
                GoodsInfo goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(parentGoodsId);
                var list = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { parentGoodsId });
                Dictionary<Guid, ChildGoodsInfo> realGoodsInfos = new Dictionary<Guid, ChildGoodsInfo>();
                if (list != null && list.Any(ent => ent.RealGoodsId != goodsInfo.GoodsId))
                {
                    realGoodsInfos = list.ToDictionary(k => k.RealGoodsId, v => v);
                    realGoodsIds.AddRange(realGoodsInfos.Keys);
                }

                else
                    realGoodsIds.Add(parentGoodsId);

                
                if (WarehouseAuths.ContainsKey(WarehouseId))
                {
                    var filialeWarnings = parentGoodsId != Guid.Empty
                            ? _stockWarning.GetStockWarningList(WarehouseId, SFilialeId, _stockDay, goodsInfo, realGoodsInfos)
                            : new List<StockWarningInfo>();
                    if (filialeWarnings.Count > 0)
                        totalWarnings.AddRange(filialeWarnings);

                    IList<StockWarningInfo> goodsSales = _stockWarning.GetMonthAvgGoodsSales(realGoodsIds, WarehouseId, selectFilialeId, _stockDay);

                    foreach (var realGoodsGroup in totalWarnings.GroupBy(act => act.GoodsId))
                    {
                        var realGoodsId = realGoodsGroup.Key;
                        var first = realGoodsGroup.First();
                        var stockWarningInfo = goodsSales.FirstOrDefault(act => act.GoodsId == realGoodsId);
                        if (stockWarningInfo != null)
                        {
                            stockWarningInfo.GoodsName = first.GoodsName;
                            stockWarningInfo.GoodsCode = first.GoodsCode;
                            stockWarningInfo.IsOnShelf = first.IsOnShelf;
                            stockWarningInfo.IsScarcity = first.IsScarcity;
                            stockWarningInfo.IsOnShelf = first.IsOnShelf;
                            stockWarningInfo.Specification = first.Specification;
                            stockWarningInfo.FirstNumberOneStockUpSale = realGoodsGroup.Sum(act => act.FirstNumberOneStockUpSale);
                            stockWarningInfo.FirstNumberTwoStockUpSale = realGoodsGroup.Sum(act => act.FirstNumberTwoStockUpSale);
                            stockWarningInfo.FirstNumberThreeStockUpSale = realGoodsGroup.Sum(act => act.FirstNumberThreeStockUpSale);

                            stockWarningInfo.SubtractPurchasingQuantity = realGoodsGroup.Sum(act => act.SubtractPurchasingQuantity);
                            stockWarningInfo.NonceWarehouseGoodsStock = realGoodsGroup.Sum(act => act.NonceWarehouseGoodsStock);
                            stockWarningInfo.RequireQuantity = realGoodsGroup.Sum(act => act.RequireQuantity);
                            stockWarningInfo.UppingQuantity = realGoodsGroup.Sum(act => act.UppingQuantity);
                            stockWarningInfo.SubtotalQuantity = realGoodsGroup.Sum(act => act.SubtotalQuantity);

                            stockWarningInfo.StockDay = _stockDay;
                            dataSource.Add(stockWarningInfo);
                        }
                    }
                }

                e.DetailTableView.DataSource = dataSource;
                var nonceWarehouseGoodsStock = e.DetailTableView.Columns.FindByUniqueName("NonceWarehouseGoodsStock");  //总库存
                var uppingQuantity = e.DetailTableView.Columns.FindByUniqueName("UppingQuantity"); //上货数
                var subtotalQuantity = e.DetailTableView.Columns.FindByUniqueName("SubtotalQuantity");  //需求数
                var requireQuantity = e.DetailTableView.Columns.FindByUniqueName("RequireQuantity");  //需求数
                var subtractPurchasingQuantity = e.DetailTableView.Columns.FindByUniqueName("SubtractPurchasingQuantity"); //采购中
                var firstNumberThreeStockUpSale = e.DetailTableView.Columns.FindByUniqueName("FirstNumberThreeStockUpSale");  //前3月
                var firstNumberTwoStockUpSale = e.DetailTableView.Columns.FindByUniqueName("FirstNumberTwoStockUpSale"); //前2月
                var firstNumberOneStockUpSale = e.DetailTableView.Columns.FindByUniqueName("FirstNumberOneStockUpSale"); //前1月
                var saleAvgCrease = e.DetailTableView.Columns.FindByUniqueName("SaleAvgCrease");  //平均增长率
                var weightedAverageSaleQuantity = e.DetailTableView.Columns.FindByUniqueName("WeightedAverageSaleQuantity");  //日平均销量
                var realityNeedPurchasingQuantity = e.DetailTableView.Columns.FindByUniqueName("RealityNeedPurchasingQuantity"); //建议备货数量

                nonceWarehouseGoodsStock.FooterText = string.Format("总库存：{0}", dataSource.Sum(act => act.NonceFilialeGoodsStock).ToString("0.##"));
                uppingQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.UppingQuantity).ToString("0.##"));
                subtotalQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.SubtotalQuantity).ToString("0.##"));
                requireQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.RequireQuantity));
                var threeStockUpSale = dataSource.Sum(act => act.FirstNumberThreeStockUpSale);
                var twoStockUpSale = dataSource.Sum(act => act.FirstNumberTwoStockUpSale);
                var firstStockUpSale = dataSource.Sum(act => act.FirstNumberOneStockUpSale);
                subtractPurchasingQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.SubtractPurchasingQuantity).ToString("0.##"));
                firstNumberThreeStockUpSale.FooterText = string.Format("小计：{0}", threeStockUpSale.ToString("0.##"));
                firstNumberTwoStockUpSale.FooterText = string.Format("小计：{0}", twoStockUpSale.ToString("0.##"));
                firstNumberOneStockUpSale.FooterText = string.Format("小计：{0}", firstStockUpSale.ToString("0.##"));
                saleAvgCrease.FooterText = string.Format("{0}%", GetAveSale(threeStockUpSale, twoStockUpSale, firstStockUpSale) * 100);
                weightedAverageSaleQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.WeightedAverageSaleQuantity).ToString("0.##"));
                realityNeedPurchasingQuantity.FooterText = string.Format("小计：{0}", dataSource.Sum(act => act.RealityNeedPurchasingQuantity).ToString("0.##"));

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private double GetAveSale(double threeStockUpSale, double twoStockUpSale, double firstStockUpSale)
        {
            var num1 = threeStockUpSale == 0 ? (twoStockUpSale > 1 ? 1 : 0) : (twoStockUpSale - threeStockUpSale) / threeStockUpSale;
            var num2 = twoStockUpSale == 0 ? (firstStockUpSale > 1 ? 1 : 0) : (firstStockUpSale - twoStockUpSale) / twoStockUpSale;
            var num = (num1 + num2) / 2;
            if (num >= 1.3)
            {
                return 1.1;
            }
            return Convert.ToDouble(num.ToString("0.#"));
        }

        /// <summary>生成报表
        ///</summary>
        protected void Ib_CreationData_Click(object sender, ImageClickEventArgs e)
        {
            if (lbx_goodslist2.Items.Count != 0)
            {
                _isTrue = true;
                _stockDay = Convert.ToInt32(TB_StockDay.Text);
                var filileValue = RCB_Filile.SelectedValue;
                var warehouseIdValue = RCB_InStock.SelectedValue;
                WarehouseId = string.IsNullOrEmpty(warehouseIdValue) ? Guid.Empty : new Guid(warehouseIdValue);
                SFilialeId = string.IsNullOrEmpty(filileValue) ? Guid.Empty : new Guid(filileValue);
                if (WarehouseId == Guid.Empty)
                {
                    _isTrue = false;
                    RGSS.DataSource = null;
                    RAM.Alert("请选择仓库！");
                }
                else
                {
                    if (SFilialeId == Guid.Empty)
                    {
                        _isTrue = false;
                        RGSS.DataSource = null;
                        RAM.Alert("请选择物流公司！");
                    }
                }
                RGSS.Rebind();
            }
            else
            {
                _isTrue = false;
                RGSS.DataSource = null;
                RAM.Alert("没有生成报表的数据！");
            }
        }

        /// <summary> 功能修改:报备统计功能修改
        /// </summary>
        protected void Ib_ExportData_Click(object sender, EventArgs e)
        {
            _stockDay = Convert.ToInt32(TB_StockDay.Text);
            var filileValue = RCB_Filile.SelectedValue;
            var warehouseIdValue = RCB_InStock.SelectedValue;
            SFilialeId = string.IsNullOrEmpty(filileValue) ? Guid.Empty : new Guid(filileValue);
            WarehouseId = string.IsNullOrEmpty(warehouseIdValue) ? Guid.Empty : new Guid(warehouseIdValue);
            if (WarehouseId == Guid.Empty)
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            if (SFilialeId == Guid.Empty)
            {
                RAM.Alert("请选择物流公司！");
                return;
            }
            OutPutExcel();
        }

        /// <summary> 生成采购单
        /// </summary>
        protected void Ib_Purching_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(RDP_ArrivalTime.DateInput.Text))
                {
                    RAM.Alert("到货时间必须选择!");
                    return;
                }
                if (RGSS.Items.Count == 0)
                {
                    RAM.Alert("无数据可生成!");
                    return;
                }
                _stockDay = Convert.ToInt32(TB_StockDay.Text);
                var filileValue = RCB_Filile.SelectedValue;
                var warehouseIdValue = RCB_InStock.SelectedValue;
                WarehouseId = string.IsNullOrEmpty(warehouseIdValue) ? Guid.Empty : new Guid(warehouseIdValue);
                if (WarehouseId == Guid.Empty)
                {
                    RAM.Alert("请选择仓库！");
                    return;
                }
                var hostingFilialeId = string.IsNullOrEmpty(filileValue) ? Guid.Empty : new Guid(filileValue);
                if (hostingFilialeId == Guid.Empty)
                {
                    RAM.Alert("请选择物流公司！");
                    return;
                }

                var goodsIds = new List<Guid>();
                foreach (GridDataItem item in RGSS.MasterTableView.Items)
                {
                    var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                    if (goodsIds.Count(w => w == goodsId) == 0)
                        goodsIds.Add(goodsId);
                }
                var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsIds.Count != goodsList.Count)
                {
                    RAM.Alert("商品中心异常，操作无效请稍后再试");
                    return;
                }
                IList<PurchaseSetInfo> purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(goodsIds, WarehouseId,hostingFilialeId);
                var purDics = new Dictionary<PurchasingInfo, IList<PurchasingDetailInfo>>();
                #region  整理生成采购申请单
                foreach (GridDataItem item in RGSS.MasterTableView.Items)
                {
                    var itemGoodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                    GoodsInfo goodsBaseInfo = goodsList.FirstOrDefault(w => w.GoodsId == itemGoodsId) ?? new GoodsInfo();
                    IList<StockWarningInfo> swList = _stockWarning.GetStockWarningListNew(WarehouseId, hostingFilialeId, itemGoodsId, _stockDay, goodsBaseInfo);
                    foreach (var stockWarningInfo in swList.Where(act => act.RealityNeedPurchasingQuantity > 0))
                    {
                        Guid goodsid = stockWarningInfo.GoodsId;
                        string goodsName = goodsBaseInfo.GoodsName;
                        int purchaseQuantity = Convert.ToInt32(stockWarningInfo.RealityNeedPurchasingQuantity);
                        string specification = stockWarningInfo.Specification ?? string.Empty;
                        Guid pGuid = Guid.NewGuid();
                        PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsBaseInfo.GoodsId);
                        if (purchaseSetInfo != null)
                        {
                            decimal price = purchaseSetInfo.PurchasePrice <= 0 ? -1 : purchaseSetInfo.PurchasePrice;
                            Guid companyId = purchaseSetInfo.CompanyId;
                            IList<PurchasingInfo> piList = companyId == Guid.Empty ?
                                _purchasing.GetPurchasingListByCompanyID(Guid.Empty, (int)PurchasingState.NoSubmit, (int)PurchasingType.Custom, WarehouseId)
                                : _purchasing.GetPurchasingList(DateTime.MinValue, DateTime.MinValue, companyId, WarehouseId, hostingFilialeId, PurchasingState.NoSubmit, PurchasingType.Custom, string.Empty, Guid.Empty, purchaseSetInfo.PersonResponsible);
                            #region  该供应商没有采购单
                            var tempPurchasingInfo =
                                        purDics.Keys.FirstOrDefault(
                                            act => act.CompanyID == companyId && act.WarehouseID == WarehouseId
                                                   && act.PersonResponsible == purchaseSetInfo.PersonResponsible && act.PurchasingFilialeId == hostingFilialeId);
                            var flag = tempPurchasingInfo == null || tempPurchasingInfo.PurchasingID == Guid.Empty;
                            if (piList.Count == 0 || piList.All(act => act.PurchasingFilialeId != hostingFilialeId)) //该供应商没有采购单
                            {
                                //新添加的采购 按单光度和按总数赠品一致
                                CompanyCussentInfo ccinfo = _companyCussent.GetCompanyCussent(companyId);
                                if (RDP_ArrivalTime.SelectedDate != null)
                                {
                                    var des = string.Format("[采购类别:{0},报备统计生成;采购人:{1}]", EnumAttribute.GetKeyName(PurchasingType.Custom),
                                        CurrentSession.Personnel.Get().RealName);

                                    IList<PurchasingDetailInfo> pdList;

                                    if (flag)
                                    {
                                        tempPurchasingInfo = new PurchasingInfo(pGuid, _codeManager.GetCode(CodeType.PH), companyId,
                                                                   ccinfo == null ? "" : ccinfo.CompanyName, hostingFilialeId, WarehouseId,
                                                                   (int)PurchasingState.NoSubmit, (int)PurchasingType.Custom, DateTime.Now,
                                                                   DateTime.MaxValue, des, Guid.Empty, "", CurrentSession.Personnel.Get().RealName)
                                        {
                                            ArrivalTime = RDP_ArrivalTime.SelectedDate.Value,
                                            Director = purchaseSetInfo.PersonResponsibleName,
                                            PersonResponsible = purchaseSetInfo.PersonResponsible,
                                            PurchasingFilialeId = hostingFilialeId
                                        };
                                        pdList = new List<PurchasingDetailInfo>();
                                    }
                                    else
                                    {
                                        pdList = purDics[tempPurchasingInfo];
                                        pGuid = tempPurchasingInfo.PurchasingID;
                                    }

                                    var detailInfo = new PurchasingDetailInfo(pGuid, goodsid,
                                                                              goodsBaseInfo.GoodsName,
                                                                              goodsBaseInfo.Units,
                                                                              goodsBaseInfo.GoodsCode,
                                                                              specification,
                                                                              companyId, price,
                                                                              purchaseQuantity, 0, 0, "",
                                                                              Guid.NewGuid(),
                                                                              (int)PurchasingGoodsType.NoGift)
                                    {
                                        CPrice = price
                                    };
                                    // 获取商品的60、30、11天销量
                                    //var purchasingDetailBll = new PurchasingDetailManager();
                                    PurchasingDetailInfo pdInfo = _purchasingDetail.GetChildGoodsSale(detailInfo.GoodsID, WarehouseId, DateTime.Now, tempPurchasingInfo.PurchasingFilialeId);
                                    if (pdInfo != null)
                                    {
                                        detailInfo.SixtyDaySales = pdInfo.SixtyDaySales;
                                        detailInfo.ThirtyDaySales = pdInfo.ThirtyDaySales;
                                        detailInfo.ElevenDaySales = pdInfo.ElevenDaySales / 11;//日均销量(11天)
                                    }
                                    else
                                    {
                                        detailInfo.SixtyDaySales = 0;
                                        detailInfo.ThirtyDaySales = 0;
                                        detailInfo.ElevenDaySales = 0;
                                    }
                                    if (flag)
                                    {
                                        pdList.Add(detailInfo);
                                        purDics.Add(tempPurchasingInfo, pdList);
                                    }
                                    else
                                    {
                                        var tempDetailInfo = pdList.FirstOrDefault(act => act.GoodsID == goodsid && act.PurchasingGoodsType == (int)PurchasingGoodsType.NoGift);
                                        if (tempDetailInfo == null)
                                        {
                                            pdList.Add(detailInfo);
                                        }
                                        else
                                        {
                                            tempDetailInfo.PlanQuantity += purchaseQuantity;
                                        }
                                        purDics[tempPurchasingInfo] = pdList;
                                    }
                                }
                            }
                            #endregion
                            #region  存在采购单
                            else if (piList.Count >= 1 && piList.Count(act => act.PurchasingFilialeId == hostingFilialeId) == 1)
                            {
                                var purchasingInfo = piList.First(act => act.PurchasingFilialeId == hostingFilialeId);
                                if (tempPurchasingInfo == null || !tempPurchasingInfo.PurchasingID.Equals(purchasingInfo.PurchasingID))
                                {
                                    tempPurchasingInfo = purchasingInfo;
                                }
                                IList<PurchasingDetailInfo> pdList = new List<PurchasingDetailInfo>();
                                var detailInfo = new PurchasingDetailInfo(tempPurchasingInfo.PurchasingID, goodsid,
                                                                          goodsBaseInfo.GoodsName,
                                                                          goodsBaseInfo.Units,
                                                                          goodsBaseInfo.GoodsCode, specification,
                                                                          companyId, price, purchaseQuantity, 0,
                                                                          0, "", Guid.NewGuid(),
                                                                          (int)PurchasingGoodsType.NoGift)
                                {
                                    CPrice = price
                                };

                                PurchasingDetailInfo purchasingDetailInfo = _purchasingDetail.GetChildGoodsSale(detailInfo.GoodsID, WarehouseId, DateTime.Now, hostingFilialeId);
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
                                if (!flag) //已经添加采购单
                                {
                                    pdList = purDics[tempPurchasingInfo];
                                }
                                else
                                {
                                    purDics.Add(tempPurchasingInfo, pdList);
                                }
                                var tempDetailInfo = pdList.FirstOrDefault(act => act.GoodsID == goodsid && act.PurchasingGoodsType == (int)PurchasingGoodsType.NoGift);
                                if (tempDetailInfo == null)
                                {
                                    pdList.Add(detailInfo);
                                }
                                else
                                {
                                    tempDetailInfo.PlanQuantity += purchaseQuantity;
                                }
                                purDics[tempPurchasingInfo] = pdList;
                            }
                            #endregion
                            else
                            {
                                RAM.Alert("已存在的采购单数据不正确，同供应商同采购公司只能有一张采购单据！");
                                return;
                            }
                        }
                        else
                        {
                            RAM.Alert("【 " + goodsName + " 】未添加商品采购设置");
                            return;
                        }
                    }
                }
                #endregion
                if (purDics.Count == 0)
                {
                    RAM.Alert("当前采购商品已满足需求无需生成采购单！");
                    return;
                }
                #region  采购单重新分配
                string msg;
                var stockDeclareManager = new StockDeclareManager(_purchasing, _purchasingDetail, _purchaseSet, _goodsCenterSao, _companyCussent);
                var result = stockDeclareManager.CreatePurchasing(purDics, goodsList.ToDictionary(act => act.GoodsName, v => v.GoodsId), purchaseSetList, out msg);
                if (!result)
                {
                    RAM.Alert(msg);
                }
                else
                {
                    RAM.Alert("生成采购单成功!");
                    RGSS.Rebind();
                }
                #endregion
            }
            catch (Exception exception)
            {
                RAM.Alert("生成采购单失败--" + exception.Message);
            }
        }

        #region 导出报备统计Excel
        /// <summary>
        /// 功能修改:导出报备统计Excel
        /// 时    间:2010-11-23
        /// 作    者:蒋赛彪
        /// </summary>
        public void OutPutExcel()
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region Excel样式

            //标题样式styletitle
            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.RED.index;
            fonttitle.Boldweight = 1;
            HSSFCellStyle styletitle = workbook.CreateCellStyle();
            styletitle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderRight = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderTop = HSSFCellStyle.BORDER_THIN;
            styletitle.SetFont(fonttitle);
            //内容字体styleContent
            HSSFFont fontcontent = workbook.CreateFont();
            fontcontent.FontHeightInPoints = 9;
            fontcontent.Color = HSSFColor.BLACK.index;
            HSSFCellStyle styleContent = workbook.CreateCellStyle();
            styleContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            styleContent.SetFont(fontcontent);
            //总计 styletotal
            HSSFFont fonttotal = workbook.CreateFont();
            fonttotal.FontHeightInPoints = 12;
            fonttotal.Color = HSSFColor.RED.index;
            fonttotal.Boldweight = 2;
            HSSFCellStyle styletotal = workbook.CreateCellStyle();
            styletotal.SetFont(fonttotal);
            #endregion

            #region 报备模板 以及sheet名字

            sheet[0] = workbook.CreateSheet("报备统计" + DateTime.Now.ToString("yyyy-MM-dd"));//添加sheet名
            sheet[0].DefaultColumnWidth = 30;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            celltitie.SetCellValue("报备统计" + DateTime.Now.ToString("yyyy-MM-dd"));
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            sheet[0].AddMergedRegion(new Region(0, 0, 0, 8));
            #endregion

            #region //列名
            HSSFRow row8 = sheet[0].CreateRow(4);
            HSSFCell cell1 = row8.CreateCell(0);
            HSSFCell cell2 = row8.CreateCell(1);
            HSSFCell cell3 = row8.CreateCell(2);
            HSSFCell cell4 = row8.CreateCell(3);
            HSSFCell cell5 = row8.CreateCell(4);
            HSSFCell cell6 = row8.CreateCell(5);
            HSSFCell cell7 = row8.CreateCell(6);
            HSSFCell cell8 = row8.CreateCell(7);
            cell1.SetCellValue("商品名");
            cell2.SetCellValue("");
            cell3.SetCellValue("");
            cell4.SetCellValue("");
            cell5.SetCellValue("");
            cell6.SetCellValue("");
            cell7.SetCellValue("");
            cell8.SetCellValue("");
            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;
            cell5.CellStyle = styletitle;
            cell6.CellStyle = styletitle;
            cell7.CellStyle = styletitle;
            cell8.CellStyle = styletitle;
            #endregion

            var goodsIdList = new List<Guid>();

            foreach (RadListBoxItem item in lbx_goodslist2.Items)
            {
                var goodsId = new Guid(item.Value);
                if (goodsIdList.Count(w => w == goodsId) == 0)
                    goodsIdList.Add(goodsId);
            }
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList).ToDictionary(k=>k.GoodsId,v=>v);

            var dataList = new List<HostingFilialeAuth>();

            if (WarehouseAuths.ContainsKey(WarehouseId))
            {
                dataList.AddRange(string.IsNullOrEmpty(RCB_Filile.SelectedValue) ||
                   RCB_Filile.SelectedValue == string.Format("{0}", Guid.Empty)
                    ? WarehouseAuths[WarehouseId].FilialeAuths
                    : WarehouseAuths[WarehouseId].FilialeAuths.Where(
                        ent => ent.HostingFilialeId == new Guid(RCB_Filile.SelectedValue)));
            }
            #region 分类报备数据填充
            int row = 5;
            //int index = 0;
            foreach (var hostingFiliale in dataList)
            {
                HSSFRow filialeRow = sheet[0].CreateRow(row);
                HSSFCell filialeCell = filialeRow.CreateCell(0);

                filialeCell.SetCellValue(hostingFiliale.HostingFilialeName);//缺货状态
                filialeCell.CellStyle = style;
                sheet[0].AddMergedRegion(new Region(row, 0, row, 8));

                row++;
                foreach (RadListBoxItem item in lbx_goodslist2.Items)
                {
                    HSSFRow rowt = sheet[0].CreateRow(row);
                    HSSFCell c1 = rowt.CreateCell(0);
                    HSSFCell c2 = rowt.CreateCell(1);
                    HSSFCell c3 = rowt.CreateCell(2);
                    HSSFCell c4 = rowt.CreateCell(3);
                    HSSFCell c5 = rowt.CreateCell(4);
                    HSSFCell c6 = rowt.CreateCell(5);
                    HSSFCell c7 = rowt.CreateCell(6);
                    HSSFCell c8 = rowt.CreateCell(7);
                    HSSFCell c9 = rowt.CreateCell(8);
                    c1.SetCellValue(item.Text);//商品名
                    c2.SetCellValue("");//商品SKU
                    c3.SetCellValue("");//现有库存
                    c4.SetCellValue("");//需求量
                    c5.SetCellValue("");//销售总数
                    c6.SetCellValue("");//均值备货量
                    c7.SetCellValue("");//建议备货量
                    c8.SetCellValue("");//缺货状态
                    c9.SetCellValue("");//上架

                    c1.CellStyle = styleContent;
                    c2.CellStyle = styleContent;
                    c3.CellStyle = styleContent;
                    c4.CellStyle = styleContent;
                    c5.CellStyle = styleContent;
                    c6.CellStyle = styleContent;
                    c7.CellStyle = styleContent;
                    c8.CellStyle = styleContent;
                    c9.CellStyle = styleContent;
                    c1.CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    row++;

                    // GridTableView 里面的数据
                    #region //列名
                    HSSFRow row9 = sheet[0].CreateRow(row);
                    HSSFCell vc1 = row9.CreateCell(1);
                    HSSFCell vc2 = row9.CreateCell(2);
                    HSSFCell vc3 = row9.CreateCell(3);
                    HSSFCell vc4 = row9.CreateCell(4);
                    HSSFCell vc5 = row9.CreateCell(5);
                    HSSFCell vc6 = row9.CreateCell(6);
                    HSSFCell vc7 = row9.CreateCell(7);
                    HSSFCell vc8 = row9.CreateCell(8);
                    vc1.SetCellValue("商品SKU");
                    vc2.SetCellValue("现有库存");
                    vc3.SetCellValue("出库量");
                    vc4.SetCellValue("销售总数");
                    vc5.SetCellValue("均值备货量");
                    vc6.SetCellValue("建议备货量");
                    vc7.SetCellValue("缺货");
                    vc8.SetCellValue("上架");
                    vc1.CellStyle = styleContent;
                    vc2.CellStyle = styleContent;
                    vc3.CellStyle = styleContent;
                    vc4.CellStyle = styleContent;
                    vc5.CellStyle = styleContent;
                    vc6.CellStyle = styleContent;
                    vc7.CellStyle = styleContent;
                    vc8.CellStyle = styleContent;
                    row++;
                    #endregion

                    var goodsId = new Guid(item.Value);
                    if(!goodsList.ContainsKey(goodsId))continue;
                    GoodsInfo goodsBaseInfo = goodsList[goodsId];
                    var realGoodsList = new List<Guid>();
                    #region

                    var childGoodsInfos = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId });
                    IList<StockWarningInfo> swList = _stockWarning.GetStockWarningList(WarehouseId, hostingFiliale.HostingFilialeId, _stockDay, goodsBaseInfo, childGoodsInfos.ToDictionary(k=>k.RealGoodsId,v=>v));
                    IList<StockWarningInfo> list = _stockWarning.GetStockWarningListForReport(WarehouseId, hostingFiliale.HostingFilialeId, realGoodsList, _stockDay);

                    int drow = 0;
                    double nonceFilialeGoodsStockTotal = 0;
                    double salesNumberTotal = 0;
                    double nonceGoodsStockTotal = 0;
                    foreach (StockWarningInfo swinfo in swList)
                    {
                        if (swinfo != null)
                        {
                            HSSFRow rowChild = sheet[0].CreateRow(row);
                            HSSFCell rc1 = rowChild.CreateCell(1);
                            HSSFCell rc2 = rowChild.CreateCell(2);
                            HSSFCell rc3 = rowChild.CreateCell(3);
                            HSSFCell rc4 = rowChild.CreateCell(4);
                            HSSFCell rc5 = rowChild.CreateCell(5);
                            HSSFCell rc6 = rowChild.CreateCell(6);
                            HSSFCell rc7 = rowChild.CreateCell(7);
                            HSSFCell rc8 = rowChild.CreateCell(8);
                            rc1.SetCellValue(swinfo.Specification);//商品规则
                            rc2.SetCellValue(swinfo.NonceFilialeGoodsStock);//现有库存
                            rc3.SetCellValue(swinfo.NonceRequest);//需求量
                            double count = swinfo.FirstNumberOneStockUpSale + swinfo.FirstNumberTwoStockUpSale + swinfo.FirstNumberThreeStockUpSale;
                            rc4.SetCellValue(count);//销售总数
                            nonceFilialeGoodsStockTotal += swinfo.NonceFilialeGoodsStock;
                            var info = list.FirstOrDefault(act => act.GoodsId == swinfo.GoodsId);
                            rc5.SetCellValue(info != null ? info.WeightedAverageSaleQuantity : swinfo.WeightedAverageSaleQuantity);//均值备货量
                            rc6.SetCellValue(swinfo.RealityNeedPurchasingQuantity);//建议备货量
                            rc7.SetCellValue(GetIsScarcity(swinfo.IsScarcity));//缺货
                            rc7.SetCellValue(GetIsOnShelf(swinfo.IsOnShelf));//状态
                            salesNumberTotal += count;
                            nonceGoodsStockTotal += swinfo.RealityNeedPurchasingQuantity;
                            rc1.CellStyle = styleContent;
                            rc2.CellStyle = styleContent;
                            rc3.CellStyle = styleContent;
                            rc4.CellStyle = styleContent;
                            rc5.CellStyle = styleContent;
                            rc6.CellStyle = styleContent;
                            rc7.CellStyle = styleContent;
                            rc8.CellStyle = styleContent;
                            rc1.CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                            row++;
                        }
                        drow++;
                        if (drow == swList.Count)
                        {
                            HSSFRow rtotal = sheet[0].CreateRow(row);
                            HSSFCell t2 = rtotal.CreateCell(2);
                            HSSFCell t4 = rtotal.CreateCell(4);
                            HSSFCell t6 = rtotal.CreateCell(6);
                            t2.SetCellValue("总库存:" + nonceFilialeGoodsStockTotal);
                            t4.SetCellValue("总销量:" + salesNumberTotal);
                            t6.SetCellValue("总备货量:" + nonceGoodsStockTotal);
                            t2.CellStyle = styletotal;
                            t4.CellStyle = styletotal;
                            t6.CellStyle = styletotal;
                            row++;
                            sheet[0].AddMergedRegion(new Region(row, 0, row, 7));
                            row++;
                        }
                    }
                    #endregion
                    //index++;
                }
            }

            sheet[0].DisplayGridlines = false;
            #endregion

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("可得网报备统计" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();

            GC.Collect();
            #endregion
        }
        #endregion

        #region 分公司和仓库绑定

        private void BindFilile()
        {
            RCB_Filile.Items.Clear();
            List<HostingFilialeAuth> fl = new List<HostingFilialeAuth>();
            var warehouse = RCB_InStock.SelectedValue;
            if (!string.IsNullOrEmpty(warehouse) && warehouse != Guid.Empty.ToString())
            {
                var warehouseId = new Guid(warehouse);
                if (WarehouseAuths.ContainsKey(warehouseId))
                {
                    fl.AddRange(WarehouseAuths[warehouseId].FilialeAuths);
                }
            }
            RCB_Filile.DataSource = fl;
            RCB_Filile.DataTextField = "HostingFilialeName";
            RCB_Filile.DataValueField = "HostingFilialeId";
            RCB_Filile.DataBind();
        }

        private void BindInStock()
        {
            var personinfo = CurrentSession.Personnel.Get();
            var warehouseAuths = WMSSao.GetWarehouseAndFilialeAuth(personinfo.PersonnelId);
            WarehouseAuths = warehouseAuths.ToDictionary(k=>k.WarehouseId,v=>v);
            RCB_InStock.DataSource = warehouseAuths;
            RCB_InStock.DataTextField = "WarehouseName";
            RCB_InStock.DataValueField = "WarehouseId";
            RCB_InStock.DataBind();
            BindFilile();
        }
        #endregion

        /// <summary>显示商品状态
        /// </summary>
        protected string GetIsScarcity(object isScarcity)
        {
            return Convert.ToBoolean(isScarcity) ? "√" : "-";
        }

        protected string GetIsOnShelf(object isOnShelf)
        {
            return Convert.ToBoolean(isOnShelf) ? "√" : "-";
        }

        /// <summary>筛选商品
        /// </summary>
        protected void Btn_Delete_Click(object sender, EventArgs e)
        {
            foreach (var item in lbx_goodslist1.SelectedItems)
            {
                lbx_goodslist1.Items.Remove(item);
            }
        }

        /// <summary>查询商品并添加商品到RadListBox
        /// </summary>
        protected void Btn_Add_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GoodsName.Text.Trim()))
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(GoodsName.Text);
                if (dic.Count != 0)
                {
                    var ids = lbx_goodslist1.Items.Count > 0
                    ? lbx_goodslist1.Items.Select(ent => ent.Value)
                    : new List<String>();
                    foreach (var keyValuePair in dic)
                    {
                        if(ids.Contains(keyValuePair.Key))continue;
                        lbx_goodslist1.Items.Add(new RadListBoxItem(keyValuePair.Value, string.Format("{0}", keyValuePair.Key)));
                    }
                }
                else
                {
                    RAM.Alert("没找到你要添加的商品！");
                }
            }
            else
            {
                RAM.Alert("请先填写商品名称！");
            }
        }

        /// <summary> 获取(采购部)所有员工的信息
        /// </summary>
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

        #region 商品的切换

        /// <summary> 添加一个选中商品到右边RadListBox
        /// </summary>
        protected void AddToRight(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbx_goodslist1.SelectedValue))
            {
                var goodsId = new Guid(lbx_goodslist1.SelectedValue);
                string goodsName = lbx_goodslist1.SelectedItem.Text;
                bool isExist = false;
                if (lbx_goodslist2.Items.Count > 0)
                {
                    isExist = lbx_goodslist2.Items.Any(ent => ent.Value.Equals(goodsId.ToString()));
                }
                if(!isExist)
                    lbx_goodslist2.Items.Add(new RadListBoxItem(goodsName, goodsId.ToString()));
                foreach (var item in lbx_goodslist1.SelectedItems)
                {
                    lbx_goodslist1.Items.Remove(item);
                }
            }
            else
            {
                RAM.Alert("请先选择数据！");
            }
        }

        /// <summary>将右边选中商品移除到左边RadListBox
        /// </summary>
        protected void RemoveToLeft(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbx_goodslist2.SelectedValue))
            {
                var goodsId = new Guid(lbx_goodslist2.SelectedValue);
                string goodsName = lbx_goodslist2.SelectedItem.Text;
                bool isExist = false;
                if (lbx_goodslist1.Items.Count > 0)
                {
                    isExist = lbx_goodslist1.Items.Any(ent => ent.Value.Equals(goodsId.ToString()));
                }
                if(!isExist)
                    lbx_goodslist1.Items.Add(new RadListBoxItem(goodsName, goodsId.ToString()));
                foreach (var item in lbx_goodslist2.SelectedItems)
                {
                    lbx_goodslist2.Items.Remove(item);
                }
            }
            else
            {
                RAM.Alert("请先选择数据！");
            }
        }

        /// <summary> 将左边商品全部添加到右边RadListBox
        /// </summary>
        protected void AllAddToRight(object sender, EventArgs e)
        {
            if (lbx_goodslist1.Items.Count == 0)
            {
                RAM.Alert("请先添加数据");
            }
            else
            {
                var ids = lbx_goodslist2.Items.Count > 0
                    ? lbx_goodslist2.Items.Select(ent => ent.Value)
                    : new List<String>();
                foreach (var source in lbx_goodslist1.Items.ToList())
                {
                    if(ids.Contains(source.Value))continue;
                    lbx_goodslist2.Items.Add(new RadListBoxItem(source.Text, source.Value));
                }
                lbx_goodslist1.Items.Clear();
            }
        }

        /// <summary> 将右边商品全部移除到左边RadListBox
        /// </summary>
        protected void AllReMoveToLeft(object sender, EventArgs e)
        {
            if (lbx_goodslist2.Items.Count == 0)
            {
                RAM.Alert("请先添加数据");
            }
            else
            {
                var ids = lbx_goodslist1.Items.Count > 0
                    ? lbx_goodslist1.Items.Select(ent => ent.Value)
                    : new List<String>();
                foreach (var source in lbx_goodslist2.Items.ToList())
                {
                    if(ids.Contains(source.Value))continue;
                    lbx_goodslist1.Items.Add(new RadListBoxItem(source.Text, source.Value));
                }
                lbx_goodslist2.Items.Clear();
            }
        }

        #endregion

        protected void RCB_InStock_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindFilile();
        }
    }
}