using System;

namespace ERP.Model
{
    /// <summary>
    /// 供应商或商品类
    /// </summary>
    [Serializable]
    public class SupplierGoodsInfo
    {
        /// <summary>
        /// 供应商或商品ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 供应商或商品名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 供应商或商品
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 供应商资质是否完整
        /// </summary>
        public int Complete { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        public string Expire { get; set; }
    }
}
