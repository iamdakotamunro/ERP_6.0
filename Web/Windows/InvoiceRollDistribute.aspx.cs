using System;
using System.Drawing;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class InvoiceRollDistribute : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HiddenField_RollId.Value = Request.QueryString["rollid"];
            }
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            var startNo = Request.QueryString["startno"];
            var endNo = Request.QueryString["endno"];
            var toUsername = TextBox_DistributeToUsername.Text.Trim();
            if (string.IsNullOrEmpty(toUsername))
            {
                Label_Alert.ForeColor = Color.Red;
                Label_Alert.Text = "请填写发票卷领取人";
                return;
            }
            if (string.IsNullOrEmpty(startNo) || string.IsNullOrEmpty(endNo))
            {
                Label_Alert.ForeColor = Color.Red;
                Label_Alert.Text = "发票“起止”号不能为空！";
                return;
            }
            string remark = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " +
                            CurrentSession.Personnel.Get().RealName + "分发给" + toUsername + "\n";


            #region modify zal 2015-07-29
            var arrayStartNo = startNo.Split(',');
            var arrayEndNo = endNo.Split(',');
            bool result = false;
            IInvoice invoice=new Invoice(GlobalConfig.DB.FromType.Write);
            for (int i = 0; i < arrayStartNo.Length; i++)
            {
                result = invoice.DistributeInvoiceRoll(new Guid(HiddenField_RollId.Value), Convert.ToInt64(arrayStartNo[i]), Convert.ToInt64(arrayEndNo[i]), remark);
            }
            #endregion

            if (result)
            {
                Label_Alert.ForeColor = Color.Green;
                RAM.ResponseScripts.Add("CancelWindow();alert('分发成功');parent.RebindGird();");  //zhangfan modified at 2012-July-18th
            }
            else
            {
                Label_Alert.ForeColor = Color.Red;
                Label_Alert.Text = "分发失败";
            }
        }
    }
}
