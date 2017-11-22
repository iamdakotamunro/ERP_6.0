using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Goods;
using ERP.DAL.Interface.IGoods;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class MyGoodInfoList : BasePage
    {
        protected Dictionary<Guid, decimal> DictionaryLastPurchasePrice;
        protected Dictionary<Guid, decimal> DictionaryKeedePrice;
        readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();
        protected GoodsPriceDao GoodsPriceDao = new GoodsPriceDao(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeGoodsClass();
                GoodsClassId = Guid.Empty;
            }
        }

        public IList<GoodsSalePriceInfo> GoodsSalePriceList
        {
            set { ViewState["GoodsSalePriceList"] = value; }
            get
            {
                if (ViewState["GoodsSalePriceList"] == null) return new List<GoodsSalePriceInfo>();
                return (IList<GoodsSalePriceInfo>)ViewState["GoodsSalePriceList"];
            }
        }

        protected void NoCompareList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsInfo> goodsBaseList = new List<GoodsInfo>();
            var pageIndex = RG_NoCompareList.CurrentPageIndex + 1;
            int pageSize = RG_NoCompareList.PageSize;
            int totalCount = 0;
            if (IsPostBack)
            {
                goodsBaseList = _goodsCenterSao.GetGoodsListToPage(GoodsClassId, SearchGoodsName, null, null, null,pageIndex, pageSize, out totalCount);
                if (goodsBaseList.Count > 0)
                {
                    var goodsIdList = goodsBaseList.Select(w => w.GoodsId).ToList();
                    ReadFetchPrice(goodsIdList);
                    GoodsSalePriceList = _goodsCenterSao.GetGoodsPriceListByGoodsList(goodsIdList).ToList();
                    DictionaryLastPurchasePrice = new Dictionary<Guid, decimal>();
                    DictionaryKeedePrice = new Dictionary<Guid, decimal>();
                    foreach (var goodsId in goodsIdList)
                    {
                        DictionaryLastPurchasePrice.Add(goodsId, GetLastPurchasePrice(goodsId));
                        DictionaryKeedePrice.Add(goodsId, GetKeedePrice(goodsId));
                    }
                }
            }

            RG_NoCompareList.DataSource = goodsBaseList;
            RG_NoCompareList.VirtualItemCount = totalCount;
        }

        /// <summary>获得 最后进货价 或 可得价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="type">1最后进货价，2可得价</param>
        /// <returns></returns>
        protected decimal GetPrice(Guid goodsId, int type)
        {
            switch (type)
            {
                case 1://最后进货价
                    if (DictionaryLastPurchasePrice != null && goodsId != Guid.Empty)
                    {
                        var keyValuePair = DictionaryLastPurchasePrice.First(w => w.Key == goodsId);
                        return keyValuePair.Value;
                    }
                    break;
                case 2://可得价
                    if (DictionaryKeedePrice != null && goodsId != Guid.Empty)
                    {
                        var keyValuePair = DictionaryKeedePrice.First(w => w.Key == goodsId);
                        return keyValuePair.Value;
                    }
                    break;
            }
            return "0.00".ToDecimal();
        }

        protected decimal CompareDifferencePrice(Guid goodsId)
        {
            var list = GoodsFetchPriceList.Where(ent => ent.GoodsPrice > 0 && ent.GoodsId == goodsId).ToList();
            if (list.Any())
            {
                var minPrice = list.Min(ent => ent.GoodsPrice);
                return GetKeedePrice(goodsId) - minPrice;
            }
            return 0;
        }

        /// <summary> 利润率 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        protected string GetInterestRate(Guid goodsId)
        {
            var keekeprice = GetPrice(goodsId, 2);
            var lastPurchasePrice = GetPrice(goodsId, 1);
            if (lastPurchasePrice == 0) return "-";
            var price = keekeprice;
            var interestRate = (price - lastPurchasePrice) / lastPurchasePrice * 100;
            if (interestRate >= 0)
            {
                return Math.Round(interestRate, 2) + "%";
            }
            return "<font color='red'>" + Math.Round(interestRate, 2) + "%" + "</font>";
        }

        #region -- 获取抓取价
        protected IList<GoodsFetchPriceInfo> GoodsFetchPriceList { get; set; }
        private void ReadFetchPrice(List<Guid> goodsIdList)
        {
            var goodsIds = goodsIdList.Select(ent => ent.ToString()).ToList();

            IList<GoodsFetchPriceInfo> dataList;
            if (goodsIds.Count>0)
            {
                var goodsIdString = "'" + string.Join("','", goodsIds.ToArray()) + "'";
                dataList = GoodsPriceDao.GetFetchPriceList(goodsIdString);
            }
            else
            {
                dataList=new List<GoodsFetchPriceInfo>();
            }
            GoodsFetchPriceList = dataList;
        }
        protected string GetFetchPrice(int siteId, object goodsId)
        {
            var info = GoodsFetchPriceList.FirstOrDefault(ent => ent.SiteId == siteId && ent.GoodsId == goodsId.ToString().ToGuid());
            if (info != null)
            {
                return info.GoodsPrice.ToString("f2");
            }
            return "0.00";
        }
        #endregion


        #region -- html展示差异价格
        protected string ToHtmlDifferencePrice(object price)
        {
            if (price.ToString().ToDecimal() > 0)
            {
                return "<font color='red'>" + price + "</font>";
            }
            return "-";
        }
        #endregion

        /// <summary> 最后进货价 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        private decimal GetLastPurchasePrice(Guid goodsId)
        {
            //TODO WMS
            //if (GoodsStockCurrentInfoList.Count > 0)
            //{
            //    var list = GoodsStockCurrentInfoList.Where(w => w.GoodsId == goodsId).OrderByDescending(w => w.RecentCDate).ToList();
            //    if (list.Count > 0)
            //    {
            //        return list[0].LowestInPrice;
            //    }
            //}
            return "0.00".ToDecimal();
        }

        /// <summary> 可得价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        private decimal GetKeedePrice(Guid goodsId)
        {
            if (GoodsSalePriceList.Count > 0)
            {
                if (KdGoodsGroupInfo != null)
                {
                    var goodsSalePriceInfo = GoodsSalePriceList.FirstOrDefault(w => w.GoodsId == goodsId && w.GroupId == KdGoodsGroupInfo.GroupId);
                    if (goodsSalePriceInfo != null)
                        return goodsSalePriceInfo.Price;
                }
            }
            return "0.00".ToDecimal();
        }

        protected GoodsGroupInfo KdGoodsGroupInfo
        {
            get
            {
                if (ViewState["KdGoodsGroupInfo"] == null)
                {
                    var keedeFilialeInfo = CacheCollection.Filiale.Get("kede");
                    if (keedeFilialeInfo != null)
                    {
                        var salePlatformInfo = CacheCollection.SalePlatform.GetListByFilialeId(keedeFilialeInfo.ID).FirstOrDefault(ent => ent.ID == new Guid("3FE5AEF4-2CFD-4998-8D88-385321179B80"));
                        //var salePlatformInfo = salePlatformList.FirstOrDefault(w => w.Url.Contains("keede.com") || w.Url.Contains("kede.com"));
                        if (salePlatformInfo != null)
                        {
                            var goodsGroupInfo = _goodsCenterSao.GetGroupInfoBySalePlatformId(salePlatformInfo.ID);
                            if (goodsGroupInfo == null)
                                return null;
                            ViewState["KdGoodsGroupInfo"] = goodsGroupInfo;
                        }
                    }
                }
                return (GoodsGroupInfo)ViewState["KdGoodsGroupInfo"];
            }
        }

        protected void ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                var goodsTextbox = e.Item.FindControl("TBox_GoodsName") as RadTextBox;
                if (goodsTextbox != null)
                {
                    SearchGoodsName = goodsTextbox.Text.Trim();
                }
                RG_NoCompareList.Rebind();
            }
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_NoCompareList, e);
        }

        public string SearchGoodsName
        {
            set { ViewState["goodsname"] = value; }
            get
            {
                if (ViewState["goodsname"] == null) return string.Empty;
                return ViewState["goodsname"].ToString();
            }
        }

        #region[加载商品分类]

        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
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

        protected void TvGoodsClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            //GoodsName = null;
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                GoodsClassId = new Guid(e.Node.Value);
                RG_NoCompareList.Rebind();
            }
            else
            {
                GoodsClassId = Guid.Empty;
            }
        }

        protected Guid GoodsClassId
        {
            get { return new Guid(ViewState["GoodsClassId"].ToString()); }
            set { ViewState["GoodsClassId"] = value.ToString(); }
        }

        #endregion
    }
}
