using System;
using System.Configuration;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using ERP.BLL.Implement.Organization;
using MIS.Enum;
using ERP.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using Cost = ERP.BLL.Implement.Inventory.Cost;
using ERP.DAL.Utilities;
using ERP.DAL.Interface.IUtilities;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用票据受理执行
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_InvoiceAccept : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportInvoice _costReportInvoice = new CostReportInvoice(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTravel _costReportTravel = new CostReportTravelDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportTermini _costReportTermini = new CostReportTerminiDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportAmount _costReportAmount = new CostReportAmountDal(GlobalConfig.DB.FromType.Write);
        private static readonly IUtility _utility = new UtilityDal(GlobalConfig.DB.FromType.Write);

        #region 属性
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
        /// O2O事业部
        /// </summary>
        private string ShopBranchId
        {
            get
            {
                if (ViewState["ShopBranchId"] == null)
                {
                    ViewState["ShopBranchId"] = GlobalConfig.ShopBranchId;
                }
                return ViewState["ShopBranchId"].ToString();
            }
        }

        /// <summary>
        /// 费用申报=>费用分类(办公费=>差旅费)
        /// </summary>
        private string CostReport_TravelExpenses
        {
            get
            {
                if (ViewState["CostReport_TravelExpenses"] == null)
                {
                    ViewState["CostReport_TravelExpenses"] = ConfigManage.GetConfigValue("CostReport_TravelExpenses");
                }
                return ViewState["CostReport_TravelExpenses"].ToString();
            }
        }

        /// <summary>
        /// 费用申报=>费用分类(保证金押金)
        /// </summary>
        private string CostReport_DepositMargins
        {
            get
            {
                if (ViewState["CostReport_DepositMargins"] == null)
                {
                    ViewState["CostReport_DepositMargins"] = ConfigManage.GetConfigValue("CostReport_DepositMargins");
                }
                return ViewState["CostReport_DepositMargins"].ToString();
            }
        }

        /// <summary>
        /// 票据列表属性
        /// </summary>
        protected List<CostReportBillInfo> CostReportBillInfoList
        {
            get
            {
                if (ViewState["CostReportBillInfoList"] == null)
                    return new List<CostReportBillInfo>();
                return ViewState["CostReportBillInfoList"] as List<CostReportBillInfo>;
            }
            set
            {
                ViewState["CostReportBillInfoList"] = value;
            }
        }

        /// <summary>
        /// 起讫列表属性
        /// </summary>
        protected List<CostReportTerminiInfo> CostReportTerminiInfoList
        {
            get
            {
                if (ViewState["CostReportTerminiInfoList"] == null)
                    return new List<CostReportTerminiInfo>();
                return ViewState["CostReportTerminiInfoList"] as List<CostReportTerminiInfo>;
            }
            set
            {
                ViewState["CostReportTerminiInfoList"] = value;
            }
        }

        /// <summary>
        /// 申请金额列表属性
        /// </summary>
        protected List<CostReportAmountInfo> CostReportAmountInfoList
        {
            get
            {
                if (ViewState["CostReportAmountInfoList"] == null)
                    return new List<CostReportAmountInfo>();
                return ViewState["CostReportAmountInfoList"] as List<CostReportAmountInfo>;
            }
            set
            {
                ViewState["CostReportAmountInfoList"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportId = Request.QueryString["ReportId"];
                if (string.IsNullOrEmpty(reportId)) return;
                CostReportInfo costReportInfo = _costReport.GetReportByReportId(new Guid(reportId));

                LoadReportKindData();//加载申报类型
                LoadBranchData();//加载部门数据
                if (!costReportInfo.AssumeBranchId.Equals(Guid.Empty))
                {
                    LoadGroupData(costReportInfo.AssumeBranchId);//根据部门id获取“小组”数据
                }
                if (!costReportInfo.AssumeGroupId.Equals(Guid.Empty))
                {
                    LoadShopData();//加载门店数据
                    AssumeShop.Visible = costReportInfo.AssumeBranchId.Equals(new Guid(ShopBranchId));
                }
                LoadReportData(costReportInfo);//初始化页面数据
            }
        }

        #region 数据准备
        //申报类型
        protected void LoadReportKindData()
        {
            var list = EnumAttribute.GetDict<CostReportKind>().OrderBy(p => p.Key);
            rbl_ReportKind.DataSource = list;
            rbl_ReportKind.DataTextField = "Value";
            rbl_ReportKind.DataValueField = "Key";
            rbl_ReportKind.DataBind();
            rbl_ReportKind.SelectedValue = string.Format("{0}", (int)CostReportKind.Before);
        }

        //部门
        protected void LoadBranchData()
        {
            var branchList = CacheCollection.Branch.GetSystemBranchList();
            ddl_AssumeBranch.DataSource = branchList.Where(w => w.ParentID == Guid.Empty);
            ddl_AssumeBranch.DataTextField = "Name";
            ddl_AssumeBranch.DataValueField = "ID";
            ddl_AssumeBranch.DataBind();
            ddl_AssumeBranch.Items.Insert(0, new ListItem("请选择", ""));
        }

        /// <summary>
        /// 根据部门id获取“小组”数据
        /// </summary>
        /// <param name="branchId">部门id</param>
        protected void LoadGroupData(Guid branchId)
        {
            var groupList = CacheCollection.Branch.GetSystemBranchListByBranchId(branchId);
            ddl_AssumeGroup.DataSource = groupList;
            ddl_AssumeGroup.DataTextField = "Name";
            ddl_AssumeGroup.DataValueField = "ID";
            ddl_AssumeGroup.DataBind();
            ddl_AssumeGroup.Items.Insert(0, new ListItem("请选择", ""));
        }

        //门店
        protected void LoadShopData()
        {
            var shopList = FilialeManager.GetAllianceFilialeList().Where(act => act.Rank == (int)FilialeRank.Partial
                        && act.ShopJoinType == (int)ShopJoinType.DirectSales).OrderBy(act => act.Name);

            ddl_AssumeShop.DataSource = shopList;
            ddl_AssumeShop.DataTextField = "Name";
            ddl_AssumeShop.DataValueField = "ID";
            ddl_AssumeShop.DataBind();
            ddl_AssumeShop.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 初始化页面数据
        //初始化页面数据
        protected void LoadReportData(CostReportInfo model)
        {
            rbl_ReportKind.SelectedValue = model.ReportKind.ToString();
            ddl_AssumeBranch.SelectedValue = model.AssumeBranchId.Equals(Guid.Empty) ? string.Empty : model.AssumeBranchId.ToString();
            ddl_AssumeGroup.SelectedValue = model.AssumeGroupId.Equals(Guid.Empty) ? string.Empty : model.AssumeGroupId.ToString();
            ddl_AssumeShop.SelectedValue = model.AssumeShopId.Equals(Guid.Empty) ? string.Empty : model.AssumeShopId.ToString();

            if ((model.ReportKind.Equals((int)CostReportKind.Before) && model.IsLastTime) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                if (model.CostsVarieties.Equals(0))
                {
                    CompanyClass.Visible = ImgCompanyClass.Visible = true;
                }
                else if (model.CostsVarieties.Equals(1))
                {
                    GoodsCode.Visible = txtGoodsCode.Visible = true;
                }
            }
            txt_GoodsCode.Text = model.GoodsCode;
            txt_CompanyClass.Text = Cost.ReadInstance.GetCompanyName(model.CompanyClassId, model.CompanyId);
            rbl_UrgentOrDefer.SelectedValue = model.UrgentOrDefer.ToString();
            txt_UrgentReason.Text = model.UrgentReason;
            txt_ReportName.Text = model.ReportName + (model.ApplyNumber > 1 ? "  " + ERP.UI.Web.Common.WebControl.ConvertToChnName(model.ApplyNumber) + "期" : string.Empty);
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportName.Text = "借款内容";
            }
            txt_StartTime.Text = model.StartTime.ToString("yyyy年MM月");
            txt_EndTime.Text = model.EndTime.ToString("yyyy年MM月");
            txt_PayCompany.Text = model.PayCompany;
            #region 预借款、凭证报销、已扣款核销 有“票据类型”
            txt_InvoiceTitle.ToolTip = txt_InvoiceTitle.Text = model.InvoiceTitle;
            rbl_InvoiceType.SelectedValue = string.Format("{0}", model.InvoiceType);
            if (model.InvoiceType.Equals((int)CostReportInvoiceType.Invoice) || model.InvoiceType.Equals((int)CostReportInvoiceType.VatInvoice))
            {
                InvoiceType.InnerText = "发票";
                InvoiceTitle.InnerHtml = "发票抬头：";

                BillNo.InnerText = "发票号码";
                BillCode.Visible = true;
                TaxAmount.InnerText = "含税金额";

                if (model.InvoiceType.Equals((int)CostReportInvoiceType.Invoice))
                {
                    NoTaxAmount.Visible = Tax.Visible = false;
                }
                else if (model.InvoiceType.Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    NoTaxAmount.Visible = Tax.Visible = true;
                }
            }
            else if (model.InvoiceType.Equals((int)CostReportInvoiceType.Voucher))
            {

                InvoiceType.InnerText = "收据";
                InvoiceTitle.InnerHtml = "收据抬头：";

                BillNo.InnerText = "收据号码";
                BillCode.Visible = false;
                TaxAmount.InnerText = "金额";
                NoTaxAmount.Visible = Tax.Visible = false;
            }
            #endregion
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportCost.Text = "预估申报";
            }
            txt_ReportCost.Text = ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(model.ReportCost));
            Lit_CapitalAmount.Text = Math.Abs(model.ReportCost).ToString(CultureInfo.InvariantCulture);
            rbl_CostType.SelectedValue = model.CostType.ToString();
            if (model.CostType.Equals(2))//转账
            {
                if (!string.IsNullOrEmpty(model.BankAccountName))
                {
                    txt_BankName.Text = model.BankAccountName.Split(',')[0];
                    txt_SubBankName.Text = model.BankAccountName.Split(',')[1];
                }
                if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
                {
                    txt_BankAccount.Text = model.BankAccount;
                    BankName.Visible = txtBankName.Visible = SubBankName.Visible = txtSubBankName.Visible = BankAccount.Visible = txtBankAccount.Visible = true;
                }
                else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
                {
                    BankName.Visible = txtBankName.Visible = SubBankName.Visible = txtSubBankName.Visible = true;
                }
            }
            if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                lit_BankName.Text = lit_SubBankName.Text = "收款";
            }
            else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_BankName.Text = lit_SubBankName.Text = "付款";

                PayBankAccountAndAssumeFiliale.Visible = true;
                if (!model.PayBankAccountId.Equals(Guid.Empty))
                {
                    var bankAccountInfo = _bankAccounts.GetBankAccounts(model.PayBankAccountId);
                    string payBankAccount = bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】";
                    txt_PayBankAccount.ToolTip = txt_PayBankAccount.Text = payBankAccount;
                    txt_AssumeFiliale.Text = CacheCollection.Filiale.GetFilialeNameAndFilialeId(model.PayBankAccountId).Split(',')[0];
                }
            }

            //押金回收生成的“凭证报销”和“费用收入”有押金编号，此处要显示
            if (!string.IsNullOrEmpty(model.DepositNo))
            {
                DepositNo.Visible = txtDepositNo.Visible = true;
                txt_DepositNo.Text = model.DepositNo;
            }

            txt_ReportMemo.Text = model.ReportMemo;

            #region 广告使用图片(广告费/购买物品)
            if (!model.InvoiceId.Equals(Guid.Empty))
            {
                var costReportInvoice = _costReportInvoice.GetInvoice(model.InvoiceId);
                if (costReportInvoice != null && !costReportInvoice.InvoiceId.Equals(Guid.Empty))
                {
                    PreA.HRef = costReportInvoice.ImagePath;
                    PreA.Visible = true;
                    UploadImgName.Text = costReportInvoice.ImagePath.Substring(costReportInvoice.ImagePath.LastIndexOf('/') + 1);
                }
            }
            #endregion

            #region 差旅费
            if (((model.ReportKind.Equals((int)CostReportKind.Before) && model.IsLastTime) || model.ReportKind == (int)CostReportKind.Later) && !model.CompanyId.Equals(Guid.Empty) && model.CompanyId.Equals(new Guid(CostReport_TravelExpenses)))
            {
                //获取差旅费
                var costReportTravelList = _costReportTravel.GetlmShop_CostReportTravelByReportId(model.ReportId);
                if (costReportTravelList.Any())
                {
                    LoadTravelData(costReportTravelList);
                }
                //获取起讫
                CostReportTerminiInfoList = _costReportTermini.GetmShop_CostReportTerminiByReportId(model.ReportId);
                if (CostReportTerminiInfoList.Any())
                {
                    RepeaterTerminiDataBind();
                }
                txt_CompanyClass.Style["color"] = "red";
                txt_CompanyClass.Style["font-weight"] = "bold";
                MessageBox.AppendScript(this, "$(\"#TraveA\").show();$(\"#TraveDetail\").show();$(\"input[id$='Hid_Travel']\").val(\"0\");");
            }
            #endregion

            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                if (!model.PayCost.Equals(0))
                {
                    IsLastTime.Visible = true;
                    rb_IsLastTime.SelectedValue = model.IsLastTime.ToString();
                    if (bool.Parse(rb_IsLastTime.SelectedValue))
                    {
                        #region 是否终结
                        rbIsEndTitle.Visible = rbIsEnd.Visible = true;
                        rb_IsEnd.SelectedValue = model.IsEnd.ToString();
                        #endregion
                    }
                }
                #region 申请金额
                //获取申请金额
                CostReportAmountInfoList = _costReportAmount.GetmShop_CostReportAmountByReportId(model.ReportId).Where(p => !p.IsSystem).ToList();
                if (CostReportAmountInfoList.Any())
                {
                    RepeaterAmountDataBind();
                    Amount.Visible = true;
                }
                #endregion
            }

            if (string.IsNullOrEmpty(model.DepositNo) && (model.ReportKind.Equals((int)CostReportKind.FeeIncome) || model.CompanyClassId.Equals(new Guid(CostReport_DepositMargins))))
            {
                Amount.Visible = false;
            }

            #region 票据类型相关
            //获取票据
            CostReportBillInfoList = _costReportBill.Getlmshop_CostReportBillByReportId(model.ReportId);
            if (CostReportBillInfoList.Any())
            {
                RepeaterBillDataBind();
                Bill.Visible = true;
            }
            #endregion
        }

        //加载差旅费
        protected void LoadTravelData(List<CostReportTravelInfo> costReportTravelInfoList)
        {
            txt_Entourage.Text = costReportTravelInfoList[0].Entourage;
            txt_TravelAddressAndCourse.Text = costReportTravelInfoList[0].TravelAddressAndCourse;

            foreach (var item in costReportTravelInfoList)
            {
                if (item.Matter == 0)
                {
                    txt_TrainChargeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_TrainChargeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 1)
                {
                    txt_CarFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_CarFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 2)
                {
                    txt_CityFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_CityFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 3)
                {
                    txt_TollsNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_TollsAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 4)
                {
                    txt_AircraftFeeNum.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_AircraftFeeAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 5)
                {
                    txt_MealsDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_MealsAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 6)
                {
                    txt_AccommodationDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_AccommodationAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else if (item.Matter == 7)
                {
                    txt_OtherDays.Text = item.DayOrNum.ToString(CultureInfo.InvariantCulture);
                    txt_OtherAmount.Text = item.Amount.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        #endregion

        #region 数据列表相关
        #region 申请金额
        //申请金额数据源
        protected void RepeaterAmountDataBind()
        {
            Repeater_Amount.DataSource = CostReportAmountInfoList.OrderBy(p => p.Num);
            Repeater_Amount.DataBind();
            lit_SumAmount.Text = ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(CostReportAmountInfoList.Sum(p => p.Amount)) + "元";
        }
        #endregion

        #region 票据(发票/收据)
        //票据数据源
        protected void RepeaterBillDataBind()
        {
            Repeater_Bill.DataSource = CostReportBillInfoList.OrderByDescending(p => p.OperatingTime).ThenByDescending(p => p.IsPay);
            Repeater_Bill.DataBind();
            lit_SumNoTaxAmount.Text = ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.NoTaxAmount)) + "元";
            lit_SumTax.Text = ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.Tax)) + "元";
            lit_SumTaxAmount.Text = ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.TaxAmount)) + "元";
        }

        #region 列表操作
        //行绑定事件
        protected void Repeater_Bill_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtBillCode = e.Item.FindControl("txtBillCode");
                var txtNoTaxAmount = e.Item.FindControl("txtNoTaxAmount");
                var txtTax = e.Item.FindControl("txtTax");

                if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Invoice))
                {
                    txtBillCode.Visible = true;
                    txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = false;
                    SumTitle.ColSpan = 4;
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    txtBillCode.Visible = true;
                    txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = true;
                    SumTitle.ColSpan = 4;
                }
                else if (int.Parse(rbl_InvoiceType.SelectedValue).Equals((int)CostReportInvoiceType.Voucher))
                {
                    txtBillCode.Visible = txtNoTaxAmount.Visible = txtTax.Visible = litSumNoTaxAmount.Visible = litSumTax.Visible = false;
                    SumTitle.ColSpan = 3;
                }
            }
        }
        #endregion
        #endregion

        #region 差旅费起讫
        //起讫数据源
        protected void RepeaterTerminiDataBind()
        {
            Repeater_Termini.DataSource = CostReportTerminiInfoList;
            Repeater_Termini.DataBind();
        }
        #endregion
        #endregion

        //受理通过
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.NoAuditing)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            bool isPayBill = false;//是否将票据设置成付款完成
            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model, out isPayBill);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model);
            }
            else if (model.ReportKind == (int)CostReportKind.Paying)
            {
                isPayBill = true;
                //已扣款核销
                PayVerification(model);
            }
            model.AuditingMemo = txt_AuditingMemo.Text;
            model.AcceptDate = DateTime.Now;

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReportBill.Updatelmshop_CostReportBillForPassByReportId(model.ReportId, true);
                    if (isPayBill)
                    {
                        _utility.UpdateFieldByPk("lmShop_CostReportBill", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                    }
                    _costReport.UpdateReport(model);

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
            }
            #endregion
        }

        //受理不通过
        protected void btn_NoPass_Click(object sender, EventArgs e)
        {
            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.NoAuditing)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            var state = (int)CostReportState.InvoiceNoPass;
            var memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据不合格;受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model.ReportId, state, txt_AuditingMemo.Text, memo, Guid.Empty);
                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
            }
            #endregion
        }

        #region 预借款
        protected void BeforeLoan(CostReportInfo model, out bool isPayBill)
        {
            //获取未付款的申请金额的数据
            var isPayAmountList = CostReportAmountInfoList.Where(p => !p.IsPay);
            if (isPayAmountList.Any())
            {
                model.State = (int)CostReportState.AlreadyAuditing;
                isPayBill = false;
            }
            else
            {
                if (model.IsLastTime)
                {
                    //预借款时，可能出现(“票据金额”=“申请金额”,“票据金额”>“申请金额”,“票据金额”<“申请金额”)三种情况
                    var difference = model.ApplyForCost - model.ActualAmount;//“票据金额”和“申请金额”的差额
                    if (difference == 0 || difference > 0)
                    {
                        model.RealityCost = difference;
                        model.State = (int)CostReportState.Complete;
                        model.FinishDate = DateTime.Now;
                        model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
                    }
                    else if (difference < 0)
                    {
                        model.RealityCost = difference;
                        model.ReportMemo = "打款金额多了" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(difference)) + "元";
                        model.State = (int)CostReportState.WaitVerify;
                        model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;" + model.ReportMemo + ";受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");

                        #region 插入系统生成的金额
                        var maxNum = CostReportAmountInfoList.Any() ? CostReportAmountInfoList.Select(p => p.Num).Max() : 0;
                        CostReportAmountInfoList.Add(new CostReportAmountInfo
                        {
                            AmountId = Guid.NewGuid(),
                            ReportId = Guid.Empty,
                            Num = maxNum + 1,
                            Amount = difference,
                            IsPay = true,
                            IsSystem = true
                        });
                        model.IsSystem = true;
                        #endregion
                    }
                }
                else
                {
                    model.ApplyNumber = ++model.ApplyNumber;
                    model.State = (int)CostReportState.CompletedMayApply;
                }
                isPayBill = true;
            }
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model)
        {
            if (string.IsNullOrEmpty(model.DepositNo))//正常流程的“凭证报销”
            {
                model.State = (int)CostReportState.AlreadyAuditing;
            }
            else//押金回收的“凭证报销”
            {
                model.State = (int)CostReportState.Complete;
            }
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 已扣款核销
        protected void PayVerification(CostReportInfo model)
        {
            model.State = (int)CostReportState.Complete;
            model.FinishDate = DateTime.Now;
            model.Memo = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("[【票据受理】:票据合格;受理说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion
    }
}