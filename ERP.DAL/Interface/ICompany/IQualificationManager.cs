using ERP.Model;
using System;
using System.Collections.Generic;

namespace ERP.DAL.Interface.ICompany
{
    public interface IQualificationManager
    {
        IList<SupplierInformationInfo> GetSupplierQualificationBySupplierId(Guid supplierId);

        bool Insert(SupplierInformationInfo supplierInformationInfo);

        bool Delete(Guid supplierInformationId);

        bool Update(SupplierInformationInfo supplierInformationInfo);
    }
}
