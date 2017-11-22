using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.B2CModel;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class GoodsDemandNotOutQuantityDetailForm : System.Web.UI.Page
    {
        static readonly WMSSao _wmsSao = new WMSSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //获取仓库下的物流配送公司
            }
        }

        public Dictionary<Guid,String> FilialeDics
        {
            get
            {
                if (ViewState["FilialeDics"] == null) return new Dictionary<Guid, String>();
                return (Dictionary<Guid, String>)ViewState["FilialeDics"];
            }
            set { ViewState["FilialeDics"] = value; }
        }

        #region 列表帮助方法

        protected string GetOrderState(int orderState)
        {
            return EnumAttribute.GetKeyName((OrderState)orderState);
        }

        protected string GetPayMode(int payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }

        protected string GetWarehouse(Guid warehouseId)
        {
            var personinfo = CurrentSession.Personnel.Get();
            var wlist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            return wlist != null ? wlist.WarehouseName : "";
        }

        #endregion

        protected IDictionary<Guid, IList<OperationLogInfo>> DicOrderClew
        {
            get
            {
                if (ViewState["DicOrderClew"] == null) return new Dictionary<Guid, IList<OperationLogInfo>>();
                return (Dictionary<Guid, IList<OperationLogInfo>>)ViewState["DicOrderClew"];
            }
            set { ViewState["DicOrderClew"] = value; }
        }

        protected string GetMisClew(object orderId)
        {
            if (orderId != null)
            {
                var order = orderId.ToString();
                try
                {
                    if (DicOrderClew.Count == 0) return "用户下单";
                    var list = DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime);
                    string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                    return str;
                }
                catch
                {
                    return "";
                }
            }
            return string.Empty;
        }

        protected void RgGoodsOrderNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var orderList = new List<DemandOrderInfo>();
            if (!string.IsNullOrEmpty(Request.QueryString["GoodsId"]) &&
                !string.IsNullOrEmpty(Request.QueryString["WarehouseId"]))
            {
                var realGoodsId = new Guid(Request.QueryString["GoodsId"]);
                var filialeId = Request.QueryString["FilialeId"]!=null?new Guid(Request.QueryString["FilialeId"]):Guid.Empty;
                var warehouseId = new Guid(Request.QueryString["WarehouseId"]);
                var serviceFilialeIds=WMSSao.GetSaleFilialeIds(warehouseId, filialeId,realGoodsId);
                foreach (var filiale in serviceFilialeIds)
                {
                    orderList.AddRange(B2CSao.GetGoodsOrdersByRealGoodsId(filiale, realGoodsId, warehouseId));
                }
                if (orderList.Count != 0)
                    DicOrderClew = OperationLogSao.GetOperationLogList(orderList.Select(ent => ent.OrderId).ToList());
            }
            RgGoodsOrder.DataSource = orderList;
        }
    }
}