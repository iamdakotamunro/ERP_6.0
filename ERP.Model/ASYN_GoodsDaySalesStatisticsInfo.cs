using System;
namespace ERP.Model
{
    /// <summary>销量异步模型信息   Add   陈重文  2016-11-9 
    /// </summary>
    public class ASYN_GoodsDaySalesStatisticsInfo
    {
        /// <summary>标识唯一ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>冗余订单号，方便查询
        /// </summary>
        public String OrderNo { get; set; }

        /// <summary>订单信息Json字符串
        /// </summary>
        public String OrderJsonStr { get; set; }

        /// <summary>订单明细信息Json字符串
        /// </summary>
        public String OrderDetailJsonStr { get; set; }

        /// <summary>订单修改前明细信息JSON格式字符串
        /// </summary>
        public String OldOrderDetailJsonStr { get; set; }

        /// <summary>销量处理状态（0新增销量，1更新销量，2删除销量）
        /// </summary>
        public Int32 HandlingStatus { get; set; }

        /// <summary>单据创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>数据是否已执行
        /// </summary>
        public Boolean IsExecuted { get; set; }

        /// <summary>新增销量
        /// </summary>
        /// <returns></returns>
        public static ASYN_GoodsDaySalesStatisticsInfo AddGoodsDaySale(String orderNo, String orderJsonStr, String orderDetailJsonStr)
        {
            return new ASYN_GoodsDaySalesStatisticsInfo
            {
                OrderNo = orderNo,
                OrderJsonStr = orderJsonStr,
                OrderDetailJsonStr = orderDetailJsonStr,
                HandlingStatus = 0
            };
        }

        /// <summary>编辑销量
        /// </summary>
        /// <returns></returns>
        public static ASYN_GoodsDaySalesStatisticsInfo EditGoodsDaySale(String orderNo, String orderJsonStr, String orderDetailJsonStr, String oldOrderDetailJsonStr)
        {
            return new ASYN_GoodsDaySalesStatisticsInfo
            {
                OrderNo = orderNo,
                OrderJsonStr = orderJsonStr,
                OrderDetailJsonStr = orderDetailJsonStr,
                OldOrderDetailJsonStr = oldOrderDetailJsonStr,
                HandlingStatus = 1
            };
        }

        /// <summary>删除销量
        /// </summary>
        /// <returns></returns>
        public static ASYN_GoodsDaySalesStatisticsInfo DeleteGoodsDaySale(String orderNo, String orderJsonStr, String orderDetailJsonStr)
        {
            return new ASYN_GoodsDaySalesStatisticsInfo
            {
                OrderNo = orderNo,
                OrderJsonStr = orderJsonStr,
                OrderDetailJsonStr = orderDetailJsonStr,
                HandlingStatus = 2
            };
        }
    }
}
