using System;
using System.Data;

namespace ERP.Model
{
    /// <summary>
    /// 申请金额
    /// </summary>
    [Serializable]
    public class CostReportAmountInfo
    {
        public CostReportAmountInfo()
        { }
        public CostReportAmountInfo(IDataReader reader)
        {
            _amountId = Guid.Parse(reader["AmountId"].ToString());
            if (reader["ReportId"] != DBNull.Value)
            {
                _reportId = Guid.Parse(reader["ReportId"].ToString());
            }
            if (reader["Num"] != DBNull.Value)
            {
                _num = int.Parse(reader["Num"].ToString());
            }
            if (reader["Amount"] != DBNull.Value)
            {
                _amount = decimal.Parse(reader["Amount"].ToString());
            }
            if (reader["IsPay"] != DBNull.Value)
            {
                _isPay = bool.Parse(reader["IsPay"].ToString());
            }
            if (reader["IsSystem"] != DBNull.Value)
            {
                _isSystem = bool.Parse(reader["IsSystem"].ToString());
            }
        }

        #region 数据库字段
        private Guid _amountId;
        private Guid _reportId;
        private int _num;
        private decimal _amount;
        private bool _isPay;
        private bool _isSystem;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid AmountId
        {
            set { _amountId = value; }
            get { return _amountId; }
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
        /// 申报次数
        /// </summary>
        public int Num
        {
            set { _num = value; }
            get { return _num; }
        }
        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal Amount
        {
            set { _amount = value; }
            get { return _amount; }
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
        /// 是否系统生成(True:是;False:否;)
        /// </summary>
        public bool IsSystem
        {
            set { _isSystem = value; }
            get { return _isSystem; }
        }
        #endregion
    }
}
