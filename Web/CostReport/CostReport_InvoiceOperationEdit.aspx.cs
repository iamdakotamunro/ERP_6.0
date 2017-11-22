using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.UI.Web.Common;
using ERP.UI.Web.Base;
using System.Text;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_InvoiceOperationEdit : WindowsPage
    {
        private static readonly ICostReportBill _costReportBill = new CostReportBillDal(GlobalConfig.DB.FromType.Write);

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
        #endregion

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
            txt_BillUnit.Text = model.BillUnit;
            txt_BillDate.Text = DateTime.Parse(model.BillDate.ToString()).ToString("yyyy-MM-dd");
            txt_NoTaxAmount.Text = model.NoTaxAmount.ToString(CultureInfo.InvariantCulture);
            txt_Tax.Text = model.Tax.ToString(CultureInfo.InvariantCulture);
            txt_BillNo.Text = model.BillNo;
            txt_TaxAmount.Text = model.TaxAmount.ToString(CultureInfo.InvariantCulture);
            txt_BillCode.Text = model.BillCode;
            txt_BillState.Text = EnumAttribute.GetKeyName((CostReportBillState)model.BillState);
            txt_Memo.Text = model.Memo;

            var invoiceType = Request.QueryString["InvoiceType"];
            if (int.Parse(invoiceType).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                VatInvoice.Visible = true;
            }
            else if (int.Parse(invoiceType).Equals((int)CostReportInvoiceType.Voucher))
            {
                lit_BillNo.Text = "收据";
                lit_TaxAmount.Visible = BillCode.Visible = txtBillCode.Visible = false;
            }
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
            #endregion

            CostReportBillInfo.BillUnit = txt_BillUnit.Text;
            CostReportBillInfo.BillDate = DateTime.Parse(txt_BillDate.Text);
            CostReportBillInfo.NoTaxAmount = decimal.Parse(txt_NoTaxAmount.Text);
            CostReportBillInfo.Tax = decimal.Parse(txt_Tax.Text);
            CostReportBillInfo.BillNo = txt_BillNo.Text;
            CostReportBillInfo.TaxAmount = decimal.Parse(txt_TaxAmount.Text);
            CostReportBillInfo.BillCode = txt_BillCode.Text;
            CostReportBillInfo.Memo = txt_Memo.Text;
            var invoiceType = Request.QueryString["InvoiceType"];
            if (int.Parse(invoiceType).Equals((int)CostReportInvoiceType.VatInvoice))
            {
                if ((decimal.Parse(txt_NoTaxAmount.Text) + decimal.Parse(txt_Tax.Text)) != decimal.Parse(txt_TaxAmount.Text))
                {
                    MessageBox.Show(this, "“未税金额”+“税额”≠“含税金额”！");
                    return;
                }
            }
            var result = _costReportBill.Updatelmshop_CostReportBillByBillId(CostReportBillInfo);
            if (result)
            {
                string remark = ERP.UI.Web.Common.WebControl.RetrunUserAndTime("【票据修改】");
                _costReportBill.Updatelmshop_CostReportBillRemarkByBillId(remark, CostReportBillInfo.BillId);
                MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
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

            var billDate = txt_BillDate.Text;
            if (string.IsNullOrEmpty(billDate))
            {
                errorMsg.Append("请选择“开票日期”！").Append("\\n");
            }
            var noTaxAmount = txt_NoTaxAmount.Text;
            if (string.IsNullOrEmpty(noTaxAmount))
            {
                errorMsg.Append("请填写“未税金额”！").Append("\\n");
            }
            var tax = txt_Tax.Text;
            if (string.IsNullOrEmpty(tax))
            {
                errorMsg.Append("请填写“税额”！").Append("\\n");
            }
            var billNo = txt_BillNo.Text;
            if (string.IsNullOrEmpty(billNo))
            {
                errorMsg.Append("请填写“" + lit_BillNo.Text + "号码”！").Append("\\n");
            }

            var invoiceType = Request.QueryString["InvoiceType"];
            if (!int.Parse(invoiceType).Equals((int)CostReportInvoiceType.Voucher))
            {
                var billCode = txt_BillCode.Text;
                if (string.IsNullOrEmpty(billCode))
                {
                    errorMsg.Append("请填写“发票代码”！").Append("\\n");
                }
            }

            return errorMsg.ToString();
        }
    }
}