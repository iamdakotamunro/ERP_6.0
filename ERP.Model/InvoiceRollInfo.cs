using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/30 10:02:31 
     * 描述    : 发票信息 For WMS
     * =====================================================================
     * 修改时间：2016/6/30 10:02:31 
     * 修改人  ：  
     * 描述    ：
     */
     [Serializable]
     [DataContract]
    public class InvoiceRollInfo
    {
        /// <summary>
        /// 发票代码
        /// </summary>
        [DataMember]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 最大发票号
        /// </summary>
        [DataMember]
        public long MaxInvoiceNo { get; set; }
    }
}
