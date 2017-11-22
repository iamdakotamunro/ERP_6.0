using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class GoodsAttrGroup : BasePage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao=new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        private IList<AttributeGroupInfo> AttrGroupList
        {
            get { return _goodsAttributeGroupSao.GetAttrGroupList().OrderBy(p=>p.OrderIndex).ToList(); }
        }

        public bool IsSelected { get; set; }

        protected bool IsShow(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            return str == "True";
        }

        protected string GetMatchType(object matchType)
        {
            return EnumAttribute.GetKeyName((MatchType)matchType);
        }

        // 绑定数据
        protected void GroupGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GroupGrid.DataSource = AttrGroupList;
        }

        protected void GroupGrid_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                var editedItem = e.Item as GridEditableItem;
                if (editedItem != null)
                {
                    int groupId = Convert.ToInt32(editedItem.GetDataKeyValue("GroupId").ToString());
                    AttributeGroupInfo groupInfo = AttrGroupList.FirstOrDefault(g => g.GroupId == groupId);

                    IsSelected = (groupInfo != null && groupInfo.MatchType == 1);
                }
            }
        }

        // 增加
        protected void GroupGrid_InsertCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                int orderIndex = int.Parse(((TextBox)editedItem.FindControl("Txt_OrderIndex")).Text);
                string strUnit = ((TextBox)editedItem.FindControl("Txt_Unit")).Text.Trim();
                string groupName = ((TextBox)editedItem.FindControl("TB_GroupName")).Text;
                string matchType = ((DropDownList)editedItem.FindControl("DDL_MatchType")).SelectedValue;
                bool enableFilter = ((CheckBox)editedItem.FindControl("CK_EditFilter")).Checked;
                bool isMChoice = ((CheckBox)editedItem.FindControl("CK_EditIsMChoice")).Checked;
                bool isPriorityFilter = ((CheckBox)editedItem.FindControl("CK_EditIsPriorityFilter")).Checked;
                bool isUploadImage = ((CheckBox)editedItem.FindControl("CK_EditIsUploadImage")).Checked;
                
                if (AttrGroupList.Any(g => g.GroupName == groupName))
                {
                    RAM.Alert("添加失败,商品属性组组名重复!");
                    return;
                }
                var groupInfo = new AttributeGroupInfo
                {
                    GroupName = groupName,
                    MatchType = Convert.ToInt32(matchType),
                    OrderIndex = orderIndex,
                    EnabledFilter = enableFilter,
                    IsMChoice = isMChoice,
                    IsPriorityFilter = isPriorityFilter,
                    Unit = strUnit,
                    IsUploadImage = isUploadImage
                };
                try
                {
                    string errorMessage;
                    var result = _goodsAttributeGroupSao.AddAttrGroup(groupInfo, out errorMessage);
                    if (result)
                    {
                        //记录工作日志
                        //var pinfo = CurrentSession.Personnel.Get();
                        //_operationLogManager.Add(pinfo.PersonnelId, pinfo.RealName, groupInfo.,
                        //                 OperationPoint.GoodsSeriesManager.Delete.GetBusinessInfo(), 1);

                        //var type = HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttributeGrouping.GetBusinessInfo();
                        //var point = HRS.Enum.OperationTypePoint.CommodityAttributeGroupingState.Add.GetBusinessInfo();
                        //var logInfo = new HRS.Model.OperationLogInfo
                        //{
                        //    LogId = Guid.NewGuid(),
                        //    PersonnelId = pinfo.PersonnelId,
                        //    RealName = pinfo.RealName,
                        //    TypeId = new Guid(type.Key),
                        //    PointId = new Guid(point.Key),
                        //    OperateTime = DateTime.Now,
                        //    Description = type.Value + "--" + point.Value,
                        //    IdentifyKey = Convert.ToString(groupInfo.GroupName)
                        //};
                        //OperationLogManager.InsertOperationLog(logInfo);
                    }
                    else
                    {
                        RAM.Alert("商品属性组添加无效!" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("商品属性组添加失败!");
                }
            }
            GroupGrid.Rebind();
        }

        // 删除
        protected void GroupGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                int groupId = Convert.ToInt32(editedItem.GetDataKeyValue("GroupId").ToString());
                try
                {
                    var attrWordsList = _goodsAttributeGroupSao.GetAttrWordsListByGroupId(groupId).Where(p => !string.IsNullOrEmpty(p.AttrWordImage));
                    if (attrWordsList.Any())
                    {
                        RAM.Alert("该“属性组”已有“属性词”上传图片，不允许删除!");
                        return;
                    }
                    string errorMessage;
                    var result = _goodsAttributeGroupSao.DeleteAttrGroup(groupId, out errorMessage);
                    if (result)
                    {
                        ////记录工作日志
                        //var pinfo = CurrentSession.Personnel.Get();
                        //var type = HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttributeGrouping.GetBusinessInfo();
                        //var point = HRS.Enum.OperationTypePoint.CommodityAttributeGroupingState.Delete.GetBusinessInfo();
                        //var logInfo = new HRS.Model.OperationLogInfo
                        //{
                        //    LogId = Guid.NewGuid(),
                        //    PersonnelId = pinfo.PersonnelId,
                        //    RealName = pinfo.RealName,
                        //    TypeId = new Guid(type.Key),
                        //    PointId = new Guid(point.Key),
                        //    OperateTime = DateTime.Now,
                        //    Description = type.Value + "--" + point.Value,
                        //    IdentifyKey = Convert.ToString(groupId)
                        //};
                        //OperationLogManager.InsertOperationLog(logInfo);
                    }
                    else
                    {
                        RAM.Alert("商品属性组删除无效!" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("商品属性组删除失败!");
                }
            }
            GroupGrid.Rebind();
        }

        // 修改
        protected void GroupGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                int groupId = Convert.ToInt32(editedItem.GetDataKeyValue("GroupId").ToString());
                int orderIndex = int.Parse(((TextBox)editedItem.FindControl("Txt_OrderIndex")).Text);
                string strUnit = ((TextBox)editedItem.FindControl("Txt_Unit")).Text.Trim();
                string groupName = ((TextBox)editedItem.FindControl("TB_GroupName")).Text;
                string matchType = ((DropDownList)editedItem.FindControl("DDL_MatchType")).SelectedValue;
                bool enableFilter = ((CheckBox)editedItem.FindControl("CK_EditFilter")).Checked;
                bool isMChoice = ((CheckBox)editedItem.FindControl("CK_EditIsMChoice")).Checked;
                bool isPriorityFilter = ((CheckBox)editedItem.FindControl("CK_EditIsPriorityFilter")).Checked;
                bool isUploadImage = ((CheckBox)editedItem.FindControl("CK_EditIsUploadImage")).Checked;

                if (AttrGroupList.Any(g => g.GroupName == groupName && g.GroupId != groupId))
                {
                    RAM.Alert("更新失败,商品属性组组名重复!");
                    return;
                }

                if (!isUploadImage)
                {
                    var attrWordsList = _goodsAttributeGroupSao.GetAttrWordsListByGroupId(groupId).Where(p => !string.IsNullOrEmpty(p.AttrWordImage));
                    if (attrWordsList.Any())
                    {
                        RAM.Alert("该“属性组”已有“属性词”上传图片，不允许取消!");
                        return;
                    }
                }

                var groupInfo = new AttributeGroupInfo
                {
                    GroupId = groupId,
                    GroupName = groupName,
                    MatchType = Convert.ToInt32(matchType),
                    OrderIndex = orderIndex,
                    EnabledFilter = enableFilter,
                    IsMChoice = isMChoice,
                    IsPriorityFilter = isPriorityFilter,
                    Unit = strUnit,
                    IsUploadImage = isUploadImage
                };
                try
                {
                    string errorMessage;
                    var result = _goodsAttributeGroupSao.UpdateAttrGroup(groupInfo, out errorMessage);

                    if (result)
                    {
                        ////记录工作日志
                        //var pinfo = CurrentSession.Personnel.Get();
                        //var type = HRS.Enum.OperationTypePoint.OperationLogTypeEnum.CommodityAttributeGrouping.GetBusinessInfo();
                        //var point = HRS.Enum.OperationTypePoint.CommodityAttributeGroupingState.Edit.GetBusinessInfo();
                        //var logInfo = new HRS.Model.OperationLogInfo
                        //{
                        //    LogId = Guid.NewGuid(),
                        //    PersonnelId = pinfo.PersonnelId,
                        //    RealName = pinfo.RealName,
                        //    TypeId = new Guid(type.Key),
                        //    PointId = new Guid(point.Key),
                        //    OperateTime = DateTime.Now,
                        //    Description = type.Value + "--" + point.Value,
                        //    IdentifyKey = Convert.ToString(groupInfo.GroupId)
                        //};
                        //OperationLogManager.InsertOperationLog(logInfo);
                    }
                    else
                    {
                        RAM.Alert("商品属性组更新无效!" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("商品属性组更新失败!");
                }
            }
            GroupGrid.Rebind();
        }
    }
}