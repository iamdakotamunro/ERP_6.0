using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class ReturnGoodsCheck : BasePage
    {
        //readonly CheckRefund _checkRefund = new CheckRefund();
        private readonly IOperationLogManager _operationLogManager =new OperationLogManager();
        private readonly ICheckRefund _refundDal = new CheckRefund(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StartTime = DateTime.Now.AddMonths(-1);
                EndTime = DateTime.Now;
            }
        }

        #region 自定义属性
        /// <summary>
        /// 退换货号
        /// </summary>
        protected string SearchKey
        {
            get { return ViewState["SearchKey"] == null ? string.Empty : ViewState["SearchKey"].ToString(); }
            set { ViewState["SearchKey"] = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        protected DateTime StartTime
        {
            get { return Convert.ToDateTime(ViewState["StartTime"]); }
            set { ViewState["StartTime"] = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                var endTime = Convert.ToDateTime(ViewState["EndTime"]);
                if (endTime != DateTime.MinValue)
                {
                    ViewState["EndTime"] = Convert.ToDateTime(ViewState["EndTime"]).ToString("yyyy-MM-dd 23:59:59");
                }
                return Convert.ToDateTime(ViewState["EndTime"]);
            }
            set { ViewState["EndTime"] = value; }
        }

        /// <summary>
        /// 检查状态
        /// </summary>
        protected string SearchCheckState
        {
            get
            {
                if (ViewState["SearchCheckState"] == null)
                {
                    ViewState["SearchCheckState"] = "0";
                }
                return ViewState["SearchCheckState"].ToString();
            }
            set { ViewState["SearchCheckState"] = value; }
        }
        protected IList<ShopFilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    IList<FilialeInfo> filialeList = CacheCollection.Filiale.GetHeadList().Where(w => w.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
                    var info = new ShopFilialeInfo
                                   {
                                       FilialeId = Guid.Empty,
                                       FilialeName = "全部"
                                   };
                    var newList = new List<ShopFilialeInfo> { info };
                    newList.AddRange(filialeList.Select(filialeInfo => new ShopFilialeInfo
                    {
                        FilialeId = filialeInfo.ID,
                        FilialeName = filialeInfo.Name
                    }));
                    ViewState["FilialeList"] = newList;
                }
                return (IList<ShopFilialeInfo>)ViewState["FilialeList"];
            }
        }
        protected string SelectedFilialeId
        {
            get
            {
                if (ViewState["SelectedFilialeId"] == null)
                {
                    ViewState["SelectedFilialeId"] = Guid.Empty.ToString();
                }
                return ViewState["SelectedFilialeId"].ToString();
            }
            set { ViewState["SelectedFilialeId"] = value; }
        }
        #endregion

        protected void RgRefund_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime startTime = StartTime == DateTime.MinValue ? DateTime.Now.AddMonths(-1) : StartTime;
            DateTime endTime = EndTime == DateTime.MinValue ? DateTime.Now : Convert.ToDateTime(EndTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            var checkRefundList = _refundDal.GetCheckRefundInfo(SearchKey, startTime, endTime, int.Parse(SearchCheckState), new Guid(SelectedFilialeId));
            //排除门店采购退回商品检查 modify by liangcanren at 2015-03-26
            checkRefundList = checkRefundList.Where(act => act.SaleFilialeId != Guid.Empty && !string.IsNullOrEmpty(act.OrderNo)).ToList();
            RGRefund.DataSource = checkRefundList;
            if (checkRefundList.Count > 0)
            {
                var pageIndex = RGRefund.CurrentPageIndex + 1;
                var pageSize = RGRefund.PageSize;
                var orderIdList = checkRefundList.Select(w => w.OrderId).ToList();
                orderIdList = pageIndex == 1 ? orderIdList.Take(pageSize).ToList() : orderIdList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                DicOrderClew = _operationLogManager.GetOperationLogList(orderIdList);
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
                string order = orderId.ToString();
                if (DicOrderClew.Count == 0) return "用户下单";
                var list = DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime);
                string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                return str;
            }
            return string.Empty;
        }

        protected void RgRefund_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                SearchKey = ((TextBox)e.Item.FindControl("TB_Search")).Text.Trim();
                SearchCheckState = ((DropDownList)e.Item.FindControl("DDL_CheckState")).SelectedValue;
                var rcbFromSource = ((RadComboBox)e.Item.FindControl("RCB_FromSource"));
                var rdp = ((RadDatePicker)e.Item.FindControl("RDP_StartTime"));
                var rdpEnd = ((RadDatePicker)e.Item.FindControl("RDP_EndTime"));
                if (rcbFromSource != null)
                {
                    SelectedFilialeId = rcbFromSource.SelectedValue;
                }
                if (rdp != null && rdp.SelectedDate != null)
                {
                    StartTime = Convert.ToDateTime(rdp.SelectedDate);
                }
                if (rdpEnd != null && rdpEnd.SelectedDate != null)
                {
                    EndTime = Convert.ToDateTime(rdpEnd.SelectedDate);
                }
                RGRefund.CurrentPageIndex = 0;
                RGRefund.Rebind();
            }
        }


        protected string GetCheckState(string checkState)
        {
            switch (checkState)
            {
                case "0":
                    return "待检查";
                case "1":
                    return "通过";
                case "2":
                    return "退回";
                default:
                    return "--";
            }
        }

        /// <summary>
        /// 获取检查公司
        /// </summary>
        /// <param name="checkFilialeId"></param>
        /// <returns></returns>
        protected string GetCheckFilialeName(object checkFilialeId)
        {
            if (new Guid(checkFilialeId.ToString()) == Guid.Empty)
            {
                return "-";
            }
            ShopFilialeInfo info = FilialeList.FirstOrDefault(w => w.FilialeId == new Guid(checkFilialeId.ToString()));
            return info == null ? "-" : info.FilialeName;
        }

        protected string GetClew(object clew)
        {
            return string.IsNullOrEmpty(clew.ToString()) ? "" : clew.ToString();
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGRefund, e);
        }


        protected string ShowReturnGoodsCheckJs(object refundId, object filialeId)
        {
            if (filialeId.ToString().Contains("-8888-"))
            {
                return string.Format("return ShowShopCheckForm('{0}')", refundId);
            }
            return string.Format("return ShowProcessForm('{0}')", refundId);
        }
    }
}