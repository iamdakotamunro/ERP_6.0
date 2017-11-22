using System;
using System.Collections;
namespace ERP.Cache.Common
{
    public abstract class Base<T> where T : new()
    {
        private static readonly Hashtable unity = new Hashtable();
        private static readonly object lockOjbect = new object();

        /// <summary>
        /// 创建实例化对象
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (lockOjbect)
                {
                    var tType = typeof (T);
                    var key = tType.FullName;
                    if (unity.ContainsKey(key))
                    {
                        return (T)unity[key];
                    }
                    var t = Activator.CreateInstance<T>();
                    unity.Add(key, t);
                    return t;
                }
            }
        }
    }
}
