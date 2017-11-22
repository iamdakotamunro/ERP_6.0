using System;
using System.Collections.Generic;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Factory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;


namespace ERP.BLL.Implement.Inventory
{
    public class BankAccountManager : BllInstance<BankAccountManager>
    {
        private readonly IBankAccounts _bankAccounts;
        private readonly IWasteBook _wasteBookDao;
        private readonly IBankAccountDao _bankAccountDao;
        private readonly Guid _reckoningElseFilialeid;

        public BankAccountManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _bankAccountDao = InventoryInstance.GetBankAccountDao(fromType);
            _bankAccounts = InventoryInstance.GetBankAccountsDao(fromType);
            _wasteBookDao = InventoryInstance.GetWasteBookDao(fromType);
            _reckoningElseFilialeid= new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        }

        public BankAccountManager(IBankAccounts bankAccounts,IBankAccountDao bankAccountDao,IWasteBook wasteBook)
        {
            _bankAccountDao = bankAccountDao;
            _bankAccounts = bankAccounts;
            _wasteBookDao = wasteBook;
            _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        }

        public BankAccountManager(IBankAccounts bankAccounts, IBankAccountDao bankAccountDao, IWasteBook wasteBook,Guid elseFilialeId)
        {
            _bankAccountDao = bankAccountDao;
            _bankAccounts = bankAccounts;
            _wasteBookDao = wasteBook;
            _reckoningElseFilialeid = elseFilialeId == Guid.Empty ? new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")) : elseFilialeId;
        }

        /// <summary>
        /// 根据资金帐号ID获取已关联公司账号的数据集合(即关联BankAccountBinding)
        /// </summary>
        /// <param name="bankaccountid"></param>
        /// <returns></returns>
        public IList<BankAccountInfo> GetList(Guid bankaccountid)
        {
            return _bankAccountDao.GetListByBankAccountId(bankaccountid);
        }
        
        public string GetBankAccountName(Guid accountId)
        {
            var info = BankAccount.Instance.Get(accountId);
            if (info == null)
            {
                return string.Empty;
            }
            return info.BankName;
        }
        
        public IList<BankAccountInfo> GetListByTargetId(Guid targetId)
        {
            return _bankAccountDao.GetListByTargetId(targetId);
        }
        
        /// <summary>
        /// 取得某银行当前金额
        /// </summary>
        /// <param name="bankAccountsId">银行ID</param>
        /// <returns></returns>
        public double GetBankAccountsNonce(Guid bankAccountsId)
        {
            return _bankAccounts.GetBankAccountsNonce(bankAccountsId);
        }

        /// <summary>
        /// 获取资金帐号
        /// ADD:2013.7.27 阮剑锋
        /// </summary>
        /// <param name="bankAccountId">帐号ID</param>
        /// <returns></returns>
        public BankAccountInfo Get(Guid bankAccountId)
        {
            return BankAccount.Instance.Get(bankAccountId);
        }

        /// <summary>
        /// 获取资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        /// <returns></returns>
        public BankAccountInfo GetBankAccounts(Guid bankAccountsId)
        {
            if (bankAccountsId == Guid.Empty) return new BankAccountInfo();
            return _bankAccounts.GetBankAccounts(bankAccountsId);
        }
        
        /// <summary>
        /// 获取资金帐号列表
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList()
        {
            return _bankAccounts.GetBankAccountsList();
        }
        
        /// <summary>
        /// 审核一笔账目for TransferForm.aspx.cs
        /// </summary>
        public void Auditing(string tradeCode)
        {
            try
            {
                _wasteBookDao.Auditing(tradeCode);
            }
            catch
            {
                throw new ApplicationException("审核失败！");
            }
        }

        public IEnumerable<BankAccountPermissionInfo> GetPermissionList(Guid bankAccountId)
        {
            return _bankAccounts.GetPermissionList(bankAccountId);
        }

        ///<summary>
        ///更改手续费
        /// </summary>
        public void UpdatePoundage(string tradeCode, decimal poundage)
        {
            DateTime dateTime = DateTime.Now;
            try
            {
                poundage = -poundage;
                _wasteBookDao.UpdatePoundage(tradeCode, poundage, dateTime);
            }
            catch
            {
                throw new ApplicationException("手续费更新失败！");
            }
        }
        
        #region 业务待添加单元测试
        /// <summary>
        /// 删除资金帐号
        /// </summary>
        /// <param name="bankAccountsId">付款帐号Id</param>
        public void Delete(Guid bankAccountsId)
        {
            if (Math.Round(_wasteBookDao.GetBalance(bankAccountsId), 3) == 0)
            {
                if (bankAccountsId != Guid.Empty)
                    _bankAccounts.Delete(bankAccountsId);
            }
            else
            {
                throw new ApplicationException("账户中仍有余额，不允许删除！");
            }
        }

        /// <summary>转帐
        /// </summary>
        /// <param name="inBankAccountsId">转入帐号Id</param>
        /// <param name="outBankAccountsId">转出帐号Id</param>
        /// <param name="sum">金额</param>
        /// <param name="poundage"></param>
        /// <param name="description">说明</param>
        /// <param name="tradeCode"></param>
        /// <param name="filialeId"> </param>
        public void Virement(Guid inBankAccountsId, Guid outBankAccountsId, decimal sum, decimal poundage, string description, string tradeCode, Guid filialeId)
        {
            if (inBankAccountsId != Guid.Empty && outBankAccountsId != Guid.Empty && sum > 0)
            {
                DateTime dateCreated = DateTime.Now;
                decimal inNonceBalance = _wasteBookDao.GetBalance(inBankAccountsId);
                decimal outNonceBalance = _wasteBookDao.GetBalance(outBankAccountsId);
                var outBankAccountsInfo = _bankAccounts.GetBankAccounts(outBankAccountsId);
                var intBankAccountsInfo = _bankAccounts.GetBankAccounts(inBankAccountsId);
                if (outBankAccountsInfo == null || intBankAccountsInfo==null) return;
                string inDescription = "[转入] [" + outBankAccountsInfo.BankName + "]" + description;
                var inWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), inBankAccountsId, tradeCode, dateCreated,
                                                        inDescription, sum, inNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Increase, filialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other
                                          };

                const string PD_DESC = "[转出] [手续费]";
                var pdWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                        PD_DESC, -poundage, outNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Decrease, filialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other
                                          };

                
                string outDescription = "[转出] [" + intBankAccountsInfo.BankName + "]" + description;
                var outWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                         outDescription, -sum, outNonceBalance, (int)AuditingState.No,
                                                         (int)WasteBookType.Decrease, filialeId)
                                           {
                                               LinkTradeCode = string.Empty,
                                               LinkTradeType = (int)WasteBookLinkTradeType.Other
                                           };

                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        _wasteBookDao.Insert(inWasteBookInfo);

                        if (poundage > 0)
                        {
                            _wasteBookDao.Insert(pdWasteBookInfo);
                        }

                        _wasteBookDao.Insert(outWasteBookInfo);
                        ts.Complete();
                    }
                    catch
                    {
                        throw new ApplicationException("转帐同步失败！");
                    }
                }
            }
        }


        /// <summary> 转帐   add chenzhongwen
        /// </summary>
        /// <param name="inBankAccountsId">转入帐号Id</param>
        /// <param name="outBankAccountsId">转出帐号Id</param>
        /// <param name="sum">金额</param>
        /// <param name="poundage">手续费</param>
        /// <param name="description">说明</param>
        /// <param name="tradeCode"></param>
        /// <param name="outFilialeId">转出公司 </param>
        /// <param name="inFilialeId"> 转入公司</param>
        /// <param name="realName"> 操作人</param>
        public WasteBookInfo NewVirement(Guid inBankAccountsId, Guid outBankAccountsId, decimal sum, decimal poundage, string description, string tradeCode, Guid outFilialeId, Guid inFilialeId, string realName)
        {
            WasteBookInfo wasteBookInfo = null;
            if (inBankAccountsId != Guid.Empty && outBankAccountsId != Guid.Empty && sum > 0)
            {
                var inBankInfo = _bankAccounts.GetBankAccounts(inBankAccountsId);
                var outBankInfo = _bankAccounts.GetBankAccounts(outBankAccountsId);
                if (inBankInfo == null || outBankInfo == null) return null;
                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime dateCreated = DateTime.Now;
                decimal inNonceBalance = _wasteBookDao.GetBalance(inBankAccountsId);
                decimal outNonceBalance = _wasteBookDao.GetBalance(outBankAccountsId);
                string inDescription;
                string outDescription;
                Boolean isOut;
                if (inFilialeId == _reckoningElseFilialeid || outFilialeId == _reckoningElseFilialeid)
                {
                    isOut = false;
                }
                else
                {
                    isOut = true;
                }
                if (outFilialeId == inFilialeId)
                {
                    inDescription = string.Format("[转入申请(来源银行:{0}；申请备注:{1}；转出申请人:{2} ；{3})]", outBankInfo.BankName + "-" + outBankInfo.AccountsName, realName, description, dateTime);
                    outDescription = string.Format("[转出申请(目标银行:{0}；申请备注:{1}；转出申请人:{2}；{3})]", inBankInfo.BankName + "-" + inBankInfo.AccountsName, description, realName, dateTime);
                }
                else
                {
                    var inFilialeName = inFilialeId == _reckoningElseFilialeid ? "其他公司" : FilialeManager.Get(inFilialeId).Name;
                    var outFilialeName = outFilialeId == _reckoningElseFilialeid ? "其他公司" : FilialeManager.Get(outFilialeId).Name;
                    inDescription = string.Format("[转入申请(来源公司:{0}；来源银行:{1}；申请备注:{2}；转出申请人:{3} ；{4})]", outFilialeName, outBankInfo.BankName + "-" + outBankInfo.AccountsName, realName, description, dateTime);
                    outDescription = string.Format("[转出申请(目标公司:{0}；目标银行:{1}；申请备注:{2}；转出申请人:{3}；{4})]", inFilialeName, inBankInfo.BankName + "-" + inBankInfo.AccountsName, description, realName, dateTime);
                }
                const string PD_DESC = "[转出] [手续费]";
                var pdWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                        PD_DESC, -poundage, outNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Decrease, outFilialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                              State = (int)WasteBookState.Currently,
                                              IsOut = isOut
                                          };
                var inWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), inBankAccountsId, tradeCode, dateCreated,
                                                        inDescription, sum, inNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Increase, inFilialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                              State = (int)WasteBookState.Currently,
                                              IsOut = isOut
                                          };
                var outWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                         outDescription, -sum, outNonceBalance, (int)AuditingState.No,
                                                         (int)WasteBookType.Decrease, outFilialeId)
                                           {
                                               LinkTradeCode = string.Empty,
                                               LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                               State = (int)WasteBookState.Currently,
                                               IsOut = isOut
                                           };
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        _wasteBookDao.Insert(inWasteBookInfo);

                        if (poundage > 0)
                        {
                            _wasteBookDao.Insert(pdWasteBookInfo);
                        }
                        _wasteBookDao.Insert(outWasteBookInfo);
                        wasteBookInfo = outWasteBookInfo;
                        ts.Complete();
                    }
                    catch
                    {
                        throw new ApplicationException("转帐同步失败！");
                    }
                }
            }
            return wasteBookInfo;
        }
        
        

        ///<summary>
        ///更改一笔账目
        /// </summary>
        public void UpdateBll(Guid outWasteBookId, string description, decimal income, string tradecode, decimal poundage, Guid bankAccountsId)
        {
            DateTime dateCreatedUpdate = DateTime.Now;
            var wasteBookInfo = _wasteBookDao.GetWasteBook(outWasteBookId);
            if(wasteBookInfo==null)return;
            string des =string.IsNullOrEmpty(wasteBookInfo.Description)?string.Empty:wasteBookInfo.Description;
            WasteBookInfo outWasteBookInfo;
            if (des.IndexOf("[增加资金]", StringComparison.Ordinal) > -1)
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, income, (int)WasteBookType.Increase);

            }
            else if (des.IndexOf("[减少资金]", StringComparison.Ordinal) > -1)
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, -income, (int)WasteBookType.Decrease);

            }
            else if (des.IndexOf("[付款]", StringComparison.Ordinal) > -1)
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, -income, (int)WasteBookType.Decrease);

            }
            else
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, -income, (int)WasteBookType.Decrease);
            }
            string inWasteBookIds = _wasteBookDao.GetWasteBookIdForUpdate(tradecode);

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    _wasteBookDao.Update(outWasteBookInfo);

                    if (des.IndexOf("[增加资金]", StringComparison.Ordinal) <= -1 && des.IndexOf("[减少资金]", StringComparison.Ordinal) <= -1 && des.IndexOf("[付款]", StringComparison.Ordinal) <= -1)
                    {
                        if (!String.IsNullOrEmpty(inWasteBookIds))
                        {
                            var inWasteBookId = new Guid(inWasteBookIds);

                            var inWasteBookInfo = new WasteBookInfo(inWasteBookId, dateCreatedUpdate, description, income, (int)WasteBookType.Increase);
                            _wasteBookDao.Update(inWasteBookInfo);
                            _wasteBookDao.UpdateBankAccountsId(inWasteBookId, bankAccountsId);
                        }
                    }
                    ts.Complete();
                }
                catch
                {
                    throw new ApplicationException("转帐同步失败！");
                }
            }
        }


        /// <summary>
        /// 新建手续费
        /// </summary>
        /// <param name="outBankAccountsId"></param>
        /// <param name="tradeCode"></param>
        /// <param name="poundage"></param>
        /// <param name="filialeId"> </param>
        public void InsertPoundage(Guid outBankAccountsId, string tradeCode, decimal poundage, Guid filialeId)
        {
            DateTime dateCreated = DateTime.Now;
            decimal outNonceBalance = _wasteBookDao.GetBalance(outBankAccountsId);
            try
            {
                const string PD_DESC = "[转出] [手续费]";
                var pdWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                        PD_DESC, -poundage, outNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Decrease, filialeId)
                {
                    LinkTradeCode = string.Empty,
                    LinkTradeType = (int)WasteBookLinkTradeType.Other
                };
                _wasteBookDao.Insert(pdWasteBookInfo);
            }
            catch
            {
                throw new ApplicationException("手续费新建失败！");
            }
        }
        #endregion

        
    }
}
