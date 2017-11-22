using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 采购申请明细
    /// 作者：刘彩军
    /// 时间：2012-06-20
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApplyStockDetailInfo
    {
        /// <summary>
        /// 单据ID
        /// </summary>
        [DataMember]
        public Guid ApplyId { get; set; }
        /// <summary>
        /// 子商品ID
        /// </summary>
        [DataMember]
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public Int32 Quantity { get; set; }
        /// <summary>
        /// 此次调拨加盟价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public Int32 GoodsStock { get; set; }

        /// <summary>
        /// 是否打印条码
        /// </summary>
        [DataMember]
        public bool IsPrint { get; set; }

        /// <summary>
        /// 加工单号
        /// </summary>
        [DataMember]
        public string ProcessNo { get; set; }

        /// <summary>
        /// 门店公司ID
        /// </summary>
        [DataMember]
        public Guid ShopFilialeId { get; set; }


        /// <summary>
        /// 申请采购方仓库ID
        /// zhangfan added at 2013-June-21th
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 接收采购公司ID
        /// zhangfan added at 2013-June-21th
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 接收采购公司ID
        /// zhangfan added at 2013-June-21th
        /// </summary>
        [DataMember]
        public Guid CompanyWarehouseId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        [DataMember]
        public Guid CompGoodsID { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [DataMember]
        public string Units { get; set; }

        /// <summary>
        /// 是否需要确认
        /// </summary>
        [DataMember]
        public bool IsComfirmed { get; set; }

        /// <summary>
        /// 确认提示信息
        /// </summary>
        [DataMember]
        public string ComfirmTips { get; set; }
    }
}
