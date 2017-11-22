using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/7/5 9:50:45 
     * 描述    :押金回收实体类
     * =====================================================================
     * 修改时间：2016/7/5 9:50:45 
     * 修改人  ：  
     * 描述    ：
     */
    public class CostReportDepositRecoveryInfo
    {
        /// <summary>
        /// 预借款单据号
        /// </summary>
        public Guid ReportId { get; set; }

        /// <summary>
        ///申报单号
        /// </summary>
        public String ReportNo { get; set; }

        /// <summary>
        /// 回收单据号
        /// </summary>
        public Guid DepositRecoveryReportId { get; set; }

        /// <summary>
        /// 回收金额
        /// </summary>
        public Decimal RecoveryCost { get; set; }

        /// <summary>
        /// 回收时间
        /// </summary>
        public DateTime RecoveryDate { get; set; }

        /// <summary>
        /// 回收类型(0:现金，1:票据)
        /// </summary>
        public bool RecoveryType { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string RecoveryRemarks { get; set; }

        /// <summary>
        /// 回收人
        /// </summary>
        public Guid RecoveryPersonnelId { get; set; }
    }
}
