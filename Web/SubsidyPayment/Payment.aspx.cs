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
using ERP.Enum.SubsidyPayment;
using ERP.BLL.Implement.FinanceModule;
using ERP.BLL.Interface;
using System.Collections;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using ERP.Environment;
using ERP.Model;

namespace ERP.UI.Web.SubsidyPayment
{
    /// <summary>
    /// 补贴打款
    /// </summary>
    public partial class Payment : BasePage
    {
        private readonly SubsidyPaymentManager _SubsidyPaymentSerivce = new SubsidyPaymentManager();
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private ViewStateHelper viewStateHelper = new ViewStateHelper();

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
        /// 状态
        /// </summary>
        public String SubsidyType
        {
            get { return viewStateHelper.GetViewState("SubsidyType", "0"); }
            set { viewStateHelper.SetViewState("SubsidyType", value); }
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

        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }
        /// <summary>
        /// 资金账户列表属性
        /// </summary>
        protected Dictionary<Guid, string> BankAccountInfoDic
        {
            get
            {
                if (ViewState["BankAccountInfoDic"] == null)
                {
                    var saleFilialeId = string.IsNullOrEmpty(Request.QueryString["SaleFilialeId"]) ? Guid.Empty.ToString() : Request.QueryString["SaleFilialeId"];
                    var bankAccountInfoDic = _bankAccounts.GetBankAccountsList(new Guid(saleFilialeId), Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                    bankAccountInfoDic.Add(new Guid("7175FA90-1214-46E7-8D15-F9053E16928C"), "支付宝【zfbahkd@keede.cn】");
                    return bankAccountInfoDic;
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();
                LoadStateData();
                LoadSubsidyTypeData();
                LoadBankAccountData();

                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now;

                StartTime = RDP_StartTime.SelectedDate.Value;
                EndTime = RDP_EndTime.SelectedDate.Value;
            }
        }

        #region 操作按钮

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

        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            RG.Columns[0].Visible = false;
            RG.Columns[9].Visible = false;
            RG.Columns[11].Visible = false;
            RG.Columns[12].Visible = false;
            RG.ExportSettings.ExportOnlyData = true;
            RG.ExportSettings.IgnorePaging = true;
            RG.ExportSettings.FileName = Server.UrlEncode("补贴打款信息");
            RG.MasterTableView.ExportToExcel();
        }


        //批量受理提示(提示申请单总数、发票总数、收据总数)
        protected void btn_Accept_Click(object sender, EventArgs e)
        {
            Hid_ID.Value = string.Empty;
            if (Request["ckId"] != null)
            {
                var datas = Request["ckId"].Split(',');
                List<string> listID = new List<string>();
                foreach (var item in datas)
                {
                    listID.Add(item.Split('&')[0]);
                }

                if (listID.Count > 0)
                {
                    Hid_ID.Value = string.Join(",", listID.ToArray());
                    ArrayList arrayList = _SubsidyPaymentSerivce.GetSumList(Hid_ID.Value.Split(',').ToList());
                    if (arrayList.Count > 0)
                    {
                        lbl_Total.Text = arrayList[0].ToString();
                        lbl_SumSubsidyAmount.Text = arrayList[1].ToString();
                    }
                }
            }
            else
            {
                RAM.Alert("请选择相关数据！");
                return;
            }
            MessageBox.AppendScript(this, "moveShow();ShowValue('" + Hid_ID.Value + "');");
        }
        protected void BtnBackClick(object sender, EventArgs e)
        {
        }
        //批量受理通过
        protected void BtnPassClick(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(Hid_ID.Value))
            //{
            //    var errorMsg = new StringBuilder();
            //    var reportIds = Hid_ID.Value.Split(',');
            //    foreach (var item in reportIds)
            //    {
            //        CostReportInfo model = _costReport.GetReportByReportId(new Guid(item));
            //        if (model.State != (int)CostReportState.NoAuditing)
            //        {
            //            errorMsg.Append("“").Append(model.ReportNo).Append("”状态已更新，不允许此操作！").Append("\\n");
            //            continue;
            //        }
            //        try
            //        {
            //            AcceptPass(model);
            //        }
            //        catch
            //        {
            //            errorMsg.Append("“").Append(model.ReportNo).Append("”保存失败！").Append("\\n");
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(errorMsg.ToString()))
            //    {
            //        MessageBox.Show(this, errorMsg.ToString());
            //    }
            //    MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            //}
        }

        #endregion 操作按钮

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
            di.Add((int)SubsidyPaymentStatusEnum.PendingPayment, "待打款");
            di.Add((int)SubsidyPaymentStatusEnum.HadPayment, "已打款");
            di.Add((int)SubsidyPaymentStatusEnum.Rejected, "核退");

            ddl_Status.DataSource = di;
            ddl_Status.DataTextField = "Value";
            ddl_Status.DataValueField = "Key";
            ddl_Status.DataBind();
        }

        /// <summary>
        /// 补贴类型状态
        /// </summary>
        public void LoadSubsidyTypeData()
        {
            IDictionary<int, string> di = new Dictionary<int, string>();
            di.Add(0, "");
            di.Add((int)SubsidyTypeEnum.Compensate, "补偿");
            di.Add((int)SubsidyTypeEnum.Gift, "赠送");

            ddl_SubsidyType.DataSource = di;
            ddl_SubsidyType.DataTextField = "Value";
            ddl_SubsidyType.DataValueField = "Key";
            ddl_SubsidyType.DataBind();
        }

        /// <summary>
        /// 根据销售公司获取资金账号
        /// </summary>
        protected void LoadBankAccountData()
        {
            RCB_AccountID.DataSource = BankAccountInfoDic;
            RCB_AccountID.DataTextField = "Value";
            RCB_AccountID.DataValueField = "Key";
            RCB_AccountID.DataBind();
            RCB_AccountID.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
        }
        #endregion 数据准备

        #region 数据列表相关

        protected void RG_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int totalCount;

            SubsidyPaymentInfo_SeachModel seachModel = new SubsidyPaymentInfo_SeachModel()
            {
                OrderNumber = OrderNumber,
                StartTime = StartTime,
                EndTime = EndTime,
                Status = Convert.ToInt32(Status),
                SaleFilialeId = SaleFilialeId,
                SalePlatformId = SalePlatformId,
                SubsidyType = Convert.ToInt32(SubsidyType),

                PageIndex = RG.CurrentPageIndex + 1,
                PageSize = RG.PageSize,
                listStatus = new List<int>() { (int)SubsidyPaymentStatusEnum.PendingPayment, (int)SubsidyPaymentStatusEnum.HadPayment, (int)SubsidyPaymentStatusEnum.Rejected, }
            };

            var list = _SubsidyPaymentSerivce.GetSubsidyPaymentList(seachModel, out totalCount);
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