using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Enum.ShopFront;
using ERP.Model;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class RequirementOrder : WindowsPage
    {
        protected IList<RequirementOrderInfo> DemandQuantityList = new List<RequirementOrderInfo>();

        protected string GoodsId
        {
            get { return Request.QueryString["GoodsId"]; }
        }

        protected string Type
        {
            get { return Request.QueryString["type"]; }
        }

        protected IDictionary<Guid, IList<OperationLogInfo>> DicOrderClew
        {
            get
            {
                if (ViewState["DicOrderClew"] == null) return new Dictionary<Guid, IList<OperationLogInfo>>();
                return (Dictionary<Guid, IList<OperationLogInfo>>)ViewState["DicOrderClew"];
            }
            set { ViewState["DicOrderClew"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    if (Type != null)
            //    {
            //        Guid warehouseId = string.IsNullOrEmpty(Request.QueryString["wareHouseId"]) ? Guid.Empty : new Guid(Request.QueryString["wareHouseId"]);
            //        if (Type == "1")
            //        {
            //            //订单
            //            rgDemandQuantity.Visible = false;

            //            if (!string.IsNullOrEmpty(GoodsId))
            //            {
            //                IList<GoodsOrderInfo> goodsOrderList = _goodsOrder.GetOrderListById(new Guid(GoodsId), warehouseId);
            //                var orderIdList = goodsOrderList.Select(w => w.OrderId).ToList();
            //                if (orderIdList.Count > 0)
            //                {
            //                    DicOrderClew = _operationLogManager.GetOperationLogList(orderIdList);
            //                }

            //                RGGoodsOrder.DataSource = goodsOrderList;
            //                RG_LensProcess.DataSource = _lensProcess.GetLensProcessList(new Guid(GoodsId), warehouseId);
            //                RG_ApplyStockList.DataSource = _applyStockDal.GetList(new Guid(GoodsId), warehouseId);
            //            }
            //            else
            //            {
            //                RGGoodsOrder.DataSource = new List<GoodsOrderInfo>();
            //                RG_LensProcess.DataSource = new List<LensProcessInfo>();
            //                RG_ApplyStockList.DataSource = new List<ApplyStockInfo>();
            //            }
            //            RGGoodsOrder.DataBind();
            //            RG_LensProcess.DataBind();
            //            RG_ApplyStockList.DataBind();
            //        }
            //        else if (Type == "2")
            //        {
            //            //占用库存
            //            RGGoodsOrder.Visible = false;

            //            if (!string.IsNullOrEmpty(GoodsId))
            //            {
            //                DemandQuantityList = _goodsOrder.GetDemandQuantity(new Guid(GoodsId), warehouseId).ToList();
            //            }
            //            else
            //            {
            //                rgDemandQuantity.DataSource = new List<GoodsOrderInfo>();
            //            }
            //            rgDemandQuantity.DataBind();
            //        }
            //        else
            //        {
            //            if (!string.IsNullOrEmpty(GoodsId))
            //            {
            //                IList<GoodsOrderInfo> goodsOrderList = _goodsOrder.GetOrderListById(new Guid(GoodsId), warehouseId);
            //                var orderIdList = goodsOrderList.Select(w => w.OrderId).ToList();
            //                if (orderIdList.Count > 0)
            //                {
            //                    DicOrderClew = _operationLogManager.GetOperationLogList(orderIdList);
            //                }

            //                RGGoodsOrder.DataSource = goodsOrderList;
            //                RG_LensProcess.DataSource = _lensProcess.GetLensProcessList(new Guid(GoodsId), warehouseId);
            //                RG_ApplyStockList.DataSource = _applyStockDal.GetList(new Guid(GoodsId), warehouseId);

            //                rgDemandQuantity.DataSource = _goodsOrder.GetDemandQuantity(new Guid(GoodsId), warehouseId);
            //            }
            //            else
            //            {
            //                RGGoodsOrder.DataSource = new List<GoodsOrderInfo>();
            //                RG_LensProcess.DataSource = new List<LensProcessInfo>();
            //                RG_ApplyStockList.DataSource = new List<ApplyStockInfo>();

            //                rgDemandQuantity.DataSource = new List<GoodsOrderInfo>();
            //            }
            //            RGGoodsOrder.DataBind();
            //            RG_LensProcess.DataBind();
            //            RG_ApplyStockList.DataBind();

            //            rgDemandQuantity.DataBind();
            //        }
            //    }

            //}
        }

        protected void RgDemandQuantity_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgDemandQuantity.DataSource = DemandQuantityList;
        }

        protected string GetOrderState(Object orderState)
        {
            return EnumAttribute.GetKeyName((OrderState)orderState);
        }

        /// <summary>
        /// 得到采购类型
        /// </summary>
        /// <param name="purchaseType"></param>
        /// <returns></returns>
        protected string GetPurchaseTypeName(object purchaseType)
        {
            if (purchaseType == null || purchaseType.ToString() == "0" || string.IsNullOrEmpty(purchaseType.ToString()))
            {
                return "-";
            }
            return EnumAttribute.GetKeyName((PurchaseType)purchaseType);
        }

        /// <summary>
        /// zhangfan added
        /// 返回采购状态描述
        /// </summary>
        /// <param name="enumKey"></param>
        /// <returns></returns>
        protected string ReturnApplyState(object enumKey)
        {
            int key;
            if (enumKey != null && int.TryParse(enumKey.ToString(), out key))
                return ApplyStockStateList.FirstOrDefault(ent => ent.Key == key).Value;
            return string.Empty;
        }

        /// <summary>
        /// zhangfan added
        /// 门店采购申请状态
        /// </summary>
        private IDictionary<int, string> ApplyStockStateList
        {
            get
            {
                if (ViewState["ShopApplyStateList"] == null)
                {
                    ViewState["ShopApplyStateList"] = EnumAttribute.GetDict<ApplyStockState>();
                }
                return ViewState["ShopApplyStateList"] as IDictionary<int, string>;
            }
        }

        protected string GetPayMode(Object payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }


        protected string GetMisClew(object orderNo)
        {
            if (orderNo != null)
            {
                string order = orderNo.ToString();
                if (DicOrderClew.Count == 0) return "用户下单";
                var list = DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime);
                string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                return str;
            }
            return string.Empty;
        }

        protected string GetType(Object type)
        {
            string typeName = string.Empty;
            switch ((int)type)
            {
                case 1:
                    typeName = "订单";
                    break;
                case 2:
                    typeName = "加工单";
                    break;
                case 3:
                    typeName = "调拨单";
                    break;
            }
            return typeName;
        }

        /// <summary> 绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Guid filialeId = Request.QueryString["FilialeId"] == null
                                 ? Guid.Empty
                                 : new Guid(Request.QueryString["FilialeId"]);
            Guid warehouseId = Request.QueryString["warehouseId"] == null
                                 ? Guid.Empty
                                 : new Guid(Request.QueryString["warehouseId"]);

            //TODO WMS
            //RgGoodsNeed.DataSource = _stockCenterSao.GoodsRequireQuantitySearchFromBaishop(new Guid(GoodsId), filialeId, warehouseId);
        }
    }
}