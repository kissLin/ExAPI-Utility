using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility {
    class CongfigSetting {

        //開發機
#if net451
        public const string Token_ConnectionString_L ="Data Source=DBA-USYS;Database=ExAPI_Auth;Initial Catalog=ExAPI_Auth;Persist Security Info=True;User ID=exapiauth;Password=exapiauth*672;Application Name=Lion.ExAPI.Utility;";
#else
        public const string Token_ConnectionString_L = "Data Source=DBA-USYS;Database=ExAPI_Auth;Initial Catalog=ExAPI_Auth;Persist Security Info=True;User ID=exapiauth;Password=exapiauth*672;Application Name=Lion.ExAPI.Utility.Core;";
#endif
        //D機
#if net451
        public const string Token_ConnectionString_D = "Data Source=DBA-DAPI;Database=LionExAPI;Initial Catalog=LionExAPI;Persist Security Info=True;User ID=lionitapi;Password=lionitapi$107;Application Name=Lion.ExAPI.Utility;";
#else
        public const string Token_ConnectionString_D = "Data Source=DBA-DAPI;Database=LionExAPI;Initial Catalog=LionExAPI;Persist Security Info=True;User ID=lionitapi;Password=lionitapi$107;Application Name=Lion.ExAPI.Utility.Core;";
#endif
        //測試機
#if net451
        public const string Token_ConnectionString_T = "Data Source=DBA-USYS;Database=ExAPI_Auth;Initial Catalog=ExAPI_Auth;Persist Security Info=True;User ID=exapiauth;Password=exapiauth*672;Application Name=Lion.ExAPI.Utility;";
#else
        public const string Token_ConnectionString_T = "Data Source=DBA-USYS;Database=ExAPI_Auth;Initial Catalog=ExAPI_Auth;Persist Security Info=True;User ID=exapiauth;Password=exapiauth*672;Application Name=Lion.ExAPI.Utility.Core;";
#endif
        //正試機
#if net451
        public const string Token_ConnectionString_F = "Data Source=DBA-API;Database=LionExAPI;Initial Catalog=LionExAPI;Persist Security Info=True;User ID=exapi01;Password=exapI*380;Application Name=Lion.ExAPI.Utility;";
#else
        public const string Token_ConnectionString_F = "Data Source=DBA-API;Database=LionExAPI;Initial Catalog=LionExAPI;Persist Security Info=True;User ID=exapi01;Password=exapI*380;Application Name=Lion.ExAPI.Utility.Core;";
#endif
        //Developer
#if net451
        public const string Token_ConnectionString_PlatForm = "Data Source=DBA-SYS;Database=LionExAPI_developers;Initial Catalog=LionExAPI_developers;Persist Security Info=True;User ID=exapidev;Password=exapidev@621;Application Name=Lion.ExAPI.Utility;";
#else
        public const string Token_ConnectionString_PlatForm = "Data Source=DBA-SYS;Database=LionExAPI_developers;Initial Catalog=LionExAPI_developers;Persist Security Info=True;User ID=exapidev;Password=exapidev@621;Application Name=Lion.ExAPI.Utility.Core;";
#endif

    }
}
