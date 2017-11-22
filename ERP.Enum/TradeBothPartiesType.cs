using ERP.Enum.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Enum
{
    /// <summary>
    /// 交易双方类型
    /// </summary>
    public enum TradeBothPartiesType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Enum("其他")]
        Other = 0,

        /// <summary>
        /// 物流配送公司对销售公司
        /// </summary>
        [Enum("物流配送公司对销售公司")]
        HostingToSale = 1,

        /// <summary>
        /// 物流配送公司之间
        /// </summary>
        [Enum("物流配送公司之间")]
        HostingToHosting =2
    }
}
