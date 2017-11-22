using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// Edit 阮剑锋 2013.9.13 加入新方法，获取可审核的部门信息 => 
    /// </summary>
    public interface ICostReportAuditingPower
    {
        /// <summary>
        /// 获取可受理的部门信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<string> GetCanAuditingBranchId(Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// 获取可受理的部门信息
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<CostReportAuditingInfo> GetCanAuditingInfo(Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        IList<CostReportAuditingInfo> GetPowerList();

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="info">权限模型</param>
        void InsertPower(CostReportAuditingInfo info);

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="info">权限模型</param>
        void UpdatePower(CostReportAuditingInfo info);

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        void DeletePower(Guid powerId);

        /// <summary>
        /// 申报部门权限
        /// </summary>
        /// <param name="info">权限模型</param>
        void UpdateReportPower(CostReportAuditingInfo info);

    }
}
