using System;
//================================================
// 功能：费用申报票据实体类
// 作者：刘彩军
// 时间：2011-February-23th
//================================================
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用申报图片信息
    /// </summary>
    [Serializable]
    public class CostReportInvoiceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CostReportInvoiceInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId">单据ID</param>
        /// <param name="imagePath">单据图片路径</param>
        /// <param name="state">状态</param>
        /// <param name="realityCost">实际金额</param>
        /// <param name="memo">备注</param>
        public CostReportInvoiceInfo(Guid invoiceId, string imagePath, int state, decimal realityCost, String memo)
        {
            InvoiceId = invoiceId;
            ImagePath = imagePath;
            State = state;
            RealityCost = realityCost;
            Memo = memo;
        }

        /// <summary>
        ///单据ID
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        ///单据图片路径
        /// </summary>
        public String ImagePath { get; set; }

        /// <summary>
        ///状态
        /// </summary>
        public Int32 State { get; set; }

        /// <summary>
        /// 实际金额
        /// </summary>
        public Decimal RealityCost { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Memo { get; set; }
    }
}
