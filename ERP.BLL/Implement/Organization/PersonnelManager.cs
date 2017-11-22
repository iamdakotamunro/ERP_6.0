using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Organization
{
    public class PersonnelManager
    {
        private readonly IPersonnelSao _personnelSao;

        public PersonnelManager()
        {
            _personnelSao=new PersonnelSao();
        }

        public PersonnelManager(IPersonnelSao personnelSao)
        {
            _personnelSao = personnelSao;
        }

        public PersonnelInfo Get(string accountNo)
        {
            return _personnelSao.Get(accountNo);
        }

        public PersonnelInfo Get(Guid personnelId)
        {
            return _personnelSao.Get(personnelId);
        }

        public string GetName(Guid personnelId)
        {
            return _personnelSao.GetName(personnelId);
        }

        public IEnumerable<PersonnelInfo> GetList()
        {
            return _personnelSao.GetList();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PersonnelInfo> GetList(Guid filialeId, Guid branchId)
        {
            return _personnelSao.GetList(filialeId,branchId);
        }
    }
}
