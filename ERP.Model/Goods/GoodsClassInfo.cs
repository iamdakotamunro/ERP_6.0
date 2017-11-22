using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��Ʒ����ģ��
    /// </summary>
    [Serializable]
    public class GoodsClassInfo
    {
        /// <summary>
        /// ������
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// ���ุ�ڵ���
        /// </summary>
        public Guid ParentClassId { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>��������
        /// </summary>
        public List<Guid> GoodsClassFieldList { get; set; }
    }
}
