namespace ERP.Cache.Common
{
    public enum Key
    {
        #region -- Old
        /// <summary>
        /// 所有的活动列表
        /// </summary>
        AllActivityList,

        /// <summary>
        /// 零点交易汇率货币列表
        /// </summary>
        ZeroExchageRateCurrencyList,

        /// <summary>
        /// 银行接口列表
        /// </summary>
        BankInterfaceList,

        /// <summary>
        /// 银行账号列表
        /// </summary>
        AllBankAccountList,

        /// <summary>
        /// 仓库列表
        /// </summary>
        AllWareHouserList,

        /// <summary>
        /// 会员供应商信息
        /// </summary>
        MemberCompanyInfo,

        /// <summary>
        /// 城市列表
        /// </summary>
        AllCityList,

        /// <summary>
        /// 地区列表
        /// </summary>
        ALLDistrictList,

        /// <summary>
        /// 
        /// </summary>
        CityListByProvinceId_,

        /// <summary>
        /// 省份列表
        /// </summary>
        ProvinceList,

        /// <summary>
        /// 
        /// </summary>
        ProvinceListByCountryId_,

        /// <summary>
        /// 国家列表
        /// </summary>
        AllCountryList,

        /// <summary>
        /// 
        /// </summary>
        RootFiliale,



        /// <summary>
        /// 对应仓库的子公司列表
        /// </summary>
        FilialeListByWareHouserId,

        /// <summary>
        /// 分公司银行列表
        /// </summary>
        FilialeBankListByFilialeId_,

        /// <summary>
        /// 部分库存进度
        /// </summary>
        SemiStockProcess,

        /// <summary>
        /// 商品跟分类列表
        /// </summary>
        GoodsRootClassList,

        /// <summary>
        /// 
        /// </summary>
        WareHouserListFilialeId,

        /// <summary>
        /// 商品列表
        /// </summary>
        AllGoodsList,

        /// <summary>
        /// 所有商品跟分类列表
        /// </summary>
        AllGoodsRootClassList,

        /// <summary>
        /// 子公司目录树列表
        /// </summary>
        FilialeTreeList,

        /// <summary>
        /// 
        /// </summary>
        FilialeIdListByCityId_,

        /// <summary>
        /// 员工列表
        /// </summary>
        AllPersonnelList,

        /// <summary>
        /// 结算货币列表
        /// </summary>
        CurrencyInfoList,



        /// <summary>
        /// 所有往来单位收付款审核权限
        /// </summary>
        AllCompanyAuditingPower,

        /// <summary>
        /// 所有往来单位收付款发票权限
        /// </summary>
        AllCompanyInvoicePower,

        ///所有的站点信息
        WebSiteInfoList,

        /// <summary>
        /// 所有对外分站点
        /// </summary>
        BranchWeSiteInfoList,

        /// <summary>
        /// 本站站点信息
        /// </summary>
        LocalWebSiteInfo,

        /// <summary>
        /// 管理系统信息
        /// </summary>
        ManageSystemInfo,

        /// <summary>
        /// 资金帐号与网站关系集合
        /// </summary>
        BankWebSiteList,

        /// <summary>
        /// 公司集合
        /// </summary>
        FilialeList,

        /// <summary>
        /// 销售平台集合
        /// </summary>
        SalePlatformList,

        /// <summary>
        /// 部门集合
        /// </summary>
        BranchList,

        /// <summary>
        /// 职务集合
        /// </summary>
        PositionList,

        /// <summary>
        /// 职务集合
        /// </summary>
        GetAllFilialeBranchPositionList,

        /// <summary>
        /// 单个员工信息
        /// </summary>
        GetPersonnel,

        /// <summary>
        /// 有操作权限的仓库ID列表
        /// </summary>
        HasPermissionWarehouseIdList,
        #endregion

        /// <summary>
        /// 
        /// </summary>
        AllBankAccount,

        /// <summary>
        /// 所有往来账务公司
        /// </summary>
        AllRelatedCompany,

        /// <summary>
        /// 子公司列表
        /// </summary>
        AllFiliale,

        /// <summary>
        /// 所有系统职务
        /// </summary>
        AllSystemPosition,

        /// <summary>
        /// 所有系统部门
        /// </summary>
        AllSystemBranch,

        /// <summary>
        /// 快递
        /// </summary>
        AllExpress,

        /// <summary>
        /// 商品责任人绑定
        /// </summary>
        AllPurchseSet,
    }
}
