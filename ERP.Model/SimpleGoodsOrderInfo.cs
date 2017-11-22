using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/30 9:44:00 
     * 描述    :订单简单信息 For WMS
     * =====================================================================
     * 修改时间：2016/6/30 9:44:00 
     * 修改人  ：  
     * 描述    ：
     */
    [Serializable]
    [DataContract]
    public class SimpleGoodsOrderInfo
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [DataMember]
        public string Direction { get; set; }

        /// <summary>
        /// 邮政编号
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 实际应收
        /// </summary>
        [DataMember]
        public decimal RealTotalPrice { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }
    }
}
