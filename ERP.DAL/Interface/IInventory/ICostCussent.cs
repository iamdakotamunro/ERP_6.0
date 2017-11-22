using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 往来单位信息接口类
    /// </summary>
    public interface ICostCussent
    {

        /// <summary>
        /// 或者往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        String GetCussentExtendInfo(Guid companyId);

        /// <summary>
        /// 更新往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="extend"></param>
        /// <returns></returns>
        void UpDatetCussentExtendInfo(Guid companyId, String extend);

        /// <summary>
        /// 获取往来费用最新结余
        /// </summary>
        /// <param name="companyClassId"></param>
        /// <returns></returns>
        decimal GetNonceCostByClassId(Guid companyClassId);

        /// <summary>
        /// 获取往来费用最新结余
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        decimal GetNonceCost(Guid companyId);

        /// <summary>
        /// 往来单位总帐计算
        /// </summary>
        /// <param name="companyClassId">分类ID</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="parentCompanyClassId">父类ID</param>
        /// <param name="assumeFilialeId">结算公司Id</param>
        /// <returns></returns>
        Double GetCussentCount(Guid companyClassId, Guid companyId, Guid parentCompanyClassId, Guid assumeFilialeId);
        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        void Insert(CostCussentInfo companyCussent);

        /// <summary>
        /// 更新往来单位
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        void Update(CostCussentInfo companyCussent);

        /// <summary>
        /// 删除往来单位信息
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        void Delete(Guid companyId);

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        CostCussentInfo GetCompanyCussent(Guid companyId);

        /// <summary>
        /// 获取往来单位信息列表
        /// </summary>
        /// <returns></returns>
        IList<CostCussentInfo> GetCompanyCussentList();

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        IList<CostCussentInfo> GetCompanyCussentList(CompanyType companyType);

        /// <summary>
        /// 获取指定类型，指定状态的往来单位信息列表
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CostCussentInfo> GetCompanyCussentList(CompanyType companyType, State state);

        /// <summary>
        /// 获取指定状态的往来单位记录
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CostCussentInfo> GetCompanyCussentList(State state);

        ///// <summary>
        ///// 获取指定分类号的往来单位列表
        ///// </summary>
        ///// <param name="companyClassId">往来单位分类编号</param>
        ///// <param name="filialeId"></param>
        ///// <param name="branchId"></param>
        ///// <param name="positionId"></param>
        ///// <returns></returns>
        //IList<CostCussentInfo> GetCompanyCussentList(Guid companyClassId, Guid filialeId, Guid branchId, Guid positionId);
        /// <summary>
        /// 获取指定分类号的往来单位列表
        /// </summary>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <returns></returns>
        IList<CostCussentInfo> GetCompanyCussentList(Guid companyClassId);

        /// <summary>
        /// 获取会员总帐
        /// </summary>
        /// <returns></returns>
        CostCussentInfo GetMemberGeneralLedger();

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        double GetNonceReckoningTotalled(Guid companyId);

        /// <summary>
        /// 判断是否包含该公司
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <returns></returns>
        bool IsBeing(string companyName);

        /// <summary>
        /// 判断是否为快递公司往来帐
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns></returns>
        bool IsExpress(Guid companyId);

        /// <summary>
        /// 判断是否为会员总帐号
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns></returns>
        bool IsMemberGeneralLedger(Guid companyId);

        /// <summary>
        /// 会员总帐是否被使用
        /// </summary>
        /// <returns></returns>
        bool IsUseMemberGeneralLedger();

        #region 费用单位权限开放

        void AddCussionPersion(Guid companyCussentId, Guid filialeId, Guid branchId);

        void DeleteCussionPersion(Guid companyCussentId, Guid filialeId, Guid branchId);

        IEnumerable<CostPermissionInfo> GetCostPermissionList(Guid filialeId, Guid branchId, Guid costCompanyId);
        /// <summary>
        /// 根据单位ID删除该单位相关的所有权限
        /// Add by Liucaijun at 2010-january-07th
        /// 删除单位时候使用，删除该单位相关的所有权限
        /// </summary>
        /// <param name="companyCussentId">单位ID</param>
        void DeleteCussionPersion(Guid companyCussentId);
        #endregion

        /// <summary>根据费用分类ID获取具有相应权限的费用单位  ADD  2014-12-09  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="companyClassId">费用分类ID</param>
        /// <returns></returns>
        IList<CostCussentInfo> GetPermissionCompanyCussentList(Guid filialeId, Guid branchId, Guid companyClassId);

        /// <summary>获取费用分类绑定的公司资金帐号   ADD  2015-01-20  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="costCompanyId">费用分类ID</param>
        /// <returns></returns>
        CostCompanyBindingBankAccountsInfo GetCostCompanyBindingBankAccountsInfo(Guid filialeId, Guid costCompanyId);

        /// <summary>更新费用分类绑定的公司资金帐号（含新增）   ADD  2015-01-20  陈重文
        /// </summary>
        /// <param name="info"> 费用分类绑定公司资金账户模型</param>
        /// <returns></returns>
        Boolean InsertOrUpdateCostCompanyBindingBankAccountsInfo(CostCompanyBindingBankAccountsInfo info);
    }
}