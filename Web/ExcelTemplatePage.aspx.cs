using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Company;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    ///<summary>
    /// 进货单位信息模版
    /// Modify by liucaijun at 2011-November-07th
    /// 代码优化
    ///</summary>
    public partial class ExcelTemplatePage : BasePage
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
            WarehouseDic = result != null && result.WarehouseDics != null ? result.WarehouseDics : new Dictionary<Guid, string>();
            RCB_Stock.DataSource = WarehouseDic;
            RCB_Stock.DataTextField = "Value";
            RCB_Stock.DataValueField = "Key";
            RCB_Stock.DataBind();
        }

        public Dictionary<Guid,String> WarehouseDic
        {
            get
            {
                if (ViewState["WarehouseDic"] == null)
                    return new Dictionary<Guid,String>();
                return (Dictionary<Guid, String>)ViewState["WarehouseDic"];
            }
            set { ViewState["WarehouseDic"] = value; }
        }

        protected void TempRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var warehouseId = string.IsNullOrEmpty(RCB_Stock.SelectedValue) ? Guid.Empty : new Guid(RCB_Stock.SelectedValue);
            TempRadGrid.DataSource = warehouseId != Guid.Empty ? _temp.GetExcelTemplateListByWarehouseId(warehouseId) : _temp.GetExcelTemplateList();
        }

        protected void RCB_Stock_SelectedIndexChanged(object obj, EventArgs e)
        {
            TempRadGrid.Rebind();
        }

        /// <summary>
        /// 获取仓库名称
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        protected string GetWarehouseName(string warehouseId)
        {
            return WarehouseDic.ContainsKey(new Guid(warehouseId))
                ? WarehouseDic[new Guid(warehouseId)]
                : string.Empty;
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            TempRadGrid.Rebind();
        }


        protected void TempRadGrid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var tempId = new Guid(editedItem.GetDataKeyValue("TempId").ToString());
                string templateName = ((TextBox) editedItem.FindControl("TB_TemplateName")).Text;
                string shipper = ((TextBox) editedItem.FindControl("TB_Shipper")).Text;
                string contactPerson = ((TextBox) editedItem.FindControl("TB_ContactPerson")).Text;
                string contactAddress = ((TextBox) editedItem.FindControl("TB_ContactAddress")).Text;
                string remarks = ((TextBox) editedItem.FindControl("TB_Remarks")).Text;
                string customer = ((TextBox) editedItem.FindControl("TB_Customer")).Text;
                var tempInfo = new ExcelTemplateInfo
                                                 {
                                                     TempId = tempId,
                                                     TemplateName = templateName,
                                                     Shipper = shipper,
                                                     ContactPerson = contactPerson,
                                                     ContactAddress = contactAddress,
                                                     Customer = customer,
                                                     Remarks = remarks,
                                                     WarehouseId = new Guid(RCB_Stock.SelectedValue)
                                                 };
                try
                {
                    _temp.Update(tempInfo);
                }
                catch (Exception ex)
                {
                    RAM.Alert("修改失败!可能修改的模板名存在相同的!"+ex.Message );
                }
            }
        }

        protected void TempRadGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var tempId =new Guid(editedItem.GetDataKeyValue("TempId").ToString());
                try
                {
                    _temp.Delete(tempId);
                }
                catch (Exception ex)
                {
                    RAM.Alert("删除失败!" + ex.Message);
                }
            }
        }

        protected void TempRadGrid_InsertCommand(object sender, GridCommandEventArgs e)
        {

            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                string templateName = ((TextBox) editedItem.FindControl("TB_TemplateName")).Text;
                string shipper = ((TextBox) editedItem.FindControl("TB_Shipper")).Text;
                string contactPerson = ((TextBox) editedItem.FindControl("TB_ContactPerson")).Text;
                string contactAddress = ((TextBox) editedItem.FindControl("TB_ContactAddress")).Text;
                string remarks = ((TextBox) editedItem.FindControl("TB_Remarks")).Text;
                string customer = ((TextBox) editedItem.FindControl("TB_Customer")).Text;
                var tempInfo = new ExcelTemplateInfo
                                                 {
                                                     TemplateName = templateName,
                                                     Shipper = shipper,
                                                     ContactPerson = contactPerson,
                                                     ContactAddress = contactAddress,
                                                     Customer = customer,
                                                     Remarks = remarks
                                                 };
                try
                {
                    _temp.Insert(tempInfo);
                }
                catch (Exception ex)
                {
                    RAM.Alert("添加失败 可能已经含有相同的模板名!" + ex.Message);
                }
            }
        }
    }
}
