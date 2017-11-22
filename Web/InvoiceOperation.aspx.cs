using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IUtilities;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2016/02/21  
     * 描述    :发票操作
     * =====================================================================
     * 修改时间：2016/02/21  
     * 修改人  ：  
     * 描述    ：
     */

    public partial class InvoiceOperation : BasePage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGetCompanyListData();
                LoadSaleFilialeData();
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_ReceiptInvoice.CurrentPageIndex = 0;
            RG_ReceiptInvoice.DataBind();
        }

        #region 数据准备

        #region[获取往来单位数据信息，包含供应商和物流公司]

        /// <summary>
        ///  获取往来单位数据信息，包含供应商和物流公司
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> CompanyCussentList()
        {
            CompanyType[] companyType = { CompanyType.Suppliers, CompanyType.Express, CompanyType.Vendors };
            var data = (List<CompanyCussentInfo>)_companyCussent.GetCompanyCussentList(companyType, State.Enable);
            return data;
        }

        /// <summary>
        /// 加载往来单位数据信息
        /// </summary>
        protected void LoadGetCompanyListData()
        {
            rcb_CompanyList.DataSource = CompanyCussentList();
            rcb_CompanyList.DataTextField = "CompanyName";
            rcb_CompanyList.DataValueField = "CompanyId";
            rcb_CompanyList.DataBind();
            rcb_CompanyList.Items.Insert(0, new RadComboBoxItem("全部", string.Empty));
        }

        protected void rcb_CompanyList_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                var companyList =
                    (IList<CompanyCussentInfo>)
                        CompanyCussentList()
                            .Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1)
                            .ToList();
                if (e.NumberOfItems >= companyList.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (CompanyCussentInfo i in companyList)
                    {
                        var item = new RadComboBoxItem { Text = i.CompanyName, Value = i.CompanyId.ToString() };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        #endregion

        //公司
        protected void LoadSaleFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo
            {
                ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")),
                Name = "ERP公司"
            });
            ddl_SaleFiliale.DataSource = list;
            ddl_SaleFiliale.DataTextField = "Name";
            ddl_SaleFiliale.DataValueField = "ID";
            ddl_SaleFiliale.DataBind();
            ddl_SaleFiliale.Items.Insert(0, new ListItem("全部", ""));
        }

        #endregion

        #region 数据列表相关

        protected void RG_ReceiptInvoice_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            #region 查询条件
            Guid filialeId = Guid.Empty;
            Guid companyId = Guid.Empty;
            DateTime? startApplyDateTime = null;
            DateTime? endApplyDateTime = null;
            string receiptStatus = string.Empty;
            string receiptNo = string.Empty;
            string invoiceNo = string.Empty;
            int? invoiceState = null;
            int? invoiceType = null;
            DateTime? startOperatingTime = null;
            DateTime? endOperatingTime = null;
            string billingUnit = string.Empty;
            int pageIndex = RG_ReceiptInvoice.CurrentPageIndex + 1;
            int pageSize = RG_ReceiptInvoice.PageSize;
            if (!string.IsNullOrEmpty(ddl_SaleFiliale.SelectedValue))
            {
                filialeId = new Guid(ddl_SaleFiliale.SelectedValue);
            }
            if (!string.IsNullOrEmpty(rcb_CompanyList.SelectedValue))
            {
                companyId = new Guid(rcb_CompanyList.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_ApplyDateTimeStart.Text))
            {
                startApplyDateTime = DateTime.Parse(txt_ApplyDateTimeStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_ApplyDateTimeEnd.Text))
            {
                endApplyDateTime = DateTime.Parse(txt_ApplyDateTimeEnd.Text).AddDays(1);
            }
            if (!string.IsNullOrEmpty(ddl_PayState.SelectedValue))
            {
                receiptStatus = ddl_PayState.SelectedValue;
            }
            if (!string.IsNullOrEmpty(txt_ReceipNo.Text))
            {
                receiptNo = txt_ReceipNo.Text;
            }
            if (!string.IsNullOrEmpty(txt_InvoiceNo.Text))
            {
                invoiceNo = txt_InvoiceNo.Text;
            }
            if (!string.IsNullOrEmpty(ddl_InvoiceState.SelectedValue))
            {
                invoiceState = int.Parse(ddl_InvoiceState.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_OperatingTimeStart.Text))
            {
                startOperatingTime = DateTime.Parse(txt_OperatingTimeStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_OperatingTimeEnd.Text))
            {
                endOperatingTime = DateTime.Parse(txt_OperatingTimeEnd.Text).AddDays(1);
            }
            if (!string.IsNullOrEmpty(txt_BillingUnit.Text))
            {
                billingUnit = txt_BillingUnit.Text;
            }
            if (!string.IsNullOrEmpty(ddl_InvoiceType.SelectedValue))
            {
                invoiceType = int.Parse(ddl_InvoiceType.SelectedValue);
            }
            #endregion

            int total;
            var data = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByPage(filialeId, companyId,
                startApplyDateTime, endApplyDateTime, receiptStatus, receiptNo, invoiceNo, invoiceState,
                startOperatingTime, endOperatingTime, billingUnit, invoiceType, pageIndex, pageSize, out total);

            #region 合计
            DataTable result = GetDataTable(data);
            var sumData = result.AsEnumerable();

            var noTaxAmount = RG_ReceiptInvoice.MasterTableView.Columns.FindByUniqueName("NoTaxAmount");
            var tax = RG_ReceiptInvoice.MasterTableView.Columns.FindByUniqueName("Tax");
            var taxAmount = RG_ReceiptInvoice.MasterTableView.Columns.FindByUniqueName("TaxAmount");
            var invoiceNum = RG_ReceiptInvoice.MasterTableView.Columns.FindByUniqueName("InvoiceNum");
            if (sumData.Any())
            {
                var sumNoTaxAmount = sumData.Sum(r => r.Field<decimal>("NoTaxAmount"));
                var sumTax = sumData.Sum(r => r.Field<decimal>("Tax"));
                var sumTaxAmount = sumData.Sum(r => r.Field<decimal>("TaxAmount"));
                if (!string.IsNullOrEmpty(ddl_InvoiceType.SelectedValue))
                {
                    if (int.Parse(ddl_InvoiceType.SelectedValue) == 5)
                    {
                        noTaxAmount.FooterText = string.Format("未税金额：{0}", WebControl.NumberSeparator(sumNoTaxAmount));
                        tax.FooterText = string.Format("税额：{0}", WebControl.NumberSeparator(sumTax));
                    }
                    else
                    {
                        noTaxAmount.FooterText = String.Empty;
                        tax.FooterText = String.Empty;
                    }
                }
                else
                {
                    noTaxAmount.FooterText = string.Format("未税金额：{0}", WebControl.NumberSeparator(sumNoTaxAmount));
                    tax.FooterText = string.Format("税额：{0}", WebControl.NumberSeparator(sumTax));
                }
                taxAmount.FooterText = string.Format("含税金额：{0}", WebControl.NumberSeparator(sumTaxAmount));
                invoiceNum.FooterText = "发票数量：" + sumData.Count();//sumData.Select(r => r.Field<int>("InvoiceTotal")).FirstOrDefault();
            }
            else
            {
                noTaxAmount.FooterText = string.Empty;
                tax.FooterText = string.Empty;
                taxAmount.FooterText = string.Empty;
            }

            #endregion

            RG_ReceiptInvoice.DataSource = data;
            RG_ReceiptInvoice.VirtualItemCount = total;
        }

        private DataTable GetDataTable(DataTable data)
        {
            var resultData = data.Clone();
            var selectData = data.AsEnumerable();
            foreach (DataRow dr in data.Rows)
            {
                if (resultData.Select("InvoiceNo='" + dr["InvoiceNo"].ToString() + "' and InvoiceCode='" + dr["InvoiceCode"].ToString() + "'").Count() == 0)
                {
                    if (selectData.Count(ent => ent.Field<string>("InvoiceNo") == dr["InvoiceNo"].ToString() && ent.Field<string>("InvoiceCode") == dr["InvoiceCode"].ToString()) > 1)
                    {
                        var first = selectData.Where(ent => ent.Field<string>("InvoiceNo") == dr["InvoiceNo"].ToString() && ent.Field<string>("InvoiceCode") == dr["InvoiceCode"].ToString())
                            .OrderBy(ent => ent.Field<DateTime>("BillingDate")).ThenBy(ent => ent.Field<DateTime>("OperatingTime"))
                            .First();
                        resultData.Rows.Add(first.ItemArray);
                    }
                    else
                    {
                        resultData.Rows.Add(dr.ItemArray);
                    }
                }
            }
            return resultData;
        }

        //行绑定事件
        protected void RG_ReceiptInvoice_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                #region 含税金额小于0，标红该行

                var taxAmount = DataBinder.Eval(e.Item.DataItem, "TaxAmount");
                if (taxAmount != null && decimal.Parse(taxAmount.ToString()) <= 0)
                {
                    e.Item.Style["color"] = "red";
                    e.Item.ToolTip = "“含税金额”小于等于0！";
                }

                #endregion
            }
        }

        #region 列表显示辅助方法

        /// <summary>
        /// 根据公司id获取公司名称
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        protected string GetFilialeName(string filialeId)
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo
            {
                ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")),
                Name = "ERP公司"
            });
            var firstOrDefault = list.FirstOrDefault(p => p.ID.Equals(new Guid(filialeId)));
            if (firstOrDefault != null)
            {
                return firstOrDefault.Name;
            }
            return "-";
        }

        #endregion

        #endregion

        #region 事件操作

        //发票删除
        protected void imgbtn_Del_Click(object sender, EventArgs e)
        {
            var invoiceId = ((ImageButton)sender).CommandArgument;
            CompanyFundReceiptInvoiceInfo model =
                _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
            if (model.InvoiceState.Equals((int)CompanyFundReceiptInvoiceState.UnSubmit))
            {
                var result =
                    _companyFundReceiptInvoice.Deletelmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
                if (result)
                {
                    MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //发票提交
        protected void imgbtn_Submit_Click(object sender, EventArgs e)
        {
            var invoiceId = ((ImageButton)sender).CommandArgument;
            CompanyFundReceiptInvoiceInfo model =
                _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
            if (model.InvoiceState.Equals((int)CompanyFundReceiptInvoiceState.UnSubmit))
            {
                string remark = WebControl.RetrunUserAndTime("【提交发票】");
                _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark, new Guid(invoiceId),
                    (int)CompanyFundReceiptInvoiceState.Submit);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //发票接收
        protected void imgbtn_Receive_Click(object sender, EventArgs e)
        {
            var invoice = ((ImageButton)sender).CommandArgument;
            var invoiceInfo = invoice.Split(',');
            string invoiceId = invoiceInfo[0];
            string invoiceType = invoiceInfo[1];
            CompanyFundReceiptInvoiceInfo model =
                _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
            if (model.InvoiceState.Equals((int)CompanyFundReceiptInvoiceState.Submit))
            {
                string remark = WebControl.RetrunUserAndTime("【接收发票】");
                if (invoiceType.Equals("1"))
                    _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark, new Guid(invoiceId),
                        (int)CompanyFundReceiptInvoiceState.Verification);
                else
                    _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark, new Guid(invoiceId),
                            (int)CompanyFundReceiptInvoiceState.Receive);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //发票待认证
        protected void imgbtn_Authenticate_Click(object sender, EventArgs e)
        {
            var invoiceId = ((ImageButton)sender).CommandArgument;
            CompanyFundReceiptInvoiceInfo model =
                _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
            if (model.InvoiceState.Equals((int)CompanyFundReceiptInvoiceState.Receive))
            {
                string remark = WebControl.RetrunUserAndTime("【发票待认证】");
                _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark, new Guid(invoiceId),
                    (int)CompanyFundReceiptInvoiceState.Authenticate);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //发票认证完成
        protected void imgbtn_Verification_Click(object sender, EventArgs e)
        {
            var invoiceId = ((ImageButton)sender).CommandArgument;
            CompanyFundReceiptInvoiceInfo model =
                _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(invoiceId));
            if (model.InvoiceState.Equals((int)CompanyFundReceiptInvoiceState.Authenticate))
            {
                string remark = WebControl.RetrunUserAndTime("【发票认证完成】");
                _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark, new Guid(invoiceId),
                    (int)CompanyFundReceiptInvoiceState.Verification);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //发票批量提交
        protected void btn_BatchSubmit_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var errorMsg = new StringBuilder();
                var invoiceIds = Request["ckId"].Split(',');
                foreach (var item in invoiceIds)
                {
                    CompanyFundReceiptInvoiceInfo model =
                        _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(item));
                    if (model.InvoiceState != (int)CompanyFundReceiptInvoiceState.UnSubmit)
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = WebControl.RetrunUserAndTime("【提交发票】");
                        _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark,
                            new Guid(item), (int)CompanyFundReceiptInvoiceState.Submit);
                    }
                    catch
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”保存失败！").Append("\\n");
                    }

                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //发票批量接收
        protected void btn_BatchReceive_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var errorMsg = new StringBuilder();
                var invoiceIds = Request["ckId"].Split(',');
                foreach (var item in invoiceIds)
                {
                    CompanyFundReceiptInvoiceInfo model =
                        _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(item));
                    if (model.InvoiceState != (int)CompanyFundReceiptInvoiceState.Submit)
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = WebControl.RetrunUserAndTime("【接收发票】");
                        _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark,
                            new Guid(item), (int)CompanyFundReceiptInvoiceState.Receive);
                    }
                    catch
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”保存失败！").Append("\\n");
                    }

                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //发票批量待认证
        protected void btn_BatchAuthenticate_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var errorMsg = new StringBuilder();
                var invoiceIds = Request["ckId"].Split(',');
                foreach (var item in invoiceIds)
                {
                    CompanyFundReceiptInvoiceInfo model =
                        _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(item));
                    if (model.InvoiceState != (int)CompanyFundReceiptInvoiceState.Receive)
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = WebControl.RetrunUserAndTime("【发票待认证】");
                        _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark,
                            new Guid(item), (int)CompanyFundReceiptInvoiceState.Authenticate);
                    }
                    catch
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”保存失败！").Append("\\n");
                    }

                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //发票批量认证完成
        protected void btn_BatchVerification_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var errorMsg = new StringBuilder();
                var invoiceIds = Request["ckId"].Split(',');
                foreach (var item in invoiceIds)
                {
                    CompanyFundReceiptInvoiceInfo model =
                        _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(new Guid(item));
                    if (model.InvoiceState != (int)CompanyFundReceiptInvoiceState.Authenticate)
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”状态已更新，不允许此操作！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = WebControl.RetrunUserAndTime("【发票认证完成】");
                        _companyFundReceiptInvoice.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(remark,
                            new Guid(item), (int)CompanyFundReceiptInvoiceState.Verification);
                    }
                    catch
                    {
                        errorMsg.Append("发票号码“").Append(model.InvoiceNo).Append("”保存失败！").Append("\\n");
                    }

                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        #endregion

        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            RG_ReceiptInvoice.Columns[0].Visible = false;
            RG_ReceiptInvoice.Columns[15].Visible = false;
            RG_ReceiptInvoice.Columns[16].Visible = false;
            RG_ReceiptInvoice.Columns[17].Visible = false;
            RG_ReceiptInvoice.Columns[18].Visible = false;
            RG_ReceiptInvoice.Columns[19].Visible = false;
            RG_ReceiptInvoice.Columns[20].Visible = false;
            RG_ReceiptInvoice.ExportSettings.ExportOnlyData = true;
            RG_ReceiptInvoice.ExportSettings.IgnorePaging = true;
            RG_ReceiptInvoice.ExportSettings.FileName = Server.UrlEncode("发票操作信息");
            RG_ReceiptInvoice.MasterTableView.ExportToExcel();
        }


        public String GetInvoiceTypeName(String invoiceType)
        {
            if (!String.IsNullOrWhiteSpace(invoiceType))
            {
                if (invoiceType.Equals("1"))
                    return "普通发票";
                else if (invoiceType.Equals("5"))
                    return "增值发票";

            }
            return String.Empty;
        }
    }
}