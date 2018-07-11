using Lion.ExAPI.Utility.Extensions;
using Lion.ExAPI.Utility.Models.V2;
using Lion.ExAPI.Utility.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Attributes {
    public class OnlyIntranetAttribute : ActionFilterAttribute {


        public override void OnActionExecuting(ActionExecutingContext Context) {

            var ipAppress = Context.HttpContext.Connection.RemoteIpAddress;

            if (!ipAppress.IsIntraNet()) {

                rCodeStatusEnum status = rCodeStatusEnum.rCode_9900;

                Context.Result = new JsonResult(new ResponseBase<String>() {
                    rCode = status.Get_rCode(),
                    rDesc = status.Get_rCodeDesc(),
                    Data = "your ip : " + ipAppress.ToString()
                });

            }


            base.OnActionExecuting(Context);
        }
    }
}
