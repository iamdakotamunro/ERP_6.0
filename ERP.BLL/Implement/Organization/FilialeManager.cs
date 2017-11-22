using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Environment;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Organization
{
    public class FilialeManager:BllInstance<FilialeManager>
    {
        
        public static FilialeInfo Get(Guid filialeId)
        {
            return GetList().FirstOrDefault(w => w.ID == filialeId);
        }

        public static string GetName(Guid filialeId)
        {
            var info = Get(filialeId);
            if (info == null)
            {
                return string.Empty;
            }
            return info.Name;
        }

        public static bool IsEntityShopFiliale(Guid filialeId)
        {
            var info = Get(filialeId);
            if (info == null)
            {
                return false;
            }
            return info.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop);
        }

        public static bool IsShop(FilialeInfo filialeInfo)
        {
            return filialeInfo.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop);
        }

        public static bool IsEntityShopFiliale(Guid filialeId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var info = Get(filialeId);
            if (info == null)
            {
                errorMessage = "公司信息未获取到";
                return false;
            }
            return info.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop);
        }

        /// <summary>
        /// 判断公司是否为联盟店类型
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool IsAllianceShopFiliale(Guid filialeId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var info = Get(filialeId);
            if (info == null)
            {
                errorMessage = "公司信息未获取到";
                return false;
            }
            return info.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop) && info.IsActive;
        }

        public static Guid GetShopHeadFilialeId(Guid filialeId)
        {
            var info = Get(filialeId);
            if (info.Rank == (int)MIS.Enum.FilialeRank.Child)
            {
                info = Get(info.ParentId);
                return info.ParentId;
            }
            if (info.Rank == (int)MIS.Enum.FilialeRank.Partial)
            {
                return info.ParentId;
            }
            return info.ID;
        }

        public static IList<FilialeInfo> GetList()
        {
            return Cache.Filiale.Instance.ToList().Where(ent => ent.ID != GlobalConfig.ERPFilialeID).ToList();
        }

        public static IList<FilialeInfo> GetHeadList()
        {
            const int RANK = (int)MIS.Enum.FilialeRank.Head;
            var list = GetList();
            return list.Where(ent => ent.Rank == RANK && ent.ID != GlobalConfig.ERPFilialeID).ToList();
        }


        /// <summary>
        /// 获取门店公司集合
        /// zhangfan added at 2013-June-9th
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetEntityShop()
        {
            return GetList().Where(ent => ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop)).ToList();
        }

        /// <summary>
        /// 获取B2C公司集合
        /// zhangfan added at 2013-Sep-22th
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetB2CFilialeList()
        {
            return GetList().Where(ent => ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.SaleCompany)).ToList();
        }

        /// <summary>
        /// 获取联盟店公司集合
        /// </summary>
        /// <returns></returns>
        public static IList<FilialeInfo> GetAllianceFilialeList()
        {
            return GetList().Where(ent => ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop) && ent.IsActive).ToList();
        }

        public static IList<FilialeInfo> GetAllHostingAndSaleFilialeList()
        {
            return GetList().Where(ent => ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.LogisticsCompany) && ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.SaleCompany) && ent.IsActive).ToList();
        } 
    }
}
