using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// ������λ������
    /// </summary>
    public class CompanyClass : BllInstance<CompanyClass>
    {
        readonly ICompanyClass _companyClassDao;

        public CompanyClass(Environment.GlobalConfig.DB.FromType fromType)
        {
            _companyClassDao = InventoryInstance.GetCompanyClassDao(fromType);
        }

        public CompanyClass(ICompanyClass companyClass)
        {
            _companyClassDao = companyClass;
        }

        /// <summary>
        /// ɾ��������λ����
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
        public void Delete(Guid companyClassId)
        {
            if (companyClassId != Guid.Empty)
            {
                if (_companyClassDao.GetFireCompanyCount(companyClassId) <= 0)
                {
                    if (_companyClassDao.GetChildCompanyClassCount(companyClassId) <= 0)
                    {
                        _companyClassDao.Delete(companyClassId);
                    }
                    else
                    {
                        throw new ApplicationException("��������λ���������������࣬������ɾ����");
                    }
                }
                else
                {
                    throw new ApplicationException("��������λ���е�λʹ�ã�������ɾ����");
                }
            }
        }
    }
}
