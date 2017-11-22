using System;
using System.Web.UI;
using ERP.BLL.Implement.Organization;
using ERP.SAL;
using ERP.UI.Web.Common;
using Framework.Common;

namespace ERP.UI.Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var loginResult = AccountManager.Login("170302", Framework.Common.MD5.Encrypt("Wang123456"), new PersonnelSao());
            //var loginResult = AccountManager.Login("070102", MD5.Encrypt("123456"), new PersonnelSao());
            var loginResult = AccountManager.Login("150367", MD5.Encrypt("123456"), new PersonnelSao());
            if (loginResult.IsSuccess) 
            {
                //Response.Redirect("CostInvoiceMoney.aspx?Token=" + loginResult.Token);
                CurrentSession.Personnel.Set(loginResult.PersonnelInfo);
                Response.Redirect("Main_New.aspx");     
            }
            else
            {
                Response.Write(loginResult.FailMessage);  
            }
        }
    }
}