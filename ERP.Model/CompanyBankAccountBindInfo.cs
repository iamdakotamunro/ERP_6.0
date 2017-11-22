using System;

namespace ERP.Model
{
    [Serializable]
    public class CompanyBankAccountBindInfo
    {
        public Guid CompanyId { get; set; }

        public Guid FilialeId { get; set; }

        public string FilialeName { get; set; }

        public string WebSite { get; set; }

        public string BankAccounts { get; set; }

        public string AccountsNumber { get; set; }

        public Guid BankAccountsId { get; set; }
    }
}
