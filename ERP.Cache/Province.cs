using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.SAL;
using ERP.SAL.WMS;

namespace ERP.Cache
{
    public class Province : Base<Province>, ICaching<ProvinceDTO>
    {
        public static Key Key
        {
            get { return Key.ProvinceList; }
        }

        public IList<ProvinceDTO> ToList()
        {
            return CacheHelper.Get(Key, () => WMSSao.GetAddressLibrary().Provinces);
        }

        public ProvinceDTO Get(Guid provinceId)
        {
            return ToList().FirstOrDefault(ent => ent.ProvinceId == provinceId);
        }

        public String GetName(Guid provinceId)
        {
            var provinceInfo = Get(provinceId);
            return provinceInfo == null ? String.Empty : provinceInfo.ProvinceName;
        }

        public void Remove()
        {
            CacheHelper.Remove(Key);
        }
    }
}
