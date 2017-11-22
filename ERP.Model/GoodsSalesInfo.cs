using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/28 17:28:42 
     * 描述    : 商品销量数据 For WMS
     * =====================================================================
     * 修改时间：2016/6/28 17:28:42 
     * 修改人  ：  
     * 描述    ：
     */
    [Serializable]
    [DataContract]
    public class GoodsSalesInfo
    {
        /// <summary>
        /// 发货仓库
        /// </summary>
        [DataMember]
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        [DataMember]
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }
    }
}
