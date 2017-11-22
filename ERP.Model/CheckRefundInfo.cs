#if !WCFModel
using Dapper.Extension;
#endif
using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 退换货检查模型
    /// </summary>
    [Serializable]
    [DataContract]
#if !WCFModel
    [TypeMapper]
#endif
    public class CheckRefundInfo
    {
        /// <summary>
        /// 退换货编号
        /// </summary>
        [DataMember]
        public Guid RefundId { get; set; }

        /// <summary>
        /// 退换货号
        /// </summary>
        [DataMember]
        public string RefundNo { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单用户
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
#if !WCFModel
        [Column("CreateDate")]
#endif
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 物流编号
        /// </summary>
        [DataMember]
        public string ExpressNo { get; set; }

        /// <summary>
        /// 物流名称
        /// </summary>
        [DataMember]
        public string ExpressName { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// 检查状态
        /// </summary>
        [DataMember]
        public int CheckState { get; set; }

        /// <summary>
        /// 管理意见
        /// </summary>
        [DataMember]
        public string Clew { get; set; }

        /// <summary>
        /// 检查备注（检查拒绝理由）
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 重启原因
        /// </summary>
        [DataMember]
        public string ReStartReason { get; set; }

        /// <summary>
        /// 退回仓库编号
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 检查公司
        /// </summary>
        [DataMember]
        public Guid CheckFilialeId { get; set; }

        /// <summary>
        /// 是否移交
        /// </summary>
        [DataMember]
        public bool IsTransfer { get; set; }
    }
}
