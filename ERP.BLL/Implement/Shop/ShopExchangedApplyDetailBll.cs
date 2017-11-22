using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;

namespace ERP.BLL.Implement.Shop
{
    public class ShopExchangedApplyDetailBll : BllInstance<ShopExchangedApplyDetailBll>
    {
        private readonly IShopExchangedApplyDetail _shopApplyDetail;

        public ShopExchangedApplyDetailBll(Environment.GlobalConfig.DB.FromType fromType)
        {
            _shopApplyDetail = new ShopExchangedApplyDetailDal(fromType);
        }

    }
}
