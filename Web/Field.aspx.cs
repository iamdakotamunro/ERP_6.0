using System;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class FieldAw : BasePage
    {
        readonly IGoodsCenterSao _goodsFieldSao = new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //主属性
        protected void FieldGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = _goodsFieldSao.GetFieldList().Where(w => w.ParentFieldId == Guid.Empty).OrderBy(w => w.OrderIndex).ToList();
            FieldGrid.DataSource = list;
        }

        //子属性
        protected void FieldGrid_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            var parentFieldId = new Guid(dataItem.GetDataKeyValue("FieldId").ToString());
            int isCompField = Convert.ToInt32((dataItem.GetDataKeyValue("IsCompField")));
            if (isCompField == 1)
            {
                e.DetailTableView.Visible = false;
            }
            else
            {
                var list = _goodsFieldSao.GetChildFieldListByFieldId(parentFieldId).OrderBy(w => w.OrderIndex).ToList();
                e.DetailTableView.DataSource = list;
                e.DetailTableView.Visible = true;
            }
        }

        //添加属性
        protected void FieldGrid_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var fieldId = Guid.NewGuid();
            Guid parentfieldId;
            string fieldName;
            string fieldValue;
            FieldInfo fieldInfo;

            switch (editedItem.OwnerTableView.DataMember)
            {
                case "TopField":
                    {
                        parentfieldId = Guid.Empty;
                        fieldName = ((TextBox)editedItem.FindControl("TB_FieldName")).Text; 
                        fieldValue = ((TextBox)editedItem.FindControl("TB_FieldValue")).Text;

                        try
                        {
                            fieldInfo = new FieldInfo
                            {
                                FieldId = fieldId,
                                ParentFieldId = parentfieldId,
                                FieldName = fieldName,
                                FieldValue = fieldValue
                            };

                            string errorMessage;
                            var result = _goodsFieldSao.AddField(fieldInfo, out errorMessage);
                            if (result)
                            {
                                //商品属性添加操作记录添加
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, fieldId, "",
                                                         OperationPoint.GoodsAttributeManager.Add.GetBusinessInfo(), string.Empty);
                                //WebControl.AddCommodityAttributeOperation(HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttribute, CurrentSession.Personnel.Get().PersonnelId, fieldId, fieldName + fieldValue, HRS.Enum.OperationTypePoint.CommodityAttributeState.AddAttribute, string.Empty);
                                FieldGrid.Rebind();
                            }
                            else
                            {
                                RAM.Alert("操作无效！" + errorMessage);
                            }
                        }
                        catch
                        {
                            RAM.Alert("商品属性信息添加失败！");
                        }
                    }
                    break;
                case "CombField":
                    {
                        GridDataItem parentItem = editedItem.OwnerTableView.ParentItem;
                        parentfieldId = new Guid(parentItem.GetDataKeyValue("FieldId").ToString());
                        fieldName = ((Label)parentItem.FindControl("LB_FieldName")).Text;
                        fieldValue = ((TextBox)editedItem.FindControl("TB_FieldValue")).Text;
                        try
                        {
                            fieldInfo = new FieldInfo
                            {
                                FieldId = fieldId,
                                ParentFieldId = parentfieldId,
                                FieldName = fieldName,
                                FieldValue = fieldValue
                            };

                            string errorMessage;
                            var result = _goodsFieldSao.AddField(fieldInfo, out errorMessage);
                            if (result)
                            {
                                //商品属性添加操作记录添加
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, fieldId, "",
                                                         OperationPoint.GoodsAttributeManager.Add.GetBusinessInfo(), string.Empty);
                                //WebControl.AddCommodityAttributeOperation(HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttribute, CurrentSession.Personnel.Get().PersonnelId, fieldId, fieldName + fieldValue, HRS.Enum.OperationTypePoint.CommodityAttributeState.AddAttribute, string.Empty);
                                editedItem.OwnerTableView.Rebind();
                            }
                            else
                            {
                                RAM.Alert("操作无效！" + errorMessage);
                            }
                        }
                        catch
                        {
                            RAM.Alert("商品属性信息添加失败！");
                        }
                    }
                    break;
            }
        }

        //修改属性
        protected void FieldGrid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;

            var fieldId = new Guid(editedItem.GetDataKeyValue("FieldId").ToString());
            if (fieldId == Guid.Empty)
            {
                RAM.Alert(string.Format("操作无效！{0}", "FieldId不能空"));
                return;
            }
            int orderIndex = Convert.ToInt32(editedItem.GetDataKeyValue("OrderIndex").ToString());
            Guid parentfieldId;
            string fieldName;
            string fieldValue;
            FieldInfo fieldInfo;

            switch (editedItem.OwnerTableView.DataMember)
            {
                case "TopField":
                    {
                        parentfieldId = Guid.Empty;
                        fieldName = ((TextBox)editedItem.FindControl("TB_FieldName")).Text;
                        fieldValue = ((TextBox)editedItem.FindControl("TB_FieldValue")).Text;
                        try
                        {
                            fieldInfo = new FieldInfo
                            {
                                FieldId = fieldId,
                                ParentFieldId = parentfieldId,
                                OrderIndex = orderIndex,
                                FieldName = fieldName,
                                FieldValue = fieldValue
                            };

                            string errorMessage;
                            var result = _goodsFieldSao.UpdateField(fieldInfo, out errorMessage);
                            if (result)
                            {
                                //商品属性更改操作记录添加
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, fieldId, "",
                                                         OperationPoint.GoodsAttributeManager.Edit.GetBusinessInfo(), string.Empty);
                                //WebControl.AddCommodityAttributeOperation(HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttribute, CurrentSession.Personnel.Get().PersonnelId, fieldId, fieldName + fieldValue, HRS.Enum.OperationTypePoint.CommodityAttributeState.EditAttribute, string.Empty);
                                FieldGrid.Rebind();
                            }
                            else
                            {
                                RAM.Alert("操作无效！" + errorMessage);
                            }
                        }
                        catch
                        {
                            RAM.Alert("商品属性信息更改失败！");
                        }
                    }
                    break;
                case "CombField":
                    {
                        GridDataItem parentItem = editedItem.OwnerTableView.ParentItem;
                        parentfieldId = new Guid(parentItem.GetDataKeyValue("FieldId").ToString());
                        fieldName = ((Label)parentItem.FindControl("LB_FieldName")).Text;
                        fieldValue = ((TextBox)editedItem.FindControl("TB_FieldValue")).Text;

                        try
                        {
                            fieldInfo = new FieldInfo
                            {
                                FieldId = fieldId,
                                ParentFieldId = parentfieldId,
                                OrderIndex = orderIndex,
                                FieldName = fieldName,
                                FieldValue = fieldValue
                            };
                            string errorMessage;
                            var result = _goodsFieldSao.UpdateField(fieldInfo, out errorMessage);
                            if (result)
                            {
                                //商品属性更改操作记录添加
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, fieldId, "",
                                                         OperationPoint.GoodsAttributeManager.Edit.GetBusinessInfo(), string.Empty);
                                //WebControl.AddCommodityAttributeOperation(HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttribute, CurrentSession.Personnel.Get().PersonnelId, fieldId, fieldName + fieldValue, HRS.Enum.OperationTypePoint.CommodityAttributeState.EditAttribute, string.Empty);
                                editedItem.OwnerTableView.Rebind();
                            }
                            else
                            {
                                RAM.Alert("操作无效！" + errorMessage);
                            }
                        }
                        catch
                        {
                            RAM.Alert("商品属性信息更改失败！");
                        }
                    }
                    break;
            }
        }

        //删除属性
        protected void FieldGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridDataItem)e.Item;
            var fieldId = new Guid(editedItem.GetDataKeyValue("FieldId").ToString());
            if (fieldId == Guid.Empty)
            {
                RAM.Alert(string.Format("操作无效！{0}", "FieldId不能空"));
                return;
            }
            try
            {
                string errorMessage;
                var result = _goodsFieldSao.DeleteField(fieldId, out errorMessage);
                if (result)
                {
                    //商品属性更改操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, fieldId, "",
                                             OperationPoint.GoodsAttributeManager.Edit.GetBusinessInfo(), string.Empty);
                    if (editedItem.OwnerTableView.DataMember == "TopField")
                    {
                        FieldGrid.Rebind();
                    }
                    else
                    {
                        editedItem.OwnerTableView.Rebind();
                    }
                }
                else
                {
                    RAM.Alert("操作无效！" + errorMessage);
                }
            }
            catch
            {
                RAM.Alert("商品属性信息删除失败！");
            }
        }

        ////上
        //protected void UpOrderIndex(object sender, ImageClickEventArgs e)
        //{
        //    var imageButton = (ImageButton)sender;
        //    var item = (GridDataItem)imageButton.Parent.Parent;
        //    var fieldId = new Guid(item.GetDataKeyValue("FieldId").ToString());
        //    int orderIndex = Convert.ToInt32(item.GetDataKeyValue("OrderIndex").ToString());

        //    string errorMessage;
        //    switch (item.OwnerTableView.DataMember)
        //    {
        //        case "TopField":
        //            {
        //                var result = FieldManager.UpdateFieldOrderIndex(fieldId, orderIndex - 1, out errorMessage);
        //                if (result)
        //                {
        //                    FieldGrid.Rebind();
        //                }
        //                else
        //                {
        //                    RAM.Alert("操作无效！" + errorMessage);
        //                }
        //            }
        //            break;
        //        case "CombField":
        //            {
        //                var result = FieldManager.UpdateFieldOrderIndex(fieldId, orderIndex - 1, out errorMessage);
        //                if (result)
        //                {
        //                    item.OwnerTableView.Rebind();
        //                }
        //                else
        //                {
        //                    RAM.Alert("操作无效！" + errorMessage);
        //                }
        //            }
        //            break;
        //    }
        //}

        ////下
        //protected void DownOrderIndex(object sender, ImageClickEventArgs e)
        //{
        //    var imageButton = (ImageButton)sender;
        //    var item = (GridDataItem)imageButton.Parent.Parent;
        //    var fieldId = new Guid(item.GetDataKeyValue("FieldId").ToString());
        //    int orderIndex = Convert.ToInt32(item.GetDataKeyValue("OrderIndex").ToString());

        //    string errorMessage;
        //    switch (item.OwnerTableView.DataMember)
        //    {
        //        case "TopField":
        //            {
        //                var result = FieldManager.UpdateFieldOrderIndex(fieldId, orderIndex + 1, out errorMessage);
        //                if (result)
        //                {
        //                    FieldGrid.Rebind();
        //                }
        //                else
        //                {
        //                    RAM.Alert("操作无效！");
        //                }
        //            }
        //            break;
        //        case "CombField":
        //            {
        //                var result = FieldManager.UpdateFieldOrderIndex(fieldId, orderIndex + 1, out errorMessage);
        //                if (result)
        //                {
        //                    item.OwnerTableView.Rebind();
        //                }
        //                else
        //                {
        //                    RAM.Alert("操作无效！");
        //                }
        //            }
        //            break;
        //    }
        //}

        protected void txt_OrderIndex_OnTextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null && dataItem.HasChildItems)
            {
                try
                {
                    var fieldId = new Guid(dataItem.GetDataKeyValue("FieldId").ToString());
                    if (fieldId == Guid.Empty)
                    {
                        RAM.Alert(string.Format("操作无效！{0}", "FieldId不能空"));
                        return;
                    }
                    var orderIndex = Convert.ToInt32(textBox.Text);
                    orderIndex = orderIndex > 0 ? orderIndex : 1;
                    string errorMessage;
                    switch (dataItem.OwnerTableView.DataMember)
                    {
                        case "TopField":
                            {
                                var result = _goodsFieldSao.UpdateFieldOrderIndex(fieldId, orderIndex, out errorMessage);
                                if (result)
                                {
                                    FieldGrid.Rebind();
                                }
                                else
                                {
                                    RAM.Alert("操作无效！");
                                }
                            }
                            break;
                        case "CombField":
                            {
                                var result = _goodsFieldSao.UpdateFieldOrderIndex(fieldId, orderIndex, out errorMessage);
                                if (result)
                                {
                                    dataItem.OwnerTableView.Rebind();
                                }
                                else
                                {
                                    RAM.Alert("操作无效！");
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
                FieldGrid.Rebind();
            }
        }
    }
}