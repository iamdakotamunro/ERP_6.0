using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
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

/*
 * 最后修改人：刘彩军
 * 修改时间：2011-August-16th
 * 修改内容：添加单据号字段
 */

namespace ERP.UI.Web.Windows
{
    public partial class CompanyFundReceiveReceiptAdd : WindowsPage
    {
        protected CodeManager CodeBll = new CodeManager();
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        SubmitController _submitController;
        private readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Read);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccountDao=new BankAccountDao(GlobalConfig.DB.FromType.Read);
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        #region -- Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                div_Service.Visible = false;
                div_Normal.Visible = true;
                BindReceiptType();
                BindShopList();
                var filialeIds = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                if (filialeIds.Any())
                {
                    ShowFiliale(filialeIds);

                    BindSaleFiliale(filialeIds.Where(ent=>ent.FilialeTypes.Contains((int)FilialeType.SaleCompany))); 
                }
                else
                {
                    RCB_FilialeList.Items.Clear();
                    RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                    RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                    RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
                }
            }
        }
        #endregion

        protected PersonnelInfo PersonnelInfoModel
        {
            get
            {
                if (ViewState["PersonnelInfoModel"] == null)
                {
                    ViewState["PersonnelInfoModel"] = CurrentSession.Personnel.Get() ?? new PersonnelInfo(null);
                }
                return (PersonnelInfo)ViewState["PersonnelInfoModel"];
            }
        }

        #region -- 往来单位搜索请求事件
        protected void RcbCompanyListOnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var dataList = GetCompanyList();
            foreach (var entity in dataList)
            {
                var item = new RadComboBoxItem { Text = entity.CompanyName, Value = entity.CompanyId.ToString() };
                RCB_CompanyList.Items.Add(item);
                item.DataBind();
            }
        }
        #endregion

        #region -- 获取公司数据信息，包含供应商和物流公司
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected List<CompanyCussentInfo> GetCompanyList()
        {
            int[] companyTypes = { (int)CompanyType.Suppliers, (int)CompanyType.Express, (int)CompanyType.Vendors };
            return _companyCussent.GetCompanyCussentList(State.Enable).Where(ent=>companyTypes.Contains(ent.CompanyType) || ent.RelevanceFilialeId!=Guid.Empty).ToList();
        }
        #endregion

        #region -- 提交插入一个收款单据

        /// <summary>提交插入一个收款单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbInsterOncLick(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                RAM.Alert("程序正在处理中，请稍候...");
                return;
            }
            CompanyFundReceiptInfo receipt;
            if (!string.IsNullOrEmpty(RcbReceiveType.SelectedValue) && RcbReceiveType.SelectedValue=="1")
            {
                if (string.IsNullOrEmpty(RcbShopList.SelectedValue) ||
                    RcbShopList.SelectedValue == string.Format("{0}", Guid.Empty))
                {
                    RAM.Alert("请选择往来单位！");
                    return;
                }
                if (string.IsNullOrEmpty(RcbSaleFiliale.SelectedValue) ||
                    RcbSaleFiliale.SelectedValue == string.Format("{0}", Guid.Empty))
                {
                    RAM.Alert("请选择收款单位！");
                    return;
                }
                if (string.IsNullOrEmpty(RtbAmount.Text.Trim()) || RTB_RealityBalance.Text.Equals("0.00"))
                {
                    RAM.Alert("应收金额不能为空");
                    return;
                }
                Regex regex=new Regex(@"^\d*(\.\d*){0,1}$");
                if (!regex.IsMatch(RtbAmount.Text.Trim()))
                {
                    RAM.Alert("应收金额必须为数字");
                    return;
                }
                receipt = new CompanyFundReceiptInfo
                {
                    ReceiptID = Guid.NewGuid(),
                    ReceiptNo = CodeBll.GetCode(CodeType.GT),
                    ReceiptType = Convert.ToInt32(CompanyFundReceiptType.Receive),
                    ApplyDateTime = DateTime.Now,
                    ApplicantID = PersonnelInfoModel.PersonnelId,
                    PurchaseOrderNo = string.Empty,
                    FilialeId = new Guid(RcbSaleFiliale.SelectedValue),
                    CompanyID = new Guid(RcbShopList.SelectedValue),
                    RealityBalance = Convert.ToDecimal(RtbAmount.Text.Trim()),
                    SettleStartDate = new DateTime(1900,1,1),
                    SettleEndDate = new DateTime(1900, 1, 1)
                };
            }
            else
            {
                var message = ValidateData();
                if (message.Length > 0)
                {
                    RAM.Alert(message);
                    return;
                }
                
                receipt = new CompanyFundReceiptInfo
                {
                    ReceiptID = Guid.NewGuid(),
                    ReceiptNo = CodeBll.GetCode(CodeType.GT),
                    ReceiptType = Convert.ToInt32(CompanyFundReceiptType.Receive),
                    ApplyDateTime = DateTime.Now,
                    ApplicantID = PersonnelInfoModel.PersonnelId,
                    PurchaseOrderNo = string.Empty
                };
                if (RDP_StartDate.SelectedDate != null)
                    receipt.SettleStartDate = (DateTime)RDP_StartDate.SelectedDate;
                if (RDP_EndDate.SelectedDate != null)
                    receipt.SettleEndDate = (DateTime)RDP_EndDate.SelectedDate;

                if (receipt.SettleEndDate < receipt.SettleStartDate)
                {
                    RAM.Alert("系统提示：截止日期必须大于开始日期！");
                    return;
                }
                receipt.ExpectBalance = Convert.ToDecimal(RTB_ExpectBalance.Text.Trim());
                receipt.RealityBalance = Convert.ToDecimal(RTB_RealityBalance.Text.Trim());
                receipt.DiscountMoney = RTB_DiscountMoney.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                receipt.DiscountCaption = RTB_DiscountCaption.Text.Trim();
                receipt.OtherDiscountCaption = RTB_OtherDiscountCaption.Text.Trim();
                receipt.CompanyID = new Guid(RCB_CompanyList.SelectedValue);
                receipt.FilialeId = new Guid(RCB_FilialeList.SelectedValue);
                receipt.RealityBalance = Convert.ToDecimal(RTB_RealityBalance.Text.Trim());
                receipt.DiscountMoney = RTB_DiscountMoney.Text.Trim() == string.Empty ? 0 : Convert.ToDecimal(RTB_DiscountMoney.Text.Trim());
                receipt.DiscountCaption = RTB_DiscountCaption.Text.Trim();
                receipt.OtherDiscountCaption = RTB_OtherDiscountCaption.Text.Trim();
                receipt.PayBankAccountsId = Guid.Empty;
                receipt.StockOrderNos = string.Empty;
            }
            
            //note 按公司收款则开具发票  2015-03-20  陈重文
            if (receipt.FilialeId == _reckoningElseFilialeid)
            {
                receipt.IsOut = false;
                receipt.HasInvoice = false;
                receipt.ReceiptStatus = Convert.ToInt32(CompanyFundReceiptState.Audited);
            }
            else
            {
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
            }
            bool isInsert = _companyFundReceipt.Insert(receipt);
            var info = _companyFundReceipt.GetFundReceiptInfoByReceiptNo(receipt.ReceiptNo);
            if (isInsert && info.ReceiptID != Guid.Empty)
            {
                //往来收付款添加收款单增加操作记录添加
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.ReceiptID, info.ReceiptNo,
                    OperationPoint.CurrentReceivedPayment.FillBill.GetBusinessInfo(), string.Empty);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                RAM.Alert("添加付款单失败！");
            }
            _submitController.Submit();
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

            if (string.IsNullOrEmpty(RTB_ExpectBalance.Text))
                return "对方余额为空！";

            Regex regex = new Regex(@"^\d*(\.\d*){0,1}$");
            if (!regex.IsMatch(RTB_ExpectBalance.Text.Trim()))
                return "对方余额必须为数字！";

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

        #region -- 结算结束日期被改变后，获取后台余额统计
        protected void RdpEndDateSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            if (RCB_CompanyList.SelectedValue.Trim() != "")
            {
                var companyId = new Guid(RCB_CompanyList.SelectedValue);
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                if (RDP_EndDate.SelectedDate != null && RDP_EndDate.SelectedDate > RDP_StartDate.SelectedDate)
                {
                    //decimal totalNumber = Reckoning.GetReckoningNonceTotalByReceiptType(companyId, (DateTime)RDP_EndDate.SelectedDate);
                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(companyId, filialeId, (DateTime)RDP_EndDate.SelectedDate);
                    RTB_SettleBalance.Text = totalNumber != 0 ? (totalNumber).ToString("#.00") : "0.00";
                }
                else
                {
                    RAM.Alert("系统提示：截止日期必须大于开始日期！");
                }
            }
            else
            {
                RDP_EndDate.SelectedDate = null;
                ClientScript.RegisterStartupScript(GetType(), "js", "<script>alert('请先选择公司单位！');</script>");
            }
        }
        #endregion

        #region -- 应收金额改变后，获取货币大写格式
        protected void RtbRealityBalanceTextChanged(object sender, EventArgs e)
        {
            LB_UpperCaseMoney.Text = WebUtility.ConvertSum(RTB_RealityBalance.Text.Trim());
        }
        #endregion

        #region -- 往来单位列表选择变动后，初始化相应的控件值
        protected void RcbCompanyListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (e.Value.Trim() != String.Empty)
            {
                if (RDP_EndDate.SelectedDate != null) { RDP_EndDate.Clear(); }
                RTB_SettleBalance.Text = "0.00";
                RTB_ExpectBalance.Text = "0.00";
                RTB_RealityBalance.Text = "0.00";
                LB_UpperCaseMoney.Text = "";
                RDP_EndDate.Enabled = true;
            }
        }
        #endregion

        #region[判断折扣说明是否显示]
        protected void RTB_DiscountMoney_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(RTB_DiscountMoney.Text) && RTB_DiscountMoney.Text != "0")
                DIV_DiscountCaption.Visible = true;
            else
                DIV_DiscountCaption.Visible = false;
        }
        #endregion

        #region[显示对应付款公司]

        protected void ShowFiliale(IEnumerable<FilialeInfo> filiales)
        {
            foreach (var filialeInfo in filiales)
            {
                if (filialeInfo!=null)
                {
                    RCB_FilialeList.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                }
            }
            RCB_FilialeList.Items.Add(new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
            RCB_FilialeList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
        }

        protected void BindSaleFiliale(IEnumerable<FilialeInfo> saleFiliales)
        {
            RcbSaleFiliale.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            foreach (var filialeInfo in saleFiliales)
            {
                if (filialeInfo != null)
                {
                    RcbSaleFiliale.Items.Add(new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                }
            }
            RcbSaleFiliale.SelectedValue = Guid.Empty.ToString();
        }

        protected void BindShopList()
        {
            var shopList = FilialeManager.GetEntityShop().Where(ent=>ent.ShopJoinType==(int)ShopJoinType.Join);
            var salePlatformList = CacheCollection.SalePlatform.GetList();
            //RcbShopList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            //门店建立对应的往来单位、取关联的往来单位
            var dics = _companyCussent.GetCompanyIdNameListByRelevanceFilialeIds(shopList.Select(ent => ent.ID).Union(salePlatformList.Select(ent=>ent.ID)));
            RcbShopList.Items.Add(new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
            foreach (var filialeInfo in dics)
            {
                if (filialeInfo != null)
                {
                    RcbShopList.Items.Add(new RadComboBoxItem(filialeInfo.CompanyName, filialeInfo.CompanyId.ToString()));
                }
            }
            RcbShopList.SelectedValue = Guid.Empty.ToString();
        }
        #endregion

        /// <summary>公司下拉加载公司银行资金帐号
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RCB_FilialeList_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var value = e.Value.Trim();
            Guid filialeId;
            Guid.TryParse(value, out filialeId);
            if (filialeId == Guid.Empty) return;
            //RCB_BankAccount.Items.Clear();
            //RCB_BankAccount.Text = string.Empty;
            //其他公司显示已绑定过公司银行账号的非主账号
            if (filialeId != _reckoningElseFilialeid)
            {
                var filialeInfo = CacheCollection.Filiale.Get(filialeId);
                if (filialeInfo != null && filialeInfo.ID != Guid.Empty)
                {
                    var bankList = _bankAccountDao.GetListByTargetId(filialeId).Where(ent => ent.IsMain).ToList();
                    if (bankList.Count == 0)
                    {
                        const string MSG = "不要问我是谁，大家都叫我雷锋";
                        RAM.Alert(string.Format("{0}没有设置主账号\r\n若需要请在【公司资金账户】页面设置公司主账号\r\n {1}", filialeInfo.Name, MSG));
                        RCB_FilialeList.SelectedValue = Guid.Empty.ToString();
                        RCB_FilialeList.Text = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrEmpty(RCB_CompanyList.SelectedValue) && RCB_CompanyList.SelectedValue!=Guid.Empty.ToString())
            {
                IList<CompanyFundReceiptInfo> companyFundReceiptInfos =_companyFundReceipt.GetFundListByCompanyId(new Guid(RCB_CompanyList.SelectedValue));
                companyFundReceiptInfos = filialeId != _reckoningElseFilialeid ? companyFundReceiptInfos.Where(act => act.FilialeId == filialeId && act.IsOut).ToList() 
                    : companyFundReceiptInfos.Where(act => act.IsOut==false).ToList();
                companyFundReceiptInfos=companyFundReceiptInfos.OrderByDescending(c => c.SettleEndDate).Take(1).ToList();
                if (companyFundReceiptInfos.Count > 0)
                {
                    RDP_StartDate.SelectedDate =
                        companyFundReceiptInfos[0].SettleEndDate.AddDays(1);
                    RDP_StartDate.Enabled = false;
                }
                else
                {
                    RDP_StartDate.SelectedDate = null;
                    RDP_StartDate.Enabled = true;
                }
            }
        }

        protected void RcbShopListListOnItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var dataList = FilialeManager.GetEntityShop();
            if (!string.IsNullOrEmpty(e.Text))
            {
                dataList = dataList.Where(ent => ent.Name.Contains(e.Text) || ent.RealName.Contains(e.Text)).ToList();
            }
            foreach (var entity in dataList)
            {
                var item = new RadComboBoxItem { Text = entity.Name, Value = entity.ID.ToString() };
                RcbShopList.Items.Add(item);
                item.DataBind();
            }
        }

        private void BindReceiptType()
        {
            RcbReceiveType.Items.Add(new RadComboBoxItem("往来单位", "0"));
            RcbReceiveType.Items.Add(new RadComboBoxItem("劳务", "1"));
            RcbReceiveType.SelectedValue = "0";
        }

        protected void RcbReceiveTypeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox) o;
            if (cb!=null)
            {
                div_Normal.Visible = cb.SelectedValue == "0";
                div_Service.Visible = cb.SelectedValue != "0";

                RcbShopList.Text = "";
                RcbSaleFiliale.Text = "";
                RcbShopList.SelectedValue = string.Format("{0}", Guid.Empty);
                RcbSaleFiliale.SelectedValue = string.Format("{0}", Guid.Empty);
                RadBalance.Text = "0.00";
            }
        }

        protected void RcbSaleFilialeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox)o;
            
            if (cb != null)
            {
                var saleFilialeId = !string.IsNullOrEmpty(cb.SelectedValue) ? new Guid(cb.SelectedValue) : Guid.Empty;
                var  companyId = !string.IsNullOrEmpty(RcbShopList.SelectedValue) ? new Guid(RcbShopList.SelectedValue) : Guid.Empty;
                if (saleFilialeId!=Guid.Empty && companyId!=Guid.Empty)
                {
                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(companyId, saleFilialeId, DateTime.Now);
                    RadBalance.Text = totalNumber != 0 ? (totalNumber).ToString("#.00") : "0.00";
                }
                else
                {
                    RadBalance.Text = "0.00";
                }
            }
            else
            {
                RadBalance.Text = "0.00";
            }
        }

        protected void RcbShopListOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox)o;

            if (cb != null)
            {
                var  companyId = !string.IsNullOrEmpty(cb.SelectedValue) ? new Guid(cb.SelectedValue) : Guid.Empty;
                var saleFilialeId = !string.IsNullOrEmpty(RcbSaleFiliale.SelectedValue) ? new Guid(RcbSaleFiliale.SelectedValue) : Guid.Empty;
                if (saleFilialeId != Guid.Empty && companyId != Guid.Empty)
                {
                    decimal totalNumber = _reckoning.GetReckoningNonceTotalledByFilialeId(companyId, saleFilialeId, DateTime.Now);
                    RadBalance.Text = totalNumber != 0 ? (totalNumber).ToString("#.00") : "0.00";
                }
                else
                {
                    RadBalance.Text = "0.00";
                }
            }
            else
            {
                RadBalance.Text = "0.00";
            }
        }
    }
}
