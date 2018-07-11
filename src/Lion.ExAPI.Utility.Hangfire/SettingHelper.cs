using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core
{
    #region SettingHelper
    public class SettingHelper
    {
        /// <summary>
        /// Get Request context save to HttpContext.Item。
        /// </summary>
        public const string Re_quest = "request";
        /// <summary>
        /// Get Reponse context save to HttpContext.Item。
        /// </summary>
        public const string Re_ponse = "Reponse";
        /// <summary>
        /// Create Request and Reponse Context Id save to HttpContext.Item。  
        /// </summary>
        public const string Re_quest_Id = "request_id";
        /// <summary>
        /// Get Application Name save to HttpContext.Item。
        /// </summary>
        public const string App_lication = "ApplicationName";
        /// <summary>
        /// Get Application Time live save to HttpContext.Item。
        /// </summary>
        public const string Action_Timer = "Action_Timer";

        /// <summary>
        /// Get ErrorMessage context save to HttpContext.Item。
        /// </summary>
        public const string ErrorMessage = "ErrorMessage";
        /// <summary>
        /// Get ErrorStackTrace context save to HttpContext.Item。
        /// </summary>
        public const string ErrorStackTrace = "ErrorStackTrace";
        /// <summary>
        /// Get InnerException context save to HttpContext.Item。
        /// </summary>
        public const string InnerException = "InnerException";

        /// <summary>
        /// json 最大檔案限制
        /// </summary>
        public const int Over_ReponseSizeContext = 924234;
        /// <summary>
        /// json 最大檔案限制
        /// </summary>
        public const int Over_ReponseSize = 1224234;

        /// <summary>
        ///  Log connect url
        /// </summary>
        public const string LogAddressUrl = "api/Log/LogConnectErrorAsync";

        public const string LogDomain = "HangfireServer:LogAddressUrl";

    }
    #endregion SettingHelper
}
