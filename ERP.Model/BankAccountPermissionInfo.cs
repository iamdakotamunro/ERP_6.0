using System;

namespace ERP.Model
{
    /// <summary>
    /// 资金账户信息类，原模型名称：PersonnelBankAccountsInfo，db:lmshop_PersonnelBankAccountsInfo
    /// </summary>
    [Serializable]
    public class BankAccountPermissionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 资金银行
        /// </summary>
        public string BankName { get; set; }


        /// <summary>
        /// 公司id
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// 职务id
        /// </summary>
        public Guid PositionID { get; set; }

        /// <summary>
        /// 职务名
        /// </summary>
        public string PositionName { get; set; }
    }
}
