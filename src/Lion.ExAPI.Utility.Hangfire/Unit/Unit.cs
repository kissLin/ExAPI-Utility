using Lion.ExAPI.Utility.Hangfire.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core {
    #region Unit
    internal class UnitT {


        #region 取得 HttpException
        public static void CheckHttpException(int statusCode, string responseBody) {
            switch (statusCode) {
                case 200:
                    return;
                case 400:
                    throw new Exception(HttpErrorCodeEnum.Bad_Request.Get_rCodeDesc());
                case 401:
                    throw new Exception(HttpErrorCodeEnum.Unauthorized.Get_rCodeDesc());
                case 402:
                    throw new Exception(HttpErrorCodeEnum.Payment_Required.Get_rCodeDesc());
                case 403:
                    throw new Exception(HttpErrorCodeEnum.Forbidden.Get_rCodeDesc());
                case 404:
                    throw new Exception(HttpErrorCodeEnum.Not_Found.Get_rCodeDesc());
                case 405:
                    throw new Exception(HttpErrorCodeEnum.Method_Not_Allowed.Get_rCodeDesc());
                case 408:
                    throw new Exception(HttpErrorCodeEnum.Request_Timeout.Get_rCodeDesc());
                case 415:
                    throw new Exception(HttpErrorCodeEnum.Unsupported_Media_Type.Get_rCodeDesc());
                case 500:
                    throw new Exception(HttpErrorCodeEnum.Internal_Server_Error.Get_rCodeDesc());
                case 502:
                    throw new Exception(HttpErrorCodeEnum.Bad_Gateway.Get_rCodeDesc());
                case 503:
                    throw new Exception(HttpErrorCodeEnum.Service_Unavailable.Get_rCodeDesc());
            }
        }
        #endregion 取得 HttpException

        #region ReturnError
        public static string ReturnError(string path, string rcode, Exception exception = null) {
            Lion.ExAPI.Utility.Hangfire.Models.V2.ResponseBase RespBaseReturnV2 = new Lion.ExAPI.Utility.Hangfire.Models.V2.ResponseBase();
            rCodeStatusEnumLog rCode = rCodeStatusEnumLog.rCode_0005;
            if (rcode != "200") {
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(exception.Message);
                if (path.IndexOf("/v1/") >= 0) {
                    ResponseBase RespBaseReturnV1 = new ResponseBase();
                    RespBaseReturnV1.IsSuccess = false;
                    RespBaseReturnV1.Data = null;
                    RespBaseReturnV1.Msg = Encoding.UTF8.GetString(utf8Bytes);
                    return JsonConvert.SerializeObject(RespBaseReturnV1);
                } else {
                    RespBaseReturnV2.Data = null;
                    RespBaseReturnV2.rCode = rcode;
                    RespBaseReturnV2.rDesc = Encoding.UTF8.GetString(utf8Bytes);
                    RespBaseReturnV2.TokenExpires = DateTime.MinValue;
                    return JsonConvert.SerializeObject(RespBaseReturnV2);
                }
            } else {
                if (path.IndexOf("/v1/") >= 0) {
                    ResponseBase RespBaseReturnV1 = new ResponseBase();
                    RespBaseReturnV1.IsSuccess = false;
                    RespBaseReturnV1.Data = null;
                    RespBaseReturnV1.Msg = "API System error";
                    return JsonConvert.SerializeObject(RespBaseReturnV1);
                } else {
                    RespBaseReturnV2.Data = null;
                    RespBaseReturnV2.rCode = rCode.Get_rCode();
                    RespBaseReturnV2.rDesc = rCode.Get_rCodeDesc();
                    RespBaseReturnV2.TokenExpires = DateTime.MinValue;
                    return JsonConvert.SerializeObject(RespBaseReturnV2);
                }
            }
        }
        #endregion ReturnError
    }
    #endregion Unit
}
