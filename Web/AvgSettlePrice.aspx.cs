using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.Model.Goods;
using Telerik.Web.UI;
using ERP.SAL.Interface;
using ERP.SAL.Goods;
using ERP.DAL.Interface.IStorage;
using ERP.DAL.Implement.Storage;
using ERP.Model;
using ERP.SAL;
using MIS.Enum;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;

namespace ERP.UI.Web
{
    public partial class AvgSettlePrice : BasePage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly IGoodsStockRecord _goodsStockRecord = new GoodsStockRecordDao();
        static readonly RealTimeGrossSettlementManager realTimeGrossSettlementManager = new RealTimeGrossSettlementManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeGoodsClass();//加载商品分类
                txt_YearMonth.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                HostingFiliale();
            }
        }

        #region 数据准备
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
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                Hid_GoodsClassId.Value = e.Node.Value;
                RG_AvgSettlePrice.CurrentPageIndex = 0;
                RG_AvgSettlePrice.Rebind();
            }
            else
            {
                Hid_GoodsClassId.Value = Guid.Empty.ToString();
            }
        }

        #endregion
        #endregion

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DDL_HostingFilialeAuth.SelectedValue) || DDL_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择公司！");
                return;
            }
            GridDataBind();
            RG_AvgSettlePrice.CurrentPageIndex = 0;
            RG_AvgSettlePrice.DataBind();
        }

        #region 数据列表相关
        protected void RG_AvgSettlePrice_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (!IsPostBack)
            {
                var query = from goodsInfo in new List<GoodsInfo>()
                            let goodsStockPriceRecordInfo = new GoodsStockPriceRecordInfo()
                            select new
                            {
                                goodsInfo.GoodsId,
                                goodsInfo.GoodsName,
                                goodsInfo.GoodsCode,
                                goodsStockPriceRecordInfo.DayTime,
                                goodsStockPriceRecordInfo.AvgSettlePrice
                            };

                RG_AvgSettlePrice.DataSource = query.ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(DDL_HostingFilialeAuth.SelectedValue) || DDL_HostingFilialeAuth.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择公司！");
                    return;
                }
                GridDataBind();
            }
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(new Guid(Hid_GoodsClassId.Value), txt_GoodsNameOrCode.Text.Trim());
            var goodsStockPriceRecordList =
                realTimeGrossSettlementManager.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(
                    new Guid(DDL_HostingFilialeAuth.SelectedItem.Value), goodsInfoList.Select(ent => ent.GoodsId), Convert.ToDateTime(txt_YearMonth.Text).AddMonths(1).AddSeconds(-1));
            var query = from goodsInfo in goodsInfoList
                        join priceInfo in goodsStockPriceRecordList on goodsInfo.GoodsId equals priceInfo.Key
                        select new
                        {
                            goodsInfo.GoodsId,
                            goodsInfo.GoodsName,
                            goodsInfo.GoodsCode,
                            DayTime = txt_YearMonth.Text,//日期
                            AvgSettlePrice = priceInfo.Value.ToString("F2")
                        };

           
            RG_AvgSettlePrice.DataSource = query.ToList();
        }
        #endregion


        #region 下拉列表
        /// <summary>
        /// 物流配送公司
        /// </summary>
        private void HostingFiliale()
        {
            var filiales = MISService.GetAllFiliales().Where(act => act.IsActive && (act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany)) || act.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            var list = filiales.Select(filiale => new HostingFilialeAuth
            {
                HostingFilialeId = filiale.ID,
                HostingFilialeName = filiale.Name
            }).ToList();
            DDL_HostingFilialeAuth.DataSource = list;
            DDL_HostingFilialeAuth.DataTextField = "HostingFilialeName";
            DDL_HostingFilialeAuth.DataValueField = "HostingFilialeId";
            DDL_HostingFilialeAuth.DataBind();
            DDL_HostingFilialeAuth.Items.Insert(0, new ListItem("", ""));
        }
        #endregion
    }
}