using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if net451
using System.Net;
#endif
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Models
{

    #region ConnectModel
    internal class ConnectModel
    {
        public string RequestId { get; set; }
        public string ApplicationName { get; set; }
        public string ActionData { get; set; }
        public string RequestForm { get; set; }

        public string SourceJsonString { get; set; }

        public object ReturnJsonString { get; set; }

        //public DateTime CreateDateTime { get; set; }

        public int DurationTime { get; set; }

        public TraceModel Info { get; set; }
    }

    #endregion ConnectModel

    #region TraceModel
    internal class TraceModel
    {
        //public string RequestId { get; set; }
#if net451
        public HttpStatusCode HttpCode { get; set; }
#else
        public string HttpCode { get; set; }
#endif

        public string AcceptFormat { get; set; }
        public string MethodType { get; set; }

        public string Token { get; set; }
        public string MachineName { get; set; }
        public string RemotrIP { get; set; }
        public string Device { get; set; }
        public string[] Session { get; set; }
        public string[] Cookies { get; set; }
    }
    #endregion TraceModel

    #region ErrModel
    internal class ErrModel
    {
        public string RequestId { get; set; }

        public string ActionData { get; set; }

        //public DateTime CreateDateTime { get; set; }

        public string InnerException { get; set; }
        public string ErrMessage { get; set; }
        public string ErrStackTrace { get; set; }
    }
    #endregion ErrModel

    #region RequestReponseModels
    internal class RequestReponseModels
    {
        public ConnectModel connectModel { get; set; }
        public ErrModel errModel { get; set; }
    }
    #endregion RequestReponseModels
}
