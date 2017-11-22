using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class ExpressCost : Base<ExpressCost>, ICaching<ExpressCostDTO>
    {
        public static Key Key
        {
            get { return Key.AllExpress; }
        }

        public IList<ExpressCostDTO> ToList()
        {
            return CacheHelper.Get(Key, WMSSao.GetExpressCosts);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }

        public ExpressCostDTO Get(Guid expressId, Guid districtID)
        {
            return ToList().SingleOrDefault(ent => ent.ExpressId == expressId && ent.DistrictId == districtID);
        }
    }
}
