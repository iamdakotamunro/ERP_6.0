using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class GoodsPriceVerificationInfo
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 价格类型1 -〉 结算价；2 -〉 加盟价。
        /// </summary>
        public int PriceType { get; set; }

        /// <summary>
        /// 商品状态 0 -> 未确认； 1 -> 已确认；
        /// </summary>
        public int State { get; set; }
    }
}
