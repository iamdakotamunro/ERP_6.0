using System;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// mini省份信息
    /// </summary>
    [Serializable]
    public class MiniProvinceInfo
    {
        /// <summary>
        /// 省份id
        /// </summary>
        public Guid ProvinceID { get; set; }

        /// <summary>
        /// 身份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 国家id
        /// </summary>
        public Guid CountryID { get; set; }
    }
}
