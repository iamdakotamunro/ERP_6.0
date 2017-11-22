using System;
//================================================
// 功能：费用审批分类实体类
// 作者：刘彩军
// 时间：2010-11-1
//================================================
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用审批分类实体类
    /// </summary>
    public class ReportClassInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ReportClassInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportClassId">费用审批类别ID</param>
        /// <param name="reportClassName">费用审批类别名称</param>
        /// <param name="minCost">最小审批金额</param>
        /// <param name="maxCost">最大审批金额</param>
        /// <param name="executeFilialeId">最后执行人公司ID</param>
        /// <param name="executeBranchId">最后执行人部门ID</param>
        /// <param name="executePositionId">最后执行人职务ID</param>
        public ReportClassInfo(Guid reportClassId, string reportClassName, Decimal minCost, Decimal maxCost, Guid executeFilialeId, Guid executeBranchId, Guid executePositionId)
        {
            ReportClassId = reportClassId;
            ReportClassName = reportClassName;
            MinCost = minCost;
            MaxCost = maxCost;
            ExecuteFilialeId = executeFilialeId;
            ExecuteBranchId = executeBranchId;
            ExecutePositionId = executePositionId;
        }

        /// <summary>
        /// 费用审批类别ID
        /// </summary>
        public Guid ReportClassId { get; set; }

        /// <summary>
        /// 费用审批类别名称
        /// </summary>
        public string ReportClassName { get; set; }

        /// <summary>
        /// 最小审批金额
        /// </summary>
        public Decimal MinCost { get; set; }

        /// <summary>
        /// 最大审批金额
        /// </summary>
        public Decimal MaxCost { get; set; }

        /// <summary>
        /// 最后执行人公司ID
        /// </summary>
        public Guid ExecuteFilialeId { get; set; }

        /// <summary>
        /// 最后执行人部门ID
        /// </summary>
        public Guid ExecuteBranchId { get; set; }

        /// <summary>
        /// 最后执行人职务ID
        /// </summary>
        public Guid ExecutePositionId { get; set; }
    }
}
