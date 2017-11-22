using System;
using ERP.BLL.Implement.Inventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;

namespace LonmeShop.AdminWeb
{
    public partial class InvicePrint : WindowsPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["gid"] != null)
                    {

                        var gid = new Guid(Request.QueryString["gid"]);
                        var getMailInfo = new Invoice(GlobalConfig.DB.FromType.Read);
                        InvoiceInfo modelgetmailinfo = getMailInfo.GetInvoice(gid);
                        LabelAddress.Text = modelgetmailinfo.Address;
                        LabelName.Text = modelgetmailinfo.Receiver;
                        LabelZip.Text = modelgetmailinfo.PostalCode;
                    }
                }
                catch { }
            }
        }
    }
}
