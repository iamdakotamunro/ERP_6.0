using System;
using System.Web.UI;
using ERP.Model;
using ERP.UI.Web.Common;

namespace ERP.UI.Web
{
    public partial class MainMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowCopyright();

            PersonnelInfo personnelInfo = CurrentSession.Personnel.Get();

            if (personnelInfo == null)
            {
                Response.Redirect("~/Error.aspx?Errmsg=" + "登陆已无效,请重新登陆!");
                Page.Response.End();
            }

            if (personnelInfo == null)
            {
                Response.Redirect("~/Error.aspx?Errmsg=" + "登陆已无效,请重新登陆!");
                Page.Response.End();
            }
        }

        

        private void ShowCopyright()
        {
            CopyrightInfo.Text = "Copyright 2007-" + DateTime.Now.Year + " All Rights Reserved";// " <a href=\"http://" + WebControl.WebRudder.WebUrl + "\" target=\"_blank\">" + WebControl.WebRudder.WebName 
        }
    }
}
