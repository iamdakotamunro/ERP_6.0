using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.Model;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>
    /// 会员总账 ADD 2014-09-09  陈重文
    /// </summary>
    public partial class MemberLedger : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO:待定
            if (!IsPostBack)
            {
                LoadSaleFilialeList();
            }
        }

        #region [加载销售公司，资金流向类型，时间控件]

        /// <summary>加载销售公司
        /// </summary>
        private void LoadSaleFilialeList()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SaleFiliale.SelectedIndex = 0;

            //资金流向类型
            RCB_FlowType.Items.Insert(0, new RadComboBoxItem("全部", "0"));
            RCB_FlowType.Items.Insert(1, new RadComboBoxItem("增加", "1"));
            RCB_FlowType.Items.Insert(2, new RadComboBoxItem("减少", "-1"));
            RCB_FlowType.SelectedIndex = 0;

            //时间控件
            RDP_StartTime.MaxDate = DateTime.Now;
            RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
            RDP_EndTime.SelectedDate = DateTime.Now;
            StartTime = RDP_StartTime.SelectedDate.Value;
            EndTime = RDP_EndTime.SelectedDate.Value;
        }

        #endregion

        #region [绑定数据源]

        protected void RGMemberReckoning_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            int pageIndex = RGMemberReckoning.CurrentPageIndex + 1;
            int pageSize = RGMemberReckoning.PageSize;
            int totalCount = 0;
            var list = new List<MemberReckoningInfo>();
            if (IsPostBack)
            {
                list = MemberCenterSao.SearchReckoningByPage(SaleFilialeID, StartTime, EndTime, FlowType, TradeCode, pageIndex, pageSize, out totalCount).ToList();
            }
            RGMemberReckoning.DataSource = list;
            RGMemberReckoning.VirtualItemCount = totalCount;
        }

        #endregion

        #region [搜索]
        /// <summary>搜索
        /// </summary>
        protected void IB_Search_Click(object sender, ImageClickEventArgs e)
        {
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            TradeCode = RTB_TradeCode.Text.Trim();
            StartTime = Convert.ToDateTime(RDP_StartTime.SelectedDate);
            EndTime = Convert.ToDateTime(RDP_EndTime.SelectedDate);
            FlowType = Convert.ToInt32(RCB_FlowType.SelectedValue);
            RGMemberReckoning.Rebind();
        }
        #endregion

        #region [根据来源ID获取名称]

        /// <summary>获取来源名称
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        protected string GetFromSourceNameById(object saleFilialeId)
        {
            var info = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == new Guid(saleFilialeId.ToString()));
            return info != null ? info.Name : string.Empty;
        }

        #endregion

        #region [ViewState]

        /// <summary>单据编号
        /// </summary>
        protected string TradeCode
        {
            get
            {
                if (ViewState["TradeCode"] == null) return string.Empty;
                return ViewState["TradeCode"].ToString();
            }
            set
            {
                ViewState["TradeCode"] = value;
            }
        }

        /// <summary>销售公司ID
        /// </summary>
        protected Guid SaleFilialeID
        {
            get
            {
                if (ViewState["SaleFilialeId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["SaleFilialeId"].ToString());
            }
            set
            {
                ViewState["SaleFilialeId"] = value;
            }
        }

        /// <summary>开始时间
        /// </summary>
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

        /// <summary>截至时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                var endTime = Convert.ToDateTime(ViewState["EndTime"]);
                if (endTime != DateTime.MinValue)
                {
                    ViewState["EndTime"] = Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                }
                return Convert.ToDateTime(ViewState["EndTime"]);
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }

        /// <summary>资金流向
        /// </summary>
        protected int FlowType
        {
            get
            {
                if (ViewState["FlowType"] == null)
                    return 0;
                return Convert.ToInt32(ViewState["FlowType"]);
            }
            set
            {
                ViewState["FlowType"] = value;
            }
        }

        #endregion
    }
}