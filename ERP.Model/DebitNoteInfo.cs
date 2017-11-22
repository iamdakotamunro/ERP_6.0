using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 借记单
    /// </summary>
    [Serializable]
    public class DebitNoteInfo
    {
        /// <summary>
        /// 借记单
        /// </summary>
        public DebitNoteInfo()
        {
        }

        /// <summary>
        /// 采购单ID
        /// </summary>
        public Guid PurchasingId { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public string PurchasingNo { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 赠品总价
        /// </summary>
        public decimal PresentAmount { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// 借记单状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 责任人ID
        /// </summary>
        public Guid PersonResponsible { get; set; }

        /// <summary>
        /// 新采购单ID
        /// </summary>
        public Guid NewPurchasingId { get; set; }

        /// <summary>采购分组ID
        /// </summary>
        public Guid? PurchaseGroupId { get; set; }

        /// <summary>赠品借记单标题（手动添加才有）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        /// zal 2016-03-02
        public DateTime? ActivityTimeStart { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        /// zal 2016-03-02
        public DateTime? ActivityTimeEnd { get; set; }
    }
}
