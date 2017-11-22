using ERP.DAL.Implement.Shop;

namespace ERP.BLL.Implement.Shop
{
    /// <summary>加盟店活动图片
    /// </summary>
    public class ShopActivityImageBll : BllInstance<ShopActivityImageBll>
    {
        private readonly ShopActivityImageDal _shopActivityImageDal;

        public ShopActivityImageBll(Environment.GlobalConfig.DB.FromType fromType)
        {
            _shopActivityImageDal = new ShopActivityImageDal(fromType);
        }
    }
}
