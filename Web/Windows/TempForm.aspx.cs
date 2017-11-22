using System;
using System.Linq;
using ERP.DAL.Implement.Company;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class TempForm : WindowsPage
    {

        private readonly ExcelTemplate _temp = new ExcelTemplate(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["TempId"] != "00000000-0000-0000-0000-000000000000")
                {
                    var tempId = new Guid(Request["TempId"]);
                    ExcelTemplateInfo tempInfo = _temp.GetExcelTemplateInfo(tempId);
                    tbxName.Text = tempInfo.TemplateName;
                    tbxCompany.Text = tempInfo.Customer;
                    tbxUsername.Text = tempInfo.Shipper;
                    tbxUser.Text = tempInfo.ContactPerson;
                    tbxAddress.Text = tempInfo.ContactAddress;
                    tbxRemarks.Text = tempInfo.Remarks;
                }
            }
        }

        protected void Bt_Temp_Save(object sender, EventArgs e)
        {
            var tempInfo = new ExcelTemplateInfo();
            string tempName = tbxName.Text;
            string companyName = tbxCompany.Text;
            string userName = tbxUsername.Text;
            string user = tbxUser.Text;
            string address = tbxAddress.Text;
            string remark = tbxRemarks.Text;
            tempInfo.TemplateName = tempName;
            tempInfo.Customer = companyName;
            tempInfo.Shipper = userName;
            tempInfo.ContactPerson = user;
            tempInfo.ContactAddress = address;
            tempInfo.Remarks = remark;
            if (Request["TempId"] == "00000000-0000-0000-0000-000000000000")
            {
                tempInfo.TempId = Guid.NewGuid();
                try
                {

                    var list = _temp.GetExcelTemplateList();
                    if (list.Count > 0)
                    {
                        var info = list.FirstOrDefault(w => w.TemplateName == tempInfo.TemplateName);
                        if (info == null)
                        {
                            _temp.Insert(tempInfo);
                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        else
                        {
                            RAM.Alert("模版名称已存在！");
                        }
                    }
                    else
                    {
                        RAM.Alert("读取模块信息异常！");
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert("添加失败，可能已经含有相同的模板名!" + ex.Message);
                }
            }
            else
            {
                tempInfo.TempId = new Guid(Request["TempId"]);
                try
                {
                    _temp.Update(tempInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    RAM.Alert("修改失败!" + ex.Message);
                }
            }
        }
    }
}
