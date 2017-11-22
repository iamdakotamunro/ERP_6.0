using System;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/17 9:58:51 
     * 描述    : 商品需求查询
     * =====================================================================
     * 修改时间：2016/8/17 9:58:51 
     * 修改人  ：  
     * 描述    ：
     */
    [Serializable]
    public class GoodsDemandSearchInfo
    {
        /// <summary>商品ID
        /// </summary>
        public Guid GoodsId { get; set; }


        /// <summary>子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>有效需求数
        /// </summary>
        public int EffictiveRequire { get; set; }

        /// <summary>可用库存数
        /// </summary>
        public int CanUseGoodsStock { get; set; }
    }
}
