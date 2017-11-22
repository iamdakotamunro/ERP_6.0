using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 财务单编码类型枚举
    /// </summary>
    [Serializable]
    [DataContract]
    public enum CodeType
    {
        /// <summary>
        /// 网上购买，订单
        /// </summary>
        [Enum("网上购买，订单")]
        [EnumMember]
        RT = 1,
        /// <summary>
        /// 会员帐号冲值
        /// </summary>
        [EnumMember]
        [Enum("会员帐号冲值")]
        RF = 2,
        /// <summary>
        /// 会员帐号退款
        /// </summary>
        [Enum("会员帐号退款")]
        [EnumMember]
        AV = 3,
        /// <summary>
        /// 往来单位，会员帐号调账，增减费用
        /// </summary>
        [Enum("往来单位，会员帐号调账，增减费用")]
        [EnumMember]
        AJ = 4,
        /// <summary>
        /// 资金账户增加资金
        /// </summary>
        [Enum("资金账户增加资金")]
        [EnumMember]
        GI = 5,
        /// <summary>
        /// 资金账户减少资金
        /// </summary>
        [Enum("资金账户减少资金")]
        [EnumMember]
        RD = 6,
        /// <summary>
        /// 资金账户转帐
        /// </summary>
        [Enum("资金账户转帐")]
        [EnumMember]
        VI = 7,
        /// <summary>
        /// 单位往来账付款
        /// </summary>
        [Enum("单位往来账付款")]
        [EnumMember]
        PY = 8,
        /// <summary>
        /// 单位往来账收款
        /// </summary>
        [Enum("单位往来账收款")]
        [EnumMember]
        GT = 9,
        /// <summary>
        /// 购买入库支出
        /// </summary>
        [Enum("购买入库支出")]
        [EnumMember]
        RK = 10,
        /// <summary>
        /// 销售出库
        /// </summary>
        [Enum("销售出库")]
        [EnumMember]
        SL = 11,
        /// <summary>
        /// --暂未用
        /// </summary>
        [EnumMember]
        [Enum("暂未用")]
        LR = 12,
        /// <summary>
        /// 进货退货单
        /// </summary>
        [Enum("进货退货单")]
        [EnumMember]
        SO = 13,
        /// <summary>
        /// 销售退货单
        /// </summary>
        [Enum("销售退货单")]
        [EnumMember]
        SI = 14,
        /// <summary>
        /// 商品盘赢单
        /// </summary>
        [Enum("商品盘赢单")]
        [EnumMember]
        BS = 15,
        /// <summary>
        /// 商品盘亏单
        /// </summary>
        [Enum("商品盘亏单")]
        [EnumMember]
        LS = 16,
        /// <summary>
        /// 调拨入库单
        /// </summary>
        [Enum("调拨入库单")]
        [EnumMember]
        TSI = 17,
        /// <summary>
        /// 调拨出库单
        /// </summary>
        [Enum("调拨出库单")]
        [EnumMember]
        TSO = 18,
        ///// <summary>
        ///// Eyesee网上购买，订单
        ///// </summary>
        //[EnumArrtibute("Eyesee网上购买，订单")]
        //[EnumMember]
        //SRT = 19,
        /// <summary>
        /// 费用申报单
        /// </summary>
        [Enum("费用申报单")]
        [EnumMember]
        RE = 20,
        /// <summary>
        /// 采购单
        /// </summary>
        [Enum("采购单")]
        [EnumMember]
        PH = 21,
        /// <summary>
        /// 盘点计划单
        /// </summary>
        [Enum("盘点计划单")]
        [EnumMember]
        IV = 22,
        /// <summary>
        /// 会员提现单
        /// </summary>
        [Enum("会员提现单")]
        [EnumMember]
        MM = 23,
        /// <summary>
        /// 积分流水编号
        /// </summary>
        [Enum("积分流水编号")]
        [EnumMember]
        SR = 24,
        /// <summary>
        /// 退货单号
        /// </summary>
        [Enum("退货单号")]
        [EnumMember]
        TH = 26,

        /// <summary>
        /// 调拨拒收入库单
        /// </summary>
        [Enum("调拨拒收入库单")]
        [EnumMember]
        TR = 27,

        /// <summary>
        /// 余额原路退回（订单作废）,艾视SDB
        /// </summary>
        [Enum("余额原路退回（订单作废）,艾视SDB")]
        [EnumMember]
        DB = 28,

        /// <summary>
        /// 加工单
        /// </summary>
        [Enum("加工单")]
        [EnumMember]
        LP = 29,

        /// <summary>
        /// 商品组合拆分
        /// </summary>
        [Enum("商品组合拆分")]
        [EnumMember]
        MS = 30,

        /// <summary>
        /// 借入单
        /// </summary>
        [Enum("借入单")]
        [EnumMember]
        BI = 31,

        /// <summary>
        /// 借入返还单
        /// </summary>
        [Enum("借入返还单")]
        [EnumMember]
        BO = 32,

        /// <summary>
        /// 借出单
        /// </summary>
        [Enum("借出单")]
        [EnumMember]
        LO = 33,

        /// <summary>
        /// 借出返还单
        /// </summary>
        [Enum("借出返还单")]
        [EnumMember]
        LI = 34,

        /// <summary>
        /// 镜拓天猫订单号
        /// </summary>
        [Enum("镜拓天猫订单号")]
        [EnumMember]
        JT = 35,

        /// <summary>入库红冲
        /// </summary>
        [Enum("入库红冲")]
        [EnumMember]
        CI = 36,

        /// <summary>出库红冲
        /// </summary>
        [Enum("出库红冲")]
        [EnumMember]
        CO = 37,

        /// <summary>汉代天猫店订单号
        /// </summary>
        [Enum("汉代天猫店订单号")]
        [EnumMember]
        HD = 38,

        /// <summary>
        /// 换货单号
        /// </summary>
        [Enum("换货单号")]
        [EnumMember]
        HH = 39,

        /// <summary>
        /// 内部采购
        /// </summary>
        [Enum("内部采购")]
        [EnumMember]
        CC = 40,
    }
}
