using ERP.DAL.Implement.Shop;


namespace ERP.BLL.Implement.Shop
{
    /// <summary>
    /// 加盟店活动广告
    /// </summary>
    public class ShopActivityNoticeBll : BllInstance<ShopActivityNoticeBll>
    {
        private readonly ShopActivityNoticeDal _activityNoticeDal;

        public ShopActivityNoticeBll(Environment.GlobalConfig.DB.FromType fromType)
        {
            _activityNoticeDal = new ShopActivityNoticeDal(fromType);
        }
    }
}
