using Dapper;
using Lion.ExAPI.Utility.Core.Models;
using Lion.ExAPI.Utility.Extensions;
using Lion.ExAPI.Utility.Helper;
using Lion.ExAPI.Utility.Models.V2;
using Lion.ExAPI.Utility.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Lion.ExAPI.Utility.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class APIDeveloperNetTokenAuthenticationAttribute : ActionFilterAttribute
    {
        private string _DBconfig;
        /// <summary>
        /// 
        /// </summary>
        public APIDeveloperNetTokenAuthenticationAttribute()
        {
            _DBconfig = CongfigSetting.Token_ConnectionString_PlatForm;
        }


        /// <summary>是否自動延長token效期</summary>
        public bool AutoTokenExtended = true;

        #region Private

        /// <summary>Token 自動延長時效（秒）</summary>
        private const int _tokenExtendedTime = 10 * 60;

        /// <summary>token值</summary>
        private string _token = String.Empty;

        /// <summary></summary>
        private DevTokenApplyPlace _apiToken = default(DevTokenApplyPlace);

        /// <summary>當次驗證狀態</summary>
        private AuthStatus _authStatus = AuthStatus.None;

        private const Char _delimiter = ' ';

        private const Char _CharSlash = '/';

        /// <summary>API_Developers_Info </summary>
        public const string API_Developers_Info = "API_Developers_Info";

        private Boolean PathUrlwithPath(DevTokenApplyPlace apiToken, string path, string PathUrl)
        {
            Boolean flag = false;
            string iisPort_return = string.Empty;
            string apiPort_return = string.Empty;
            List<String> result = new List<String>();
            List<String> api = new List<String>();
            List<String> iisPort = new List<String>();

            #region 防呆
            if (apiToken.apiPort == null)
                return flag;
            else
                api = apiToken.apiPort.Split('|').ToList();

            if (apiToken.iisPort == null)
                return flag;
            else
                iisPort = apiToken.iisPort.Split('|').ToList();
            #endregion 防呆

            foreach (var item in api)
            {
                var j = 0;
                var api1 = item.Split(',').ToList();
                for (int i = 0; i < api1.Count; i++)
                {
                    if (iisPort[j][iisPort[j].Length - 1] == _CharSlash)
                        iisPort_return = iisPort[j].Substring(0, iisPort[j].Length - 1).Replace(" ", "");
                    else
                        iisPort_return = iisPort[j].Substring(0, iisPort[j].Length).Replace(" ", "");

                    if (api1[i][0] == _CharSlash)
                        apiPort_return = api1[i];
                    else
                        apiPort_return = _CharSlash + api1[i];

                    if (string.Equals(apiPort_return, path, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(iisPort_return, PathUrl.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)
                           || string.Equals(PathUrl.Replace(" ", ""), "http://localhost", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                        }
                    }
                    result.Add(iisPort_return + apiPort_return);
                }
                j++;
            }
            apiToken.iisPortandapiPort = result;
            return flag;
        }


        private AuthStatus TokenAuthentication(string token, string ipAddress, string path, string PathUrl)
        {

            AuthStatus authStatus = AuthStatus.None;

           // _apiToken = CacheHelper.GetValue(token) as DevTokenApplyPlace;

            if (_apiToken == null || DateTime.Now > _apiToken.expire)
            {
                using (var conn = new SqlConnection(_DBconfig))
                {
                    conn.Open();

                    _apiToken = conn.Query<DevTokenApplyPlace>("CheckAPI_Token",
                                new { token = token },
                                commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
            }

            if (_apiToken == null)
            {
                //ToKen是否已存在
                authStatus = AuthStatus.Revoked_TokenNotExists;
            }
            else if (_apiToken.sts == false)
            {
                //Token 是否停用
                authStatus = AuthStatus.Revoked_TokenDisabled;
            }
            else if (DateTime.Now > _apiToken.expire)
            {
                //ToKen是否過期
                authStatus = AuthStatus.Revoked_Expired;
            }
            else if (!String.IsNullOrWhiteSpace(ipAddress) && !String.IsNullOrWhiteSpace(_apiToken.UserIP) &&
               !ipAddress.Equals("::1") && !ipAddress.Equals("127.0.0.1") && !_apiToken.UserIP.Contains(ipAddress))
            {
                //IP權限判斷，判斷使用者IP是否可以使用此API
                authStatus = AuthStatus.Revoked_NotAllowIP;
            }
            else if (String.IsNullOrWhiteSpace(_apiToken.apiPort))
            {
                //是否有api加入
                authStatus = AuthStatus.Revoked_NotAllowUrl;
            }
            else if (String.IsNullOrWhiteSpace(_apiToken.iisPort))
            {
                //是否有IIS Domain加入
                authStatus = AuthStatus.Revoked_NotAllowUrl;
            }
            else if (!PathUrlwithPath(_apiToken, path, PathUrl))
            {
                //呼叫的api是否有包含在資料庫中
                authStatus = AuthStatus.Revoked_NotAllowUrl;
            }
            else
            {
                //驗證通過
                authStatus = AuthStatus.Authorized;
            }


            return authStatus;
        }


        private string AuthExtendTime(DevTokenApplyPlace apiToken, string token, int _tokenExtendedTime_, AuthStatus authStatus)
        {

            if (AutoTokenExtended && !String.IsNullOrWhiteSpace(token) && authStatus == AuthStatus.Authorized)
            {
                //無cache 及 過期前 (_tokenExtendedTime/10)秒 提早SQL update
                int SlidingExpiration = -1 * (int)Math.Round(_tokenExtendedTime_ / 10.0);
                if (apiToken == null || apiToken.expire.AddSeconds(SlidingExpiration) < DateTime.Now)
                {

                    using (SqlConnection conn = new SqlConnection(_DBconfig))
                    {
                        string sqlStr = string.Empty;

                        conn.Open();

                        var updateTmp = conn.Query<DevToken>("CheckAPI_Token_UpDate",
                                new { token = token, TokenStatus = apiToken.TokenStatus, _tokenExtendedTime_ = _tokenExtendedTime_ },
                                commandType: CommandType.StoredProcedure).FirstOrDefault();

                        if (updateTmp != null)
                        {
                            if (string.Equals(apiToken.TokenStatus, "T", StringComparison.OrdinalIgnoreCase))
                            {
                                _apiToken.expire = updateTmp.token_texpire;
                            }
                            if (string.Equals(apiToken.TokenStatus, "F", StringComparison.OrdinalIgnoreCase))
                            {
                                _apiToken.expire = updateTmp.token_fexpire;
                            }
                           // CacheHelper.CacheInsert(token, apiToken, apiToken.expire);
                        }
                    }
                }
                return _apiToken.expire.ToString("yyyy-MM-ddTHH:mm:ssK");
            }
            return null;
        }
        #endregion Private


        /// <summary>
        ///
        /// </summary>
        /// <param name="Context"></param>
        public override void OnActionExecuting(HttpActionContext Context)
        {
            var _request = Context.Request.RequestUri;
            var httpurl = _request.Scheme +"://"+_request.Host;
            string Path = _request.LocalPath;


            //版本不對
            if (Context.Request.RequestUri.ToString().IndexOf("/V2/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //沒有加入token
                if (Context.Request.Headers.Authorization == null)
                {
                    _authStatus = AuthStatus.Revoked_AuthorizationNotExists;
                }

                //token 格式不對
                else if (!Context.Request.Headers.Authorization.ToString().StartsWith("basic" + _delimiter, StringComparison.OrdinalIgnoreCase))
                {
                    _authStatus = AuthStatus.Revoked_AuthorizationFormatIncorrect;
                }

                //取得token 的值
                else
                {
                    _token = Context.Request.Headers.Authorization.ToString().Split(_delimiter)[1].Trim();
                    _authStatus = TokenAuthentication(_token, HttpContext.Current.Request.UserHostAddress, Path, httpurl);
                }
            }
            else
            {
                _authStatus = AuthStatus.Revoked_ErrorUrlPath;

            }

            if (_authStatus != AuthStatus.Authorized)
            {
                ResponseBase<Object> Result = new ResponseBase<Object>();
                Result.rCode = _authStatus.Get_rCode();
                Result.rDesc = _authStatus.Get_rCodeDesc();
                Context.Response = Context.Request.CreateResponse(HttpStatusCode.OK, Result);
            }
            else
            {
                HttpContext.Current.Items[API_Developers_Info] = _apiToken;
            }

            base.OnActionExecuting(Context);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Context"></param>
        public override void OnActionExecuted(HttpActionExecutedContext Context)
        {
            if (Context.Exception == null)
            {
                ResponseBase<Object> Result = new ResponseBase<Object>();

                if (AutoTokenExtended && !String.IsNullOrWhiteSpace(_token) && _authStatus == AuthStatus.Authorized)
                {
                    if (Context.Response != null)
                    {
                        if (Context.Response.Content.ReadAsStringAsync().Result != null)
                        {
                            if (UnitHelper.checkApiFormat(Context.Response.Content.ReadAsStringAsync().Result.ToString()))
                            {
                                ResponseBase<Object> responseBody = JsonConvert.DeserializeObject<ResponseBase<Object>>(Context.Response.Content.ReadAsStringAsync().Result.ToString());
                                if (responseBody != null)
                                {
                                    Result.Data = responseBody.Data;
                                    Result.rCode = responseBody.rCode;
                                    Result.rDesc = responseBody.rDesc;
                                    Result.TokenExpires = Convert.ToDateTime(AuthExtendTime(_apiToken, _token, _tokenExtendedTime, _authStatus));
                                }
                            }
                            else
                            {
                                _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                            }
                        }
                        else
                        {
                            _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                        }
                    }
                    else
                    {
                        _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                    }
                }

                if (_authStatus != AuthStatus.Authorized)
                {
                    Result.rCode = _authStatus.Get_rCode();
                    Result.rDesc = _authStatus.Get_rCodeDesc();
                }
                

                Context.Response = Context.Request.CreateResponse(HttpStatusCode.OK, Result);
            }

            base.OnActionExecuted(Context);
        }





    }
}
