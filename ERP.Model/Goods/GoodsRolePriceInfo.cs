using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsRolePriceInfo
    {
        /// <summary>
        /// 商品组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 销售平台ID
        /// </summary>
        public Guid SalePlatformID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 会员角色ID
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 会员角色ID
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount { get; set; }
    }
}
