using ERP.Model;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Organization
{
    public class AccountManager
    {
        public static LoginResultInfo Login(string userName, string password, IPersonnelSao iPersonnelSao)
        {
            var result = SAL.AccountSao.Login(userName, password);
            if (result.IsSuccess)
            {
                result.PersonnelInfo = new PersonnelManager(iPersonnelSao).Get(result.PersonnelInfo.PersonnelId);
            }
            return result;
        }
    }
}
