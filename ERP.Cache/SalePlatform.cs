using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using Keede.Ecsoft.Model;

namespace ERP.Cache
{
    public class SalePlatform : Base<SalePlatform>, ICaching<SalePlatformInfo>
    {
        public static Key Key
        {
            get { return Key.SalePlatformList; }
        }

        public IList<SalePlatformInfo> ToList()
        {
            return SAL.MISService.GetAllSalePlatform().ToList();
        }

        public string GetName(Guid id)
        {
            var info = ToList().FirstOrDefault(ent => ent.ID == id);
            if (info != null)
            {
                return info.Name;
            }
            return string.Empty;
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
