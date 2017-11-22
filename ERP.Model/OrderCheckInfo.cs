//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2008年1月16日
// 文件创建人:杨海飞
// 最后修改时间:
// 最后一次修改人:
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 订单完成统计信息
    /// </summary>
    [Serializable]
    public class OrderCheckInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid WarehouseId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 快递ID
        /// </summary>
        public Guid Expressid { get; set; }
        
        /// <summary>
        /// 快递名
        /// </summary>
        public string ExpressName { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderNums { get; set; }

        //added by dyy at 2009 Oct 14th

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderIds { get; set; }

        //end add

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public OrderCheckInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressid">快递ID</param>
        /// <param name="expressname">快递名</param>
        /// <param name="ordernums">订单号</param>
        public OrderCheckInfo(Guid expressid, string expressname, int ordernums)
        {
            Expressid = expressid;
            ExpressName = expressname;
            OrderNums = ordernums;
        }

        //added by dyy at 2009 Oct 14th
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressid">快递ID</param>
        /// <param name="expressname">快递名</param>
        /// <param name="ordernums">订单号</param>
        /// <param name="orderIds">订单ID</param>
        public OrderCheckInfo(Guid expressid, string expressname, int ordernums, String orderIds)
        {
            Expressid = expressid;
            ExpressName = expressname;
            OrderNums = ordernums;
            OrderIds = orderIds;
        }
        //end add
    }
}
