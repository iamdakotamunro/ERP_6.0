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
        /// �����ʽ��ʺ�ID��ȡ�ѹ�����˾�˺ŵ����ݼ���(������BankAccountBinding)
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
        /// ȡ��ĳ���е�ǰ���
        /// </summary>
        /// <param name="bankAccountsId">����ID</param>
        /// <returns></returns>
        public double GetBankAccountsNonce(Guid bankAccountsId)
        {
            return _bankAccounts.GetBankAccountsNonce(bankAccountsId);
        }

        /// <summary>
        /// ��ȡ�ʽ��ʺ�
        /// ADD:2013.7.27 ���
        /// </summary>
        /// <param name="bankAccountId">�ʺ�ID</param>
        /// <returns></returns>
        public BankAccountInfo Get(Guid bankAccountId)
        {
            return BankAccount.Instance.Get(bankAccountId);
        }

        /// <summary>
        /// ��ȡ�ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�Id</param>
        /// <returns></returns>
        public BankAccountInfo GetBankAccounts(Guid bankAccountsId)
        {
            if (bankAccountsId == Guid.Empty) return new BankAccountInfo();
            return _bankAccounts.GetBankAccounts(bankAccountsId);
        }
        
        /// <summary>
        /// ��ȡ�ʽ��ʺ��б�
        /// </summary>
        /// <returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList()
        {
            return _bankAccounts.GetBankAccountsList();
        }
        
        /// <summary>
        /// ���һ����Ŀfor TransferForm.aspx.cs
        /// </summary>
        public void Auditing(string tradeCode)
        {
            try
            {
                _wasteBookDao.Auditing(tradeCode);
            }
            catch
            {
                throw new ApplicationException("���ʧ�ܣ�");
            }
        }

        public IEnumerable<BankAccountPermissionInfo> GetPermissionList(Guid bankAccountId)
        {
            return _bankAccounts.GetPermissionList(bankAccountId);
        }

        ///<summary>
        ///����������
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
                throw new ApplicationException("�����Ѹ���ʧ�ܣ�");
            }
        }
        
        #region ҵ�����ӵ�Ԫ����
        /// <summary>
        /// ɾ���ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�Id</param>
        public void Delete(Guid bankAccountsId)
        {
            if (Math.Round(_wasteBookDao.GetBalance(bankAccountsId), 3) == 0)
            {
                if (bankAccountsId != Guid.Empty)
                    _bankAccounts.Delete(bankAccountsId);
            }
            else
            {
                throw new ApplicationException("�˻���������������ɾ����");
            }
        }

        /// <summary>ת��
        /// </summary>
        /// <param name="inBankAccountsId">ת���ʺ�Id</param>
        /// <param name="outBankAccountsId">ת���ʺ�Id</param>
        /// <param name="sum">���</param>
        /// <param name="poundage"></param>
        /// <param name="description">˵��</param>
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
                string inDescription = "[ת��] [" + outBankAccountsInfo.BankName + "]" + description;
                var inWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), inBankAccountsId, tradeCode, dateCreated,
                                                        inDescription, sum, inNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Increase, filialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other
                                          };

                const string PD_DESC = "[ת��] [������]";
                var pdWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), outBankAccountsId, tradeCode, dateCreated,
                                                        PD_DESC, -poundage, outNonceBalance, (int)AuditingState.Hide,
                                                        (int)WasteBookType.Decrease, filialeId)
                                          {
                                              LinkTradeCode = string.Empty,
                                              LinkTradeType = (int)WasteBookLinkTradeType.Other
                                          };

                
                string outDescription = "[ת��] [" + intBankAccountsInfo.BankName + "]" + description;
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
                        throw new ApplicationException("ת��ͬ��ʧ�ܣ�");
                    }
                }
            }
        }


        /// <summary> ת��   add chenzhongwen
        /// </summary>
        /// <param name="inBankAccountsId">ת���ʺ�Id</param>
        /// <param name="outBankAccountsId">ת���ʺ�Id</param>
        /// <param name="sum">���</param>
        /// <param name="poundage">������</param>
        /// <param name="description">˵��</param>
        /// <param name="tradeCode"></param>
        /// <param name="outFilialeId">ת����˾ </param>
        /// <param name="inFilialeId"> ת�빫˾</param>
        /// <param name="realName"> ������</param>
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
                    inDescription = string.Format("[ת������(��Դ����:{0}�����뱸ע:{1}��ת��������:{2} ��{3})]", outBankInfo.BankName + "-" + outBankInfo.AccountsName, realName, description, dateTime);
                    outDescription = string.Format("[ת������(Ŀ������:{0}�����뱸ע:{1}��ת��������:{2}��{3})]", inBankInfo.BankName + "-" + inBankInfo.AccountsName, description, realName, dateTime);
                }
                else
                {
                    var inFilialeName = inFilialeId == _reckoningElseFilialeid ? "������˾" : FilialeManager.Get(inFilialeId).Name;
                    var outFilialeName = outFilialeId == _reckoningElseFilialeid ? "������˾" : FilialeManager.Get(outFilialeId).Name;
                    inDescription = string.Format("[ת������(��Դ��˾:{0}����Դ����:{1}�����뱸ע:{2}��ת��������:{3} ��{4})]", outFilialeName, outBankInfo.BankName + "-" + outBankInfo.AccountsName, realName, description, dateTime);
                    outDescription = string.Format("[ת������(Ŀ�깫˾:{0}��Ŀ������:{1}�����뱸ע:{2}��ת��������:{3}��{4})]", inFilialeName, inBankInfo.BankName + "-" + inBankInfo.AccountsName, description, realName, dateTime);
                }
                const string PD_DESC = "[ת��] [������]";
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
                        throw new ApplicationException("ת��ͬ��ʧ�ܣ�");
                    }
                }
            }
            return wasteBookInfo;
        }
        
        

        ///<summary>
        ///����һ����Ŀ
        /// </summary>
        public void UpdateBll(Guid outWasteBookId, string description, decimal income, string tradecode, decimal poundage, Guid bankAccountsId)
        {
            DateTime dateCreatedUpdate = DateTime.Now;
            var wasteBookInfo = _wasteBookDao.GetWasteBook(outWasteBookId);
            if(wasteBookInfo==null)return;
            string des =string.IsNullOrEmpty(wasteBookInfo.Description)?string.Empty:wasteBookInfo.Description;
            WasteBookInfo outWasteBookInfo;
            if (des.IndexOf("[�����ʽ�]", StringComparison.Ordinal) > -1)
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, income, (int)WasteBookType.Increase);

            }
            else if (des.IndexOf("[�����ʽ�]", StringComparison.Ordinal) > -1)
            {
                outWasteBookInfo = new WasteBookInfo(outWasteBookId, dateCreatedUpdate, description, -income, (int)WasteBookType.Decrease);

            }
            else if (des.IndexOf("[����]", StringComparison.Ordinal) > -1)
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

                    if (des.IndexOf("[�����ʽ�]", StringComparison.Ordinal) <= -1 && des.IndexOf("[�����ʽ�]", StringComparison.Ordinal) <= -1 && des.IndexOf("[����]", StringComparison.Ordinal) <= -1)
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
                    throw new ApplicationException("ת��ͬ��ʧ�ܣ�");
                }
            }
        }


        /// <summary>
        /// �½�������
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
                const string PD_DESC = "[ת��] [������]";
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
                throw new ApplicationException("�������½�ʧ�ܣ�");
            }
        }
        #endregion

        
    }
}
