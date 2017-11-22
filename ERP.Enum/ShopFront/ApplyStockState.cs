using System;
using ERP.Enum.Attribute;

namespace ERP.Enum.ShopFront
{
    /// <summary>
    /// 门店采购申请状态
    /// </summary>
    [Serializable]
    public enum ApplyStockState
    {
        #region  原门店采购申请状态
        /// <summary>
        /// 申请中
        /// </summary>
        [Enum("申请中")]
        Applying = 1,

        /// <summary>
        /// 部分完成
        /// </summary>
        [Enum("部分完成")]
        PartFinish = 2,

        /// <summary>
        /// 完成
        /// </summary>
        [Enum("完成")]
        Finish = 3,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Cancel = 4,
        #endregion

        #region  联盟店采购申请状态  add
        /// <summary>
        /// 待付款
        /// </summary>
        /// 
        [Enum("待付款")]
        Obligation = 5,

        /// <summary>
        /// 付款确认
        /// </summary>
        /// 
        [Enum("付款确认")]
        PayConfirmed = 6,

        /// <summary>
        /// 发货确认
        /// </summary>
        /// 
        [Enum("发货确认")]
        Confirming = 7,

        /// <summary>
        /// 等待发货
        /// </summary>
        /// 
        [Enum("等待发货")]
        Delivering = 8,

        /// <summary>
        /// 等待收货
        /// </summary>
        /// 
        [Enum("等待收货")]
        Finishing = 9,

        /// <summary>
        /// 异常待审核
        /// </summary>
        /// 
        [Enum("异常待审核")]
        CheckPending = 10,

        ///// <summary>
        ///// 审核通过
        ///// </summary>
        ///// 
        //[Enum("审核通过")]
        //Pended = 11,

        ///// <summary>
        /////审核不通过
        ///// </summary>
        ///// 
        //[Enum("审核不通过")]
        //PendRefuse = 12,

        /// <summary>
        /// 部分完成
        /// </summary>
        /// 
        [Enum("部分完成")]
        PartFinished = 11,

        /// <summary>
        /// 交易完成
        /// </summary>
        /// 
        [Enum("交易完成")]
        Finished = 12,

        /// <summary>
        /// 交易关闭
        /// </summary>
        /// 
        [Enum("交易关闭")]
        Close = 13,

        #endregion
    }
}