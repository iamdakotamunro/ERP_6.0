using System;

namespace ERP.SAL.WMS
{
    public class CityDTO
    {
        /// <summary>城市ID
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>城市名称
        /// </summary>
        public String CityName { get; set; }

        /// <summary>省份ID
        /// </summary>
        public Guid ProvinceId { get; set; }
    }
}
