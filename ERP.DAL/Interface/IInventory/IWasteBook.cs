using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>�ʽ����ӿ�  ����޸��ύ  ������  2014-12-25
    /// </summary>
    public interface IWasteBook
    {
        #region [�����ʽ���]

        /// <summary>�����ʽ���
        /// </summary>
        /// <param name="wasteBook">�ʽ���</param>
        int Insert(WasteBookInfo wasteBook);

        #endregion

        #region [��������ʽ���]

        ///<summary>�����ʽ�����Ϣ
        /// </summary>
        /// <param name="wasteBook">�ʽ���</param>
        void Update(WasteBookInfo wasteBook);

        /// <summary>����DateCreated�ֶ�
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        void UpdateDateTime(string tradeCode);

        /// <summary>�����ʽ�������
        /// </summary>
        /// <param name="wastebookId">�ʽ�������ID</param>
        /// <param name="description">����</param>
        void UpdateDescription(Guid wastebookId, String description);

        /// <summary> �ʽ������ʱ��������������Ϣ
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="description">����</param>
        void UpdateDescriptionForAuditing(string tradeCode, string description);

        /// <summary>�����ʽ���������
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="poundage">������</param>
        /// <param name="dateTime">ʱ��</param>
        void UpdatePoundage(string tradeCode, decimal poundage, DateTime dateTime);

        /// <summary>�����ʽ���������
        /// </summary>
        /// <param name="wastebookId">�ʽ���ID</param>
        /// <param name="dateCreated">ʱ��</param>
        /// <param name="poundage">������</param>
        void UpdatePoundageForReckoning(Guid wastebookId, DateTime dateCreated, decimal poundage);

        /// <summary> �޸ķ��ü�¼ʱ�޸��ʽ���
        /// </summary>
        /// <param name="tradeCode">����ID</param>
        /// <param name="accountReceivable">���</param>
        /// <param name="description">����</param>
        /// <param name="dateTime">ʱ��</param>
        void UpdateForReckoningCost(string tradeCode, double accountReceivable, string description, DateTime dateTime);

        /// <summary> �����ʽ��� �����˺�ID
        /// </summary>
        /// <param name="wasteBookId">�ʽ���ID</param>
        /// <param name="bankAccountsId">�����˺�ID</param>
        void UpdateBankAccountsId(Guid wasteBookId, Guid bankAccountsId);

        ///<summary>����ʽ���
        /// </summary>
        /// <param name="tradeCode">�˵����</param>
        void Auditing(string tradeCode);

        /// <summary>�����ʽ���IsOut�ֶ�ΪTrue,ͬʱ���¶���(ǰ̨�º����뷢Ʊ�ã������ط�����)
        /// </summary>
        /// <param name="orderIds">����Ids </param>
        /// <param name="paidNo">������ˮ��</param>
        /// <returns></returns>
        Boolean RenewalWasteBookByIsOut(IEnumerable<Guid> orderIds, IEnumerable<string> paidNo);

        /// <summary>
        /// ���½���Ӷ��Ĳ���״̬
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-21 ����״̬(0:δ����1:�Ѵ���)
        bool UpdateOperateState();
        #endregion

        #region [ɾ���ʽ���]

        /// <summary>ɾ���ʽ���
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        void DeleteWasteBook(string tradeCode);

        /// <summary> ɾ���ʽ��������ѵļ�¼
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="poundage"></param>
        void DeleteWasteBookPoundage(string tradeCode, decimal poundage);

        #endregion

        #region [��ȡ�����˺��������ѣ���¼��]

        /// <summary>��ȡ�����˺����
        /// </summary>
        /// <param name="bankAccountsId">�����˻�ID</param>
        /// <returns></returns>
        decimal GetBalance(Guid bankAccountsId);

        /// <summary>��ȡ�ʽ���������
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        decimal GetPoundage(string tradeCode);

        /// <summary>��ȡ�ʽ���������
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        decimal GetPoundageForReckoning(string tradeCode);

        /// <summary>��ȡͬһ���ݺ��ʽ�����¼�����ж��Ƿ��������ѣ�
        /// </summary>
        /// <param name="tradeCode">���ݺ�</param>
        /// <returns></returns>
        decimal GetTradeCodeNum(string tradeCode);

        #endregion

        #region [��ȡ�ʽ���ID]

        /// <summary>���ݱ�Ż�ȡ�����ѵ��ʽ���ID
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        string GetWasteBookId(string tradeCode);

        /// <summary>��ȡ�ʽ��������ѵ�WasteBookId
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        string GetWasteBookIdForUpdate(string tradeCode);

        /// <summary> ��ȡ�����ѵ��ʽ���ID
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        string GetWasteBookIdForReckoning(string tradeCode);

        #endregion

        #region [��ȡ�ʽ���]

        /// <summary>��ȡ�ʽ�����Ϣ
        /// </summary>
        /// <param name="wasteBookId">�ʽ���ID</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBook(Guid wasteBookId);

        /// <summary>���ݵ��ݱ�Ż�ȡ�ʽ���
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBookByBankAccountsId(string tradeCode);

        /// <summary>���ݵ��ݱ�Ż�ȡ�ʽ���
        /// </summary>
        /// <param name="linkTradeCode">�������ݱ��</param>
        /// <param name="wasteSource">1:��è������������������Ӷ��;2:���ִ���;3:�������׽��</param>
        /// <returns></returns>
        /// zal 2016-06-15
        WasteBookInfo GetWasteBookByLinkTradeCodeAndWasteSource(string linkTradeCode, int wasteSource);

        /// <summary>��ȡ�ʽ�����Ϣ����ʷ���ݿ⣩
        /// </summary>
        /// <param name="wasteBookId">�ʽ���ID</param>
        /// <param name="dateTime">ʱ��</param>
        /// <param name="keepyear">�������</param>
        /// <returns></returns>
        WasteBookInfo GetWasteBook(Guid wasteBookId, DateTime dateTime, int keepyear);

        /// <summary> ��ȡ�ʽ�����������
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <returns></returns>
        WasteTypeInfo GetWasteBookInfo(String tradeCode);

        #endregion

        #region �ʽ�����ȡ��ط���

        /// <summary>
        /// ������ݻ�ȡ�ʽ�����Ϣ add 2015-06-15 CAA
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="year"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        IList<FundPaymentDaysInfo> GetFundPaymentDaysInfos(int keepyear, int year, Guid saleFilialeId, string bankName);

        /// <summary>
        /// ��ȡ��˾�������ʽ���Ϣ
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        IList<FundPaymentDaysInfo> GetFundPaymentDaysBankInfos(int keepyear, Guid filialeId, int year, string bankName);

        #endregion
        #region [��ȡ�ʽ�������]

        /// <summary>�����в���Ȩ�������˺ŵ��ʽ���
        /// </summary>
        /// <param name="bankAccountsId">�����ʺ�Id</param>
        /// <param name="personnelId">Ա��ID</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, Guid personnelId);

        /// <summary>��ȡ�ʽ��б�
        /// </summary>
        /// <param name="bankAccountsId">�����˺�ID</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">��ֹʱ��</param>
        /// <param name="receiptType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="minIncome">��Χ С</param>
        /// <param name="maxIncome">��Χ ��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="saleFilialeId">��˾ID</param>
        /// <param name="branchId">����ID</param>
        /// <param name="positionId">ְ��ID</param>
        /// <param name="keepyear">������������</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookList(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear);

        /// <summary>��ȡ�ʽ��б���ҳ��
        /// </summary>
        /// <param name="bankAccountsId">�����˺�ID</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">��ֹʱ��</param>
        /// <param name="receiptType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="minIncome">��Χ С</param>
        /// <param name="maxIncome">��Χ ��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="saleFilialeId">��˾ID</param>
        /// <param name="branchId">����ID</param>
        /// <param name="positionId">ְ��ID</param>
        /// <param name="keepyear">������������</param>
        /// <param name="startPage">��ǰҳ</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>��ȡ�ʽ��б���ҳ��
        /// </summary>
        /// <param name="bankAccountsId">�����˺�ID</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">��ֹʱ��</param>
        /// <param name="receiptType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="minIncome">��Χ С</param>
        /// <param name="maxIncome">��Χ ��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="saleFilialeId">��˾ID</param>
        /// <param name="branchId">����ID</param>
        /// <param name="positionId">ְ��ID</param>
        /// <param name="keepyear">������������</param>
        /// <param name="isCheck"> �Ƿ����</param>
        /// <param name="startPage">��ǰҳ</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListToPage(Guid bankAccountsId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid saleFilialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>��ȡ�ʽ����б���ҳ��
        /// </summary>
        /// <param name="saleFilialeId">��˾ID</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">��ֹʱ��</param>
        /// <param name="receiptType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="minIncome">��Χ С</param>
        /// <param name="maxIncome">��Χ ��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="filialeId">Ա����˾ID</param>
        /// <param name="branchId">����ID</param>
        /// <param name="positionId">ְ��ID</param>
        /// <param name="keepyear">���漸������</param>
        /// <param name="isCheck">�Ƿ����</param>
        /// <param name="startPage">��ǰҳ</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <returns></returns>
        IList<WasteBookInfo> GetWasteBookListBySaleFilialeIdToPage(Guid saleFilialeId, DateTime startDate, DateTime endDate,
                                              ReceiptType receiptType, AuditingState auditingState, double minIncome,
                                              double maxIncome, string tradeCode, Guid filialeId, Guid branchId, Guid positionId, int keepyear, int isCheck,
                                              int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// ����˾ë��ʹ�á���ȡ����Ӷ��(��Ӷ����һ�������Ŷ�Ӧ������Ӷ��ĺͣ�ԭ�򣺵����������ͱ�ϵͳ�������ڶ�Զ����������±�ϵͳһ��������������Ӷ��)
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-20
        IList<WasteBookInfo> GetWasteBookByDateCreatedForProfits();

        #endregion

    }
}
