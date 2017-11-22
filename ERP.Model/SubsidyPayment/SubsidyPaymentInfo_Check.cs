using System;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// 补贴审核、补贴打款——财务审核
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo_Check
    {
        #region Model

        /// <summary>
        /// 主键：补贴打款ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// 审核是否通过
        /// </summary>
        [DataMember]
        public bool IsApproved { get; set; }
        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }
        
        /// <summary>
        /// 最后修改人
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        #endregion Model
    }
}
