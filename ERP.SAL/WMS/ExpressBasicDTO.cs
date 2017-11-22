using System;

namespace ERP.SAL.WMS
{
    public class ExpressBasicDTO
    {
        /// <summary>仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>快递ID
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>快递公司全称
        /// </summary>
        public string ExpressFullName { get; set; }

        /// <summary> 快递公司简称
        /// </summary>
        public string ExpressShortName { get; set; }

        /// <summary>支付方式
        /// </summary>
        public Byte PayMode { get; set; }

        /// <summary>
        /// 对账往来单位ID
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}
