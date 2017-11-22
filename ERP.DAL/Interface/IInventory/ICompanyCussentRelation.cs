using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyCussentRelation
    {
        IList<CompanyCussentRelationInfo> GetCompanyCussentRelationInfoList(Guid companyId);

        IList<CompanyCussentRelationInfo> GetEditCompanyCussentRelationInfoList(String accountNo, Guid companyId);

        bool Insert(List<CompanyCussentRelationInfo> companyCussentRelation);

        bool Update(List<CompanyCussentRelationInfo> companyCussentRelation);

        void Delete(String accountNo, Guid companyId);

        bool IsExist(Guid companyId);

        List<AuthorizeCompanyDTO> GetAuthorizeCompanyDtos(string accountNo, Guid saleFilialeId);

        bool Save(List<Guid> deleteIds, List<CompanyCussentRelationInfo> insertList);
    }
}
