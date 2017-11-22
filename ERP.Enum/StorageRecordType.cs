using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>出入库单据类型（新版对应WMS）
    /// </summary>
    public enum StorageRecordType
    {
        /// <summary>
        /// 采购进货
        /// </summary>
        [Enum("采购进货")]
        BuyStockIn = 1,

        /// <summary>
        /// 销售退货
        /// </summary>
        [Enum("销售退货")]
        SellReturnIn = 2,

        /// <summary>
        /// 借入申请
        /// </summary>
        [Enum("借入申请")]
        BorrowIn = 3,

        /// <summary>
        /// 借出返还
        /// </summary>
        [Enum("借出返还")]
        LendIn = 4,

        /// <summary>
        /// 采购退货
        /// </summary>
        [Enum("采购退货")]
        BuyStockOut = 5,

        /// <summary>
        /// 售后退货
        /// </summary>
        [Enum("售后退货")]
        AfterSaleOut = 6,

        /// <summary>
        /// 销售出库
        /// </summary>
        [Enum("销售出库")]
        SellStockOut = 7,

        /// <summary>
        /// 借出申请
        /// </summary>
        [Enum("借出申请")]
        LendOut = 8,

        /// <summary>
        /// 借入返还
        /// </summary>
        [Enum("借入返还")]
        BorrowOut = 9,

        /// <summary>
        /// 内部采购(仓仓)
        /// </summary>
        [Enum("内部采购")]
        InnerPurchase = 10,


        /*------------------------------------只用于历史数据查询使用，不对应实际业务 zal 2017-02-10-----------------------------------------------*/
        /// <summary>
        /// 商品盘盈
        /// </summary>
        /// 只用于历史数据查询
        [Enum("商品盘盈(旧)")]
        BeyondStockIn = 21,

        /// <summary>
        /// 商品盘亏
        /// </summary>
        /// 只用于历史数据查询
        [Enum("商品盘亏(旧)")]
        LossStockOut = 22,

        /// <summary>
        /// 调拨入库
        /// </summary>
        /// 只用于历史数据查询
        [Enum("调拨入库(旧)")]
        TransferStockIn = 23,

        /// <summary>
        /// 调拨出库
        /// </summary>
        /// 只用于历史数据查询
        [Enum("调拨出库(旧)")]
        TransferStockOut = 24,

        /// <summary>
        /// 调拨拒收入库
        /// </summary>
        /// 只用于历史数据查询
        [Enum("调拨拒收入库(旧)")]
        TransferRefuse = 25,

        /// <summary>
        /// 出库冲红
        /// </summary>
        /// 只用于历史数据查询
        [Enum("出库冲红(旧)")]
        CancellationIn = 26,

        /// <summary>
        /// 入库冲红
        /// </summary>
        /// 只用于历史数据查询
        [Enum("入库冲红(旧)")]
        CancellationOut = 27,

        /// <summary>
        /// 拆分组合入库
        /// </summary>
        /// 只用于历史数据查询
        [Enum("拆分组合入库(旧)")]
        SplitCombinationIn = 28,

        /// <summary>
        /// 拆分组合出库
        /// </summary>
        /// 只用于历史数据查询
        [Enum("拆分组合出库(旧)")]
        SplitCombinationOut = 29,
        /*------------------------------------只用于历史数据查询使用，不对应实际业务 zal 2017-02-10-----------------------------------------------*/
    }
}
