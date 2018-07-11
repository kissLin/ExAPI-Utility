using Lion.ExAPI.Utility.Hangfire.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core.Middleware {

#if NETSTANDARD1_6
    public class LogRequestMiddleware {
        private readonly RequestDelegate next;
        private readonly ILogger<LogRequestMiddleware> _logger;

        public LogRequestMiddleware(RequestDelegate next, ILogger<LogRequestMiddleware> logger) {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {

            #region 取得 Request
            using (RequestIdentifier.Ensure(context)) {
                context.Items[SettingHelper.Re_quest_Id] = context.Features.Get<IHttpRequestIdentifierFeature>().TraceIdentifier;
                var requestBodyStream = new MemoryStream();
                var originalRequestBody = context.Request.Body;
                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                context.Items[SettingHelper.Re_quest] = requestBodyText;
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
                await next(context).ConfigureAwait(false);
                context.Request.Body = originalRequestBody;
            }
            #endregion 取得 Request
        }
    }
#endif
}
