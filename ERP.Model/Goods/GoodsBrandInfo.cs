using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��ƷƷ����Ϣ��
    /// </summary>
    [Serializable]
    public class GoodsBrandInfo
    {
        /// <summary>
        /// Ʒ�Ʊ��
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Ʒ������
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Ʒ��Logo������
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// Ʒ������
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ��Ʒ�����ĵ��б�
        /// </summary>
        public IEnumerable<GoodsInformationInfo> GoodsInformationList { get; set; }
    }
}
