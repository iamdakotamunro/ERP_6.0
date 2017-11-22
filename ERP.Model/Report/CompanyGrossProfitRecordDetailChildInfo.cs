using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Report
{
    /// <summary>
    /// 公司毛利明细子表
    /// </summary>
    /// zal 2016-07-20
    [Serializable]
    public class CompanyGrossProfitRecordDetailChildInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 库存或者订单ID
        /// </summary>
        public Guid StockAndOrderId { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        ///商品数量 
        /// </summary>
        public int Quantity { get; set; }

        #region 不对应数据库字段
         /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }
        #endregion
    }
}
