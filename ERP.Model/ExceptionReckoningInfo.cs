using System;

namespace ERP.Model
{
    /// <summary>异常往来帐金额模型（对账服务使用）  2015-03-26  陈重文
    /// </summary>
    public class ExceptionReckoningInfo
    {
        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public String FilialeName { get; set; }

        /// <summary>异常金额
        /// </summary>
        public Decimal DiffMoney { get; set; }

        /// <summary>确认总金额(系统确认金额--->系统确认金额) （对账完成最后一步使用，其他地方无用）
        /// </summary>
        public Decimal SumConfirmMoney { get; set; }

        /// <summary>实际总金额 (财务确认金额--->对账金额)（对账完成最后一步使用，其他地方无
        /// </summary>
        public Decimal SumFinanceConfirmMoney { get; set; }

        /// <summary>
        /// </summary>
        public Boolean IsOut { get; set; }
    }
}
