using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/09/10  
     * 描述    :设置商品加盟价批发价
     * =====================================================================
     * 修改时间：2015/09/10   
     * 修改人  ：  
     * 描述    ：
     */
    public partial class SetAffiliateAndShopPrice : BasePage
    {

        //获取商品采购设置
        protected Dictionary<Guid, decimal> DicGoodsPurchasePrice = new Dictionary<Guid, decimal>();

        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        static readonly IGoodsPriceChange _goodsPriceChange = new GoodsPriceChangeDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeGoodsClass();
            }
        }

        #region[加载商品分类]
        /// <summary>
        /// 创建商品分类树
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

        /// <summary>
        /// 遍历产品分类
        /// </summary>
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

        /// <summary>
        /// 创建节点
        /// </summary>
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        /// <summary>
        /// 商品分类树点击事件
        /// </summary>
        protected void TvGoodsClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
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

                goodsPriceSerachList = _goodsCenterSao.GetGoodsPriceGridByPage(goodsClassId, brandId, txt_GoodsNameOrCode.Text, startPage, pageSize, out recordCount);
                if (goodsPriceSerachList != null && goodsPriceSerachList.Count > 0)
                {
                    var goodsIds = goodsPriceSerachList.Select(ent => ent.GoodsID);
                    DicGoodsPurchasePrice = _purchaseSet.GetPurchaseSetList(goodsIds.ToList(),
                        new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")).ToDictionary(ent => ent.GoodsId, ent => ent.PurchasePrice);
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RG_GoodsPriceList.Rebind();
        }
        #endregion

        #region 修改价格
        //修改加盟价
        protected void txt_ChangeJoinPrice_OnTextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var joinPriceNew = Convert.ToDecimal(textBox.Text);
                try
                {
                    string errorMsg;
                    bool result = _goodsCenterSao.UpdateJoinPrice(goodsId, joinPriceNew, out errorMsg);
                    if (result)
                    {
                        var goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString();
                        var goodsName = dataItem.GetDataKeyValue("GoodsName").ToString();
                        var joinPriceOld = Convert.ToDecimal(dataItem.GetDataKeyValue("JoinPrice").ToString());
                        if (!joinPriceOld.Equals(joinPriceNew))
                        {
                            var goodsPriceChange = new Model.GoodsPriceChange
                            {
                                Id = Guid.NewGuid(),
                                Name = CurrentSession.Personnel.Get().RealName,
                                Datetime = DateTime.Now,
                                GoodsId = goodsId,
                                GoodsName = goodsName,
                                GoodsCode = goodsCode,
                                SaleFilialeId = Guid.Empty,
                                SaleFilialeName = string.Empty,
                                SalePlatformId = Guid.Empty,
                                SalePlatformName = string.Empty,
                                OldPrice = joinPriceOld,
                                NewPrice = joinPriceNew,
                                Quota = joinPriceOld - joinPriceNew,
                                Type = 1
                            };
                            _goodsPriceChange.AddGoodsPriceChange(goodsPriceChange);
                        }
                        RG_GoodsPriceList.Rebind();
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
            }
        }

        //修改批发价
        protected void txt_ChangeWholesalePrice_OnTextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var wholesalePriceNew = Convert.ToDecimal(textBox.Text);
                try
                {
                    string errorMsg;
                    bool result = _goodsCenterSao.UpdateWholesalePrice(goodsId, wholesalePriceNew, out errorMsg);
                    if (result)
                    {
                        var goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString();
                        var goodsName = dataItem.GetDataKeyValue("GoodsName").ToString();
                        var wholesalePriceOld = Convert.ToDecimal(dataItem.GetDataKeyValue("WholesalePrice").ToString());

                        var goodsPriceChange = new Model.GoodsPriceChange
                        {
                            Id = Guid.NewGuid(),
                            Name = CurrentSession.Personnel.Get().RealName,
                            Datetime = DateTime.Now,
                            GoodsId = goodsId,
                            GoodsName = goodsName,
                            GoodsCode = goodsCode,
                            SaleFilialeId = Guid.Empty,
                            SaleFilialeName = string.Empty,
                            SalePlatformId = Guid.Empty,
                            SalePlatformName = string.Empty,
                            OldPrice = wholesalePriceOld,
                            NewPrice = wholesalePriceNew,
                            Quota = wholesalePriceOld - wholesalePriceNew,
                            Type = 2
                        };
                        _goodsPriceChange.AddGoodsPriceChange(goodsPriceChange);
                        RG_GoodsPriceList.Rebind();
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
            }
        }
        #endregion

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

        #region 自定义属性
        /// <summary> 
        /// 商品分类ID
        /// </summary>
        protected Guid GoodsClassId
        {
            get
            {
                return ViewState["GoodsClassId"] == null ? Guid.Empty : new Guid(ViewState["GoodsClassId"].ToString());
            }
            set { ViewState["GoodsClassId"] = value.ToString(); }
        }
        #endregion
    }
}