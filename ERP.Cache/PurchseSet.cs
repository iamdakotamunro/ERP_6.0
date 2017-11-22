using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ERP.Cache.Common;
using ERP.Cache.Interface;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.Model;
using Keede.Ecsoft.Model;
using System.Collections.Concurrent;

namespace ERP.Cache
{
    /// <summary>
    /// 采购责任人绑定缓存，仓库,公司->商品列表
    /// </summary>
    public class PurchseSet : Base<PurchseSet>, ICaching<KeyValuePair<Guid, Guid>, List<Guid>>
    {
        private static readonly ConcurrentDictionary<KeyValuePair<Guid, Guid>, List<Guid>> _localCache = new ConcurrentDictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
        private static TimeSpan _duration = new TimeSpan(0, 1, 0);// 本地缓存保留1分钟
        private static Timer _timer = null;

        public PurchseSet()
        {
            _timer = new Timer(RefreshCache, null, _duration, _duration);
        }

        public void Load()
        {
            var data = new PurchaseSet(Environment.GlobalConfig.DB.FromType.Read).GetKeyAndValueGuids();
            // 移除db中没有的key
            if (_localCache.Count > 0)
            {
                List<Guid> removedVal = null;
                foreach (var keyToRemove in _localCache.Keys.Except(data.Keys).ToList())
                {
                    _localCache.TryRemove(keyToRemove, out removedVal);
                }
            }

            // 添加或更新
            foreach (var keyValuePair in data)
            {
                _localCache.AddOrUpdate(keyValuePair.Key, keyValuePair.Value, (key, oldVal) => keyValuePair.Value);
            }
        }

        public IList<KeyValuePair<Guid, Guid>> GetAllKeys()
        {
            var result = _localCache.Keys.ToList();
            return result;
        }

        public void Set(KeyValuePair<Guid, Guid> key, List<Guid> value)
        {
            _localCache.AddOrUpdate(key, value, (k, oldVal) => value);
        }

        public List<Guid> Get(KeyValuePair<Guid, Guid> key)
        {
            List<Guid> val = null;
            _localCache.TryGetValue(key, out val);
            if (val == null)
            {
                val = new List<Guid>();
            }
            return val;

        }

        public void Remove(KeyValuePair<Guid, Guid> key)
        {
            List<Guid> removedVal = null;
            _localCache.TryRemove(key, out removedVal);
        }

        private void RefreshCache(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                Load();
            }
            catch { }
            finally
            {
                _timer.Change(_duration, _duration);
            }
        }
    }
}
