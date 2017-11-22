using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.SAL.Interface
{
    public interface IPersonnelSao
    {
        IList<PersonnelInfo> GetList();

        PersonnelInfo Get(Guid personnelId);

        PersonnelInfo Get(string accountNo);

        string GetName(Guid personnelId);

        IEnumerable<PersonnelInfo> GetList(Guid filialeId, Guid branchId);

        IList<PersonnelInfo> GetAccountInfoByRealName(string realName);
    }
}
