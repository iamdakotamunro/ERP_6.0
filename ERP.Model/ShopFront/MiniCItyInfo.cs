using System;

namespace Keede.Ecsoft.Model.ShopFront
{ 
    /// <summary>
    /// mini城市信息
    /// </summary>
    [Serializable]
    public class MiniCItyInfo
    {
        /// <summary>
        /// 城市id
        /// </summary>
        public Guid CityID { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 省份id
        /// </summary>
        public Guid ProvinceID { get; set; }
    }
}
