using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.Finance
{
    /// <summary>
    /// 供应商档案
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorDTO
    {
        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string CompanyShortName { get; set; }

        [DataMember]
        public bool IsSynchro { get; set; }

        [DataMember]
        public Guid FilialeId { get; set; }

        [DataMember]
        public string BankAccounts { get; set; }

        [DataMember]
        public string TaxNumber { get; set; }
    }
}
