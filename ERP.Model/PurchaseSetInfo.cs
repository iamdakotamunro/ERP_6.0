using System;
using System.Runtime.Serialization;
using System.Security.AccessControl;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 商品采购设置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PurchaseSetInfo
    {
        #region --> 附加属性

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        #endregion

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 物流配送公司ID
        /// </summary>
        [DataMember]
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 采购价
        /// </summary>
        [DataMember]
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 责任人ID
        /// </summary>
        [DataMember]
        public Guid PersonResponsible { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 促销ID
        /// </summary>
        [DataMember]
        public Guid PromotionId { get; set; }

        /// <summary>
        /// 缺货商品数
        /// </summary>
        [DataMember]
        public int GoodsCount { get; set; }

        /// <summary>
        /// 缺货订单数
        /// </summary>
        [DataMember]
        public int OrderCount { get; set; }

        /// <summary>
        /// 责任人名称
        /// </summary>
        [DataMember]
        public string PersonResponsibleName { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        public string WarehouseName { get; set; }

        /// <summary>
        /// 是否自动报备商品
        /// </summary>
        [DataMember]
        public bool IsStockUp { get; set; }

        /// <summary>
        /// 采购分组ID，ID为空表示为“默认”
        /// </summary>
        [DataMember]
        public Guid PurchaseGroupId { get; set; }

        /// <summary>
        /// 采购分组名称
        /// </summary>
        [DataMember]
        public string PurchaseGroupName { get; set; }

        /// <summary>
        /// 报备形式:1常规(月周期)；2触发报备
        /// </summary>
        [DataMember]
        public int FilingForm { get; set; }

        /// <summary>
        /// 备货日
        /// </summary>
        [DataMember]
        public int StockUpDay { get; set; }

        #region --> 报备形式：1常规(月周期)

        /// <summary>
        /// 第一周
        /// </summary>
        [DataMember]
        public int FirstWeek { get; set; }

        /// <summary>
        /// 第二周
        /// </summary>
        [DataMember]
        public int SecondWeek { get; set; }

        /// <summary>
        /// 第三周
        /// </summary>
        [DataMember]
        public int ThirdWeek { get; set; }

        /// <summary>
        /// 第四周
        /// </summary>
        [DataMember]
        public int FourthWeek { get; set; }

        #endregion

        #region --> 报备形式：2触发报备

        /// <summary>
        /// 触发时报备量
        /// </summary>
        [DataMember]
        public int FilingTrigger { get; set; }

        /// <summary>
        /// 不足
        /// </summary>
        [DataMember]
        public int Insufficient { get; set; }

        #endregion

        /// <summary>是否删除(0禁用，1启用)
        /// </summary>
        [DataMember]
        public int IsDelete { get; set; }

        /// <summary>
        /// 负责人当月缺货率
        /// </summary>
        [DataMember]
        public decimal ShortageRate { get; set; }
    }
}
