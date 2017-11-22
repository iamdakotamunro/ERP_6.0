using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class InternalPriceSetInfo
    {
        /// <summary>商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>商品类型
        /// </summary>
        public String GoodsTypeName { get; set; }

        /// <summary>公司ID
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>利润比例
        /// </summary>
        public String ReserveProfitRatio { get; set; }
    }
}
