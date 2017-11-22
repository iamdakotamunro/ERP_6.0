using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Enum.Attribute;
using Telerik.Web.UI;
using System.Configuration;
using ERP.UI.Web.Base;
using System.Collections;
using System.Text;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_InvoiceOperationManage : BasePage
    {
        private static readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);

        #region 属性
        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }

        /// <summary>
        /// O2O事业部
        /// </summary>
        private string ShopBranchId
        {
            get
            {
                if (ViewState["ShopBranchId"] == null)
                {
                    ViewState["ShopBranchId"] = GlobalConfig.ShopBranchId;
                }
                return ViewState["ShopBranchId"].ToString();
            }
        }

        /// <summary>
        /// 发票抬头列表属性
        /// </summary>
        protected IList<FilialeInfo> FilialeInfoList
        {
            get
            {
                if (ViewState["FilialeInfoList"] == null)
                    return CacheCollection.Filiale.GetHeadList();
                return ViewState["FilialeInfoList"] as List<FilialeInfo>;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInvoiceTitleData();
                LoadBillStateData();
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (!Hid_IsFirstPage.Value.Equals("0"))
            {
                RG_Invoice.CurrentPageIndex = 0;
            }
            GridDataBind();
            RG_Invoice.DataBind();
        }

        #region 数据准备
        //申请人数据绑定
        protected void rcb_ReportPersonnel_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var personnelList = _personnelSao.GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in personnelList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.RealName, item.PersonnelId.ToString()));
                    }
                }
            }
        }

        //发票抬头
        protected void LoadInvoiceTitleData()
        {
            ddl_InvoiceTitle.DataSource = FilialeInfoList;
            ddl_InvoiceTitle.DataTextField = "RealName";
            ddl_InvoiceTitle.DataValueField = "ID";
            ddl_InvoiceTitle.DataBind();
            ddl_InvoiceTitle.Items.Insert(0, new ListItem("请选择", ""));
        }

        //票据状态
        protected void LoadBillStateData()
        {
            var list = EnumAttribute.GetDict<CostReportBillState>();
            ddl_BillState.DataSource = list;
            ddl_BillState.DataTextField = "Value";
            ddl_BillState.DataValueField = "Key";
            ddl_BillState.DataBind();
            ddl_BillState.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 数据列表相关
        protected void RG_Invoice_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            #region 查询条件
            Guid reportPersonnel = Guid.Empty;
            Guid invoiceTitleFilialeId = Guid.Empty;
            DateTime? reportDateStart = null;
            DateTime? reportDateEnd = null;
            string reportNo = string.Empty;
            string billNo = string.Empty;
            int? billState = null;
            DateTime? operatingTimestart = null;
            DateTime? operatingTimeend = null;
            int invoiceType = 0;
            int pageIndex = RG_Invoice.CurrentPageIndex + 1;
            int pageSize = RG_Invoice.PageSize;

            if (!string.IsNullOrEmpty(rcb_ReportPersonnel.SelectedValue))
            {
                reportPersonnel = new Guid(rcb_ReportPersonnel.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddl_InvoiceTitle.SelectedValue))
            {
                invoiceTitleFilialeId = new Guid(ddl_InvoiceTitle.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_ReportDateStart.Text))
            {
                reportDateStart = DateTime.Parse(txt_ReportDateStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_ReportDateEnd.Text))
            {
                reportDateEnd = DateTime.Parse(txt_ReportDateEnd.Text).AddDays(1);
            }
            if (!string.IsNullOrEmpty(txt_ReportNo.Text))
            {
                reportNo = txt_ReportNo.Text;
            }
            if (!string.IsNullOrEmpty(txt_BillNo.Text))
            {
                billNo = txt_BillNo.Text;
            }
            if (!string.IsNullOrEmpty(ddl_BillState.SelectedValue))
            {
                billState = int.Parse(ddl_BillState.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_OperatingTimeStart.Text))
            {
                operatingTimestart = DateTime.Parse(txt_OperatingTimeStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_OperatingTimeEnd.Text))
            {
                operatingTimeend = DateTime.Parse(txt_OperatingTimeEnd.Text).AddDays(1);
            }
            if (!string.IsNullOrEmpty(ddl_InvoiceType.SelectedValue))
            {
                invoiceType = int.Parse(ddl_InvoiceType.SelectedValue);
            }
            #endregion

            DataTable sumTable;
            int total;
            var data = _costReportBill.Getlmshop_CostReportBillByPage(reportPersonnel, invoiceTitleFilialeId, reportDateStart, reportDateEnd, string.Empty, reportNo, billNo, billState, operatingTimestart, operatingTimeend, invoiceType, pageIndex, pageSize, out sumTable, out total);

            #region 合计
            var sumData = sumTable.AsEnumerable();
            if (sumData.Any())
            {
                var sumActualAmountList = sumData.Select(r => r.Field<decimal>("ActualAmount")).ToList();
                var sumBillTotalList = sumData.Select(r => r.Field<int>("BillTotal")).ToList();
                var sumNoTaxAmountList = sumData.Select(r => r.Field<decimal>("NoTaxAmount")).ToList();
                var sumTaxList = sumData.Select(r => r.Field<decimal>("Tax")).ToList();
                var sumTaxAmountList = sumData.Select(r => r.Field<decimal>("TaxAmount")).ToList();
                var invoiceTypeNameList = sumData.Select(r => r.Field<string>("InvoiceTypeName")).ToList();

                StringBuilder htmlFooter = new StringBuilder();
                if (invoiceTypeNameList.Count == 1)
                {
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>票据类型</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>实际金额(元)</td><td>&nbsp;</td><td>票据总数(张)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>未税金额(元)</td><td>税额(元)</td><td>含税金额(元)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgAltRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[0]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[0]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>").Append("合计：").Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                }
                else if (invoiceTypeNameList.Count == 2)
                {
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>票据类型</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>实际金额(元)</td><td>&nbsp;</td><td>票据总数(张)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>未税金额(元)</td><td>税额(元)</td><td>含税金额(元)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgAltRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[0]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[0]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[1]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[1]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[1]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[1]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[1]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[1]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgAltRow'><td>&nbsp;</td><td align='right'>").Append("合计：").Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                }
                else if (invoiceTypeNameList.Count == 3)
                {
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>票据类型</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>实际金额(元)</td><td>&nbsp;</td><td>票据总数(张)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>未税金额(元)</td><td>税额(元)</td><td>含税金额(元)</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgAltRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[0]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[0]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[0]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[0]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[1]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[1]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[1]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[1]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[1]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[1]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgAltRow'><td>&nbsp;</td><td align='right'>").Append(invoiceTypeNameList[2]).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList[2]))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList[2]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList[2]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList[2]))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList[2]))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                    htmlFooter.Append("<tr class='rgRow'><td>&nbsp;</td><td align='right'>").Append("合计：").Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumActualAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumBillTotalList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumNoTaxAmountList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxList.Sum()))).Append("</td><td>").Append(string.Format("{0}", ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(sumTaxAmountList.Sum()))).Append("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
                }

                lit_FooterTable.Text = string.IsNullOrEmpty(htmlFooter.ToString()) ? string.Empty : "<table id='FooterTable' style='display: none;'>" + htmlFooter + "</table>";
            }
            else
            {
                lit_FooterTable.Text = string.Empty;
            }
            #endregion

            RG_Invoice.VirtualItemCount = total;
            RG_Invoice.DataSource = data;
        }

        //行绑定事件
        private int _rowspan = 1;
        private string _reportNo = string.Empty;
        private string _cssClass = string.Empty;
        protected void RG_Invoice_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                #region 含税金额小于0(红色)
                var taxAmount = DataBinder.Eval(e.Item.DataItem, "TaxAmount");
                if (taxAmount != null && decimal.Parse(taxAmount.ToString()) <= 0)
                {
                    e.Item.Style["color"] = "red";
                    e.Item.ToolTip = "“含税金额”小于等于0！";
                }
                #endregion

                #region “申报金额”>=1万元(绿色);“申报金额”>=10万元(黄色);“申报金额”>=100万元(紫色)
                var reportCost = DataBinder.Eval(e.Item.DataItem, "ReportCost");
                if (decimal.Parse(reportCost.ToString()) >= 1000000)
                {
                    e.Item.Style["color"] = "#CC00FF";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 100000)
                {
                    e.Item.Style["color"] = "#FF9900";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                else if (decimal.Parse(reportCost.ToString()) >= 10000)
                {
                    e.Item.Style["color"] = "#009900";
                    e.Item.Cells[2].Style["color"] = "black";
                }
                #endregion

                #region 票据金额和实际金额不一致(蓝色)
                var actualAmount = DataBinder.Eval(e.Item.DataItem, "ActualAmount");//申请金额的总和(此金额不包含系统自动生成的金额)
                var applyForCost = DataBinder.Eval(e.Item.DataItem, "ApplyForCost");//票据金额的总和
                if (decimal.Parse(actualAmount.ToString()) != decimal.Parse(applyForCost.ToString()))
                {
                    e.Item.Style["color"] = "blue";
                    e.Item.ToolTip = "票据金额和实际金额不一致！";
                }
                #endregion

                #region 合并单元格(一条费用申报有多条票据信息)
                if (_reportNo.Equals(DataBinder.Eval(e.Item.DataItem, "ReportNo")))
                {
                    _rowspan++;
                    e.Item.Cells[3].Visible = false;
                    e.Item.Cells[4].Visible = false;
                    e.Item.Cells[5].Visible = false;
                    e.Item.Cells[6].Visible = false;
                    e.Item.Cells[7].Visible = false;
                    e.Item.Cells[8].Visible = false;
                    //e.Item.Cells[9].Visible = false;
                    //e.Item.Cells[25].Visible = false;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[3].RowSpan = _rowspan;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[4].RowSpan = _rowspan;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[5].RowSpan = _rowspan;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[6].RowSpan = _rowspan;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[7].RowSpan = _rowspan;
                    RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[8].RowSpan = _rowspan;
                    //RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[9].RowSpan = _rowspan;
                    //RG_Invoice.Items[e.Item.ItemIndex - (_rowspan - 1)].Cells[25].RowSpan = _rowspan;
                }
                else
                {
                    _rowspan = 1;
                    _reportNo = string.Empty;
                    if (e.Item.ItemIndex % 2 == 0)
                    {
                        _cssClass = _cssClass == "rgRow" ? "rgAltRow" : "rgRow";
                    }
                    else
                    {
                        _cssClass = _cssClass == "rgAltRow" ? "rgRow" : "rgAltRow";
                    }
                    _reportNo = DataBinder.Eval(e.Item.DataItem, "ReportNo").ToString();
                }
                e.Item.CssClass = _cssClass;
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
            FilialeInfoList.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            var firstOrDefault = FilialeInfoList.FirstOrDefault(p => p.ID.Equals(new Guid(filialeId)));
            if (firstOrDefault != null)
            {
                return firstOrDefault.RealName;
            }
            return "-";
        }
        #endregion
        #endregion

        #region 事件操作
        //票据删除
        protected void imgbtn_Del_Click(object sender, EventArgs e)
        {
            var billId = ((ImageButton)sender).CommandArgument;
            CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
            if (model.BillState.Equals((int)CostReportBillState.UnSubmit))
            {
                var result = _costReportBill.Deletelmshop_CostReportBillByBillId(new Guid(billId));
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
        //票据提交
        protected void imgbtn_Submit_Click(object sender, EventArgs e)
        {
            var billId = ((ImageButton)sender).CommandArgument;
            CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
            if (model.BillState.Equals((int)CostReportBillState.UnSubmit))
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【提交票据】");
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(billId), (int)CostReportBillState.Submit);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }
        //票据接收
        protected void imgbtn_Receive_Click(object sender, EventArgs e)
        {
            var billId = ((ImageButton)sender).CommandArgument;
            var invoiceType = ((ImageButton)sender).CommandName;
            CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
            if (model.BillState.Equals((int)CostReportBillState.Submit))
            {
                int state;
                string remarkMsg = string.Empty;
                if (int.Parse(invoiceType).Equals((int)CostReportInvoiceType.VatInvoice))
                {
                    state = (int)CostReportBillState.Receive;
                    remarkMsg = "【接收票据】";
                }
                else
                {
                    state = (int)CostReportBillState.Verification;
                    remarkMsg = "【票据认证完成】";
                }
                var remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime(remarkMsg);
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(billId), state);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //增票待认证(需认证)
        protected void btn_Authenticate_Click(object sender, EventArgs e)
        {
            var errorMsg = new StringBuilder();
            if (!string.IsNullOrEmpty(Hid_BillId.Value))
            {
                var billIds = Hid_BillId.Value.Split(',');
                foreach (var item in billIds)
                {
                    CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(item));
                    if (!model.BillState.Equals((int)CostReportBillState.Receive))
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”不符合“" + EnumAttribute.GetKeyName(CostReportBillState.Receive) + "”状态！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据待认证】");
                        _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(item), (int)CostReportBillState.Finish);
                    }
                    catch
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”保存失败！").Append("\\n");
                    }
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                errorMsg.Append("请选择相关数据！");
            }
            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                MessageBox.Show(this, errorMsg.ToString());
            }
        }

        //增票待认证(无需认证)
        protected void btn_NoAuthenticate_Click(object sender, EventArgs e)
        {
            var errorMsg = new StringBuilder();
            if (!string.IsNullOrEmpty(Hid_BillId.Value))
            {
                var billIds = Hid_BillId.Value.Split(',');
                foreach (var item in billIds)
                {
                    CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(item));
                    if (!model.BillState.Equals((int)CostReportBillState.Receive))
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”不符合“" + EnumAttribute.GetKeyName(CostReportBillState.Receive) + "”状态！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据认证完成】");
                        _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(item), (int)CostReportBillState.Verification);
                    }
                    catch
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”保存失败！").Append("\\n");
                    }
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                errorMsg.Append("请选择相关数据！");
            }
            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                MessageBox.Show(this, errorMsg.ToString());
            }
        }

        //认证完成
        protected void imgbtn_FinishAuthenticate_Click(object sender, EventArgs e)
        {
            var billId = ((ImageButton)sender).CommandArgument;
            CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
            if (model.BillState.Equals((int)CostReportBillState.Finish))
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据认证完成】");
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(billId), (int)CostReportBillState.Verification);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }

        //退回
        protected void imgbtn_Back_Click(object sender, EventArgs e)
        {
            var billId = ((ImageButton)sender).CommandArgument;
            CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(billId));
            if (!model.BillState.Equals((int)CostReportBillState.UnSubmit))
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据退回】");
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, new Guid(billId), (int)CostReportBillState.UnSubmit);
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                MessageBox.Show(this, "状态已更新，不允许此操作！");
            }
        }
        #endregion

        //批量操作提示(提示票据单总数、含税金额)
        protected void btn_Batch_Click(object sender, EventArgs e)
        {
            Hid_BillId.Value = string.Empty;
            lit_AuthenticateMsg.Text = string.Empty;
            lit_Msg.Text = string.Empty;
            var errorMsg = new StringBuilder();
            if (Request["ckId"] != null)
            {
                var datas = Request["ckId"].Split(',');
                string billIds = string.Empty;
                foreach (var item in datas)
                {
                    var billId = item.Split('&')[0];
                    var billState = item.Split('&')[1];
                    var billNo = item.Split('&')[2];
                    if (!billState.Equals(ddl_BillState.SelectedValue))
                    {
                        errorMsg.Append("“票据号码：").Append(billNo).Append("”不符合“" + ddl_BillState.SelectedItem.Text + "”状态！").Append("<br/>");
                        continue;
                    }
                    billIds += "," + billId;
                }

                if (!string.IsNullOrEmpty(billIds))
                {
                    Hid_BillId.Value = billIds.Substring(1);
                    ArrayList arrayList = _costReportBill.GetSumBill(Hid_BillId.Value.Split(','));
                    if (arrayList.Count > 0)
                    {
                        errorMsg.Append("<b>票据单</b><span style='color:red; padding:0 5px 0 5px;'>" + arrayList[0] + "</span>张、<b>含税金额</b><span style='color:red; padding:0 5px 0 5px;'>" + ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(arrayList[1].ToString())) + "</span>元");
                    }
                }
            }
            else
            {
                errorMsg.Append("<span style='color:red;'>请选择相关数据！</span>");
            }

            if (Hid_Type.Value.Equals("1"))
            {
                lit_Msg.Text = errorMsg.ToString();
                MessageBox.AppendScript(this, "ShowValue('" + Hid_BillId.Value + "');moveShow();");
            }
            else if (Hid_Type.Value.Equals("2"))
            {
                lit_AuthenticateMsg.Text = errorMsg.ToString();
                MessageBox.AppendScript(this, "ShowValue('" + Hid_BillId.Value + "');moveShowAuthenticate('" + Hid_BillId.Value + "');");
            }
        }

        //批量操作
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            var errorMsg = new StringBuilder();
            if (!string.IsNullOrEmpty(Hid_BillId.Value))
            {
                var billIds = Hid_BillId.Value.Split(',');
                foreach (var item in billIds)
                {
                    CostReportBillInfo model = _costReportBill.Getlmshop_CostReportBillByBillId(new Guid(item));
                    if (!model.BillState.ToString().Equals(ddl_BillState.SelectedValue))
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”不符合“" + ddl_BillState.SelectedItem.Text + "”状态！").Append("\\n");
                        continue;
                    }
                    try
                    {
                        BatchPass(model);
                    }
                    catch
                    {
                        errorMsg.Append("“票据号码：").Append(model.BillNo).Append("”保存失败！").Append("\\n");
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        #region 批量操作
        protected void BatchPass(CostReportBillInfo model)
        {
            #region 票据提交
            if (int.Parse(ddl_BillState.SelectedValue).Equals((int)CostReportBillState.UnSubmit))
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【提交票据】");
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, model.BillId, (int)CostReportBillState.Submit);
            }
            #endregion

            #region 票据接收
            if (int.Parse(ddl_BillState.SelectedValue).Equals((int)CostReportBillState.Submit))
            {
                int state;
                string remarkMsg = string.Empty;
                if (!model.Tax.Equals(0))//增值税专用发票
                {
                    state = (int)CostReportBillState.Receive;
                    remarkMsg = "【接收票据】";
                }
                else
                {
                    state = (int)CostReportBillState.Verification;
                    remarkMsg = "【票据认证完成】";
                }
                var remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime(remarkMsg);
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, model.BillId, state);
            }
            #endregion

            #region 完成认证
            if (int.Parse(ddl_BillState.SelectedValue).Equals((int)CostReportBillState.Finish))
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据认证完成】");
                _costReportBill.Updatelmshop_CostReportBillByBillId(remark, model.BillId, (int)CostReportBillState.Verification);
            }
            #endregion
        }
        #endregion

        //导出Excel
        protected void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            RG_Invoice.Columns[0].Visible = false;
            RG_Invoice.Columns[15].Visible = false;
            RG_Invoice.Columns[16].Visible = false;
            RG_Invoice.Columns[17].Visible = false;
            RG_Invoice.Columns[18].Visible = false;
            RG_Invoice.Columns[19].Visible = false;
            RG_Invoice.ExportSettings.ExportOnlyData = true;
            RG_Invoice.ExportSettings.IgnorePaging = true;
            RG_Invoice.ExportSettings.FileName = Server.UrlEncode("票据信息");
            RG_Invoice.MasterTableView.ExportToExcel();
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "/CostReport/CostReport_InvoiceOperationManage.aspx";
            return ERP.UI.Web.Common.WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
    }
}