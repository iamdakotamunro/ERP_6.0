using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>費用分類（新增和編輯）  ADD 2015-01-20  陳重文
    /// </summary>
    public partial class CostCompanyForm : WindowsPage
    {
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        private readonly ICostCussent _costCussentDao = new CostCussent(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CostCompanyId != Guid.Empty)
                {
                    Page.Title = "编辑费用类型";
                    BindValue();
                }
                else
                {
                    Page.Title = "添加费用类型";
                }
                IList<CostCompanyClassInfo> companyClassList = WebControl.RecursionCostClass(Guid.Empty, 0);
                companyClassList.Insert(0, new CostCompanyClassInfo());
                RCB_CompanyClassId.DataSource = companyClassList;
                RCB_CompanyClassId.DataTextField = "CompanyClassName";
                RCB_CompanyClassId.DataValueField = "CompanyClassId";
                RCB_CompanyClassId.DataBind();
            }
        }

        /// <summary>费用分类ID
        /// </summary>
        public Guid CostCompanyId
        {
            get
            {
                var costCompanyId = Request.QueryString["CompanyId"];
                return string.IsNullOrWhiteSpace(costCompanyId) ? Guid.Empty : new Guid(costCompanyId);
            }
        }

        /// <summary>编辑费用分类绑定数据
        /// </summary>
        private void BindValue()
        {
            CostCussentInfo costInfo = _costCussentDao.GetCompanyCussent(CostCompanyId);
            RTB_CompanyName.Text = costInfo.CompanyName;
            RCB_CompanyClassId.SelectedValue = costInfo.CompanyClassId.ToString();
            RTB_Linkman.Text = costInfo.Linkman;
            RTB_Mobile.Text = costInfo.Mobile;
            RTB_Address.Text = costInfo.Address;
            RTB_PostalCode.Text = costInfo.PostalCode;
            RTB_Phone.Text = costInfo.Phone;
            RTB_Fax.Text = costInfo.Fax;
            RTB_WebSite.Text = costInfo.WebSite;
            RTB_Email.Text = costInfo.Email;
            RTB_BankAccounts.Text = costInfo.BankAccounts;
            RTB_AccountNumber.Text = costInfo.AccountNumber;
            DDL_CompanyType.SelectedValue = string.Format("{0}", costInfo.CompanyType);
            RBL_State.SelectedValue = string.Format("{0}", costInfo.State);
            RTB_Description.Text = costInfo.Description;
            RTB_SubjectInfo.Text = costInfo.SubjectInfo;
        }

        /// <summary>保存费用分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, ImageClickEventArgs e)
        {
            var companyName = RTB_CompanyName.Text.Trim();

            if (string.IsNullOrEmpty(RCB_CompanyClassId.SelectedValue) ||
                RCB_CompanyClassId.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择所属分类！");
                return;
            }

            #region [新增]

            if (CostCompanyId == Guid.Empty)
            {
                if (_costCussentDao.IsBeing(companyName))
                {
                    RAM.Alert("系统提示：已经具有相同名称的费用分类！");
                    return;
                }
                var costCompanyId = Guid.NewGuid();
                var costInfo = new CostCussentInfo
                                               {
                                                   CompanyId = costCompanyId,
                                                   CompanyClassId = new Guid(RCB_CompanyClassId.SelectedValue),
                                                   CompanyName = companyName,
                                                   Linkman = RTB_Linkman.Text.Trim(),
                                                   Address = RTB_Address.Text.Trim(),
                                                   PostalCode = RTB_PostalCode.Text.Trim(),
                                                   Phone = RTB_Phone.Text.Trim(),
                                                   Mobile = RTB_Mobile.Text.Trim(),
                                                   Fax = RTB_Fax.Text.Trim(),
                                                   WebSite = RTB_WebSite.Text.Trim(),
                                                   Email = RTB_Email.Text.Trim(),
                                                   BankAccounts = RTB_BankAccounts.Text.Trim(),
                                                   AccountNumber = RTB_AccountNumber.Text.Trim(),
                                                   DateCreated = DateTime.Now,
                                                   CompanyType = Convert.ToInt32(DDL_CompanyType.SelectedValue),
                                                   State = Convert.ToInt32(RBL_State.SelectedValue),
                                                   Description = RTB_Description.Text.Trim(),
                                                   SubjectInfo = RTB_SubjectInfo.Text.Trim(),
                                               };
                try
                {
                    _costCussentDao.Insert(costInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    RAM.Alert("系统繁忙，请尝试稍后操作！");
                    SAL.LogCenter.LogService.LogError("添加费用分类信息异常", "财务管理", ex);
                }
            }

            #endregion

            #region [修改]

            else
            {
                var oldInfo = _costCussentDao.GetCompanyCussent(CostCompanyId);
                if (oldInfo.CompanyName != companyName)
                {
                    if (_costCussentDao.IsBeing(companyName))
                    {
                        RAM.Alert("系统提示：已经具有相同名称的费用分类！");
                        return;
                    }
                }
                var newCostInfo = new CostCussentInfo
                {
                    CompanyId = CostCompanyId,
                    CompanyClassId = new Guid(RCB_CompanyClassId.SelectedValue),
                    CompanyName = companyName,
                    Linkman = RTB_Linkman.Text.Trim(),
                    Address = RTB_Address.Text.Trim(),
                    PostalCode = RTB_PostalCode.Text.Trim(),
                    Phone = RTB_Phone.Text.Trim(),
                    Mobile = RTB_Mobile.Text.Trim(),
                    Fax = RTB_Fax.Text.Trim(),
                    WebSite = RTB_WebSite.Text.Trim(),
                    Email = RTB_Email.Text.Trim(),
                    BankAccounts = RTB_BankAccounts.Text.Trim(),
                    AccountNumber = RTB_AccountNumber.Text.Trim(),
                    DateCreated = DateTime.Now,
                    CompanyType = Convert.ToInt32(DDL_CompanyType.SelectedValue),
                    State = Convert.ToInt32(RBL_State.SelectedValue),
                    Description = RTB_Description.Text.Trim(),
                    SubjectInfo = RTB_SubjectInfo.Text.Trim(),
                };
                try
                {
                    _costCussentDao.Update(newCostInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    RAM.Alert("系统繁忙，请尝试稍后操作！");
                    SAL.LogCenter.LogService.LogError("更新费用分类信息异常", "财务管理", ex);
                }
            }

            #endregion
        }

        /// <summary>绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RepFilialeList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var filialeId = (HiddenField)e.Item.FindControl("hfFilialeId");
                //发票打款帐号（必须公司主账号）
                var rcbInvoiceAccounts = (RadComboBox)e.Item.FindControl("RCB_InvoiceAccounts");
                //凭证打款帐号（必须公司非主帐号）
                var rcbVoucherAccounts = (RadComboBox)e.Item.FindControl("RCB_VoucherAccounts");
                //现金打款账号（必须公司非主帐号）
                var rcbCashAccounts = (RadComboBox)e.Item.FindControl("RCB_CashAccounts");
                //无凭证打款账号（必须公司非主帐号）
                var rcbNoVoucherAccounts = (RadComboBox)e.Item.FindControl("RCB_NoVoucherAccounts");

                #region [获取公司帐号并绑定到指令控件]

                IList<BankAccountInfo> allBankList = _bankAccountDao.GetListByTargetId(new Guid(filialeId.Value)).Where(ent => ent.IsUse).ToList();
                IList<BankAccountInfo> mainBankList = allBankList.Where(ent => ent.IsMain).ToList();
                IList<BankAccountInfo> notIsMainList = _bankAccounts.GetBankAccountsListByNotIsMain().ToList();
                foreach (var info in mainBankList)
                {
                    rcbInvoiceAccounts.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                }
                foreach (var info in notIsMainList)
                {
                    rcbVoucherAccounts.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                    rcbCashAccounts.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                    rcbNoVoucherAccounts.Items.Add(new RadComboBoxItem(info.BankName + "-" + info.AccountsName, info.BankAccountsId.ToString()));
                }
                rcbInvoiceAccounts.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                rcbVoucherAccounts.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                rcbCashAccounts.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                rcbNoVoucherAccounts.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
                #endregion

                if (CostCompanyId != Guid.Empty)
                {
                    var info = _costCussentDao.GetCostCompanyBindingBankAccountsInfo(new Guid(filialeId.Value), CostCompanyId);
                    if (info == null)
                    {
                        return;
                    }
                    if (info.InvoiceAccountsId != Guid.Empty)
                    {
                        rcbInvoiceAccounts.SelectedValue = info.InvoiceAccountsId.ToString();
                    }
                    if (info.VoucherAccountsId != Guid.Empty)
                    {
                        rcbVoucherAccounts.SelectedValue = info.VoucherAccountsId.ToString();
                    }
                    if (info.CashAccountsId != Guid.Empty)
                    {
                        rcbCashAccounts.SelectedValue = info.CashAccountsId.ToString();
                    }
                    if (info.NoVoucherAccountsId != Guid.Empty)
                    {
                        rcbNoVoucherAccounts.SelectedValue = info.NoVoucherAccountsId.ToString();
                    }
                }
            }
        }
    }
}