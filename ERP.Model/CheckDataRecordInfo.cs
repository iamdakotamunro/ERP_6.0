using System;

namespace ERP.Model
{
    /// <summary>
    /// 快递对账记录模型
    /// </summary>
    [Serializable]
    public class CheckDataRecordInfo
    {
        /// <summary>
        /// 对账Id
        /// </summary>
        public Guid CheckId { get; set; }

        /// <summary>
        /// 快递原始文件(.xls .csv)
        /// </summary>
        public string OriginalFilePath { get; set; }

        /// <summary>
        /// 校对文件(.xls .csv)
        /// </summary>
        public string ContrastFilePath { get; set; }

        /// <summary>
        /// 确认文件(.xls .csv)
        /// </summary>
        public string ConfirmFilePath { get; set; }

        /// <summary>
        /// 完成对账文件(.xls .csv)
        /// </summary>
        public string FinishFilePath { get; set; }

        /// <summary>
        /// 对账类型(1，快递运费对账，2，快递代收对账)
        /// </summary>
        public int CheckType { get; set; }

        /// <summary>
        /// 对账的快递公司Id
        /// </summary>
        public Guid CheckCompanyId { get; set; }

        /// <summary>
        /// 对账的快递公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 对账创建日期
        /// </summary>
        public DateTime CheckCreateDate { get; set; }

        /// <summary>
        /// 对账公司
        /// </summary>
        public Guid CheckFilialeId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string CheckDescription { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public Guid CheckPersonnelId { get; set; }

        /// <summary>
        /// 对账人
        /// </summary>
        public string CheckUser { get; set; }

        /// <summary>
        /// 对账处理状态
        /// </summary>
        public int CheckDataState { get; set; }
    }
}
