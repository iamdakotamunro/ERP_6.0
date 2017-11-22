using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位收付款审核权限表
    /// </summary>
    [Serializable]
    public class CompanyAuditingPowerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CompanyAuditingPowerInfo()
        {
            LowerMoney = 0;
            UpperMoney = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerid">权限ID</param>
        /// <param name="uppermoney">审核上限金额</param>
        /// <param name="lowermoney">审核下限金额</param>
        /// <param name="companyid">往来单位ID</param>
        /// <param name="filialeid">子公司ID</param>
        /// <param name="branchid">部门ID</param>
        /// <param name="positionid">职位ID</param>
        /// <param name="bindingtype">绑定类型</param>
        /// <param name="parentPowerID">所属直接权限（扩展权限使用，直接权限为Empty）</param>
        public CompanyAuditingPowerInfo(Guid powerid, decimal uppermoney, decimal lowermoney, Guid companyid, Guid filialeid, Guid branchid, Guid positionid, int bindingtype, Guid parentPowerID)
        {
            PowerID = powerid;
            UpperMoney = uppermoney;
            LowerMoney = lowermoney;
            CompanyID = companyid;
            FilialeId = filialeid;
            BranchID = branchid;
            PositionID = positionid;
            BindingType = bindingtype;
            ParentPowerID = parentPowerID;
        }


        #region --属性

        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PowerID { get; set; }

        /// <summary>
        /// 审核上限金额
        /// </summary>
        public decimal UpperMoney { get; set; }

        /// <summary>
        /// 审核下限金额
        /// </summary>
        public decimal LowerMoney { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 子公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// 职位ID
        /// </summary>
        public Guid PositionID { get; set; }

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
