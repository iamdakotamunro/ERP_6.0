using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// 收款单：费用申报
    /// </summary>
    [Serializable]
    [DataContract]
    public class CostReportDTO
    {
        /// <summary>
        ///申报ID
        /// </summary>
        [DataMember]
        public Guid ReportId { get; set; }

        /// <summary>
        ///申报单号
        /// </summary>
        [DataMember]
        public String ReportNo { get; set; }

        /// <summary>
        ///收/付款银行支行
        /// </summary>
        [DataMember]
        public String BankAccountName { get; set; }

        /// <summary>
        ///结算方式:1现金 2转账
        /// </summary>
        [DataMember]
        public Int32 CostType { get; set; }

        /// <summary>
        ///费用申报人ID
        /// </summary>
        [DataMember]
        public Guid ReportPersonnelId { get; set; }

        /// <summary>
        ///申报时间
        /// </summary>
        [DataMember]
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// 打款分类
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 实际金额
        /// </summary>
        [DataMember]
        public Decimal RealityCost { get; set; }
    }
}