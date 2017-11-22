using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 往来单位控制类
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
        /// 删除往来单位分类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
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
                        throw new ApplicationException("该往来单位分类仍有下属分类，不允许删除！");
                    }
                }
                else
                {
                    throw new ApplicationException("该往来单位仍有单位使用，不允许删除！");
                }
            }
        }
    }
}
