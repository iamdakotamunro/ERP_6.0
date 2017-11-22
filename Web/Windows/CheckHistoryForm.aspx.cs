using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>对账记录生成收款单据填写付款账号  陈重文  2015-04-09
    /// </summary>
    public partial class CheckHistoryForm : WindowsPage
    {
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccounts _bankAccounts=new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccountDao _bankAccountDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["ReceiptNo"]))
            {
                if (!IsPostBack)
                {
                    var receiptNosStr = Request.QueryString["ReceiptNo"];
                    string[] receiptNos = receiptNosStr.Split(',');
                    IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
                    foreach (var receiptNo in receiptNos)
                    {
                        if (!string.IsNullOrWhiteSpace(receiptNo))
                        {
                            var info = _companyFundReceipt.GetFundReceiptInfoByReceiptNo(receiptNo);
                            var cInfo = _companyCussent.GetCompanyCussent(info.CompanyID);
                            if (cInfo != null)
                            {
                                info.CompanyName = cInfo.CompanyName;
                            }
                            list.Add(info);
                        }
                    }
                    RepReceiptNoList.DataSource = list;
                    RepReceiptNoList.DataBind();
                }
            }
        }

        /// <summary>绑定单据公司和银行账号
        /// </summary>
        protected void RepReceiptNo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //公司
                var rcbFilialeList = (RadComboBox)e.Item.FindControl("RCB_FilialeList");
                //银行帐号
                var rcbBankAccounts = (RadComboBox)e.Item.FindControl("RCB_BankAccounts");
                //公司ID
                var filialeId = new Guid(((HiddenField)e.Item.FindControl("hfFilialeId")).Value);
                IList<BankAccountInfo> list = new BindingList<BankAccountInfo>();
                if (filialeId == _reckoningElseFilialeid)
                {
                    rcbFilialeList.Items.Insert(0, new RadComboBoxItem("ERP", _reckoningElseFilialeid.ToString()));
                    list = _bankAccounts.GetList().Where(ent => ent.IsMain == false).ToList();
                }
                else
                {
                    var filialeInfo = CacheCollection.Filiale.Get(filialeId);
                    if (filialeInfo != null)
                    {
                        rcbFilialeList.Items.Insert(0, new RadComboBoxItem(filialeInfo.Name, filialeInfo.ID.ToString()));
                        list = _bankAccountDao.GetListByTargetId(filialeId).Where(ent => ent.IsMain).ToList();
                    }
                }
                rcbFilialeList.SelectedIndex = 0;
                var bankBalanceList = _bankAccounts.GetBalanceList();
                foreach (var info in list)
                {
                    if (info.IsUse)
                    {
                        var bankBalanceInfo =
                            bankBalanceList.FirstOrDefault(act => act.BankAccountId == info.BankAccountsId);
                        rcbBankAccounts.Items.Add(new RadComboBoxItem(string.Format("{0}-{1}[{2}]", info.BankName, info.AccountsName, 
                            bankBalanceInfo != null ? WebControl.NumberSeparator(bankBalanceInfo.NonceBalance) : "0"), info.BankAccountsId.ToString()));
                    }  
                }
                rcbBankAccounts.Items.Insert(0, new RadComboBoxItem("请选择银行", Guid.Empty.ToString()));
            }
        }

        /// <summary>保存
        /// </summary>
        protected void BtnSave_Click(object sender, ImageClickEventArgs e)
        {
            var dic = new Dictionary<Guid, Guid>();
            foreach (RepeaterItem item in RepReceiptNoList.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var receiptId = (HiddenField)item.FindControl("hfReceiptID");
                    //银行帐号
                    var rcbBankAccounts = ((RadComboBox)item.FindControl("RCB_BankAccounts"));
                    if (rcbBankAccounts.SelectedValue == Guid.Empty.ToString())
                    {
                        RAM.Alert("系统提示：有单据银行账号未选择！");
                        return;
                    }
                    dic.Add(new Guid(receiptId.Value), new Guid(rcbBankAccounts.SelectedValue));
                }
            }
            foreach (var item in dic)
            {
                _companyFundReceipt.SetPayBankAccountsId(item.Key, item.Value);
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}
