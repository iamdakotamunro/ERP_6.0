using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 框架加工合格证信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class FrameProcessCertificateInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ProcessNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid SaleFilialeID { get; set; }

        /// <summary>
        /// 配镜师
        /// </summary>
        [DataMember]
        public string Optician { get; set; }

        /// <summary>
        /// 配镜时间
        /// </summary>
        [DataMember]
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LeftEyeInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RightEyeInfo { get; set; }
        /// <summary>
        /// 瞳距
        /// </summary>
        [DataMember]
        public string PD { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }
    }
}
