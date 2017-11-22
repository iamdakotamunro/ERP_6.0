using System;
using System.Collections.Generic;

namespace ERP.Model
{
    /// <summary>
    /// add by liangcanren 2015-04-16
    /// for 采购额度设置 数据绑定
    /// </summary>
    [Serializable]
    public class TicketLimitBindInfo
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ComapnyName { get; set; }

        public Dictionary<Guid,decimal> LimitSettingsList { get; set; }
    }
}
