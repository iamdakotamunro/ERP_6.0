using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>发票管理 
    /// </summary>
    public partial class InvoiceRoll : BasePage
    {
        //readonly Invoice _invoice = new Invoice();

        protected SubmitController SubmitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["InsertGoodsOrder"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["InsertGoodsOrder"] = SubmitController;
            }
            return (SubmitController)ViewState["InsertGoodsOrder"];
        }

        /// <summary>页面加载
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {

            }
        }

        /// <summary>绑定数据源
        /// </summary>
        protected void RadGrid_InvoiceRollList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            ReadInvoiceRollList();
        }

        /// <summary>列表源数据
        /// </summary>
        private void ReadInvoiceRollList()
        {
            RadGrid_InvoiceRollList.DataSource = Invoice.ReadInstance.GetInvoiceRollList();
        }

        /// <summary>新建
        /// </summary>
        protected void RadGrid_InvoiceRollList_InsertCommand(object sender, GridCommandEventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                return;
            }
            var editedItem = e.Item as GridEditableItem;
            var newId = Guid.NewGuid();
            if (editedItem != null)
            {
                var textBoxReceiptor = editedItem.FindControl("TextBox_Receiptor") as TextBox;
                var textBoxInvoiceCode = editedItem.FindControl("TextBox_InvoiceCode") as TextBox;
                var textBoxInvoiceStartNo = editedItem.FindControl("TextBox_InvoiceStartNo") as TextBox;
                var textBoxInvoiceEndNo = editedItem.FindControl("TextBox_InvoiceEndNo") as TextBox;
                var textBoxInvoiceRollCount = editedItem.FindControl("TextBox_InvoiceRollCount") as TextBox;
                var radDatePickerTime = editedItem.FindControl("RadDatePicker_Time") as RadDatePicker;
                var radComboBoxFiliale = editedItem.FindControl("RCB_Filiale") as RadComboBox;
                var invoiceRollInfo = new Keede.Ecsoft.Model.InvoiceRoll
                                          {
                                              Id = newId,
                                              Receiptor = textBoxReceiptor!=null?textBoxReceiptor.Text:string.Empty,
                                              InvoiceCode = textBoxInvoiceCode!=null?textBoxInvoiceCode.Text:string.Empty,
                                              InvoiceStartNo =textBoxInvoiceStartNo==null?0:int.Parse(string.IsNullOrEmpty(textBoxInvoiceStartNo.Text.Trim()) ? "0" : textBoxInvoiceStartNo.Text.Trim()),
                                              InvoiceEndNo = textBoxInvoiceEndNo==null?0:int.Parse(string.IsNullOrEmpty(textBoxInvoiceEndNo.Text.Trim()) ? "0" : textBoxInvoiceEndNo.Text.Trim()),
                                              InvoiceRollCount = textBoxInvoiceRollCount==null?0:int.Parse(string.IsNullOrEmpty(textBoxInvoiceRollCount.Text.Trim()) ? "0" : textBoxInvoiceRollCount.Text.Trim()),
                                              CreateTime =radDatePickerTime!=null?radDatePickerTime.SelectedDate ?? DateTime.Now:DateTime.Now,
                                              FilialeId =radComboBoxFiliale!=null?new Guid(radComboBoxFiliale.SelectedValue):Guid.Empty
                                          };
                invoiceRollInfo.InvoiceCount = invoiceRollInfo.InvoiceEndNo - invoiceRollInfo.InvoiceStartNo + 1;
                if (invoiceRollInfo.InvoiceCount < 1)
                {
                    RAM.Alert("发票份数是：" + invoiceRollInfo.InvoiceCount + "份，填写发票号码错误");
                    return;
                }
                var success = Invoice.WriteInstance.AddInvoiceRoll(invoiceRollInfo);
                if (success)
                {
                    var personnelInfo = CurrentSession.Personnel.Get();
                    //发票库管理发票入库操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceRollInfo.Id, invoiceRollInfo.InvoiceCode,
                                              OperationPoint.InvoiceStorageManage.InvoiceIntoLibrary.GetBusinessInfo(), string.Empty);
                    SubmitController.Submit();
                    RAM.ResponseScripts.Add("location.href=location.href;");
                }
                else
                {
                    RAM.Alert("购买发票添加失败");
                }
            }
        }

        /// <summary>编辑
        /// </summary>
        protected void RadGrid_InvoiceRollList_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var textBoxReceiptor = editedItem.FindControl("TextBox_Receiptor") as TextBox;
                var textBoxInvoiceCode = editedItem.FindControl("TextBox_InvoiceCode") as TextBox;
                var textBoxInvoiceStartNo = editedItem.FindControl("TextBox_InvoiceStartNo") as TextBox;
                var textBoxInvoiceEndNo = editedItem.FindControl("TextBox_InvoiceEndNo") as TextBox;
                var textBoxInvoiceRollCount = editedItem.FindControl("TextBox_InvoiceRollCount") as TextBox;
                var radDatePickerTime = editedItem.FindControl("RadDatePicker_Time") as RadDatePicker;
                var radComboBoxFiliale = editedItem.FindControl("RCB_Filiale") as RadComboBox;
                var invoiceRollInfo = new Keede.Ecsoft.Model.InvoiceRoll
                {
                    Id = new Guid(editedItem.GetDataKeyValue("Id").ToString()),
                    Receiptor = textBoxReceiptor != null ? textBoxReceiptor.Text : string.Empty,
                    InvoiceCode = textBoxInvoiceCode != null ? textBoxInvoiceCode.Text : string.Empty,
                    InvoiceStartNo = textBoxInvoiceStartNo == null ? 0 : int.Parse(string.IsNullOrEmpty(textBoxInvoiceStartNo.Text.Trim()) ? "0" : textBoxInvoiceStartNo.Text.Trim()),
                    InvoiceEndNo = textBoxInvoiceEndNo == null ? 0 : int.Parse(string.IsNullOrEmpty(textBoxInvoiceEndNo.Text.Trim()) ? "0" : textBoxInvoiceEndNo.Text.Trim()),
                    InvoiceRollCount = textBoxInvoiceRollCount == null ? 0 : int.Parse(string.IsNullOrEmpty(textBoxInvoiceRollCount.Text.Trim()) ? "0" : textBoxInvoiceRollCount.Text.Trim()),
                    CreateTime = radDatePickerTime != null ? radDatePickerTime.SelectedDate ?? DateTime.Now : DateTime.Now,
                    FilialeId = radComboBoxFiliale != null ? new Guid(radComboBoxFiliale.SelectedValue) : Guid.Empty
                };
                invoiceRollInfo.InvoiceCount = invoiceRollInfo.InvoiceEndNo - invoiceRollInfo.InvoiceStartNo + 1;
                if (invoiceRollInfo.InvoiceCount < 1)
                {
                    RAM.Alert("发票份数是：" + invoiceRollInfo.InvoiceCount + "份，填写发票号码错误");
                    return;
                }
                var success = Invoice.WriteInstance.UpdateInvoiceRoll(invoiceRollInfo);
                if (success)
                {
                    var personnelInfo = CurrentSession.Personnel.Get();
                    //发票库管理编辑操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceRollInfo.Id, invoiceRollInfo.InvoiceCode, OperationPoint.InvoiceStorageManage.Edit.GetBusinessInfo(), string.Empty);
                    ReadInvoiceRollList();
                }
                else
                {
                    RAM.Alert("编辑发票失败，其中的发票卷可能分发在打印了");
                }
            }
        }

        /// <summary>显示当前登录人姓名
        /// </summary>
        protected string GetReceiptorName(string receiptor)
        {
            if (string.IsNullOrEmpty(receiptor))
            {
                return CurrentSession.Personnel.Get().RealName;
            }
            return receiptor;
        }

        /// <summary>显示公司列表
        /// </summary>
        /// <returns></returns>
        public List<FilialeInfo> LoadFiliale()
        {
            return CacheCollection.Filiale.GetHeadList().ToList();
        }

        /// <summary>显示公司名称
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public string GetFilialeName(object filialeId)
        {
            try
            {
                if (filialeId == null) return string.Empty;
                var filialeInfo = CacheCollection.Filiale.Get(new Guid(filialeId.ToString()));
                return filialeInfo != null ? filialeInfo.Name : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>编辑选择具体公司
        /// </summary>
        protected void RadGrid_InvoiceRollList_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType != GridItemType.EditFormItem) return;
            var editableItem = (GridEditableItem)e.Item;
            var rcbFiliale = (RadComboBox)editableItem.FindControl("RCB_Filiale");
            var hfFilialeId = (HiddenField)editableItem.FindControl("hfFilialeId");
            if (hfFilialeId != null && rcbFiliale != null)
            {
                rcbFiliale.SelectedValue = hfFilialeId.Value;
            }
        }
    }
}
