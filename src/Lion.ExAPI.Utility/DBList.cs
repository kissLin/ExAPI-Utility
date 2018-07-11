using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core {
    internal class DBList {

        private static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Get Main Project setting
        /// </summary>
        static DBList() {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public static string _DBconfig { get { return Configuration.GetSection("HangfireDB").Value; } }
    }
}
