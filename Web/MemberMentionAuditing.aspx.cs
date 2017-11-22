using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    ///<summary>
    /// 提现申请页面
    /// Add by liucaijun at 2011-October-27th
    ///</summary>
    public partial class MemberMentionAuditing : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWebType();
                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now;
                StartTime = RDP_StartTime.SelectedDate.Value;
                EndTime = RDP_EndTime.SelectedDate.Value;
            }
        }

        #region[设置列表数据源]
        protected void RgMemberMentionApplyNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = RG_MemberMentionApply.CurrentPageIndex + 1;
            var pageSize = RG_MemberMentionApply.PageSize;
            int totalCount;
            IList<MemberMentionApplyInfo> list = MemberCenterSao.GetMemberMentionApplyByPage(SearchKey, string.Empty, string.Empty,
                                                                                               StartTime, EndTime,
                                                                                               int.Parse(SearchState),
                                                                                               SaleFilialeId, SalePlatformId, string.Empty, pageIndex,
                                                                                               pageSize, out totalCount);
            RG_MemberMentionApply.DataSource = list;
            RG_MemberMentionApply.VirtualItemCount = totalCount;
        }
        #endregion

        #region[Ajax页面返回]
        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_MemberMentionApply, e);
        }
        #endregion

        #region[搜索]
        protected void LbSearchClick(object sender, ImageClickEventArgs e)
        {
            StartTime = RDP_StartTime.SelectedDate != null ? RDP_StartTime.SelectedDate.Value : DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate != null ? RDP_EndTime.SelectedDate.Value : DateTime.MinValue;
            SearchKey = RTB_Member.Text.Trim();
            SearchState = DDL_Select.SelectedValue;
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            if (SalePlatformId == Guid.Empty && !string.IsNullOrWhiteSpace(SearchKey))
            {
                RAM.Alert("温馨提示：通过会员名称搜索数据必须选择具体的销售平台，谢谢配合！");
                return;
            }
            RG_MemberMentionApply.Rebind();
        }
        #endregion

        #region[获取状态]
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        protected string GetState(object state)
        {
            return EnumAttribute.GetKeyName((MemberMentionState)state);
        }
        #endregion

        #region[绑定网站类型]

        /// <summary>绑定网站类型
        /// </summary>
        public void BindWebType()
        {
            var list = CacheCollection.Filiale.GetHeadList().Where(ent => ent.IsActive && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_SaleFiliale.SelectedIndex = 0;

            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
        }

        /// <summary>获取来源ID
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        protected string GetFromSourceNameById(object saleFilialeId)
        {
            var fid = new Guid(saleFilialeId.ToString());
            var info = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == fid);
            return info != null ? info.Name : string.Empty;
        }

        #endregion

        #region[ViewState]
        ///<summary>搜索会员名
        ///</summary>
        public String SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null)
                    return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set
            {
                ViewState["SearchKey"] = value;
            }
        }

        /// <summary>银行名称
        /// </summary>
        public String BankAccountName
        {
            get
            {
                if (ViewState["BankAccountName"] == null)
                    return string.Empty;
                return ViewState["BankAccountName"].ToString();
            }
            set
            {
                ViewState["BankAccountName"] = value;
            }
        }

        /// <summary>搜索状态
        /// </summary>
        public String SearchState
        {
            get
            {
                if (ViewState["SearchState"] == null)
                    return "1";//提现审核 默认显示  待审核
                return ViewState["SearchState"].ToString();
            }
            set
            {
                ViewState["SearchState"] = value;
            }
        }

        /// <summary>销售公司ID
        /// </summary>
        public Guid SaleFilialeId
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

        /// <summary>销售平台
        /// </summary>
        public Guid SalePlatformId
        {
            get
            {
                if (ViewState["SalePlatformId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["SalePlatformId"].ToString());
            }
            set
            {
                ViewState["SalePlatformId"] = value;
            }
        }

        /// <summary>起始时间
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
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }

        #endregion

        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var radComboBox = o as RadComboBox;
            if (radComboBox == null) return;
            var rcbSaleFiliale = radComboBox.Parent.FindControl("RCB_SaleFiliale") as RadComboBox;
            var rcbSalePlatform = radComboBox.Parent.FindControl("RCB_SalePlatform") as RadComboBox;
            if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
            if (rcbSaleFiliale == null) return;
            var rcbSaleFilialeId = new Guid(rcbSaleFiliale.SelectedValue);
            if (rcbSaleFilialeId == Guid.Empty)
            {
                //rcbSalePlatform.Items.Clear();
                SalePlatformId = Guid.Empty;
                IsEnabledRTB_Member();
                return;
            }
            RCB_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            IsEnabledRTB_Member();
        }

        protected void RCB_SalePlatform_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
            IsEnabledRTB_Member();
        }

        private void IsEnabledRTB_Member()
        {
            if (SalePlatformId == Guid.Empty)
            {
                RTB_Member.Text = string.Empty;
                RTB_Member.Enabled = false;
            }
            else
            {
                RTB_Member.Enabled = true;
            }
        }
    }
}
