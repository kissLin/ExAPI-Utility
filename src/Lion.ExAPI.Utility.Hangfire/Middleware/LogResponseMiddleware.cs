using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core.Middleware {
#if NETSTANDARD1_6
    public class LogResponseMiddleware {
        private readonly RequestDelegate next;
        private readonly ILogger<LogResponseMiddleware> _logger;
        private readonly IHostingEnvironment _env;

        public LogResponseMiddleware(RequestDelegate next, IHostingEnvironment env, ILogger<LogResponseMiddleware> logger) {

            if (next == null) {
                throw new ArgumentNullException(nameof(next));
            }
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (logger == null) {
                throw new ArgumentNullException(nameof(logger));
            }
            this.next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            string responseBody = string.Empty;
            #region 取得 Response
            using (var memoryResponse = new MemoryStream()) {
                var originalResponse = context.Response.Body;
                try {
                    context.Response.Body = memoryResponse;
                    await this.next(context).ConfigureAwait(false);
                    memoryResponse.Seek(0, SeekOrigin.Begin);
                    responseBody = new StreamReader(memoryResponse).ReadToEnd();
                    UnitT.CheckHttpException(context.Response.StatusCode, responseBody);
                    memoryResponse.Seek(0, SeekOrigin.Begin);
                    await memoryResponse.CopyToAsync(originalResponse);
                }
                finally {
                    // This is what you're missing
                    context.Response.Body = originalResponse;
                    context.Items[SettingHelper.Re_ponse] = responseBody;
                    context.Items[SettingHelper.App_lication] = _env.ApplicationName;
                }
            }
            #endregion 取得 Response
        }
    }
#endif
}
