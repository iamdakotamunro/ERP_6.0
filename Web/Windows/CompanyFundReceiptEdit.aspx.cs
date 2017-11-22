using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Lonme.WebControls;
using MIS.Enum;
using OperationLog.Core;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyFundReceiptEdit : WindowsPage
    {
        protected PersonnelInfo PersonnelInfoModel;
        protected CodeManager CodeBll = new CodeManager();
        private readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);
        //其他公司
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        protected CompanyFundReceiptInfo CompanyFundReceiptInfoModel;

        #region -- Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    HF_ReceiptID.Value = Request.QueryString["ID"];
                    var companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Read);
                    CompanyFundReceiptInfoModel = companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(Request.QueryString["ID"]));
                }

                RcbReceiveType.Items.Add(new RadComboBoxItem("往来单位", "0"));
                RcbReceiveType.Items.Add(new RadComboBoxItem("劳务", "1"));

                if (CompanyFundReceiptInfoModel.IsServiceFee)
                {
                    BindShopList(CompanyFundReceiptInfoModel.CompanyID);
                    BindSaleFiliale(CompanyFundReceiptInfoModel.FilialeId);
                    RtbAmount.Text = string.Format("{0}", CompanyFundReceiptInfoModel.RealityBalance);
                    div_Service.Visible = true;
                    div_Normal.Visible = false;
                    RcbReceiveType.SelectedValue = "1";
                    PL_ReceiveGoods.Visible = false;

                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId, DateTime.Now);
                    RadBalance.Text = totalNumber != 0 ? (totalNumber).ToString("#.00") : "0.00";
                }
                else
                {
                    RCB_CompanyList_Bind();
                    BindValue();
                    RCB_CompanyList.Enabled = false;
                    RCB_FilialeList.Enabled = false;
                    div_Service.Visible = false;
                    div_Normal.Visible = true;
                    RcbReceiveType.SelectedValue = "0";
                }
                var flag = Request.QueryString["Flag"];
                if (flag != null && flag.Equals("1"))//type=1表示查看
                {
                    LB_Save.Visible = LB_Cancel.Visible = false;
                    RtbAmount.Enabled = false;
                }
            }
        }
        #endregion

        #region -- 页面载入时，指定需要修改的往来单位收付款单据信息绑定到相应的控件里
        protected void BindValue()
        {
            if (CompanyFundReceiptInfoModel != null)
            {
                ShowFiliale();
                RTB_PurchaseOrderNo.Text = CompanyFundReceiptInfoModel.PurchaseOrderNo;
                RCB_CompanyList.SelectedValue = CompanyFundReceiptInfoModel.CompanyID.ToString();
                RCB_FilialeList.SelectedValue = CompanyFundReceiptInfoModel.FilialeId.ToString();
                RDP_StartDate.SelectedDate = CompanyFundReceiptInfoModel.SettleStartDate;
                RDP_EndDate.SelectedDate = CompanyFundReceiptInfoModel.SettleEndDate;
                decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(CompanyFundReceiptInfoModel.CompanyID, CompanyFundReceiptInfoModel.FilialeId, CompanyFundReceiptInfoModel.SettleEndDate);
                RTB_SettleBalance.Text = (-totalNumber).ToString(CultureInfo.InvariantCulture);
                if (CompanyFundReceiptInfoModel.ReceiptType == (int)CompanyFundReceiptType.Receive)
                {
                    RTB_SettleBalance.Text = totalNumber.ToString(CultureInfo.InvariantCulture);
                }
                RTB_ExpectBalance.Text = CompanyFundReceiptInfoModel.ExpectBalance.ToString(CultureInfo.InvariantCulture);
                RTB_RealityBalance.Text = CompanyFundReceiptInfoModel.RealityBalance.ToString(CultureInfo.InvariantCulture);
                RTB_DiscountMoney.Text = CompanyFundReceiptInfoModel.DiscountMoney.ToString(CultureInfo.InvariantCulture);
                RTB_DiscountCaption.Text = CompanyFundReceiptInfoModel.DiscountCaption;
                RTB_OtherDiscountCaption.Text = CompanyFundReceiptInfoModel.OtherDiscountCaption;

                if ((CompanyFundReceiptType)CompanyFundReceiptInfoModel.ReceiptType == CompanyFundReceiptType.Payment)
                {
                    PL_ReceiveGoods.Visible = true;
                    if (CompanyFundReceiptInfoModel.PurchaseOrderNo.Trim() == String.Empty)
                    {
                        span_PurchaseOrderNo.Style.Add("display", "none");
                        noReceivedGoods.Checked = false;
                        receivedGoods.Checked = true;
                    }
                    else
                    {
                        span_PurchaseOrderNo.Style.Add("display", "");
                        noReceivedGoods.Checked = true;
                        receivedGoods.Checked = false;
                    }
                }
                else
                {
                    PL_ReceiveGoods.Visible = false;

                }
                HF_ReceiptType.Value = string.Format("{0}", CompanyFundReceiptInfoModel.ReceiptType);

                if (CompanyFundReceiptInfoModel.ReceiptType == (int)CompanyFundReceiptType.Receive)
                {
                    Literal_DiscountMoney.Text = "收款折扣";
                    Literal_RealityBalance.Text = "应收金额";
                }
            }
        }
        #endregion

        #region -- 绑定往来单位列表
        /// <summary>
        /// 绑定往来单位列表
        /// </summary>
        protected void RCB_CompanyList_Bind()
        {
            RCB_CompanyList.DataSource = GetCompanyList();
            RCB_CompanyList.DataTextField = "CompanyName";
            RCB_CompanyList.DataValueField = "CompanyId";
            RCB_CompanyList.DataBind();
        }

        protected void BindSaleFiliale(Guid saleFilialeId)
        {
            var saleFiliales = CacheCollection.Filiale.GetSaleFilialeList();
            RcbSaleFiliale.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            foreach (var filialeInfo in saleFiliales)
            {
                if (filialeInfo != null)
                {
                    RcbSaleFiliale.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                }
            }
            RcbSaleFiliale.SelectedValue = string.Format("{0}", saleFilialeId);
        }

        protected void BindShopList(Guid companyId)
        {
            var shopList = FilialeManager.GetEntityShop().Where(ent => ent.ShopJoinType == (int)ShopJoinType.Join);
            var salePlatformList = CacheCollection.SalePlatform.GetList();
            //RcbShopList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            //门店建立对应的往来单位、取关联的往来单位
            var dics = _companyCussent.GetCompanyIdNameListByRelevanceFilialeIds(shopList.Select(ent => ent.ID).Union(salePlatformList.Select(ent => ent.ID)));
            RcbShopList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            foreach (var filialeInfo in dics)
            {
                if (filialeInfo != null)
                {
                    RcbShopList.Items.Add(new RadComboBoxItem(filialeInfo.CompanyName, filialeInfo.CompanyId.ToString()));
                }
            }
            RcbShopList.SelectedValue = string.Format("{0}", companyId);
        }
        #endregion

        #region -- 获取公司数据信息，包含供应商和物流公司
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected List<CompanyCussentInfo> GetCompanyList()
        {
            var data = (List<CompanyCussentInfo>)_companyCussent.GetCompanyCussentList(CompanyType.Suppliers);
            data.AddRange(_companyCussent.GetCompanyCussentList(CompanyType.Express));
            data.AddRange(_companyCussent.GetCompanyCussentList(CompanyType.Vendors));
            return data;
        }
        #endregion

        #region -- 提交(收)付款单据的修改
        /// <summary>
        /// 提交(收)付款单据的修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LB_Save_OncLick(object sender, EventArgs e)
        {
            var receipt = CompanyFundReceiptInfoModel; 
            if (!string.IsNullOrEmpty(RcbReceiveType.SelectedValue) && RcbReceiveType.SelectedValue=="1")
            {
                if (string.IsNullOrEmpty(RtbAmount.Text.Trim()) || RTB_RealityBalance.Text.Equals("0.00"))
                {
                    RAM.Alert("应收金额不能为空");
                    return;
                }
                Regex regex = new Regex(@"^\d*(\.\d*){0,1}$");
                if (!regex.IsMatch(RtbAmount.Text.Trim()))
                {
                    RAM.Alert("应收金额必须为数字");
                    return;
                }
                receipt.RealityBalance = Convert.ToDecimal(RtbAmount.Text.Trim());
            }
            else
            {
                if (noReceivedGoods.Checked)
                {
                    if (RTB_PurchaseOrderNo.Text.Trim() == string.Empty)
                    {
                        RAM.Alert("在未到货情况下，请填写采购单号！");
                        return;
                    }
                }
                var message = ValidateData();
                if (message.Length > 0)
                {
                    RAM.Alert(message);
                    return;
                }
                if (RTB_PurchaseOrderNo != null)
                {
                    receipt.PurchaseOrderNo = RTB_PurchaseOrderNo.Text.Trim();
                }
                if (RDP_StartDate.SelectedDate >= RDP_EndDate.SelectedDate)
                {
                    RAM.Alert("收款单截至日期必须大于开始日期！");
                    return;
                }
                receipt.CompanyID = new Guid(RCB_CompanyList.SelectedValue);
                receipt.FilialeId = new Guid(RCB_FilialeList.SelectedValue);
                if (RDP_StartDate.SelectedDate != null)
                {
                    receipt.SettleStartDate = (DateTime)RDP_StartDate.SelectedDate;
                    if (RDP_EndDate.SelectedDate != null) receipt.SettleEndDate = (DateTime)RDP_EndDate.SelectedDate;
                }
                receipt.ExpectBalance = Convert.ToDecimal(RTB_ExpectBalance.Text.Trim());
                receipt.RealityBalance = Convert.ToDecimal(RTB_RealityBalance.Text.Trim());
                receipt.DiscountMoney = RTB_DiscountMoney.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                receipt.DiscountCaption = RTB_DiscountCaption.Text.Trim();
                receipt.OtherDiscountCaption = RTB_OtherDiscountCaption.Text.Trim();
                receipt.ReceiptStatus = CompanyFundReceiptInfoModel.ReceiptStatus;
            }
            var filialeInfo = CacheCollection.Filiale.Get(receipt.FilialeId);
            if (filialeInfo != null && filialeInfo.ID != Guid.Empty)
            {
                receipt.IsOut = true;
                receipt.HasInvoice = true;
                receipt.ReceiptStatus = Convert.ToInt32(CompanyFundReceiptState.WaitInvoice);
            }
            else
            {
                receipt.IsOut = false;
                receipt.HasInvoice = false;
                receipt.ReceiptStatus = Convert.ToInt32(CompanyFundReceiptState.Audited);
            }

            ICompanyFundReceipt companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
            var isSave = companyFundReceipt.Update(receipt);
            if (isSave)
            {
                //往来收付款修改操作记录添加
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receipt.ReceiptID, receipt.ReceiptNo,
                    OperationPoint.CurrentReceivedPayment.Edit.GetBusinessInfo(), string.Empty);
                ClientScript.RegisterStartupScript(GetType(), "js", "<script>CloseAndRebind();</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "js", "<script>alert('保存收付款单失败！');</script>");
            }
        }

        private string ValidateData()
        {
            if (string.IsNullOrEmpty(RCB_CompanyList.SelectedValue) ||
                RCB_CompanyList.SelectedValue == string.Format("{0}", Guid.Empty))
                return "请选择往来单位！";

            if (string.IsNullOrEmpty(RCB_FilialeList.SelectedValue) ||
                RCB_FilialeList.SelectedValue == string.Format("{0}", Guid.Empty))
                return "请选择收款公司！";

            if (RDP_StartDate.SelectedDate == null)
                return "请选择开始时间！";

            if (RDP_EndDate.SelectedDate == null)
                return "请选择截止日期！";

            Regex regex = new Regex(@"^\d*(\.\d*){0,1}$");
            if (string.IsNullOrEmpty(RTB_RealityBalance.Text.Trim()) || RTB_RealityBalance.Text.Equals("0.00"))
                return "应收金额不能为空！";

            if (!regex.IsMatch(RTB_RealityBalance.Text.Trim()))
                return "应收金额必须为数字！";

            if (!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && !regex.IsMatch(RTB_DiscountMoney.Text.Trim()))
                return "收款折扣必须为数字！";

            if (!string.IsNullOrEmpty(RTB_SettleBalance.Text.Trim()) && !string.IsNullOrEmpty(RTB_RealityBalance.Text.Trim()))
            {
                if (decimal.Parse(RTB_SettleBalance.Text) != decimal.Parse(RTB_RealityBalance.Text) && string.IsNullOrEmpty(RTB_OtherDiscountCaption.Text))
                {
                    return "差额说明不能为空！";
                }
            }
            return "";

        }
        #endregion

        #region -- 选择结算结束日期后，获取后台余额统计
        protected void RDP_EndDate_OnSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            if (RCB_CompanyList.SelectedValue.Trim() != "")
            {
                var companyId = new Guid(RCB_CompanyList.SelectedValue);
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                if (RDP_EndDate.SelectedDate != null)
                {
                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(companyId, filialeId, (DateTime)RDP_EndDate.SelectedDate);
                    if (HF_ReceiptType.Value == string.Format("{0}", CompanyFundReceiptType.Receive))
                    {
                        RTB_SettleBalance.Text = totalNumber.ToString(CultureInfo.InvariantCulture);
                    }
                    if (HF_ReceiptType.Value == string.Format("{0}", CompanyFundReceiptType.Payment))
                    {
                        RTB_SettleBalance.Text = (-totalNumber).ToString(CultureInfo.InvariantCulture);
                    }
                }
                if (RDP_EndDate.SelectedDate != null && RDP_EndDate.SelectedDate <= RDP_StartDate.SelectedDate)
                {
                    RAM.Alert("截至日期必须大于开始日期！");
                }
            }
            else
            {
                RDP_EndDate.SelectedDate = null;
                ClientScript.RegisterStartupScript(GetType(), "js", "<script>alert('请先选择公司单位！');</script>");
            }
        }
        #endregion

        #region -- 应付（收）款项文本变动时，获取货币大写格式
        protected void RTB_RealityBalance_TextChanged(object sender, EventArgs e)
        {
            LB_UpperCaseMoney.Text = WebUtility.ConvertSum(RTB_RealityBalance.Text.Trim());
        }
        #endregion

        #region -- 往来单位列表选择变动时，初始化部分控件的值
        protected void RCB_CompanyList_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (e.Value.Trim() != String.Empty)
            {
                RDP_EndDate.Clear();
                RTB_SettleBalance.Text = "0.00";
                RTB_ExpectBalance.Text = "0.00";
                RTB_RealityBalance.Text = "0.00";
                LB_UpperCaseMoney.Text = "";
                RTB_DiscountMoney.Text = "0.00";
                RTB_DiscountCaption.Text = "";
                RTB_OtherDiscountCaption.Text = "";
            }

            if (noReceivedGoods.Checked)
            {
                span_PurchaseOrderNo.Style.Add("display", "");
            }
        }
        #endregion

        protected void ShowFiliale()
        {
            IList<FilialeInfo> filialeList = CacheCollection.Filiale.GetHeadList();
            RCB_FilialeList.DataSource = filialeList;
            RCB_FilialeList.DataTextField = "Name";
            RCB_FilialeList.DataValueField = "ID";
            RCB_FilialeList.DataBind();
            RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
        }
    }
}
