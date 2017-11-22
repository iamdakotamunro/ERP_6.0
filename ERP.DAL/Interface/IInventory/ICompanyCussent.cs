using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;
using ERP.SAL.WMS;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 往来单位信息接口类
    /// </summary>
    public interface ICompanyCussent
    {
        /// <summary>
        /// 或者往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        string GetCussentExtendInfo(Guid companyId);

        /// <summary>
        /// 更新往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="extend"> </param>
        /// <returns></returns>
        void UpDatetCussentExtendInfo(Guid companyId, string extend);

        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        void Insert(CompanyCussentInfo companyCussent);

        /// <summary>
        /// 更新往来单位
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        void Update(CompanyCussentInfo companyCussent);

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
        CompanyCussentInfo GetCompanyCussent(Guid companyId);

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="filialeId">绑定公司Id</param>
        /// <returns></returns>
        /// zal 2016-03-16
        CompanyCussentInfo GetCompanyCussentInfoByCompanyIdAndFilialeId(Guid companyId, Guid filialeId);

        /// <summary>
        /// 获取往来单位信息列表
        /// </summary>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList();

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType);

        /// <summary>
        /// 获取指定类型，指定状态的往来单位信息列表
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType, State state);

        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType[] companyType, State state);

        /// <summary>
        /// 获取指定状态的往来单位记录
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(State state);

        /// <summary>
        /// 获取指定分类号的往来单位列表
        /// </summary>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(Guid companyClassId);

        /// <summary>
        /// 通过往来单位名称获取往来单位列表  模糊搜索
        /// </summary>
        /// <param name="companyName">往来单位分类编号</param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentListByCompanyName(string companyName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyClassId"></param>
        /// <returns></returns>
        IList<Guid> GetCompanyIdList(Guid companyClassId);

        /// <summary>
        /// 获取会员总帐
        /// </summary>
        /// <returns></returns>
        CompanyCussentInfo GetMemberGeneralLedger();

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        double GetNonceReckoningTotalled(Guid companyId);

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <param name="filialeId"> </param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        double GetNonceReckoningTotalled(Guid companyId, Guid filialeId);

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

        IList<CompanyCussentInfo> GetCompanyCussentListByPersion(Guid companyClassId, Guid persionID);

        /// <summary>
        /// 更新往来单位折扣信息
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="discountMemo">折扣信息</param>
        /// <returns></returns>
        void UpDatetCussentDiscountMemoInfo(Guid companyId, string discountMemo);

        /// <summary>
        /// 根据往来单位获取折扣信息
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        string GetCussentDiscountMemo(Guid companyId);

        /// <summary>
        /// 添加往来单位对应我方银行账号信息
        /// </summary>
        void InsertCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>
        /// 更新往来单位对应我方银行账号信息
        /// </summary>
        void UpdateCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>
        /// 获取所有往来单位对应我方银行账号信息
        /// </summary>
        /// <returns></returns>
        IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList();

        IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList(Guid companyId);

        /// <summary>
        /// 删除指定往来单位对应我方银行账号信息
        /// </summary>
        /// <returns></returns>
        void DelCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>根据公司ID找出对应的绑定的往来单位
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentByFilialeId(Guid filialeId);

        /// <summary>CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        IList<CompanyBalanceDetailInfo> GetCompanyBalanceDetailList();

        /// <summary>
        ///  查找供应商往来余额非0的往来公司 CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetCompanyBalanceDetailFilialeIdList(Guid companyId);

        /// <summary>CompanyBalance
        /// </summary>
        /// <returns></returns>
        IList<CompanyBalanceInfo> GetCompanyBalanceList();

        /// <summary>保存供应商绑定到公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        bool SaveCompanyBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>删除供应商绑定到公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        bool DeleteCompanyBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>供应是否绑定该公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        bool GetCompanyIsBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>获取供应商绑定的公司Ids
        /// </summary>
        /// <param name="companyId">供应商Id</param>
        /// <returns></returns>
        IList<Guid> GetCompanyBindingFiliale(Guid companyId);

        /// <summary>获取供应商应付款 ADD 2015-03-12  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyName">供应商名称</param>
        /// <param name="year">年份</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetCompanyPaymentDaysList(Guid filialeId, string companyName, int year, bool initData);

        /// <summary>获取没有设置账期的供应商当月应付款 ADD 2015-06-11  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyName">供应商名称</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetCompanyNotPaymentDaysList(Guid filialeId, string companyName, bool initData);


        /// <summary>  新加获取无账期数据 add by liangcanren at 2015-08-10
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        CompanyPaymentDaysInfo GetCompanyNotPaymentDaysInfos(Guid filialeId, int year);


        /// <summary>
        /// 获取采购供应商入库金额明细 ADD 2015-08-07  梁灿仁
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetPurchasingCompanyPaymentDaysInfos(Guid filialeId, int year, string companyName);

        /// <summary>获取采购入库金额 ADD 2015-08-07  梁灿仁
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        CompanyPaymentDaysInfo GetPurchasingCompanyPaymentDaysInfo(Guid filialeId, int year);

        /// <summary>判断此往来单位是否存在 ADD 2015-03-14 陈重文
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        Boolean IsExistCompanyInfo(Guid companyId);

        /// <summary>
        /// 更新供应商资质是否完整
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="completeState">是否完整</param>
        /// <param name="expire">是否过期</param>
        Boolean UpdateQualificationCompleteState(Guid companyId, int completeState,string expire);

        /// <summary>
        /// 供应商资质数据查询
        /// </summary>
        /// <param name="companyType">往来单位类型</param>
        /// <param name="state">往来单位状态</param>
        /// <param name="searchKey">搜索关键字(名称)</param>
        /// <param name="complete">是否完整</param>
        /// <param name="expire">是否过期</param>
        /// <returns></returns>
        IList<SupplierGoodsInfo> GetSupplierGoodsInfos(CompanyType companyType, State state,string searchKey,int complete,int expire);

        /// <summary>
        /// 获取往来单位字典
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        IDictionary<Guid, String> GetCompanyDic();


        /// <summary>判断此往来单位是否被搁置
        /// </summary>
        /// <param name="thirdCompanyID">第三方公司ID</param>
        /// <returns></returns>
        Boolean IsAbeyanced(Guid thirdCompanyID);

        /// <summary>获取已被关联的公司列表 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetRelevanceFilialeIdList();

        /// <summary>
        /// 通过关联公司获取对应的往来单位
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        CompanyCussentInfo GetCompanyByRelevanceFilialeId(Guid filialeId);

        /// <summary>
        /// 根据关联公司获取往来单位ID
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        Guid GetCompanyIdByRelevanceFilialeId(Guid filialeId);

        /// <summary>
        /// 根据关联公司获取往来单位ID列表
        /// </summary>
        /// <param name="filialeIds"></param>
        /// <returns></returns>
        List<PurchaseFilialeAuth> GetCompanyIdNameListByRelevanceFilialeIds(IEnumerable<Guid> filialeIds);

        /// <summary>
        /// 通过往来单位id获取关联的公司id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Guid GetRelevanceFilialeIdByCompanyId(Guid companyId);


        Dictionary<Guid, Guid> GetGoodsAndCompanyDic(IEnumerable<Guid> goodsId);

        Dictionary<Guid, String> GetAbroadCompanyList();
    }
}