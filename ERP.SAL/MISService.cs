using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using MIS.Model.View;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;
using SalePlatformInfo = Keede.Ecsoft.Model.SalePlatformInfo;
using MIS.Enum;
using ERP.Environment;

namespace ERP.SAL
{
    public class MISService
    {
        private static IList<FilialeInfo> _filialeLocalCache = null;
        private static object _filialeLocalCacheLocker = new object();
        private static System.Timers.Timer _filialeCacheRefresher = new System.Timers.Timer(60 * 1000);// 1分钟更新一次

        static MISService()
        {
            _filialeCacheRefresher.Elapsed += RefreshFilialeCache;
            _filialeCacheRefresher.Start();

        }

        private static void RefreshFilialeCache(object sender, ElapsedEventArgs e)
        {
            _filialeCacheRefresher.Stop();
            _filialeCacheRefresher.Enabled = false;

            try
            {
                Interlocked.Exchange(ref _filialeLocalCache, GetAllMisFiliales().ToList());
            }
            catch { }
            finally
            {
                _filialeCacheRefresher.Enabled = true;
                _filialeCacheRefresher.Start();
            }
        }

        /// <summary>
        /// 读取公司信息
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<FilialeInfo> GetAllMisFiliales()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var items = client.Instance.GetAllFiliale();
                if (items != null && items.Any())
                {
                    foreach (var item in items.OrderBy(w => w.OrderIndex))
                    {
                        if (item.IsActive)
                        {
                            yield return new FilialeInfo
                            {
                                ID = item.ID,
                                Address = item.Address,
                                Code = item.Code,
                                Description = item.Description,
                                Name = item.Name,
                                RealName = item.RealName,
                                ParentId = item.ParentId,
                                Rank = item.Rank,
                                ShopJoinType = item.ShopJoinType,
                                TaxNumber = item.TaxNumber,
                                RegisterAddress = item.RegisterAddress,
                                IsActive = item.IsActive,
                                FilialeTypes = item.FilialeTypes,
                                GoodsTypes = item.GoodsTypes,
                                IsSaleFiliale = item.IsSaleFiliale
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有公司集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FilialeInfo> GetAllFiliales()
        {
            if (_filialeLocalCache != null)
            {
                return _filialeLocalCache;
            }
            lock (_filialeLocalCacheLocker)
            {
                if (_filialeLocalCache == null)
                {
                    Interlocked.CompareExchange(ref _filialeLocalCache, GetAllMisFiliales().ToList(), null);
                }
                return _filialeLocalCache;
            }
        }
        /// <summary>
        /// 获取公司列表 
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllFilialeList()
        {
            return GetAllFiliales().Where(ent => ent.Rank == (int)FilialeRank.Head && ent.ID != GlobalConfig.ERPFilialeID).ToList();
        }

        /// <summary>
        /// 获取销售公司和物流配送公司列表 
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllSaleAndHostingFilialeList()
        {
            return GetAllFiliales().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany) || ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
        }

        /// <summary>
        /// 获取销售公司列表 
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllSaleList()
        {
            return GetAllFiliales().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
        }

        /// <summary>
        /// 获取物流配送公司列表 
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllHostingFilialeList()
        {
            return GetAllFiliales().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany)).ToList();
        }

        /// <summary>
        /// 获取门店集合
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllShopList()
        {
            return GetAllFiliales().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop)).ToList();
        }

        /// <summary>
        /// 获取所有销售平台集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SalePlatformInfo> GetAllSalePlatform()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var items = client.Instance.GetAllSalePlatform().ToList();
                foreach (var item in items)
                {
                    yield return new SalePlatformInfo
                    {
                        FilialeId = item.FilialeId,
                        ID = item.ID,
                        IsActive = item.IsActive,
                        Name = item.Name,
                        Url = item.Url,
                        ExternalName = item.ExternalName,
                        AccountCheckingType = item.AccountCheckingType
                    };
                }
            }
        }

        public static IList<MenuInfo> GetMenuList(Guid systemId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetMenuList(systemId).OrderBy(w => w.OrderIndex).ToList();
            }
        }

        /// <summary>
        /// 获取所有部门集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BranchInfo> GetAllBranch()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetAllFilialeBranch().OrderBy(w => w.OrderIndex).ToList();
            }
        }

        /// <summary>
        /// 获取所有职位
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PositionInfo> GetAllFilialeBranchPositionList()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetAllPosition().OrderBy(w => w.OrderIndex).ToList();
            }
        }

        public static FilialeInfo GetFiliaeInfo(Guid filialeId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var item = client.Instance.GetFilialeInfo(filialeId);
                if (item == null)
                    return null;
                return new FilialeInfo
                {
                    Address = item.Address,
                    Code = item.Code,
                    ID = item.ID,
                    IsActive = item.IsActive,
                    ParentId = item.ParentId,
                    Name = item.Name,
                    Rank = item.Rank,
                    FilialeTypes = item.FilialeTypes,
                    ShopJoinType = item.ShopJoinType,
                    GoodsTypes = item.GoodsTypes
                };
            }
        }

        #region  获取系统部门和职务
        /// <summary>
        /// 获取所有部门集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SystemBranchInfo> GetAllSystemBranch()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetAllSystemBranch().OrderBy(w => w.OrderIndex).ToList();
            }
        }

        /// <summary>
        /// 获取所有职位
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SystemPositionInfo> GetAllSystemPositionList()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetSystemBranchPosition().OrderBy(w => w.OrderIndex).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filialeiId"></param>
        /// <param name="systemBranchId"></param>
        /// <returns></returns>
        public static IList<LoginAccountInfo> GetLoginAccountListBySystemBranchId(Guid filialeiId, Guid systemBranchId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetLoginAccountListBySystemBranchId(filialeiId, systemBranchId);
            }
        }
        #endregion

        #region

        public static LoginAccountInfo GetAccountInfoByPersonnelId(Guid personnelId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.GetAccountInfoByPersonnelId(personnelId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personnelIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, Guid> GetBranchIdsByPersonnelIds(IList<Guid> personnelIds)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var result = client.Instance.GetAccountsByPersonnelIds(personnelIds);
                if (result != null)
                {
                    result = result.Where(p => p != null && p.PositionInfo != null && (!p.PositionInfo.BranchId.Equals(Guid.Empty))).ToList();
                    return result.ToDictionary(k => k.PersonnelId, v => v.PositionInfo.BranchId);
                }
                return new Dictionary<Guid, Guid>();
            }
        }
        #endregion
    }
}
