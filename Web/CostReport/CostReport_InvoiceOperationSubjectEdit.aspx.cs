using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.UI.Web.Common;
using ERP.UI.Web.Base;
using System.Text;
using Telerik.Web.UI;
using ERP.Model.Goods;
using ERP.SAL.Interface;
using ERP.SAL;
using System.Web.UI;

namespace ERP.UI.Web.CostReport
{
    /// <summary>
    /// 费用发票——科目归类
    /// </summary>
    public partial class CostReport_InvoiceOperationSubjectEdit : WindowsPage
    {
        //TODO:待定
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);

        private static readonly ISubjectSao _subjectClassSao = new SubjectSao();

        #region 属性

        /// <summary>
        /// 票据信息模型
        /// </summary>
        private CostReportBillInfo CostReportBillInfo
        {
            get
            {
                if (ViewState["CostReportBillInfo"] == null)
                {
                    return null;
                }
                return (CostReportBillInfo)ViewState["CostReportBillInfo"];
            }
            set { ViewState["CostReportBillInfo"] = value; }
        }

        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var billId = Request.QueryString["BillId"];
                if (!string.IsNullOrEmpty(billId))
                {
                    LoadBillData(billId);//初始化页面数据
                }
            }
        }

        //初始化页面数据
        protected void LoadBillData(string billId)
        {
            CostReportBillInfo model = CostReportBillInfo = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
        }

        //保存数据
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            #region 验证数据

            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }

            #endregion 验证数据

            var result = _costReportBill.Updatelmshop_CostReportBillByBillId(CostReportBillInfo);
            if (result)
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据修改】");
                _costReportBill.Updatelmshop_CostReportBillRemarkByBillId(remark, CostReportBillInfo.BillId);
                MessageBox.AppendScript(this, "CloseAndRebind();");
            }
            else
            {
                MessageBox.Show(this, "保存失败！");
            }
        }

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();

            return errorMsg.ToString();
        }

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGridSubjectClass();
        }

        /// <summary>
        /// 绑定表格数据
        /// </summary>
        private void BindGridSubjectClass()
        {
            List<SubjectInfo> list = new List<SubjectInfo>();
            list.Add(new SubjectInfo()
            {
                SubjectID = "2b8300cc-122d-460d-9f87-75115a073622",
                SubjectName = "二级",
                Amount = 2.22m
            });
            list.Add(new SubjectInfo()
            {
                SubjectID = "2",
                SubjectName = "一级科目",
                Amount = 3m
            });
            RadGrid1.DataSource = list;
        }

        /// <summary>
        /// 获取QueryString的BillId
        /// </summary>
        /// <returns></returns>
        private Guid GetBillID()
        {
            Guid g = Guid.Empty;
            var billId = Request.QueryString["BillId"];
            if (!string.IsNullOrEmpty(billId))
            {
                Guid.TryParse(billId, out g);
            }
            return g;
        }

        protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode && (!e.Item.OwnerTableView.IsItemInserted))
            {
                RadComboBox radComboBox = (e.Item as GridEditableItem).FindControl("RadComboBox1") as RadComboBox;
                radComboBox.Items[0].Text = DataBinder.Eval(e.Item.DataItem, "SubjectName").ToString();
            }
        }

        protected void RadGrid1_ItemCommand(object source, GridCommandEventArgs e)
        {
            hidden_EditIndex.Text = e.Item.ItemIndex.ToString();
            if (e.CommandName == RadGrid.CancelCommandName)
            {
                hidden_EditIndex.Text = "";
            }
            if (e.CommandName == RadGrid.EditCommandName)
            {
                string tempID = e.Item.OriginalClientID;
                hidden_EditComboBoxID.Text = tempID + "_RadComboBox1";
            }

            if (e.CommandName == RadGrid.InitInsertCommandName)
            {
                e.Canceled = true;

                //添加 设置默认值
                System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
                newValues["Amount"] = "2.22";

                //Insert the item and rebind
                e.Item.OwnerTableView.InsertItem(newValues);

                GridEditableItem insertedItem = e.Item.OwnerTableView.GetInsertItem();
                RadComboBox Txt_SubjectName = insertedItem.FindControl("RadComboBox1") as RadComboBox;
                hidden_EditComboBoxID.Text = Txt_SubjectName.ClientID;
            }
            //if (e.CommandName == RadGrid.InitInsertCommandName)
            //{
            //    e.Canceled = true;
            //    e.Item.OwnerTableView.InsertItem();

            //    GridEditableItem insertedItem = e.Item.OwnerTableView.GetInsertItem();
            //    TextBox Txt_SubjectName = insertedItem.FindControl("Txt_SubjectName") as TextBox;
            //    Txt_SubjectName.Text = hidden_SelectedSubjectName.Text;
            //}
        }

        // 增加
        protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                string Txt_SubjectID = ((RadComboBox)editedItem.FindControl("RadComboBox1")).SelectedValue;
                string Txt_SubjectName = ((RadComboBox)editedItem.FindControl("RadComboBox1")).Text;
                string Txt_Amount = ((TextBox)editedItem.FindControl("Txt_Amount")).Text;

                decimal amount = 0;
                decimal.TryParse(Txt_Amount, out amount);

                if (string.IsNullOrEmpty(Txt_SubjectID))
                {
                    RAM.Alert("请选择科目分类！");
                    return;
                }
                var model = new SubjectInfo
                {
                    BillId = GetBillID(),
                    SubjectID = Txt_SubjectID,
                    SubjectName = Txt_SubjectName,
                    Amount = amount,
                };
                try
                {
                    string errorMessage;
                    var result = _subjectClassSao.Add(model, out errorMessage);
                    if (!result)
                    {
                        RAM.Alert("添加失败！" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("添加失败！");
                }
            }
            RadGrid1.Rebind();
        }

        // 删除
        protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                int SubjectID = Convert.ToInt32(editedItem.GetDataKeyValue("SubjectID").ToString());
                try
                {
                    string errorMessage;
                    var result = _subjectClassSao.Delete(SubjectID, out errorMessage);
                    if (!result)
                    {
                        RAM.Alert("删除失败！" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("删除失败！");
                }
            }
            RadGrid1.Rebind();
        }

        // 修改
        protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                string Txt_SubjectID = ((RadComboBox)editedItem.FindControl("RadComboBox1")).SelectedValue;
                string Txt_SubjectName = ((RadComboBox)editedItem.FindControl("RadComboBox1")).Text;
                string Txt_Amount = ((TextBox)editedItem.FindControl("Txt_Amount")).Text;

                decimal amount = 0;
                decimal.TryParse(Txt_Amount, out amount);

                var model = new SubjectInfo
                {
                    BillId = GetBillID(),
                    SubjectID = Txt_SubjectID,
                    SubjectName = Txt_SubjectName,
                    Amount = amount,
                };
                try
                {
                    string errorMessage;
                    var result = _subjectClassSao.Update(model, out errorMessage);

                    if (result)
                    {
                    }
                    else
                    {
                        RAM.Alert("更新失败！" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("更新失败！");
                }
            }
            RadGrid1.Rebind();
        }

        #region 科目分类

        protected void RadTreeView1_DataBound(object sender, EventArgs e)
        {
            RadTreeNode rootNode = CreateNode("科目分类", true, Guid.Empty.ToString());
            rootNode.Category = "SubjectClass";
            rootNode.Selected = true;

            ((RadTreeView)sender).Nodes.Add(rootNode);

            IList<SubjectClassInfo> listModel = _subjectClassSao.GetAllSubjectClassList().OrderBy(act => act.OrderIndex).ToList();
            RecursivelySubjectClass(Guid.Empty, rootNode, listModel, sender);
        }

        /// <summary>
        /// 遍历科目分类
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="node"></param>
        /// <param name="listModel"></param>
        private void RecursivelySubjectClass(Guid classId, IRadTreeNodeContainer node, IList<SubjectClassInfo> listModel, object sender)
        {
            IList<SubjectClassInfo> listChild = listModel.Where(w => w.ParentClassId == classId).ToList();
            foreach (SubjectClassInfo item in listChild)
            {
                RadTreeNode treeNode = CreateNode(item.ClassName, false, item.ClassId.ToString());
                if (node == null)
                {
                    ((RadTreeView)sender).Nodes.Add(treeNode);
                }
                else
                {
                    node.Nodes.Add(treeNode);
                }
                RecursivelySubjectClass(item.ClassId, treeNode, listModel, sender);
            }
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        #endregion 科目分类
    }
}