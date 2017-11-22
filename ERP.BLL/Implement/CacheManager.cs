using System.Collections.Generic;
using System.Linq;
using ERP.Cache.Common;

namespace ERP.BLL.Implement
{
    public class CacheManager
    {
        /// <summary>
        /// 所有缓存的键
        /// </summary>
        public static IList<Key> Keys
        {
            get
            {
                return Framework.Core.Utility.EnumUtility.GetEnumList<Key>().ToList();
            }
        }

        public static void RemoveAll()
        {
            foreach (var key in Keys)
            {
                CacheHelper.Remove(key);
            }
        }

        public static void Remove(string keyString)
        {
            Key key;
            if (System.Enum.TryParse(keyString, out key))
            {
                CacheHelper.Remove(key);
            }
        }
    }
}
