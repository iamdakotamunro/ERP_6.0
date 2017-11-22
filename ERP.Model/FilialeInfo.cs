using System;
using System.Collections.Generic;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 公司模型
    /// </summary>
    [Serializable]
    public class FilialeInfo
    {
        /// <summary>
        /// 记录ID
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
        /// 是否有加工能力
        /// </summary>
        public bool IsProcess { get; set; }

        /// <summary>
        /// 现在账户
        /// </summary>
        public Guid CashAccountId { get; set; }

        /// <summary>
        /// 银行账户
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
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisterAddress { get; set; }

        /// <summary>
        /// 公司经营方式：物流、销售等
        /// </summary>
        public List<Int32> FilialeTypes { get; set; }

        /// <summary>
        /// 公司经营范围
        /// </summary>
        public List<Int32> GoodsTypes { get; set; }

        /// <summary>
        /// 是否为销售公司
        /// </summary>
        public bool IsSaleFiliale { get; set; }
    }
}
