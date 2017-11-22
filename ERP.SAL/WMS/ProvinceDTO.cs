using System;

namespace ERP.SAL.WMS
{
    public class ProvinceDTO
    {
        /// <summary>国家ID
        /// </summary>
        public Guid CountryId { get; set; }

        /// <summary>省份ID
        /// </summary>
        public Guid ProvinceId { get; set; }

        /// <summary>省份名称
        /// </summary>
        public String ProvinceName { get; set; }
    }
}
