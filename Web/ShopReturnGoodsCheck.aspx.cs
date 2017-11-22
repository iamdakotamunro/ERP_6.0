using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class ShopReturnGoodsCheck : BasePage
    {
        //readonly CheckRefund _checkRefund = new CheckRefund();
        private readonly IOperationLogManager _operationLogManager =
            new OperationLogManager();
        private static readonly ICheckRefund _refundDal = new CheckRefund(GlobalConfig.DB.FromType.Read);
        #region 状态保存
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

        protected IDictionary<Guid, IList<OperationLogInfo>> DicOrderClew
        {
            get
            {
                if (ViewState["DicOrderClew"] == null) return new Dictionary<Guid, IList<OperationLogInfo>>();
                return (Dictionary<Guid, IList<OperationLogInfo>>)ViewState["DicOrderClew"];
            }
            set { ViewState["DicOrderClew"] = value; }
        }
        #endregion

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StartTime = DateTime.Now.AddMonths(-1);
                EndTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgShopRefundNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime startTime = StartTime == DateTime.MinValue ? DateTime.Now.AddMonths(-1) : StartTime;
            DateTime endTime = EndTime == DateTime.MinValue ? DateTime.Now : EndTime;
            var checkRefundList = _refundDal.GetShopCheckRefundList(SearchKey, startTime, 
                endTime, int.Parse(SearchCheckState), new List<Guid>());
            RgShopRefund.DataSource = checkRefundList;
            if (checkRefundList.Count > 0)
            {
                var pageIndex = RgShopRefund.CurrentPageIndex + 1;
                var pageSize = RgShopRefund.PageSize;
                var orderIdList = checkRefundList.Select(w => w.OrderId).ToList();
                orderIdList = pageIndex == 1 
                    ? orderIdList.Take(pageSize).ToList() 
                    : orderIdList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                DicOrderClew = _operationLogManager.GetOperationLogList(orderIdList);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgShopRefundItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                SearchKey = ((TextBox)e.Item.FindControl("TbSearch")).Text.Trim();
                SearchCheckState = ((DropDownList)e.Item.FindControl("DdlCheckState")).SelectedValue;
                var rdp = ((RadDatePicker)e.Item.FindControl("RdpStartTime"));
                var rdpEnd = ((RadDatePicker)e.Item.FindControl("RdpEndTime"));
                if (rdp != null && rdp.SelectedDate != null)
                {
                    StartTime = Convert.ToDateTime(rdp.SelectedDate);
                }
                if (rdpEnd != null && rdpEnd.SelectedDate != null)
                {
                    EndTime = Convert.ToDateTime(rdpEnd.SelectedDate);
                }
                RgShopRefund.Rebind();
            }
        }

        /// <summary> 根据id获取MIS的管理意见
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        protected string GetMisClew(object id)
        {
            if (id != null)
            {
                string order = id.ToString();
                if (DicOrderClew.Count == 0) return "联盟店退货";
                var list = DicOrderClew[new Guid(order)].OrderBy(act => act.OperateTime);
                string str = list.Aggregate("", (current, item) => current + (item.Description + "\n"));
                return str;
            }
            return string.Empty;
        }

        /// <summary>
        /// Grid请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgShopRefund, e);
        }

        /// <summary>
        /// 获取检查状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected object GetCheckState(object state)
        {
            return EnumAttribute.GetKeyName((CheckState)state);
        }
    }
}