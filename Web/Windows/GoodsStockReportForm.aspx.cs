using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Storage;
using ERP.SAL.Goods;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class GoodsStockReportForm : WindowsPage
    {

        private static readonly GoodsStockSettleRecordBll _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(new GoodsStockRecordDao(), new GoodsCenterSao());
        public int GoodsType
        {
            get
            {
                return Convert.ToInt32(Request.QueryString["GoodsType"]);
            }
        }

        public int Year
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["Year"])?DateTime.Now.Year:
                    Convert.ToInt32(Request.QueryString["Year"]);
            } 
        }

        public int Month
        {
            get
            {
                if (ViewState["Month"]==null)
                {
                    return string.IsNullOrEmpty(Request.QueryString["Month"]) ? DateTime.Now.Year :
                        Convert.ToInt32(Request.QueryString["Month"]);
                }
                return Convert.ToInt32(ViewState["Month"]);
            }
            set
            {
                ViewState["Month"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgGoodsStockOnItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "Search") return;
            Month = Convert.ToInt32(((RadComboBox)e.Item.FindControl("RcbMonth")).SelectedValue);
            RgGoodsStock.CurrentPageIndex = 0;
            RgGoodsStock.Rebind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgGoodsStockNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var dataList = _goodsStockSettleRecordBll.SelectMonthGoodsStockRecordInfos(GoodsType, Year, Month);
            RgGoodsStock.DataSource = dataList.OrderByDescending(act => act.TotalAmount).ToList();
        }

        /// <summary>
        /// 加载月份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgGoodsStockOnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var rcbMonth = e.Item.FindControl("RcbMonth") as RadComboBox;
                if (rcbMonth != null)
                {
                    //加载年份
                    for (var i = 1; i <= 12; i++)
                    {
                        rcbMonth.Items.Add(new RadComboBoxItem(string.Format("{0}月份", i), string.Format("{0}", i)));
                    }
                    rcbMonth.SelectedValue = string.Format("{0}", Month);
                }
            }
        }

        protected void IbExportOnClick(object sender, EventArgs e)
        {
            string fileName = string.Format("{0}{1}月份商品月末库存金额详细", Year, Month);
            fileName = Server.UrlEncode(fileName);
            RgGoodsStock.ExportSettings.ExportOnlyData = true;
            RgGoodsStock.ExportSettings.IgnorePaging = true;
            RgGoodsStock.ExportSettings.FileName = fileName;
            RgGoodsStock.MasterTableView.ExportToExcel();
        }
    }
}