using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using Keede.Ecsoft.Model;

namespace ERP.Cache
{
    public class Filiale : Base<Filiale>, ICaching<FilialeInfo>
    {
        public static Key Key
        {
            get { return Key.AllFiliale; }
        }

        public IList<FilialeInfo> ToList()
        {
            return  SAL.MISService.GetAllFiliales().ToList();
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
