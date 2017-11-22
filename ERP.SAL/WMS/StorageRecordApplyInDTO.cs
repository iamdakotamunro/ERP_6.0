using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/10 9:50:27 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/10 9:50:27 
     * 修改人  ：  
     * 描述    ：
     */
    public class StorageRecordApplyInDTO
    {
        public List<StorageRecordApplyInDetailDTO> Details { get; set; }

        /// <summary>
        /// 托管公司ID
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        public Guid SaleFilialeId { get; set; }
        
        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid OperatorId { get; set; }
        
        /// <summary>
        /// 操作者姓名
        /// </summary>
        public string OperatorName { get; set; }
        
        /// <summary>
        /// 采购责任人姓名
        /// </summary>
        public string PurchaseResponsiblePersonName { get; set; }
        
        /// <summary>
        /// 来源单据号
        /// </summary>
        public string SourceNo { get; set; }
        
        /// <summary>
        /// 储位类型
        /// </summary>
        public byte StorageType { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }
       
        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }
    }
}
