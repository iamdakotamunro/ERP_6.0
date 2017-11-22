using System;
using System.Collections;
using System.Collections.Generic;

namespace ERP.Cache.Common
{
    /// <summary>
    /// 有委托事件的缓存管理
    /// </summary>
    public class CacheHelper
    {
        private static readonly Hashtable data = new Hashtable();
        private static readonly object lockObject = new object();

        public static T Get<T>(Key key, Func<T> fun) where T : class
        {
            lock (lockObject)
            {
                if (data.ContainsKey(key))
                {
                    return data[key] as T;
                }
                var value = fun();
                Set(key, value);
                return value;
            }
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(Key key) where T : class
        {
            lock (lockObject)
            {
                if (data.ContainsKey(key))
                {
                    return data[key] as T;
                }
                return default(T);
            }
        }

        /// <summary>
        /// 加入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(Key key, T value)
        {
            lock (lockObject)
            {
                if (data.ContainsKey(key))
                {
                    data[key] = value;
                }
                else
                {
                    data.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 清除指定的缓存值
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(Key key)
        {
            lock (lockObject)
            {
                if (data.ContainsKey(key))
                {
                    data.Remove(key);
                }
            }
        }

        /// <summary>
        /// 清楚所有的缓存
        /// </summary>
        public static void Clear()
        {
            lock (lockObject)
            {
                data.Clear();
            }
        }

        ///// <summary>
        ///// 所有缓存的键
        ///// </summary>
        //private static IList<Key> Keys
        //{
        //    get
        //    {
        //        IDictionaryEnumerator CacheEnum = HttpRuntime.Cache.GetEnumerator();
        //        IList<string> allKeys = new List<string>();
        //        while (CacheEnum.MoveNext())
        //        {
        //            allKeys.Add(CacheEnum.Key.ToString());
        //        }
        //        return allKeys;
        //    }
        //}
    }
}
