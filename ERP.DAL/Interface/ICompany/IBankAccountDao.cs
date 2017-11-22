using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.ICompany
{
    public interface IBankAccountDao
    {
        void Insert(BankAccountInfo info);

        void Delete(Guid bankAccountsId, Guid targetId);

        void Update(Guid oldBankAccountsId, BankAccountInfo info);

        IList<BankAccountInfo> GetListByBankAccountId(Guid bankaccountid);

        IEnumerable<BankAccountInfo> GetList();

        BankAccountInfo Get(BankAccountInfo sinfo);

        IList<BankAccountInfo> GetListByTargetId(Guid targetId);

        /// <summary>获取绑定的资金账户(主账号/非主账号)
        /// </summary>
        /// <param name="isMain">是否主账号（true 主）</param>
        /// <returns></returns>
        IList<BankAccountInfo> GetListByNotIsMain(Boolean isMain);
    }
}
