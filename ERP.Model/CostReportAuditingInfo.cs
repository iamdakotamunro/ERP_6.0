using System;
//================================================
// 功能：费用审批权限实体类
// 作者：刘彩军
// 时间：2011-February-24th
//================================================
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用审批权限信息
    /// </summary>
    [Serializable]
    public class CostReportAuditingInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CostReportAuditingInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerId">权限ID</param>
        /// <param name="auditingFilialeId">审核公司ID</param>
        /// <param name="auditingBranchId">审核部门</param>
        /// <param name="auditingPositionId">审核职务</param>
        /// <param name="minAmount">审批金额范围（最小）</param>
        /// <param name="maxAmount">审批金额范围（最大）</param>
        /// <param name="reportBranchId">申报部门</param>
        /// <param name="description">描述</param>
        /// <param name="kind">类型（审核权限/票据受理权限）</param>
        public CostReportAuditingInfo(Guid powerId, Guid auditingFilialeId, Guid auditingBranchId, Guid auditingPositionId, Decimal minAmount, Decimal maxAmount, string reportBranchId, string description, int kind)
        {
            PowerId = powerId;
            AuditingFilialeId = auditingFilialeId;
            AuditingBranchId = auditingBranchId;
            AuditingPositionId = auditingPositionId;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            ReportBranchId = reportBranchId;
            Description = description;
            Kind = kind;
        }

        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PowerId { get; set; }

        /// <summary>
        ///审核公司ID
        /// </summary>
        public Guid AuditingFilialeId { get; set; }

        /// <summary>
        ///审核部门
        /// </summary>
        public Guid AuditingBranchId { get; set; }

        /// <summary>
        ///审核职务
        /// </summary>
        public Guid AuditingPositionId { get; set; }

        /// <summary>
        ///审批金额范围（最小）
        /// </summary>
        public Decimal MinAmount { get; set; }

        /// <summary>
        ///审批金额范围（最大）
        /// </summary>
        public Decimal MaxAmount { get; set; }

        /// <summary>
        ///申报部门
        /// </summary>
        public String ReportBranchId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 类型（审核权限/票据受理权限）
        /// </summary>
        public int Kind { get; set; }
    }
}
