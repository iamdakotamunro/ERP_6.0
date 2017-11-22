//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��5��3��
// �ļ�������:����
// ����޸�ʱ��:2010��3��23��
// ���һ���޸���:������
//================================================

using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model.ASYN;
using ERP.Model.Report;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>�{��ߩרTһ �����˽ӿ�  ����޸��ύ  ������  2014-12-24 
    /// </summary>
    public interface IReckoning
    {
        #region [����������]

        /// <summary>����������
        /// </summary>
        /// <param name="reckoningInfo">�����¼��Ŀ��</param>
        /// <param name="errorMessage"></param>
        bool Insert(ReckoningInfo reckoningInfo, out string errorMessage);

        /// <summary>
        /// ���ڿ���𻵲���������
        /// </summary>
        /// <param name="reckoningInfo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool IsExist(ReckoningInfo reckoningInfo, out string errorMsg);

        /// <summary>�����첽������
        /// </summary>
        /// <param name="asynInfo">�첽������ģ��</param>
        /// <returns></returns>
        bool InsertAsyn(ASYNReckoningInfo asynInfo);

        #endregion

        #region [����������]

        /// <summary>���������˼�¼ID�޸�������AccountReceivable��׷��Description��DateCreated
        /// </summary>
        /// <param name="reckoningInfo"></param>
        void Update(ReckoningInfo reckoningInfo);

        /// <summary>�����޸������˶������ͣ����˷����ã�
        /// </summary>
        /// <param name="lstModify">�����˼���</param>
        /// <param name="checkType">�������� 1 �Ѷ��� 0 δ���� 2 �쳣����</param>
        void UpdateCheckState(IList<ReckoningInfo> lstModify, Int32 checkType);

        /// <summary>
        /// �޸������˶�������
        /// </summary>
        /// <param name="linkTradeCode">ԭʼ���ݺ�</param>
        /// <param name="linkTradeType">��Ӧ��������</param>
        /// <param name="reckoningType">�˵����ͣ�0���룬1֧��</param>
        /// <param name="reckoningCheckType">�����˶�������</param>
        /// <param name="isChecked">�������� 1 �Ѷ��� 0 δ���� 2 �쳣����</param>
        /// zal 2016-06-05
        bool UpdateCheckState(string linkTradeCode, int linkTradeType, int reckoningType, int reckoningCheckType, int isChecked);

        /// <summary> ���������� ׷�� Description
        /// </summary>
        /// <param name="reckoningId">������ID</param>
        /// <param name="description"> </param>
        void UpdateDescription(Guid reckoningId, String description);

        /// <summary>�������׷�ӱ�ע��Ϣ���������Ϣ��
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="description"></param>
        void UpdateDescriptionForAuditing(string tradeCode, string description);

        /// <summary>������==�����
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        void Auditing(string tradeCode);

        /// <summary>���������
        /// </summary>
        /// <param name="linkTradeCode">ԭʼ���ݺ� </param>
        void CancellationReckoning(string linkTradeCode);

        #endregion

        #region [ɾ��������]

        /// <summary> ���ݵ��ݱ��ɾ����������Ϣ
        /// </summary>
        /// <param name="tradeCode">���ݱ��</param>
        void Delete(string tradeCode);

        /// <summary>�����첽����IDɾ���첽������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteAsyn(Guid id);

        #endregion

        #region [��ȡ�����˼���]

        /// <summary>���Ƿ����,����,�˵����ͻ�ȡ�����ˣ�δ���ˣ������б���ʾ   
        /// </summary>
        /// <param name="companyClass">������λ����</param>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="startDate">��ʼ����</param>
        /// <param name="endDate">��������</param>
        /// <param name="cType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="receiptType">����/֧��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <param name="keepyear">������������</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <param name="money">���</param>
        /// <param name="start">��ǰҳ</param>
        /// <param name="limit">ÿҳ��ʾ����</param>
        /// <returns></returns>
        IList<ReckoningInfo> GetValidateDataPage(Guid companyClass, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType, AuditingState auditingState,
            ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, params int[] money);

        /// <summary>���Ƿ����,����,�˵����ͻ�ȡ�����ˣ�δ���ˣ������б���ʾ   
        /// </summary>
        /// <param name="companyClassId">������λ����</param>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="startDate">��ʼ����</param>
        /// <param name="endDate">��������</param>
        /// <param name="cType">��������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="receiptType">����/֧��</param>
        /// <param name="tradeCode">���ݱ��</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <param name="keepyear">������������</param>
        /// <param name="recordCount">�ܼ�¼��</param>
        /// <param name="type"> </param>
        /// <param name="isOut"></param>
        /// <param name="money">���</param>
        /// <param name="start">��ǰҳ</param>
        /// <param name="limit">ÿҳ��ʾ����</param>
        /// <param name="isCheck"> </param>
        /// <returns></returns>
        IList<ReckoningInfo> GetValidateDataPage(Guid companyClassId, Guid companyId, Guid filialeId, DateTime startDate, DateTime endDate, CheckType cType, AuditingState auditingState,
            ReceiptType receiptType, String tradeCode, Guid warehouseId, int keepyear, long start, int limit, out int recordCount, int isCheck, int type, bool? isOut, params int[] money);

        /// <summary>��ȡ�����ʼ��ϣ����˷���ʹ�ã� ADD  2015-03-16  ������ 
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="reckoningCheckType">��������</param>
        /// <param name="checkType">����״̬</param>
        /// <returns></returns>
        IList<ReckoningInfo> GetReckoningListByReconciliation(Guid companyId, DateTime startTime, DateTime endTime, ReckoningCheckType reckoningCheckType, CheckType checkType);

        /// <summary>��ȡ�첽������
        /// </summary>
        /// <param name="top">top</param>
        /// <returns></returns>
        IList<ASYNReckoningInfo> GetAsynList(int top);

        #endregion

        #region [��ȡ��������Ϣ]

        /// <summary> ���������˼�¼ID��ȡ��������Ϣ
        /// </summary>
        /// <param name="reckoningId">�����¼Id</param>
        /// <returns></returns>
        ReckoningInfo GetReckoning(Guid reckoningId);

        /// <summary>���������˼�¼ID��ȡ��������Ϣ������ʷ���ݿ⣩
        /// </summary>
        /// <param name="reckoningId">������ID</param>
        /// <param name="dateTime">ʱ��</param>
        /// <param name="keepyear">�����������</param>
        /// <returns></returns>
        ReckoningInfo GetReckoning(Guid reckoningId, DateTime dateTime, int keepyear);

        /// <summary>����������λID��ԭʼ���ݺš��������ͻ�ȡ��������Ϣ
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="linkTradeCode">ԭʼ���ݺ�</param>
        /// <param name="checkType">��������</param>
        /// <param name="reckoningCheckType"></param>
        /// <returns></returns>
        ReckoningInfo GetReckoningInfo(Guid companyId, string linkTradeCode, CheckType checkType, int reckoningCheckType=2);

        #endregion

        #region [��ȡ��������]

        /// <summary>����������λID��ȡ��������
        /// </summary>
        /// <param name="companyId">������λ���</param>
        decimal GetTotalled(Guid companyId);

        /// <summary>����������λ����˾�����ڣ���ȡ��������
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="endDate">���� </param>
        /// <returns></returns>
        decimal GetReckoningNonceTotalledByFilialeId(Guid companyId, Guid filialeId, DateTime endDate);

        /// <summary>����������λ����˾����ȡ�������ʣ�������ˣ�
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <returns></returns>
        decimal GetNonceTotalled(Guid companyId, Guid filialeId);

        /// <summary>���ݹ�˾��������λ�����ڡ�����״̬��ȡ��������
        /// </summary>
        /// <param name="filialeId">��˾ID </param>
        /// <param name="companyId">������λ</param>
        /// <param name="endDate">��������</param>
        /// <param name="startDate">��ʼ����</param>
        /// <param name="reckoningState">��������</param>
        /// <returns></returns>
        decimal GetReckoningNonceTotalled(Guid filialeId, Guid companyId, DateTime startDate, DateTime endDate, int reckoningState);

        #endregion

        #region [��ȡ�����˵������� ��0���룬1֧����-1δ��ȡ����]

        /// <summary> ����������ID��ȡ�����˵������ͣ�0���룬1֧����-1δ��ȡ����
        /// </summary>
        /// <param name="reckoningId"></param>
        int GetReckoningType(Guid reckoningId);

        #endregion

        #region [�ж��������Ƿ����]

        /// <summary>���ݹ�˾��������λ���������ͣ�ԭʼ���ݺ��ж��Ƿ���ڸ�������
        /// </summary>
        /// <param name="filialeID">��˾ID</param>
        /// <param name="companyID">������λID</param>
        /// <param name="reckoningType">�����˵�λ����</param>
        /// <param name="linkTradeCode">ԭʼ���ݺ�</param>
        /// <returns></returns>
        bool Exists(Guid filialeID, Guid companyID, int reckoningType, string linkTradeCode);

        #endregion

        #region [��ȡ������ID]

        /// <summary>��ȡ������λ�����������һ����¼ID�����ڼ�¼���˵��
        /// </summary>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="companyId">������λID</param>
        /// <param name="startTime">��ʼ����</param>
        /// <param name="endTime">��������</param>
        /// <returns></returns>
        Guid GetReckoningInfoByDateLast(Guid filialeId, Guid companyId, DateTime startTime, DateTime endTime);

        /// <summary>���ݵ��ݱ�Ż�ԭʼ���ݺŻ�ȡ������ID
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        Guid GetReckoningInfoId(string tradeCode, int? isChecked);

        #endregion

        #region ��Ӧ�̶���ʹ�� add by liangcanren at 2015-05-29

        /// <summary>
        ///  ��ȡ��ⵥ��Ӧ��δ���˵�������
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        Guid GetReckoningInfoByTradeCode(string tradeCodeOrLinkTradeCode);

        /// <summary>
        /// �ж���ⵥ��Ӧ���������Ƿ����
        /// </summary>
        /// <param name="tradeCodeOrLinkTradeCode"></param>
        /// <returns></returns>
        bool IsExists(string tradeCodeOrLinkTradeCode);


        /// <summary>
        /// �������ʱ�ʶΪ�Ѷ���
        /// </summary>
        /// <param name="reckoningid"></param>
        /// <param name="isChecked"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        bool UpdateReckoningIsChecked(Guid reckoningid, int isChecked, DateTime startTime);

        /// <summary>
        /// ��ȡ�������ڸ���δ���˵��������ܶ�
        /// </summary>
        /// <param name="companyId">������λ</param>
        /// <param name="filialeId"></param>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="isChecked">�Ƿ����</param>
        /// <param name="stockNos">��ⵥ��</param>
        /// <param name="removerNos"></param>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetTotalledByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked, IList<string> stockNos, IList<string> removerNos);


        /// <summary>
        /// ��ȡ�ɹ���Id��ȡ�Ѷ��˵���ⵥ���͵������ʽ��
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="linkTradeId"></param>
        /// <param name="isChecked"></param>
        /// <param name="reckoningType"></param>
        /// <param name="linkTradeCodeType"></param>
        /// <returns></returns>
        decimal GetTotalledAccountReceivableByLinkTradeId(Guid filialeId,Guid companyId, Guid linkTradeId, int isChecked, int reckoningType, IList<int> linkTradeCodeType);

        /// <summary>
        /// �ѽ��вɹ�������������˽��
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="purchasingNo"></param>
        /// <param name="isChecked"></param>
        /// <param name="reckoningType"></param>
        /// <param name="linkTradeCodeType"></param>
        /// <returns></returns>
        decimal GetTotalledAccountReceivable(Guid filialeId, Guid companyId, string purchasingNo, int isChecked,int reckoningType,
            IList<int> linkTradeCodeType);

        /// <summary>���ݵ��ݺŻ�ȡ��������
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="linkTradeCode">������λ���</param>
        decimal GetTotalledByLinkTradeCode(Guid companyId, string linkTradeCode);

        #endregion

        #region  �����ʴ浵��Ӧ�����ѯ�������ͳ��  ADD BY LiangCanren AT 2015-08-17

        /// <summary>
        /// ��ѯ��ǰ�·���������ʼ�����������
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        IList<RecordReckoningInfo> SelectRecordReckoningInfos(DateTime startTime, DateTime endTime);

        /// <summary>
        /// ��ȡ���굱ǰ�·��¹�˾��Ӧ��Ӧ����
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecords(int year, int month);

        /// <summary>
        /// ��ȡ��˾���굱ǰ�·��¹�Ӧ�̶�Ӧ��Ӧ��������
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthPaymentsRecordsDetail(int year, int month, Guid filialeId, string companyName);

        /// <summary>
        /// ��ȡ���굱ǰ�·��¹�˾��Ӧ�Ĳɹ�������ܽ��
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecords(int year, int month);

        /// <summary>
        /// ��ȡ��˾���굱ǰ�·��¹�Ӧ�̶�Ӧ�Ĳɹ�������ܽ��
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<SupplierPaymentsRecordInfo> SelectCurrentMonthStockRecordsDetail(int year, int month, Guid filialeId, string companyName);

        #endregion

        bool CheckByPurchaseOrder(IEnumerable<string> purchaseOrder,Guid filialeId,Guid thirdCompanyId,DateTime startTime);

        bool CheckByStorageTradeCode(IEnumerable<string> purchaseOrder, Guid filialeId, Guid thirdCompanyId, DateTime startTime);

        bool CheckByDate(Guid companyId, Guid filialeId, DateTime startTime, DateTime endTime, int isChecked,
            IList<string> stockNos, IList<string> removerNos);
    }
}
