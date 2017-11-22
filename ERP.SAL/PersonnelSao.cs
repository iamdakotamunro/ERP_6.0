using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.SAL.Interface;

namespace ERP.SAL
{
    public class PersonnelSao : IPersonnelSao
    {
        public IList<PersonnelInfo> GetList()
        {
            var list = new List<PersonnelInfo>();
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var items = client.Instance.GetAllLoginAccount();
                list.AddRange(items.Select(item => new PersonnelInfo(item))
                    .Where(personnelInfo => personnelInfo.PersonnelId != Guid.Empty && personnelInfo.IsActive));
            }
            return list;
        }

        public PersonnelInfo Get(Guid personnelId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.GetAccountInfoByPersonnelId(personnelId);
                return new PersonnelInfo(info);
            }
        }

        public PersonnelInfo Get(string accountNo)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.GetAccountInfo(accountNo);
                return new PersonnelInfo(info);
            }
        }

        public IList<PersonnelInfo> GetAccountInfoByRealName(string realName)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.GetAccountInfoByRealName(realName);
                List<PersonnelInfo> personnel = new List<PersonnelInfo>();
                foreach (var loginAccountInfo in info)
                {
                    personnel.Add(new PersonnelInfo
                    {
                        PersonnelId = loginAccountInfo.PersonnelId,
                        RealName = loginAccountInfo.RealName,
                        AccountNo = loginAccountInfo.AccountNo,
                        EnterpriseNo = loginAccountInfo.AccountNo,
                        IsActive = loginAccountInfo.IsActive,
                        SystemBrandPositionId = loginAccountInfo.SystemBranchPositionID,
                        FilialeId = loginAccountInfo.PositionInfo != null ? loginAccountInfo.PositionInfo.FilialeId : Guid.Empty,
                        BranchId = loginAccountInfo.PositionInfo != null ? loginAccountInfo.PositionInfo.BranchId : Guid.Empty,
                        PositionId = loginAccountInfo.PositionInfo != null ? loginAccountInfo.PositionInfo.ID : Guid.Empty,
                        CurrentFilialeId = Guid.Empty
                    });
                }
                return personnel;
            }
        }


        public string GetName(Guid personnelId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.GetAccountInfoByPersonnelId(personnelId);
                if (info != null)
                {
                    return info.RealName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<PersonnelInfo> GetList(Guid filialeId, Guid branchId)
        {
            var list = new List<PersonnelInfo>();
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var items = client.Instance.GetAllLoginAccount();
                list.AddRange(items.Select(item => new PersonnelInfo(item))
                    .Where(personnelInfo => personnelInfo.PersonnelId != Guid.Empty && personnelInfo.IsActive
                        && personnelInfo.FilialeId == filialeId && personnelInfo.BranchId == branchId));
            }
            return list;
        }
    }
}
