using Dapper;
using Lion.ExAPI.Utility.Core.Helper;
using Lion.ExAPI.Utility.Extensions;
using Lion.ExAPI.Utility.Helper;
using Lion.ExAPI.Utility.Models;
using Lion.ExAPI.Utility.Models.V2;
using Lion.ExAPI.Utility.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Attributes {
    public class TokenAuthenticationAttribute : ActionFilterAttribute {

        private string _DBconfig;
        /// <summary>
        /// 
        /// </summary>
        public TokenAuthenticationAttribute() {
            string DBconfig = null;
            if (string.Equals(DBList._DBconfig, "L", StringComparison.OrdinalIgnoreCase)) {
                DBconfig = CongfigSetting.Token_ConnectionString_L;
            } else if (string.Equals(DBList._DBconfig, "D", StringComparison.OrdinalIgnoreCase)) {
                DBconfig = CongfigSetting.Token_ConnectionString_D;
            } else if (string.Equals(DBList._DBconfig, "T", StringComparison.OrdinalIgnoreCase)) {
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

            _apiToken = CacheHelper.GetValue(token) as TokenApplyPlace;

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
                        //CacheHelper.CacheInsert(token, _apiToken, _apiToken.ExpireDateTime);
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
                                CacheHelper.CacheInsert(token, apiToken, apiToken.ExpireDateTime);
                            }
                        }
                    }
                }
                return _apiToken.ExpireDateTime.ToString("yyyy-MM-ddTHH:mm:ssK");
            }
            return null;
        }




        #endregion

        public override void OnActionExecuting(ActionExecutingContext Context) {

            var _request = Context.HttpContext.Request;
            string ipAppress = Context.HttpContext.Connection.RemoteIpAddress.ToString();

            string AuthorizationHeader = _request.Headers["Authorization"].ToString().Trim();

            //版本不對
            if (Context.HttpContext.Request.Path.Value.IndexOf("/V2/", StringComparison.OrdinalIgnoreCase) >= 0) {
                //沒有加入token
                if (string.IsNullOrWhiteSpace(AuthorizationHeader)) {
                    _authStatus = AuthStatus.Revoked_AuthorizationNotExists;
                }

                //token 格式不對
                else if (!(AuthorizationHeader.StartsWith("basic" + _delimiter, StringComparison.OrdinalIgnoreCase))) {
                    _authStatus = AuthStatus.Revoked_AuthorizationFormatIncorrect;
                }

                //取得token驗證
                else {
                    _token = AuthorizationHeader.Split(_delimiter)[1].Trim();
                    _authStatus = TokenAuthentication(_token, ipAppress);
                }

            } else {
                _authStatus = AuthStatus.Revoked_ErrorUrlPath;
            }

            if (_authStatus != AuthStatus.Authorized) {
                Context.Result = new JsonResult(new ResponseBase<String>() {
                    rCode = _authStatus.Get_rCode(),
                    rDesc = _authStatus.Get_rCodeDesc()
                });
            }

            base.OnActionExecuting(Context);



        }


        public override void OnResultExecuting(ResultExecutingContext Context) {


            // update token expire time
            if (AutoTokenExtended && !String.IsNullOrWhiteSpace(_token) && _authStatus == AuthStatus.Authorized) {
                //沒有回傳值
                if (Context.Result != null) {
                    var jsonResult = Context.Result as ObjectResult;
                    if (jsonResult != null) {
                        dynamic oldResult = jsonResult.Value as ResponseBase;

                        //回傳格式不對 ResponseBase
                        if (oldResult == null) {
                            var jsonStr = JsonConvert.SerializeObject(jsonResult.Value);

                            //回傳格式不對 ResponseBase<Object>
                            if (!UnitHelper.checkApiFormat(jsonStr)) {
                                _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                            } else {
                                oldResult = JsonConvert.DeserializeObject<Lion.ExAPI.Utility.Models.V2.ResponseBase<Object>>(jsonStr);
                                if (oldResult != null) {
                                    oldResult.TokenExpires = Convert.ToDateTime(AuthExtendTime(_apiToken, _token, _tokenExtendedTime, _authStatus));
                                    Context.Result = new JsonResult(oldResult);
                                } else {
                                    _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                                }
                            }
                        } else {
                            oldResult.TokenExpires = Convert.ToDateTime(AuthExtendTime(_apiToken, _token, _tokenExtendedTime, _authStatus));
                            Context.Result = new JsonResult(oldResult);
                        }

                    }
                } else {
                    _authStatus = AuthStatus.Rvoked_ErrorReponseBase;
                }
            }


            //判斷回傳格式
            if (_authStatus != AuthStatus.Authorized) {
                Context.Result = new JsonResult(new ResponseBase<String>() {
                    rCode = _authStatus.Get_rCode(),
                    rDesc = _authStatus.Get_rCodeDesc()
                });
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



            base.OnResultExecuting(Context);



        }


    }
}
