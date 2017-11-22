using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/15 18:24:16 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/15 18:24:16 
     * 修改人  ：  
     * 描述    ：
     */
    public class AutoReturnGoodsRequestDTO
    {
        /// <summary>
        /// 托管公司
        /// </summary>
        public Guid HostingFilialeId { get; set; }
        /// <summary>
        /// 储位
        /// </summary>
        public byte StorageType { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }
    }
}
