using System;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model.ShopFront;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 新需求  针对门店采购申请发货确认 需确认商品提示
    /// add by liangcanren at 2015-04-15
    /// </summary>
    public partial class ShopConfirmApplyDetailForm : WindowsPage
    {
        private static readonly IApplyStockDAL _applyStock = new ApplyStockDAL(GlobalConfig.DB.FromType.Write);
        private readonly ApplyStockBLL _applyStockBll = new ApplyStockBLL(_applyStock);

        /// <summary>
        /// 重复点击
        /// </summary>
        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        /// <summary>
        /// 采购申请ID
        /// </summary>
        protected Guid ApplyId
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ApplyId"]))
                {
                    return new Guid(Request.QueryString["ApplyId"]);
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 当前采购单
        /// </summary>
        protected ApplyStockInfo CurrentStockInfo
        {
            get
            {
                if (ViewState["CurrentStockInfo"] == null)
                {
                    ViewState["CurrentStockInfo"] = _applyStock.FindById(ApplyId);
                }
                return ViewState["CurrentStockInfo"] as ApplyStockInfo;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据源绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgApplyStockDetailOnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var applyStockDetailList = _applyStockBll.FindDetailList(ApplyId);
            if(applyStockDetailList==null)return;
            RgApplyStockDetail.DataSource = applyStockDetailList;
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgApplyStockDetailItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (e.Item.RowIndex > 1)
                {
                    var applygoodsinfo = (ApplyStockDetailInfo)e.Item.DataItem;

                    if (applygoodsinfo.IsComfirmed)  //需确认时标红
                    {
                        e.Item.Style.Add("background-color", "#FF6666");
                    }
                }
            }
        }

        /// <summary>
        /// 采购确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IbcConfirmOnClick(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                RAM.Alert("程序正在处理中，请稍候...");
                return;
            }
            var applyInfo = _applyStock.FindById(ApplyId);
            if (applyInfo==null)
            {
                RAM.Alert("未找到该采购申请单据!");
                return;
            }
            if (applyInfo.StockState>(int)ApplyStockState.Confirming)
            {
                RAM.Alert("该要货申请单据状态已修改!");
                return;
            }
            string errorMsg;
            var applyStockBll=new ApplyStockBLL(_applyStock,new GoodsCenterSao(),
                new PurchaseSet(GlobalConfig.DB.FromType.Write),new ShopExchangedApplyDetailDal(GlobalConfig.DB.FromType.Write) );
            var result = applyStockBll.UpdateStockStateErp(ApplyId, (int)ApplyStockState.Delivering, out errorMsg);
            if (!result)
            {
                RAM.Alert(errorMsg);
            }
            else
            {
                RAM.Alert("要货确认成功!");
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
    }
}