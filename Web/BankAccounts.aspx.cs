using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>资金账户
    /// </summary>
    public partial class BankAccountsAw : BasePage
    {
        private readonly IBankAccounts _bankAccountsWrite = new BankAccounts(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BankAccountsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BankAccountsGrid.DataSource = _bankAccountsWrite.GetList();
        }

        protected void BankAccountsGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var dataItem = e.Item as GridDataItem;
            if (dataItem != null)
            {
                var bankAccountsId = new Guid(dataItem.GetDataKeyValue("BankAccountsId").ToString());
                try
                {
                    //判断“费用账户”是否已绑定 CostCompany
                    ICostCussent costCussentDao=new CostCussent(GlobalConfig.DB.FromType.Read);
                    IList<CostCussentInfo> costCussentList = costCussentDao.GetCompanyCussentList();
                    var list = costCussentList.Where(w => w.InvoiceAccountsId == bankAccountsId || w.VoucherAccountsId == bankAccountsId ||
                            w.CashAccountsId == bankAccountsId || w.NoVoucherAccountsId == bankAccountsId).ToList();
                    string message = list.Aggregate("", (current, info) => current + (info.CompanyName + "\n"));
                    if (list.Count > 0)
                    {
                        RAM.Alert("该资金账户已经绑定到费用账户：\n" + message + "\n不允许删除");
                    }
                    else
                    {
                        //判断“往来单位资料”是否已绑定 CompanyBankAccounts
                        IList<FilialeInfo> filialeList = GlobalConfig.IsTestWebSite ?                         
CacheCollection.Filiale.GetHeadList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && ent.ID.Equals(new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"))).ToList() :
CacheCollection.Filiale.GetHeadList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && !ent.ID.Equals(new Guid("ED58311F-FE6B-4CD9-85E9-FDA26EA209A0"))).ToList();
                        foreach (var filialeInfo in filialeList)
                        {
                            //删除B2C中的资金账户
                            B2CSao.BankAccountsDelete(filialeInfo.ID, bankAccountsId);
                        }
                        //删除本地资金账户
                        _bankAccountsWrite.DeleteBankPersion(bankAccountsId);
                        BankAccountManager.WriteInstance.Delete(bankAccountsId);
                    }
                }
                catch (Exception exp)
                {
                    RAM.Alert("资金账户删除失败！\\n\\n错误提示：" + exp.Message);
                }
            }
        }

        protected string GetPaymentTypeName(object paymentType)
        {
            if (Convert.ToInt32(paymentType) > Convert.ToInt32(PaymentType.SwipeCard))
                return "待设置";
            return EnumAttribute.GetKeyName((PaymentType)paymentType);
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(BankAccountsGrid, e);
        }

        protected void txt_OrderIndex_OnTextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null)
            {
                try
                {
                    var bankAccountsId = new Guid(dataItem.GetDataKeyValue("BankAccountsId").ToString());
                    var orderIndex = Convert.ToInt32(textBox.Text);
                    orderIndex = orderIndex > 0 ? orderIndex : 1;
                    _bankAccountsWrite.UpdateOrderIndex(bankAccountsId, orderIndex);
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
            }
            BankAccountsGrid.Rebind();
        }
    }
}