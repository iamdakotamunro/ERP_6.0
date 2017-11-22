using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MIS.Model.View;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PersonnelInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginAccountInfo"></param>
        public PersonnelInfo(LoginAccountInfo loginAccountInfo)
        {
            if (loginAccountInfo != null)
            {
                PersonnelId = loginAccountInfo.PersonnelId;
                RealName = loginAccountInfo.RealName;
                AccountNo = loginAccountInfo.AccountNo;
                EnterpriseNo = loginAccountInfo.AccountNo;
                IsActive = loginAccountInfo.IsActive;
                SystemBrandPositionId = loginAccountInfo.SystemBranchPositionID;
                if (loginAccountInfo.PositionInfo != null)
                {
                    FilialeId = loginAccountInfo.PositionInfo.FilialeId;
                    BranchId = loginAccountInfo.PositionInfo.BranchId;
                    PositionId = loginAccountInfo.PositionInfo.ID;
                }
            }
        }

        public PersonnelInfo()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid PersonnelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid BranchId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid PositionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 系统部门职务ID
        /// </summary>
        public Guid SystemBrandPositionId { get; set; }

        /// <summary>
        /// 当前缓存公司
        /// </summary>
        public Guid CurrentFilialeId { get; set; }

    }
}
