using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/7/1 17:24:44 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/7/1 17:24:44 
     * 修改人  ：  
     * 描述    ：
     */
    public class UpateInvoiceInfo
    {
        /// <summary>
        /// 订单发票号集合
        /// </summary>
        public List<UpateInvoiceDetailInfo> UpateInvoiceDetailList { get; set; }

        /// <summary>
        /// 需要更新的发票卷集合
        /// </summary>
        public IDictionary<Guid, long> UpateInvoiceDicList { get; set; }
    }

    public class UpateInvoiceDetailInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

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
