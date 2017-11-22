//Author: zal
//createtime:2016-8-5 14:49:44
//Description: 

using System;
using System.Data;
namespace ERP.Model
{
    /// <summary>
    /// 差旅费
    /// </summary>
    [Serializable]
    public class CostReportTravelInfo
    { 
        public CostReportTravelInfo()
        {}
        public CostReportTravelInfo(IDataReader reader)
        {
            _travelId = Guid.Parse(reader["TravelId"].ToString());
            if(reader["ReportId"]!=DBNull.Value)
            {
                _reportId = Guid.Parse(reader["ReportId"].ToString());
            }           
            if(reader["Entourage"]!=DBNull.Value)
            {
                _entourage = reader["Entourage"].ToString();
            }           
            if(reader["TravelAddressAndCourse"]!=DBNull.Value)
            {
                _travelAddressAndCourse = reader["TravelAddressAndCourse"].ToString();
            }           
            if(reader["Matter"]!=DBNull.Value)
            {
                _matter = int.Parse(reader["Matter"].ToString());
            }           
            if(reader["DayOrNum"]!=DBNull.Value)
            {
                _dayOrNum = decimal.Parse(reader["DayOrNum"].ToString());
            }           
            if(reader["Amount"]!=DBNull.Value)
            {
                _amount = decimal.Parse(reader["Amount"].ToString());
            }
            if (reader["OperatingTime"] != DBNull.Value)
            {
                _operatingTime = DateTime.Parse(reader["OperatingTime"].ToString());
            }
        }
        
        #region 数据库字段
        private Guid _travelId;   
        private Guid _reportId;   
        private string _entourage;   
        private string _travelAddressAndCourse;   
        private int _matter;
        private decimal _dayOrNum;   
        private decimal _amount;
        private DateTime? _operatingTime;
        #endregion  
        
        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid  TravelId 
        {
            set{_travelId=value;}
            get{return _travelId;}
        }
        /// <summary>
        /// lmShop_CostReport表的主键
        /// </summary>
        public Guid  ReportId 
        {
            set{_reportId=value;}
            get{return _reportId;}
        }
        /// <summary>
        /// 随同人员
        /// </summary>
        public string  Entourage 
        {
            set{_entourage=value;}
            get{return _entourage;}
        }
        /// <summary>
        /// 出差地点及历程
        /// </summary>
        public string  TravelAddressAndCourse 
        {
            set{_travelAddressAndCourse=value;}
            get{return _travelAddressAndCourse;}
        }
        /// <summary>
        /// 项目(0:火车费;1:汽车费;2:市内交通费;3:过路费;4:飞机费;5:餐费;6:住宿费;7:其它;)
        /// </summary>
        public int  Matter 
        {
            set{_matter=value;}
            get{return _matter;}
        }
        /// <summary>
        /// 天/张数
        /// </summary>
        public decimal DayOrNum 
        {
            set{_dayOrNum=value;}
            get{return _dayOrNum;}
        }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal  Amount 
        {
            set{_amount=value;}
            get{return _amount;}
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