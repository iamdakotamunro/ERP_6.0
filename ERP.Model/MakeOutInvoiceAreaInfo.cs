using System;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一 订单开票地区绑定模型   ADD 2014-12-22  陈重文
    /// </summary>
    [Serializable]
    public class MakeOutInvoiceAreaInfo
    {
        /// <summary>省份ID
        /// </summary>
        public Guid ProvinceId { get; set; }

        /// <summary>城市ID
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>必开发票和选开发票  1.必开发票  2选开发票   0未设置
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeId { get; set; }
    }
}
