using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.UserControl;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class TemplateManage : BasePage
    {
        private static readonly DAL.Implement.Company.TemplateManage _templateManage = 
            new DAL.Implement.Company.TemplateManage(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected Dictionary<int, string> GetEnumDic()
        {
            var dic = (Dictionary<int, string>) EnumAttribute.GetDict<TemplateType>();
            return dic;
        }

        protected void RgTempNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgTemp.DataSource = _templateManage.GetTemplateList();
        }

        protected void RgTempUpdateCommand(object sender, GridCommandEventArgs e)
        {
            var edit = e.Item as GridEditableItem;
            if (edit != null)
            {
                var tempId = new Guid(edit.GetDataKeyValue("TemplateID").ToString());
                string templateCaption = ((TextBox)edit.FindControl("TBTemplateCaption")).Text;
                string templateContent = ((TextBox)edit.FindControl("TBTemplateContent")).Text;
                int templateType = Convert.ToInt32(((RadComboBox)edit.FindControl("RCBTemplateType")).SelectedValue);
                var info = new TemplateInfo
                {
                    TemplateID = tempId,
                    TemplateCaption = templateCaption,
                    TemplateContent = templateContent,
                    TemplateType = templateType
                };
                try
                {
                    _templateManage.Update(info);
                }
                catch (Exception ex)
                {
                    RAM.Alert("修改失败！" + ex.Message);
                }
            }
        }

        protected void RgTempDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var edit = e.Item as GridEditableItem;
            if (edit != null)
            {
                var tempId = new Guid(edit.GetDataKeyValue("TemplateID").ToString());
                try
                {
                    _templateManage.Delete(tempId);
                }
                catch (Exception ex)
                {
                    RAM.Alert("删除失败！" + ex.Message);
                }
            }
        }

        protected void RgTempInsertCommand(object sender, GridCommandEventArgs e)
        {
            var editItem = e.Item as GridEditableItem;
            if (editItem != null)
            {
                string templateCaption = ((TextBox)editItem.FindControl("TBTemplateCaption")).Text;
                string templateContent = ((TextBox)editItem.FindControl("TBTemplateContent")).Text;
                int templateType = Convert.ToInt32(((RadComboBox)editItem.FindControl("RCBTemplateType")).SelectedValue);
                var info = new TemplateInfo
                {
                    TemplateCaption = templateCaption,
                    TemplateContent = templateContent,
                    TemplateType = templateType,
                    TemplateState = 1
                };
                try
                {
                    _templateManage.Insert(info);
                }
                catch (Exception ex)
                {
                    RAM.Alert("添加失败，可能已存在相同的模板名！" + ex.Message);
                }
            }
        }

        protected void TemplateState_CheckedChanged(object sender, EventArgs e)
        {
            var ccb = sender as ConfirmCheckBox;
            if (ccb != null)
            {
                var item = ccb.Parent.Parent as GridDataItem;
                if (item != null)
                {
                    var tempId = new Guid(item.GetDataKeyValue("TemplateID").ToString());
                    int ischecked = ccb.Checked ? 1 : 0;

                    try
                    {
                        _templateManage.UpdateState(tempId, ischecked);
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("修改失败！" + ex.Message);
                    }
                }
            }
        }

        protected void RcbDataBinding(object sender, EventArgs e)
        {
            if (!rgTemp.MasterTableView.IsItemInserted)
            {
                var rcb = sender as RadComboBox;
                if (rcb != null)
                {
                    var item = rcb.Parent.Parent as GridDataItem;
                    if (item != null)
                    {
                        var tempId = new Guid(item.GetDataKeyValue("TemplateID").ToString());
                        ((RadComboBox)item.FindControl("RCBTemplateType")).SelectedValue = string.Format("{0}",
                            (from f in _templateManage.GetTemplateList() where f.TemplateID == tempId select f.TemplateType).FirstOrDefault());
                    }
                }
            }
        }
    }
}
