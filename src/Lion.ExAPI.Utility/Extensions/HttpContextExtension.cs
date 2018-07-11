using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lion.ExAPI.Utility.Core.Extensions {
    public static class HttpContextExtension {

        /// <summary>取得Remote IP</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRequestIP(this HttpContext context) {


            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ip))
                ip = context.Connection.RemoteIpAddress.ToString();


            if (string.IsNullOrWhiteSpace(ip))
                ip = context.Request.Headers["REMOTE_ADDR"].FirstOrDefault();


            return ip;
        }


    }
}
