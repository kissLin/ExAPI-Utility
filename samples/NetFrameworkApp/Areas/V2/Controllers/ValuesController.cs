using Lion.ExAPI.Utility.Hangfire.Core;
using Lion.ExAPI.Utility.Hangfire.Core.Serivce;
using NetFrameworkApp.EnumSetting;
using NetFrameworkApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace NetFrameworkApp.Areas.V2.Controllers
{
    public class ValuesController : ApiController
    {
        private MiddlewareController _myLog;

        public ValuesController()
        {
            _myLog = new MiddlewareController();
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            //await postLog("test");
            return new string[] { "value1", "value2" };
        }
        private async Task postLog(string text)
        {
            var url = "http://10.30.3.149:8098/";
            var client = new HttpClient()
            {
                BaseAddress = new Uri(url + "api/v2/Values"),
                //Timeout = TimeSpan.FromMilliseconds(100)
            };
            try
            {
                var data = await client.PostAsync(url + "api/v2/Values", new StringContent(JsonConvert.SerializeObject(text).ToString(), Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                //mylog.HangfireErrorLog(e.Message, HttpContext.Current.Server.MapPath("~/Log/Temp"));
            }
            finally
            {
                client.Dispose();
            }
        }
        // GET: api/Values
        //[NetTokenAuthentication]
        //public ResponseBase Get()
        //{
        //    //預設: 回傳的結果
        //    ResponseBase<AuthWithOTPResult> RespBaseReturn = new ResponseBase<AuthWithOTPResult>()
        //    {
        //        Data = new AuthWithOTPResult()
        //        {
        //            requestId = null,
        //            isVerified = false,
        //            authResultCode = AuthenticationOTPEnum.None,
        //            authResultMsg = String.Empty
        //        },
        //        rCode = rCodeStatusEnum.none.Get_rCode(),
        //        rDesc = rCodeStatusEnum.none.Get_rCodeDesc()
        //    };

        //    //Trace Log 
        //    RespBaseReturn.Data.requestId = HttpContext.Current.Items[SettingHelper.Re_quest_Id].ToString();
        //    RespBaseReturn.Data.isVerified = true;
        //    RespBaseReturn.Data.authResultCode = AuthenticationOTPEnum.Success;
        //    RespBaseReturn.Data.authResultMsg = AuthenticationOTPEnum.Success.Get_rCodeDesc();
        //    _myLog.ClientLog(JsonConvert.SerializeObject(RespBaseReturn), HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name);


        //    return new ResponseBase { };
        //}

        // GET: api/Values/5
        //[APIDeveloperNetTokenAuthentication]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Values
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Values/5
        public void Delete(int id)
        {
        }
    }
}
