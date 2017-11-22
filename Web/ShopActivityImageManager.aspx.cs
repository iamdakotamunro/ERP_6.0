using System;
using System.Linq;
using ERP.DAL.Implement.Shop;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model.ShopFront;

namespace ERP.UI.Web
{
    /// <summary>门店首页活动图片管理
    /// </summary>
    public partial class ShopActivityImageManager : BasePage
    {
        private readonly ShopActivityImageDal _activityImageDalWrite = new ShopActivityImageDal(GlobalConfig.DB.FromType.Write);

        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var list = _activityImageDalWrite.SelectShopActivityImageInfo();
                if (list.Count <= 0) return;
                //网站
                var web = list.FirstOrDefault(ent => ent.ShopActivityImageType == (int)ShopActivityImageType.Web);
                if (web != null && !string.IsNullOrWhiteSpace(web.ShopActivityImage))
                    Editor_DescriptionWeb.Content = web.ShopActivityImage;
                //手机
                var phone = list.FirstOrDefault(ent => ent.ShopActivityImageType == (int)ShopActivityImageType.Phone);
                if (phone != null && !string.IsNullOrWhiteSpace(phone.ShopActivityImage))
                    Editor_DescriptionPhone.Content = phone.ShopActivityImage;
            }
        }

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_UpdateGoods(object sender, EventArgs e)
        {
            var index = RadMultiPage1.SelectedIndex;
            ShopActivityImageInfo shopActivityImageInfo;
            switch (index)
            {
                case 0://网站
                    shopActivityImageInfo = new ShopActivityImageInfo
                   {
                       ShopActivityImage = Editor_DescriptionWeb.Content,
                       ShopActivityImageType = (int)ShopActivityImageType.Web
                   };
                    break;
                case 1://手机
                    shopActivityImageInfo = new ShopActivityImageInfo
                    {
                        ShopActivityImage = Editor_DescriptionPhone.Content,
                        ShopActivityImageType = (int)ShopActivityImageType.Phone
                    };
                    break;
                default:
                    shopActivityImageInfo = null;
                    break;
            }
            if (shopActivityImageInfo == null) return;
            var result = _activityImageDalWrite.InsertOrUpdate(shopActivityImageInfo);
            RAM.Alert(result ? "保存成功" : "保存失败");
        }
    }
}