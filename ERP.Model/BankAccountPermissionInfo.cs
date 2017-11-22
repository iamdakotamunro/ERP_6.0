using System;

namespace ERP.Model
{
    /// <summary>
    /// �ʽ��˻���Ϣ�࣬ԭģ�����ƣ�PersonnelBankAccountsInfo��db:lmshop_PersonnelBankAccountsInfo
    /// </summary>
    [Serializable]
    public class BankAccountPermissionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// �ʽ�����
        /// </summary>
        public string BankName { get; set; }


        /// <summary>
        /// ��˾id
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// ��˾��
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>
        /// ����id
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// ְ��id
        /// </summary>
        public Guid PositionID { get; set; }

        /// <summary>
        /// ְ����
        /// </summary>
        public string PositionName { get; set; }
    }
}
