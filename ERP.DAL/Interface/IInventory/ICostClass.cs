using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICost
    {
        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyClass">往来单位分类类实例</param>
        void Insert(CostCompanyClassInfo companyClass);

        /// <summary>
        /// 更新往来单位信息
        /// </summary>
        /// <param name="companyClass">往来单位分类类实例</param>
        void Update(CostCompanyClassInfo companyClass);

        /// <summary>
        /// 删除往来单位分类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        void Delete(Guid companyClassId);

        /// <summary>
        /// 获取指定编号往来单位类实例
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        /// <returns></returns>
        CostCompanyClassInfo GetCompanyClass(Guid companyClassId);

        /// <summary>
        /// 获取指定编号的往来单位父类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        /// <returns></returns>
        CostCompanyClassInfo GetParentCompanyClass(Guid companyClassId);

        /// <summary>
        /// 获取往来单位列表
        /// </summary>
        /// <returns></returns>
        IList<CostCompanyClassInfo> GetCompanyClassList();

        /// <summary>
        /// 获取指定分类的子分类列表
        /// </summary>
        /// <param name="parentCompanyClassId">父分类编号</param>
        /// <returns></returns>
        IList<CostCompanyClassInfo> GetChildCompanyClassList(Guid parentCompanyClassId);

        /// <summary>
        /// 获取子分类数量
        /// </summary>
        /// <param name="companyClassId">分类编号</param>
        /// <returns>返回int型,子分类数量</returns>
        int GetChildCompanyClassCount(Guid companyClassId);

        /// <summary>
        /// 返回直接绑定到该分类的公司数量,不计子分类中公司的数量
        /// </summary>
        /// <param name="companyClassId">公司编号</param>
        /// <returns>返回int型,绑定公司分类数量</returns>
        int GetFireCompanyCount(Guid companyClassId);

        bool CanDelete(Guid companyClassId);

        /// <summary>获取具有权限的费用分类 ADD  2014-12-09 陈重文 
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <returns></returns>
        IList<CostCompanyClassInfo> GetPermissionCompanyClassList(Guid filialeId, Guid branchId);
    }
}
