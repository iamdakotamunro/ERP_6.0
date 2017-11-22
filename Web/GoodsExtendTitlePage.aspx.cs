using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class GoodsExtendTitlePage : BasePage
    {
        readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected IDictionary<int, string> GetEnumDic()
        {
            IDictionary<int, string> dic = EnumAttribute.GetDict<GoodsServicePosition>();
            return dic;
        }

        protected string GetEnumDic(int key)
        {
            IDictionary<int, string> dic = EnumAttribute.GetDict<GoodsServicePosition>();
            return dic[key];
        }

        // 数据绑定
        protected void RGTitle_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RGTitle.DataSource = _goodsCenterSao.GetDescriptionExtendTitleInfoList();
        }

        // 增加
        protected void RGTitle_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            try
            {
                var info = new DescriptionExtendTitleInfo
                {
                    Id = Guid.NewGuid(),
                    Title = ((TextBox)editedItem.FindControl("TB_TitleName")).Text,
                    Position = Convert.ToInt32(((RadComboBox)editedItem.FindControl("RCB_Position")).SelectedValue)
                };
                string errorMessage;
                var result = _goodsCenterSao.AddDescriptionExtendTitle(info, out errorMessage);
                if (result == false)
                {
                    RAM.Alert("添加标题无效" + errorMessage);
                }
            }
            catch (Exception exp)
            {
                RAM.Alert("添加标题失败！\\n\\n错误提示：" + exp.Message);
            }
        }

        // 删除
        protected void RGTitle_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            try
            {
                var id = new Guid(editedItem.GetDataKeyValue("Id").ToString());
                string errorMessage;
                var result = _goodsCenterSao.DeleteDescriptionExtendTitle(id, out errorMessage);
                if (result == false)
                {
                    RAM.Alert("删除标题无效！" + errorMessage);
                }
            }
            catch (Exception exp)
            {
                RAM.Alert("删除标题失败！\\n\\n错误提示：" + exp.Message);
            }
        }

        // 修改
        protected void RGTitle_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            try
            {
                var info = new DescriptionExtendTitleInfo
                {
                    Id = new Guid(editedItem.GetDataKeyValue("Id").ToString()),
                    Title = ((TextBox)editedItem.FindControl("TB_TitleName")).Text,
                    Position = Convert.ToInt32(((RadComboBox)editedItem.FindControl("RCB_Position")).SelectedValue)
                };
                string errorMessage;
                var result = _goodsCenterSao.UpdateDescriptionExtendTitle(info, out errorMessage);
                if (result == false)
                {
                    RAM.Alert("修改标题无效！" + errorMessage);
                }
            }
            catch (Exception exp)
            {
                RAM.Alert("修改标题失败！\\n\\n错误提示：" + exp.Message);
            }
        }
    }
}