using Lion.ExAPI.Utility.Hangfire.Core;
using Lion.ExAPI.Utility.Hangfire.Models;
using Lion.ExAPI.Utility.Hangfire.Unit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace Lion.ExAPI.Utility.Hangfire.Models {
    internal class NetHandlingLogContext {

        //public ConnectModel conModel = new ConnectModel();
        //public ErrModel errModel = new ErrModel();
        //public TraceModel traceModel = new TraceModel();

        public ConnectModel conModel;
        public ErrModel errModel;
        public TraceModel traceModel;
        private HttpContext _Context;
        private HttpActionExecutedContext _filterContext;
        private Log mylog;


        public NetHandlingLogContext(HttpActionExecutedContext filterContext, HttpContext Context) {

            conModel = new ConnectModel();
            errModel = new ErrModel();
            traceModel = new TraceModel();
            mylog = new Log();

            conModel.RequestId = HttpContext.Current.Items[SettingHelper.Re_quest_Id].ToString();
            conModel.ApplicationName = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
            conModel.DurationTime = Int32.Parse(HttpContext.Current.Items[SettingHelper.Action_Timer].ToString());
            if (Context.Request != null)
            {
                if (Context.Request != null && Context.Request.AcceptTypes != null)
                {
                    foreach (var d in Context.Request.AcceptTypes)
                    {
                        traceModel.AcceptFormat += d.ToString() + "; ";
                    }
                }

                traceModel.MethodType = Context.Request.RequestType?.ToString();
                if (traceModel.MethodType == "GET")
                {
                    conModel.SourceJsonString = Context.Request.Url.Query;
                }
                else
                {
                    string rawRequest;
                    using (var stream = new StreamReader(filterContext.Request.Content.ReadAsStreamAsync().Result))
                    {
                        stream.BaseStream.Position = 0;
                        rawRequest = stream.ReadToEnd();
                        conModel.SourceJsonString = rawRequest;
                    }
                }

                traceModel.MachineName = Environment.MachineName;

                traceModel.RemotrIP  = Context.Request.UserHostAddress?.ToString();

                conModel.ActionData = Context.Request.Url?.LocalPath.ToString();

                traceModel.Device = Context.Request.UserAgent?.ToString();

                traceModel.Token = filterContext.Request.Headers.Authorization?.ToString();
            }

            if (filterContext.Exception != null)
            {
                errModel.RequestId = conModel.RequestId;
                errModel.ActionData = Context.Request.Url?.LocalPath.ToString();
                errModel.ErrMessage = filterContext.Exception.Message.ToString();
                errModel.ErrStackTrace = filterContext.Exception.StackTrace.ToString();
                if (filterContext.Exception.InnerException != null)
                {
                    errModel.InnerException = filterContext.Exception.InnerException.ToString();
                }
            }

            // reponse
            traceModel.HttpCode = filterContext.Response.StatusCode;
            var content = filterContext.Response.Content;
            var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
            if (bytes != null)
            {
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(System.Text.Encoding.UTF8.GetString(bytes).ToString()) > SettingHelper.Over_ReponseSizeContext)
                {
                    conModel.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = conModel.ApplicationName + "\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
                    mylog.ReponseSizeLog(System.Text.Encoding.UTF8.GetString(bytes).ToString(), Context.Items[SettingHelper.Re_quest_Id].ToString(), HttpContext.Current.Server.MapPath("~/Log/Temp"));
                }
                else
                {
                    conModel.ReturnJsonString = System.Text.Encoding.UTF8.GetString(bytes).ToString();
                }
            }

            // reponse
            //if (Context.Response != null)
            //{
            //    traceModel.HttpCode = filterContext.Response.StatusCode;
            //    if (filterContext.Response != null && filterContext.Response.Content != null)
            //    {
            //        if (checkContext(filterContext))
            //        {
            //            var bytes = filterContext.Response.Content.ReadAsByteArrayAsync().Result;
            //            if (System.Text.ASCIIEncoding.ASCII.GetByteCount(System.Text.Encoding.UTF8.GetString(bytes).ToString()) > SettingHelper.Over_ReponseSizeContext)
            //            {
            //                conModel.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = conModel.ApplicationName + "\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
            //                mylog.ReponseSizeLog(System.Text.Encoding.UTF8.GetString(bytes).ToString(), Context.Items[SettingHelper.Re_quest_Id].ToString(), HttpContext.Current.Server.MapPath("~/Log/Temp"));
            //            }
            //            else
            //            {
            //                conModel.ReturnJsonString = System.Text.Encoding.UTF8.GetString(bytes).ToString();
            //            }
            //        }
            //    }
            //}


            //if (Context.Response != null)
            //{
            //    traceModel.HttpCode = filterContext.Response.StatusCode;
            //    if (filterContext.Response != null && filterContext.Response.Content != null)
            //    {
            //        if (checkContext(filterContext))
            //        {
            //            var bytes = filterContext.Response.Content.ReadAsByteArrayAsync().Result;
            //            if (filterContext.Request.RequestUri.ToString().ToLower().Contains("/v2/"))
            //            {
            //                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(System.Text.Encoding.UTF8.GetString(bytes).ToString()) > SettingHelper.Over_ReponseSizeContext)
            //                {
            //                    conModel.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = conModel.ApplicationName + "\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
            //                    mylog.ReponseSizeLog(System.Text.Encoding.UTF8.GetString(bytes).ToString(), Context.Items[SettingHelper.Re_quest_Id].ToString(), HttpContext.Current.Server.MapPath("~/Log/Temp"));
            //                }
            //                else
            //                {
            //                    //20170930 Eric 避開 非V2 ResponseBase 格式 EX:Odata
            //                    try
            //                    {
            //                        Models.V2.ResponseBase myresultV2 = JsonConvert.DeserializeObject<Models.V2.ResponseBase>(System.Text.Encoding.UTF8.GetString(bytes).ToString());
            //                        if (myresultV2.rCode == null && myresultV2.Data == null && myresultV2.rDesc == null)
            //                        {
            //                            throw new Exception();
            //                        }
            //                        else
            //                        {
            //                            conModel.ReturnJsonString = myresultV2;
            //                        }
            //                    }
            //                    catch
            //                    {
            //                        object resultV2 = System.Text.Encoding.UTF8.GetString(bytes).ToString();
            //                        conModel.ReturnJsonString = resultV2;
            //                    }
            //                }

            //            }
            //            else if (filterContext.Request.RequestUri.ToString().ToLower().Contains("/v1/"))
            //            {
            //                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(System.Text.Encoding.UTF8.GetString(bytes).ToString()) > SettingHelper.Over_ReponseSizeContext)
            //                {
            //                    conModel.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = conModel.ApplicationName + "\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
            //                    mylog.ReponseSizeLog(System.Text.Encoding.UTF8.GetString(bytes).ToString(), Context.Items[SettingHelper.Re_quest_Id].ToString(), HttpContext.Current.Server.MapPath("~/Log/Temp"));
            //                }
            //                else
            //                {
            //                    //20170930 Eric 避開 非V1 ResponseBase 格式 EX:Odata
            //                    try
            //                    {
            //                        ResponseBase myresultV1 = JsonConvert.DeserializeObject<ResponseBase>(System.Text.Encoding.UTF8.GetString(bytes).ToString());
            //                        if (myresultV1.Msg == null && myresultV1.Data == null)
            //                        {
            //                            throw new Exception();
            //                        }
            //                        else
            //                        {
            //                            conModel.ReturnJsonString = myresultV1;
            //                        }
            //                        conModel.ReturnJsonString = myresultV1;
            //                    }
            //                    catch
            //                    {
            //                        object resultV1 = System.Text.Encoding.UTF8.GetString(bytes).ToString();
            //                        conModel.ReturnJsonString = resultV1;
            //                    }
            //                }

            //            }
            //            else
            //            {
            //                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(System.Text.Encoding.UTF8.GetString(bytes).ToString()) > SettingHelper.Over_ReponseSizeContext)
            //                {
            //                    conModel.ReturnJsonString = new ReponseSizeLog { Dec = "Size too Big ", root = conModel.ApplicationName + "\\Log\\Temp\\ReponseSizeLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString().Substring(0, 2) + "\\" + Context.Items[SettingHelper.Re_quest_Id].ToString() + ".json" };
            //                    mylog.ReponseSizeLog(System.Text.Encoding.UTF8.GetString(bytes).ToString(), Context.Items[SettingHelper.Re_quest_Id].ToString(), HttpContext.Current.Server.MapPath("~/Log/Temp"));
            //                }
            //                else
            //                {
            //                    conModel.ReturnJsonString = System.Text.Encoding.UTF8.GetString(bytes).ToString();
            //                }
            //            }
            //        }
            //    }
            //}


            //if (HttpContext.Current.Items[SettingHelper.ErrorMessage] != null) {
            //    errModel.RequestId = conModel.RequestId;
            //    errModel.ActionData = Context.Request.Url?.LocalPath.ToString();
            //    errModel.ErrMessage = HttpContext.Current.Items[SettingHelper.ErrorMessage].ToString();
            //    errModel.ErrStackTrace = HttpContext.Current.Items[SettingHelper.ErrorStackTrace].ToString();
            //}


            conModel.Info = traceModel;
            _Context = Context;
            _filterContext = filterContext;
        }

        public ConnectModel returnconModel() {
            return conModel;
        }

        public ErrModel returnerrModel() {
            return errModel;
        }
    }
}
