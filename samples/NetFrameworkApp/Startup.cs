using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Lion.ExAPI.Utility.Hangfire;


[assembly: OwinStartup(typeof(NetFrameworkApp.Startup))]

namespace NetFrameworkApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
