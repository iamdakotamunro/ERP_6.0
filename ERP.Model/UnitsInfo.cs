//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年12月27日
// 文件创建人:马力
// 最后修改时间:2006年12月27日
// 最后一次修改人:马力
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 货物单位类,如:件
    /// </summary>
    [Serializable]
    public class UnitsInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public UnitsInfo() { }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="unitsId">单位Id</param>
        /// <param name="units">单位名称</param>
        public UnitsInfo(Guid unitsId,string units)
        {
            UnitsId = unitsId;
            Units = units;
        }

        /// <summary>
        /// 计量单位ID
        /// </summary>
        public Guid UnitsId { get; private set; }

        /// <summary>
        /// 计量单位名称
        /// </summary>
        public string Units { get; set; }
    }
}
