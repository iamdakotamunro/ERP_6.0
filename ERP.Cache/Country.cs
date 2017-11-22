using System.Collections.Generic;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class Country : Base<Country>, ICaching<CountryDTO>
    {
        public static Key Key
        {
            get { return Key.AllCountryList; }
        }

        public IList<CountryDTO> ToList()
        {
            return CacheHelper.Get(Key, () => WMSSao.GetAddressLibrary().Countries);
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
