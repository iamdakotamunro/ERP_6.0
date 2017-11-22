using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using System.Text;
using ERP.Model.SubsidyPayment;
using ERP.Enum.RefundsMoney;
using ERP.Model.RefundsMoney;
using ERP.BLL.Implement.FinanceModule;
using ERP.BLL.Interface;

namespace ERP.UI.Web.RefundsMoney
{
    /// <summary>
    /// 退款打款UI
    /// </summary>
    public partial class RefundsMoney : BasePage
    {
        private readonly RefundsMoneyManager _refundsMoneySerivce = new RefundsMoneyManager();
        private ViewStateHelper viewStateHelper = new ViewStateHelper();

        //TODO:待定

        #region 属性

        /// <summary>
        /// 单据号
        /// </summary>
        public String OrderNumber
        {
            get
            {
                if (ViewState["OrderNumber"] == null)
                    return string.Empty;
                return ViewState["OrderNumber"].ToString();
            }
            set { ViewState["OrderNumber"] = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToShortDateString() + " 00:00:00");
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["StartTime"]).ToShortDateString() + " 00:00:00");
            }
            set
            {
                ViewState["StartTime"] = value.ToString();
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null || Convert.ToDateTime(ViewState["EndTime"]) == DateTime.MinValue) return DateTime.Now;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndTime"] = value.ToString();
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public String Status
        {
            get { return viewStateHelper.GetViewState("Status", "0"); }
            set { viewStateHelper.SetViewState("Status", value); }
        }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        public Guid SaleFilialeId
        {
            get { return viewStateHelper.GetViewState_Guid("SaleFilialeId"); }
            set { viewStateHelper.SetViewState_Guid("SaleFilialeId", value); }
        }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId
        {
            get { return viewStateHelper.GetViewState_Guid("SalePlatformId"); }
            set { viewStateHelper.SetViewState_Guid("SalePlatformId", value); }
        }

        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();
                LoadStateData();

                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now;

                StartTime = RDP_StartTime.SelectedDate.Value;
                EndTime = RDP_EndTime.SelectedDate.Value;
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            OrderNumber = RTB_OrderNumber.Text.Trim();
            StartTime = RDP_StartTime.SelectedDate != null ? RDP_StartTime.SelectedDate.Value : DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate != null ? RDP_EndTime.SelectedDate.Value : DateTime.MinValue;
            Status = ddl_Status.SelectedValue;

            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = string.IsNullOrEmpty(RCB_SalePlatform.SelectedValue) ? Guid.Empty : new Guid(RCB_SalePlatform.SelectedValue);
            RG.Rebind();
        }

        #region 数据准备

        /// <summary>
        /// 销售公司
        /// </summary>
        public void LoadSaleFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList().Where(ent => ent.IsActive && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        /// <summary>
        /// 销售平台
        /// </summary>
        public void LoadSalePlatformData(Guid saleFilialeId)
        {
            RCB_SalePlatform.DataSource = CacheCollection.SalePlatform.GetListByFilialeId(saleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        /// <summary>
        /// 状态
        /// </summary>
        public void LoadStateData()
        {
            IDictionary<int, string> di = new Dictionary<int, string>();
            di.Add(0, "");
            di.Add((int)RefundsMoneyStatusEnum.PendingPayment, "待打款");
            di.Add((int)RefundsMoneyStatusEnum.HadPayment, "已打款");

            ddl_Status.DataSource = di;
            ddl_Status.DataTextField = "Value";
            ddl_Status.DataValueField = "Key";
            ddl_Status.DataBind();
        }

        #endregion 数据准备

        #region 数据列表相关

        protected void RG_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int totalCount;

            RefundsMoneyInfo_SeachModel seachModel = new RefundsMoneyInfo_SeachModel()
            {
                OrderNumber = OrderNumber,
                StartTime = StartTime,
                EndTime = EndTime,
                Status = Convert.ToInt32(Status),
                SaleFilialeId = SaleFilialeId,
                SalePlatformId = SalePlatformId,

                PageIndex = RG.CurrentPageIndex + 1,
                PageSize = RG.PageSize,
                listStatus = new List<int>() { (int)RefundsMoneyStatusEnum.PendingPayment, (int)RefundsMoneyStatusEnum.HadPayment, }
            };

            var list = _refundsMoneySerivce.GetRefundsMoneyList(seachModel, out totalCount);
            RG.DataSource = list;
            RG.VirtualItemCount = totalCount;
        }

        #region 列表显示辅助方法

        /// <summary>
        /// 获取销售公司名称
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        protected string GetSaleFilialeName(object saleFilialeId)
        {
            var info = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == new Guid(saleFilialeId.ToString()));
            return info != null ? info.Name : string.Empty;
        }

        /// <summary>
        /// 获取销售平台名称
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <returns></returns>
        protected string GetSalePlatformName(object salePlatformId)
        {
            var info = CacheCollection.SalePlatform.Get(new Guid(salePlatformId.ToString()));
            return info != null ? info.Name : string.Empty;
        }

        #endregion 列表显示辅助方法

        #endregion 数据列表相关

        #region SelectedIndexChanged事件

        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            RCB_SalePlatform.Items.Clear();
            SalePlatformId = Guid.Empty;

            if (!SaleFilialeId.Equals(Guid.Empty))
            {
                LoadSalePlatformData(SaleFilialeId);
            }
        }

        #endregion SelectedIndexChanged事件
    }
}