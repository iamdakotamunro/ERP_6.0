using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class ManuallyCheckBillDetailInfo
    {
        public ManuallyCheckBillDetailInfo()
        { }
        public ManuallyCheckBillDetailInfo(IDataReader reader)
        {
            _id = Guid.Parse(reader["Id"].ToString());
            if (reader["ManuallyCheckBillId"] != DBNull.Value)
            {
                _manuallyCheckBillId = Guid.Parse(reader["ManuallyCheckBillId"].ToString());
            }
            if (reader["SystemOrderNo"] != DBNull.Value)
            {
                _systemOrderNo = reader["SystemOrderNo"].ToString();
            }
            if (reader["ThirdOrderNo"] != DBNull.Value)
            {
                _thirdOrderNo = reader["ThirdOrderNo"].ToString();
            }
            if (reader["OrderTime"] != DBNull.Value)
            {
                _orderTime = DateTime.Parse(reader["OrderTime"].ToString());
            }
            if (reader["MemberId"] != DBNull.Value)
            {
                _memberId = Guid.Parse(reader["MemberId"].ToString());
            }
            if (reader["SystemOrderAmount"] != DBNull.Value)
            {
                _systemOrderAmount = decimal.Parse(reader["SystemOrderAmount"].ToString());
            }
            if (reader["ThirdOrderAmount"] != DBNull.Value)
            {
                _thirdOrderAmount = decimal.Parse(reader["ThirdOrderAmount"].ToString());
            }
            if (reader["Balance"] != DBNull.Value)
            {
                _balance = decimal.Parse(reader["Balance"].ToString());
            }
            if (reader["ConfirmAmount"] != DBNull.Value)
            {
                _confirmAmount = decimal.Parse(reader["ConfirmAmount"].ToString());
            }
            if (reader["ContactsReckoningDifference"] != DBNull.Value)
            {
                _contactsReckoningDifference = decimal.Parse(reader["ContactsReckoningDifference"].ToString());
            }
        }

        #region 数据库字段
        private Guid _id;
        private Guid _manuallyCheckBillId;
        private string _systemOrderNo;
        private string _thirdOrderNo;
        private DateTime _orderTime;
        private Guid _memberId;
        private decimal _systemOrderAmount;
        private decimal _thirdOrderAmount;
        private decimal _balance;
        private decimal _confirmAmount;
        private decimal _contactsReckoningDifference;
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
        public Guid ManuallyCheckBillId
        {
            set { _manuallyCheckBillId = value; }
            get { return _manuallyCheckBillId; }
        }
        /// <summary>
        /// 系统订单号
        /// </summary>
        public string SystemOrderNo
        {
            set { _systemOrderNo = value; }
            get { return _systemOrderNo; }
        }
        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string ThirdOrderNo
        {
            set { _thirdOrderNo = value; }
            get { return _thirdOrderNo; }
        }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime
        {
            set { _orderTime = value; }
            get { return _orderTime; }
        }
        /// <summary>
        /// 会员Id
        /// </summary>
        public Guid MemberId
        {
            set { _memberId = value; }
            get { return _memberId; }
        }
        /// <summary>
        /// 系统订单金额
        /// </summary>
        public decimal SystemOrderAmount
        {
            set { _systemOrderAmount = value; }
            get { return _systemOrderAmount; }
        }
        /// <summary>
        /// 第三方订单金额
        /// </summary>
        public decimal ThirdOrderAmount
        {
            set { _thirdOrderAmount = value; }
            get { return _thirdOrderAmount; }
        }
        /// <summary>
        /// 差额
        /// </summary>
        public decimal Balance
        {
            set { _balance = value; }
            get { return _balance; }
        }
        /// <summary>
        /// 财务确认金额
        /// </summary>
        public decimal ConfirmAmount
        {
            set { _confirmAmount = value; }
            get { return _confirmAmount; }
        }
        /// <summary>
        /// 往来账差异
        /// </summary>
        public decimal ContactsReckoningDifference
        {
            set { _contactsReckoningDifference = value; }
            get { return _contactsReckoningDifference; }
        }
        #endregion
    }
}
