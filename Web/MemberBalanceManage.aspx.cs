using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>会员余额管理     ADD  2014-09-04  陈重文 
    /// </summary>
    public partial class MemberBalanceManage : BasePage
    {
       // private readonly BankAccountManager _bankAccountManager = new BankAccountManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowData();
            }
        }

        #region [加载数据]

        /// <summary>加载数据
        /// </summary>
        private void LoadShowData()
        {
            //加载销售公司
            var list = CacheCollection.Filiale.GetHeadList().Where(ent => ent.IsActive && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RCB_SaleFiliale.DataSource = list;
            RCB_SaleFiliale.DataTextField = "Name";
            RCB_SaleFiliale.DataValueField = "ID";
            RCB_SaleFiliale.DataBind();
            RCB_SaleFiliale.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SaleFiliale.SelectedIndex = 0;

            //销售平台
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;

            //会员余额操作类型枚举
            var typeList = EnumAttribute.GetDict<MemberBalanceChangeTypeEnum>();
            RCB_Type.DataSource = typeList;
            RCB_Type.DataBind();
            RCB_Type.Items.Insert(0, new RadComboBoxItem("全部", "0"));
            RCB_Type.SelectedIndex = 0;

            //会员余额操作状态枚举
            var stateList = EnumAttribute.GetDict<MemberBalanceChangeStateEnum>();
            RCB_State.DataSource = stateList;
            RCB_State.DataBind();
            RCB_State.Items.Insert(0, new RadComboBoxItem("全部", "0"));
            RCB_State.SelectedValue = "2";//首次默认显示待确认状态

            //时间控件
            RDP_StartTime.MaxDate = DateTime.Now;
            RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
            RDP_EndTime.SelectedDate = DateTime.Now;
            StartTime = RDP_StartTime.SelectedDate.Value;
            EndTime = RDP_EndTime.SelectedDate.Value;
        }

        #endregion

        #region [绑定数据源，首次加载显示待确认状态的数据]

        /// <summary>绑定数据源
        /// </summary>
        protected void RadGridMemberBalanceManage_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = IsSerarch ? 1 : RadGridMemberBalanceManage.CurrentPageIndex + 1;
            var pageSize = RadGridMemberBalanceManage.PageSize;
            int totalCount;
            IList<MemeberBalanceChangeInfo> list = MemberCenterSao.SearchUserBalanceChangeListByPage(SaleFilialeID, SalePlatformID, MemberId, ChangeState,
                                                                         ChangeType, TradeCode, StartTime, EndTime, TypeOfProblemId, bool.Parse(rbl_IsOfficial.SelectedValue),
                                                                         pageIndex, pageSize, out totalCount);
            RadGridMemberBalanceManage.DataSource = list;
            RadGridMemberBalanceManage.VirtualItemCount = totalCount;

            #region 绑定导出Grid
            IList<MemeberBalanceChangeInfo> exportList = MemberCenterSao.SearchUserBalanceChangeListByPage(SaleFilialeID, SalePlatformID, MemberId, ChangeState,
                                                                         ChangeType, TradeCode, StartTime, EndTime, TypeOfProblemId, bool.Parse(rbl_IsOfficial.SelectedValue),
                                                                         1, 999999, out totalCount);
            ExportExcel.DataSource = exportList;
            ExportExcel.DataBind();
            #endregion
        }

        #endregion

        #region [搜索]

        /// <summary>搜索
        /// </summary>
        protected void IB_Search_Click(object sender, ImageClickEventArgs e)
        {
            MemberId = string.IsNullOrEmpty(RCB_Member.SelectedValue) ? Guid.Empty : new Guid(RCB_Member.SelectedValue);
            StartTime = Convert.ToDateTime(RDP_StartTime.SelectedDate);
            EndTime = Convert.ToDateTime(RDP_EndTime.SelectedDate);
            TradeCode = RTB_ReceiptNo.Text.Trim();
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
            ChangeState = Convert.ToInt32(RCB_State.SelectedValue);
            ChangeType = Convert.ToInt32(RCB_Type.SelectedValue);
            IsSerarch = true;
            TypeOfProblemId = RcbProblemId.SelectedValue != null && !string.IsNullOrEmpty(RcbProblemId.SelectedValue)
                ? new Guid(RcbProblemId.SelectedValue)
                : Guid.Empty;
            if (SalePlatformID == Guid.Empty && MemberId != Guid.Empty)
            {
                RAM.Alert("温馨提示：通过会员名称搜索数据必须选择具体的销售平台，谢谢配合！");
                return;
            }
            RadGridMemberBalanceManage.Rebind();
        }

        #endregion

        #region [会员下拉列表搜索]

        /// <summary>会员搜索
        /// </summary>
        protected void RCB_MemberItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            string userName = e.Text.Trim();

            if (!string.IsNullOrEmpty(userName) && userName.Length >= 2)
            {
                var salePlatformId = new Guid(RCB_SalePlatform.SelectedValue);
                var list = MemberCenterSao.GetUserToDic(salePlatformId, userName);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key.ToString()
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        #endregion

        #region 销售公司、销售平台OnSelectedIndexChanged事件

        /// <summary>销售公司OnSelectedIndexChanged事件
        /// </summary>
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
                if (rcbSalePlatform != null)
                    rcbSalePlatform.Items.Clear();
                SalePlatformID = Guid.Empty;
                RCB_SalePlatform.SelectedValue = Guid.Empty.ToString();
                IsEnabledRCB_Member();
                return;
            }
            RCB_SalePlatform.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).ToList();
            RCB_SalePlatform.DataTextField = "Name";
            RCB_SalePlatform.DataValueField = "ID";
            RCB_SalePlatform.DataBind();
            RCB_SalePlatform.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            RCB_SalePlatform.SelectedIndex = 0;
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
            IsEnabledRCB_Member();
        }

        /// <summary>销售平台OnSelectedIndexChanged事件
        /// </summary>
        protected void RCB_SalePlatform_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeID = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformID = new Guid(RCB_SalePlatform.SelectedValue);
            BindProblemType(SalePlatformID);
            IsEnabledRCB_Member();
        }

        #endregion

        #region [是否可用会员搜索]

        private void IsEnabledRCB_Member()
        {
            if (SalePlatformID == Guid.Empty)
            {
                RCB_Member.Text = string.Empty;
                RCB_Member.Enabled = false;
            }
            else
            {
                RCB_Member.Enabled = true;
            }
        }

        #endregion

        #region [OnPageIndexChanged事件]

        protected void RadGridMemberBalanceManage_OnPageIndexChanged(object source, GridPageChangedEventArgs e)
        {
            IsSerarch = false;
        }

        #endregion

        #region [页面展示获取银行名称]

        /// <summary>获取银行名称
        /// </summary>
        /// <param name="bankAccountId">银行ID</param>
        /// <param name="payBankName">支付宝/银行名称</param>
        /// <returns></returns>
        protected string GetBankAccountName(object bankAccountId, object payBankName)
        {
            var id = bankAccountId.ToString();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var info = BankAccountManager.ReadInstance.GetBankAccounts(new Guid(id));
                if (info == null)
                    return String.Empty;
                if (info.BankAccountsId != Guid.Empty)
                    return info.BankName + " - " + info.AccountsName;
            }
            return payBankName == null ? String.Empty : payBankName.ToString();
        }

        #endregion

        #region [页面展示获取枚举状态描述]

        protected string GetState(object state)
        {
            try
            {
                return EnumAttribute.GetKeyName((MemberBalanceChangeStateEnum)state);
            }
            catch (Exception)
            {
                return "未知状态";
            }
        }

        #endregion

        #region 获取用户操作权限
        /// <summary>获取用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint()
        {
            const string PAGE_NAME = "MemberBalanceManage.aspx";
            const string AUDITING = "MemberBalanceAuditing";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, AUDITING);
        }
        #endregion

        #region [ViewState]

        protected Guid MemberId
        {
            get
            {
                if (ViewState["MemberId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["MemberId"].ToString());
            }
            set
            {
                ViewState["MemberId"] = value;
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

        /// <summary>单据号
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

        /// <summary>销售平台ID
        /// </summary>
        protected Guid SalePlatformID
        {
            get
            {
                if (ViewState["SalePlatformID"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["SalePlatformID"].ToString());
            }
            set
            {
                ViewState["SalePlatformID"] = value;
            }
        }

        /// <summary>操作状态
        /// </summary>
        protected int ChangeState
        {
            get
            {
                if (ViewState["ChangeState"] == null)
                    return 2;//默认返回待确认
                return Convert.ToInt32(ViewState["ChangeState"]);
            }
            set
            {
                ViewState["ChangeState"] = value;
            }
        }

        /// <summary>操作类型
        /// </summary>
        protected int ChangeType
        {
            get
            {
                if (ViewState["ChangeType"] == null)
                    return 0;
                return Convert.ToInt32(ViewState["ChangeType"]);
            }
            set
            {
                ViewState["ChangeType"] = value;
            }
        }

        protected bool IsSerarch
        {
            get
            {
                return ViewState["IsSerarch"] != null && Convert.ToBoolean(ViewState["IsSerarch"]);
            }
            set
            {
                ViewState["IsSerarch"] = value;
            }
        }

        /// <summary>
        /// 问题类型ID
        /// </summary>
        protected Guid TypeOfProblemId
        {
            get
            {
                if (ViewState["TypeOfProblemId"] == null) return Guid.Empty;
                return new Guid(ViewState["TypeOfProblemId"].ToString());
            }
            set
            {
                ViewState["TypeOfProblemId"] = value;
            }
        }
        #endregion

        #region [AjaxRequest]

        /// <summary>AjaxRequest
        /// </summary>
        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RadGridMemberBalanceManage, e);
        }

        #endregion

        #region  绑定问题类型
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salePlateId"></param>
        protected void BindProblemType(Guid salePlateId)
        {
            var dics = new Dictionary<Guid, string>
            {
                {Guid.Empty,string.Empty}
            };
            var data = MemberCenterSao.GetMemberTypeProblemTypeList(salePlateId);
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    dics.Add(item.Key, item.Value);
                }
            }
            RcbProblemId.DataSource = dics;
            RcbProblemId.DataBind();
        }
        #endregion

        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            DataGridToExcel.GridViewToExcel(ExportExcel, "会员余额信息");
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }
    }
}