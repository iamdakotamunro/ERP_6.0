using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using AllianceShop.Common.Extension;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
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
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>
    /// 商品销量排行
    /// </summary>
    public partial class SalesRankingsAw : BasePage
    {
        //public Dictionary<Guid, int> StockList;

        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly SalesGoodsRankingManager _goodsRankingManager = new SalesGoodsRankingManager(_goodsCenterSao, new SalesGoodsRanking(GlobalConfig.DB.FromType.Read));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ViewState["TopNum"] = 100;
                var brandList = _goodsCenterSao.GetAllBrandList();
                GoodsBrandInfos = brandList.ToDictionary(k => k.BrandId, v => v.Brand);
                RCB_Brand.DataSource = brandList;
                RCB_Brand.DataBind();
                LoadData();
                GetTreeGoodsClass();
                CblType.SelectedIndex = 0;
            }
        }

        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().OrderBy(act => act.OrderIndex).ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
            tree_GoodsClass.Attributes.CssStyle.Value = "display:none;";
        }

        //遍历产品分类
        private void RecursivelyGoodsClass(Guid goodsClassId, IRadTreeNodeContainer node, IList<GoodsClassInfo> goodsClassList)
        {
            IList<GoodsClassInfo> childGoodsClassList = goodsClassList.Where(w => w.ParentClassId == goodsClassId).ToList();
            foreach (GoodsClassInfo goodsClassInfo in childGoodsClassList)
            {
                RadTreeNode goodsClassNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                if (node == null)
                    TVGoodsClass.Nodes.Add(goodsClassNode);
                else
                    node.Nodes.Add(goodsClassNode);
                RecursivelyGoodsClass(goodsClassInfo.ClassId, goodsClassNode, goodsClassList);
            }
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        protected void Rcb_SalePlatform_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RCB_SalePlatform.Items.Clear();
            RCB_SalePlatform.Text = "";
            var filialeId =!string.IsNullOrEmpty(RCB_SaleFiliale.SelectedValue)?RCB_SaleFiliale.SelectedValue.ToGuid():Guid.Empty;
            if (filialeId != Guid.Empty)
            {
                IList<SalePlatformInfo> salePlatformInfos = CacheCollection.SalePlatform.GetListByFilialeId(filialeId);
                if (salePlatformInfos.Count == 0)
                {
                    var filialeInfos = new List<FilialeInfo>();
                    var filiale = CacheCollection.Filiale.GetChildShopList(filialeId).ToList();
                    filialeInfos.AddRange(filiale);
                    foreach (var filialeInfo in filiale)
                    {
                        filialeInfos.AddRange(CacheCollection.Filiale.GetChildShopList(filialeInfo.ID).ToList());
                    }
                    RCB_SalePlatform.DataSource = CB_ContainDisableSalePlatform.Checked ? filialeInfos : filialeInfos.Where(ent => ent.IsActive).ToList();
                    RCB_SalePlatform.DataBind();
                }
                else
                {
                    RCB_SalePlatform.DataSource = CB_ContainDisableSalePlatform.Checked ? salePlatformInfos : salePlatformInfos.Where(ent => ent.IsActive).ToList();
                    RCB_SalePlatform.DataBind();
                }
                RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部平台", Guid.Empty.ToString()));
            }
        }

        private void LoadData()
        {
            var saleFiliales = new List<ProxyFiliale> { new ProxyFiliale(Guid.Empty, "全部销售公司", new List<int>()) };
            if (WarehouseAuths != null && WarehouseAuths.Count > 0)
            {
                foreach (var warehouseFilialeAuth in WarehouseAuths)
                {
                    foreach (var hostingFilialeAuth in warehouseFilialeAuth.FilialeAuths)
                    {
                        saleFiliales.AddRange(hostingFilialeAuth.ProxyFiliales);
                    }
                }
            }
            RCB_SaleFiliale.DataSource = saleFiliales.GroupBy(p => new { p.ProxyFilialeId, p.ProxyFilialeName }).Select(g => new { g.Key.ProxyFilialeId, g.Key.ProxyFilialeName });
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.SelectedIndex = 0;


            StartTime = DateTime.Now.AddDays(-30);
            EndTime = DateTime.Now;
            RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
            RDP_EndTime.SelectedDate = DateTime.Now;
        }

        /// <summary>授权仓储和物流配送公司
        /// </summary>
        protected List<WarehouseFilialeAuth> WarehouseAuths
        {
            get
            {
                if (ViewState["WarehouseAuths"] == null)
                    ViewState["WarehouseAuths"] = WMSSao.GetWarehouseAndFilialeAuth(CurrentSession.Personnel.Get().PersonnelId);
                return (List<WarehouseFilialeAuth>)ViewState["WarehouseAuths"];
            }
            set
            {
                ViewState["WarehouseAuths"] = value;
            }
        }

        protected Guid ClassId
        {
            get
            {
                if (ViewState["ClassId"] == null) return Guid.Empty;
                return new Guid(ViewState["ClassId"].ToString());
            }
            set
            {
                ViewState["ClassId"] = value.ToString();
            }
        }
        /// <summary>
        /// 品牌ID
        /// </summary>
        protected Guid BrandId
        {
            get { return ViewState["BrandId"] == null ? Guid.Empty : new Guid(ViewState["BrandId"].ToString()); }
            set { ViewState["BrandId"] = value.ToString(); }
        }

        protected Guid FilialeId
        {
            get
            {
                if (ViewState["FilialeId"] == null)
                {
                    ViewState["FilialeId"] = CurrentSession.Personnel.Get().FilialeId;
                }
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value.ToString();
            }
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                if (Convert.ToDateTime(ViewState["EndTime"]) != DateTime.MinValue)
                {
                    return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).ToString("yyyy-MM-dd 23:59:59.997"));
                }
                return DateTime.MinValue;
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }

        protected int TopNum
        {
            get
            {
                if (ViewState["TopNum"] == null) return 100;
                return Convert.ToInt32(ViewState["TopNum"]);
            }
            set
            {
                ViewState["TopNum"] = value;
            }
        }

        protected int SalesType
        {
            get
            {
                if (ViewState["SalesType"] == null) return 1;
                return Convert.ToInt32(ViewState["SalesType"]);
            }
            set
            {
                ViewState["SalesType"] = value;
            }
        }

        protected Dictionary<Guid, string> GoodsBrandInfos
        {
            get
            {
                if (ViewState["GoodsBrandInfos"] == null) return new Dictionary<Guid, string>();
                return (Dictionary<Guid, string>)ViewState["GoodsBrandInfos"];
            }
            set
            {
                ViewState["GoodsBrandInfos"] = value;
            }
        }

        /// <summary>
        /// 生成报表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_CreationData_Click(object sender, ImageClickEventArgs e)
        {
            if (RDP_StartTime.SelectedDate == null)
            {
                RAMSalesRankings.Alert("请选择起始日期！");
                return;
            }
            if (RDP_EndTime.SelectedDate == null)
            {
                RAMSalesRankings.Alert("请选择截止日期！");
                return;
            }

            #region 选择系列时必须选择销售公司
            if (CkSeries.Checked)
            {
                if (string.IsNullOrEmpty(RCB_SaleFiliale.SelectedValue))
                {
                    RAMSalesRankings.Alert("请选择具体销售公司！");
                    return;
                }
                else if (RCB_SaleFiliale.SelectedValue.ToGuid().Equals(Guid.Empty))
                {
                    RAMSalesRankings.Alert("请选择具体销售公司！");
                    return;
                }
            }
            #endregion

            StartTime = RDP_StartTime.SelectedDate.Value;
            EndTime = RDP_EndTime.SelectedDate.Value;
            TopNum = string.IsNullOrEmpty(TB_TopNum.Text) ? 100 : Convert.ToInt32(TB_TopNum.Text);
            ClassId = new Guid(TVGoodsClass.SelectedNode.Value);
            BrandId = !string.IsNullOrEmpty(RCB_Brand.SelectedValue) ? new Guid(RCB_Brand.SelectedValue) : Guid.Empty;
            RGSGR.CurrentPageIndex = 0;
            RGSGR.Rebind();
        }

        /// <summary>
        /// 导出Excel事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_ExportData_Click(object sender, EventArgs e)
        {
            string fileName = Regex.Replace(TVGoodsClass.SelectedNode.Text, @"[\s\|\-\/\<>\*\?\\]", "") + StartTime.ToShortDateString() + "-" + EndTime.ToShortDateString() + "销量排行";
            fileName = Server.UrlEncode(fileName);
            RGSGR.ExportSettings.ExportOnlyData = true;
            RGSGR.HorizontalAlign = HorizontalAlign.Right;
            RGSGR.ExportSettings.IgnorePaging = true;
            RGSGR.ExportSettings.FileName = fileName;
            RGSGR.MasterTableView.ExportToExcel();
        }

        protected void OnCheckedChanged_ContainDisableSalePlatform(object sender, EventArgs e)
        {
            RCB_SalePlatform.Items.Clear();
            var filialeId = RCB_SaleFiliale.SelectedValue.ToGuid();
            if (filialeId == Guid.Empty)
            {
                return;
            }
            IList<SalePlatformInfo> salePlatformInfos = CB_ContainDisableSalePlatform.Checked ? CacheCollection.SalePlatform.GetListByFilialeId(filialeId) : CacheCollection.SalePlatform.GetListByFilialeId(filialeId).Where(ent => ent.IsActive).ToList();
            if (salePlatformInfos.Count == 0)
            {
                var filialeInfos = new List<FilialeInfo>();
                var filiale = CacheCollection.Filiale.GetChildShopList(filialeId).ToList();
                filialeInfos.AddRange(filiale);
                foreach (var filialeInfo in filiale)
                {
                    filialeInfos.AddRange(CacheCollection.Filiale.GetChildShopList(filialeInfo.ID).ToList());
                }
                RCB_SalePlatform.DataSource = CB_ContainDisableSalePlatform.Checked ? filialeInfos : filialeInfos.Where(ent => ent.IsActive).ToList();
                RCB_SalePlatform.DataBind();
            }
            else
            {
                RCB_SalePlatform.DataSource = salePlatformInfos;
                RCB_SalePlatform.DataBind();
            }
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部平台", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 生成销售信息数据,add by Li Zhongkai,2015-05-28
        /// </summary>
        protected void Rgsgr_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<SaleRaningShowInfo> dataSoure = new List<SaleRaningShowInfo>();
            if (IsPostBack)
            {
                var type = CblType.SelectedValue;
                var goodsName = TB_GoodsName.Text.Trim();
                var goodsCode = TB_GoodsCode.Text.Trim();
                if (goodsName == "支持模糊搜索") goodsName = string.Empty;
                if (goodsCode == "仅支持精确搜索") goodsCode = string.Empty;
                var saleType = type == "0" && CkSeries.Checked ? "3" : type;
                hfType.Value = saleType;
                hfParamter.Value = RCB_SaleFiliale.SelectedValue + "$" + (string.IsNullOrEmpty(RCB_SalePlatform.SelectedValue) ? string.Format("{0}", Guid.Empty) : RCB_SalePlatform.SelectedValue) + "$" +
                                   StartTime + "$" + EndTime;
                var goodsId = Guid.Empty;
                if (!string.IsNullOrEmpty(goodsCode))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoByCode(goodsCode);
                    if (goodsInfo != null) goodsId = goodsInfo.GoodsId;
                }
                var saleFilialeId = string.IsNullOrEmpty(RCB_SaleFiliale.SelectedValue) ? Guid.Empty : RCB_SaleFiliale.SelectedValue.ToGuid();
                var salePlatformId = string.IsNullOrEmpty(RCB_SalePlatform.SelectedValue) ? Guid.Empty : RCB_SalePlatform.SelectedValue.ToGuid();
                var list = _goodsRankingManager.GetGoodsSalesRanking(TopNum, ClassId, BrandId, goodsName, goodsId,
                                                                   saleFilialeId,
                                                                   salePlatformId, StartTime, EndTime,
                                                                   CB_ContainDisableSalePlatform.Checked, Convert.ToInt32(saleType));
                list = list.OrderByDescending(act => act.GoodsPrice).ToList();
                Dictionary<Guid,string> dics=new Dictionary<Guid, string>();
                switch (saleType)
                {
                    case "0"://按销量
                        dataSoure = list;
                        var goodsIds = dataSoure.Select(ent => ent.Id).Distinct().ToList();
                        var goodsInfos=_goodsCenterSao.GetGoodsListByGoodsIds(goodsIds).ToDictionary(k=>k.GoodsId,v=>v.GoodsName);
                        foreach (var item in list)
                        {
                            item.Name = goodsInfos.ContainsKey(item.Id)?goodsInfos[item.Id]: "";
                        }
                        break;
                    case "1"://按平台
                        var headList = CacheCollection.SalePlatform.GetList();
                        var filialeList = CacheCollection.Filiale.GetShopList();
                        foreach (var item in list)
                        {
                            var salePlatformInfo = headList.FirstOrDefault(act => act.ID == item.SalePlatformId);
                            if (salePlatformInfo == null)
                            {
                                var filialeInfo = filialeList.FirstOrDefault(act => act.ID == item.SalePlatformId);
                                item.Name = filialeInfo != null ? filialeInfo.Name : "ERP";
                            }
                            else
                            {
                                item.Name = salePlatformInfo.Name;
                            }
                        }
                        dataSoure = list;
                        break;
                    case "2"://按品牌
                        foreach (var item in list)
                        {
                            item.Name = GoodsBrandInfos.ContainsKey(item.BrandId) ? GoodsBrandInfos[item.BrandId] : string.Empty;
                        }
                        dataSoure = list;
                        break;
                    case "3"://按系列
                        var seriesList = list.Where(act => act.SeriesId != Guid.Empty).ToList();
                        dics = _goodsCenterSao.GetSeriesDict(saleFilialeId,seriesList.Select(act => act.SeriesId).ToList());
                        foreach (var item in list)
                        {
                            if (item.SeriesId != Guid.Empty)
                                item.Name = dics.ContainsKey(item.SeriesId) ? dics[item.SeriesId] : item.Name;
                        }
                        dataSoure = list;
                        break;
                    default:
                        dataSoure = list;
                        break;
                }
                
                #region 数据合计
                var txtMsg = RGSGR.MasterTableView.Columns.FindByUniqueName("Name");   //文本
                var preSalesNumber = RGSGR.MasterTableView.Columns.FindByUniqueName("PreSalesNumber");  //上一期销量合计
                var salesNumber = RGSGR.MasterTableView.Columns.FindByUniqueName("SalesNumber");
                var preSalesPrice = RGSGR.MasterTableView.Columns.FindByUniqueName("PreGoodsPrice");
                var salesPrice = RGSGR.MasterTableView.Columns.FindByUniqueName("GoodsPrice");
                var zeroNumber = RGSGR.MasterTableView.Columns.FindByUniqueName("ZeroNumber");
                var purchasePrice = RGSGR.MasterTableView.Columns.FindByUniqueName("PurchasePrice");
                txtMsg.FooterText = "汇总数据：";
                preSalesNumber.FooterText = dataSoure.Sum(ent => ent.PreSalesNumber).ToString("G");
                salesNumber.FooterText = dataSoure.Sum(ent => ent.SalesNumber).ToString("G");
                preSalesPrice.FooterText = dataSoure.Sum(ent => ent.PreGoodsPrice).ToString("N");
                salesPrice.FooterText = dataSoure.Sum(ent => ent.GoodsPrice).ToString("N");
                zeroNumber.FooterText = dataSoure.Sum(ent => ent.ZeroNumber).ToString("G");
                purchasePrice.FooterText = dataSoure.Sum(ent => ent.PurchasePrice).ToString("N");

                txtMsg.FooterStyle.HorizontalAlign = HorizontalAlign.Right;
                preSalesNumber.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                salesNumber.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                preSalesPrice.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                salesPrice.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                zeroNumber.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                purchasePrice.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
                #endregion
            }
            RGSGR.DataSource = dataSoure.OrderByDescending(act => act.SalesNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CblTypeSelectIndexChanged(object sender, EventArgs e)
        {
            var type = (RadioButtonList)sender;
            CkSeries.Visible = type.SelectedValue == "0";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SalesTotalData
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 上一期销量
        /// </summary>
        public int PreSalesNumber { get; set; }

        /// <summary>
        /// 本期销量(非0元)
        /// </summary>
        public int SalesNumber { get; set; }

        /// <summary>
        /// 0元销量，add by Li Zhongkai,2015-05-27
        /// </summary>
        public int ZeroNumber { get; set; }

        /// <summary>
        /// 销量增长率
        /// </summary>
        public string SalesNumIncrease { get; set; }

        /// <summary>
        /// 上一期销售额
        /// </summary>
        public decimal PreSalesPrice { get; set; }

        /// <summary>
        /// 本期销售额
        /// </summary>
        public decimal SalesPrice { get; set; }

        /// <summary>
        /// 销售额增长率
        /// </summary>
        public string SalesPriceIncrease { get; set; }

        /// <summary>
        /// 商品系列ID
        /// </summary>
        public Guid SeriesId { get; set; }
    }
}