using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 框架加工单
    /// </summary>
    [Serializable]
    public class LensProcessInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 加工单号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public int OperateState { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 加工公司
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 加工仓库
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单留言
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 管理信息
        /// </summary>
        public string Clew { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 汇总单号
        /// </summary>
        public string GatherNo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }

        /// <summary>
        /// 发货公司
        /// </summary>
        public Guid FromFilialeId { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public Guid FromWarehouseId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 加工者
        /// </summary>
        public string Processor { get; set; }

        /// <summary>
        /// 加工时间
        /// </summary>
        public DateTime ProcessDate { get; set; }
    }
}
