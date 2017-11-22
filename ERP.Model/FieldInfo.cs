//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��3��18��
// �ļ�������:����
// ����޸�ʱ��:2006��3��18��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;

namespace ERP.Model
{
    /// <summary>
    /// ��Ʒ������Ϣ��
    /// </summary>
    [Serializable]
    public class FieldInfo
    {
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public FieldInfo()
        {
            ChildFields = new List<FieldInfo>();
        }

        /// <summary>
        /// ���Ա��
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// �����Ա��
        /// </summary>
        public Guid ParentFieldId { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// ����ֵ
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public IList<FieldInfo> ChildFields { get; set; }
    }
}
