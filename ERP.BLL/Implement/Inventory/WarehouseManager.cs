using System;
using System.Collections.Generic;
using System.Linq;
using ERP.SAL;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class WarehouseManager
    {
        /// <summary>功   能:获取该员工可授权仓库的集合
        /// </summary>
        public static IList<WarehouseAuth> GetWarehouseIsPermission(Guid personnelId)
        {
            return WMSSao.GetWarehouseAuth(personnelId);
        }

        /// <summary> 获得仓库列表
        /// ADD:2013.7.27 阮剑锋
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, string> GetWarehouseDic()
        {
            return WMSSao.GetAllCanUseWarehouseDics();
        }

        public static List<WarehouseBasicDTO> GetList()
        {
            return GetWarehouseDic().Keys.Select(WMSSao.GetWarehouseDetail).ToList();
        }

        public static List<WarehouseBasicDTO> GetContainsCustomerServiceStorageWarehouseList()
        {
            return GetList().Where(ent => ent.StorageTypes.Any(e => e.Key == (byte)KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.StorageType.S)).ToList();
        }

        public static List<WarehouseBasicDTO> GetContainsBadStorageWarehouseList()
        {
            return GetList().Where(ent => ent.StorageTypes.Any(e => e.Key == (byte)KeedeGroup.WMS.Infrastructure.CrossCutting.Enum.StorageType.H)).ToList();
        }

        /// <summary>ADD:2013.7.27 阮剑锋
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WarehouseBasicDTO Get(Guid id)
        {
            return WMSSao.GetWarehouseDetail(id);
        }

        /// <summary> ADD:2013.7.27 阮剑锋
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetName(Guid id)
        {
            var dic = WMSSao.GetAllCanUseWarehouseDics();
            if (dic == null)
                return string.Empty;

            if (dic.ContainsKey(id))
            {
                return dic[id];
            }
            return string.Empty;
        }
    }
}
