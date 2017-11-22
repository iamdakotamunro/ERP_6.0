using System;

namespace ERP.SAL.WMS
{
    public class DistrictDTO
    {
        /// <summary>地区ID
        /// </summary>
        public Guid DistrictId { get; set; }

        /// <summary>地区名称
        /// </summary>
        public String DistrictName { get; set; }

        /// <summary>城市ID
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>邮政编码
        /// </summary>
        public String ZipCode { get; set; }
    }
}
