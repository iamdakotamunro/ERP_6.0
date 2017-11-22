using System;

namespace ERP.Model
{
    /// <summary>
    /// 供应商资质表
    /// </summary>
    [Serializable]
    public class SupplierInformationInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 商品ID或品牌ID
        /// </summary>
        public Guid IdentifyId { get; set; }

        /// <summary>
        /// 资质类型
        /// </summary>
        public int QualificationType { get; set; }

        /// <summary>
        /// 证书号码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime? UploadDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? OverdueDate { get; set; }

        /// <summary>
        /// 资料类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string ExtensionName { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 判断是否是新加资质 1 新增
        /// </summary>
        public int IsNew { get; set; }
    }
}
