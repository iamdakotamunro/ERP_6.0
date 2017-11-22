using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Model
{
    /// <summary>采购单拆分模型  ADD  2015-05-06  陈重文
    /// </summary>
    public class PurchasingGoodsSplitInfo
    {

        /// <summary>采购公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>总额度
        /// </summary>
        public Decimal TotalPurchasingLimit { get; set; }

        /// <summary>采购额度
        /// </summary>
        public Decimal PurchasingLimit { get; set; }

        /// <summary>剩余采购额度
        /// </summary>
        public Decimal SurplusPurchasingLimit
        {
            get
            {
                if (PurchasingGoodsItemList == null || PurchasingGoodsItemList.Count == 0)
                {
                    return PurchasingLimit;
                }
                return PurchasingLimit - PurchasingGoodsItemList.Sum(ent => ent.TotalGoodsAmount);
            }
        }

        /// <summary>是否其他公司
        /// </summary>
        public Boolean IsElseFilialeId { get; set; }

        /// <summary>拆分后采购单商品集合
        /// </summary>
        public IList<PurchasingGoodsItemInfo> PurchasingGoodsItemList { get; set; }
    }

    /// <summary>拆分后采购单商品集合
    /// </summary>
    public class PurchasingGoodsItemInfo
    {
        /// <summary>采购单商品CODE
        /// </summary>
        public String GoodsCode { get; set; }

        /// <summary>商品总金额
        /// </summary>
        public Decimal TotalGoodsAmount { get; set; }

        public Boolean IsOut { get; set; }
    }
}
