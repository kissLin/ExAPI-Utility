 using Dapper;
using Lion.ExAPI.Utility.Extensions;
using Lion.ExAPI.Utility.Helper;
using Lion.ExAPI.Utility.Models;
using Lion.ExAPI.Utility.Models.V2;
using Lion.ExAPI.Utility.Settings;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Lion.ExAPI.Utility.Attributes {

    /// <summary>
    ///
    /// </summary>
    public class NetTokenAuthenticationAttribute : ActionFilterAttribute {
        private string _DBconfig;

        /// <summary>
        ///
        /// </summary>
        public NetTokenAuthenticationAttribute() {
            string DBconfig = null;
            if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["HangfireDB"], "L", StringComparison.OrdinalIgnoreCase)) {
                DBconfig = CongfigSetting.Token_ConnectionString_L;
            } else if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["HangfireDB"], "D", StringComparison.OrdinalIgnoreCase)) {
                DBconfig = CongfigSetting.Token_ConnectionString_D;
            } else if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["HangfireDB"], "T", StringComparison.OrdinalIgnoreCase)) {
                DBconfig = CongfigSetting.Token_ConnectionString_T;
            } else {
                DBconfig = CongfigSetting.Token_ConnectionString_F;
            }
            _DBconfig = DBconfig;
        }

        /// <summary>是否自動延長token效期</summary>
        public bool AutoTokenExtended = true;

        #region Private

        /// <summary>Token 自動延長時效（秒）</summary>
        private const int _tokenExtendedTime = 10 * 60;

        /// <summary>token值</summary>
        private string _token = String.Empty;

        /// <summary></summary>
        private TokenApplyPlace _apiToken = default(TokenApplyPlace);

        /// <summary>當次驗證狀態</summary>
        private AuthStatus _authStatus = AuthStatus.None;

        private const Char _delimiter = ' ';

        private AuthStatus TokenAuthentication(string token, string ipAddress) {
            AuthStatus authStatus = AuthStatus.None;

            _apiToken = NetCacheHelper.GetValue(token) as TokenApplyPlace;

            if (_apiToken == null || DateTime.Now > _apiToken.ExpireDateTime) {
                using (SqlConnection conn = new SqlConnection(_DBconfig)) {
                    String strQuery = @"SELECT top 1 Token, ExpireDateTime, ApplicationName, AllowIPs
                                    from dbo.Token LEFT JOIN dbo.Application ON dbo.Application.Id = dbo.Token.ApplicationID
                                    where Token = @token";

                    conn.Open();

                    _apiToken = conn.Query<TokenApplyPlace>(strQuery, new { token = token }).SingleOrDefault();

                    if (_apiToken != null && DateTime.Now < _apiToken.ExpireDateTime) {
                        //token未過期才緩存

                        //set cache
                        //NetCacheHelper.Add(token, _apiToken, _apiToken.ExpireDateTime);
                    }
                }
            }

            if (_apiToken == null) {
                //ToKen是否已存在
                authStatus = AuthStatus.Revoked_TokenNotExists;
            } else if (DateTime.Now > _apiToken.ExpireDateTime) {
                //ToKen是否過期
                authStatus = AuthStatus.Revoked_Expired;
            } else if (String.IsNullOrWhiteSpace(_apiToken.ApplicationName)) {
                //ToKen是否有相關的系統在使用
                authStatus = AuthStatus.Revoked_AppNotExists;
            } else if (!String.IsNullOrWhiteSpace(ipAddress) && !String.IsNullOrWhiteSpace(_apiToken.AllowIPs) &&
                !ipAddress.Equals("::1") && !ipAddress.Equals("127.0.0.1") && !_apiToken.AllowIPs.Contains(ipAddress)
                ) {
                //IP權限判斷，判斷使用者IP是否可以使用此API
                authStatus = AuthStatus.Revoked_NotAllowIP;
            } else {
                //驗證通過
                authStatus = AuthStatus.Authorized;
            }

            return authStatus;
        }

        private string AuthExtendTime(TokenApplyPlace apiToken, string token, int _tokenExtendedTime_, AuthStatus authStatus) {
            if (AutoTokenExtended && !String.IsNullOrWhiteSpace(token) && authStatus == AuthStatus.Authorized) {
                //無cache 及 過期前 (_tokenExtendedTime/10)秒 提早SQL update
                int SlidingExpiration = -1 * (int)Math.Round(_tokenExtendedTime_ / 10.0);
                if (apiToken == null || apiToken.ExpireDateTime.AddSeconds(SlidingExpiration) < DateTime.Now) {
                    using (SqlConnection conn = new SqlConnection(_DBconfig)) {
                        string sqlStr = @"UPDATE TOP (1) dbo.Token
                                        SET RefreshDateTime = GETDATE(),
                                            ExpireDateTime = DATEADD(s, @tokenExtendedTime, GETDATE())
                                        OUTPUT INSERTED.RefreshDateTime, INSERTED.ExpireDateTime
                                        WHERE Token = @token";

                        conn.Open();
                        var updateTmp = conn.Query<TokenApplyPlace>(sqlStr, new { token = token, tokenExtendedTime = _tokenExtendedTime_ }).SingleOrDefault();
                        if (updateTmp != null) {
                            _apiToken.RefreshDateTime = updateTmp.RefreshDateTime;
                            _apiToken.ExpireDateTime = updateTmp.ExpireDateTime;

                            if(_apiToken != null && _apiToken.ExpireDateTime != null)
                            {
                                NetCacheHelper.Add(token, _apiToken, _apiToken.ExpireDateTime);
                            }
                        }
                    }
                }
                return _apiToken.ExpireDateTime.ToString("yyyy-MM-ddTHH:mm:ssK");
            }
            return null;
        }

        #endregion Private

        /// <summary>
        ///
        /// </summary>
        /// <param name="Context"></param>
        public override void OnActionExecuting(HttpActionContext Context) {
           
            //版本不對
            if (Context.Request.RequestUri.ToString().IndexOf("/V2/", StringComparison.OrdinalIgnoreCase) >= 0) {
                //沒有加入token
                if (Context.Request.Headers.Authorization == null) {
                    _authStatus = AuthStatus.Revoked_AuthorizationNotExists;
                }

                //token 格式不對
                else if (!Context.Request.Headers.Authorization.ToString().StartsWith("basic" + _delimiter, StringComparison.OrdinalIgnoreCase)) {
                    _authStatus = AuthStatus.Revoked_AuthorizationFormatIncorrect;
                }

                //取得token 的值
                else {
                    _token = Context.Request.Headers.Authorization.ToString().Split(_delimiter)[1].Trim();
                    _authStatus = TokenAuthentication(_token, HttpContext.Current.Request.UserHostAddress);
                }
            } else {
                _authStatus = AuthStatus.Revoked_ErrorUrlPath;

            }

            if (_authStatus != AuthStatus.Authorized) {
                ResponseBase<Object> Result = new ResponseBase<Object>();
                Result.rCode = _authStatus.Get_rCode();
                Result.rDesc = _authStatus.Get_rCodeDesc();
                Context.Response = Context.Request.CreateResponse(HttpStatusCode.OK, Result);
            } 

            base.OnActionExecuting(Context);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Context"></param>
        public override void OnActionExecuted(HttpActionExecutedContext Context) {
            if (Context.Exception == null) {
                ResponseBase<Object> Result = new ResponseBase<Object>();

                if (AutoTokenExtended && !String.IsNullOrWhiteSpace(_token) && _authStatus == AuthStatus.Authorized) {
                    if (Context.Response != null) {
                        if (Context.Response.Content.ReadAsStringAsync().Result != null) {
                            if (UnitHelper.checkApiFormat(Context.Response.Content.ReadAsStringAsync().Result.ToString())) {
                                ResponseBase<Object> responseBody = JsonConvert.DeserializeObject<ResponseBase<Object>>(Context.Response.Content.ReadAsStringAsync().Result.ToString());
                                if (responseBody != null) {
                                    Result.Data = responseBody.Data;
                                    Result.rCode = responseBody.rCode;
                                    Result.rDesc = responseBody.rDesc;
                                    Result.TokenExpires = Convert.ToDateTime(AuthExtendTime(_apiToken, _token, _tokenExtendedTime, _authStatus));
                                }
                            } else {
                                _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                            }
                        } else {
                            _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                        }
                    } else {
                        _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                    }
                }

                if (_authStatus != AuthStatus.Authorized) {
                    Result.rCode = _authStatus.Get_rCode();
                    Result.rDesc = _authStatus.Get_rCodeDesc();
                } else {
                    //_request.Host;
                    //_request.Path;
                    //string PathAndQuery = _request.Path.ToString() + _request.QueryString.ToString();
                    //

                    //todo: 利用開發者平台權限表，確認權限

                    //var claims = new List<Claim> {
                    //       new Claim(ClaimsIdentity.DefaultNameClaimType, apiToken.ApplicationName)};

                    //var id = new ClaimsIdentity(claims, "CustomApiKeyAuth", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                    //Context.HttpContext.User = new ClaimsPrincipal(id);
                }

                Context.Response = Context.Request.CreateResponse(HttpStatusCode.OK, Result);
            }

            base.OnActionExecuted(Context);
        }
    }
}