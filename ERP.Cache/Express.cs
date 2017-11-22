using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class Express : Base<Express>, ICaching<ExpressBasicDTO>
    {
        public static Key Key
        {
            get { return Key.AllExpress; }
        }

        public IList<ExpressBasicDTO> ToList()
        {
            return CacheHelper.Get(Key, WMSSao.GetExpresses);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }

        public ExpressBasicDTO Get(Guid expressId)
        {
            return ToList().FirstOrDefault(ent => ent.ExpressId == expressId);
        }
    }
}
