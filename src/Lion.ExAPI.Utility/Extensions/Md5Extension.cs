using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Extensions {
    /// <summary>
    /// 
    /// </summary>
    public static class Md5Extension {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String ComputeMd5(this String str) {

            using (var md5 = MD5.Create()) {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

                return BitConverter.ToString(data).Replace("-", String.Empty).ToLower();

            }

        }
    }
}
