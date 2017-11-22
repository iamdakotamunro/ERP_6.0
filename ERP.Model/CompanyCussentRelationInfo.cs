using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位类关权限联表
    /// </summary>
    [Serializable]
    public class CompanyCussentRelationInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public String AccountNo { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        
        /// <summary>
        /// 往来单位编号
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售公司名称
        /// </summary>
        public string SaleFilialeName { get; set; }

        public CompanyCussentRelationInfo() { }

        public CompanyCussentRelationInfo(Guid id,String accountNo,String accountName,Guid companyId,String companyName,Guid saleFilialeId,String saleFilialeName)
        {
            Id = id;
            AccountNo = accountNo;
            AccountName = accountName;
            CompanyId = companyId;
            CompanyName = companyName;
            SaleFilialeId = saleFilialeId;
            SaleFilialeName = saleFilialeName;
        }
    }
}
