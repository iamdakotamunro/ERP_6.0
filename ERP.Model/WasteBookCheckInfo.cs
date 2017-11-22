//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��5��9��
// �ļ�������:����
// ����޸�ʱ��:2010��3��20��
// ���һ���޸���:������
//================================================
using System;
using System.Runtime.Serialization;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// �ʽ����˶�����
    /// </summary>
    [Serializable]
    [DataContract]
    public class WasteBookCheckInfo
    {
        /// <summary>
        /// ���˱�ID
        /// </summary>
        [DataMember]
        public Guid WasteBookId { get; set; }

        /// <summary>
        /// �˶Խ��
        /// </summary>
        [DataMember]
        public decimal CheckMoney { get; set; }

        /// <summary>
        /// �˶�����ע
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// �˶�ʱ��
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public WasteBookCheckInfo()
        {
            
        }
    }
}
