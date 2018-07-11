using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Helper {
    public class CacheHelper {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>Cache絕對過期(數分鐘後移除)</summary>
        public static void CacheInsertAddMinutes(string key, object value, int minute) {
            if (value == null)
                return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(minute)));
        }

        /// <summary>Cache絕對過期(指定時間移除)</summary>
        public static void CacheInsert(string key, object value, DateTime absExpTime) {
            if (value == null)
                return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(absExpTime));
        }


        /// <summary>Cache相對過期（發呆數分鐘後移除）</summary>
        public static void CacheInsertFromMinutes(string key, object value, int minute) {
            if (value == null)
                return;
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(minute)));
        }


        /// <summary>Insert Cache</summary>
        public static void CacheInsert(string key, object value) {
            _memoryCache.Set(key, value);
        }

        /// <summary>Remove Cache by key</summary>
        public static void CacheRemove(string key) {
            _memoryCache.Remove(key);
        }

        /// <summary>Get Cache Value by key</summary>
        public static object GetValue(string key) {
            return _memoryCache.Get(key);
        }

    }
}
