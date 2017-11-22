//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年5月9日
// 文件创建人:马力
// 最后修改时间:2010年3月20日
// 最后一次修改人:刘修明
//================================================
using System;
using System.Runtime.Serialization;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 资金流核对数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class WasteBookCheckInfo
    {
        /// <summary>
        /// 记账本ID
        /// </summary>
        [DataMember]
        public Guid WasteBookId { get; set; }

        /// <summary>
        /// 核对金额
        /// </summary>
        [DataMember]
        public decimal CheckMoney { get; set; }

        /// <summary>
        /// 核对有误备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 核对时间
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public WasteBookCheckInfo()
        {
            
        }
    }
}
