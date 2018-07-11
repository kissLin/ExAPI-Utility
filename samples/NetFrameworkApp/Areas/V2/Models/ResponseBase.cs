using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetFrameworkApp.Areas.V2.Models
{
    public class ResponseBase<T>
    {
        public T Data { get; set; }

        public string rDesc { get; set; }

        public string rCode { get; set; }
    }
}