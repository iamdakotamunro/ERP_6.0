using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/10 11:23:38 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/10 11:23:38 
     * 修改人  ：  
     * 描述    ：
     */
    public class StorageRecordApplyOutDTO
    {
        /// <summary>
        /// 出库单明细
        /// </summary>
        public List<StorageRecordApplyOutDetailDTO> Details { get; set; }
        /// <summary>
        /// 物流配送公司ID
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
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
        /// 出库单号
        /// </summary>
        public string OutBillNo { get; set; }
        /// <summary>
        /// 出库说明
        /// </summary>
        public string OutDescription { get; set; }
        /// <summary>
        /// 储位类型
        /// </summary>
        public byte StorageType { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAfterSaleInferior { get; set; }
    }
}
