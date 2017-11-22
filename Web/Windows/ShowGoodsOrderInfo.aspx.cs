using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowGoodsOrderInfo : WindowsPage
    {
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected Guid RealGoodsId
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["realGoodsId"])?new Guid(Request.QueryString["realGoodsId"]):Guid.Empty;
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["warehouseId"])? new Guid(Request.QueryString["warehouseId"]):Guid.Empty;
            }
        }

        protected void RGGoodsOrder_NeedSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsOrderDemandInfo> newList=new List<GoodsOrderDemandInfo>();

            var declareBills = WMSSao.GetDeclareBillNos(WarehouseId, RealGoodsId); // 每个单据中的缺货数量

            //采购中的商品数量
            var purchasingGoodsDics = _purchasingDetail.GetStockDeclarePursingGoodsDics(WarehouseId,
                    new[] { PurchasingState.NoSubmit, PurchasingState.Purchasing, PurchasingState.PartComplete, PurchasingState.StockIn, PurchasingState.WaitingAudit, PurchasingState.Refusing },
                    new List<Guid> {RealGoodsId});

            var totalQuantity = declareBills.Sum(ent => ent.StockQuantity) + declareBills.Sum(ent => ent.UppingQuantity) + purchasingGoodsDics.Values.Sum(ent=>ent);
            foreach (var orderBill in declareBills.SelectMany(ent=>ent.OrderBills).OrderBy(ent=>ent.Quantity))
            {
                if (orderBill.Quantity > totalQuantity)
                {
                    newList.Add(new GoodsOrderDemandInfo
                    {
                        OrderId = orderBill.OrderId,
                        BusinessType = 1,
                        OrderNo = orderBill.OrderNo,
                        SaleFilialeId = orderBill.FilialeId,
                        Quantity = orderBill.Quantity,
                        LackQuantity = totalQuantity > 0 ? orderBill.Quantity - totalQuantity : orderBill.Quantity
                    });
                }
                totalQuantity -= orderBill.Quantity;
            }
            foreach (var storageRecords in declareBills.Where(ent=>ent.StorageBills.Count>0))
            {
                foreach (var storageRecord in storageRecords.StorageBills)
                {
                    if (storageRecord.Value > totalQuantity)
                    {
                        newList.Add(new GoodsOrderDemandInfo
                        {
                            OrderId = Guid.NewGuid(),
                            BusinessType = 2,
                            OrderNo = storageRecord.Key,
                            SaleFilialeId = storageRecords.HostingFilialeId,
                            Quantity = storageRecord.Value,
                            LackQuantity = totalQuantity > 0 ? storageRecord.Value - totalQuantity : storageRecord.Value
                        });
                    }
                    totalQuantity -= storageRecord.Value;
                }
            }
            
            RGGoodsOrder.DataSource = newList;
            var orderList = newList.Where(ent => ent.BusinessType == 1);
            if (orderList.Any())
            {
                var pageIndex = RGGoodsOrder.CurrentPageIndex;
                int pageSize = RGGoodsOrder.PageSize;
                var orderIdList = orderList.Select(w => w.OrderId).ToList();
                orderIdList = pageIndex == 0 ? orderIdList.Take(pageSize).ToList() : orderIdList.Skip(pageSize * pageIndex).Take(pageSize).ToList();
                if (orderIdList.Count != 0)
                    DicOrderClew = OperationLogSao.GetOperationLogList(orderIdList);
            }
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

        protected string GetMisClew(object orderNo)
        {
            if (orderNo != null)
            {
                string order = orderNo.ToString();
                if (DicOrderClew.Count == 0) return "用户下单";
                var list = DicOrderClew.ContainsKey(new Guid(order)) ? DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime).ToList() : new List<OperationLogInfo>();
                string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                return str;
            }
            return string.Empty;
        }

        protected string GetPayMode(Object payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }
        protected string GetPayState(object payState)
        {
            return EnumAttribute.GetKeyName((PayState)payState);
        }

        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGGoodsOrder, e);
        }

        protected void RGGoodsOrderItemDataBound(object sender, GridItemEventArgs e)
        {
            var dataItem = e.Item as GridDataItem;
            if (dataItem!=null)
            {
                var quantity=Convert.ToInt32(dataItem.GetDataKeyValue("LackQuantity"));
                if(quantity<0)
                    dataItem.Style.Add("background-color", "red");
            }
        }
    }
}
