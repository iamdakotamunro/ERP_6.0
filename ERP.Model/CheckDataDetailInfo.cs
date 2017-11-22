using System;

namespace ERP.Model
{
    /// <summary>
    /// 对账记录明细模型
    /// </summary>
    [Serializable]
    public class CheckDataDetailInfo
    {
        /// <summary>
        /// 对账记录id
        /// </summary>
        public Guid CheckId { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 上传Excel金额
        /// </summary>
        public decimal PostMoney { get; set; }

        /// <summary>
        /// 上传Excel重量(代收为0，非代收记录)
        /// </summary>
        public double PostWeight { get; set; }

        /// <summary>
        /// 系统金额
        /// </summary>
        public decimal ServerMoney { get; set; }

        /// <summary>
        /// 系统重量
        /// </summary>
        public double ServerWeight { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        public decimal DiffMoney { get; set; }

        /// <summary>
        /// 系统确认金额
        /// </summary>
        public decimal SystemConfirmMoney { get; set; }

        /// <summary>
        /// 财务确认金额
        /// </summary>
        public decimal FinanceConfirmMoney { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 错误记录
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int DataState { get; set; }

        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public String FilialeName { get; set; }

        /// <summary>
        /// </summary>
        public Boolean IsOut { get; set; }

        /// <summary>省份名称
        /// </summary>
        public String ProvinceName { get; set; }

        /// <summary>城市名称
        /// </summary>
        public String CityName { get; set; }

    }
}
