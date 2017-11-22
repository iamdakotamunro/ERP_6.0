using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class City : Base<City>, ICaching<CityDTO>
    {
        public static Key Key
        {
            get { return Key.AllCityList; }
        }

        public IList<CityDTO> ToList()
        {
            return CacheHelper.Get(Key, () => WMSSao.GetAddressLibrary().Cities);
        }

        public CityDTO Get(Guid cityId)
        {
            return ToList().FirstOrDefault(ent => ent.CityId == cityId);
        }

        public String GetName(Guid cityId)
        {
            var cityInfo = Get(cityId);
            return cityInfo == null ? String.Empty : cityInfo.CityName;
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
