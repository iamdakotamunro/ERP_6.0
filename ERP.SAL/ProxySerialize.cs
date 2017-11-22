using System;
using KeedeGroup.WMS.Application.Contract.Proxy;
using Newtonsoft.Json;

namespace ERP.SAL
{
    public class ProxySerialize : IProxySerialize
    {
        #region Implementation of IProxySerialize

        /// <summary> 反序列化
        /// </summary>
        /// <param name="jsonText"/><typeparam name="T"/>
        public T ToObj<T>(string jsonText)
        {
            if (String.IsNullOrWhiteSpace(jsonText))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(jsonText);
        }

        /// <summary> 序列化
        /// </summary>
        /// <param name="obj"/>
        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        #endregion
    }
}
