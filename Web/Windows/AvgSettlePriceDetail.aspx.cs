using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IStorage;
using System;
using System.Web.UI;
using ERP.BLL.Implement;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AvgSettlePriceDetail : Page
    {
        static readonly RealTimeGrossSettlementManager realTimeGrossSettlementManager = new RealTimeGrossSettlementManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lit_GoodsName.Text = Request.QueryString["GoodsName"];
            }
        }

        #region 数据列表相关
        protected void RG_AvgSettlePriceDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            if (String.IsNullOrWhiteSpace(Request.QueryString["HostingFilialeId"])) 
            {
                RAM.Alert("请选择公司!");
                return;
            }
            RG_AvgSettlePriceDetail.DataSource =
                realTimeGrossSettlementManager.GetArchivedUnitPriceHistoryList(
                    new Guid(Request.QueryString["HostingFilialeId"]), new Guid(Request.QueryString["GoodsId"]));
        }
        #endregion
    }
}