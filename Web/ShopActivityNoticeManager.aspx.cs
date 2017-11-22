using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Shop;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.UserControl;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ShopActivityNoticeManager : BasePage
    {
        private static readonly ShopActivityNoticeDal _shopActivityNotice = new ShopActivityNoticeDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void txt_OrderIndex_OnTextChanged(object sender, EventArgs e)
        {

            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null)
            {
                try
                {
                    var noticeId = new Guid(dataItem.GetDataKeyValue("NoticeID").ToString());
                    var orderIndex = Convert.ToInt32(textBox.Text);
                    orderIndex = orderIndex > 0 ? orderIndex : 1;
                    _shopActivityNotice.UpdateOrderIndex(noticeId, orderIndex);
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
            }
            NoteGrid.Rebind();
        }

        //下
        protected void DownOrderIndex(object sender, ImageClickEventArgs e)
        {
            var imageButton = (ImageButton)sender;
            var item = (GridDataItem)imageButton.Parent.Parent;
            var noticeID = new Guid(item.GetDataKeyValue("NoticeID").ToString());
            int orderIndex = Convert.ToInt32(item.GetDataKeyValue("OrderIndex").ToString());
            var result = _shopActivityNotice.UpdateOrderIndex(noticeID, orderIndex + 1);
            if (result)
            {
                NoteGrid.Rebind();
            }
            else
            {
                RAM.Alert("操作无效！");
            }
        }

        protected void RadGridAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            AjaxRequest(NoteGrid, e);
        }
        #region[绑定数据源]
        protected void NoteGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var notesList = _shopActivityNotice.SelectNoticeList().OrderBy(d => d.OrderIndex).ToList();
            NoteGrid.DataSource = notesList;
        }
        #endregion


        //删除
        protected void NoteGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridDataItem)e.Item;
            var noteId = new Guid(editedItem.GetDataKeyValue("NoticeID").ToString());
            try
            {
                var result = _shopActivityNotice.DeleteShopActivityNoticeInfo(noteId);
                if (result)
                {
                    NoteGrid.Rebind();
                }
                else
                {
                    RAM.Alert("操作无效！");
                }
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }


        #region[选择是否是公告]
        protected void IsNotice_CheckedChanged(object sender, EventArgs e)
        {
            var ccb = sender as ConfirmCheckBox;
            if (ccb != null)
            {
                var item = ccb.Parent.Parent as GridDataItem;
                if (item != null)
                {
                    var noteId = new Guid(item.GetDataKeyValue("NoticeID").ToString());
                    var isNote = ccb.Checked;
                    try
                    {
                        var result = _shopActivityNotice.UpdateIsNotice(isNote, noteId);
                        if (result)
                        {
                            NoteGrid.Rebind();
                        }
                    }
                    catch
                    {
                        RAM.Alert("设置公告失败！");
                    }
                }
            }
        }
        #endregion

        #region[选择是否显示]
        protected void IsShow_CheckedChanged(object sender, EventArgs e)
        {
            var ccb = sender as ConfirmCheckBox;
            if (ccb != null)
            {
                var item = ccb.Parent.Parent as GridDataItem;
                if (item != null)
                {
                    var noticeID = new Guid(item.GetDataKeyValue("NoticeID").ToString());
                    bool isShow = ccb.Checked;
                    var success = _shopActivityNotice.UpdateIsShow(isShow, noticeID);
                    if (success)
                    {
                        NoteGrid.Rebind();
                    }
                    else
                    {
                        RAM.Alert("设置显示失败！");
                    }
                }
            }
        }
        #endregion


    }
}