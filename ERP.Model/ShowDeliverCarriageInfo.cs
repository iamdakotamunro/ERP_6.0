using System;

namespace ERP.Model
{
    /// <summary>
    /// 城市绑定运费显示模型
    /// </summary>
    [Serializable]
    public class ShowDeliverCarriageInfo
    {
        /// <summary>
        /// 城市下辖区分组数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        ///城市Id
        /// </summary>
        public Guid CityID { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 分组最大数
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// 快递Id
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public int PayMode { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public Guid WarehouseID { get; set; }

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
        public double AddWeightFee { get; set; }

        /// <summary>
        /// 中转费
        /// </summary>
        public double TransferFee { get; set; }

        /// <summary>
        /// 面单费
        /// </summary>
        public double BaseFee { get; set; }

        /// <summary>
        /// 操作费
        /// </summary>
        public double OperaterFee { get; set; }

        /// <summary>
        /// 计费方式
        /// </summary>
        public int ChargingMode { get; set; }

        public ShowDeliverCarriageInfo()
        {
            
        }

        public ShowDeliverCarriageInfo(int totalCount,Guid cityId,int maxCount,Guid expressId,int payMode,Guid filialeId,Guid warehouseId,double initialWeight,
            double initialCarriage)
        {
            TotalCount = totalCount;
            CityID = cityId;
            MaxCount = maxCount;
            ExpressId = expressId;
            PayMode = payMode;
            FilialeID = filialeId;
            WarehouseID = warehouseId;
            InitialWeight = initialWeight;
            InitialCarriage = initialCarriage;
        }
    }
}
