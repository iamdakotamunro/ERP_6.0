using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Keede.Ecsoft.Model
{
    /// <summary>�{��ߩרTһ �ʽ���ģ��  ����޸��ύ  ������  2014-12-25 
    /// </summary>
    [Serializable]
    [DataContract]
    public class WasteBookInfo
    {
        /// <summary>���˱�ID
        /// </summary>
        [DataMember]
        public Guid WasteBookId { get; set; }

        /// <summary> �����˻�ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary> �˵����
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string TradeCodeforT { get; set; }

        /// <summary> ��������
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>����
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary> ����
        /// </summary>
        [DataMember]
        public decimal Income { get; set; }

        /// <summary>Ȩ��ʵ������
        /// </summary>
        [DataMember]
        public decimal NonceBalance { get; set; }

        /// <summary>���״̬
        /// </summary>
        [DataMember]
        public int AuditingState { get; set; }

        /// <summary>���˱�����
        /// </summary>
        [DataMember]
        public int WasteBookType { get; set; }

        /// <summary>���۹�˾ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId
        {
            get;
            set;
        }

        /// <summary>�����ʽ����ĵ��ݺ�
        /// </summary>
        [DataMember]
        public string LinkTradeCode { get; set; }

        /// <summary>�����ʽ����Ķ�Ӧ��������
        /// </summary>
        [DataMember]
        public int LinkTradeType { get; set; }

        /// <summary>�ʽ�������״̬��ö��WasteBookState   1.������2��壩
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>�ʽ�����ˮ��(��¼���С����������ݺ�)
        /// </summary>
        [DataMember]
        public string BankTradeCode { get; set; }

        /// <summary>��¼�ֶΣ������Լ��õ�һ���ֶ� �����ڲ�ѯ��
        /// </summary>
        [DataMember]
        public bool IsOut { get; set; }

        /// <summary>
        /// �ʽ�����Դ(1:��è������������������Ӷ��;2:���ִ���;3:�������׽��;)
        /// </summary>
        /// zal 2016-05-18
        [DataMember]
        public int WasteSource { get; set; }

        /// <summary>
        /// ����״̬(0:δ����1:�Ѵ���)
        /// </summary>
        /// zal 2016-09-21
        [DataMember]
        public int OperateState { get; set; }

        /// <summary>�Ƿ�ת��
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public Boolean IsTranfer
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TradeCode))
                {
                    return false;
                }
                if (TradeCode.Length < 2)
                {
                    return false;
                }
                return TradeCode.Substring(0, 2) == "VI";
            }
        }

        /// <summary>Ĭ�Ϲ��캯��
        /// </summary>
        public WasteBookInfo()
        {
        }

        /// <summary> ���ڲ�ѯ����
        /// </summary>
        /// <param name="wasteBookId">���˱�ID</param>
        /// <param name="bankAccountsId">�����˻�ID</param>
        /// <param name="tradeCode">�˵����</param>
        /// <param name="dateCreated">��������</param>
        /// <param name="description">����</param>
        /// <param name="income">����</param>
        /// <param name="nonceBalance">Ȩ��ʵ������</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="wasteBookType">���˱�����</param>
        /// <param name="saleFilialeId">��˾ID </param>
        public WasteBookInfo(Guid wasteBookId, Guid bankAccountsId, string tradeCode, DateTime dateCreated, string description, decimal income, decimal nonceBalance, int auditingState, int wasteBookType, Guid saleFilialeId)
        {
            WasteBookId = wasteBookId;
            BankAccountsId = bankAccountsId;
            TradeCode = tradeCode;
            DateCreated = dateCreated;
            Description = description;
            Income = income;
            NonceBalance = nonceBalance;
            AuditingState = auditingState;
            WasteBookType = wasteBookType;
            SaleFilialeId = saleFilialeId;
        }

        /// <summary> ���ڲ��뷽��
        /// </summary>
        /// <param name="wasteBookId">���˱�ID</param>
        /// <param name="bankAccountsId">�����˻�ID</param>
        /// <param name="tradeCode">�˵����</param>
        /// <param name="description">����</param>
        /// <param name="income">����</param>
        /// <param name="auditingState">���״̬</param>
        /// <param name="wasteBookType">���˱�����</param>
        /// <param name="saleFilialeId"></param>
        public WasteBookInfo(Guid wasteBookId, Guid bankAccountsId, string tradeCode, string description, decimal income, int auditingState, int wasteBookType, Guid saleFilialeId)
        {
            WasteBookId = wasteBookId;
            BankAccountsId = bankAccountsId;
            TradeCode = tradeCode;
            Description = description;
            Income = income;
            AuditingState = auditingState;
            WasteBookType = wasteBookType;
            SaleFilialeId = saleFilialeId;
        }

        /// <summary>���ڸ��·���
        /// </summary>
        /// <param name="wasteBookId">���˱�ID</param>
        /// <param name="dateCreated">��������</param>
        /// <param name="description">����</param>
        /// <param name="income">����</param>
        /// <param name="wasteBookType">���˱�����</param>
        public WasteBookInfo(Guid wasteBookId, DateTime dateCreated, string description, decimal income, int wasteBookType)
        {
            WasteBookId = wasteBookId;
            DateCreated = dateCreated;
            Description = description;
            Income = income;
            WasteBookType = wasteBookType;
        }

        /// <summary>
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is WasteBookInfo)
                return (compareObj as WasteBookInfo).WasteBookId == WasteBookId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (WasteBookId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + WasteBookId;
            return stringRepresentation.GetHashCode();
        }
    }
}
