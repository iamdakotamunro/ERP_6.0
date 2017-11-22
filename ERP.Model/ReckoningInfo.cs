//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��12��31��
// �ļ�������:����
// ����޸�ʱ��:2010��8��12��
// ���һ���޸���:������
//================================================
using System;
using ERP.Enum;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>�{��ߩרTһ ������ģ��  ����޸��ύ  ������  2014-12-24 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReckoningInfo
    {
        /// <summary>�����˼�¼ID
        /// </summary>
        [DataMember]
        public Guid ReckoningId { get; set; }

        /// <summary> ��˾ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>��˾����
        /// </summary>
        [DataMember]
        public string FilialeName { get; set; }

        /// <summary>������λID
        /// </summary>
        [DataMember]
        public Guid ThirdCompanyID { get; set; }

        /// <summary>������λ���� 
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>���ݱ��
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>��������
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>��Ŀ����
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>���ݽ��.Ϊ��������ʾӦ���ʡ�.Ϊ��������ʾӦ���ʡ�
        /// </summary>
        [DataMember]
        public decimal AccountReceivable { get; set; }

        /// <summary>������λ��ǰ���
        /// </summary>
        [DataMember]
        public decimal NonceTotalled { get; set; }

        /// <summary> ������ʵ����ͣ�0���룬1֧��
        /// </summary>
        [DataMember]
        public int ReckoningType { get; set; }

        /// <summary> ������ʵ�״̬  ��Ӧö�� ReckoningStateType  1����  ��2���
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary> ����״̬ 
        /// </summary>
        [DataMember]
        public int IsChecked { get; set; }

        /// <summary> �������״̬ ��Ӧö�� AuditingState
        /// </summary>
        [DataMember]
        public int AuditingState { get; set; }

        /// <summary>ԭʼ���ݺ�
        /// </summary>
        [DataMember]
        public string LinkTradeCode { get; set; }

        /// <summary> ���������Ƿ��ǻ���������ն����� ������ԣ���ʱֻ�ж���ʱ����
        /// </summary>
        [DataMember]
        public bool IsRefuse { get; set; }

        /// <summary> ����Ŀ���ĸ��ֿ������
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary> �жϹ��캯�������ͣ��÷�������׼ȷ�Ĺ��캯��
        /// </summary>
        [DataMember]
        public ContructType ContructType { get; set; }

        /// <summary> �����˶�������  ��Ӧö��  ReckoningCheckType
        /// </summary>
        [DataMember]
        public int ReckoningCheckType { get; set; }

        /// <summary>��������ReckoningCheck����EMPTY��δ��õ�����
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary> ��ݲ��
        /// </summary>
        [DataMember]
        public double Carriage { get; set; }

        /// <summary> ��������
        /// </summary>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>�����ܼ�
        /// </summary>
        [DataMember]
        public decimal JoinTotalPrice { get; set; }

        /// <summary>��Ӧ�������� 
        /// </summary>
        [DataMember]
        public int LinkTradeType { get; set; }

        /// <summary>��¼�ֶΣ������Լ��õ�һ���ֶ� �����ڲ�ѯ��
        /// </summary>
        [DataMember]
        public bool IsOut { get; set; }

        /// <summary>������λ���й�˾���������˵������������λ������ʾ����
        /// </summary>
        [DataMember]
        public decimal ComCurrBalance { get; set; }

        /// <summary>������λ��˾�����������ʱ���£�
        /// </summary>
        [DataMember]
        public decimal CurrentTotalled { get; set; }

        /// <summary>
        /// �Ƿ�����ͬһ��˾��ͬһ������λ��ͬһ������ʵ����͡�ͬһ������ʵ�״̬��ͬһ�������״̬��ͬһԭʼ���ݺš�ͬһ���ݽ�������ͬ�������¼
        /// ����True��������False��
        /// </summary>
        /// zal 206-12-24
        [DataMember]
        public bool IsAllow { get; set; }

        /// <summary> Ĭ�Ϲ��캯��
        /// </summary>
        public ReckoningInfo()
        {
            ContructType = ContructType.Default;
        }

        /// <summary> 
        /// </summary>
        /// <param name="guReckoningID">�˵����</param>
        public ReckoningInfo(Guid guReckoningID)
        {
            ContructType = ContructType.Default;
            ReckoningId = guReckoningID;
        }

        /// <summary>���ڲ�ѯ����
        /// </summary>
        /// <param name="reckoningId">�˵����</param>
        /// <param name="filialeId">�ֹ�˾ID</param>
        /// <param name="companyId">������λID</param>
        /// <param name="tradeCode">�˵����</param>
        /// <param name="dateCreated">�½�����</param>
        /// <param name="description">����</param>
        /// <param name="accountReceivable">���.Ϊ��������ʾӦ���ʡ�.Ϊ��������ʾӦ���ʡ�</param>
        /// <param name="nonceTotalled">��������λ��ǰ���</param>
        /// <param name="reckoningType">�˵����ͣ�1.Ӧ�տ�2.Ӧ����3.����4.�Ѹ�5.�����ʽ�6.�����ʽ�</param>
        /// <param name="reckoningState">�˵�״̬</param>
        /// <param name="isChecked">Ĭ����0</param>
        /// <param name="auditingState">Ĭ��0(int)AuditingState.Unverify</param>
        /// <param name="originalTradeCode">Ĭ����null</param>
        public ReckoningInfo(Guid reckoningId, Guid filialeId, Guid companyId, string tradeCode, DateTime dateCreated, string description, decimal accountReceivable, decimal nonceTotalled, int reckoningType, int reckoningState, int isChecked, int auditingState, string originalTradeCode)
        {
            ReckoningId = reckoningId;
            FilialeId = filialeId;
            ThirdCompanyID = companyId;
            TradeCode = tradeCode;
            DateCreated = dateCreated;
            Description = description;
            AccountReceivable = accountReceivable;
            NonceTotalled = nonceTotalled;
            ReckoningType = reckoningType;
            State = reckoningState;
            IsChecked = isChecked;
            AuditingState = auditingState;
            LinkTradeCode = originalTradeCode;

            //��ʵ�����ݿ�����ֶΣ��жϹ��캯������
            ContructType = ContructType.Select;
        }


        /// <summary>���ڲ��뷽��
        /// </summary>
        /// <param name="reckoningId">�˵����</param>
        /// <param name="filialeId">�ֹ�˾ID</param>
        /// <param name="companyId">������λID</param>
        /// <param name="tradeCode">�˵����</param>
        /// <param name="description">����</param>
        /// <param name="accountReceivable">���.Ϊ��������ʾӦ���ʡ�.Ϊ��������ʾӦ���ʡ�</param>
        /// <param name="reckoningType">�˵����ͣ�1.Ӧ�տ�2.Ӧ����3.����4.�Ѹ�5.�����ʽ�6.�����ʽ�</param>
        /// <param name="reckoningState">�˵�״̬</param>
        /// <param name="isChecked">Ĭ����0</param>
        /// <param name="auditingState">Ĭ��0(int)AuditingState.Unverify</param>
        /// <param name="originalTradeCode">Ĭ����null</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        public ReckoningInfo(Guid reckoningId, Guid filialeId, Guid companyId, string tradeCode, string description, decimal accountReceivable, int reckoningType, int reckoningState, int isChecked, int auditingState, string originalTradeCode, Guid warehouseId)
        {
            ReckoningId = reckoningId;
            FilialeId = filialeId;
            ThirdCompanyID = companyId;
            TradeCode = tradeCode;
            Description = description;
            AccountReceivable = accountReceivable;
            ReckoningType = reckoningType;
            State = reckoningState;
            IsChecked = isChecked;
            AuditingState = auditingState;
            LinkTradeCode = originalTradeCode;
            WarehouseId = warehouseId;

            //��ʵ�����ݿ�����ֶΣ��жϹ��캯������
            ContructType = ContructType.Insert;
        }

        /// <summary>���ڸ��·���  
        /// </summary>
        /// <param name="description">����</param>
        /// <param name="accountReceivable">���.Ϊ��������ʾӦ���ʡ�.Ϊ��������ʾӦ���ʡ�</param>
        /// <param name="reckoningId">�˵����</param>
        /// <param name="dateCreated">��������</param>
        public ReckoningInfo(decimal accountReceivable, string description, Guid reckoningId, DateTime dateCreated)
        {
            AccountReceivable = accountReceivable;
            Description = description;
            ReckoningId = reckoningId;
            DateCreated = dateCreated;

            //��ʵ�����ݿ�����ֶΣ��жϹ��캯������
            ContructType = ContructType.Update;
        }
    }
}
