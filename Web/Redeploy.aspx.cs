using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>
    /// </summary>
    public partial class RedeployAw : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IOperationLogManager _operationLogManager =new OperationLogManager();
        private readonly IGoodsOrder _goodsOrder=new GoodsOrder(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                
            }
        }

        protected void RGGoodsOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = RGGoodsOrder.CurrentPageIndex + 1;
            var pageSize = RGGoodsOrder.PageSize;
            int totalCount = 0;
            IList<GoodsOrderInfo> goodsOrderList =new List<GoodsOrderInfo>();
            var authWarehouseList = WMSSao.GetWarehouseAuthDic(CurrentSession.Personnel.Get().PersonnelId);
            if (authWarehouseList != null && authWarehouseList.WarehouseDics != null && authWarehouseList.WarehouseDics.Count > 0)
            {
                var authWarehouseIds= authWarehouseList.WarehouseDics.Select(act => act.Key).ToList();
                goodsOrderList = _goodsOrder.GetOrderList(authWarehouseIds,StartTime, EndTime, SearchGoods, SearchKey, new List<OrderState> { OrderState.RequirePurchase }, pageIndex, pageSize, out totalCount);
            }
            else
            {
                RAM.Alert("当前登录人没有授权仓库！");
            }
            TextBoxDate.Text = CountRepeat(goodsOrderList);
            var expression = new GridSortExpression { FieldName = "OrderTime", SortOrder = GridSortOrder.Ascending };
            RGGoodsOrder.MasterTableView.SortExpressions.AddSortExpression(expression);
            RGGoodsOrder.DataSource = goodsOrderList;
            RGGoodsOrder.VirtualItemCount = totalCount;
            if (goodsOrderList.Count > 0)
            {
                DicOrderClew = _operationLogManager.GetOperationLogList(goodsOrderList.Select(act => act.OrderId).ToList());
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

        /// <summary> 根据订单id获取MIS的管理意见
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
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

        protected List<Guid> AuthWarehouseIds
        {
            get
            {
                if (ViewState["AuthWarehouseIds"]==null)return new List<Guid>();
                return (List<Guid>)ViewState["AuthWarehouseIds"];
            }
            set
            {
                ViewState["AuthWarehouseIds"] = value;
            }
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                if (Convert.ToDateTime(ViewState["EndTime"]) == DateTime.MinValue) return DateTime.MinValue;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }

        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set
            {
                ViewState["SearchKey"] = value;
            }
        }

        #region[搜索商品]
        public Guid SearchGoods
        {
            get
            {
                if (ViewState["SearchGoods"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["SearchGoods"].ToString());
            }
            set
            {
                ViewState["SearchGoods"] = value;
            }
        }
        #endregion

        protected void LB_SendSMS_Click(object sender, EventArgs e)
        {
            //统一发送短信代码

        }

        private string CountRepeat(IEnumerable<GoodsOrderInfo> goodsOrderList)
        {
            try
            {
                var dt = new DataTable("tempCount");
                var col1 = new DataColumn("dates");
                var col2 = new DataColumn("counts");

                dt.Columns.Add(col1);
                dt.Columns.Add(col2);

                if (goodsOrderList != null)
                {
                    foreach (GoodsOrderInfo goodsOrderInfo in goodsOrderList)
                    {


                        string dateString = goodsOrderInfo.OrderTime.ToString("yyyyMMdd");
                        DataRow[] ddr = dt.Select(string.Format("dates={0}", dateString));
                        if (ddr.Length == 0)
                        {
                            DataRow dr = dt.NewRow();
                            dr["dates"] = dateString;
                            dr["counts"] = 1;
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            foreach (DataRow row in ddr)
                            {
                                row.BeginEdit();
                                row["counts"] = Convert.ToInt32(row["counts"]) + 1;
                                row.EndEdit();
                                dt.AcceptChanges();
                            }

                        }
                    }
                }
                string msg = string.Empty;
                if (dt.Rows.Count > 0)
                {

                    for (int i = dt.Rows.Count - 1; i > -1; i--)
                    {

                        msg += "[" + dt.Rows[i][0] + "==>" + dt.Rows[i][1] + "]";
                    }

                }
                return msg;
            }
            catch
            {
                return "计算出错";
            }
        }

        protected void LB_Redeploy_Click(object sender, EventArgs e)
        {
            SearchKey = string.Empty;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            RGGoodsOrder.Rebind();
        }

        protected void LBRefresh_Refresh(object sender, EventArgs e)
        {
            RGGoodsOrder.Rebind();
        }

        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list.Select(item => new RadComboBoxItem
                    {
                        Text = item.Value,
                        Value = item.Key,
                    }))
                    {
                        combo.Items.Add(item);
                    }
                }
            }
        }

        protected void LBSearch_Click(object sender, EventArgs e)
        {
            StartTime = R_StartTime.SelectedDate != null ? R_StartTime.SelectedDate.Value : DateTime.MinValue;
            EndTime = R_EndTime.SelectedDate != null ? R_EndTime.SelectedDate.Value : DateTime.MinValue;
            SearchKey = TB_Search.Text.Trim();
            SearchGoods = string.IsNullOrEmpty(RCB_Goods.SelectedValue) ? Guid.Empty : new Guid(RCB_Goods.SelectedValue);
            if (SearchGoods != Guid.Empty)
            {
                var goodsValue = RCB_Goods.SelectedValue;
                if (goodsValue != Guid.Empty.ToString())
                {
                    SearchGoods = new Guid(goodsValue);
                }
            }
            RGGoodsOrder.Rebind();
        }
    }
}