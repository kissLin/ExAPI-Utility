using Lion.ExAPI.Utility.Hangfire.Core.Serivce;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core.Middleware {
#if NETSTANDARD1_6
    public class ExceptionMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        MiddlewareController controller = new MiddlewareController();

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory logger) {
            _next = next;
            _logger = logger.CreateLogger<ExceptionMiddleware>();
        }


        public async Task Invoke(HttpContext context) {

            var sw = Stopwatch.StartNew();
            try {
                await _next(context).ConfigureAwait(false);
                sw.Stop();
                context.Items[SettingHelper.Action_Timer] = sw.ElapsedMilliseconds;
                await controller.EnterInsertAsync(context);
            }
            catch (Exception exception) {
                await HandleExceptionAsync(context, exception);
            }
        }

        #region HandleExceptionAsync
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            MiddlewareController controller = new MiddlewareController();
            var code = HttpStatusCode.BadRequest;
            string body = string.Empty;

            if (context.Response.HasStarted || context.Response.StatusCode >= 400 || context.Response.StatusCode <= 599
                || context.Response.ContentLength.HasValue || !string.IsNullOrEmpty(context.Response.ContentType)) {

                body = UnitT.ReturnError(context.Request.Path.Value.ToLower(), context.Response.StatusCode.ToString(), exception);
                context.Items[SettingHelper.Re_ponse] = body;
                //Eric fix 20170928
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                await controller.EnterInsertAsync(context, exception);
                await context.Response.WriteAsync(body);
            }
        }
        #endregion HandleExceptionAsync
    }
#endif
}
