using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.Cache.Common;
using ERP.DAL.Factory;
using ERP.Environment;
using ERP.SAL;
using Keede.Ecsoft.Model;
using MIS.Enum;
using MIS.Model.View;
using FilialeInfo = Keede.Ecsoft.Model.FilialeInfo;
using SalePlatformInfo = Keede.Ecsoft.Model.SalePlatformInfo;

namespace ERP.UI.Web.Common
{
    /// <summary>公司，销售平台，部门，职务不使用缓存存储，直接从MIS服务获取  2015-04-27  陈重文
    /// Func : The Cache Data Collection of this Application
    /// Coder: lcj
    /// Date : 2013 5 28th
    /// </summary>
    public class CacheCollection
    {
        /// <summary>
        /// 获取所有往来单位收付款审核权限的信息
        /// Add by liucaijun at 2011-June-15th
        /// </summary>
        /// <returns></returns>
        public static IList<CompanyAuditingPowerInfo> GetAllCompanyAuditingPowerList()
        {
            return CacheHelper.Get(Key.AllCompanyAuditingPower, () => InventoryInstance.GetCompanyAuditingPowerDao(GlobalConfig.DB.FromType.Read).GetALLCompanyAuditingPower());
        }

        #region [公司缓存]

        public class Filiale
        {
            /// <summary>
            /// 获取所有公司
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetList()
            {
                //公司信息不缓存
                //return CacheHelper.Get(Key.FilialeList, MISService.GetAllFiliales).ToList();
                return MISService.GetAllFiliales().Where(ent=>ent.IsActive).ToList();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filialeId"></param>
            /// <returns></returns>
            public static FilialeInfo Get(Guid filialeId)
            {
                return GetList().FirstOrDefault(ent => ent.ID == filialeId);
            }


            public static bool IsB2CFiliale(Guid filialeId)
            {
                var info = Get(filialeId);
                if (info == null)
                {
                    return false;
                }
                return info.FilialeTypes.Contains((int)FilialeType.SaleCompany);
            }


            public static string GetName(Guid filialeId)
            {
                var info = Get(filialeId);
                if (info != null)
                {
                    return info.Name;
                }
                return string.Empty;
            }

            /// <summary>
            /// 根据银行账号Id获取结算公司名称和结算公司Id
            /// </summary>
            /// <param name="bankaccountId">银行账号Id</param>
            /// <returns></returns>
            /// zal 2015-11-11
            public static string GetFilialeNameAndFilialeId(Guid bankaccountId)
            {
                string filialeName = "-", filialeId = Guid.Empty.ToString();
                if (!bankaccountId.Equals(Guid.Empty))
                {
                    //主账号一定会绑定公司(即存在绑定表中)，一个公司有多个主账号
                    var bankAccountBindList = BankAccountManager.ReadInstance.GetList(bankaccountId);
                    var filialeInfoList = GetHeadList();
                    var joinQuery = (from bankAccountBind in bankAccountBindList
                                     join filialeInfo in filialeInfoList
                                     on bankAccountBind.TargetId equals filialeInfo.ID
                                     select new
                                     {
                                         filialeName = filialeInfo.Name,
                                         filialeId = filialeInfo.ID
                                     }).ToList();

                    if (joinQuery.Any())
                    {
                        var firstOrDefault = joinQuery.First();
                        filialeName = firstOrDefault.filialeName;
                        filialeId = firstOrDefault.filialeId.ToString();
                    }
                    else
                    {
                        filialeName = "ERP公司";
                        filialeId = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID");
                    }
                }
                return filialeName + "," + filialeId;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="codeKey"></param>
            /// <returns></returns>
            public static FilialeInfo Get(string codeKey)
            {
                return GetList().FirstOrDefault(ent => ent.Code.Contains(codeKey));
            }

            /// <summary>
            /// 获取总公司列表 
            /// </summary>
            /// <returns></returns>
            public static FilialeInfo GetERPCompany()
            {
                return GetList().FirstOrDefault(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && ent.Rank == (int)FilialeRank.Head);
            }

            /// <summary>
            /// 获取总公司列表 
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetHeadList()
            {
                return GetHostingAndSaleFilialeList();
                //const int RANK = (int)FilialeRank.Head;
                //var list = GetList();
                //return list.Where(ent => ent.Rank == RANK && ent.ID != GlobalConfig.ERPFilialeID && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            }

            /// <summary>
            /// 获取总公司列表 
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetPlaformAndShopFilialeList()
            {
                const int RANK = (int)FilialeRank.Head;
                var list = GetList();
                return list.Where(ent => ent.Rank == RANK && (ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) || ent.FilialeTypes.Contains((int)FilialeType.EntityShop))).ToList();
            }

            /// <summary>
            /// 获取门店公司列表 
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetShopList()
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop)).ToList();
            }

            /// <summary>
            /// 获取门店总公司列表 
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetShopHeadCompanyList()
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop) && ent.Rank == (int)FilialeRank.Head).ToList();
            }

            /// <summary>
            /// 根据公司类型获取公司列表
            /// </summary>
            /// <param name="filialeType"></param>
            /// <returns></returns>
            public static IList<FilialeInfo> GetShopAllianceList(int filialeType)
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains(filialeType)).ToList();
            }


            /// <summary>
            /// 获取子门店公司列表 
            /// </summary>
            /// <returns></returns>
            public static IList<FilialeInfo> GetChildShopList(Guid parentId)
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop) && ent.ParentId == parentId).ToList();
            }

            /// <summary>
            /// add by liangcanren at 2015-03-26 
            /// 判断是否为门店
            /// </summary>
            /// <param name="shopId"></param>
            /// <returns></returns>
            public static bool IsShop(Guid shopId)
            {
                return GetList().Any(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop) && ent.ID == shopId);
            }

            /// <summary>
            /// 获取所有的物流公司
            /// </summary>
            /// <returns></returns>
            public static List<FilialeInfo> GetHostingFilialeList()
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany)).ToList();
            }

            /// <summary>
            /// 获取所有的销售公司
            /// </summary>
            /// <returns></returns>
            public static List<FilialeInfo> GetSaleFilialeList()
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            }

            /// <summary>
            /// 获取所有的物流公司
            /// </summary>
            /// <returns></returns>
            public static List<FilialeInfo> GetHostingAndSaleFilialeList()
            {
                return GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany) || ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
            }
        }

        #endregion

        #region [销售平台缓存]

        /// <summary> 销售平台缓存
        /// </summary>
        public class SalePlatform
        {
            /// <summary>
            /// 获取所有销售平台
            /// </summary>
            /// <returns></returns>
            public static IList<SalePlatformInfo> GetList()
            {
                //return CacheHelper.Get(Key.SalePlatformList, MISService.GetAllSalePlatform).ToList();
                return MISService.GetAllSalePlatform().ToList();
            }

            public static IList<SalePlatformInfo> GetListByFilialeId(Guid filialeId)
            {
                return GetList().Where(ent => ent.FilialeId == filialeId).ToList();
            }

            public static SalePlatformInfo Get(Guid salePlatformId)
            {
                return GetList().FirstOrDefault(ent => ent.ID == salePlatformId);
            }
        }

        #endregion

        #region [部门缓存]

        /// <summary> 部门缓存
        /// </summary>
        public class Branch
        {
            /// <summary>
            /// 获取所有部门
            /// </summary>
            /// <returns></returns>
            public static IList<BranchInfo> GetList()
            {
                //return CacheHelper.Get(Key.BranchList, MISService.GetAllBranch).ToList();
                return MISService.GetAllBranch().ToList();
            }

            /// <summary>
            /// 获取所有部门
            /// </summary>
            /// <returns></returns>
            public static IList<BranchInfo> GetList(Guid filialeId)
            {
                return GetList().Where(b => b.FilialeId == filialeId).ToList();
            }

            /// <summary>
            /// 获取所有部门
            /// </summary>
            /// <returns></returns>
            public static IList<BranchInfo> GetList(Guid filialeId, Guid parentBranchId)
            {
                var list = GetList();
                return list.Where(b => b.FilialeId == filialeId && b.ParentBranchId == parentBranchId).ToList();
            }

            public static string GetName(Guid filialeId, Guid branchId)
            {
                var info = Get(filialeId, branchId);
                if (info != null)
                {
                    return info.Name;
                }
                return string.Empty;
            }

            public static BranchInfo Get(Guid filialeId, Guid branchId)
            {
                return GetList().FirstOrDefault(ent => ent.ID == branchId);
            }

            /// <summary>
            /// 获取所有的系统部门
            /// </summary>
            /// <returns></returns>
            public static IList<SystemBranchInfo> GetSystemBranchList()
            {
                return CacheHelper.Get(Key.AllSystemBranch, MISService.GetAllSystemBranch).ToList();
                //return MISService.GetAllSystemBranch().ToList();
            }

            /// <summary>
            /// 获取系统部门下的组
            /// </summary>
            /// <returns></returns>
            public static IList<SystemBranchInfo> GetSystemBranchListByBranchId(Guid branchId)
            {
                return GetSystemBranchList().Where(act => act.ParentID == branchId).ToList();
            }
        }

        #endregion

        #region [职务缓存]

        /// <summary>职务缓存
        /// </summary>
        public class Position
        {
            /// <summary>
            /// 获取所有职位
            /// </summary>
            /// <returns></returns>
            public static IList<PositionInfo> GetList()
            {
                return CacheHelper.Get(Key.GetAllFilialeBranchPositionList, MISService.GetAllFilialeBranchPositionList).ToList();
            }

            /// <summary>
            /// 获取所有职位
            /// </summary>
            /// <returns></returns>
            public static IList<PositionInfo> GetList(Guid filialeId)
            {
                return GetList().Where(ent => ent.FilialeId == filialeId).ToList();
            }

            /// <summary>
            /// 获取所有职位
            /// </summary>
            /// <returns></returns>
            public static IList<PositionInfo> GetList(Guid filialeId, Guid branchId)
            {
                return GetList().Where(ent => ent.FilialeId == filialeId && ent.BranchId == branchId).ToList();
            }

            public static PositionInfo Get(Guid filialeId, Guid branchId, Guid positionId)
            {
                return GetList().FirstOrDefault(ent => ent.ID == positionId);
            }

            public static string GetName(Guid filialeId, Guid branchId, Guid positionId)
            {
                var info = Get(filialeId, branchId, positionId);
                if (info != null)
                {
                    return info.Name;
                }
                return string.Empty;
            }

            /// <summary>
            /// 获取所有职位
            /// </summary>
            /// <returns></returns>
            public static IList<SystemPositionInfo> GetSystemPositionList()
            {
                return CacheHelper.Get(Key.AllSystemPosition, MISService.GetAllSystemPositionList).ToList();
            }

            /// <summary>
            /// 通过员工绑定的系统职务id查询系统职务
            /// </summary>
            /// <param name="branchPositionId"></param>
            /// <returns></returns>
            public static SystemPositionInfo GetPostionBySystemBrachPositionId(Guid branchPositionId)
            {
                return GetSystemPositionList().FirstOrDefault(ent => ent.SystemBranchPositionID == branchPositionId);
            }
        }

        #endregion
    }
}