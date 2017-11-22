using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/7/18 14:30:36 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/7/18 14:30:36 
     * 修改人  ：  
     * 描述    ：
     */
    public class AgainInvoiceInfo
    {
        /// <summary>
        /// 发票编号
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// 收发票地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 品名规格
        /// </summary>
        public string InvoiceContent { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public long InvoiceNo { get; set; }
    }
}
