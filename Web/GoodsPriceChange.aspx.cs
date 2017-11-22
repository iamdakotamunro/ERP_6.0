using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/7/17  
     * 描述    :商品改价记录
     * =====================================================================
     * 修改时间：2015/7/17  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class GoodsPriceChange : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IGoodsPriceChange _goodsPriceChange = new GoodsPriceChangeDal(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    //LoadSaleFiliale();
            //}
        }

        #region 加载销售公司和销售平台

        //private void LoadSaleFiliale()
        //{
        //    rcb_SaleFiliale.DataSource = CacheCollection.Filiale.GetHeadList();
        //    rcb_SaleFiliale.DataTextField = "Name";
        //    rcb_SaleFiliale.DataValueField = "ID";
        //    rcb_SaleFiliale.DataBind();
        //    rcb_SaleFiliale.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
        //    rcb_SaleFiliale.SelectedIndex = 0;
        //}
        /// <summary>
        /// 加载销售公司
        /// </summary>
        /// <summary>
        /// 销售公司OnSelectedIndexChanged事件
        /// </summary>
        //protected void rcb_SaleFiliale_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        //{
        //    var rcbSaleFilialeId = new Guid(rcb_SaleFiliale.SelectedValue);

        //    rcb_SalePlatform.Items.Clear();
        //    rcb_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId);
        //    rcb_SalePlatform.DataTextField = "Name";
        //    rcb_SalePlatform.DataValueField = "ID";
        //    rcb_SalePlatform.DataBind();
        //    rcb_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
        //    rcb_SalePlatform.SelectedIndex = 0;
        //}

        #endregion

        #region 绑定数据源
        protected void RG_GoodsPriceChange_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var dataList = new List<Model.GoodsPriceChange>();
            if (IsPostBack)
            {
                var startPage = RG_GoodsPriceChange.CurrentPageIndex + 1;
                int pageSize = RG_GoodsPriceChange.PageSize;
                var goodsId = Guid.Empty;
                if (!string.IsNullOrWhiteSpace(txt_GoodsCode.Text))
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoByCode(txt_GoodsCode.Text);
                    if (goodsInfo != null)
                    {
                        goodsId = goodsInfo.GoodsId;
                    }
                }
                int total;
                dataList = _goodsPriceChange.GetAllGoodsPriceChange(null, null, txt_GoodsName.Text, goodsId, int.Parse(rcb_Type.SelectedValue), startPage, pageSize, out total);
                RG_GoodsPriceChange.VirtualItemCount = total;
            }
            RG_GoodsPriceChange.DataSource = dataList;
        }
        #endregion

        //搜索
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RG_GoodsPriceChange.CurrentPageIndex = 0;
            RG_GoodsPriceChange.Rebind();
        }

        public string GetPriceType(string type)
        {
            string typeName = string.Empty;
            switch (type)
            {
                case "0":
                    typeName = "销售价";
                    break;
                case "1":
                    typeName = "加盟价";
                    break;
                case "2":
                    typeName = "批发价";
                    break;

            }
            return typeName;
        }
    }
}