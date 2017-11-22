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
        /// ����ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccounts">�ʺ�ʵ��</param>
        void Insert(BankAccountInfo bankAccounts);

        /// <summary>
        /// �����ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccounts">�ʺ�ʵ��</param>
        void Update(BankAccountInfo bankAccounts);

        /// <summary>
        /// ɾ���ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�Id</param>
        void Delete(Guid bankAccountsId);
               /// <summary>
        /// ȡ��ĳ���е�ǰ���
        /// </summary>
        /// <param name="bankAccountsId">����ID</param>
        /// <returns></returns>
        double GetBankAccountsNonce(Guid bankAccountsId);
        /// <summary>
        /// ��ȡ�ʽ��ʺ�
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�Id</param>
        /// <returns></returns>
        BankAccountInfo GetBankAccounts(Guid bankAccountsId);

        /// <summary>
        /// ��ȡ�ʽ��ʺ��б�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(Guid filialeId, Guid branchId, Guid positionId);
        /// <summary>
        /// ��ȡ�����ʽ��ʺ��б�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList();

        /// <summary>
        /// ��ȡ�ʽ��ʺ��б�����BankAccountBinding
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetList();

        /// <summary>
        /// ��ȡ�����ʽ��ʺ��б�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsNoBindingList();

        /// <summary>
        /// ��ȡָ��֧�����͵��ʺ�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(PaymentType paymentType);

        /// <summary>
        /// ��ȡָ��֧����������ʺ�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(PaymentType[] paymentTypes);

        /// <summary>
        /// �Ƿ���ǰ̨���з�Χ��
        /// </summary>
        /// <param name="bankAccountsId">���б��</param>
        /// <returns></returns>
        bool IsFace(Guid bankAccountsId);

        /// <summary>
        /// ��ȡ��ǰ˳���
        /// </summary>
        /// <returns></returns>
        int GetOrderIndex();

        /// <summary>
        /// ���������ʺ����
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�ID</param>
        /// <param name="orderIndex">���</param>
        /// <returns></returns>
        /// zal 2015-09-22
        bool UpdateOrderIndex(Guid bankAccountsId,int orderIndex);
        
        /// <summary>
        /// Func : �õ������Լ���ӿ�����
        /// Coder: dyy
        /// Date : 2010 Jan.4th
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, String> GetBankInterface();
        /// <summary>
        /// �������л�ȡ�ò������е��û�
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeBankAccoutid"></param>
        /// <returns></returns>
        IList<BankAccountPermissionInfo> GetBankPersionByBankId(Guid filialeId, Guid branchId, Guid postionId, Guid bankAccountsId, Guid filialeBankAccoutid);

        IEnumerable<BankAccountPermissionInfo> GetPermissionList(Guid bankAccountId);

        #region ɾ��Ȩ��
        /// <summary>
        /// ɾ��Ȩ��
        /// </summary>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        void DeleteBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId);

        /// <summary>
        /// Add by Liucaijun at 2010-january-07th
        /// ɾ�������˺�ʱʹ��,ɾ�����˺���ص�����Ȩ��
        /// </summary>
        void DeleteBankPersion(Guid bankAccountsId);
        #endregion

        #region ����Ȩ��
        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="bankAccountsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="postionId"></param>
        void AddBankPersion(Guid bankAccountsId, Guid filialeId, Guid branchId, Guid postionId);
        #endregion

        /// <summary>
        /// ���ݹ�˾ID������ID������ID��ְ��ID��ȡ��ص�Ȩ��
        /// Add by liucaijun at 2011-January-30th
        /// </summary>
        /// <returns></returns>
        BankAccountPermissionInfo GetPersonnelBankAccountsList(Guid bankId, Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>
        /// ��ȡ�󶨵������ʺ�
        /// Add by liucaijun at 2011-August-10th
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBindBankAccounts();

        /// <summary>
        /// ���빫˾�����˻��󶨹�ϵ
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="bankAccountsId"></param>
        /// <returns></returns>
        bool InsertBindBankAccounts(Guid filialeId, Guid bankAccountsId);

        /// <summary>
        /// ��ȡ��������б�
        /// </summary>
        /// <returns></returns>
        IList<BankAccountBalanceInfo> GetBalanceList();

        /// <summary>���ݶ������۹�˾��ƽ̨ID��ȡ��Ӧ��Ȩ�޵������˺��б�
        /// </summary>
        /// <param name="targetId">��˾��ƽ̨ID</param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsList(Guid targetId, Guid filialeId, Guid branchId, Guid positionId);

        
        /// <summary>
        /// ���ݵ��̻�ȡ�ʻ�(�������˵�������λ�ո���)
        /// </summary>
        /// <returns></returns>
        IList<ShopBankAccountsInfo> GetBankAccountsByShopId();
        
        /// <summary>���������˺��Ƿ������˺�
        /// </summary>
        /// <param name="bankAccountsId">�����˺�ID</param>
        /// <param name="isMain">�Ƿ����˺�</param>
        /// <returns></returns>
        bool SetBankAccountsIsMain(Guid bankAccountsId, bool isMain);

        /// <summary>��ȡ���з����ʺ���Ϣ���˻����ʽ���ҳ���ã� ADD 2015-03-03  ������
        /// </summary>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsListByNotIsMain();

        /// <summary>��ȡ��Ȩ�޵ķ����ʺ���Ϣ���˻���� 2015-05-05  ������
        /// </summary>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="branchId">����ID</param>
        /// <param name="positionId">ְ��ID</param>
        /// <returns></returns>
        IList<BankAccountInfo> GetBankAccountsListByNotIsMain(Guid filialeId, Guid branchId, Guid positionId);

        /// <summary>��ȡ���������ʺŵ����  2015-05-15  ������
        /// </summary>
        /// <returns></returns>
        double GetBankAccountsAllNonceBalance();
    }
}
