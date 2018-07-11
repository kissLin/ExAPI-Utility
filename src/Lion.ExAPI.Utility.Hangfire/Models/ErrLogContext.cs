using Lion.ExAPI.Utility.Hangfire.Models;
using Lion.ExAPI.Utility.Hangfire.Unit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Core.Models
{
    internal class ErrLogContext
    {
        private readonly HttpContext _context;
        private readonly string _message;
        private readonly string _stacktrace;
        private readonly string _innerException;
        private Log log = new Log();

        public ErrLogContext(HttpContext context, Exception ex)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            if(_message != null)
            {
                _message = ex.Message;
            }
            if(ex.StackTrace != null)
            {
                _stacktrace = ex.StackTrace.ToString();
            }
            if (ex.InnerException != null)
            {
                _innerException = ex.InnerException.ToString();
            }

        }

        internal ErrModel GetErrData()
        {
            ErrModel errMsg = new ErrModel();
            DateTime myDate = DateTime.Now;
            try
            {

                #region Request ID
                if (!string.IsNullOrEmpty(_context.Items[SettingHelper.Re_quest_Id].ToString()))
                {
                    errMsg.RequestId = _context.Items[SettingHelper.Re_quest_Id].ToString();
                }
                #endregion Request ID

                #region  錯誤訊息
                if (_message != null)
                {
                    errMsg.ErrMessage = _message;
                    errMsg.ErrStackTrace = _stacktrace;
                    errMsg.InnerException = _innerException;
                }

                if (_context.Items["ErrorMessage"] != null)
                {
                    errMsg.ErrMessage = _context.Items["ErrorMessage"].ToString();
                    errMsg.ErrStackTrace = _context.Items["ErrorStackTrace"].ToString();
                }

                #endregion  錯誤訊息

                #region URL路徑 && 呼叫方法
                if (_context.Request.Path.HasValue)
                {
                    errMsg.ActionData = _context.Request.Path.Value;
                }
                #endregion URL路徑 && 呼叫方法

                return errMsg;
            }
            catch (Exception ex)
            {
                log.ObjectErrorLog(ex.Message + ex.StackTrace, Directory.GetCurrentDirectory());
            }
            return errMsg;
        }
    }

    internal class HandlingLogContext
    {

        private readonly HttpContext _context;
        private ConnectModel _handlingMongo;
        private Log log = new Log();


        public HandlingLogContext(HttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }


        internal ConnectModel GetData()
        {
            string formValues = string.Empty;
            string itemValues = string.Empty;
            DateTime myDate = DateTime.Now;
            TraceModel pro = new TraceModel();
            _handlingMongo = new ConnectModel();

            try
            {
                #region Request ID
                if (!string.IsNullOrEmpty(_context.Items[SettingHelper.Re_quest_Id].ToString()))
                {
                    _handlingMongo.RequestId = _context.Items[SettingHelper.Re_quest_Id].ToString();
                }
                #endregion Request ID

                #region 專案名稱 && 產生時間 && Action結束時間
                if (!string.IsNullOrEmpty(_context.Items[SettingHelper.App_lication].ToString()))
                {
                    _handlingMongo.ApplicationName = _context.Items[SettingHelper.App_lication].ToString();
                }

                if (_context.Items[SettingHelper.Action_Timer] != null)
                {
                    _handlingMongo.DurationTime = Int32.Parse(_context.Items[SettingHelper.Action_Timer].ToString());
                }
                else
                {
                    _handlingMongo.DurationTime = 0;
                }


                #endregion 專案名稱 && 產生時間 && Action結束時間

                #region Form Resource
                if (_context.Request.HasFormContentType)
                {
                    var form = _context.Request.Form;
                    if (form != null)
                    {
                        foreach (string key in form.Keys)
                        {
                            formValues += key + ": " + form[key].ToString() + " ;";
                        }
                        if (formValues.Length > 0)
                        {
                            formValues = formValues.Substring(0, formValues.Length - 1);
                        }
                        _handlingMongo.RequestForm = System.Net.WebUtility.UrlDecode(formValues);//RequestForm
                    }
                }
                #endregion Form Resource

                #region Request 來源
                if (!string.IsNullOrEmpty(_context.Items[SettingHelper.Re_quest].ToString()))
                {
                    _handlingMongo.SourceJsonString = _context.Items[SettingHelper.Re_quest].ToString();
                }
                else
                {
                    _handlingMongo.SourceJsonString = System.Net.WebUtility.UrlDecode(_context.Request.QueryString.Value);//SourceJsonString
                }
                #endregion Request 來源

                #region Reponse 來源
                if (!string.IsNullOrWhiteSpace(_context.Items[SettingHelper.Re_ponse].ToString()))
                {
                    if (_context.Request.Path.Value.ToLower().IndexOf("/v1/") >= 0)
                    {

                        if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_context.Items[SettingHelper.Re_ponse].ToString()) > SettingHelper.Over_ReponseSizeContext)
                        {
                            _handlingMongo.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = _handlingMongo.ApplicationName + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
                            log.ReponseSizeLog(_context.Items[SettingHelper.Re_ponse].ToString(), _context.Items[SettingHelper.Re_quest_Id].ToString(), Directory.GetCurrentDirectory());
                        }
                        else
                        {
                            //20170930 Eric 避開 非V1 ResponseBase 格式 EX:Odata
                            try
                            {
                                ResponseBase myresultV1 = JsonConvert.DeserializeObject<ResponseBase>(_context.Items[SettingHelper.Re_ponse].ToString());
                                if (myresultV1.Msg == null)
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    _handlingMongo.ReturnJsonString = myresultV1;
                                }
                            }
                            catch
                            {
                                object resultV1 = _context.Items[SettingHelper.Re_ponse].ToString();
                                _handlingMongo.ReturnJsonString = resultV1;
                            }
                        }

                    }
                    else if (_context.Request.Path.Value.ToLower().IndexOf("/v2/") >= 0)
                    {
                        if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_context.Items[SettingHelper.Re_ponse].ToString()) > SettingHelper.Over_ReponseSizeContext)
                        {
                            _handlingMongo.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = _handlingMongo.ApplicationName + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
                            log.ReponseSizeLog(_context.Items[SettingHelper.Re_ponse].ToString(), _context.Items[SettingHelper.Re_quest_Id].ToString(), Directory.GetCurrentDirectory());
                        }
                        else
                        {
                            //20170930 Eric 避開 非V2 ResponseBase 格式 EX:Odata
                            try
                            {
                                Lion.ExAPI.Utility.Hangfire.Models.V2.ResponseBase resultV2 = JsonConvert.DeserializeObject<Lion.ExAPI.Utility.Hangfire.Models.V2.ResponseBase>(_context.Items[SettingHelper.Re_ponse].ToString());
                                if (resultV2.rCode == null)
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    _handlingMongo.ReturnJsonString = resultV2;
                                }
                            }
                            catch
                            {
                                object resultV2 = _context.Items[SettingHelper.Re_ponse].ToString();
                                _handlingMongo.ReturnJsonString = resultV2;
                            }


                        }
                    }
                    else
                    {
                        if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_context.Items[SettingHelper.Re_ponse].ToString()) > SettingHelper.Over_ReponseSizeContext)
                        {
                            _handlingMongo.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = _handlingMongo.ApplicationName + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + _context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
                            log.ReponseSizeLog(_context.Items[SettingHelper.Re_ponse].ToString(), _context.Items[SettingHelper.Re_quest_Id].ToString(), Directory.GetCurrentDirectory());
                        }
                        else
                        {
                            _handlingMongo.ReturnJsonString = _context.Items[SettingHelper.Re_ponse].ToString();
                        }
                    }
                }
                #endregion Reponse 來源

                #region  狀態代碼
                if (!string.IsNullOrEmpty(_context.Response.StatusCode.ToString()))
                {
                    pro.HttpCode = _context.Response.StatusCode.ToString();//HttpCode
                }
                #endregion  狀態代碼

                #region 接收資訊格訊
                if (!string.IsNullOrWhiteSpace(_context.Request.Headers["Accept"].ToString().Trim()))
                {
                    pro.AcceptFormat = _context.Request.Headers["Accept"].ToString();
                }
                #endregion 接收資訊格訊

                #region RequestCookie 來源
                string cookieValues = string.Empty;
                if (_context.User.Claims.Count<Claim>() > 0)
                {
                    var cookie = _context.User.Claims;
                    pro.Cookies = new string[_context.User.Claims.Count<Claim>()];
                    int i = 0;
                    if (cookie != null)
                    {
                        foreach (Claim claim in cookie)
                        {
                            pro.Cookies[i++] = claim.Type + ": " + System.Net.WebUtility.HtmlDecode(claim.Value);
                        }
                    }
                }
                string sessionValues = string.Empty;
                if (_context.Request.Cookies.Count > 0)
                {
                    var cookies = _context.Request.Cookies;
                    pro.Session = new string[_context.Request.Cookies.Count];
                    int i = 0;
                    if (cookies != null)
                    {
                        foreach (string key in cookies.Keys)
                        {
                            pro.Session[i++] = key + ": " + System.Net.WebUtility.HtmlDecode(cookies[key].ToString());
                        }
                    }
                }
                #endregion RequestCookie 來源

                #region MachineName
                if (!string.IsNullOrEmpty(Environment.MachineName))
                {
                    pro.MachineName = Environment.MachineName;
                }

                #endregion MachineName

                #region clinet IP
                if (_context.Connection.RemoteIpAddress != null)
                {
                    pro.RemotrIP = _context.Connection.RemoteIpAddress.ToString();//RemotrIP
                }
                #endregion clinet IP

                #region 裝置來源
                if (!string.IsNullOrEmpty(_context.Request.Headers["User-Agent"].ToString()))
                {
                    pro.Device = _context.Request.Headers["User-Agent"].ToString();//Browser
                }
                #endregion 裝置來源

                #region URL路徑 && 呼叫方法
                if (_context.Request.Path.HasValue)
                {
                    _handlingMongo.ActionData = _context.Request.Path.Value;
                    pro.MethodType = _context.Request.Method.ToString();//Method 
                }
                #endregion URL路徑 && 呼叫方法

                #region Token資訊 
                if (_context.Request.Headers != null)
                {
                    pro.Token = _context.Request.Headers["Authorization"].ToString();
                }
                #endregion  Token資訊 

                _handlingMongo.Info = pro;

                return _handlingMongo;
            }
            catch (Exception ex)
            {
                log.ObjectErrorLog(ex.Message + ex.StackTrace, Directory.GetCurrentDirectory());
            }
            return _handlingMongo;
        }

    }

    #region RequestIdentifier
    internal class RequestIdentifier : IDisposable
    {
        private readonly bool _addedFeature;
        private readonly bool _updatedIdentifier;
        private readonly string _originalIdentifierValue;
        private readonly HttpContext _context;
        private readonly IHttpRequestIdentifierFeature _feature;

        private RequestIdentifier(HttpContext context)
        {
            _context = context;
            _feature = context.Features.Get<IHttpRequestIdentifierFeature>();

            if (_feature == null)
            {
                _feature = new HttpRequestIdentifierFeature()
                {
                    TraceIdentifier = Guid.NewGuid().ToString()
                };
                context.Features.Set(_feature);
                _addedFeature = true;
            }
            else if (string.IsNullOrEmpty(_feature.TraceIdentifier))
            {
                _originalIdentifierValue = _feature.TraceIdentifier;
                _feature.TraceIdentifier = Guid.NewGuid().ToString();
                _updatedIdentifier = true;
            }
        }

        public static IDisposable Ensure(HttpContext context)
        {
            return new RequestIdentifier(context);
        }

        public void Dispose()
        {
            if (_addedFeature)
            {
                _context.Features.Set<IHttpRequestIdentifierFeature>(null);
            }
            else if (_updatedIdentifier)
            {
                _feature.TraceIdentifier = _originalIdentifierValue;
            }
        }
    }
    #endregion RequestIdentifier
}
