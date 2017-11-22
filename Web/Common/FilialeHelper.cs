using System;
using System.Linq;
using AllianceShop.Common.Extension;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using MIS.Enum;

namespace ERP.UI.Web.Common
{
    /// <summary>判断公司
    /// </summary>
    public class FilialeHelper
    {
        /// <summary>判断销售公司是否为门店公司
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public static bool IsEntityShop(Guid saleFilialeId)
        {
            var filialeInfo = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == saleFilialeId);
            if (filialeInfo != null)
            {
                return filialeInfo.FilialeTypes.Contains((int)FilialeType.EntityShop);
            }
            return false;
        }

        public static bool IsEntityShopByOrderId(string orderId)
        {
            var oId = orderId.ToGuid();
            if (oId != Guid.Empty)
            {
                IGoodsOrder goodsOrder=new GoodsOrder(GlobalConfig.DB.FromType.Read);
                var orderInfo = goodsOrder.GetGoodsOrder(oId);
                if (orderInfo != null)
                {
                    var saleFilialeId = orderInfo.SaleFilialeId;
                    var filialeInfo = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == saleFilialeId);
                    if (filialeInfo != null)
                    {
                        return filialeInfo.FilialeTypes.Contains((int)FilialeType.EntityShop);
                    }
                }
            }
            return false;
        }

        /// <summary>判断销售公司是否为B2C公司
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public static bool IsB2C(Guid saleFilialeId)
        {
            var filialeInfo = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == saleFilialeId);
            if (filialeInfo != null)
            {
                return filialeInfo.FilialeTypes.Contains((int)FilialeType.SaleCompany);
            }
            return false;
        }
    }
}