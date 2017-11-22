using System;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// mini地区信息
    /// </summary>
     [Serializable]
   public class MiniDistrictInfo
    {
         /// <summary>
         /// 区县id
         /// </summary>
         public Guid DistrictID { get; set; }

         /// <summary>
         /// 区县名称
         /// </summary>
         public string DistrictName { get; set; }

         /// <summary>
         /// 城市id
         /// </summary>
        public Guid CityID{ get; set; }

    }
}
