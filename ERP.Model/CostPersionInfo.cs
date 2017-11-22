using System;

namespace ERP.Model
{
    /// <summary>
    /// 费用单位权限开放类
    /// </summary>
    [Serializable]
    public class CostPermissionInfo
    {
        /// <summary>
        /// 费用单位ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 费用单位名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string BranchName { get; set; }
    }
}
