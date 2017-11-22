using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/30 19:15:14 
     * 描述    : 订单发票详细信息 For WMS
     * =====================================================================
     * 修改时间：2016/6/30 19:15:14 
     * 修改人  ：  
     * 描述    ：
     */
     [Serializable]
     [DataContract]
    public class GoodsOrderInvoiceInfo
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 运输费
        /// </summary>
        [DataMember]
        public decimal Carriage { get; set; }

        /// <summary>
        /// 促销价值
        /// </summary>
        [DataMember]
        public decimal PromotionValue { get; set; }

        /// <summary>
        /// 实际支付
        /// </summary>
        [DataMember]
        public decimal PaidUp { get; set; }

        /// <summary>
        /// 实际应收
        /// </summary>
        [DataMember]
        public decimal RealTotalPrice { get; set; }

        /// <summary>
        /// 支付模式
        /// </summary>
        [DataMember]
        public int PayMode { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [DataMember]
        public string Direction { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        [DataMember]
        public string ExpressNo { get; set; }

        /// <summary>
        /// 订单明细集合
        /// </summary>
        [DataMember]
        public List<GoodsOrderDetails> GoodsOrderDetailList { get; set; }

        /// <summary>
        /// 发票打印信息
        /// </summary>
        [DataMember]
        public List<SimpleInvoiceInfo> SimpleInvoiceList { get; set; }
    }

    [DataContract]
    public class GoodsOrderDetails
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public double Quantity { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember]
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 总计（数量*销售价）
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

    }

}
