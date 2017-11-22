using System;

namespace ERP.Model
{
    /// <summary>往来单位账期模型  ADD  2015-02-03  陈重文 
    /// </summary>
    [Serializable]
    public class CompanyPaymentDaysInfo
    {

        public Guid CompanyID { get; set; }

        /// <summary>往来单位名称
        /// </summary>
        public String CompanyName { get; set; }

        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public String FilialeName { get; set; }

        /// <summary>账期
        /// </summary>
        public Int32 PaymentDays { get; set; }

        /// <summary>账期字符串
        /// </summary>
        public String PaymentDaysStr
        {
            get
            {
                if (PaymentDays == 0)
                {
                    return "-";
                }
                return PaymentDays + "个月";
            }
        }

        /// <summary>搜索年份
        /// </summary>
        public Int32 Year { get; set; }

        /// <summary>账期应付款(空表示没有)
        /// </summary>
        public Decimal PaymentDaysDue { get; set; }

        /// <summary>一月
        /// </summary>
        public Decimal Jan { get; set; }

        /// <summary>二月
        /// </summary>
        public Decimal Feb { get; set; }

        /// <summary>三月
        /// </summary>
        public Decimal Mar { get; set; }

        /// <summary>四月
        /// </summary>
        public Decimal Apr { get; set; }

        /// <summary>五月
        /// </summary>
        public Decimal May { get; set; }

        /// <summary>六月
        /// </summary>
        public Decimal Jun { get; set; }

        /// <summary>七月
        /// </summary>
        public Decimal July { get; set; }

        /// <summary>八月
        /// </summary>
        public Decimal Aug { get; set; }

        /// <summary>九月
        /// </summary>
        public Decimal Sept { get; set; }

        /// <summary>十月
        /// </summary>
        public Decimal Oct { get; set; }

        /// <summary>十一月
        /// </summary>
        public Decimal Nov { get; set; }

        /// <summary>十二月
        /// </summary>
        public Decimal December { get; set; }

        /// <summary>一月
        /// </summary>
        public Decimal Jan1{ get; set; }

        /// <summary>二月
        /// </summary>
        public Decimal Feb2{ get; set; }

        /// <summary>三月
        /// </summary>
        public Decimal Mar3 { get; set; }

        /// <summary>四月
        /// </summary>
        public Decimal Apr4 { get; set; }

        /// <summary>五月
        /// </summary>
        public Decimal May5{ get; set; }

        /// <summary>六月
        /// </summary>
        public Decimal Jun6 { get; set; }

        /// <summary>七月
        /// </summary>
        public Decimal July7 { get; set; }

        /// <summary>八月
        /// </summary>
        public Decimal Aug8{ get; set; }

        /// <summary>九月
        /// </summary>
        public Decimal Sept9{ get; set; }

        /// <summary>十月
        /// </summary>
        public Decimal Oct10 { get; set; }

        /// <summary>十一月
        /// </summary>
        public Decimal Nov11 { get; set; }

        /// <summary>十二月
        /// </summary>
        public Decimal December12 { get; set; }
    }
}
