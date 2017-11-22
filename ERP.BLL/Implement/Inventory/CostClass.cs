using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 费用分类
    /// </summary>
    public class Cost : BllInstance<Cost>
    {
        private readonly ICost _costDao;
        private readonly ICostCussent _costCussent;
        public Cost(Environment.GlobalConfig.DB.FromType fromType)
        {
            _costDao = InventoryInstance.GetCostDao(fromType);
            _costCussent = InventoryInstance.GetCostCussentDao(fromType);
        }

        /// <summary>
        /// 费用分类
        /// </summary>
        /// <param name="companyClassId">费用分类Id</param>
        /// <param name="companyId">具体类别Id</param>
        /// <returns></returns>
        /// zal 2015-11-13
        public string GetCompanyName(Guid companyClassId, Guid companyId)
        {
            string companyName = string.Empty;
            if (!companyClassId.Equals(Guid.Empty))
            {
                var costCompanyClassInfo = _costDao.GetCompanyClass(companyClassId);
                if (costCompanyClassInfo != null)
                {
                    companyName = costCompanyClassInfo.CompanyClassName;
                    if (!companyId.Equals(Guid.Empty))
                    {
                        var costCussentInfo = _costCussent.GetCompanyCussent(companyId);
                        if (costCussentInfo != null)
                        {
                            companyName += "－" + costCussentInfo.CompanyName;
                        }
                    }
                }
            }
            return companyName;
        }
    }
}
