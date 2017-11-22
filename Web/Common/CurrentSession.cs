using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.BLL.Implement.Inventory;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.WMS;
using Framework.Common;
using Keede.Ecsoft.Model;
using Config.Keede.Library;

namespace ERP.UI.Web.Common
{
    /// <summary>系统，登录员工，配置公司放置于Session
    /// </summary>
    public class CurrentSession
    {
        #region [系统信息]

        /// <summary>系统信息
        /// </summary>
        public class System
        {
            internal static string Key
            {
                get { return "SystemId"; }
            }

            internal static string ShopKey
            {
                get { return "ShopWebSiteId"; }
            }

            public static Guid ID
            {
                get
                {
                    var val = HttpContext.Current.Session[Key];
                    if (val == null)
                    {
                        val = ConfManager.GetAppsetting(Key);
                        HttpContext.Current.Session[Key] = val;
                        if (val == null || val.ToString() == string.Empty)
                        {
                            throw new ApplicationException("未找到 AppSetting 里的 SystemId 配置！");
                        }
                    }

                    return val.ToString().ToGuid();
                }
            }
        }

        #endregion

        #region [登录员工信息，授权仓库]

        /// <summary>登录员工信息，授权仓库
        /// </summary>
        public class Personnel
        {
            internal static string Key
            {
                get { return "PersonnelInfo"; }
            } 

            /// <summary>获取当前有权限访问的仓库
            /// </summary>
            internal static IList<WarehouseAuth> WarehouseList
            {
                get
                {
                    const string KEY = "WareHouseList";
                    var w = HttpContext.Current.Session[KEY];
                    var list = w as List<WarehouseAuth>;
                    if (list == null || list.Count==0 || list.Any(ent => ent.Storages == null || ent.Storages.Count == 0))
                    {
                        var personnelInfo = Get();
                        list = WarehouseManager.GetWarehouseIsPermission(personnelInfo.PersonnelId).ToList();
                        HttpContext.Current.Session[KEY] = list;
                    }
                    return list;
                }
            }

            /// <summary> 缓存员工登陆信息
            /// </summary>
            /// <param name="personnelInfo">员工对象</param>
            public static void Set(PersonnelInfo personnelInfo)
            {
                if (personnelInfo != null)
                {
                    HttpContext.Current.Session[Key] = personnelInfo;
                }
            }

            public static PersonnelInfo Get()
            {
                var info = HttpContext.Current.Session[Key];
                if (info != null)
                {
                    return (PersonnelInfo)info;
                }
                return null;
            }

            public static void Remove()
            {
                HttpContext.Current.Session.Remove(Key);
            }
        }

        #endregion

        #region [配置公司（ERP公司）]

        /// <summary>配置公司（ERP公司）
        /// </summary>
        public class Filiale
        {
            internal static string Key
            {
                get { return "FilialeInfo"; }
            }

            public static Guid ID
            {
                get { return Get().ID; }
            }

            public static FilialeInfo Get()
            {
                var val = HttpContext.Current.Session[Key];
                if (val == null)
                {
                    var filialeId = ConfManager.GetAppsetting("FilialeId");
                    if (!string.IsNullOrEmpty(filialeId))
                    {
                        val = CacheCollection.Filiale.GetList().FirstOrDefault(ent => ent.ID == new Guid(filialeId));
                        if (val != null)
                        {
                            HttpContext.Current.Session[Key] = val;
                        }
                    }
                }
                return (FilialeInfo)val;
            }
        }

        #endregion
    }
}