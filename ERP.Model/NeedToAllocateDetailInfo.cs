using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>
    /// 需调拨统计基本业务明细模型
    /// </summary>
    public class NeedToAllocateDetailInfo : NeedToAllocateInfo
    {
        /// <summary>
        /// 业务数据编号
        /// </summary>
        public String BusinessNo { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public String GoodsCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public String Specification { get; set; }

        /// <summary>
        /// 如果是订单则收货人
        /// </summary>
        public String Consignee { get; set; }

        /// <summary>
        /// 如果是订单则有效时间
        /// </summary>
        public DateTime EffectiveTime { get; set; }
    }
}
