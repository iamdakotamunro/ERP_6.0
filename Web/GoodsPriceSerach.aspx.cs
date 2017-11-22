using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>商品价格查询  ADD 2014-08-08 陈重文
    /// </summary>
    public partial class GoodsPriceSerach : BasePage
    {
        private Dictionary<Guid, List<GroupGoodsPriceModel>> _dicGroupGoodsPrice;
        readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeGoodsClass();
                BrandSource();
            }
            else
            {
                RAM.ResponseScripts.Add("removeClass();");
            }
        }

        #region[加载商品分类]

        /// <summary>创建商品分类树
        /// </summary>
        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
        }

        /// <summary>遍历产品分类
        /// </summary>
        private void RecursivelyGoodsClass(Guid goodsClassId, IRadTreeNodeContainer node, IEnumerable<GoodsClassInfo> goodsClassList)
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

        /// <summary>创建节点
        /// </summary>
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        /// <summary>商品分类树点击事件
        /// </summary>
        protected void TvGoodsClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            SearchGoodsNameOrCode = string.Empty;
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                GoodsClassId = new Guid(e.Node.Value);
                RG_GoodsPriceList.CurrentPageIndex = 0;
                RG_GoodsPriceList.Rebind();
            }
            else
            {
                GoodsClassId = Guid.Empty;
            }
        }

        #endregion

        #region 商品品牌
        /// <summary>
        /// zal 2015-07-15
        /// </summary>
        private void BrandSource()
        {
            rcb_Brand.DataSource = _goodsCenterSao.GetAllBrandList();
            rcb_Brand.DataTextField = "Brand";
            rcb_Brand.DataValueField = "BrandId";
            rcb_Brand.DataBind();
            rcb_Brand.Items.Insert(0, new RadComboBoxItem("所有品牌", Guid.Empty.ToString()));
        }
        /// <summary>
        /// 自动查询
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// zal 2015-08-05
        protected void rcb_Brand_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text.Trim()))
            {
                var brandList = _goodsCenterSao.GetAllBrandList().Where(p => p.Brand.Contains(e.Text.Trim())).ToList();
                if (e.NumberOfItems >= brandList.Count())
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in brandList)
                    {
                        combo.Items.Add(new RadComboBoxItem(item.Brand, item.BrandId.ToString()));
                    }
                }
            }
        }
        #endregion

        #region [绑定数据源]
        /// <summary>
        /// 商品价格数据源绑定
        /// </summary>
        protected void RG_GoodsPriceList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsPriceSerachInfo> goodsPriceSerachList = null;
            if (IsPostBack)
            {
                var startPage = RG_GoodsPriceList.CurrentPageIndex + 1;
                int pageSize = RG_GoodsPriceList.PageSize;
                int recordCount;

                Guid? goodsClassId = null, brandId = Guid.Empty;
                if (GoodsClassId != Guid.Empty)
                {
                    goodsClassId = GoodsClassId;
                }
                if (!string.IsNullOrEmpty(rcb_Brand.SelectedValue))
                {
                    brandId = new Guid(rcb_Brand.SelectedValue);
                }
                SearchGoodsNameOrCode = string.Empty;
                if (!string.IsNullOrEmpty(txt_GoodsNameOrCode.Text))
                {
                    SearchGoodsNameOrCode = txt_GoodsNameOrCode.Text.Trim();
                }
                goodsPriceSerachList = _goodsCenterSao.GetGoodsPriceGridByPage(goodsClassId, brandId, SearchGoodsNameOrCode, startPage, pageSize, out recordCount);
                if (goodsPriceSerachList != null && goodsPriceSerachList.Count > 0)
                {
                    var goodsIds = goodsPriceSerachList.Select(ent => ent.GoodsID);
                    DicGoodsPurchasePrice = _purchaseSet.GetPurchaseSetList(goodsIds.ToList(), Guid.Empty).ToDictionary(ent => ent.GoodsId, ent => ent.PurchasePrice);
                    _dicGroupGoodsPrice = _goodsCenterSao.GetGroupGoodsPriceDictionary(goodsPriceSerachList.Select(p => p.GoodsID).ToList());
                }

                RG_GoodsPriceList.VirtualItemCount = recordCount;
            }
            if (goodsPriceSerachList == null)
            {
                goodsPriceSerachList = new List<GoodsPriceSerachInfo>();
            }
            RG_GoodsPriceList.DataSource = goodsPriceSerachList;
        }
        #endregion

        #region [搜索]
        //搜索事件 zal 2015-07-15                                                                                             
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RG_GoodsPriceList.Rebind();
            if (GoodsClassId == Guid.Empty && !string.IsNullOrEmpty(rcb_Brand.SelectedValue) && !new Guid(rcb_Brand.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择“商品分类”！");
            }
        }
        #endregion

        /// <summary>
        /// 增加各个平台的价格显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// zal 2015-07-15
        protected void RG_GoodsPriceList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var groupDetailModelList = _goodsCenterSao.GetGroupDetailList().Where(p => p.GroupID != new Guid("C27139B4-083F-4473-95BA-C86648901FAD"));
            var salePlatformInfoList = CacheCollection.SalePlatform.GetList();
            #region 标题头
            if (e.Item.ItemType == GridItemType.Header)
            {
                var row1 = "<tr>";
                var row2 = "<tr>";
                int i = 0;
                foreach (var model in groupDetailModelList)
                {
                    var officialSalePlatformIDs = model.OfficialSalePlatformIDs.Where(o => o != new Guid("63710AD6-A235-458F-BFCF-1BFEE63CAB35"));
                    var colspan = officialSalePlatformIDs.Count() + model.ThirdSalePlatformIDs.Count;
                    row1 += "<td class='Group' style='padding-top:5px; padding-bottom:5px; border-bottom:1px solid #3d556c;" + (i == 0 ? " border-left:0px;" : " border-left:1px solid #3d556c;") + "'  colspan=\"" + colspan + "\">" + model.Name + "</td>";
                    foreach (var item in officialSalePlatformIDs)
                    {
                        var name =
                            salePlatformInfoList.Where(p => p.ID == new Guid(item.ToString()))
                                .Select(p => p.ExternalName)
                                .FirstOrDefault();
                        row2 += "<td class='title' style='padding-top:5px; padding-bottom:5px; " + (i == 0 ? " border-left:0px;" : "border-left:1px solid #3d556c;") + "'>" + (name != null && name.Length >= 5 ? name : name + "价") + "</td>";
                    }
                    foreach (var item in model.ThirdSalePlatformIDs)
                    {
                        var name =
                            salePlatformInfoList.Where(p => p.ID == new Guid(item.ToString()))
                                .Select(p => p.ExternalName)
                                .FirstOrDefault();
                        row2 += "<td class='title' style='padding-top:5px; padding-bottom:5px; " + ((i == 0 && officialSalePlatformIDs.Count() == 0)
                            ? " border-left:0px;" : "border-left:1px solid #3d556c;") + "'>" + (name != null && name.Length >= 5 ? name : name + "价") + "</td>";
                    }
                    i++;
                }
                row1 += "</tr>";
                row2 += "</tr>";
                var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row1 + row2 + "</table>";
                e.Item.Cells[6].Text = headerText;
            }
            #endregion

            #region 行数据
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var goodsId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("GoodsID");
                string str = "<table cellspacing='0' cellpadding='0' style=\"width: 100%; text-align:center;\"><tr>";
                int i = 0;
                foreach (var model in groupDetailModelList)
                {
                    var groupGoodsPriceModel = _dicGroupGoodsPrice[goodsId].Where(p => p.GroupInfo.GroupID == model.GroupID).ToList();
                    //官网价
                    var officialPrice = groupGoodsPriceModel.Select(p => p.Price).FirstOrDefault();
                    var thirdPriceList = groupGoodsPriceModel.Select(p => p.ThirdPriceList).FirstOrDefault();
                    var officialSalePlatformIDs = model.OfficialSalePlatformIDs.Where(o => o != new Guid("63710AD6-A235-458F-BFCF-1BFEE63CAB35"));

                    foreach (var item in officialSalePlatformIDs)
                    {
                        var salefilialeId =
                            salePlatformInfoList.Where(p => p.ID == new Guid(item.ToString()))
                                .Select(p => p.FilialeId)
                                .FirstOrDefault();

                        if (officialPrice.Equals(0))
                        {
                            str += "<td>0.00</td>";
                        }
                        else
                        {
                            str += "<td><span style='text-decoration: underline; cursor: pointer' onclick=\"ProductSales('" + goodsId + "','" + salefilialeId + "','" + item + "')\">" + officialPrice + "</span></td>";
                        }
                    }

                    foreach (var item in model.ThirdSalePlatformIDs)
                    {
                        if (thirdPriceList != null)
                        {
                            var salefilialeId =
                               salePlatformInfoList.Where(p => p.ID == new Guid(item.ToString()))
                                   .Select(p => p.FilialeId)
                                   .FirstOrDefault();

                            var price = thirdPriceList.Where(p => p.SalePlatformId == item)
                                .Select(p => p.Price)
                                .FirstOrDefault();

                            if (price.Equals(0))
                            {
                                if (officialPrice.Equals(0))
                                {
                                    str += "<td>0.00</td>";
                                }
                                else
                                {
                                    str += "<td><span style='text-decoration: underline; cursor: pointer' onclick=\"ProductSales('" + goodsId + "','" + salefilialeId + "','" + item + "')\">" + officialPrice + "</span></td>";
                                }
                            }
                            else
                            {
                                str += "<td><span style='text-decoration: underline; cursor: pointer' onclick=\"ProductSales('" + goodsId + "','" + salefilialeId + "','" + item + "')\">" + price + "</span></td>";
                            }
                        }
                        else
                        {
                            str += "<td>0.00</td>";
                        }
                    }
                    i++;
                }
                str += "</tr></table>";
                e.Item.Cells[6].Text = str;
            }
            #endregion
        }

        #region [获取商品的采购价]
        /// <summary>获取商品的采购价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        protected decimal GetGoodsPurchasePrice(Guid goodsId)
        {
            return goodsId != Guid.Empty ? DicGoodsPurchasePrice.FirstOrDefault(ent => ent.Key == goodsId).Value : 0;
        }
        #endregion

        #region [ViewState]

        /// <summary>搜索的商品名称或编号
        /// </summary>
        public string SearchGoodsNameOrCode
        {
            set { ViewState["GoodsNameOrCode"] = value; }
            get
            {
                return ViewState["GoodsNameOrCode"] == null ? string.Empty : ViewState["GoodsNameOrCode"].ToString();
            }
        }

        /// <summary> 商品分类ID
        /// </summary>
        protected Guid GoodsClassId
        {
            get
            {
                return ViewState["GoodsClassId"] == null ? Guid.Empty : new Guid(ViewState["GoodsClassId"].ToString());
            }
            set { ViewState["GoodsClassId"] = value.ToString(); }
        }

        protected Dictionary<Guid, decimal> DicGoodsPurchasePrice
        {
            get
            {
                if (ViewState["DicGoodsPurchasePrice"] == null) return new Dictionary<Guid, decimal>();
                return (Dictionary<Guid, decimal>)ViewState["DicGoodsPurchasePrice"];
            }
            set { ViewState["DicGoodsPurchasePrice"] = value; }
        }

        #endregion
    }
}