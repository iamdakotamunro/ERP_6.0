using ERP.DAL.Implement.Order;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IOrder;
using ERP.DAL.Interface.IStorage;
using ERP.Environment;

namespace ERP.DAL.Factory
{
    public class OrderInstance
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public static IGoodsOrder GetGoodsOrderDao(GlobalConfig.DB.FromType fromType)
        {
            return new GoodsOrder(fromType);
        }

        /// <summary>
        /// 购物车记录信息
        /// </summary>
        public static IGoodsOrderDetail GetGoodsOrderDetailDao(GlobalConfig.DB.FromType fromType)
        {
            return new GoodsOrderDetail(fromType);
        }

        /// <summary>
        /// 退回检查信息
        /// </summary>
        public static ICheckRefund GetCheckRefundDao(GlobalConfig.DB.FromType fromType)
        {
            { return new CheckRefund(fromType); }
        }
        
        /// <summary>
        /// 记录结算价和加盟价零确认数据
        /// </summary>
        public static IBorrowLendDao GetBorrowLendDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new BorrowLendDao(fromType); }
        }
    }
}
