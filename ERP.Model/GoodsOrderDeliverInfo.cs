using System;

namespace ERP.Model
{
    /// <summary>
    /// 快递运费模型
    /// </summary>
    [Serializable]
    public class GoodsOrderDeliverInfo
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单总重
        /// </summary>
        public double TotalWeight { get; set; }

        /// <summary>
        /// 快递运费
        /// </summary>
        public double CarriageFee { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 最大允许差值
        /// </summary>
        public double MaxWrongValue { get; set; }

        /// <summary>省份名称
        /// </summary>
        public String ProvinceName { get; set; }

        /// <summary>城市名称
        /// </summary>
        public String CityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsOrderDeliverInfo()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="totalWeight">订单总重</param>
        /// <param name="carriage">快递运费</param>
        /// <param name="expressId">快递公司Id</param>
        /// <param name="expressNo"></param>
        /// <param name="worngValue"></param>
        public GoodsOrderDeliverInfo(Guid orderId,double totalWeight,double carriage,Guid expressId,string expressNo,double worngValue)
        {
            OrderId = orderId;
            TotalWeight = totalWeight;
            CarriageFee = carriage;
            ExpressId = expressId;
            ExpressNo = expressNo;
            MaxWrongValue = worngValue;
        }
    }
}
