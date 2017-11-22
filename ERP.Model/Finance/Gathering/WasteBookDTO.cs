using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// 收款单：资金流（转账）
    /// 收款单类型Type（true：正的收款单、false：负的收款单）
    /// </summary>
    [Serializable]
    [DataContract]
    public class WasteBookDTO
    {
        /// <summary>
        /// 记账本ID
        /// </summary>
        [DataMember]
        public Guid WasteBookId { get; set; }

        /// <summary>
        /// 银行账户ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 账单编号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        [DataMember]
        public decimal Income { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 收款单类型（true：正的收款单、false：负的收款单）
        /// </summary>
        [DataMember]
        public bool Type { get; set; }
    }
}