//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��12��27��
// �ļ�������:����
// ����޸�ʱ��:2006��12��27��
// ���һ���޸���:����
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ���ﵥλ��,��:��
    /// </summary>
    [Serializable]
    public class UnitsInfo
    {
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public UnitsInfo() { }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        /// <param name="unitsId">��λId</param>
        /// <param name="units">��λ����</param>
        public UnitsInfo(Guid unitsId,string units)
        {
            UnitsId = unitsId;
            Units = units;
        }

        /// <summary>
        /// ������λID
        /// </summary>
        public Guid UnitsId { get; private set; }

        /// <summary>
        /// ������λ����
        /// </summary>
        public string Units { get; set; }
    }
}
