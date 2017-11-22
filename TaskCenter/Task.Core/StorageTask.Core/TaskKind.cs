namespace StorageTask.Core
{
    public enum TaskKind
    {
        /// <summary>
        /// 添加销售出库单据
        /// </summary>
        AddSellOut,

        /// <summary>
        /// 计算库存余额流水
        /// </summary>
        CalculateStockBalanceFlow,

        /// <summary>
        /// 生成往来帐
        /// </summary>
        BuildReckoning,

        /// <summary>
        /// 第二次完成订单
        /// </summary>
        CompleteOrderWithSecond,

        /// <summary>快递下单异常业务
        /// </summary>
        ExpressOrderExceptionBusiness,

        /// <summary>菜鸟快递异常订单处理
        /// </summary>
        ExpressBusiness,

        /// <summary>
        /// 快递编号处理
        /// </summary>
        ExpressNo,

        /// <summary>菜鸟电子面单回扫异常数据处理业务
        /// </summary>
        FlyBackBusiness,

        /// <summary>生成快递单号
        /// </summary>
        GenerateMailNoBusiness,

        /// <summary>
        /// 自动报备(采购)
        /// </summary>
        AutoPurchasing,

        /// <summary>
        /// 进货申报
        /// </summary>
        AutoStockDeclare,

        /// <summary>订单发货率统计
        /// </summary>
        OrderDeliveryRatioDeclare,

        /// <summary>
        /// 商品库存、结算价及供应商月销售额存档
        /// </summary>
        GoodsStockRecordAndSales,

        /// <summary>
        /// 供应商应付款和采购入库存档
        /// </summary>
        SupplierPaymentsAndPurchasing,

        /// <summary>
        /// 商品毛利
        /// </summary>
        GoodsGrossProfit,

        /// <summary>
        /// 公司毛利
        /// </summary>
        CompanyGrossProfit,

        /// <summary>
        /// 供应商资质
        /// </summary>
        QualificationManager,

        /// <summary>申通双11临时任务 
        /// </summary>
        STO11TempTask,

        /// <summary>订单销量异步任务
        /// </summary>
        GoodsDaySalesStatisticsAsynTask,

        /// <summary>
        /// 销售公司每天凌晨生成采购单及入库单任务（来自B2C的订单）
        /// </summary>
        GenerateSaleFilialeStockInAndPurchaseFormTask,

        /// <summary>
        /// 销售订单关联的即时结算价
        /// </summary>
        GenerateSaleOrderGoodsSettlement,

        /// <summary>
        /// 商品即时结算价按月归档
        /// </summary>
        ArchiveLastMonthGoodsSettlement
    }
}
