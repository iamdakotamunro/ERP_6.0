using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    [DataContract]
    public class VocabularyInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 词汇名称
        /// </summary>
        [DataMember]
        public string VocabularyName { get; set; }

        /// <summary>
        /// 状态(0：禁用,1：启用)
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime OperatingTime { get; set; }
    }
}
