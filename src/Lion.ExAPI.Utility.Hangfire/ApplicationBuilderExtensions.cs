using Lion.ExAPI.Utility.Hangfire.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Lion.ExAPI.Utility.Hangfire.Core {

    /// <summary>
    /// Create Log Package pulgin to Save Log
    /// </summary>
    public static class ApplicationBuilderExtensions {

        public static List<IApplicationBuilder> UseLoggerPackage(this IApplicationBuilder app, IConfigurationRoot config) {
            #region 防呆 防url不符合
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }
            if(config.GetSection(SettingHelper.LogDomain).Value ==string.Empty)
            {
                throw new ArgumentNullException("HangfireServer:LogAddressUrl 沒有被加入參數!!");
            }
            if (app.ApplicationServices.GetRequiredService<IHttpContextAccessor>() == null)
            {
                throw new ArgumentNullException("IHttpContextAccessor 沒有被註冊!!");
            }
            #endregion 防呆

            ToolsContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(), config);
            List<IApplicationBuilder> data = new List<IApplicationBuilder>();
            data.Add(app.UseMiddleware<ExceptionMiddleware>());
            data.Add(app.UseMiddleware<LogResponseMiddleware>());
            data.Add(app.UseMiddleware<LogRequestMiddleware>());
            return data;
        }

    }

    /// <summary>
    /// Create Live Context to get Need Project Infomation
    /// </summary>
    public static class ToolsContext
    {
        private static IHttpContextAccessor _contextAccessor;
        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot config => _configuration;

        public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor.HttpContext;

        internal static void Configure(IHttpContextAccessor contextAccessor, IConfigurationRoot configuration)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
    }


}
