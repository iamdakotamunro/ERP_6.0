using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/10 9:51:35 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/10 9:51:35 
     * 修改人  ：  
     * 描述    ：
     */
    public class StorageRecordApplyInDetailDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 应进货数
        /// </summary>
        public int InQuantity { get; set; }
        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public string Sku { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }

        public Byte ShelfType { get; set; }
    }
}
