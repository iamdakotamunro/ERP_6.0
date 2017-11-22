using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 商品采购采购价或报备天数更改记录
    /// </summary>
    [Serializable]
    public class PurchaseSetLogInfo
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public Guid LogId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        ///// <summary>
        ///// 仓库ID
        ///// </summary>
        //public Guid WarehouseId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 采购价或报备天数修改前值
        /// </summary>
        public decimal OldValue { get; set; }

        /// <summary>
        /// 采购价或报备天数修改差值
        /// </summary>
        public decimal ChangeValue { get; set; }

        /// <summary>
        /// 采购价或报备天数修改后值
        /// </summary>
        public decimal NewValue { get; set; }

        /// <summary>
        /// 变更事由
        /// </summary>
        public string ChangeReason { get; set; }

        /// <summary>
        /// 变更日期
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public Guid Applicant { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Guid Auditor { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Statue { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int LogType { get; set; }

        /// <summary>
        /// 物流配送公司
        /// </summary>
        public Guid HostingFilialeId { get; set; }
    }
}
