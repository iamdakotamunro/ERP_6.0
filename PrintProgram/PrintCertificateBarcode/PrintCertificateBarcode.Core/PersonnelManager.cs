using System;

namespace PrintCertificateBarcode.Core
{
    public class PersonnelManager
    {
        public static string GetRealNameByAccountNo(string accountNo,out Guid personnelId)
        {
            using (var client = new Framework.WCF.ServiceClient<MIS.Service.Contract.IService>("Group.MIS"))
            {
                var accountInfo = client.Instance.GetAccountInfo(accountNo);
                if (accountInfo == null)
                {
                    personnelId = Guid.Empty;
                    return string.Empty;
                }
                personnelId = accountInfo.PersonnelId;
                return accountInfo.RealName;
            }
        }
    }
}
