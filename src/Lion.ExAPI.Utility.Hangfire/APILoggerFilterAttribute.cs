using Lion.ExAPI.Utility.Hangfire.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Diagnostics;
using Lion.ExAPI.Utility.Hangfire.Core;
using System.Configuration;
using Lion.ExAPI.Utility.Hangfire.Unit;
using System.Net.Http.Headers;
using System.Threading;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Web.Hosting;
using System.IO;
using System.IO.Compression;

namespace Lion.ExAPI.Utility.Hangfire {
    public class APILoggerFilterAttribute : ActionFilterAttribute {
        private static RequestReponseModels Models;
        private static Log mylog ;
        private static readonly HttpClient Client;
        //private static WebClient webClient;

        static APILoggerFilterAttribute()
        {
            var handler = new HttpClientHandler() {AutomaticDecompression = DecompressionMethods.GZip };
            Client = new HttpClient(handler);
            Client.Timeout = TimeSpan.FromMilliseconds(100);
            Client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            mylog = new Log();
            Models = new RequestReponseModels();
        }



        public override void OnActionExecuting(HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);
            actionContext.Request.Properties[actionContext.ActionDescriptor.ActionName] = Stopwatch.StartNew();
            HttpContext.Current.Items[SettingHelper.Re_quest_Id] = Guid.NewGuid().ToString();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            rCodeStatusEnumLog rCode0005 = rCodeStatusEnumLog.rCode_0005;

            try {
               base.OnActionExecuted(actionExecutedContext);

                if (actionExecutedContext.Exception != null) {
                    throw new Exception();
                }
            }
            catch  {

                if (actionExecutedContext.Request.RequestUri.ToString().ToLower().Contains("/v2/")) {
                    Models.V2.ResponseBase<string> rError = new Models.V2.ResponseBase<string>() {
                        Data = string.Empty,
                        rCode = rCode0005.Get_rCode(),
                        rDesc = rCode0005.Get_rCodeDesc()
                    };
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(actionExecutedContext.Response.StatusCode, rError);
                } else if (actionExecutedContext.Request.RequestUri.ToString().ToLower().Contains("/v1/")) {
                    ResponseBase<string> rError = new ResponseBase<string>() {
                        Data = null,
                        IsSuccess = false,
                        Msg = "API System error"
                    };
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(actionExecutedContext.Response.StatusCode, rError);
                } else {
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(actionExecutedContext.Response.StatusCode, "系統錯誤，請通知api人員謝謝!!");
                }

            }
            finally {
                //計算時間
                Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[actionExecutedContext.ActionContext.ActionDescriptor.ActionName];

                HttpContext.Current.Items[SettingHelper.Action_Timer] = stopwatch.ElapsedMilliseconds;
                NetHandlingLogContext data = new NetHandlingLogContext(actionExecutedContext, HttpContext.Current);
                Models.connectModel = data.returnconModel();
                Models.errModel = data.returnerrModel();
                //postLoghttpWebRequest(Models);
                postLogWebClient(Models);

                //postLog(Models).ConfigureAwait(false);


                //Start(Models);


                //Task.Run(async () =>
                //{
                //    await postLog(Models).ConfigureAwait(false);

                //}).ConfigureAwait(false);




                //Task.Run(() =>
                //{
                //    HttpRequestAsync(Models);

                //}).ConfigureAwait(false);

            }
        }


        //private static void HttpRequestAsync(RequestReponseModels models)
        //{
        //    using (var socket = new TcpClient("10.30.3.149", 8099))
        //    {
        //        var headerContent = new StringBuilder();
        //        headerContent.AppendLine("POST /api/Log/LogConnectErrorAsync HTTP/1.0");
        //        headerContent.AppendLine("Accept: */*");
        //        headerContent.AppendLine("Host: " + "10.30.3.149");
        //        headerContent.AppendLine("Content-Type: application/javascript; charset=utf8");
        //        headerContent.AppendLine("Content-Length: " + JsonConvert.SerializeObject(models).Length);
        //        headerContent.AppendLine("Connection: Close");
        //        headerContent.AppendLine();

        //        var header = Encoding.UTF8.GetBytes(headerContent.ToString());
        //        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(models).ToString());

        //        using (var stream = socket.GetStream())
        //        {
        //            stream.Write(header, 0, header.Length);
        //            stream.Write(body, 0, body.Length);
        //        }
        //    }
        //}


        //非同步

        //private void Start(RequestReponseModels models)
        //{
        //    //建立一個執行緒，並且傳入一個委派物件 ParameterizedThreadStart，'
        //    //並且設定指向 PrintOddNumber 方法。               
        //    Thread oThreadA = new Thread(new ParameterizedThreadStart(postLog));

        //    //設定執行緒的 Name
        //    oThreadA.Name = "HangfireLog";
        //    oThreadA.IsBackground = true;
        //    //啟動執行緒物件，並且傳入參數
        //    oThreadA.Start(models);
        //}

        //private async void postLog(object value)
        //{
        //    var url = System.Configuration.ConfigurationManager.AppSettings["LogAddressUrl"];
        //    var client = new HttpClient()
        //    {
        //        BaseAddress = new Uri(url + "api/Log/LogConnectErrorAsync"),
        //        Timeout = TimeSpan.FromMilliseconds(100)
        //    };
        //    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        //    client.DefaultRequestHeaders.Add("Keep-Alive", "600");
        //    try
        //    {
        //        await client.PostAsync(url + "api/Log/LogConnectErrorAsync", new StringContent(JsonConvert.SerializeObject(value).ToString(), Encoding.UTF8, "application/json")).ConfigureAwait(false);
        //    }
        //    catch (Exception e)
        //    {
        //        mylog.HangfireErrorLog(e.Message, HttpContext.Current.Server.MapPath("~/Log/Temp"));
        //    }
        //    finally
        //    {
        //        client.Dispose();
        //    }
        //}

        //Task 非同步
        private static async Task postLog(RequestReponseModels models)
        {

            var url = System.Configuration.ConfigurationManager.AppSettings["LogAddressUrl"];
            try
            {
                await Client.PostAsync(url + "api/Log/LogConnectErrorAsync", new StringContent(JsonConvert.SerializeObject(models).ToString(), Encoding.UTF8, "application/json")).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                mylog.HangfireErrorLog(e.Message, HttpContext.Current.Server.MapPath("~/Log/Temp"));
            }
        }


        private static void postLogWebClient(RequestReponseModels models)
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["LogAddressUrl"];
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    //webClient.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string json = JsonConvert.SerializeObject(models);
                    webClient.UploadString(url + "api/Log/LogConnectErrorAsync", json);
                }
            }
            catch (Exception e)
            {
                mylog.HangfireErrorLog(e.Message, HttpContext.Current.Server.MapPath("~/Log/Temp"));
            }
        }


        private static void postLoghttpWebRequest(RequestReponseModels models)
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["LogAddressUrl"];
            string postData = JsonConvert.SerializeObject(models);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url+ "api/Log/LogConnectErrorAsync");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = "application/json";
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Count());
            }
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                string message = String.Format("POST failed. Received HTTP {0}", httpWebResponse.StatusCode);
                throw new ApplicationException(message);
            }
        }




        //壓縮


        //private async Task postLog(RequestReponseModels models)
        //{

        //    using (HttpClientHandler handler = new HttpClientHandler())
        //    {
        //        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        //        using (HttpClient client = new HttpClient(handler, false))
        //        {
        //            string json = JsonConvert.SerializeObject(models);
        //            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        //            MemoryStream ms = new MemoryStream();
        //            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
        //            {
        //                gzip.Write(jsonBytes, 0, jsonBytes.Length);
        //            }
        //            ms.Position = 0;
        //            StreamContent content = new StreamContent(ms);
        //            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //            content.Headers.ContentEncoding.Add("gzip");
        //            await client.PostAsync("http://localhost:53763/api/Log/LogConnectErrorAsync", content);
        //        }
        //    }
        //}
    }

    public class GZipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}
