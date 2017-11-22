using System;
using System.Collections.Generic;

namespace AutoPurchasing.Core.Model
{
    [Serializable]
    public class PurchasingGoods
    {
        /// <summary>
        /// 商品每月第几日采购
        /// </summary>
        public int FilingDay { get; set; }

        /// <summary>
        /// 商品采购前的备货天数
        /// </summary>
        public int StockingDays { get; set; }

        /// <summary>
        /// 商品采购后的到货天数
        /// </summary>
        public int ArrivalDays { get; set; }

        /// <summary>
        /// 商品采购入库的仓库ID，空值说明的全部仓库
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 商品采购入库的仓库，空值说明的全部仓库
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 采购商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 采购商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 采购供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 采购供应商
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 采购分组ID
        /// </summary>
        public Guid PurchaseGroupId { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 每箱数量
        /// </summary>
        public int PackQuantity { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 最近一次的采购日期
        /// </summary>
        public DateTime LastPurchasingDate { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public Guid PersonResponsible { get; set; }

        /// <summary>
        /// 报备形式:1常规(月周期)；2触发报备
        /// </summary>
        public int FilingForm { get; set; }

        /// <summary>
        /// 备货日（1周一，3周三）
        /// </summary>
        public int StockUpDay { get; set; }

        public Guid PromotionId { get; set; }

        public Guid HostingFilialeId { get; set; }

        #region --> 报备形式：1常规(月周期)

        /// <summary>
        /// 第一周
        /// </summary>
        public int FirstWeek { get; set; }

        /// <summary>
        /// 第二周
        /// </summary>
        public int SecondWeek { get; set; }

        /// <summary>
        /// 第三周
        /// </summary>
        public int ThirdWeek { get; set; }

        /// <summary>
        /// 第四周
        /// </summary>
        public int FourthWeek { get; set; }

        #endregion

        #region --> 报备形式：2触发报备

        /// <summary>
        /// 触发时报备量
        /// </summary>
        public int FilingTrigger { get; set; }

        /// <summary>
        /// 不足
        /// </summary>
        public int Insufficient { get; set; }

        #endregion

        /// <summary>
        /// 采购商品里的子商品
        /// </summary>
        public IList<ChildGoodsSalePurchasing> ChildGoodsSalePurchasingList { get; set; }

    }
}
