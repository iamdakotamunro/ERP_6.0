using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using Cost = ERP.BLL.Implement.Inventory.Cost;
using WebControl = ERP.UI.Web.Common.WebControl;
using System.Collections.Generic;
using Telerik.Web.UI;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;

namespace ERP.UI.Web.CostReport
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2015/11/11  
     * 描述    :费用申报收付款执行
     * =====================================================================
     * 修改时间：2015/11/11  
     * 修改人  ：  
     * 描述    ：
     */
    public partial class CostReport_ReceivablesPayment : WindowsPage
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
        private static readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReckoning _costReckoning = new CostReckoning(GlobalConfig.DB.FromType.Write);
        private static readonly ICostReportInvoice _costReportInvoice = new CostReportInvoice(GlobalConfig.DB.FromType.Read);
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
        /// ERP公司
        /// </summary>
        private string ErpFiliale
        {
            get
            {
                if (ViewState["ERPFiliale"] == null)
                {
                    ViewState["ERPFiliale"] = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID");
                }
                return ViewState["ERPFiliale"].ToString();
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

        /// <summary>
        /// 结算账号列表属性
        /// </summary>
        protected Dictionary<Guid, string> BankAccountInfoDic
        {
            get
            {
                if (ViewState["BankAccountInfoDic"] == null)
                {
                    return _bankAccounts.GetBankAccountsList(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse).ToDictionary(p => p.BankAccountsId, p => p.BankName + "【" + p.AccountsName + "】");
                }
                return ViewState["BankAccountInfoDic"] as Dictionary<Guid, string>;
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
                LoadAssumeFilialeData(costReportInfo);//结算公司
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

        //结算公司
        protected void LoadAssumeFilialeData(CostReportInfo model)
        {
            var list = CacheCollection.Filiale.GetHeadList();
            if (!model.AssumeFilialeId.Equals(Guid.Empty) && model.InvoiceTitleFilialeId.Equals(Guid.Empty) && !model.AssumeFilialeId.Equals(new Guid(ErpFiliale)))
            {
                list = list.Where(p => p.ID.Equals(model.AssumeFilialeId)).ToList();
            }
            else if (!model.InvoiceTitleFilialeId.Equals(new Guid(ErpFiliale)))
            {
                list = list.Where(p => p.ID.Equals(model.InvoiceTitleFilialeId)).ToList();
            }
            list.Add(new FilialeInfo { ID = new Guid(ErpFiliale), RealName = "ERP公司" });

            ddl_AssumeFiliale.DataSource = list;
            ddl_AssumeFiliale.DataTextField = "RealName";
            ddl_AssumeFiliale.DataValueField = "ID";
            ddl_AssumeFiliale.DataBind();
            ddl_AssumeFiliale.Items.Insert(0, new ListItem("请选择", ""));

            LoadPayBankAccountData(model.AssumeFilialeId);
        }

        //结算账号
        protected void LoadPayBankAccountData(Guid filialeId)
        {
            if (!filialeId.Equals(Guid.Empty))
            {
                var list = _bankAccounts.GetBankAccountsList(Personnel.FilialeId, Personnel.BranchId, Personnel.PositionId).Where(ent => ent.IsUse);

                //根据公司id查找该公司绑定的所有银行账号
                var bankAccountInfoList = BankAccountManager.ReadInstance.GetListByTargetId(filialeId);
                if (bankAccountInfoList.Any())
                {
                    //在当前登录人有权限的银行列表中，筛选出所有与发票抬头公司id相等的“结算账号”
                    var joinQuery = (from bankAccountInfo in bankAccountInfoList
                                     join listInfo in list
                                     on bankAccountInfo.BankAccountsId equals listInfo.BankAccountsId
                                     select new BankAccountInfo
                                     {
                                         BankName = listInfo.BankName,
                                         AccountsName = listInfo.AccountsName,
                                         BankAccountsId = listInfo.BankAccountsId
                                     }).ToList();
                    list = joinQuery;
                }
                else
                {
                    list = list.Where(p => !p.IsMain);
                }

                foreach (var info in list)
                {
                    var name = info.BankName + "【" + info.AccountsName + "】";
                    rcb_PayBankAccount.Items.Add(new RadComboBoxItem(name, info.BankAccountsId.ToString()));
                }
            }
            rcb_PayBankAccount.Items.Insert(0, new RadComboBoxItem("请选择", ""));
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
            txt_GoodsCode.Text = model.GoodsCode;
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
            txt_CompanyClass.Text = Cost.ReadInstance.GetCompanyName(model.CompanyClassId, model.CompanyId);
            rbl_UrgentOrDefer.SelectedValue = model.UrgentOrDefer.ToString();
            txt_UrgentReason.Text = model.UrgentReason;
            txt_ReportName.Text = model.ReportName + (model.ApplyNumber > 1 ? "  " + WebControl.ConvertToChnName(model.ApplyNumber) + "期" : string.Empty);
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportName.Text = "借款内容";
            }
            txt_StartTime.Text = model.StartTime.ToString("yyyy年MM月");
            txt_EndTime.Text = model.EndTime.ToString("yyyy年MM月");
            txt_PayCompany.Text = model.PayCompany;
            //“申报类型”是费用收入时，“收款人”变成“付款人”
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_PayCompany.Text = "付款";
            }
            else
            {
                #region 预借款、凭证报销、已扣款核销 有“票据类型”

                BillType.Visible = true;
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
            }
            if (model.ReportKind.Equals((int)CostReportKind.Before))
            {
                lit_ReportCost.Text = "预估申报";
            }
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_Amount.Text = "收款";
            }
            else
            {
                if (model.RealityCost > 0)
                {
                    lit_Amount.Text = "付款";
                }
                else if (model.RealityCost < 0)
                {
                    lit_Amount.Text = "收款";
                }
            }
            txt_Amount.Text = WebControl.RemoveDecimalEndZero(Math.Abs(model.RealityCost));
            txt_ReportCost.Text = WebControl.RemoveDecimalEndZero(Math.Abs(model.ReportCost));
            Lit_CapitalAmount.Text = Math.Abs(model.ReportCost).ToString(CultureInfo.InvariantCulture);
            rbl_CostType.SelectedValue = model.CostType.ToString();
            if (model.CostType.Equals(2)) //转账
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
            txt_Poundage.Text = model.Poundage.ToString(CultureInfo.InvariantCulture);
            txt_TradeNo.Text = model.TradeNo;
            if (model.ReportKind.Equals((int)CostReportKind.Before) || model.ReportKind.Equals((int)CostReportKind.Later))
            {
                lit_BankName.Text = lit_SubBankName.Text = "收款";
            }
            else if (model.ReportKind.Equals((int)CostReportKind.Paying) || model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                lit_BankName.Text = lit_SubBankName.Text = "付款";
            }
            if (!model.PayBankAccountId.Equals(Guid.Empty))
            {
                ddl_AssumeFiliale.SelectedValue = model.AssumeFilialeId.ToString();
                rcb_PayBankAccount.SelectedValue = model.PayBankAccountId.ToString();

                #region  当是第一次付款，“申报类型”是预借款或者凭证报销，状态是“待付款”或者“待收款确认”时，“结算公司”和“结算账号”是可选的
                if (model.State.Equals((int)CostReportState.AlreadyAuditing) ||
                    model.State.Equals((int)CostReportState.WaitVerify))
                {
                    if (model.ReportKind == (int)CostReportKind.Before && !model.PayCost.Equals(0))
                    {
                        ddl_AssumeFiliale.Enabled = rcb_PayBankAccount.Enabled = false;
                    }
                    else if (model.ReportKind == (int)CostReportKind.FeeIncome)
                    {
                        ddl_AssumeFiliale.Enabled = rcb_PayBankAccount.Enabled = false;
                    }
                }
                else
                {
                    ddl_AssumeFiliale.Enabled = rcb_PayBankAccount.Enabled = false;
                }
                #endregion
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

            if (model.State == (int)CostReportState.AlreadyAuditing)
            {
                btn_Pay.Text = lit_AuditingMemo.Text = "付款";
            }
            else if (model.State == (int)CostReportState.WaitVerify)
            {
                btn_Pay.Text = lit_AuditingMemo.Text = "收款";
            }
            else if (model.State == (int)CostReportState.Pay)
            {
                btn_Pay.Text = lit_AuditingMemo.Text = "完成";
                btn_Save.Visible = false;
                btn_Back.Visible = false;
                btn_BackToPre.Visible = true;
            }

            if (model.IsSystem && model.RealityCost < 0)
            {
                btn_Back.Visible = false;
            }
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

        #region SelectedIndexChanged事件
        //部门选择
        protected void ddl_AssumeBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ddl_AssumeBranch.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                LoadGroupData(new Guid(selectedValue));
                ddl_AssumeShop.SelectedValue = string.Empty;
                AssumeShop.Visible = false;
            }
        }

        //小组选择
        protected void ddl_AssumeGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            var assumeBranch = ddl_AssumeBranch.SelectedValue;
            if (assumeBranch.Equals(ShopBranchId))
            {
                var selectedValue = ddl_AssumeGroup.SelectedValue;
                if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
                {
                    LoadShopData();
                    AssumeShop.Visible = true;
                }
                else
                {
                    ddl_AssumeShop.SelectedValue = string.Empty;
                    AssumeShop.Visible = false;
                }
            }
        }

        //结算公司选择
        protected void ddl_AssumeFiliale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ddl_AssumeFiliale.SelectedValue;
            rcb_PayBankAccount.Text = string.Empty;
            rcb_PayBankAccount.Items.Clear();
            if (!string.IsNullOrEmpty(selectedValue) && new Guid(selectedValue) != Guid.Empty)
            {
                LoadPayBankAccountData(new Guid(selectedValue));
            }
        }

        //结算账号搜索
        protected void rcb_PayBankAccount_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var bankAccountDic = BankAccountInfoDic.Where(p => p.Value.Contains(e.Text));
                Int32 totalCount = BankAccountInfoDic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in bankAccountDic)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Value, item.Key.ToString()));
                    }
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
            lit_SumAmount.Text = WebControl.RemoveDecimalEndZero(CostReportAmountInfoList.Sum(p => p.Amount)) + "元";
        }
        #endregion

        #region 票据(发票/收据)
        //票据数据源
        protected void RepeaterBillDataBind()
        {
            Repeater_Bill.DataSource = CostReportBillInfoList.OrderByDescending(p => p.OperatingTime).ThenByDescending(p => p.IsPay);
            Repeater_Bill.DataBind();
            lit_SumNoTaxAmount.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.NoTaxAmount)) + "元";
            lit_SumTax.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.Tax)) + "元";
            lit_SumTaxAmount.Text = WebControl.RemoveDecimalEndZero(CostReportBillInfoList.Sum(p => p.TaxAmount)) + "元";
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

        //付款
        protected void btn_Pay_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            #region 是否是“完成打款”页面调用
            bool isFinish = false;
            var type = Request.QueryString["Type"];
            if (type != null && type.Equals("Finish"))
            {
                isFinish = true;
            }
            #endregion

            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (isFinish)
            {
                if (model.State != (int)CostReportState.Pay)
                {
                    MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                    return;
                }
            }
            else
            {
                if (model.State != (int)CostReportState.AlreadyAuditing && model.State != (int)CostReportState.WaitVerify)
                {
                    MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                    return;
                }
            }

            bool execute = false;//是否执行完成操作
            bool need = false;//是否需要完成
            decimal realityCost = model.RealityCost;
            var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));

            #region 结算账号
            string payBankAccount = bankAccountInfo == null ? "暂无结算" : (bankAccountInfo.BankName + "【" + bankAccountInfo.AccountsName + "】");
            #endregion

            if (isFinish)
            {
                need = true;
            }
            else
            {
                if (model.CostType.Equals(1) || (model.CostType.Equals(2) && (bankAccountInfo != null && !bankAccountInfo.IsFinish)))
                {
                    //在申请页面完成
                    need = true;
                }
            }

            if (model.ReportKind == (int)CostReportKind.Before)
            {
                //预借款
                BeforeLoan(model, need, payBankAccount, out execute);
            }
            else if (model.ReportKind == (int)CostReportKind.Later)
            {
                //凭证报销
                VoucherPay(model, need, payBankAccount);
            }
            else if (model.ReportKind == (int)CostReportKind.FeeIncome)
            {
                execute = true;
                //费用收入
                FeeIncome(model, payBankAccount);
            }

            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.PayBankAccountId = string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue) ? Guid.Empty : new Guid(rcb_PayBankAccount.SelectedValue);
            model.AssumeFilialeId = string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeFiliale.SelectedValue);
            model.Poundage = string.IsNullOrEmpty(txt_Poundage.Text) ? 0 : decimal.Parse(txt_Poundage.Text);
            model.TradeNo = txt_TradeNo.Text;
            model.AuditingMemo = txt_AuditingMemo.Text;
            if (bankAccountInfo != null && bankAccountInfo.IsMain)
            {
                model.IsOut = true;
            }
            else
            {
                model.IsOut = false;
            }

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateReport(model);
                    if (execute || need)
                    {
                        _costReport.UpdatePayCostAndExecuteDate(model.ReportId, realityCost);
                        _utility.UpdateFieldByPk("lmShop_CostReportAmount", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                        _utility.UpdateFieldByPk("lmShop_CostReportBill", "IsPay", new object[] { true }, "ReportId", model.ReportId.ToString());
                        model.RealityCost = realityCost;
                        string errorMessage;
                        bool result = ExecuteFinishHandle(model, Personnel, out errorMessage);
                        if (!result)
                        {
                            throw new Exception(errorMessage);
                        }
                    }

                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, "保存失败！"+ex.Message);
                }
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }

        //保存数据
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            //CostReportState.WaitVerify 此状态对于“个人”是【待付款确认】，对于“公司”是【待收款确认】
            CostReportInfo model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (model.State != (int)CostReportState.AlreadyAuditing && model.State != (int)CostReportState.WaitVerify)
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }

            model.AssumeBranchId = string.IsNullOrEmpty(ddl_AssumeBranch.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeBranch.SelectedValue);
            model.AssumeGroupId = string.IsNullOrEmpty(ddl_AssumeGroup.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeGroup.SelectedValue);
            model.AssumeShopId = string.IsNullOrEmpty(ddl_AssumeShop.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeShop.SelectedValue);
            model.PayBankAccountId = string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue) ? Guid.Empty : new Guid(rcb_PayBankAccount.SelectedValue);
            model.AssumeFilialeId = string.IsNullOrEmpty(ddl_AssumeFiliale.SelectedValue) ? Guid.Empty : new Guid(ddl_AssumeFiliale.SelectedValue);
            model.Poundage = string.IsNullOrEmpty(txt_Poundage.Text) ? 0 : decimal.Parse(txt_Poundage.Text);
            model.TradeNo = txt_TradeNo.Text;
            model.AuditingMemo = txt_AuditingMemo.Text;
            model.Memo = WebControl.RetrunUserAndTime("[【保存数据】:已保存;" + lit_AuditingMemo.Text + "说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
            var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));
            if (bankAccountInfo != null && bankAccountInfo.IsMain)
            {
                model.IsOut = true;
            }
            else
            {
                model.IsOut = false;
            }

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
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
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }

        //退回;
        protected void btn_Back_Click(object sender, EventArgs e)
        {
            var model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (!model.State.Equals((int)CostReportState.AlreadyAuditing) && !model.State.Equals((int)CostReportState.WaitVerify) && !model.State.Equals((int)CostReportState.Pay))
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }
            var state = (int)CostReportState.AuditingNoPass;
            var memo = WebControl.RetrunUserAndTime("[【收/付款】:退回;退回说明:" + (string.IsNullOrEmpty(txt_BackMemo.Text) ? "暂无说明" : txt_BackMemo.Text) + ";]");

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReportBill.Updatelmshop_CostReportBillForPassByReportId(model.ReportId, false);
                    _costReport.UpdateRealityCost(model.ReportId, state, model.RealityCost, memo);
                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }

        //退回到“待付款”(用于完成收/付款);
        protected void btn_BackToPre_Click(object sender, EventArgs e)
        {
            var model = _costReport.GetReportByReportId(new Guid(Request.QueryString["ReportId"]));
            if (!model.State.Equals((int)CostReportState.Pay))
            {
                MessageBox.Show(this, "该单据状态已更新，不允许此操作！");
                return;
            }
            var state = (int)CostReportState.AlreadyAuditing;
            var memo = WebControl.RetrunUserAndTime("[【完成收/付款】:退回;退回说明:" + (string.IsNullOrEmpty(txt_BackMemo.Text) ? "暂无说明" : txt_BackMemo.Text) + ";]");

            #region 保存数据
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _costReport.UpdateRealityCost(model.ReportId, state, model.RealityCost, memo);
                    //添加操作日志
                    _operationLogManager.Add(Personnel.PersonnelId, Personnel.RealName, model.ReportId, model.ReportNo, OperationPoint.CostDeclare.AuditDeclare.GetBusinessInfo(), 1, "");

                    ts.Complete();
                    MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    MessageBox.Show(this, "保存失败！");
                }
                finally
                {
                    ts.Dispose();
                }
            }
            #endregion
        }

        #region 预借款
        protected void BeforeLoan(CostReportInfo model, bool need, string payBankAccount, out bool execute)
        {
            execute = false;
            if (need || (model.IsSystem && model.RealityCost < 0))
            {
                if (model.Deposit.Equals(1))
                {
                    model.State = (int)CostReportState.Complete;
                    model.FinishDate = DateTime.Now;
                    model.DepositNo = new CodeManager().GetCode(CodeType.RE);
                    model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
                }
                else
                {
                    if (model.IsSystem)
                    {
                        if (model.RealityCost > 0)
                        {
                            model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
                        }
                        else if (model.RealityCost < 0)
                        {
                            execute = true;
                            model.Memo = WebControl.RetrunUserAndTime("[【已收款】:已收入" + WebControl.RemoveDecimalEndZero(Math.Abs(model.RealityCost)) + "元;结算账号:" + payBankAccount + ";收款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
                        }
                        model.State = (int)CostReportState.Complete;
                        model.FinishDate = DateTime.Now;
                    }
                    else if (model.IsLastTime)
                    {
                        model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");

                        //预借款时，可能出现(“票据金额”=“申请金额”,“票据金额”>“申请金额”,“票据金额”<“申请金额”)三种情况
                        var difference = model.ApplyForCost - model.ActualAmount;//“票据金额”和“申请金额”的差额
                        if (difference == 0 || difference > 0)
                        {
                            model.RealityCost = difference;
                            model.State = (int)CostReportState.Complete;
                            model.FinishDate = DateTime.Now;
                        }
                        else if (difference < 0)
                        {
                            model.RealityCost = difference;
                            model.ReportMemo = "打款金额多了" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(difference)) + "元";
                            model.State = (int)CostReportState.WaitVerify;

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
                        model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
                    }
                }
            }
            else
            {
                model.State = (int)CostReportState.Pay;
                model.Memo = WebControl.RetrunUserAndTime("[【待付款】:待支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;待付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
            }
        }
        #endregion

        #region 凭证报销
        protected void VoucherPay(CostReportInfo model, bool need, string payBankAccount)
        {
            if (need)
            {
                model.State = (int)CostReportState.Complete;
                model.FinishDate = DateTime.Now;
                model.Memo = WebControl.RetrunUserAndTime("[【已付款】:已支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
            }
            else
            {
                model.State = (int)CostReportState.Pay;
                model.Memo = WebControl.RetrunUserAndTime("[【待付款】:待支付" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;待付款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
            }
        }
        #endregion

        #region 费用收入
        protected void FeeIncome(CostReportInfo model, string payBankAccount)
        {
            model.State = (int)CostReportState.Complete;
            model.FinishDate = DateTime.Now;
            model.Memo = WebControl.RetrunUserAndTime("[【已收款】:已收入" + WebControl.RemoveDecimalEndZero(model.RealityCost) + "元;结算账号:" + payBankAccount + ";收款说明:" + (string.IsNullOrEmpty(txt_AuditingMemo.Text) ? "暂无说明" : txt_AuditingMemo.Text) + ";]");
        }
        #endregion

        #region 执行完成时的操作
        protected bool ExecuteFinishHandle(CostReportInfo model, PersonnelInfo personnelInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                model.RealityCost = -model.RealityCost;
            }

            var costReportBll = new BLL.Implement.Inventory.CostReport(_bankAccounts, _personnelSao, _costReckoning);
            #region 新增资金流
            var wasteBookInfo = costReportBll.AddWasteBookInfo(model, personnelInfo, false);
            if (_wasteBook.Insert(wasteBookInfo) <= 0)
            {
                errorMsg = "新增资金流失败！";
                return false;
            }
            #endregion

            #region 新增帐务记录
            var costReckoningInfo = costReportBll.AddCostReckoningInfo(model, personnelInfo, true);
            if (_costReckoning.Insert(costReckoningInfo) <= 0)
            {
                errorMsg = "新增帐务记录失败！";
                return false;
            }
            #endregion

            #region 与门店费用交互
            if (model.AssumeBranchId == new Guid(ShopBranchId) && !model.AssumeShopId.Equals(Guid.Empty))
            {
                string strErrorMsg;
                var description = string.Format("费用申报{0}{1}可用余额!", model.ReceiptNo, model.RealityCost < 0 ? "添加" : "扣除");
                var parentId = FilialeManager.GetShopHeadFilialeId(model.AssumeShopId);//获取门店所属公司
                if (!ShopSao.DeductBalance(parentId, model.AssumeShopId, model.RealityCost, description, out strErrorMsg))
                {
                    errorMsg = strErrorMsg;
                    return false;
                }

                var costRecordDto = costReportBll.AddCostRecordDto(model);
                if (!ShopSao.InsertCostRecord(parentId, costRecordDto))
                {
                    errorMsg = "扣除门店费用失败！";
                    return false;
                }
            }
            #endregion

            #region 新增资金流(手续费)
            if (model.RealityCost >= 0 && model.Poundage > 0 && !model.ReportKind.Equals((int)CostReportKind.FeeIncome))
            {
                var wasteBookInfoPoundage = costReportBll.AddWasteBookInfo(model, personnelInfo, true);
                if (_wasteBook.Insert(wasteBookInfoPoundage) <= 0)
                {
                    errorMsg = "新增手续费失败！";
                    return false;
                }
            }
            #endregion

            return true;
        }
        #endregion

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();

            #region 公共数据
            var assumeBranchId = ddl_AssumeBranch.SelectedValue;
            if (string.IsNullOrEmpty(assumeBranchId))
            {
                errorMsg.Append("请选择“申报部门”！").Append("\\n");
            }
            var bankAccount = rcb_PayBankAccount.SelectedValue;
            if (string.IsNullOrEmpty(bankAccount))
            {
                errorMsg.Append("请选择“结算账号”！").Append("\\n");
            }
            var assumeFilialeId = ddl_AssumeFiliale.SelectedValue;
            if (string.IsNullOrEmpty(assumeFilialeId) || new Guid(assumeFilialeId).Equals(Guid.Empty))
            {
                errorMsg.Append("请填写“结算公司”！").Append("\\n");
            }
            var tradeNo = txt_TradeNo.Text;
            if (string.IsNullOrEmpty(tradeNo))
            {
                errorMsg.Append("请填写“交易流水号”！").Append("\\n");
            }
            var poundage = txt_Poundage.Text;
            if (!string.IsNullOrEmpty(poundage))
            {
                var result = WebControl.CheckPoundage(Decimal.Parse(txt_ReportCost.Text), Decimal.Parse(poundage));
                if (!result)
                {
                    errorMsg.Append("“手续费”【应小于等于6】或者【不超过付款金额的2%】！").Append("\\n");
                }
            }
            #endregion

            #region 如果“票据类型”是“收据”且收据抬头是“ERP公司”，则“结算账号”必须是ERP公司所属“结算账号”
            var invoiceType = rbl_InvoiceType.SelectedValue;
            if (!string.IsNullOrEmpty(invoiceType) && int.Parse(invoiceType) == (int)CostReportInvoiceType.Voucher && !string.IsNullOrEmpty(rcb_PayBankAccount.SelectedValue))
            {
                var invoiceTitleFilialeId = Hid_InvoiceTitleFilialeId.Value;
                if (invoiceTitleFilialeId.Equals(ErpFiliale))
                {
                    var bankAccountInfo = _bankAccounts.GetBankAccounts(new Guid(rcb_PayBankAccount.SelectedValue));
                    if (bankAccountInfo != null && bankAccountInfo.IsMain)
                    {
                        errorMsg.Append("请选择ERP公司所属“结算账号”！").Append("\\n");
                    }
                }
            }
            #endregion

            return errorMsg.ToString();
        }
    }
}