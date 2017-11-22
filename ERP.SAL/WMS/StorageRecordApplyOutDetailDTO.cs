using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/10 11:23:52 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/8/10 11:23:52 
     * 修改人  ：  
     * 描述    ：
     */
    public class StorageRecordApplyOutDetailDTO
    {
        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 出货数量
        /// </summary>
        public int OutQuantity { get; set; }
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
    }
}
