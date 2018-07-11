using NetFrameworkApp.EnumSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetFrameworkApp.Models
{
    public class AuthWithOTPResult
    {
        public String requestId { get; set; }
        public Boolean isVerified { get; set; }
        public AuthenticationOTPEnum authResultCode { get; set; }
        public String authResultMsg { get; set; }
    }
}