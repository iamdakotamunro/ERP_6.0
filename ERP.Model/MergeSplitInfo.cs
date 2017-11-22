using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 商品组合拆分
    /// </summary>
    [Serializable]
    [DataContract]
    public class MergeSplitInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid MergeSplitId { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [DataMember]
        public string MergeSplitNo { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 原商品
        /// </summary>
        [DataMember]
        public string OldGoods { get; set; }

        /// <summary>
        /// 新商品
        /// </summary>
        [DataMember]
        public string NewGoods { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        [DataMember]
        public Guid SubmitPersonnelId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public Guid AuditPersonnelId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime AuditDate { get; set; }
    }
}
