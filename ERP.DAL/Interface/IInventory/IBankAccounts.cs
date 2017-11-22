using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using ERP.Model.ShopFront;

namespace ERP.DAL.Interface.IInventory
{
    public interface IBankAccounts
    {
        /// <summary>
        /// 添加资金帐号
        /// </summary>
        /// <param name="bankAccounts">帐号实例</param>
        void Insert(BankAccountInfo bankAccounts);

        /// <summary>
        /// 更新资金帐号
        /// </summary>
        /// <param name="bankAccounts">帐号实例</param>
        void Update(BankAccountInfo bankAccounts);

        /// <summary>
        /// 删除资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        void Delete(Guid bankAccountsId);
               /// <summary>
        /// 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        double GetBankAccountsNonce(Guid bankAccountsId);
        /// <summary>
        /// 获取资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        /// <returns></returns>
        BankAccountInfo GetBankAccounts(Guid bankAccountsId);

        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(Guid filialeId, Guid branchId, Guid positionId);
        /// <summary>
        /// 获取所有资金帐号列表
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList();

        /// <summary>
        /// 获取资金帐号列表不关联BankAccountBinding
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetList();

        /// <summary>
        /// 获取所有资金帐号列表
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsNoBindingList();

        /// <summary>
        /// 获取指定支付类型的帐号
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(PaymentType paymentType);

        /// <summary>
        /// 获取指定支付类型组的帐号
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(PaymentType[] paymentTypes);

        /// <summary>
        /// 是否在前台陈列范围内
        /// </summary>
        /// <param name="bankAccountsId">银行编号</param>
        /// <returns></returns>
        bool IsFace(Guid bankAccountsId);

        /// <summary>
        /// 获取当前顺序号
        /// </summary>
        /// <returns></returns>
        int GetOrderIndex();

        /// <summary>
        /// 更新银行帐号序号
        /// </summary>
        /// <param name="bankAccountsId">银行帐号ID</param>
        /// <param name="orderIndex">序号</param>
        /// <returns></returns>
        /// zal 2015-09-22
        bool UpdateOrderIndex(Guid bankAccountsId,int orderIndex);
        
        /// <summary>
        /// Func : 得到银行以及其接口名字
        /// Coder: dyy
        /// Date : 2010 Jan.4th
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, String> GetBankInterface();
        /// <summary>
        /// 根据银行获取该操作银行的用户
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeBankAccoutid"></param>
        /// <returns></returns>
        IList<BankAccountPermissionInfo> GetBankPersionByBankId(Guid filialeId, Guid branchId, Guid postionId, Guid bankAccountsId, Guid filialeBankAccoutid);

        IEnumerable<BankAccountPermissionInfo> GetPermissionList(Guid bankAccountId);

        #region 删除权限
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        void DeleteBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId);

        /// <summary>
        /// Add by Liucaijun at 2010-january-07th
        /// 删除银行账号时使用,删除该账号相关的所有权限
        /// </summary>
        void DeleteBankPersion(Guid bankAccountsId);
        #endregion

        #region 增加权限
        /// <summary>
        /// 增加权限
        /// </summary>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        void AddBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId);
        #endregion

        /// <summary>
        /// 根据公司ID、部门ID、银行ID、职务ID获取相关的权限
        /// Add by liucaijun at 2011-January-30th
        /// </summary>
        /// <returns></returns>
        BankAccountPermissionInfo GetPersonnelBankAccountsList(Guid bankId, Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// 获取绑定的银行帐号
        /// Add by liucaijun at 2011-August-10th
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBindBankAccounts();

        /// <summary>
        /// 插入公司银行账户绑定关系
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="bankAccountsId"></param>
        /// <returns></returns>
        bool InsertBindBankAccounts(Guid filialeId, Guid bankAccountsId);

        /// <summary>
        /// 获取银行余额列表
        /// </summary>
        /// <returns></returns>
        IList<BankAccountBalanceInfo> GetBalanceList();

        /// <summary>根据订单销售公司或平台ID获取对应有权限的银行账号列表
        /// </summary>
        /// <param name="targetId">公司或平台ID</param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(Guid targetId, Guid filialeId, Guid branchId, Guid positionId);

        
        /// <summary>
        /// 根据店铺获取帐户(用于联盟店往来单位收付款)
        /// </summary>
        /// <returns></returns>
        IList<ShopBankAccountsInfo> GetBankAccountsByShopId();
        
        /// <summary>设置银行账号是否是主账号
        /// </summary>
        /// <param name="bankAccountsId">银行账号ID</param>
        /// <param name="isMain">是否主账号</param>
        /// <returns></returns>
        bool SetBankAccountsIsMain(Guid bankAccountsId, bool isMain);

        /// <summary>获取所有非主帐号信息含账户余额（资金流页面用） ADD 2015-03-03  陈重文
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsListByNotIsMain();

        /// <summary>获取有权限的非主帐号信息含账户余额 2015-05-05  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="positionId">职务ID</param>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsListByNotIsMain(Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>获取所有银行帐号的余额  2015-05-15  陈重文
        /// </summary>
        /// <returns></returns>
        double GetBankAccountsAllNonceBalance();
    }
}
