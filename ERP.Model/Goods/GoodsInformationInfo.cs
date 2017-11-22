using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品资料表
    /// </summary>
    [Serializable]
    public class GoodsInformationInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 商品ID、供应商ID
        /// </summary>
        public Guid IdentifyId { get; set; }

        /// <summary>
        /// 商品资质、供应商资质类型名称
        /// 商品品牌资质(资料名称)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 往来单位名称、商品名称
        /// </summary>
        public string IdentifyName { get; set; }

        /// <summary>
        /// 资质类型
        /// </summary>
        public int QualificationType { get; set; }

        /// <summary>
        /// 证书号码 、 
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
        /// 判断是否新增 1为新增
        /// </summary>
        public int IsNew { get; set; }

        /// <summary>
        /// 授权公司
        /// </summary>
        public Guid FilialeId { get; set; }
    }
}
