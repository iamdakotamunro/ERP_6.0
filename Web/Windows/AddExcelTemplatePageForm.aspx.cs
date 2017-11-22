using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Company;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class AddExcelTemplatePageForm : System.Web.UI.Page
    {
        private readonly ExcelTemplate _temp = new ExcelTemplate(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStock();
            }
        }

        /// <summary>
        /// 加载仓库
        /// </summary>
        private void BindStock()
        {
            var personinfo = CurrentSession.Personnel.Get();
            var result = WMSSao.GetWarehouseAuthDic(personinfo.PersonnelId);
            RCB_Stock.DataSource = result!=null && result.WarehouseDics!=null?result.WarehouseDics:new Dictionary<Guid, string>();
            RCB_Stock.DataTextField = "Value";
            RCB_Stock.DataValueField = "Key";
            RCB_Stock.DataBind();
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            string templateName = txt_TemplateName.Text;
            string shipper = txt_Shipper.Text;
            string contactPerson = txt_ContactPerson.Text;
            string contactAddress = txt_ContactAddress.Text;
            string remarks = txt_Remarks.Text;
            string customer = txt_Customer.Text;
            var warehouseId = new Guid(RCB_Stock.SelectedValue);
            var tempInfo = new ExcelTemplateInfo
            {
                TemplateName = templateName,
                Shipper = shipper,
                ContactPerson = contactPerson,
                ContactAddress = contactAddress,
                Customer = customer,
                Remarks = remarks,
                WarehouseId = warehouseId
            };
            try
            {
                if (_temp.ExcelTemplateIsExists(tempInfo.TemplateName, tempInfo.WarehouseId))
                {
                    RAM.Alert("添加失败 已经含有相同的模板名!");
                    return;
                }
                bool result = _temp.Add(tempInfo);
                if (result)
                {
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                else
                {
                    RAM.Alert("添加失败!");
                }

            }
            catch (Exception ex)
            {
                RAM.Alert("添加失败 可能已经含有相同的模板名!" + ex.Message);
            }
        }
    }
}