using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/11 13:36:53 
     * 描述    : 单据类型
     * =====================================================================
     * 修改时间：2016/8/11 13:36:53 
     * 修改人  ：  
     * 描述    ：
     */
    public enum DocumentType
    {
        /// <summary>
        /// 新入库单
        /// </summary>
        [Enum("新入库单")]
        NewInDocument = 0,
        /// <summary>
        /// 新出库单
        /// </summary>
        [Enum("新出库单")]
        NewOutDocument = 1,
        /// <summary>
        /// 红冲单
        /// </summary>
        [Enum("红冲单")]
        RedDocument = 2
    }
}
