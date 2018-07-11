using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lion.ExAPI.Utility.Hangfire.Core.Serivce;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreApp.Controllers
{
    [Route("api/v2/[controller]")]//API需要更改
    public class ValuesController : Controller
    {
        //set log
        private MiddlewareController _myLog;
        private IHostingEnvironment _env;

        public ValuesController(IHostingEnvironment env)
        {
            _myLog = new MiddlewareController();
            _env = env;
        }

        [HttpGet]
        public string Get()
        {
            //var i = 0;
            //var data = 1 / i;
            return "test";
        }

        //[HttpGet]
        //[TokenAuthentication]//api Token 驗證
        //public ResponseBase<string> Get()
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
        //    //RespBaseReturn.Data.requestId = HttpContext.Items[SettingHelper.Re_quest_Id].ToString();
        //    //RespBaseReturn.Data.isVerified = true;
        //    //RespBaseReturn.Data.authResultCode = AuthenticationOTPEnum.Success;
        //    //RespBaseReturn.Data.authResultMsg = AuthenticationOTPEnum.Success.Get_rCodeDesc();
        //    //_myLog.ClientLog(JsonConvert.SerializeObject(RespBaseReturn), _env.ApplicationName);

        //    return new ResponseBase<string>();
        //}


        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
