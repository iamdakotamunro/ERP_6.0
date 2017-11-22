using System;

namespace ERP.Model
{
    /// <summary>
    /// ���õ�λȨ�޿�����
    /// </summary>
    [Serializable]
    public class CostPermissionInfo
    {
        /// <summary>
        /// ���õ�λID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ���õ�λ����
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// ��˾ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// ��˾��
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public Guid BranchID { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string BranchName { get; set; }
    }
}
