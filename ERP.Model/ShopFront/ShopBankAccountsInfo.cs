using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 银行帐户模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopBankAccountsInfo
    {
        [DataMember]
        public Guid BankAccountsId { get; set; }

        [DataMember]
        public string BankName { get; set; }

        [DataMember]
        public decimal NonceBalance { get; set; }
    }
}
