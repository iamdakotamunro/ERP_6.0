using System;

namespace ERP.SAL.WMS
{
    public class AutoReturnGoodsGridInfo
    {
        ///<summary>可退货数
        /// </summary>
        public Int32 CanReturnQuantity { get; set; }
     
        ///<summary>商品编号
        /// </summary>
        public String GoodsCode { get; set; }

        ///<summary>商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
    
        ///<summary>商品名称
        /// </summary>
        public String GoodsName { get; set; }
    
        ///<summary>子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        ///<summary>规格
        /// </summary>
        public String Sku { get; set; }
   
        /// <summary>计量单位
        /// </summary>
        public String Unit { get; set; }
        
    }
}
