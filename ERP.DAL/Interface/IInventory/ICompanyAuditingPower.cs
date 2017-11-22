using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

/*
 * 创建人：刘彩军
 * 创建时间：2011-June-08th
 * 文件作用:往来单位收付款审核权限接口
 */
namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyAuditingPower
    {

        /// <summary>
        /// 获取所有往来单位绑定的权限
        /// </summary>
        /// <returns></returns>
        IList<CompanyAuditingPowerInfo> GetALLCompanyAuditingPower();

        /// <summary>
        /// 添加往来单位收付款审核权限
        /// </summary>
        /// <param name="info">权限模型</param>
        void InsertCompanyAuditingPower(CompanyAuditingPowerInfo info);

        /// <summary>
        /// 根据往来单位获取所绑定的权限
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        IList<CompanyAuditingPowerInfo> GetCompanyAuditingPowerByCompanyID(Guid companyId);

        /// <summary>
        /// 根据登录人所在公司、部门、职务获取往来单位权限
        /// </summary>
        /// <param name="filialeId">公司</param>
        /// <param name="branchId">部门</param>
        /// <param name="positionId">职务</param>
        /// <returns></returns>
        /// zal 2016-04-27
        IList<CompanyAuditingPowerInfo> GetCompanyAuditingPower(Guid filialeId, Guid branchId,Guid positionId);

        /// <summary>
        /// 修改往来单位收付款审核权限
        /// </summary>
        /// <param name="info">权限模型</param>
        /// <param name="updateType">修改模式：0按权限ID修改，1按所属权限ID修改</param>
        void UpdateCompanyAuditingPower(CompanyAuditingPowerInfo info, int updateType);

        /// <summary>
        /// 根据所属权限ID获取所绑定的权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        /// <returns></returns>
        IList<CompanyAuditingPowerInfo> GetCompanyAuditingPowerByPowerID(Guid powerId);

        /// <summary>
        /// 删除往来单位收付款审核权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        void DeleteCompanyAuditingPower(Guid powerId);
    }
}
