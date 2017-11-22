using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

/*
 * 创建人：刘彩军
 * 创建时间：2011-June-09th
 * 文件作用:往来单位收付款发票权限接口
 */
namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyInvoicePower
    {
        /// <summary>
        /// 获取所有往来单位绑定的权限
        /// </summary>
        /// <returns></returns>
        IList<CompanyInvoicePowerInfo> GetALLCompanyInvoicePower();

        /// <summary>
        /// 添加往来单位收付款发票权限
        /// </summary>
        /// <param name="info">权限模型</param>
        void InsertCompanyInvoicePower(CompanyInvoicePowerInfo info);

        /// <summary>
        /// 根据往来单位获取所绑定的权限
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        IList<CompanyInvoicePowerInfo> GetCompanyInvoicePowerByCompanyID(Guid companyId);

        /// <summary>
        /// 修改往来单位收付款发票权限
        /// </summary>
        /// <param name="info">权限模型</param>
        /// <param name="updateType">修改模式：0按权限ID修改，1按所属权限ID修改</param>
        void UpdateCompanyInvoicePower(CompanyInvoicePowerInfo info, int updateType);
        
        /// <summary>
        /// 根据所属权限ID获取所绑定的权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        /// <returns></returns>
        IList<CompanyInvoicePowerInfo> GetCompanyInvoicePowerByPowerID(Guid powerId);

        /// <summary>
        /// 删除往来单位收付款发票权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        void DeleteCompanyInvoicePower(Guid powerId);
    }
}
