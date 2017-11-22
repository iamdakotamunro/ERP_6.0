using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位对应的我方付款银行账号
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyBankAccountsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CompanyBankAccountsInfo() { }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }
        /// <summary>
        /// 银行账号ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="filialeId">公司编号 </param>
        /// <param name="bankAccountsId">银行账号编号 </param>
        public CompanyBankAccountsInfo(Guid companyId, Guid filialeId, Guid bankAccountsId)
        {
            CompanyId = companyId;
            FilialeId = filialeId;
            BankAccountsId = bankAccountsId;
        }
    }
}
