using System.Collections.Generic;

namespace ERP.SAL.WMS
{
    public class AddressLibraryDTO
    {
        /// <summary>国家
        /// </summary>
        public List<CountryDTO> Countries { get; set; }

        /// <summary>省份
        /// </summary>
        public List<ProvinceDTO> Provinces { get; set; }

        /// <summary> 城市
        /// </summary>
        public List<CityDTO> Cities { get; set; }

        /// <summary>地区
        /// </summary>
        public List<DistrictDTO> Districts { get; set; }
    }
}
