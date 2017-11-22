using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using Keede.Ecsoft.Model;

namespace ERP.Cache
{
    /// <summary>
    /// 往来账务单位
    /// </summary>
    public class RelatedCompany : Base<RelatedCompany>,ICaching<CompanyCussentInfo>
    {
        private static readonly ICompanyCussent _dao = InventoryInstance.GetCompanyCussentDao(Environment.GlobalConfig.DB.FromType.Read);

        public static Key Key
        {
            get { return Key.AllRelatedCompany; }
        }

        public IList<CompanyCussentInfo> ToList()
        {
            return CacheHelper.Get(Key, () => _dao.GetCompanyCussentList());
        }

        public CompanyCussentInfo Get(Guid id)
        {
            return ToList().FirstOrDefault(ent => ent.CompanyId == id);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
