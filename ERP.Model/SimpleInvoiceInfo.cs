using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/30 10:15:18 
     * 描述    : 发票信息类 For WMS
     * =====================================================================
     * 修改时间：2016/6/30 10:15:18 
     * 修改人  ：  
     * 描述    ：
     */
     [DataContract]
    public class SimpleInvoiceInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public string InvoiceName { get; set; }

        /// <summary>
        /// 品名规格
        /// </summary>
        [DataMember]
        public string InvoiceContent { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        [DataMember]
        public double InvoiceSum { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [DataMember]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        [DataMember]
        public long InvoiceNo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SimpleInvoiceDetailInfo : SimpleInvoiceInfo
    {
        /// <summary>
        /// 发票编号
        /// </summary>
        [DataMember]
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// 开票人姓名
        /// </summary>
        [DataMember]
        public string Receiver { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 发票状态：0 未开1 申请2 已开3 取消
        /// </summary>
        [DataMember]
        public int InvoiceState { get; set; }

        /// <summary>
        /// 收发票地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 发票是否报税提交
        /// </summary>
        [DataMember]
        public bool IsCommit { get; set; }
    }
}
