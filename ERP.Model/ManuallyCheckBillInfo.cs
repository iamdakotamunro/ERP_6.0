using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class ManuallyCheckBillInfo
    {
        public ManuallyCheckBillInfo()
        { }
        public ManuallyCheckBillInfo(IDataReader reader)
        {
            _id = Guid.Parse(reader["Id"].ToString());
            if (reader["CheckBillPersonnelId"] != DBNull.Value)
            {
                _checkBillPersonnelId = Guid.Parse(reader["CheckBillPersonnelId"].ToString());
            }
            if (reader["SalePlatformId"] != DBNull.Value)
            {
                _salePlatformId = Guid.Parse(reader["SalePlatformId"].ToString()); 
            }
            if (reader["TradeCode"] != DBNull.Value)
            {
                _tradeCode = reader["TradeCode"].ToString();
            }
            if (reader["CheckState"] != DBNull.Value)
            {
                _checkState = int.Parse(reader["CheckState"].ToString());
            }
            if (reader["ThirdOrderTotalAmount"] != DBNull.Value)
            {
                _thirdOrderTotalAmount = decimal.Parse(reader["ThirdOrderTotalAmount"].ToString());
            }
            if (reader["UnusualOrderQuantity"] != DBNull.Value)
            {
                _unusualOrderQuantity = int.Parse(reader["UnusualOrderQuantity"].ToString());
            }
            if (reader["ConfirmTotalAmount"] != DBNull.Value)
            {
                _confirmTotalAmount = decimal.Parse(reader["ConfirmTotalAmount"].ToString());
            }
            if (reader["ReceiptState"] != DBNull.Value)
            {
                _receiptState = int.Parse(reader["ReceiptState"].ToString());
            }
            if (reader["CheckBillDate"] != DBNull.Value)
            {
                _checkBillDate = DateTime.Parse(reader["CheckBillDate"].ToString());
            }
            if (reader["State"] != DBNull.Value)
            {
                _state = int.Parse(reader["State"].ToString());
            }
            if (reader["Memo"] != DBNull.Value)
            {
                _memo = reader["Memo"].ToString();
            }
        }

        #region 数据库字段
        private Guid _id;
        private Guid _checkBillPersonnelId;
        private Guid _salePlatformId;
        private string _tradeCode;
        private int _checkState;
        private decimal _thirdOrderTotalAmount;
        private int _unusualOrderQuantity;
        private decimal _confirmTotalAmount;
        private int _receiptState;
        private DateTime _checkBillDate;
        private int _state;
        private string _memo;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 对账人Id
        /// </summary>
        public Guid CheckBillPersonnelId
        {
            set { _checkBillPersonnelId = value; }
            get { return _checkBillPersonnelId; }
        }
        /// <summary>
        /// 销售平台Id
        /// </summary>
        public Guid SalePlatformId
        {
            set { _salePlatformId = value; }
            get { return _salePlatformId; }
        }
        /// <summary>
        /// 单据号
        /// </summary>
        public string TradeCode
        {
            set { _tradeCode = value; }
            get { return _tradeCode; }
        }
        /// <summary>
        /// 对账状态(-1:所有;0:未对账;1:已经对账;2:异常对账;3:对账中;)
        /// </summary>
        public int CheckState
        {
            set { _checkState = value; }
            get { return _checkState; }
        }
        /// <summary>
        /// 第三方订单总金额
        /// </summary>
        public decimal ThirdOrderTotalAmount
        {
            set { _thirdOrderTotalAmount = value; }
            get { return _thirdOrderTotalAmount; }
        }
        /// <summary>
        /// 异常订单数
        /// </summary>
        public int UnusualOrderQuantity
        {
            set { _unusualOrderQuantity = value; }
            get { return _unusualOrderQuantity; }
        }
        /// <summary>
        /// 财务确认总金额
        /// </summary>
        public decimal ConfirmTotalAmount
        {
            set { _confirmTotalAmount = value; }
            get { return _confirmTotalAmount; }
        }
        /// <summary>
        /// 收款状态(-1:全部;0:未收款;1:已收款;)
        /// </summary>
        public int ReceiptState
        {
            set { _receiptState = value; }
            get { return _receiptState; }
        }
        /// <summary>
        /// 对账日期
        /// </summary>
        public DateTime CheckBillDate
        {
            set { _checkBillDate = value; }
            get { return _checkBillDate; }
        }
        /// <summary>
        /// 数据处理状态(0:对账原始表;1:双方对比表;2:财务确认表;3:对账结果表;)
        /// </summary>
        public int State
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            set { _memo = value; }
            get { return _memo; }
        }
        #endregion
    }
}
