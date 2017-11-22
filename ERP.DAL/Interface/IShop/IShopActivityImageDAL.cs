using System.Collections.Generic;
using Keede.Ecsoft.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    /// <summary>
    /// 加盟店活动图片
    /// </summary>
    public interface IShopActivityImageDal
    {
        /// <summary>
        /// 插入或修改活动图片
        /// </summary>
        /// <returns></returns>
         bool InsertOrUpdate(ShopActivityImageInfo imageInfo);

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
         List<ShopActivityImageInfo> SelectShopActivityImageInfo();
    }
}
