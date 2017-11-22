using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/17 15:17:34 
     * 描述    : 储位类型
     * =====================================================================
     * 修改时间：2016/8/17 15:17:34 
     * 修改人  ：  
     * 描述    ：
     */
    [Serializable]
    public enum StorageAuthType
    {
        /// <summary>
        /// 整件区。
        /// </summary>
        [Enum("整件区")]
        //WholeArea,
        Z = 1,

        /// <summary>
        /// 零配区
        /// </summary>
        [Enum("零配区")]
        L = 2,
        //RetailArea,

        /// <summary>
        /// 售后区
        /// </summary>
        [Enum("售后区")]
        S = 3,
        //CustomerServiceArea,

        /// <summary>
        /// 样品区
        /// </summary>
        [Enum("样品区")]
        J = 4,
        //SampleArea,

        /// <summary>
        /// 坏件区
        /// </summary>
        [Enum("坏件区")]
        H = 5
        //BadArea
    }
}
