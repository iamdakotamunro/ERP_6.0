using System;
using System.Collections.Generic;
using System.Linq;
using B2C.Service.Contract;
using Framework.WCF;
using MIS.Enum;
using MIS.Model.View;

namespace ERP.SAL
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ClientProxy
    {
        private static IList<SystemInfo> _systemList;

        static ClientProxy()
        {
            if (_systemList == null)
            {
                using (var client = new ServiceClient<MIS.Service.Contract.IService>("Group.MIS"))
                {
                    _systemList = client.Instance.GetAllSystem();
                }
            }
        }

        #region -- MIS Client

        internal static ServiceClient<MIS.Service.Contract.IService> CreateMISWcfClient()
        {
            return new ServiceClient<MIS.Service.Contract.IService>("Group.MIS");
        }

        internal static ServiceClient<MIS.Service.Contract.IService> CreateMISServiceClient()
        {
            return new ServiceClient<MIS.Service.Contract.IService>("Group.MIS");
        }
        #endregion

        #region -- HRS Client

        internal static ServiceClient<HRS.WCF.Contract.IService> CreateHRSWcfClient()
        {
            return new ServiceClient<HRS.WCF.Contract.IService>("Group.HRS");
        }
        #endregion
      
        #region -- B2C Client

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        internal static ServiceClient<IKeedeAdmin> CreateB2CWcfClient(Guid saleFilialeId)
        {
            var endPointName = GetEndPointName(saleFilialeId, ServiceType.B2C);
            return new ServiceClient<IKeedeAdmin>(endPointName);
        }

        #endregion

        #region -- Shop Client
        ///// <summary>
        ///// 门店
        ///// </summary>
        ///// <param name="saleFilialeId"></param>
        ///// <returns></returns>
        //internal static ServiceClient<ShopSystem.Company.Service.Interface.IService> CreateShopWcfClient(Guid saleFilialeId)
        //{
        //    var endPointName = GetEndPointName(saleFilialeId, ServiceType.Shop);
        //    return new ServiceClient<ShopSystem.Company.Service.Interface.IService>(endPointName);
        //}

        /// <summary>
        /// 加盟店
        /// </summary>
        /// <returns></returns>
        internal static ServiceClient<AllianceShop.Contract.IDomainService> CreateShopStoreWcfClient(Guid saleFilialeId)
        {
            var endPointName = GetEndPointName(saleFilialeId, ServiceType.Shop);
            if (endPointName.Length == 0)
            {
                var filialeInfo = MISService.GetAllFiliales().FirstOrDefault(ent => ent.ID == saleFilialeId);
                if (filialeInfo != null && filialeInfo.ParentId != Guid.Empty)
                    endPointName = GetEndPointName(filialeInfo.ParentId, ServiceType.Shop);
                else
                    endPointName = "Shop.Keede";
            }
            return new ServiceClient<AllianceShop.Contract.IDomainService>(endPointName);
        }

        #endregion

        #region -- EndPointName

        /// <summary>获取总结点名称
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        private static string GetEndPointName(Guid filialeId, ServiceType serviceType)
        {
            //再次判断是否为空
            if (_systemList == null || _systemList.Count == 0)
            {
                using (var client = new ServiceClient<MIS.Service.Contract.IService>("Group.MIS"))
                {
                    _systemList = client.Instance.GetAllSystem();
                }
            }
            var info = _systemList.FirstOrDefault(ent => ent.FilialeId == filialeId && ent.ServiceType != null && ent.ServiceType.ToLower() == serviceType.ToString().ToLower());
            if (info != null)
            {
                return info.ServiceEndPointName;
            }
            return string.Empty;
        }

        #endregion

    }
}
