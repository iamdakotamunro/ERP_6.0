using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// 补贴审核、补贴打款——搜索的Model
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo_SeachModel
    {
        /// <summary>
        /// 单据号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// 补贴类型
        /// </summary>
        [DataMember]
        public int SubsidyType { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示多少条数据
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 显示哪些状态的数据（为空时，状态全部显示）
        /// </summary>
        public List<int> listStatus { get; set; }
    }
}