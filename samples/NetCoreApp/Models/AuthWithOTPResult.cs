using NetCoreApp.EnumSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreApp.Models
{
    public class AuthWithOTPResult
    {
        public String requestId { get; set; }
        public Boolean isVerified { get; set; }
        public AuthenticationOTPEnum authResultCode { get; set; }
        public String authResultMsg { get; set; }
    }
}
