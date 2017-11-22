using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位收付款发票权限表
    /// </summary>
    [Serializable]
    public class CompanyInvoicePowerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CompanyInvoicePowerInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerid">权限唯一ID</param>
        /// <param name="invoicestype">发票类型</param>
        /// <param name="companyid">往来单位ID</param>
        /// <param name="filialeid">子公司ID</param>
        /// <param name="branchid">部门ID</param>
        /// <param name="positionid">职位ID</param>
        /// <param name="auditorid">员工ID</param>
        /// <param name="bindingtype">绑定类型</param>
        /// <param name="parentPowerID">所属直接权限（扩展权限使用，直接权限为Empty）</param>
        public CompanyInvoicePowerInfo(Guid powerid, int invoicestype, Guid companyid, Guid filialeid, Guid branchid, Guid positionid, Guid auditorid, int bindingtype, Guid parentPowerID)
        {
            PowerID = powerid;
            InvoicesType = invoicestype;
            CompanyID = companyid;
            FilialeID = filialeid;
            BranchID = branchid;
            PositionID = positionid;
            AuditorID = auditorid;
            BindingType = bindingtype;
            ParentPowerID = parentPowerID;
        }

        #region --属性

        /// <summary>
        /// 权限唯一ID
        /// </summary>
        public Guid PowerID { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public int InvoicesType { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 子公司ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// 职位ID
        /// </summary>
        public Guid PositionID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public Guid AuditorID { get; set; }

        /// <summary>
        /// 绑定类型
        /// </summary>
        public int BindingType { get; set; }

        /// <summary>
        /// 所属直接权限（扩展权限使用，直接权限为Empty）
        /// </summary>
        public Guid ParentPowerID { get; set; }

        #endregion
    }
}
