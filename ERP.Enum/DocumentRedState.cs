using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/11 13:39:00 
     * 描述    : 单据红冲状态
     * =====================================================================
     * 修改时间：2016/8/11 13:39:00 
     * 修改人  ：  
     * 描述    ：
     */
    public enum DocumentRedState
    {
        /// <summary>待审核
        /// </summary>
        [Enum("待审核")]
        WaitAudit = 1,

        /// <summary>核退
        /// </summary>
        [Enum("核退")]
        Refuse = 2,

        /// <summary>待红冲
        /// </summary>
        [Enum("待红冲")]
        WaitRed = 3,

        /// <summary>完成
        /// </summary>
        [Enum("完成")]
        Finished = 4,

        /// <summary>红冲状态
        /// </summary>
        [Enum("红冲状态")]
        Red = 5
    }
}
