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

namespace ERP.UI.Web
{
    public partial class MemberMentionApply : BasePage
    {
        #region 属性
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
                    return "0";//提现打款 默认显示  待打款
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

        /// <summary>
        /// 体现申请单号
        /// </summary>
        protected string ApplyNo
        {
            get
            {
                if (ViewState["ApplyNo"] == null) return string.Empty;
                return ViewState["ApplyNo"].ToString();
            }
            set
            {
                ViewState["ApplyNo"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();
                LoadMemberMentionStateData();
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
            StartTime = RDP_StartTime.SelectedDate != null ? RDP_StartTime.SelectedDate.Value : DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate != null ? RDP_EndTime.SelectedDate.Value : DateTime.MinValue;
            SearchKey = RTB_Member.Text.Trim();
            SearchState = ddl_MemberMentionState.SelectedValue;
            if (DDL_BankAccount.SelectedItem == null || DDL_BankAccount.SelectedItem.Value.Equals("0"))
            {
                BankAccountName = string.Empty;
            }
            else
            {
                BankAccountName = DDL_BankAccount.SelectedItem.Text;
            }
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            SalePlatformId = string.IsNullOrEmpty(RCB_SalePlatform.SelectedValue) ? Guid.Empty : new Guid(RCB_SalePlatform.SelectedValue);
            ApplyNo = RdApplyNo.Text;
            RG_MemberMentionApply.Rebind();
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
        /// 会员体现状态
        /// </summary>
        public void LoadMemberMentionStateData()
        {
            var list = EnumAttribute.GetDict<MemberMentionState>();
            list.Remove((int)MemberMentionState.NoPass);
            list.Remove((int)MemberMentionState.Invalid);
            ddl_MemberMentionState.DataSource = list;
            ddl_MemberMentionState.DataTextField = "Value";
            ddl_MemberMentionState.DataValueField = "Key";
            ddl_MemberMentionState.DataBind();
        }
        #endregion

        #region 数据列表相关
        protected void RG_MemberMentionApply_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = RG_MemberMentionApply.CurrentPageIndex + 1;
            var pageSize = RG_MemberMentionApply.PageSize;
            int totalCount;
            IList<MemberMentionApplyInfo> list = MemberCenterSao.GetMemberMentionApplyByPage(SearchKey, ApplyNo, BankAccountName, StartTime, EndTime, Convert.ToInt32(SearchState), SaleFilialeId, SalePlatformId, RdBankAccounts.Text, pageIndex, pageSize, out totalCount);
            RG_MemberMentionApply.DataSource = list;
            RG_MemberMentionApply.VirtualItemCount = totalCount;
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
        #endregion
        #endregion

        #region SelectedIndexChanged事件
        protected void RCB_SaleFiliale_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SaleFilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            RCB_SalePlatform.Items.Clear();
            SalePlatformId = Guid.Empty;

            if (!SaleFilialeId.Equals(Guid.Empty))
            {
                RTB_Member.Enabled = true;
                RdBankAccounts.Enabled = true;

                LoadSalePlatformData(SaleFilialeId);
            }
            else
            {
                RTB_Member.Enabled = false;
                RTB_Member.Text = string.Empty;
                RdBankAccounts.Enabled = false;
                RdBankAccounts.Text = string.Empty;
            }
        }
        #endregion

        //批量操作
        protected void btn_Batch_Click(object sender, EventArgs e)
        {
            Hid_SelectedValue.Value = string.Empty;
            string paras = string.Empty;
            decimal amountTotal = 0;
            int count = 0;
            string filialeId = string.Empty;
            var errorMsg = new StringBuilder();
            if (Request["ckId"] != null)
            {
                var datas = Request["ckId"].Split(',');
                foreach (var item in datas)
                {
                    var parameter = item.Split('$')[0];
                    var amount = item.Split('$')[1];
                    var saleFilialeId = item.Split('$')[2];
                    var userName = item.Split('$')[3];

                    if (string.IsNullOrEmpty(filialeId))
                    {
                        filialeId = saleFilialeId;
                    }

                    if (!filialeId.Equals(saleFilialeId))
                    {
                        errorMsg.Append("所选数据不属于同一个销售公司，请重新选择！").Append("\\n");
                    }
                    paras += "," + parameter;
                    amountTotal += decimal.Parse(amount);
                    count++;
                }
            }
            else
            {
                errorMsg.Append("请选择相关数据！");
            }

            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                MessageBox.Show(this, errorMsg.ToString());
            }
            else
            {
                Hid_SelectedValue.Value = paras.Substring(1);
                MessageBox.AppendScript(this, "$(function () {ShowValue('" + Hid_SelectedValue.Value + "');BatchOperate('" + Server.UrlEncode(paras.Substring(1)) + "','" + filialeId + "','" + amountTotal + "','" + count + "');});");
            }
        }

        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            RG_MemberMentionApply.Columns[0].Visible = false;
            RG_MemberMentionApply.Columns[12].Visible = false;
            RG_MemberMentionApply.Columns[13].Visible = false;
            RG_MemberMentionApply.ExportSettings.ExportOnlyData = true;
            RG_MemberMentionApply.ExportSettings.IgnorePaging = true;
            RG_MemberMentionApply.ExportSettings.FileName = Server.UrlEncode("提现打款信息");
            RG_MemberMentionApply.MasterTableView.ExportToExcel();
        }
    }
}
