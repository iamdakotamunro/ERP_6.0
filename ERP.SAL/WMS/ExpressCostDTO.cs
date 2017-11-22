using System;

namespace ERP.SAL.WMS
{
    public class ExpressCostDTO
    {
        public Guid DistrictId { get; set; }

        public Guid ExpressId { get; set; }

        /// <summary>
        /// 计费方式，参考枚举ExpressCostBillingMethod
        /// </summary>
        public byte BillingMethod { get; set; }

        /// <summary>
        /// 首重
        /// </summary>
        public double InitialWeight { get; set; }

        /// <summary>
        /// 首重运费
        /// </summary>
        public double InitialCarriage { get; set; }

        /// <summary>
        /// 续重运费
        /// </summary>
        public double ContinuedWeightFee { get; set; }

        /// <summary>
        /// 中转费
        /// </summary>
        public double TransitFee { get; set; }

        /// <summary>
        /// 面单费
        /// </summary>
        public double SurfaceFee { get; set; }

        /// <summary>
        /// 操作费
        /// </summary>
        public double OperateFee { get; set; }
    }
}
