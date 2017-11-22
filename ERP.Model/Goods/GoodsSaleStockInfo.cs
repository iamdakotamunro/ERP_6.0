using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��Ʒ����ģ��
    /// </summary>
    [Serializable]
    public class GoodsSaleStockInfo
    {

        /// <summary>
        /// ����
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }
        
        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string ApplyReason { get; set; }
        
        /// <summary>
        /// ������
        /// </summary>
        public Guid Applicant { get; set; }
        
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime ApplyTime { get; set; }
        
        /// <summary>
        /// �����
        /// </summary>
        public Guid Auditor { get; set; }
        
        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime AuditTime { get; set; }
        
        /// <summary>
        /// �������
        /// </summary>
        public string AuditReason { get; set; }

        /// <summary>
        /// �����״̬
        /// </summary>
        public int SaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SaleStockState { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public int ReplenishmentCycle { get; set; }
    }

    /// <summary>
    /// �������� Grid
    /// </summary>
    [Serializable]
    public class GoodsSaleStockGridModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OldSaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NewSaleStockType { get; set; }
    }
}
