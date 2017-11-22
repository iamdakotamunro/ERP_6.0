using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class District : Base<District>, ICaching<DistrictDTO>
    {
        public static Key Key
        {
            get { return Key.ALLDistrictList; }
        }

        public IList<DistrictDTO> ToList()
        {
            return CacheHelper.Get(Key, () => WMSSao.GetAddressLibrary().Districts);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }

        public DistrictDTO Get(Guid districtId)
        {
            return ToList().FirstOrDefault(ent => ent.DistrictId == districtId);
        }
    }
}
