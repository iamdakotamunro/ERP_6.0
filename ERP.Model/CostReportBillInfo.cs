using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class CostReportBillInfo
    {
        public CostReportBillInfo()
        { }
        public CostReportBillInfo(IDataReader reader)
        {
            _billId = Guid.Parse(reader["BillId"].ToString());
            if (reader["ReportId"] != DBNull.Value)
            {
                _reportId = Guid.Parse(reader["ReportId"].ToString());
            }
            if (reader["BillUnit"] != DBNull.Value)
            {
                _billUnit = reader["BillUnit"].ToString();
            }
            if (reader["BillDate"] != DBNull.Value)
            {
                _billDate = DateTime.Parse(reader["BillDate"].ToString());
            }
            if (reader["BillNo"] != DBNull.Value)
            {
                _billNo = reader["BillNo"].ToString();
            }
            if (reader["BillCode"] != DBNull.Value)
            {
                _billCode = reader["BillCode"].ToString();
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
            if (reader["BillState"] != DBNull.Value)
            {
                _billState = int.Parse(reader["BillState"].ToString());
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
            if (reader["IsPay"] != DBNull.Value)
            {
                _isPay = bool.Parse(reader["IsPay"].ToString());
            }
            if (reader["IsPass"] != DBNull.Value)
            {
                _isPass = bool.Parse(reader["IsPass"].ToString());
            }
        }

        #region 数据库字段
        private Guid _billId;
        private Guid _reportId;
        private string _billUnit;
        private DateTime? _billDate;
        private string _billNo;
        private string _billCode;
        private decimal _noTaxAmount;
        private decimal _tax;
        private decimal _taxAmount;
        private int _billState;
        private DateTime? _operatingTime;
        private string _memo;
        private string _remark;
        private bool _isPay;
        private bool _isPass;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid BillId
        {
            set { _billId = value; }
            get { return _billId; }
        }
        /// <summary>
        /// lmShop_CostReport表的主键
        /// </summary>
        public Guid ReportId
        {
            set { _reportId = value; }
            get { return _reportId; }
        }
        /// <summary>
        /// 开票单位
        /// </summary>
        public string BillUnit
        {
            set { _billUnit = value; }
            get { return _billUnit; }
        }
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? BillDate
        {
            set { _billDate = value; }
            get { return _billDate; }
        }
        /// <summary>
        /// 票据号码
        /// </summary>
        public string BillNo
        {
            set { _billNo = value; }
            get { return _billNo; }
        }
        /// <summary>
        /// 票据代码
        /// </summary>
        public string BillCode
        {
            set { _billCode = value; }
            get { return _billCode; }
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
        /// 票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)
        /// </summary>
        public int BillState
        {
            set { _billState = value; }
            get { return _billState; }
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
        /// <summary>
        /// 是否付款(True:是;False:否;)
        /// </summary>
        public bool IsPay
        {
            set { _isPay = value; }
            get { return _isPay; }
        }
        /// <summary>
        /// 是否受理通过(True:是;False:否;)
        /// </summary>
        public bool IsPass
        {
            set { _isPass = value; }
            get { return _isPass; }
        }
        #endregion
    }
}
