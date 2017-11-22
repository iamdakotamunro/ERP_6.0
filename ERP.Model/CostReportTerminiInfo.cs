//Author: zal
//createtime:2016-8-5 14:50:14
//Description: 

using System;
using System.Data;
namespace ERP.Model
{
    [Serializable]
    public class CostReportTerminiInfo
    {
        public CostReportTerminiInfo()
        { }
        public CostReportTerminiInfo(IDataReader reader)
        {
            _terminiId = Guid.Parse(reader["TerminiId"].ToString());
            if (reader["ReportId"] != DBNull.Value)
            {
                _reportId = Guid.Parse(reader["ReportId"].ToString());
            }
            if (reader["StartDay"] != DBNull.Value)
            {
                _startDay = DateTime.Parse(reader["StartDay"].ToString());
            }
            if (reader["EndDay"] != DBNull.Value)
            {
                _endDay = DateTime.Parse(reader["EndDay"].ToString());
            }
            if (reader["TerminiLocation"] != DBNull.Value)
            {
                _terminiLocation = reader["TerminiLocation"].ToString();
            }
            if (reader["OperatingTime"] != DBNull.Value)
            {
                _operatingTime = DateTime.Parse(reader["OperatingTime"].ToString());
            }
        }

        #region 数据库字段
        private Guid _terminiId;
        private Guid _reportId;
        private DateTime? _startDay;
        private DateTime? _endDay;
        private string _terminiLocation;
        private DateTime? _operatingTime;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid TerminiId
        {
            set { _terminiId = value; }
            get { return _terminiId; }
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
        /// 起日
        /// </summary>
        public DateTime? StartDay
        {
            set { _startDay = value; }
            get { return _startDay; }
        }
        /// <summary>
        /// 止日
        /// </summary>
        public DateTime? EndDay
        {
            set { _endDay = value; }
            get { return _endDay; }
        }
        /// <summary>
        /// 起讫地点
        /// </summary>
        public string TerminiLocation
        {
            set { _terminiLocation = value; }
            get { return _terminiLocation; }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperatingTime
        {
            set { _operatingTime = value; }
            get { return _operatingTime; }
        }
        #endregion
    }
}