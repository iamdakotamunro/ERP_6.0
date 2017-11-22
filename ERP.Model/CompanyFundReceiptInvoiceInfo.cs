//Author: zal
//createtime:2016/1/12 16:29:08
//Description: 

using System;
using System.Data;

namespace ERP.Model
{
    [Serializable]
    public class CompanyFundReceiptInvoiceInfo
    {
        public CompanyFundReceiptInvoiceInfo()
        { }
        public CompanyFundReceiptInvoiceInfo(IDataReader reader)
        {
            _invoiceId = Guid.Parse(reader["InvoiceId"].ToString());
            if (reader["ReceiptID"] != DBNull.Value)
            {
                _receiptId = Guid.Parse(reader["ReceiptID"].ToString());
            }
            if (reader["BillingUnit"] != DBNull.Value)
            {
                _billingUnit = reader["BillingUnit"].ToString();
            }
            if (reader["BillingDate"] != DBNull.Value)
            {
                _billingDate = DateTime.Parse(reader["BillingDate"].ToString());
            }
            if (reader["InvoiceNo"] != DBNull.Value)
            {
                _invoiceNo = reader["InvoiceNo"].ToString();
            }
            if (reader["InvoiceCode"] != DBNull.Value)
            {
                _invoiceCode = reader["InvoiceCode"].ToString();
            }
            if (reader["NoTaxAmount"] != DBNull.Value)
            {
                _noTaxAmount = decimal.Parse(reader["NoTaxAmount"].ToString());
            }
            if (reader["Tax"] != DBNull.Value)
            {
                _tax = decimal.Parse(reader["Tax"].ToString());
            }
            if (reader["TaxAmount"] != DBNull.Value)
            {
                _taxAmount = decimal.Parse(reader["TaxAmount"].ToString());
            }
            if (reader["InvoiceState"] != DBNull.Value)
            {
                _invoiceState =int.Parse(reader["InvoiceState"].ToString());
            }
            if (reader["OperatingTime"] != DBNull.Value)
            {
                _operatingTime = DateTime.Parse(reader["OperatingTime"].ToString());
            }
            if (reader["Memo"] != DBNull.Value)
            {
                _memo = reader["Memo"].ToString();
            }
            if (reader["Remark"] != DBNull.Value)
            {
                _remark = reader["Remark"].ToString();
            }
        }

        #region 数据库字段
        private Guid _invoiceId;
        private Guid _receiptId;
        private string _billingUnit;
        private DateTime? _billingDate;
        private string _invoiceNo;
        private string _invoiceCode;
        private decimal _noTaxAmount;
        private decimal _tax;
        private decimal _taxAmount;
        private int _invoiceState;
        private DateTime? _operatingTime;
        private string _memo;
        private string _remark;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid InvoiceId
        {
            set { _invoiceId = value; }
            get { return _invoiceId; }
        }
        
        /// <summary>
        /// lmshop_CompanyFundReceipt表的主键
        /// </summary>
        public Guid ReceiptID
        {
            set { _receiptId = value; }
            get { return _receiptId; }
        }
        /// <summary>
        /// 开票单位
        /// </summary>
        public string BillingUnit
        {
            set { _billingUnit = value; }
            get { return _billingUnit; }
        }
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? BillingDate
        {
            set { _billingDate = value; }
            get { return _billingDate; }
        }
        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo
        {
            set { _invoiceNo = value; }
            get { return _invoiceNo; }
        }
        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode
        {
            set { _invoiceCode = value; }
            get { return _invoiceCode; }
        }
        /// <summary>
        /// 未税金额
        /// </summary>
        public decimal NoTaxAmount
        {
            set { _noTaxAmount = value; }
            get { return _noTaxAmount; }
        }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal Tax
        {
            set { _tax = value; }
            get { return _tax; }
        }
        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal TaxAmount
        {
            set { _taxAmount = value; }
            get { return _taxAmount; }
        }
        /// <summary>
        /// 发票状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)
        /// </summary>
        public int InvoiceState
        {
            set { _invoiceState = value; }
            get { return _invoiceState; }
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperatingTime
        {
            set { _operatingTime = value; }
            get { return _operatingTime; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            set { _memo = value; }
            get { return _memo; }
        }
        /// <summary>
        /// 流程记录
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        #endregion
    }
}