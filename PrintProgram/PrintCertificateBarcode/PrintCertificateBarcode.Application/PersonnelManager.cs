using System;

namespace PrintLabel.Manager
{
    public class PersonnelManager
    {
        public static string GetRealNameByAccountNo(string accountNo,out Guid personnelId)
        {
            using (var client = new Framework.WCF.WcfClient<MIS.Service.Contract.IService>("Group.MIS"))
            {
                var accountInfo = client.Call(e=>e.GetAccountInfo(accountNo));
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
