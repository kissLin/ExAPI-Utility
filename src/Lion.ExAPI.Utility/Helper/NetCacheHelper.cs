using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Lion.ExAPI.Utility.Helper {
    /// <summary>
    /// 
    /// </summary>
    public class NetCacheHelper {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(string key) {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Get(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absExpiration"></param>
        public static void Add(string key, object value, DateTimeOffset absExpiration) {
            MemoryCache memoryCache = MemoryCache.Default;
            memoryCache.Add(key, value, absExpiration);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key) {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key)) {
                memoryCache.Remove(key);
            }
        }
    }
}
