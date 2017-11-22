using ERP.Enum;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 记账本类型类
    /// </summary>
    public class WasteTypeInfo
    {
        /// <summary>
        /// 本模型共用到4种类型：0 增加资金、1 减少资金、5 转出资金有手续费、6 转出资金无手续费
        /// </summary>
        public WasteBookType WasteBookType { get; set; }

        /// <summary>
        /// 增加资金
        /// </summary>
        public WasteBookInfo Increase { get; set; }

        /// <summary>
        /// 减少资金
        /// </summary>
        public WasteBookInfo Decrease { get; set; }

        /// <summary>
        /// 转入资金
        /// </summary>
        public WasteBookInfo TransferIn { get; set; }

        /// <summary>
        /// 转出资金
        /// </summary>
        public WasteBookInfo TransferOut { get; set; }

        /// <summary>
        /// 转出手续费
        /// </summary>
        public WasteBookInfo TransferFee { get; set; }


        /// <summary>
        /// Default Contructor
        /// </summary>
        public WasteTypeInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wasteBookType">0 增加资金、1 减少资金、5 转出资金有手续费、6 转出资金无手续费</param>
        /// <param name="increase">增加资金</param>
        /// <param name="decrease">减少资金</param>
        /// <param name="transferIn">转入资金</param>
        /// <param name="transferOut">转出资金</param>
        /// <param name="transferFee">转出手续费</param>
        public WasteTypeInfo(WasteBookType wasteBookType, WasteBookInfo increase, WasteBookInfo decrease, WasteBookInfo transferIn, WasteBookInfo transferOut, WasteBookInfo transferFee)
        {
            WasteBookType = wasteBookType;
            Increase = increase;
            Decrease = decrease;
            TransferIn = transferIn;
            TransferOut = transferOut;
            TransferFee = transferFee;
        }


    }
}