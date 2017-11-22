using System;
using System.Collections.Generic;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ��˾ģ��
    /// </summary>
    [Serializable]
    public class FilialeInfo
    {
        /// <summary>
        /// ��¼ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShopJoinType { get; set; }

        /// <summary>
        /// �Ƿ��мӹ�����
        /// </summary>
        public bool IsProcess { get; set; }

        /// <summary>
        /// �����˻�
        /// </summary>
        public Guid CashAccountId { get; set; }

        /// <summary>
        /// �����˻�
        /// </summary>
        public Guid BankAccountId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeliverFilialeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeliverWarehouseName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ˰��
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// ע���ַ
        /// </summary>
        public string RegisterAddress { get; set; }

        /// <summary>
        /// ��˾��Ӫ��ʽ�����������۵�
        /// </summary>
        public List<Int32> FilialeTypes { get; set; }

        /// <summary>
        /// ��˾��Ӫ��Χ
        /// </summary>
        public List<Int32> GoodsTypes { get; set; }

        /// <summary>
        /// �Ƿ�Ϊ���۹�˾
        /// </summary>
        public bool IsSaleFiliale { get; set; }
    }
}
