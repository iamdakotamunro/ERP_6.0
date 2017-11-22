using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.DAL.Implement.Company;
using ERP.Model;

namespace ERP.Cache
{
    public class BankAccount : Base<BankAccount>, ICaching<BankAccountInfo>
    {
        public  static Key Key
        {
            get { return Key.AllBankAccount; }
        }


        public IList<BankAccountInfo> ToList()
        {
            return CacheHelper.Get(Key, () =>new BankAccountDao(Environment.GlobalConfig.DB.FromType.Read).GetList().ToList());
        }

        public BankAccountInfo Get(Guid id)
        {
            return ToList().FirstOrDefault(ent => ent.BankAccountsId == id);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
