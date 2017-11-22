using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyPurchaseGoupDao
    {
        int Insert(CompanyPurchaseGoupInfo info, out string errorMessage);

        bool InsertList(List<CompanyPurchaseGoupInfo> list, out string errorMessage);

        int Update(CompanyPurchaseGoupInfo info, out string errorMessage);

        IList<CompanyPurchaseGoupInfo> GetCompanyPurchaseGoupList(Guid companyId);

        bool IsExist(Guid companyId, Guid purchaseGroupId);

        CompanyPurchaseGoupInfo GetCompanyPurchaseGoupInfo(Guid purchaseGroupId);

        bool Delete(Guid companyId, Guid purchaseGroupId, out string errorMessage);
    }
}
