using System;

namespace ERP.Model
{
    /// <summary>库存周转信息  2015-06-12  陈重文  更新
    /// </summary>
    [Serializable]
    public class StockTurnOverInfo
    {
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>是否下架  true 下架，false 上架
        /// </summary>
        public bool State { get; set; }

        /// <summary>是否缺货 true 缺货，false 不缺货
        /// </summary>
        public bool IsScarcity { get; set; }

        /// <summary>商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary> 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>当前库存数
        /// </summary>
        public int StockNums { get; set; }

        /// <summary>最后进货价
        /// </summary>
        public decimal RecentInPrice { get; set; }

        /// <summary>最后进货日期(短日期格式 2015-06-13)
        /// </summary>
        public String RecentCDate { get; set; }

        /// <summary>是否统计绩效
        /// </summary>
        public bool IsStatisticalPerformance { get; set; }

        /// <summary>是否统计绩效 （√）
        /// </summary>
        public string IsStatisticalPerformanceStr { get; set; }

        /// <summary>供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>供应商名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>责任人ID
        /// </summary>
        public Guid PersonResponsible { get; set; }

        /// <summary>责任人姓名
        /// </summary>
        public string PersonResponsibleName { get; set; }

        /// <summary> 销售数量
        /// </summary>
        public int SaleNums { get; set; }

        /// <summary>30天/平均周转天数
        /// </summary>
        public decimal AvgTurnOver { get; set; }

        /// <summary>库存周转天数  备注：1，零销量，按库存量最大的排上面  2，周转天数最大的排上面
        /// </summary>
        public int TurnOverDays { get; set; }

        /// <summary>周转情况（文字描述）
        /// </summary>
        public string TurnOverStr { get; set; }

        /// <summary>报备周转天数
        /// </summary>
        public string TurnOverByFiling { get; set; }

        /// <summary>是否下架 （√）
        /// </summary>
        public string IsStateStr { get; set; }

        /// <summary>是否缺货 （√）
        /// </summary>
        public string IsScarcityStr { get; set; }

        /// <summary>无销量0，有销量1 (排序用)  备注：1，零销量，按库存量最大的排上面  2，周转天数最大的排上面
        /// </summary>
        public int SaleNumSort { get; set; }

        /// <summary>无库存0，有库存1 (排序用)  备注：1，零销量，按库存量最大的排上面  2，周转天数最大的排上面
        /// </summary>
        public int StockNumSort { get; set; }
    }
}
